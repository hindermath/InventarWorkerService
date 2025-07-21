namespace InventarWorkerCommon.Models.Software;

public record SoftwareInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public DateTime? InstallDate { get; set; }
    public string InstallLocation { get; set; } = string.Empty;
    public long? Size { get; set; }
    public string UninstallString { get; set; } = string.Empty;
    public string DisplayIcon { get; set; } = string.Empty;
    public bool IsSystemComponent { get; set; }
    public string Architecture { get; set; } = string.Empty;
}
