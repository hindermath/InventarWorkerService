namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Provides memory usage and capacity details of the system.
/// </summary>
public record MemoryInfo
{
    public long TotalPhysicalMemory { get; set; }
    public long AvailablePhysicalMemory { get; set; }
    public long UsedPhysicalMemory { get; set; }
    public double MemoryUsagePercentage { get; set; }
    public long ManagedMemoryUsage { get; set; }
}
