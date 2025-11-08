namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Captures a snapshot of performance metrics such as CPU and memory usage.
/// </summary>
public record PerformanceMetrics
{
    /// <summary>
    /// Gets the current overall CPU usage in percent (0–100).
    /// </summary>
    public double CpuUsage { get; init; }

    /// <summary>
    /// Gets the current process memory usage in bytes.
    /// </summary>
    public long MemoryUsage { get; init; }

    /// <summary>
    /// Gets the current number of threads in the process.
    /// </summary>
    public int ThreadCount { get; init; }

    /// <summary>
    /// Gets the timestamp when this snapshot was taken.
    /// </summary>
    public DateTime Timestamp { get; init; }
}
