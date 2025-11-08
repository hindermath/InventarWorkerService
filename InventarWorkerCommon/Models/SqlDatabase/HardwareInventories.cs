namespace InventarWorkerCommon.Models.SqlDatabase;

/// <summary>
/// Represents a hardware inventory record persisted in the SQL database.
/// </summary>
public record HardwareInventories
{
    /// <summary>
    /// Gets or sets the unique identifier of the hardware inventory record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key referencing the related machine record.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the computer name reported during inventory collection.
    /// </summary>
    public string? ComputerName { get; set; }

    /// <summary>
    /// Gets or sets the model name of the computer hardware, if available.
    /// </summary>
    public string? ComputerModel { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer of the computer hardware, if available.
    /// </summary>
    public string? ComputerManufacturer { get; set; }

    /// <summary>
    /// Gets or sets the system architecture (for example, x64, arm64), if available.
    /// </summary>
    public string? Architecture { get; set; }

    /// <summary>
    /// Gets or sets the processor name or model.
    /// </summary>
    public string? ProcessorName { get; set; }

    /// <summary>
    /// Gets or sets the number of physical or logical processor cores, if known.
    /// </summary>
    public int? ProcessorCores { get; set; }

    /// <summary>
    /// Gets or sets the total physical memory in gigabytes, if known.
    /// </summary>
    public double? TotalMemoryGB { get; set; }

    /// <summary>
    /// Gets or sets the available physical memory in gigabytes, if known.
    /// </summary>
    public double? AvailableMemoryGB { get; set; }

    /// <summary>
    /// Gets or sets the percentage of memory in use (0–100), if known.
    /// </summary>
    public double? MemoryUsagePercent { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
};