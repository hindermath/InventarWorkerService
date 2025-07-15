namespace InventarViewerApp.Models.Database;

public record HardwareInventoryRecord(
    int Id,
    int MachineId,
    string? ComputerName,
    string? ComputerModel,
    string? ComputerManufacturer,
    string? Architecture,
    string? ProcessorName,
    int? ProcessorCores,
    double? TotalMemoryGB,
    double? AvailableMemoryGB,
    double? MemoryUsagePercent,
    DateTime CreatedAt
);