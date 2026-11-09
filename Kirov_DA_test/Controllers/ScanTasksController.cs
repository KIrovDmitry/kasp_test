using Kirov_DA_test.Models;
using Kirov_DA_test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kirov_DA_test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScanTasksController : ControllerBase
    {
        private readonly IScanTaskStore _taskStore;
        private readonly ScanBackgroundService _scanService;

        public ScanTasksController(IScanTaskStore taskStore, ScanBackgroundService scanService)
        {
            _taskStore = taskStore;
            _scanService = scanService;
        }

        [HttpPost]
        public async Task<IActionResult> StartScan([FromBody] ScanTask task)
        {
            task.Id = Guid.NewGuid();
            task.Status = "Queued";
            task.LastUpdated = DateTime.UtcNow;

            await _taskStore.AddAsync(task);

            var cts = new CancellationTokenSource();
            await _scanService.EnqueueAsync(task, cts);

            return AcceptedAtAction(nameof(GetScanStatus), new { id = task.Id }, task);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScanStatus(Guid id)
        {
            var task = await _taskStore.GetByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public IActionResult CancelScan(Guid id)
        {
            if (_scanService.TryGetToken(id, out var cts))
            {
                var cancelledAt = DateTime.UtcNow;
                cts.Cancel();

                return Ok(new
                {
                    TaskId = id,
                    IsSuccess = true,
                    CancelledAt = cancelledAt
                });
            }

            return NotFound(new { TaskId = id, IsSuccess = false, Message = "Задача не найдена или уже завершена" });
        }
    }
}