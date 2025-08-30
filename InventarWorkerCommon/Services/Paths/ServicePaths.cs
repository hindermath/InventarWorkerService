using System.IO;
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
    /// The directory path is constructed by combining the system's temporary path with an environment
    /// variable. If the specified environment variable is not set, a default directory name is used.
    /// </summary>
    /// <returns>The full path of the service status directory.</returns>
    public static string GetServiceStatusDirectory()
    {
        return System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            Environment.GetEnvironmentVariable("SERVICESTATUSDIRECTORY") ?? "InventarWorkerService");
    }
}