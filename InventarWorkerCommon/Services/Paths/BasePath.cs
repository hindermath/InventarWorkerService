namespace InventarWorkerCommon.Services.Paths;

/// <summary>
/// Provides functionality to retrieve the base path of the system
/// based on the operating system.
/// </summary>
public class BasePath
{
    // DE: Der Test-Hook hält Unit-Tests von systemverwalteten Produktionspfaden fern.
    // EN: The test hook keeps unit tests away from service-manager-owned production paths.
    internal static string? UnixBasePathOverride { get; set; }

    /// <summary>
    /// Retrieves the base path of the system depending on the operating system.
    /// </summary>
    /// <returns>
    /// Returns the base directory path as a string.
    /// For Windows, it is the common application data directory.
    /// On Unix-like systems, it is a service-manager-owned location outside user-controlled profiles.
    /// </returns>
    public static string GetBasePath()
    {
        if (!string.IsNullOrWhiteSpace(UnixBasePathOverride))
        {
            return UnixBasePathOverride;
        }

        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
        }

        return OperatingSystem.IsMacOS() ? "/Library/Application Support" : "/var/lib";
    }
}
