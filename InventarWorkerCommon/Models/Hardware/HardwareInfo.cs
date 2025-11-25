using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.Software;

namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Aggregates system, CPU, memory, disks, network, OS and software inventory data.
/// </summary>
public class HardwareInfo
{
    /// <summary>
    /// Gets or sets the timestamp when this hardware snapshot was collected.
    /// </summary>
    public DateTime CollectionTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets general system information about the machine.
    /// </summary>
    public SystemInfo System { get; set; } = new();

    /// <summary>
    /// Gets or sets CPU-related information for the machine.
    /// </summary>
    public CpuInfo Cpu { get; set; } = new();

    /// <summary>
    /// Gets or sets memory information, including capacity and usage.
    /// </summary>
    public MemoryInfo Memory { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of physical or logical disks.
    /// </summary>
    public List<DiskInfo> Disks { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of network interfaces and their details.
    /// </summary>
    public List<NetworkInfo> NetworkInterfaces { get; set; } = new();

    /// <summary>
    /// Gets or sets operating system information.
    /// </summary>
    public OsInfo OperatingSystem { get; set; } = new();

    /// <summary>
    /// Gets or sets the software inventory captured for the machine.
    /// </summary>
    public SoftwareInventory Software { get; set; } = new(); // Neu hinzugefügt
}