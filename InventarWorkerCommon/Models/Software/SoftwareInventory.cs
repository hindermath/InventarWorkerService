namespace InventarWorkerCommon.Models.Software;

/// <summary>
/// Represents a snapshot of software-related information on a machine, including
/// installed applications, running processes, services, startup programs, environment
/// variables, and runtime details.
/// </summary>
public class SoftwareInventory
{
    /// <summary>
    /// Gets or sets the time when this software inventory was collected.
    /// </summary>
    public DateTime CollectionTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the list of installed software discovered on the machine.
    /// </summary>
    public List<SoftwareInfo> InstalledSoftware { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of processes that were running at collection time.
    /// </summary>
    public List<ProcessInfo> RunningProcesses { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of Windows services and their states.
    /// </summary>
    public List<ServiceInfo> WindowsServices { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of configured startup programs.
    /// </summary>
    public List<string> StartupPrograms { get; set; } = new();

    /// <summary>
    /// Gets or sets selected environment variables captured during inventory.
    /// </summary>
    public List<string> EnvironmentVariables { get; set; } = new();

    /// <summary>
    /// Gets or sets runtime details such as .NET and PowerShell versions.
    /// </summary>
    public RuntimeInfo Runtime { get; set; } = new();
}