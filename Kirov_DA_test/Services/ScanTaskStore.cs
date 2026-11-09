using Kirov_DA_test.Models;
using System.Collections.Concurrent;

namespace Kirov_DA_test.Services
{
    public class ScanTaskStore : IScanTaskStore
    {
        private readonly ConcurrentDictionary<Guid, ScanTask> _store = new ConcurrentDictionary<Guid, ScanTask>();

        public Task<ScanTask> AddAsync(ScanTask task)
        {
            _store.TryAdd(task.Id, task);
            return Task.FromResult(task);
        }

        public Task<ScanTask?> GetByIdAsync(Guid id)
        {
            _store.TryGetValue(id, out var task);
            if (task == null) { 
                return Task.FromResult<ScanTask?>(null); 
            }
            else { 
                return Task.FromResult(task); 
            }
        }

        public Task UpdateAsync(ScanTask task)
        {
            _store[task.Id] = task;
            return Task.CompletedTask;
        }
    }
}
