using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Services.Paths;

namespace InventarWorkerCommon.Services.Status;

/// <summary>
/// The ServiceStatusWriter class is responsible for managing and writing the status, logs, statistics,
/// performance metrics, and hardware inventory data of a service to files in a specified directory.
/// </summary>
public class ServiceStatusWriter
{
    private readonly string _statusDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// The <c>ServiceStatusWriter</c> class provides mechanisms for managing, serializing, and writing
    /// service-related data such as status, logs, statistics, performance metrics, and hardware inventory
    /// to a specified directory. This ensures that the service's state and activities are persistently
    /// stored for tracking, diagnostics, and operational insights.
    /// </summary>
    public ServiceStatusWriter(string statusDirectory = "inventar-service")
    {
        if (ServicePath.ExistsServiceStatusPath(Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory)) is
            false)
        {
            var directory =
                ServicePath.CreateServiceStatusPath(Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory));
            _statusDirectory = directory.FullName;
        }
        else
        {
            _statusDirectory = Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory);
        }

        _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };
    }

    /// <summary>
    /// Saves the current state and operational information of the service to a status file
    /// in JSON format within the designated status directory.
    /// </summary>
    /// <param name="status">An instance of <c>ServiceStatus</c> containing the current details
    /// about the service's state, such as operational status, processed items,
    /// and timestamps.</param>
    public void WriteStatus(ServiceStatus status)
    {
        var statusFile = Path.Combine(_statusDirectory, "status.json");
        var json = JsonSerializer.Serialize(status, _jsonOptions);
        File.WriteAllText(statusFile, json);
    }

    /// <summary>
    /// Appends a log entry to the service log file, containing a timestamp and the specified message.
    /// </summary>
    /// <param name="message">The message to be recorded in the service log.</param>
    public void WriteLog(string message)
    {
        var logFile = Path.Combine(_statusDirectory, "service.log");
        var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n";
        File.AppendAllText(logFile, logEntry);
    }

    /// <summary>
    /// The <c>WriteStatistics</c> method serializes and writes service statistics data,
    /// such as processing metrics and operational insights, to a predefined JSON file
    /// in the service's designated directory. This ensures that statistical information
    /// is properly stored and accessible for analysis.
    /// </summary>
    /// <param name="stats">An instance of the <c>ServiceStatistics</c> class containing
    /// statistical data related to the service's operations.</param>
    public void WriteStatistics(ServiceStatistics stats)
    {
        var statsFile = Path.Combine(_statusDirectory, "statistics.json");
        var json = JsonSerializer.Serialize(stats, _jsonOptions);
        File.WriteAllText(statsFile, json);
    }

    /// <summary>
    /// Writes performance metrics data to a JSON file within the service's status directory.
    /// This method serializes the provided <c>PerformanceMetrics</c> object to ensure
    /// that detailed metrics such as CPU usage, memory usage, thread count, and a timestamp are
    /// persistently stored for monitoring and diagnostics purposes.
    /// </summary>
    /// <param name="metrics">An instance of <c>PerformanceMetrics</c> containing information
    /// about CPU usage, memory usage, thread count, and the timestamp at which the metrics were captured.</param>
    public void WritePerformanceMetrics(PerformanceMetrics metrics)
    {
        var metricsFile = Path.Combine(_statusDirectory, "metrics.json");
        var json = JsonSerializer.Serialize(metrics, _jsonOptions);
        File.WriteAllText(metricsFile, json);
    }

    /// <summary>
    /// Saves the hardware inventory information to a JSON file in the specified service status directory.
    /// The file name is timestamped to ensure uniqueness and retain a historical record of hardware data.
    /// </summary>
    /// <param name="hardwareInfo">
    /// An instance of <c>HardwareInfo</c> containing details about the system's hardware,
    /// including CPU, memory, disks, network interfaces, operating system, and installed software.
    /// </param>
    /// <returns>
    /// A <c>Task</c> representing the asynchronous save operation.
    /// </returns>
    public async Task WriteHardwareInventory(HardwareInfo hardwareInfo)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var inventoryFile = Path.Combine(_statusDirectory, $"hardware_inventory_{timestamp}.json");
        var json = JsonSerializer.Serialize(hardwareInfo, _jsonOptions);
        await File.WriteAllTextAsync(inventoryFile, json);
    }
}