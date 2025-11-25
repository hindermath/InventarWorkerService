namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Provides operating system information such as version, architecture and description.
/// </summary>
public record OsInfo
{
    /// <summary>
    /// Gets or sets the OS platform (for example, Windows, Linux, macOS).
    /// </summary>
    public string Platform { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the OS version string.
    /// </summary>
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the service pack or update level, if applicable.
    /// </summary>
    public string ServicePack { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets a descriptive label of the OS distribution/build.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the OS architecture (for example, x64, arm64).
    /// </summary>
    public string Architecture { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the OS is 64-bit.
    /// </summary>
    public bool Is64Bit { get; set; }

    /// <summary>
    /// Gets or sets the number of logical processors reported by the OS.
    /// </summary>
    public int ProcessorCount { get; init; }

    /// <summary>
    /// Gets or sets the current user's domain name.
    /// </summary>
    public string UserDomainName { get; init; } = string.Empty;
}
