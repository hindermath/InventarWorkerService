using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Services.Hardware;
using InventarWorkerCommon.Services.Software;
using InventarWorkerCommon.Services.Status;
using static InventarWorkerCommon.Helpers.Calculate.AverageProcessingTime;

namespace HarvesterWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ServiceStatusWriter _statusWriter = new("harvester-service");
    private readonly HardwareInventoryService _hardwareInventoryService;
    private readonly SoftwareInventoryService _softwareInventoryService; 
    private int _processedItems = 0;
    private DateTime _startTime = DateTime.Now;

    public Worker(ILogger<Worker> logger,
        HardwareInventoryService hardwareInventoryService,
        SoftwareInventoryService softwareInventoryService)
    {
        _logger = logger;
        _hardwareInventoryService = hardwareInventoryService;
        _softwareInventoryService = softwareInventoryService;
    }

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
            try
            {
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
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}