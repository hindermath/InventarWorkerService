namespace InventarWorkerCommon.Models.Service;

public record SystemInfo
{
    public string MachineName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public TimeSpan Uptime { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
}
