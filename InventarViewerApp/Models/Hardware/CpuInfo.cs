namespace InventarViewerApp.Models.Hardware;

public record CpuInfo
{
    public int ProcessorCount { get; set; }
    public string ProcessorName { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public double CurrentUsage { get; set; }
}
