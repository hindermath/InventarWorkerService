namespace InventarWorkerService.Models;

public record PerformanceMetrics
{
    public double CpuUsage { get; init; }
    public long MemoryUsage { get; init; }
    public int ThreadCount { get; init; }
    public DateTime Timestamp { get; init; }
}
