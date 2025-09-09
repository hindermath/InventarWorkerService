namespace InventarWorkerCommon.Services.Paths;

/// <summary>
/// Provides methods for managing and retrieving platform-specific paths used by services.
/// </summary>
public static class ServicePath
{
    /// <summary>
    /// Retrieves the platform-specific path used to store or access service status information.
    /// Combines the base directory with a service status directory environment variable or defaults to a predefined
    /// name if the variable is not set.
    /// </summary>
    /// <returns>
    /// A string representing the computed service status path.
    /// </returns>
    public static string GetServiceStatusPath()
    {
        return System.IO.Path.Combine(BasePath.GetBasePath(),
            Environment.GetEnvironmentVariable("SERVICESTATUSDIRECTORY") ?? "InventarWorkerService");
    }

    /// <summary>
    /// Creates a directory at the specified service status path if it does not already exist.
    /// Validates the creation by checking if the directory exists after the attempt.
    /// </summary>
    /// <param name="serviceStatusPath">The path where the service status directory is to be created.</param>
    /// <returns>
    /// A boolean value indicating whether the directory exists after the creation attempt.
    /// Returns true if the directory exists, false otherwise.
    /// </returns>
    public static DirectoryInfo CreateServiceStatusPath(string serviceStatusPath)
    {
        return Directory.CreateDirectory(serviceStatusPath);
    }

    /// <summary>
    /// Checks whether the specified service status path exists in the file system.
    /// </summary>
    /// <param name="serviceStatusPath">The path to the service status directory to check for existence.</param>
    /// <returns>
    /// A boolean value indicating whether the specified path exists in the file system.
    /// Returns true if the path exists, false otherwise.
    /// </returns>
    public static bool ExistsServiceStatusPath(string serviceStatusPath)
    {
        return Directory.Exists(serviceStatusPath);   
    }
}