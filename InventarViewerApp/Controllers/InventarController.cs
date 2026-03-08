using InventarWorkerCommon.Models.SqlDatabase;
using InventarWorkerCommon.Services.Database;
using Microsoft.AspNetCore.Mvc;

namespace InventarViewerApp.Controllers;

/// <summary>
/// DE: Stellt Endpunkte für das Inventar-Management bereit, mit Fokus auf Maschinen sowie
/// zugehörige Hardware- und Softwaredaten.
/// EN: Provides endpoints for inventory management, focusing on machines and their related
/// hardware and software data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventarController : ControllerBase
{
    private readonly SqliteDbService _sqliteDbService;

    /// <summary>
    /// DE: Initialisiert den Controller mit dem SQLite-Dienst für Maschinen- und Inventarabfragen.
    /// EN: Initializes the controller with the SQLite service for machine and inventory queries.
    /// </summary>
    /// <param name="sqliteDbService">
    /// DE: Datenbankdienst für den Zugriff auf Maschinen-, Hardware- und Softwaredaten.
    /// EN: Database service used to access machine, hardware, and software data.
    /// </param>
    public InventarController(SqliteDbService sqliteDbService)
    {
        _sqliteDbService = sqliteDbService;
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
            var machines = await _sqliteDbService.GetMachinesAsync();
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
            var machine = await _sqliteDbService.GetMachineByIdAsync(id);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            return Ok(machine);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Maschine", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves the name of a machine identified by its unique ID from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the machine whose name is to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains
    /// an <see cref="ActionResult{T}"/> containing the name of the machine as a string,
    /// or an error response if the machine is not found or in case of a failure.</returns>
    [HttpGet("machines/{id:int}/name")]
    public async Task<ActionResult<string>> GetMachineNameByIdAsync(int id)
    {
        try
        {
            var machine = await _sqliteDbService.GetMachineByIdAsync(id);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            return Ok(machine.Name);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen des Maschinen-Namens", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves information about a specific machine based on its name, including details such as
    /// operating system, creation time, and other metadata.
    /// </summary>
    /// <param name="machineName">The name of the machine to retrieve information for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an
    /// <see cref="ActionResult{T}"/> containing a <see cref="Machine"/> object if found, or an error
    /// response in case the machine is not found or an internal error occurs.</returns>
    [HttpGet("machines/{machineName}")]
    public async Task<ActionResult<Machine>> GetMachine(string machineName)
    {
        try
        {
            var machine = await _sqliteDbService.GetMachineByNameAsync(machineName);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            return Ok(machine);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Maschine", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves the ID of a machine by its name from the database.
    /// </summary>
    /// <param name="machineName">The name of the machine for which the ID is being retrieved.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains
    /// an <see cref="ActionResult{T}"/> with the machine's ID if found, or an error response
    /// if the machine is not found or an error occurs.</returns>
    [HttpGet("machines/{machineName}/id")]
    public async Task<ActionResult<int>> GetMachineIdByNameAsync(string machineName)
    {
        try
        {
            var machine = await _sqliteDbService.GetMachineByNameAsync(machineName);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            return Ok(machine.Id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Maschinen-ID", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves the latest hardware inventory for a specific machine identified by its ID.
    /// </summary>
    /// <param name="machineId">The unique identifier of the machine for which the hardware inventory is requested.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains
    /// an <see cref="ActionResult"/> containing the latest hardware inventory details, or an error response if no data is found or a failure occurs.</returns>
    [HttpGet("machines/{machineId:int}/hardware")]
    public async Task<ActionResult> GetHardwareInventory(int machineId)
    {
        try
        {
            var hardware = await _sqliteDbService.GetLatestHardwareInventoryAsync(machineId);
            if (hardware == null)
                return NotFound(new { message = "Keine Hardware-Inventar-Daten gefunden" });

            return Ok(hardware);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Hardware-Daten", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves the hardware inventory details of a specific machine identified by its name,
    /// including the most recent hardware specifications and configurations.
    /// </summary>
    /// <param name="machineName">The name of the machine for which the hardware inventory data is to be fetched.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an
    /// <see cref="ActionResult"/> with the hardware inventory details of the machine, or an error response
    /// in case the machine or its inventory data is not found.</returns>
    [HttpGet("machines/{machineName}/hardware")]
    public async Task<ActionResult> GetHardwareInventory(string machineName)
    {
        try
        {
            var machine = await _sqliteDbService.GetMachineByNameAsync(machineName);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            var hardware = await _sqliteDbService.GetLatestHardwareInventoryAsync(machine.Id);
            if (hardware == null)
                return NotFound(new { message = "Keine Hardware-Inventar-Daten gefunden" });

            return Ok(hardware);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Hardware-Daten", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves the latest software inventory for a specific machine from the database,
    /// including details about the installed software.
    /// </summary>
    /// <param name="machineId">The unique identifier of the machine for which the software inventory is being retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ActionResult"/>
    /// with the software inventory details or an appropriate error response if the data cannot be retrieved.</returns>
    [HttpGet("machines/{machineId:int}/software")]
    public async Task<ActionResult> GetSoftwareInventory(int machineId)
    {
        try
        {
            var software = await _sqliteDbService.GetLatestSoftwareInventoryAsync(machineId);
            if (software == null)
                return NotFound(new { message = "Keine Software-Inventar-Daten gefunden" });

            return Ok(software);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Software-Daten", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves the latest software inventory details for a machine identified by its name.
    /// </summary>
    /// <param name="machineName">The name of the machine for which the software inventory is to be fetched.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ActionResult"/>
    /// with the software inventory data if available, or an appropriate error response in case of a failure.
    /// </returns>
    [HttpGet("machines/{machineName}/software")]
    public async Task<ActionResult> GetSoftwareInventory(string machineName)
    {
        try
        {
            var machine = await _sqliteDbService.GetMachineByNameAsync(machineName);
            if (machine == null)
                return NotFound(new { message = "Maschine nicht gefunden" });

            var software = await _sqliteDbService.GetLatestSoftwareInventoryAsync(machine.Id);
            if (software == null)
                return NotFound(new { message = "Keine Software-Inventar-Daten gefunden" });

            return Ok(software);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Fehler beim Abrufen der Software-Daten", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a detailed hardware overview for all machines, including their IDs, names,
    /// operating systems, last seen timestamps, and the most recent hardware inventory data.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains
    /// an <see cref="ActionResult"/> with a list of objects, where each object represents a machine
    /// and its associated hardware overview, or an error response in case of a failure.</returns>
    [HttpGet("hardware-overview")]
    public async Task<ActionResult> GetHardwareOverview()
    {
        try
        {
            // Nutzen Sie die vorhandene hardwareinventory View
            var machines = await _sqliteDbService.GetMachinesAsync();
            var result = new List<object>();

            foreach (var machine in machines)
            {
                var hardware = await _sqliteDbService.GetLatestHardwareInventoryAsync(machine.Id);
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
