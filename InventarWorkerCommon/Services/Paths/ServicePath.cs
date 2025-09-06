namespace InventarWorkerCommon.Services.Paths;

/// <summary>
/// Provides methods for managing and retrieving platform-specific paths used by services.
/// </summary>
public static class ServicePath
{
    /// <summary>
    /// Retrieves the platform-specific path used to store or access service status information.
    /// Combines the base directory with a service status directory environment variable or defaults to a predefined name if the variable is not set.
    /// </summary>
    /// <returns>
    /// A string representing the computed service status path.
    /// </returns>
    public static string GetServiceStatusPath()
    {
        return System.IO.Path.Combine(BasePath.GetBasePath(),
            Environment.GetEnvironmentVariable("SERVICESTATUSDIRECTORY") ?? "InventarWorkerService");
    }
}