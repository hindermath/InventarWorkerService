namespace InventarWorkerCommon.Models.Service;

/// <summary>
/// Represents statistical information related to the service's operations.
/// </summary>
public record ServiceStatistics
{
    public int TotalProcessedItems { get; init; }
    public double AverageProcessingTime { get; init; }
    public TimeSpan Uptime { get; init; }
    public long MemoryUsage { get; init; }
}
