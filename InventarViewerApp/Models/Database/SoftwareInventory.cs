namespace InventarViewerApp.Models.Database;

public record SoftwareInventoryRecord(
    int Id,
    int MachineId,
    string? ProcessesJson,
    string? InstalledSoftwareJson,
    string? ServicesJson,
    string? EnvironmentJson,
    string? StartupProgramsJson,
    string? RuntimeJson,
    DateTime CreatedAt
);