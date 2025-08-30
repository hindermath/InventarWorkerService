namespace InventarWorkerCommon.Models.SqlDatabase;

public record HardwareInventories
{
    public int Id { get; set; }
    public int MachineId { get; set; }
    public string? ComputerName { get; set; }
    public string? ComputerModel { get; set; }
    public string? ComputerManufacturer { get; set; }
    public string? Architecture { get; set; }
    public string? ProcessorName { get; set; }
    public int? ProcessorCores { get; set; }
    public double? TotalMemoryGB { get; set; }
    public double? AvailableMemoryGB { get; set; }
    public double? MemoryUsagePercent { get; set; }
    public DateTime CreatedAt { get; set; }
};