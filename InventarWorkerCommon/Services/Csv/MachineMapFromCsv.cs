using CsvHelper.Configuration;
using InventarWorkerCommon.Models.SqlDatabase;

namespace InventarWorkerCommon.Services.Csv;

/// <summary>
/// Provides a mapping configuration for the `MachineFromCsv` class to CSV files,
/// defining how properties such as name, operating system, and other machine details
/// correspond to specific column headers in the CSV.
/// </summary>
public class MachineMapFromCsv : ClassMap<MachineFromCsv>
{
    /// <summary>
    /// Configures the mapping between the `MachineFromCsv` class and CSV file data.
    /// Assigns column names in the CSV file to the corresponding properties in
    /// the `MachineFromCsv` class. Supports multiple possible column header names
    /// for each property to accommodate varying CSV formats. Provides optional
    /// mapping for properties where data may not always be present.
    /// </summary>
    public MachineMapFromCsv()
    {
        Map(m => m.Name).Name("Name", "MachineName", "Computer");
        Map(m => m.OperatingSystem).Name("OperatingSystem", "OS", "Platform").Optional();
        Map(m => m.LastSeen).Name("LastSeen", "LastActivity", "LastOnline").Optional();
        Map(m => m.IPv4).Name("IPv4", "INET").Optional();
        Map(m => m.IPv6).Name("IPv6", "INET6").Optional();
        Map(m => m.FQDN).Name("FQDN", "DNS Name").Optional();
        Map(m => m.Disabled).Name("Disabled", "Disable");
        Map(m => m.Deprovisioned).Name("Deprovisioned", "Deprovision");
    }
}