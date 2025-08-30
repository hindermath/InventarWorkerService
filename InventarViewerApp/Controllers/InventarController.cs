using InventarWorkerCommon.Models.Database;
using InventarWorkerCommon.Services.Database;
using Microsoft.AspNetCore.Mvc;

namespace InventarViewerApp.Controllers;

/// <summary>
/// Controller that provides endpoints related to inventory management, focusing on machines
/// and their associated hardware and software details.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventarController : ControllerBase
{
    private readonly DatabaseService _databaseService;

    public InventarController(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    /// <summary>
    /// Retrieves a list of all machines from the database, including their details such as ID,
    /// name, operating system, and timestamps for creation or last seen activity.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains
    /// an <see cref="ActionResult{T}"/> containing a list of <see cref="Machine"/> objects,
    /// or an error response in case of a failure.</returns>
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

    /// <summary>
    /// Retrieves a specific machine by its unique numeric identifier from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the machine to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains
    /// an <see cref="ActionResult{T}"/> containing the <see cref="Machine"/> object if found,
    /// or an error response if the machine does not exist or an unexpected error occurs.</returns>
    [HttpGet("machines/{id:int}")]
    public async Task<ActionResult<Machine>> GetMachine(int id)
    {
        try
        {
            var machine = await _databaseService.GetMachineByIdAsync(id);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            return Ok(machine);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Maschine", error = ex.Message });
        }
    }
    
    [HttpGet("machines/{id:int}/name")]
    public async Task<ActionResult<string>> GetMachineNameByIdAsync(int id)
    {
        try
        {
            var machine = await _databaseService.GetMachineByIdAsync(id);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            return Ok(machine.Name);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen des Maschinen-Namens", error = ex.Message });
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
    
    [HttpGet("machines/{machineName}/id")]
    public async Task<ActionResult<int>> GetMachineIdByNameAsync(string machineName)
    {
        try
        {
            var machine = await _databaseService.GetMachineByNameAsync(machineName);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            return Ok(machine.Id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Maschinen-ID", error = ex.Message });
        }
    }
    
    [HttpGet("machines/{machineId:int}/hardware")]
    public async Task<ActionResult>GetHardwareInventory(int machineId) 
    {
        try
        {
            var hardware = await _databaseService.GetLatestHardwareInventoryAsync(machineId);
            if (hardware == null)
                return NotFound(new { message = "Keine Hardware-Inventar-Daten gefunden" });

            return Ok(hardware);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Hardware-Daten", error = ex.Message });
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

    [HttpGet("machines/{machineId:int}/software")]
    public async Task<ActionResult> GetSoftwareInventory(int machineId)
    {
        try
        {
            var software = await _databaseService.GetLatestSoftwareInventoryAsync(machineId);
            if (software == null)
                return NotFound(new { message = "Keine Software-Inventar-Daten gefunden" });

            return Ok(software);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Software-Daten", error = ex.Message });
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