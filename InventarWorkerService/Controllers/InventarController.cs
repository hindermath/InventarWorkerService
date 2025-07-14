using InventarWorkerService.Services.Hardware;
using InventarWorkerService.Services.Software;
using Microsoft.AspNetCore.Mvc;

namespace InventarWorkerService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventarController : ControllerBase
{
    private readonly HardwareInventoryService _hardwareService;
    private readonly SoftwareInventoryService _softwareService;

    public InventarController(HardwareInventoryService hardwareService, SoftwareInventoryService softwareService)
    {
        _hardwareService = hardwareService;
        _softwareService = softwareService;
    }

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