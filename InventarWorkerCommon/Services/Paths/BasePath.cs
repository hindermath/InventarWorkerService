namespace InventarWorkerCommon.Services.Paths;

/// <summary>
/// Provides functionality to retrieve the base path of the system
/// based on the operating system.
/// </summary>
public class BasePath
{
    /// <summary>
    /// Retrieves the base path of the system depending on the operating system.
    /// </summary>
    /// <returns>
    /// Returns the base directory path as a string.
    /// For Windows, it is the common application data directory.
    /// For MacOS, it is the shared users directory.
    /// For other operating systems, it defaults to '/var/tmp'.
    /// </returns>
    public static string GetBasePath()
    {
        return OperatingSystem.IsWindows()
            ?
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            :
            OperatingSystem.IsMacOS() ? "/Users/Shared" : "/var/tmp";
    }
}