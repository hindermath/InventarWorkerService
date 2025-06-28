using System.Text.Json;
using InventarWorkerService.Models;
using InventarWorkerService.Services.Hardware;
using InventarWorkerService.Services.Software;
using InventarWorkerService.Services.Status;

namespace InventarWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FileBasedStatusService _statusService;
    private readonly HardwareInventoryService _hardwareInventoryService;
    private readonly SoftwareInventoryService _softwareInventoryService;
    private int _processedItems = 0;
    private DateTime _startTime = DateTime.Now;

    public Worker(ILogger<Worker> logger, 
                  HardwareInventoryService hardwareInventoryService,
                  SoftwareInventoryService softwareInventoryService)
    {
        _logger = logger;
        _statusService = new FileBasedStatusService();
        _hardwareInventoryService = hardwareInventoryService;
        _softwareInventoryService = softwareInventoryService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _statusService.WriteStatus(new ServiceStatus
        {
            State = "Starting",
            StartTime = _startTime,
            ProcessedItems = 0
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Hardware- und Software-Inventarisierung durchführen
                await ProcessInventoryItems();
                
                _processedItems++;
                
                // Status aktualisieren
                _statusService.WriteStatus(new ServiceStatus
                {
                    State = "Running",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastActivity = DateTime.Now
                });
                
                // Statistiken aktualisieren
                _statusService.WriteStatistics(new ServiceStatistics
                {
                    TotalProcessedItems = _processedItems,
                    AverageProcessingTime = CalculateAverageProcessingTime(),
                    Uptime = DateTime.Now - _startTime,
                    MemoryUsage = GC.GetTotalMemory(false)
                });
                
                var message = $"Inventarisierung abgeschlossen: {_processedItems} Durchläufe";
                _logger.LogInformation(message);
                _statusService.WriteLog(message);
                
                await Task.Delay(30000, stoppingToken); // Alle 30 Sekunden
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in Worker");
                _statusService.WriteLog($"ERROR: {ex.Message}");
                _statusService.WriteStatus(new ServiceStatus
                {
                    State = "Error",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastError = ex.Message
                });
                
                await Task.Delay(5000, stoppingToken); // Bei Fehlern kürzere Pause
            }
        }

        _statusService.WriteStatus(new ServiceStatus
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
            // Parallel sammeln für bessere Performance
            var hardwareTask = _hardwareInventoryService.CollectHardwareInfoAsync();
            var softwareTask = _softwareInventoryService.CollectSoftwareInventoryAsync();
            
            await Task.WhenAll(hardwareTask, softwareTask);
            
            var hardwareInfo = await hardwareTask;
            var softwareInventory = await softwareTask;
            
            // Software-Inventar zu Hardware-Info hinzufügen
            hardwareInfo.Software = softwareInventory;
            
            // Inventar speichern
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
            await _statusService.WriteHardwareInventory(hardwareInfo);
            _logger.LogInformation("Inventar gespeichert.");
            
            // Kurzzusammenfassung loggen
            LogInventorySummary(hardwareInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Speichern des Inventars");
        }
    }

    private void LogInventorySummary(HardwareInfo hardwareInfo)
    {
        var summary = $@"
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
===============================";
        
        _logger.LogInformation(summary);
    }

    private double CalculateAverageProcessingTime()
    {
        var totalTime = DateTime.Now - _startTime;
        return _processedItems > 0 ? totalTime.TotalMilliseconds / _processedItems : 0;
    }
}