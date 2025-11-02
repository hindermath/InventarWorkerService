using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Helpers.Calculate;
using InventarWorkerCommon.Helpers.Exceptions;
using InventarWorkerCommon.Models.Network;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.SqlDatabase;
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
    private string _machineName;
    private object _serviceStatus;
    private int _machineId;
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
    /// Handles and logs exceptions with appropriate error messages and status updates.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="stoppingToken">Cancellation token for the delay operation.</param>
    private async Task HandleExceptionAsync(Exception exception, CancellationToken stoppingToken)
    {
        string errorMessage = exception switch
        {
            NetworkInformation.HostResolutionException hostEx => 
                $"Machine {hostEx.HostIdentifier} could not be resolved",
            NetworkInformation.NetworkInformationMissingException netEx => 
                $"Machine {netEx.MachineName} has no IPv4, IPv6 or FQDN information",
            ArgumentNullException argNullEx => $"Argument is null. {argNullEx.Message}",
            ArgumentException argEx => $"Argument is invalid. {argEx.Message}",
            NotSupportedException notSupEx => $"Operation not supported. {notSupEx.Message}",
            JsonException jsonEx => $"JSON parsing error. {jsonEx.Message}",
            InvalidOperationException invOpEx => $"Invalid operation, {invOpEx.Message}",
            _ => $"Error, {exception.Message}"
        };

        _logger.LogError(exception, errorMessage);
        _statusWriter.WriteLog($"Error: {exception.Message}");
        _statusWriter.WriteStatus(new ServiceStatus
        {
            State = "Error",
            StartTime = _startTime,
            ProcessedItems = _processedItems,
            LastError = exception.Message
        });

        #if DEBUG
            await Task.Delay(100, stoppingToken);
        #else
            await Task.Delay(5000, stoppingToken);
        #endif
    }

    /// <summary>
    /// Executes the background loop until cancellation is requested, periodically
    /// updating the service status and writing statistics/logs.
    /// </summary>
    /// <param name="stoppingToken">Token that signals when the service should stop.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var serviceContainer = Services();
        // is not needed here (yet)! _apiService = serviceContainer.ApiService;
        _sqliteDbService = serviceContainer.DbService;
        _mongoDbService = serviceContainer.MongoDbService;

        // Write initial status
        _statusWriter.WriteStatus(new ServiceStatus
        {
            State = "Starting",
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
                    if (string.IsNullOrEmpty(activeMachineWithNetworkInfo.IPv4) is false)
                    {
                        try
                        {
                            _hostInformationResult =
                                await ResolveMachine.ResolveIpToHostInfoAsync(activeMachineWithNetworkInfo.IPv4);
                        }
                        catch (NetworkInformation.HostResolutionException hostResolutionException)
                        {
                            await HandleExceptionAsync(hostResolutionException, stoppingToken);
                        }
                        catch (Exception exception)
                        {
                            await HandleExceptionAsync(exception, stoppingToken);
                        }
                    }
                    else if (string.IsNullOrEmpty(activeMachineWithNetworkInfo.IPv6) is false)
                    {
                        try
                        {
                            _hostInformationResult =
                                await ResolveMachine.ResolveIpToHostInfoAsync(activeMachineWithNetworkInfo.IPv6);
                        }
                        catch (NetworkInformation.HostResolutionException hostResolutionException)
                        {
                            await HandleExceptionAsync(hostResolutionException, stoppingToken);
                        }
                        catch (Exception exception)
                        {
                            await HandleExceptionAsync(exception, stoppingToken);
                        }
                    }
                    else if (string.IsNullOrEmpty(activeMachineWithNetworkInfo.FQDN) is false)
                    {
                        try
                        {
                            var iPv4 = await ResolveMachine.ResolveFqdnToIpv4Async(activeMachineWithNetworkInfo.FQDN);
                            _hostInformationResult = await ResolveMachine.ResolveIpToHostInfoAsync(iPv4);
                        }
                        catch (NetworkInformation.HostResolutionException hostResolutionException)
                        {
                            await HandleExceptionAsync(hostResolutionException, stoppingToken);
                        }
                        catch (Exception exception)
                        {
                            await HandleExceptionAsync(exception, stoppingToken);
                        }
                    }
                    else
                    {
                        throw new NetworkInformation.NetworkInformationMissingException(activeMachineWithNetworkInfo
                            .Name);
                    }

                    using var workerServiceContainer =
                        Services(clientApiFqdn: _hostInformationResult.AddressList.First() ??_hostInformationResult.HostName);
                    _apiService = workerServiceContainer.ApiService;
                    try
                    {
                        _serviceStatus = await _apiService.GetServiceStatusAsync();
                    }
                    catch (Exception exception)
                    {
                        await HandleExceptionAsync(exception, stoppingToken);
                    }

                    try
                    {
                        // convert status to string and deserialize as JsonDocument
                        var serviceStatusString = _serviceStatus.ToString();
                        var serviceStatusJsonDocument = JsonDocument.Parse(serviceStatusString);

                        // Deserialize JsonDocument into Dictionary for easier access
                        var serviceStatusJsonData =
                            JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                                serviceStatusJsonDocument.RootElement.GetRawText(), _jsonOptions);

                        _machineName = serviceStatusJsonData.ContainsKey("machineName") &&
                                       serviceStatusJsonData["machineName"].ValueKind == JsonValueKind.String
                            ? serviceStatusJsonData["machineName"].GetString()
                            : Environment.MachineName;
                    }
                    catch (JsonException jsonException)
                    {
                        await HandleExceptionAsync(jsonException, stoppingToken);
                    }
                    catch (ArgumentNullException argumentNullException)
                    {
                        await HandleExceptionAsync(argumentNullException, stoppingToken);
                    }
                    catch (ArgumentException argumentException)
                    {
                        await HandleExceptionAsync(argumentException, stoppingToken);
                    }
                    catch (NotSupportedException notSupportedException)
                    {
                        await HandleExceptionAsync(notSupportedException, stoppingToken);
                    }
                    catch (InvalidOperationException invalidOperationException)
                    {
                        await HandleExceptionAsync(invalidOperationException, stoppingToken);
                    }
                    catch (Exception exception)
                    {
                        await HandleExceptionAsync(exception, stoppingToken);
                    }

                    // Store machine information in the database
                    var machine = new Machine
                    {
                        Name = _machineName,
                        OperatingSystem = Environment.OSVersion.ToString(),
                        LastSeen = DateTime.UtcNow,
                        IPv4 = _hostInformationResult.IPv4Addresses.FirstOrDefault(),
                        IPv6 = _hostInformationResult.IPv6Addresses.FirstOrDefault(),
                        FQDN = _hostInformationResult.HostName,
                        LastHarvested = DateTime.UtcNow
                    };
                    _machineId = await _sqliteDbService.SaveOrUpdateMachineAsync(machine);

                    if (_machineId > 0)
                    {
                        // api-service abfrage des Software und Hardware Inventars
                        var softwareInventory = await _apiService.GetSoftwareInventoryAsync();
                        var hardwareInventory = await _apiService.GetHardwareInventoryAsync();

                        await _sqliteDbService.SaveSoftwareInventoryAsync(_machineId, softwareInventory);
                        await _mongoDbService.SaveSoftwareInventoryAsync(_machineId, softwareInventory);
                        await _sqliteDbService.SaveHardwareInventoryAsync(_machineId, hardwareInventory);

                        _processedItems++;
                    }

                    // Update status
                    _statusWriter.WriteStatus(new ServiceStatus
                    {
                        State = "Running",
                        StartTime = _startTime,
                        ProcessedItems = _processedItems,
                        LastActivity = DateTime.Now
                    });

                    // Write statistics
                    _statusWriter.WriteStatistics(new ServiceStatistics
                    {
                        TotalProcessedItems = _processedItems,
                        AverageProcessingTime = _averageProcessingTime.CalculateAverageProcessingTime(_processedItems,
                            _startTime),
                        Uptime = DateTime.Now - _startTime,
                        MemoryUsage = GC.GetTotalMemory(false)
                    });

                    var message = $"Inventory completed successfully: {_processedItems} Runs";
                    _logger.LogInformation(message);
                    _statusWriter.WriteLog(message);
#if DEBUG
                    await Task.Delay(100, stoppingToken);
#else
                    await Task.Delay(30000, stoppingToken);
#endif
                }
            }
            catch (NetworkInformation.NetworkInformationMissingException networkInformationMissingException)
            {
                await HandleExceptionAsync(networkInformationMissingException, stoppingToken);
            }
            catch (NetworkInformation.HostResolutionException hostResolutionException)
            {
                await HandleExceptionAsync(hostResolutionException, stoppingToken);
            }
            catch (ArgumentNullException argumentNullException)
            {
                await HandleExceptionAsync(argumentNullException, stoppingToken);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                await HandleExceptionAsync(invalidOperationException, stoppingToken);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(exception, stoppingToken);
            }
        }
    }
}