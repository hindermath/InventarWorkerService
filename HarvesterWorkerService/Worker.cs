using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Models.Network;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using InventarWorkerCommon.Services.Hardware;
using InventarWorkerCommon.Services.Network;
using InventarWorkerCommon.Services.Software;
using InventarWorkerCommon.Services.Status;
using static InventarWorkerCommon.Helpers.Calculate.AverageProcessingTime;
using static InventarWorkerCommon.Services.Common.Initialize;

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
    private readonly JsonSerializerOptions _jsonOptions;
    private int _processedItems = 0;
    private DateTime _startTime = DateTime.Now;
    private ApiService _apiService;
    private SqliteDbService _sqliteDbService;
    private MongoDbService _mongoDbService;


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
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

    }

    /// <summary>
    /// Executes the background loop until cancellation is requested, periodically
    /// updating the service status and writing statistics/logs.
    /// </summary>
    /// <param name="stoppingToken">Token that signals when the service should stop.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var serviceContainer = Services();
        // wird hier (noch9 nicht benötigt) _apiService = serviceContainer.ApiService;
        _sqliteDbService = serviceContainer.DbService;
        _mongoDbService = serviceContainer.MongoDbService;


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
            var allActiveMachines = await _sqliteDbService.GetAllActiveMachinesAsync();

            try
            {
                foreach (var machine in allActiveMachines)
                {
                    HostInformationResult hostInformationResult = await ResolveMachine.ResolveFqdnToHostInfoAsync(machine.FQDN, preferIPv4: true);

                    // Falls wir nur einen FQDN haben, zu IP auflösen
                    if (!string.IsNullOrEmpty(machine.FQDN))
                    {
                        hostInformationResult = await ResolveMachine.ResolveFqdnToHostInfoAsync(machine.FQDN, preferIPv4: true);
                        if (hostInformationResult.IPv4Addresses != null)
                        {
                            _logger.LogInformation($"Resolved {machine.FQDN} to {hostInformationResult.IPv4Addresses}");
                        }
                    }

                    // Fallback auf gespeicherte IPv4, falls vorhanden
                    //targetIp ??= machine.IPv4;

                    // if (string.IsNullOrEmpty(targetIp))
                    // {
                    //     _logger.LogWarning($"No valid IP address found for machine {machine.FQDN ?? "unknown"}");
                    //     continue;
                    // }

                    // Ping-Test vor der Verbindung
                    if (await ResolveMachine.IsMachineReachableAsync(machine.IPv4) is false)
                    {
                        _logger.LogCritical($"Machine {machine.IPv4} is not reachable, skipping...");
                        continue;
                    }
                    // Versuche FQDN aus IPv4 zu ermitteln, falls noch nicht vorhanden
                    var hostInfo = await ResolveMachine.ResolveIpToHostInfoAsync(machine.IPv4);
                    if (string.IsNullOrEmpty(hostInfo.HostName) && string.IsNullOrEmpty(machine.IPv4) is false)
                    {
                        hostInfo.HostName = await ResolveMachine.ResolveIpToFqdnAsync(machine.IPv4);
                        if (string.IsNullOrEmpty(hostInfo.HostName) is false)
                        {
                            _logger.LogInformation($"Resolved {machine.IPv4} to {hostInfo.HostName}");
                        }
                        else
                        {
                            _logger.LogCritical($"Machine IP {machine.IPv4} or FQDN {hostInfo.HostName} is not reachable, skipping...");
                            continue;
                        }
                    }
                    else
                    {
                        hostInfo.HostName = machine.FQDN;
                    }

                    using var workerServiceContainer = Services(clientApiFqdn: hostInfo.HostName ?? machine.IPv4);
                    _apiService = workerServiceContainer.ApiService;

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
#if DEBUG
                    await Task.Delay(100, stoppingToken);
#else
                    await Task.Delay(30000, stoppingToken);
#endif
                    // Am Ende der foreach die Services und Service-Container disposen
                }
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