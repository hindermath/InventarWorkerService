namespace InventarWorkerCommon.Models.Database;

public record SoftwareInventories
{
    public int Id { get; set; }
    public int MachineId { get; set; }
    public string? ProcessesJson { get; set; }
    public string? InstalledSoftwareJson { get; set; }
    public string? ServicesJson { get; set; }
    public string? EnvironmentJson { get; set; }
    public string? StartupProgramsJson { get; set; }
    public string? RuntimeJson { get; set; }
    public DateTime CreatedAt { get; set; }
};