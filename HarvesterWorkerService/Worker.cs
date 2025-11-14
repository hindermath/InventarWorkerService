using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Helpers.Calculate;
using InventarWorkerCommon.Helpers.Exceptions;
using InventarWorkerCommon.Models.Network;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.SqlDatabase;
using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using InventarWorkerCommon.Services.Network;
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
    private readonly AverageProcessingTime _averageProcessingTime = new();
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly DateTime _startTime = DateTime.Now;
    private int _processedItems;
    private int _nonProcessedItems;
    private string _machineName;
    private object _serviceStatus;
    private int _machineId;
    private ApiService _apiService;
    private SqliteDbService _sqliteDbService;
    private MongoDbService _mongoDbService;
    private HostInformationResult _hostInformationResult;

    /// <summary>
    /// Represents a background worker responsible for handling periodic tasks within the HarvesterWorkerService,
    /// such as writing service status, managing statistics, and logging activities.
    /// </summary>
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };
    }

    /// <summary>
    /// Handles the given exception by logging the error details and updating the service status.
    /// </summary>
    /// <param name="exception">The exception to be handled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task HandleExceptionAsync(Exception exception)
    {
        string errorMessage = exception switch
        {
            NetworkInformation.HostNetworkInformationCannotResolveException hostEx =>
                $"Machine {hostEx.MachineName} could not be resolved.",
            NetworkInformation.NetworkInformationMissingException netEx => 
                $"Machine {netEx.MachineName} has no IPv4, IPv6 or FQDN information.",
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
    }

    /// <summary>
    /// Executes the background loop until cancellation is requested, periodically
    /// updating the service status and writing statistics/logs.
    /// </summary>
    /// <param name="stoppingToken">Token that signals when the service should stop.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var serviceContainer = Services();
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
            var startProcessingTime = DateTime.Now;
            // Query from the DB of non-deactivated un-deprovisioned machines with network information
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
                        catch (NetworkInformation.HostNetworkInformationCannotResolveException hostResolutionException)
                        {
                            await HandleExceptionAsync(hostResolutionException);
                        }
                        catch (Exception exception)
                        {
                            await HandleExceptionAsync(exception);
                        }
                    }
                    else if (string.IsNullOrEmpty(activeMachineWithNetworkInfo.IPv6) is false)
                    {
                        try
                        {
                            _hostInformationResult =
                                await ResolveMachine.ResolveIpToHostInfoAsync(activeMachineWithNetworkInfo.IPv6);
                        }
                        catch (NetworkInformation.HostNetworkInformationCannotResolveException hostResolutionException)
                        {
                            await HandleExceptionAsync(hostResolutionException);
                        }
                        catch (Exception exception)
                        {
                            await HandleExceptionAsync(exception);
                        }
                    }
                    else if (string.IsNullOrEmpty(activeMachineWithNetworkInfo.FQDN) is false)
                    {
                        try
                        {
                            var iPv4 = await ResolveMachine.ResolveFqdnToIpv4Async(activeMachineWithNetworkInfo.FQDN);
                            _hostInformationResult = await ResolveMachine.ResolveIpToHostInfoAsync(iPv4);
                        }
                        catch (NetworkInformation.HostNetworkInformationCannotResolveException hostResolutionException)
                        {
                            await HandleExceptionAsync(hostResolutionException);
                        }
                        catch (Exception exception)
                        {
                            await HandleExceptionAsync(exception);
                        }
                    }
                    else
                    {
                        throw new NetworkInformation.NetworkInformationMissingException(activeMachineWithNetworkInfo
                            .Name);
                    }

                    try
                    {
                        if (_hostInformationResult.IsSuccess is false)
                        {
                            throw new NetworkInformation.HostNetworkInformationCannotResolveException(_hostInformationResult.HostName);
                        }

                        await using var workerServiceContainer =
                            Services(clientApiFqdn: _hostInformationResult.AddressList.First() ??_hostInformationResult.HostName);
                        _apiService = workerServiceContainer.ApiService;
                        _serviceStatus = await _apiService.GetServiceStatusAsync();

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
                        _machineId = await _sqliteDbService.SaveOrUpdateMachineAsync(machine, isHarvester: true);

                    }
                    catch (JsonException jsonException)
                    {
                        await HandleExceptionAsync(jsonException);
                    }
                    catch (ArgumentNullException argumentNullException)
                    {
                        await HandleExceptionAsync(argumentNullException);
                    }
                    catch (ArgumentException argumentException)
                    {
                        await HandleExceptionAsync(argumentException);
                    }
                    catch (NotSupportedException notSupportedException)
                    {
                        await HandleExceptionAsync(notSupportedException);
                    }
                    catch (InvalidOperationException invalidOperationException)
                    {
                        await HandleExceptionAsync(invalidOperationException);
                    }
                    catch (Exception exception)
                    {
                        await HandleExceptionAsync(exception);
                    }

                    if (_machineId > 0)
                    {
                        // Software and Hardware Inventory Query
                        var softwareInventory = await _apiService.GetSoftwareInventoryAsync();
                        var hardwareInventory = await _apiService.GetHardwareInventoryAsync();

                        await _sqliteDbService.SaveSoftwareInventoryAsync(_machineId, softwareInventory);
                        await _sqliteDbService.SaveHardwareInventoryAsync(_machineId, hardwareInventory);
                        await _mongoDbService.SaveSoftwareInventoryAsync(_machineId, softwareInventory);
                        await _mongoDbService.SaveHardwareInventoryAsync(_machineId, hardwareInventory);

                        _processedItems++;

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

                        var message = $"Collecting inventory completed successfully: {_processedItems} runs";
                        _logger.LogInformation(message);
                        _statusWriter.WriteLog(message);

                    }
                    else
                    {
                        _nonProcessedItems++;

                        var message = $"Collecting inventory incomplete!!! {_nonProcessedItems} runs";
                        _logger.LogError(message);
                        _statusWriter.WriteLog(message);
                    }
                }
            }
            catch (NetworkInformation.NetworkInformationMissingException networkInformationMissingException)
            {
                await HandleExceptionAsync(networkInformationMissingException);
            }
            catch (NetworkInformation.HostNetworkInformationCannotResolveException hostResolutionException)
            {
                await HandleExceptionAsync(hostResolutionException);
            }
            catch (ArgumentNullException argumentNullException)
            {
                await HandleExceptionAsync(argumentNullException);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                await HandleExceptionAsync(invalidOperationException);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(exception);
            }
#if DEBUG
            await Task.Delay(Convert.ToInt32((30_000 - Convert.ToInt32((DateTime.Now - startProcessingTime).TotalMilliseconds))), stoppingToken);
#else
            // 86,400,000 ms = 24h - Minus the milliseconds difference
            // of the time consumed for processing the machine table
            await Task.Delay(
                Convert.ToInt32((86_400_000 - Convert.ToInt32((DateTime.Now - startProcessingTime).TotalMilliseconds))),
                stoppingToken);
#endif

        }

        _statusWriter.WriteStatus(new ServiceStatus
        {
            State = "Stopped",
            StartTime = _startTime,
            ProcessedItems = _processedItems,
            LastActivity = DateTime.Now
        });
    }
}