using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Services.Hardware;
using InventarWorkerCommon.Services.Software;
using InventarWorkerCommon.Services.Status;
using static InventarWorkerCommon.Helpers.Calculate.AverageProcessingTime;

namespace HarvesterWorkerService;

/// <summary>
/// Background worker that periodically writes service status and statistics, and logs activity
/// for the HarvesterWorkerService.
/// </summary>
/// <remarks>
/// Documentation for Worker Services can be found here:
/// EN: https://learn.microsoft.com/en-us/dotnet/core/extensions/workers
/// DE: https://learn.microsoft.com/de-de/dotnet/core/extensions/workers
/// </remarks>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ServiceStatusWriter _statusWriter = new("harvester-service");
    private readonly HardwareInventoryService _hardwareInventoryService;
    private readonly SoftwareInventoryService _softwareInventoryService; 
    private int _processedItems = 0;
    private DateTime _startTime = DateTime.Now;

    /// <summary>
    /// Initializes a new instance of the <see cref="Worker"/> class.
    /// </summary>
    /// <param name="logger">The logger used to write diagnostic information.</param>
    /// <param name="hardwareInventoryService">Service that provides hardware inventory processing.</param>
    /// <param name="softwareInventoryService">Service that provides software inventory processing.</param>
    public Worker(ILogger<Worker> logger,
        HardwareInventoryService hardwareInventoryService,
        SoftwareInventoryService softwareInventoryService)
    {
        _logger = logger;
        _hardwareInventoryService = hardwareInventoryService;
        _softwareInventoryService = softwareInventoryService;
    }

    /// <summary>
    /// Executes the background loop until cancellation is requested, periodically
    /// updating the service status and writing statistics/logs.
    /// </summary>
    /// <param name="stoppingToken">Token that signals when the service should stop.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Write initial status
        _statusWriter.WriteStatus(new ServiceStatus
        {
            State = "Starting HarvesterWorkerService",
            StartTime = _startTime,
            ProcessedItems = 0
        });
        
        while (!stoppingToken.IsCancellationRequested)
        {
            // Abfrage aus der DB von nicht-deaktivierten uns -deprovisionierten Maschinen
            // soll ein Array an IPv4, Maschinen-ID und Maschinenname zurückgeben
            try
            {
                // der try-Zweig muss dann in die foreach umschlossen werden
                // in der foreach dann jedesmal den Service-Container mit den spezifischen Parametern initialisieren
                // api-service hard und software per REST API abfrage
                // ermittelten Daten in SQL-DB und MongoDB speichern
                _processedItems++;
                
                // Update status
                _statusWriter.WriteStatus(new ServiceStatus
                {
                    State = "Running HarvesterWorkerService",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastActivity = DateTime.Now
                });
                
                _statusWriter.WriteStatistics(new ServiceStatistics
                {
                    TotalProcessedItems = _processedItems,
                    AverageProcessingTime = CalculateAverageProcessingTime(_processedItems,
                        _startTime),
                    Uptime = DateTime.Now - _startTime,
                    MemoryUsage = GC.GetTotalMemory(false)
                });

                var message = $"Inventory completed: {_processedItems} Runs";
                _logger.LogInformation(message);
                _statusWriter.WriteLog(message);
                await Task.Delay(30000, stoppingToken);
                // Am Ende der foreach die Services und Service-Container disposen
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HarvesterWorkerService");
                _statusWriter.WriteLog($"Error: {ex.Message}");
                _statusWriter.WriteStatus(new ServiceStatus
                {
                    State = "Error HarvesterWorkerService",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastError = ex.Message
                });
                // bei Fehler ggf. hier auch die aktuellen Services und Service-Container disposen!
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}