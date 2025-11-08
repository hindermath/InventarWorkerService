namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Represents disk information including capacity, file system and usage.
/// </summary>
public record DiskInfo
{
    /// <summary>
    /// Gets or sets the drive name or letter (for example, "C:").
    /// </summary>
    public string DriveName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the drive (for example, Fixed, Removable, Network).
    /// </summary>
    public string DriveType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file system used by the drive (for example, NTFS, ext4, APFS).
    /// </summary>
    public string FileSystem { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total capacity of the drive in bytes.
    /// </summary>
    public long TotalSize { get; set; }

    /// <summary>
    /// Gets or sets the available free space on the drive in bytes.
    /// </summary>
    public long AvailableSpace { get; set; }

    /// <summary>
    /// Gets or sets the used space on the drive in bytes.
    /// </summary>
    public long UsedSpace { get; set; }

    /// <summary>
    /// Gets or sets the percentage of used space (0–100).
    /// </summary>
    public double UsagePercentage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the drive is ready and accessible.
    /// </summary>
    public bool IsReady { get; set; }
}
