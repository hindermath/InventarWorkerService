namespace InventarWorkerCommon.Models.SqlDatabase;

/// <summary>
/// Represents a software inventory record persisted in the SQL database. Complex collections
/// are stored as JSON blobs to simplify persistence and retrieval.
/// </summary>
public record SoftwareInventories
{
    /// <summary>
    /// Gets or sets the unique identifier of the software inventory record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key referencing the related machine record.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets a JSON representation of running processes captured during inventory.
    /// </summary>
    public string? ProcessesJson { get; set; }

    /// <summary>
    /// Gets or sets a JSON representation of installed software.
    /// </summary>
    public string? InstalledSoftwareJson { get; set; }

    /// <summary>
    /// Gets or sets a JSON representation of Windows services and their states.
    /// </summary>
    public string? ServicesJson { get; set; }

    /// <summary>
    /// Gets or sets a JSON representation of selected environment variables.
    /// </summary>
    public string? EnvironmentJson { get; set; }

    /// <summary>
    /// Gets or sets a JSON representation of configured startup programs.
    /// </summary>
    public string? StartupProgramsJson { get; set; }

    /// <summary>
    /// Gets or sets a JSON representation of runtime details (for example, .NET/PowerShell versions).
    /// </summary>
    public string? RuntimeJson { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
};