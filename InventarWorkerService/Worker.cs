using InventarWorkerService.Models;
using InventarWorkerService.Services;
using InventarWorkerService.StatusService.FileBased;
using System.Text.Json;
using InventarWorkerService.Services.Hardware;

namespace InventarWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FileBasedStatusService _statusService;
    private readonly HardwareInventoryService _hardwareInventoryService;
    private int _processedItems = 0;
    private DateTime _startTime = DateTime.Now;

    public Worker(ILogger<Worker> logger, HardwareInventoryService hardwareInventoryService)
    {
        _logger = logger;
        _statusService = new FileBasedStatusService();
        _hardwareInventoryService = hardwareInventoryService;
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
                // Hardware-Inventarisierung durchführen
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
                
                var message = $"Hardware-Inventarisierung abgeschlossen: {_processedItems} Durchläufe";
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
        _logger.LogInformation("Starte Hardware-Inventarisierung...");
        
        try
        {
            var hardwareInfo = await _hardwareInventoryService.CollectHardwareInfoAsync();
            
            // Hardware-Informationen speichern
            await SaveHardwareInventory(hardwareInfo);
            
            _logger.LogInformation("Hardware-Inventarisierung erfolgreich abgeschlossen");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei der Hardware-Inventarisierung");
            throw;
        }
    }

    private async Task SaveHardwareInventory(HardwareInfo hardwareInfo)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"hardware_inventory_{timestamp}.json";
            var filePath = Path.Combine("inventory", fileName);
            
            // Verzeichnis erstellen falls nicht vorhanden
            Directory.CreateDirectory("inventory");
            
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var jsonData = JsonSerializer.Serialize(hardwareInfo, jsonOptions);
            await File.WriteAllTextAsync(filePath, jsonData);
            
            _logger.LogInformation("Hardware-Inventar gespeichert: {FilePath}", filePath);
            
            // Kurzzusammenfassung loggen
            LogHardwareSummary(hardwareInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Speichern des Hardware-Inventars");
            throw;
        }
    }

    private void LogHardwareSummary(HardwareInfo hardwareInfo)
    {
        var summary = $@"
=== Hardware-Inventar Zusammenfassung ===
System: {hardwareInfo.System.MachineName} ({hardwareInfo.OperatingSystem.Platform})
CPU: {hardwareInfo.Cpu.ProcessorName} ({hardwareInfo.Cpu.ProcessorCount} Kerne)
Speicher: {hardwareInfo.Memory.TotalPhysicalMemory / (1024 * 1024 * 1024)} GB total, {hardwareInfo.Memory.MemoryUsagePercentage:F1}% verwendet
Festplatten: {hardwareInfo.Disks.Count} Laufwerke
Netzwerk: {hardwareInfo.NetworkInterfaces.Count} Schnittstellen
Uptime: {hardwareInfo.System.Uptime.Days} Tage, {hardwareInfo.System.Uptime.Hours:D2}:{hardwareInfo.System.Uptime.Minutes:D2}
==========================================";
        
        _logger.LogInformation(summary);
    }

    private double CalculateAverageProcessingTime()
    {
        var totalTime = DateTime.Now - _startTime;
        return _processedItems > 0 ? totalTime.TotalMilliseconds / _processedItems : 0;
    }
}