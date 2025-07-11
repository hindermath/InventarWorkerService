using Microsoft.AspNetCore.Mvc;
using InventarViewerApp.Services;
using InventarViewerApp.Models.Database;

namespace InventarViewerApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventarController : ControllerBase
{
    private readonly DatabaseService _databaseService;

    public InventarController(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet("machines")]
    public async Task<ActionResult<List<Machine>>> GetMachines()
    {
        try
        {
            var machines = await _databaseService.GetMachinesAsync();
            return Ok(machines);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Maschinen", error = ex.Message });
        }
    }

    [HttpGet("machines/{machineName}")]
    public async Task<ActionResult<Machine>> GetMachine(string machineName)
    {
        try
        {
            var machine = await _databaseService.GetMachineByNameAsync(machineName);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            return Ok(machine);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Maschine", error = ex.Message });
        }
    }

    [HttpGet("machines/{machineName}/hardware")]
    public async Task<ActionResult> GetHardwareInventory(string machineName)
    {
        try
        {
            var machine = await _databaseService.GetMachineByNameAsync(machineName);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            var hardware = await _databaseService.GetLatestHardwareInventoryAsync(machine.Id);
            if (hardware == null)
                return NotFound(new { message = "Keine Hardware-Inventar-Daten gefunden" });

            return Ok(hardware);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Hardware-Daten", error = ex.Message });
        }
    }

    [HttpGet("machines/{machineName}/software")]
    public async Task<ActionResult> GetSoftwareInventory(string machineName)
    {
        try
        {
            var machine = await _databaseService.GetMachineByNameAsync(machineName);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            var software = await _databaseService.GetLatestSoftwareInventoryAsync(machine.Id);
            if (software == null)
                return NotFound(new { message = "Keine Software-Inventar-Daten gefunden" });

            return Ok(software);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Software-Daten", error = ex.Message });
        }
    }

    [HttpGet("hardware-overview")]
    public async Task<ActionResult> GetHardwareOverview()
    {
        try
        {
            // Nutzen Sie die vorhandene hardwareinventory View
            var machines = await _databaseService.GetMachinesAsync();
            var result = new List<object>();

            foreach (var machine in machines)
            {
                var hardware = await _databaseService.GetLatestHardwareInventoryAsync(machine.Id);
                if (hardware != null)
                {
                    result.Add(new
                    {
                        MachineId = machine.Id,
                        MachineName = machine.Name,
                        OperatingSystem = machine.OperatingSystem,
                        LastSeen = machine.LastSeen,
                        Hardware = hardware
                    });
                }
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Hardware-Übersicht", error = ex.Message });
        }
    }
}