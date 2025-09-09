namespace InventarWorkerCommon.Models.Service;

/// <summary>
/// Represents the current status of a service.
/// </summary>
public record ServiceStatus
{
    public string State { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime LastActivity { get; init; }
    public int ProcessedItems { get; init; }
    public string LastError { get; init; } = string.Empty;
}
