using Kirov_DA_test.Models;

namespace Kirov_DA_test.Services
{
    public interface IScanTaskStore
    {
        Task<ScanTask> AddAsync(ScanTask task);
        Task<ScanTask?> GetByIdAsync(Guid id);
        Task UpdateAsync(ScanTask task);
    }
}
