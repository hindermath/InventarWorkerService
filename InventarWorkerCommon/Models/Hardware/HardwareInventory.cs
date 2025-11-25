using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.Software;

namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Represents a comprehensive hardware and software inventory snapshot of a machine.
/// </summary>
public record HardwareInventory
{
    /// <summary>
    /// Gets or sets the timestamp when the inventory was collected.
    /// </summary>
    public DateTime CollectionTime { get; init; } = DateTime.Now;

    /// <summary>
    /// Gets or sets general system information.
    /// </summary>
    public SystemInfo System { get; init; } = new();

    /// <summary>
    /// Gets or sets CPU details.
    /// </summary>
    public CpuInfo Cpu { get; init; } = new();

    /// <summary>
    /// Gets or sets memory capacity and usage information.
    /// </summary>
    public MemoryInfo Memory { get; init; } = new();

    /// <summary>
    /// Gets or sets the collection of disks detected on the machine.
    /// </summary>
    public List<DiskInfo> Disks { get; init; } = new();

    /// <summary>
    /// Gets or sets network interface information for the machine.
    /// </summary>
    public List<NetworkInfo> NetworkInterfaces { get; init; } = new();

    /// <summary>
    /// Gets or sets operating system details.
    /// </summary>
    public OsInfo OperatingSystem { get; init; } = new();

    /// <summary>
    /// Gets or sets the software inventory collected together with the hardware snapshot.
    /// </summary>
    public SoftwareInventory Software { get; init; } = new(); // Neu hinzugefügt
}