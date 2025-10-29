using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Helpers.Calculate;
using InventarWorkerCommon.Helpers.Exceptions;
using InventarWorkerCommon.Models.Network;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using InventarWorkerCommon.Services.Hardware;
using InventarWorkerCommon.Services.Network;
using InventarWorkerCommon.Services.Software;
using InventarWorkerCommon.Services.Status;
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
    private HostInformationResult _hostInformationResult;


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
        
        while (stoppingToken.IsCancellationRequested is false)
        {
            // Abfrage aus der DB von nicht-deaktivierten uns -deprovisionierten Maschinen mit Netzwerkinformationen
            var allActiveMachinesWithNetworkInfo = await _sqliteDbService.GetAllActiveMachinesWithNetworkInfoAsync();

            try
            {
                if (allActiveMachinesWithNetworkInfo is null)
                {
                    throw new ArgumentNullException(nameof(allActiveMachinesWithNetworkInfo));
                }
                else if (allActiveMachinesWithNetworkInfo.Count == 0)
                {
                    throw new InvalidOperationException("No active machines found with network information.");
                }

                foreach (var activeMachineWithNetworkInfo in allActiveMachinesWithNetworkInfo)
                {
                    // hat Machine entweder eine IPv4, IPv6 oder einen FQDN?
                    // Nein throw new Exception (irgendetwas mit Network, dass beim catch-Abfangen kann (oder eigene erstellen)
                    if (string.IsNullOrEmpty(activeMachineWithNetworkInfo.IPv4) is false)
                    {
                        // try-catch
                        _hostInformationResult = await ResolveMachine.ResolveIpToHostInfoAsync(activeMachineWithNetworkInfo.IPv4);
                        // elseif IPv6 elsif FQDN else throw exception

                        // if (await ResolveMachine.IsMachineReachableAsync(activeMachineWithNetworkInfo.IPv4))
                        // {
                        //     _hostInformationResult = await ResolveMachine.ResolveIpToHostInfoAsync(activeMachineWithNetworkInfo.IPv4);
                        // }
                        // else
                        // {
                        //     throw new NetworkInformation.HostResolutionException(activeMachineWithNetworkInfo.IPv4);
                        // }
                    }
                    else
                    {
                        throw new NetworkInformation.NetworkInformationMissingException(activeMachineWithNetworkInfo
                            .Name);
                    }

                    if (string.IsNullOrEmpty(activeMachineWithNetworkInfo.IPv6) is false)
                    {
                        if (await ResolveMachine.IsMachineReachableAsync(activeMachineWithNetworkInfo.IPv4))
                        {
                            _hostInformationResult = await ResolveMachine.ResolveIpToHostInfoAsync(activeMachineWithNetworkInfo.IPv4);
                        }
                        else
                        {
                            throw new NetworkInformation.HostResolutionException(activeMachineWithNetworkInfo.IPv4);
                        }
                    }
                    else
                    {
                        throw new NetworkInformation.NetworkInformationMissingException(activeMachineWithNetworkInfo
                            .Name);
                    }

                    if (string.IsNullOrEmpty(activeMachineWithNetworkInfo.FQDN) is false)
                    {
                        // FQDN2IP
                        if (await ResolveMachine.IsMachineReachableAsync(activeMachineWithNetworkInfo.FQDN))
                        {
                            var iPv4 = await ResolveMachine.ResolveFqdnToIpv4Async(activeMachineWithNetworkInfo.FQDN);
                            _hostInformationResult = await ResolveMachine.ResolveIpToHostInfoAsync(iPv4);
                        }
                        else
                        {
                            throw new NetworkInformation.HostResolutionException(activeMachineWithNetworkInfo.IPv4);
                        }
                    }
                    else
                    {
                        throw new NetworkInformation.NetworkInformationMissingException(activeMachineWithNetworkInfo
                            .Name);
                    }

                    // in der foreach dann jedesmal den Service-Container mit den spezifischen Parametern initialisieren
                    using var workerServiceContainer =
                        Services(clientApiFqdn: _hostInformationResult.HostName ?? activeMachineWithNetworkInfo.IPv4);
                    _apiService = workerServiceContainer.ApiService;

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
                _logger.LogError(networkInformationMissingException,
                    $"Machine {networkInformationMissingException.MachineName} has no IPv4, IPv6 or FQDN information");
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
                _logger.LogError(hostResolutionException,
                    $"Machine {hostResolutionException.HostIdentifier} could not be resolved");
                _statusWriter.WriteLog($"Error: {hostResolutionException.Message}");
                _statusWriter.WriteStatus(new ServiceStatus
                {
                    State = "Error HarvesterWorkerService",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastError = hostResolutionException.Message
                });
            }
            catch (ArgumentNullException argumentNullException)
            {
                _logger.LogError(argumentNullException, "Error in HarvesterWorkerService");
                _statusWriter.WriteLog($"Error: {argumentNullException.Message}");
                _statusWriter.WriteStatus(new ServiceStatus
                {
                    State = "Error HarvesterWorkerService",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastError = argumentNullException.Message
                });
            }
            catch (InvalidOperationException invalidOperationException)
            {
                _logger.LogError(invalidOperationException, "Error in HarvesterWorkerService");
                _statusWriter.WriteLog($"Error: {invalidOperationException.Message}");
                _statusWriter.WriteStatus(new ServiceStatus
                {
                    State = "Error HarvesterWorkerService",
                    StartTime = _startTime,
                    ProcessedItems = _processedItems,
                    LastError = invalidOperationException.Message
                });
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