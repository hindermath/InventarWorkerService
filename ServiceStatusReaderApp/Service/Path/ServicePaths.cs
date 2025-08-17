using System.Runtime.InteropServices;

namespace ServiceStatusReaderApp.Service.Path;

/// <summary>
/// Provides utility methods to retrieve platform-specific paths
/// used for storing or accessing service status information.
/// </summary>
public static class ServicePaths
{
    /// <summary>
    /// Retrieves the directory path used for storing or accessing service status information.
    /// The returned path varies depending on the operating system.
    /// For example, it returns a platform-specific path for Windows, Linux, and macOS systems.
    /// </summary>
    /// <returns>
    /// A string representing the platform-specific directory path for storing service status information.
    /// </returns>
    public static string GetStatusDirectory()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
            //     "InventarWorkerService");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "/var/lib/inventar-worker-service";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "/usr/local/var/inventar-worker-service";
        }
        
        return "/tmp/inventar-worker-service";
    }
}