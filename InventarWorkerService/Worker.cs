using InventarWorkerService.Models;
using InventarWorkerService.StatusService.FileBased;

namespace InventarWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FileBasedStatusService _statusService;
    private int _processedItems = 0;
    private DateTime _startTime = DateTime.Now;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _statusService = new FileBasedStatusService();
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
                // Ihre Geschäftslogik hier
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
                
                var message = $"Verarbeitet: {_processedItems} Items";
                _logger.LogInformation(message);
                _statusService.WriteLog(message);
                
                await Task.Delay(5000, stoppingToken);
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
        // Simulation der Inventar-Verarbeitung
        await Task.Delay(1000);
    }

    private double CalculateAverageProcessingTime()
    {
        var totalTime = DateTime.Now - _startTime;
        return _processedItems > 0 ? totalTime.TotalMilliseconds / _processedItems : 0;
    }
}