namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Represents information about a disk drive, including capacity, file system, and usage.
/// </summary>
public record DiskInfo
{
    /// <summary>
    /// Gets the drive name or letter (for example, "C:").
    /// </summary>
    public string DriveName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of the drive (for example, "Fixed", "Removable", "Network").
    /// </summary>
    public string DriveType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the file system used by the drive (for example, "NTFS", "ext4", "APFS").
    /// </summary>
    public string FileSystem { get; init; } = string.Empty;

    /// <summary>
    /// Gets the total capacity of the drive in bytes.
    /// </summary>
    public long TotalSize { get; init; }

    /// <summary>
    /// Gets the available free space on the drive in bytes.
    /// </summary>
    public long AvailableSpace { get; init; }

    /// <summary>
    /// Gets the used space on the drive in bytes.
    /// </summary>
    public long UsedSpace { get; init; }

    /// <summary>
    /// Gets the percentage of used space (0–100).
    /// </summary>
    public double UsagePercentage { get; init; }

    /// <summary>
    /// Gets a value indicating whether the drive is ready and accessible.
    /// </summary>
    public bool IsReady { get; init; }
}
