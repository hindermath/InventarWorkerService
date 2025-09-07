namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Describes CPU-related information such as core count, name, architecture and current usage.
/// </summary>
public record CpuInfo
{
    public int ProcessorCount { get; set; }
    public string ProcessorName { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public double CurrentUsage { get; set; }
}
