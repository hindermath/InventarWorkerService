namespace InventarWorkerCommon.Models.Service;

/// <summary>
/// Represents system-related information, including details about the machine, user, domain,
/// uptime, platform, and architecture.
/// </summary>
public record SystemInfo
{
    public string MachineName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public TimeSpan Uptime { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
}
