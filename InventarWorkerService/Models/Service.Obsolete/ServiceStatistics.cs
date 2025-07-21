namespace InventarWorkerService.Models.Service;

public record ServiceStatistics
{
    public int TotalProcessedItems { get; init; }
    public double AverageProcessingTime { get; init; }
    public TimeSpan Uptime { get; init; }
    public long MemoryUsage { get; init; }
}
