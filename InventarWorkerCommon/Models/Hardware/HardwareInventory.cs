using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.Software;

namespace InventarWorkerCommon.Models.Hardware;

public record HardwareInventory
{
    public DateTime CollectionTime { get; set; } = DateTime.Now;
    public SystemInfo System { get; set; } = new();
    public CpuInfo Cpu { get; set; } = new();
    public MemoryInfo Memory { get; set; } = new();
    public List<DiskInfo> Disks { get; set; } = new();
    public List<NetworkInfo> NetworkInterfaces { get; set; } = new();
    public OsInfo OperatingSystem { get; set; } = new();
    public SoftwareInventory Software { get; set; } = new(); // Neu hinzugefügt
}