namespace InventarWorkerService.Models.Hardware;

public record DiskInfo
{
    public string DriveName { get; set; } = string.Empty;
    public string DriveType { get; set; } = string.Empty;
    public string FileSystem { get; set; } = string.Empty;
    public long TotalSize { get; set; }
    public long AvailableSpace { get; set; }
    public long UsedSpace { get; set; }
    public double UsagePercentage { get; set; }
    public bool IsReady { get; set; }
}
