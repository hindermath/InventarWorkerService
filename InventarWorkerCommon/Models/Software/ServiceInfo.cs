namespace InventarWorkerCommon.Models.Software;

public record ServiceInfo
{
    public string ServiceName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StartType { get; set; } = string.Empty;
    public string ServiceAccount { get; set; } = string.Empty;
    public string BinaryPath { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Dependencies { get; set; } = new();
    public bool CanStop { get; set; }
    public bool CanPauseAndContinue { get; set; }
}
