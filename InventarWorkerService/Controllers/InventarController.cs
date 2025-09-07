using InventarWorkerCommon.Services.Hardware;
using InventarWorkerCommon.Services.Software;
using Microsoft.AspNetCore.Mvc;

namespace InventarWorkerService.Controllers;

/// <summary>
/// Provides inventory-related API endpoints to retrieve hardware, software, and combined inventory data
/// from the worker services.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventarController : ControllerBase
{
    private readonly HardwareInventoryService _hardwareService;
    private readonly SoftwareInventoryService _softwareService;

    /// <summary>
    /// Creates a new instance of the InventarController with required inventory services.
    /// </summary>
    /// <param name="hardwareService">Service used to read hardware inventory information.</param>
    /// <param name="softwareService">Service used to read software inventory information.</param>
    public InventarController(HardwareInventoryService hardwareService, SoftwareInventoryService softwareService)
    {
        _hardwareService = hardwareService;
        _softwareService = softwareService;
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
            return StatusCode(500, new { error = ex.Message });
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
            return StatusCode(500, new { error = ex.Message });
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
            return StatusCode(500, new { error = ex.Message });
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