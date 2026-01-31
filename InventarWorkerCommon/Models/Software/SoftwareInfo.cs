namespace InventarWorkerCommon.Models.Software;

/// <summary>
/// Represents information about an installed software package or application.
/// </summary>
public record SoftwareInfo
{
    /// <summary>
    /// Gets or sets the display name of the software.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version string of the software.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the publisher or vendor of the software.
    /// </summary>
    public string Publisher { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the installation date, if known.
    /// </summary>
    public DateTime? InstallDate { get; set; }

    /// <summary>
    /// Gets or sets the installation directory of the software.
    /// </summary>
    public string InstallLocation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the approximate size of the installation in bytes, if available.
    /// </summary>
    public long? Size { get; set; }

    /// <summary>
    /// Gets or sets the command used to uninstall the software.
    /// </summary>
    public string UninstallString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a path to an icon associated with the software, if any.
    /// </summary>
    public string DisplayIcon { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this is a system component (typically hidden from normal listings).
    /// </summary>
    public bool IsSystemComponent { get; set; }

    /// <summary>
    /// Gets or sets the software architecture (for example, x86, x64, arm64).
    /// </summary>
    public string Architecture { get; set; } = string.Empty;
}
