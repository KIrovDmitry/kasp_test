namespace Kirov_DA_test.Models
{
    public class ScanTask
    {
        public Guid Id { get; set; }
        public string RepositoryUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public string? Result { get; set; }
    }
}
