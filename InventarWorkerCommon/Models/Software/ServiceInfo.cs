namespace InventarWorkerCommon.Models.Software;

/// <summary>
/// Represents information about a Windows service or service-like component.
/// </summary>
public record ServiceInfo
{
    /// <summary>
    /// Gets or sets the internal service name.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user-friendly display name of the service.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status/state of the service (for example, Running, Stopped).
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the configured start type (for example, Automatic, Manual, Disabled).
    /// </summary>
    public string StartType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the account under which the service runs.
    /// </summary>
    public string ServiceAccount { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full path to the service executable or command.
    /// </summary>
    public string BinaryPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a textual description of the service.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of other services this service depends on.
    /// </summary>
    public List<string> Dependencies { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the service supports Stop.
    /// </summary>
    public bool CanStop { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the service supports Pause and Continue.
    /// </summary>
    public bool CanPauseAndContinue { get; set; }
}
