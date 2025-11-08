namespace InventarWorkerCommon.Models.Service;

/// <summary>
/// Represents system-related information, including details about the machine, user, domain,
/// uptime, platform, and architecture.
/// </summary>
public record SystemInfo
{
    /// <summary>
    /// Gets or sets the machine name (host name) of the current system.
    /// </summary>
    public string MachineName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the currently logged-in user.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the domain of the current machine or user, if available.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration for which the system has been running.
    /// </summary>
    public TimeSpan Uptime { get; set; }

    /// <summary>
    /// Gets or sets the platform identifier (for example, Windows, Linux, macOS).
    /// </summary>
    public string Platform { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the system architecture (for example, x64, arm64).
    /// </summary>
    public string Architecture { get; set; } = string.Empty;
}
