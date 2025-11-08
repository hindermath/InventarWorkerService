namespace InventarWorkerCommon.Models.Service;

/// <summary>
/// Represents statistical information related to the service's operations.
/// </summary>
public record ServiceStatistics
{
    /// <summary>
    /// Gets the total count of items processed by the service during its current lifetime.
    /// </summary>
    public int TotalProcessedItems { get; init; }

    /// <summary>
    /// Gets the average processing time per item in milliseconds.
    /// </summary>
    public double AverageProcessingTime { get; init; }

    /// <summary>
    /// Gets the duration for which the service has been running.
    /// </summary>
    public TimeSpan Uptime { get; init; }

    /// <summary>
    /// Gets the current memory usage of the service process in bytes.
    /// </summary>
    public long MemoryUsage { get; init; }
}
