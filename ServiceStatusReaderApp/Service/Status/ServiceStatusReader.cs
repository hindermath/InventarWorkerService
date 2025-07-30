using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Models.Service;

namespace ServiceStatusReaderApp.ServiceStatus;

public class ServiceStatusReader
{
    private readonly string _statusDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    public ServiceStatusReader(string statusDirectory = "/tmp/inventar-service")
    {
        _statusDirectory = statusDirectory;
        _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };
    }

    /// <summary>
    /// Reads the current service status from the specified status file.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="InventarWorkerCommon.Models.Service.ServiceStatus"/>
    /// containing the current status information, or null if the status file does not exist
    /// or if an error occurs during the reading process.
    /// </returns>
    public InventarWorkerCommon.Models.Service.ServiceStatus? ReadStatus()
    {
        try
        {
            var statusFile = Path.Combine(_statusDirectory, "status.json");
            if (!File.Exists(statusFile)) return null;
            
            var json = File.ReadAllText(statusFile);
            return JsonSerializer.Deserialize<InventarWorkerCommon.Models.Service.ServiceStatus>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Lesen des Status: {ex.Message}");
            return null;
        }
    }

    public ServiceStatistics? ReadStatistics()
    {
        try
        {
            var statsFile = Path.Combine(_statusDirectory, "statistics.json");
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

    public List<string> ReadRecentLogs(int maxLines = 50)
    {
        try
        {
            var logFile = Path.Combine(_statusDirectory, "service.log");
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

    public bool IsServiceRunning()
    {
        var status = ReadStatus();
        if (status == null) return false;
        
        // Service gilt als laufend, wenn der letzte Status < 30 Sekunden alt ist
        var timeSinceLastActivity = DateTime.Now - status.LastActivity;
        return status.State == "Running" && timeSinceLastActivity.TotalSeconds < 30;
    }
}