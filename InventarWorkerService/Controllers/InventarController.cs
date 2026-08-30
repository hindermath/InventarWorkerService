using InventarWorkerCommon.Services.Hardware;
using InventarWorkerCommon.Services.Software;
using Microsoft.AspNetCore.Mvc;

namespace InventarWorkerService.Controllers;

/// <summary>
/// DE: Stellt abgesicherte API-Endpunkte für Hardware-, Software- und kombinierte Inventardaten bereit.
/// EN: Provides secured API endpoints for hardware, software, and combined inventory data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventarController : ControllerBase
{
    private readonly HardwareInventoryService _hardwareService;
    private readonly SoftwareInventoryService _softwareService;
    private readonly ILogger<InventarController> _logger;

    /// <summary>
    /// DE: Initialisiert den Controller mit Inventardiensten und internem Logger.
    /// EN: Initializes the controller with inventory services and an internal logger.
    /// </summary>
    /// <param name="hardwareService">DE: Dienst für Hardwaredaten. EN: Service for hardware data.</param>
    /// <param name="softwareService">DE: Dienst für Softwaredaten. EN: Service for software data.</param>
    /// <param name="logger">
    /// DE: Logger für interne Diagnosen ohne Ausgabe vertraulicher Details an Clients.
    /// EN: Logger for internal diagnostics without exposing sensitive details to clients.
    /// </param>
    public InventarController(
        HardwareInventoryService hardwareService,
        SoftwareInventoryService softwareService,
        ILogger<InventarController> logger)
    {
        _hardwareService = hardwareService;
        _softwareService = softwareService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current hardware inventory of the machine.
    /// </summary>
    /// <returns>HTTP 200 with hardware info on success; HTTP 500 with an error message on failure.</returns>
    [HttpGet("hardware")]
    public async Task<IActionResult> GetHardwareInventory()
    {
        try
        {
            var hardwareInfo = await _hardwareService.GetHardwareInfoAsync();
            return Ok(hardwareInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Inventar-Endpunkt ist intern fehlgeschlagen.");
            return StatusCode(500, new { error = "Interner Serverfehler" });
        }
    }

    /// <summary>
    /// Gets the current software inventory of the machine.
    /// </summary>
    /// <returns>HTTP 200 with software info on success; HTTP 500 with an error message on failure.</returns>
    [HttpGet("software")]
    public async Task<IActionResult> GetSoftwareInventory()
    {
        try
        {
            var softwareInfo = await _softwareService.GetSoftwareInfoAsync();
            return Ok(softwareInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Inventar-Endpunkt ist intern fehlgeschlagen.");
            return StatusCode(500, new { error = "Interner Serverfehler" });
        }
    }

    /// <summary>
    /// Gets a combined hardware and software inventory along with a UTC timestamp.
    /// </summary>
    /// <returns>HTTP 200 with combined inventory; HTTP 500 with an error message on failure.</returns>
    [HttpGet("full")]
    public async Task<IActionResult> GetFullInventory()
    {
        try
        {
            var hardware = await _hardwareService.GetHardwareInfoAsync();
            var software = await _softwareService.GetSoftwareInfoAsync();

            return Ok(new
            {
                Hardware = hardware,
                Software = software,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Inventar-Endpunkt ist intern fehlgeschlagen.");
            return StatusCode(500, new { error = "Interner Serverfehler" });
        }
    }

    /// <summary>
    /// Returns a simple health/status payload for the worker service.
    /// </summary>
    /// <returns>HTTP 200 with status information.</returns>
    [HttpGet("status")]
    public IActionResult GetServiceStatus()
    {
        return Ok(new
        {
            Status = "Running",
            ServiceName = "InventarWorkerService",
            Timestamp = DateTime.UtcNow,
            MachineName = Environment.MachineName
        });
    }
}
