namespace Kirov_DA_test.Services
{
    public interface IScanQueueService
    {
        Task<Guid> EnqueueScanAsync(string repositoryUrl);
        Task<(bool IsSuccess, DateTime CancelledAt)> CancelScanAsync(Guid taskId);
    }
}
