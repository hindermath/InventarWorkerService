namespace InventarWorkerService.Models;

public record HardwareInfo
{
    public DateTime CollectedAt { get; set; } = DateTime.Now;
    public SystemInfo System { get; set; } = new();
    public CpuInfo Cpu { get; set; } = new();
    public MemoryInfo Memory { get; set; } = new();
    public List<DiskInfo> Disks { get; set; } = new();
    public List<NetworkInfo> NetworkInterfaces { get; set; } = new();
    public OsInfo OperatingSystem { get; set; } = new();
}
