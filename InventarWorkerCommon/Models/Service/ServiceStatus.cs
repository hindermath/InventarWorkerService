namespace InventarWorkerCommon.Models.Service;

/// <summary>
/// Represents the current status of a service.
/// </summary>
public record ServiceStatus
{
    /// <summary>
    /// Gets the current state of the service (for example, Running, Stopped, Error).
    /// </summary>
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// Gets the time when the service was started.
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// Gets the time of the last recorded activity by the service.
    /// </summary>
    public DateTime LastActivity { get; init; }

    /// <summary>
    /// Gets the number of items processed by the service in the current run.
    /// </summary>
    public int ProcessedItems { get; init; }

    /// <summary>
    /// Gets the last error message logged by the service, if any.
    /// </summary>
    public string LastError { get; init; } = string.Empty;
}
