using Kirov_DA_test.Models;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Kirov_DA_test.Services
{
    public class ScanBackgroundService : BackgroundService
    {
        private readonly IScanTaskStore _taskStore;
        private readonly ILogger<ScanBackgroundService> _logger;

        private readonly Channel<ScanTask> _channel = Channel.CreateUnbounded<ScanTask>();

        private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _tokens = new();

        private readonly SemaphoreSlim _semaphore = new(2, 2);

        public ScanBackgroundService(IScanTaskStore taskStore, ILogger<ScanBackgroundService> logger)
        {
            _taskStore = taskStore;
            _logger = logger;
        }

        public async Task<Guid> EnqueueAsync(ScanTask task, CancellationTokenSource cts)
        {
            _tokens[task.Id] = cts;
            await _channel.Writer.WriteAsync(task);
            return task.Id;
        }

        public bool TryGetToken(Guid id, out CancellationTokenSource? cts)
        {
            return _tokens.TryGetValue(id, out cts);
        }

        public void RemoveToken(Guid id)
        {
            _tokens.TryRemove(id, out _);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Фоновый сервис сканирования запущен.");

            await foreach (var task in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                if (_tokens.TryGetValue(task.Id, out var cts) && cts.IsCancellationRequested)
                {
                    task.Status = "Cancelled";
                    task.LastUpdated = DateTime.UtcNow;
                    await _taskStore.UpdateAsync(task);
                    _logger.LogInformation("Задача {TaskId} отменена до начала выполнения.", task.Id);
                    continue;
                }

                _ = ProcessTaskAsync(task, stoppingToken);
            }
        }

        private async Task ProcessTaskAsync(ScanTask task, CancellationToken stoppingToken)
        {
            await _semaphore.WaitAsync(stoppingToken);

            try
            {
                var token = _tokens[task.Id].Token;

                token.ThrowIfCancellationRequested();

                task.Status = "Running";
                task.LastUpdated = DateTime.UtcNow;
                await _taskStore.UpdateAsync(task);
                _logger.LogInformation("Начато сканирование задачи {TaskId} для {Url}", task.Id, task.RepositoryUrl);

                await Task.Delay(60000, token);

                task.Status = "Completed";
                task.Result = $"Найдено 3 уязвимости в репозитории {task.RepositoryUrl}";
                task.LastUpdated = DateTime.UtcNow;
                await _taskStore.UpdateAsync(task);
                _logger.LogInformation("Сканирование задачи {TaskId} завершено успешно.", task.Id);
            }
            catch (OperationCanceledException)
            {
                task.Status = "Cancelled";
                task.LastUpdated = DateTime.UtcNow;
                await _taskStore.UpdateAsync(task);
                _logger.LogInformation("Сканирование задачи {TaskId} было отменено.", task.Id);
            }
            catch (Exception ex)
            {
                task.Status = "Failed";
                task.Result = ex.Message;
                task.LastUpdated = DateTime.UtcNow;
                await _taskStore.UpdateAsync(task);
                _logger.LogError(ex, "Ошибка при сканировании задачи {TaskId}", task.Id);
            }
            finally
            {
                _semaphore.Release();
                RemoveToken(task.Id);
            }
        }
    }
}
