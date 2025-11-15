using CsvHelper.Configuration;
using InventarWorkerCommon.Services.Database;
using InventarWorkerCommon.Models.SqlDatabase;

namespace InventarWorkerCommon.Models.SqlDatabase;
/// <summary>
/// Modell für Machine-Daten aus CSV-Import
/// </summary>
public class MachineFromCsv
{
    /// <summary>
    /// Represents the name of the machine. This property is used to identify the machine
    /// uniquely within the context of the application. It is a required field and
    /// cannot be null or empty.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Represents the operating system installed on the machine. This property holds
    /// information about the platform, which can be used for compatibility checks or
    /// system categorization. It is an optional field and may be null if the information
    /// is not available.
    /// </summary>
    public string? OperatingSystem { get; set; }

    /// <summary>
    /// Represents the last recorded date and time a machine was active or reachable.
    /// This property is used to track the machine's most recent activity and is
    /// optional, allowing null values if the information is not available.
    /// </summary>
    public DateTime? LastSeen { get; set; }

    // Extended properties/fields for the harvester service
    /// <summary>
    /// Gets or sets the IPv4 address associated with the machine.
    /// </summary>
    public string? IPv4 { get; set; }

    /// <summary>
    /// Gets or sets the IPv6 address of the machine.
    /// </summary>
    public string? IPv6 { get; set; }

    /// <summary>
    /// Gets or sets the fully qualified domain name (FQDN) of the machine.
    /// </summary>
    public string? FQDN { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the machine is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the machine has been deprovisioned.
    /// </summary>
    public bool Deprovisioned { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the machine was last harvested by the harvester service.
    /// </summary>
    public DateTime? LastHarvested { get; set; }
}

/// <summary>
/// CSV-Mapping für Machine-Import
/// </summary>
public class MachineMap : ClassMap<SqlDatabase.MachineFromCsv>
{
    /// <summary>
    /// Defines the mapping configuration for CSV import of machine data, enabling fields such as
    /// name, operating system, and last seen date to map to specific CSV column headers.
    /// </summary>
    public MachineMap()
    {
        Map(m => m.Name).Name("Name", "MachineName", "Computer");
        Map(m => m.OperatingSystem).Name("OperatingSystem", "OS", "Platform");
        Map(m => m.LastSeen).Name("LastSeen", "LastActivity", "LastOnline").Optional();
    }
}
