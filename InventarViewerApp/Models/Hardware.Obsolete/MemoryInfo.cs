namespace InventarViewerApp.Models.Hardware;

public record MemoryInfo
{
    public long TotalPhysicalMemory { get; set; }
    public long AvailablePhysicalMemory { get; set; }
    public long UsedPhysicalMemory { get; set; }
    public double MemoryUsagePercentage { get; set; }
    public long ManagedMemoryUsage { get; set; }
}
