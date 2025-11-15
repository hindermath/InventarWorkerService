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

}

/// <summary>
/// Provides a mapping configuration for the `MachineFromCsv` class to CSV files,
/// defining how properties such as name, operating system, and other machine details
/// correspond to specific column headers in the CSV.
/// </summary>
public class MachineMap : ClassMap<MachineFromCsv>
{
    /// <summary>
    /// Configures the mapping between the `MachineFromCsv` class and CSV file data.
    /// Assigns column names in the CSV file to the corresponding properties in
    /// the `MachineFromCsv` class. Supports multiple possible column header names
    /// for each property to accommodate varying CSV formats. Provides optional
    /// mapping for properties where data may not always be present.
    /// </summary>
    public MachineMap()
    {
        Map(m => m.Name).Name("Name", "MachineName", "Computer");
        Map(m => m.OperatingSystem).Name("OperatingSystem", "OS", "Platform").Optional();
        Map(m => m.LastSeen).Name("LastSeen", "LastActivity", "LastOnline").Optional();
        Map(m => m.IPv4).Name("IPv4", "IPv4").Optional();
        Map(m => m.IPv6).Name("IPv6", "IPv6").Optional();
        Map(m => m.FQDN).Name("FQDN", "FQDN").Optional();
        Map(m => m.Disabled).Name("Disabled", "Disabled");
        Map(m => m.Deprovisioned).Name("Deprovisioned", "Deprovisioned");
    }
}
