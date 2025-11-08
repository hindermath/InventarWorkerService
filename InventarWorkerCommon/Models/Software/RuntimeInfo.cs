namespace InventarWorkerCommon.Models.Software;

/// <summary>
/// Represents information about runtime environments and tooling available on the system.
/// </summary>
public record RuntimeInfo
{
    /// <summary>
    /// Gets or sets the installed .NET runtime version.
    /// </summary>
    public string DotNetVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of installed .NET frameworks/runtimes.
    /// </summary>
    public List<string> InstalledFrameworks { get; set; } = new();

    /// <summary>
    /// Gets or sets the installed PowerShell version, if available.
    /// </summary>
    public string PowerShellVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of available PowerShell modules.
    /// </summary>
    public List<string> AvailableModules { get; set; } = new();
}
