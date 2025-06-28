using System.Text.Json;
using InventarWorkerService.Models;

namespace InventarWorkerService.StatusService.FileBased;

public class FileBasedStatusService
{
    private readonly string _statusDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    public FileBasedStatusService(string statusDirectory = "/tmp/inventar-service")
    {
        _statusDirectory = statusDirectory;
        _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        
        Directory.CreateDirectory(_statusDirectory);
    }

    public void WriteStatus(ServiceStatus status)
    {
        var statusFile = Path.Combine(_statusDirectory, "status.json");
        var json = JsonSerializer.Serialize(status, _jsonOptions);
        File.WriteAllText(statusFile, json);
    }

    public void WriteLog(string message)
    {
        var logFile = Path.Combine(_statusDirectory, "service.log");
        var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n";
        File.AppendAllText(logFile, logEntry);
    }

    public void WriteStatistics(ServiceStatistics stats)
    {
        var statsFile = Path.Combine(_statusDirectory, "statistics.json");
        var json = JsonSerializer.Serialize(stats, _jsonOptions);
        File.WriteAllText(statsFile, json);
    }

    public void WritePerformanceMetrics(PerformanceMetrics metrics)
    {
        var metricsFile = Path.Combine(_statusDirectory, "metrics.json");
        var json = JsonSerializer.Serialize(metrics, _jsonOptions);
        File.WriteAllText(metricsFile, json);
    }
}