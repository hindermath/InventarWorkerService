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
using InventarWorkerCommon.Helpers.Calculate;
using InventarWorkerCommon.Helpers.Exceptions;
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
    private readonly AverageProcessingTime _averageProcessingTime = new AverageProcessingTime();
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
            // Abfrage aus der DB von nicht-deaktivierten uns -deprovisionierten Maschinen mit Netzwerkinformationen
            var allActiveMachinesWithNetworkInfo = await _sqliteDbService.GetAllActiveMachinesWithNetworkInfoAsync();

            try
            {
                foreach (var activeMachineWithNetworkInfo in allActiveMachinesWithNetworkInfo)
                {
                    // hat Machine entweder eine IPv4, IPv6 oder einen FQDN?
                    if (string.IsNullOrEmpty(activeMachineWithNetworkInfo.IPv4) ||
                        string.IsNullOrEmpty(activeMachineWithNetworkInfo.IPv6) ||
                        string.IsNullOrEmpty(activeMachineWithNetworkInfo.FQDN))
                    {
                        throw new NetworkInformation.NetworkInformationMissingException(activeMachineWithNetworkInfo
                            .Name);
                    }

                    // Nein throw new Exception (irgendetwas mit Network, dass beim catch-Abfangen kann (oder eigene erstellen)
                    // Ja, dann geht es hier weiter
                    // Hat FQDN, Dann ResolveFqdnToHostInfoAsync -> infos ggf. in entsprechende Variablen speichern
                    HostInformationResult hostInformationResult =
                        await ResolveMachine.ResolveFqdnToHostInfoAsync(activeMachineWithNetworkInfo.FQDN,
                            preferIPv4: true);
                    // In den Hostinformation sollten wir nun mindestens eine IPv4 haben, falls mehrere die erste aus der Liste nehmen
                    // Alternative Liste foreach
                    // ist diese/eine erreichbar?
                    // Nein throw new Exception (irgendetwas mit Network, dass beim catch-Abfangen kann (oder eigene erstellen)
                    // Ja, dann geht es hier weiter
                    //                     using var workerServiceContainer = Services(clientApiFqdn: hostInfo.HostName ?? activeMachineWithNetworkInfo.IPv4);
                    // _apiService = workerServiceContainer.ApiService;
                    // und dann der Rest des Worker-try-Zweigs


                    // Falls wir nur einen FQDN haben, zu IP auflösen
                    if (string.IsNullOrEmpty(activeMachineWithNetworkInfo.FQDN) is false)
                    {
                        hostInformationResult =
                            await ResolveMachine.ResolveFqdnToHostInfoAsync(activeMachineWithNetworkInfo.FQDN,
                                preferIPv4: true);
                        if (hostInformationResult.IPv4Addresses != null)
                        {
                            _logger.LogInformation(
                                $"Resolved {activeMachineWithNetworkInfo.FQDN} to {hostInformationResult.IPv4Addresses}");
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
                    if (await ResolveMachine.IsMachineReachableAsync(activeMachineWithNetworkInfo.IPv4) is false)
                    {
                        _logger.LogCritical(
                            $"Machine {activeMachineWithNetworkInfo.IPv4} is not reachable, skipping...");
                        continue;
                    }

                    // Versuche FQDN aus IPv4 zu ermitteln, falls noch nicht vorhanden
                    var hostInfo = await ResolveMachine.ResolveIpToHostInfoAsync(activeMachineWithNetworkInfo.IPv4);
                    if (string.IsNullOrEmpty(hostInfo.HostName) &&
                        string.IsNullOrEmpty(activeMachineWithNetworkInfo.IPv4) is false)
                    {
                        hostInfo.HostName =
                            await ResolveMachine.ResolveIpToFqdnAsync(activeMachineWithNetworkInfo.IPv4);
                        if (string.IsNullOrEmpty(hostInfo.HostName) is false)
                        {
                            _logger.LogInformation(
                                $"Resolved {activeMachineWithNetworkInfo.IPv4} to {hostInfo.HostName}");
                        }
                        else
                        {
                            _logger.LogCritical(
                                $"Machine IP {activeMachineWithNetworkInfo.IPv4} or FQDN {hostInfo.HostName} is not reachable, skipping...");
                            continue;
                        }
                    }
                    else
                    {
                        hostInfo.HostName = activeMachineWithNetworkInfo.FQDN;
                    }

                    using var workerServiceContainer =
                        Services(clientApiFqdn: hostInfo.HostName ?? activeMachineWithNetworkInfo.IPv4);
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
                        AverageProcessingTime = _averageProcessingTime.CalculateAverageProcessingTime(_processedItems,
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
            // catch Network oder eigene Exception
            catch (NetworkInformation.NetworkInformationMissingException networkInformationMissingException)
            {
                _logger.LogError(networkInformationMissingException, $"Machine {networkInformationMissingException.MachineName} has no IPv4, IPv6 or FQDN information");
                _statusWriter.WriteLog($"Error: {networkInformationMissingException.Message}");
                _statusWriter.WriteStatus(new ServiceStatus
                {
                    State = "Error HarvesterWorkerService",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastError = networkInformationMissingException.Message
                });
            }
            catch (NetworkInformation.HostResolutionException hostResolutionException)
            {
            }
            // bei Fehler ggf. hier auch die aktuellen Services und Service-Container disposen!
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in HarvesterWorkerService");
                _statusWriter.WriteLog($"Error: {exception.Message}");
                _statusWriter.WriteStatus(new ServiceStatus
                {
                    State = "Error HarvesterWorkerService",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastError = exception.Message
                });
                // bei Fehler ggf. hier auch die aktuellen Services und Service-Container disposen!
                #if DEBUG
                    await Task.Delay(100, stoppingToken);
                #else
                    await Task.Delay(5000, stoppingToken);
                #endif
            }
        }
    }
}