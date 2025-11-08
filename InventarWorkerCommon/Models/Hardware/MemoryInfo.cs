namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Provides memory usage and capacity details of the system.
/// </summary>
public record MemoryInfo
{
    /// <summary>
    /// Gets or sets the total amount of physical memory (RAM) installed, in bytes.
    /// </summary>
    public long TotalPhysicalMemory { get; set; }

    /// <summary>
    /// Gets or sets the currently available physical memory, in bytes.
    /// </summary>
    public long AvailablePhysicalMemory { get; set; }

    /// <summary>
    /// Gets or sets the currently used physical memory, in bytes.
    /// </summary>
    public long UsedPhysicalMemory { get; set; }

    /// <summary>
    /// Gets or sets the percentage of physical memory currently in use (0–100).
    /// </summary>
    public double MemoryUsagePercentage { get; set; }

    /// <summary>
    /// Gets or sets the managed memory currently used by the running .NET process, in bytes.
    /// </summary>
    public long ManagedMemoryUsage { get; set; }
}
