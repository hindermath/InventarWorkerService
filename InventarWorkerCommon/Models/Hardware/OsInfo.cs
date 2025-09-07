namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Provides operating system information such as version, architecture and description.
/// </summary>
public record OsInfo
{
    public string Platform { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string ServicePack { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public bool Is64Bit { get; set; }
    public int ProcessorCount { get; set; }
    public string UserDomainName { get; set; } = string.Empty;
}
