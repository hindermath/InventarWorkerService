namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Captures a snapshot of performance metrics such as CPU and memory usage.
/// </summary>
public record PerformanceMetrics
{
    public double CpuUsage { get; init; }
    public long MemoryUsage { get; init; }
    public int ThreadCount { get; init; }
    public DateTime Timestamp { get; init; }
}
