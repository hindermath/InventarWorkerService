using InventarWorkerService.Models.Software;

namespace InventarWorkerService.Models;

public record SoftwareInventory
{
    public DateTime CollectionTime { get; set; } = DateTime.Now;
    public List<SoftwareInfo> InstalledSoftware { get; set; } = new();
    public List<ProcessInfo> RunningProcesses { get; set; } = new();
    public List<ServiceInfo> WindowsServices { get; set; } = new();
    public List<string> StartupPrograms { get; set; } = new();
    public List<string> EnvironmentVariables { get; set; } = new();
    public RuntimeInfo Runtime { get; set; } = new();
}
