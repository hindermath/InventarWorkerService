using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Services.Paths;

namespace InventarWorkerCommon.Services.Status;

/// <summary>
/// Provides methods to read and interpret the status, statistics, and logs of a service.
/// </summary>
public class ServiceStatusReader
{
    private readonly string _statusDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Provides methods to read and interpret the status, statistics, and logs of a service.
    /// </summary>
    /// <remarks>
    /// This class allows checking the service status, retrieving detailed service statistics,
    /// fetching recent log entries, and confirming if the service is currently active.
    /// </remarks>
    public ServiceStatusReader(string statusDirectory = "inventar-service")
    {
        if (ServicePath.ExistsServiceStatusPath(Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory)))
            _statusDirectory = Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory);
        _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };
    }

    /// <summary>
    /// Reads the current service status from a predefined status file.
    /// If the status file does not exist or an error occurs during reading,
    /// the method returns null.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="ServiceStatus"/> representing the current service status,
    /// or null if the status file is missing or could not be read.
    /// </returns>
    public ServiceStatus? ReadStatus()
    {
        try
        {
            var statusFile = System.IO.Path.Combine(_statusDirectory, "status.json");
            if (!File.Exists(statusFile)) return null;
            
            var json = File.ReadAllText(statusFile);
            return JsonSerializer.Deserialize<ServiceStatus>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Lesen des Status: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Reads the service's statistical data from a predefined statistics file.
    /// If the file does not exist or an error occurs during reading, the method returns null.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="ServiceStatistics"/> containing the statistical information,
    /// or null if the statistics file is missing or could not be read.
    /// </returns>
    public ServiceStatistics? ReadStatistics()
    {
        try
        {
            var statsFile = System.IO.Path.Combine(_statusDirectory, "statistics.json");
            if (!File.Exists(statsFile)) return null;
            
            var json = File.ReadAllText(statsFile);
            return JsonSerializer.Deserialize<ServiceStatistics>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Lesen der Statistiken: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Reads the most recent log entries from the service log file.
    /// If the log file does not exist or an error occurs during reading,
    /// the method returns an empty list.
    /// </summary>
    /// <param name="maxLines">The maximum number of recent log lines to read. Defaults to 10.</param>
    /// <returns>A list of strings containing the most recent log entries, or an empty list if the log file is missing or an error occurs.</returns>
    public List<string> ReadRecentLogs(int maxLines = 10)
    {
        try
        {
            var logFile = System.IO.Path.Combine(_statusDirectory, "service.log");
            if (!File.Exists(logFile)) return new List<string>();
            
            var lines = File.ReadAllLines(logFile);
            return lines.TakeLast(maxLines).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Lesen der Logs: {ex.Message}");
            return new List<string>();
        }
    }

    /// <summary>
    /// Determines whether the service is currently running by checking the state and the recency of the last activity.
    /// </summary>
    /// <returns>
    /// True if the service is in a running state and the last activity occurred within the past 30 seconds; otherwise, false.
    /// </returns>
    public bool IsServiceRunning()
    {
        var status = ReadStatus();
        if (status == null) return false;
        
        // Service gilt als laufend, wenn der letzte Status < 30 Sekunden alt ist
        var timeSinceLastActivity = DateTime.Now - status.LastActivity;
        return status.State == "Running" && timeSinceLastActivity.TotalSeconds < 30;
    }
}