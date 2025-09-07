using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Services.Hardware;
using InventarWorkerCommon.Services.Software;
using InventarWorkerCommon.Services.Status;
using static InventarWorkerCommon.Helpers.Calculate.AverageProcessingTime;

namespace InventarWorkerService;

/// <summary>
/// Background worker that periodically collects hardware and software inventory data
/// and writes status, statistics, and logs for the InventarWorkerService.
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ServiceStatusWriter _statusWriter;
    private readonly HardwareInventoryService _hardwareInventoryService;
    private readonly SoftwareInventoryService _softwareInventoryService;
    private int _processedItems = 0;
    private DateTime _startTime = DateTime.Now;

    /// <summary>
    /// Initializes a new instance of the Worker.
    /// </summary>
    /// <param name="logger">Logger used for operational messages and error reporting.</param>
    /// <param name="hardwareInventoryService">Service to collect hardware information.</param>
    /// <param name="softwareInventoryService">Service to collect software information.</param>
    public Worker(ILogger<Worker> logger, 
                  HardwareInventoryService hardwareInventoryService,
                  SoftwareInventoryService softwareInventoryService)
    {
        _logger = logger;
        _statusWriter = new ServiceStatusWriter();
        _hardwareInventoryService = hardwareInventoryService;
        _softwareInventoryService = softwareInventoryService;
    }

    /// <summary>
    /// Continuously executes the worker's main logic for processing inventory tasks
    /// and updates the status service with the current execution state.
    /// </summary>
    /// <param name="stoppingToken">A cancellation token that signals the worker to stop.</param>
    /// <returns>A Task representing the asynchronous execution of the worker's main loop.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _statusWriter.WriteStatus(new ServiceStatus
        {
            State = "Starting",
            StartTime = _startTime,
            ProcessedItems = 0
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Conduct hardware and software inventory.
                await ProcessInventoryItems();
                
                _processedItems++;
                
                // Update status
                _statusWriter.WriteStatus(new ServiceStatus
                {
                    State = "Running",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastActivity = DateTime.Now
                });
                
                // Update statistics
                _statusWriter.WriteStatistics(new ServiceStatistics
                {
                    TotalProcessedItems = _processedItems,
                    AverageProcessingTime = CalculateAverageProcessingTime(_processedItems,
                        _startTime),
                    Uptime = DateTime.Now - _startTime,
                    MemoryUsage = GC.GetTotalMemory(false)
                });
                
                var message = $"Inventarisierung abgeschlossen: {_processedItems} Durchläufe";
                _logger.LogInformation(message);
                _statusWriter.WriteLog(message);
                
                await Task.Delay(30000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in Worker");
                _statusWriter.WriteLog($"ERROR: {ex.Message}");
                _statusWriter.WriteStatus(new ServiceStatus
                {
                    State = "Error",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastError = ex.Message
                });
                
                await Task.Delay(5000, stoppingToken);
            }
        }

        _statusWriter.WriteStatus(new ServiceStatus
        {
            State = "Stopped",
            StartTime = _startTime,
            ProcessedItems = _processedItems,
            LastActivity = DateTime.Now
        });
    }

    private async Task ProcessInventoryItems()
    {
        _logger.LogInformation("Starte Hardware- und Software-Inventarisierung...");
        
        try
        {
            // Collect in parallel for better performance
            var hardwareTask = _hardwareInventoryService.CollectHardwareInfoAsync();
            var softwareTask = _softwareInventoryService.CollectSoftwareInventoryAsync();
            
            await Task.WhenAll(hardwareTask, softwareTask);
            
            var hardwareInfo = await hardwareTask;
            var softwareInventory = await softwareTask;
            
            // Add software inventory to hardware information
            hardwareInfo.Software = softwareInventory;
            
            // Save inventory
            await SaveInventory(hardwareInfo);
            
            _logger.LogInformation("Inventarisierung erfolgreich abgeschlossen");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei der Inventarisierung");
            throw;
        }
    }

    private async Task SaveInventory(HardwareInfo hardwareInfo)
    {
        try
        {
            await _statusWriter.WriteHardwareInventory(hardwareInfo);
            _logger.LogInformation("Inventar gespeichert.");
            
            // Log short summary
            LogInventorySummary(hardwareInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Speichern des Inventars");
        }
    }

    private void LogInventorySummary(HardwareInfo hardwareInfo)
    {
        var summary = $"""
            === Inventar Zusammenfassung ===
            System: {hardwareInfo.System.MachineName} ({hardwareInfo.OperatingSystem.Platform})
            CPU: {hardwareInfo.Cpu.ProcessorName} ({hardwareInfo.Cpu.ProcessorCount} Kerne)
            Speicher: {hardwareInfo.Memory.TotalPhysicalMemory / (1024 * 1024 * 1024)} GB total, {hardwareInfo.Memory.MemoryUsagePercentage:F1}% verwendet
            Festplatten: {hardwareInfo.Disks.Count} Laufwerke
            Netzwerk: {hardwareInfo.NetworkInterfaces.Count} Schnittstellen
            Software: {hardwareInfo.Software.InstalledSoftware.Count} installierte Programme
            Prozesse: {hardwareInfo.Software.RunningProcesses.Count} laufende Prozesse
            Services: {hardwareInfo.Software.WindowsServices.Count} Windows-Services
            Uptime: {hardwareInfo.System.Uptime.Days} Tage, {hardwareInfo.System.Uptime.Hours:D2}:{hardwareInfo.System.Uptime.Minutes:D2}
            ===============================
            """;
        
        _logger.LogInformation(summary);
    }
}