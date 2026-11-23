using CsvHelper;
using InventarWorkerCommon.Models.SqlDatabase;

namespace InventarWorkerCommon.Services.Csv;

/// <summary>
/// DE: Erhält Header-Aliase und optionale Spalten mit der generierten Zuordnung.
/// EN: Preserves header aliases and optional columns through generated mapping.
/// </summary>
[CsvRecordMap(typeof(MachineFromCsv))]
public partial class MachineMapFromCsv
{
    /// <summary>DE: CSV-Zuordnung für Name. EN: CSV mapping for Name.</summary>
    [CsvColumn("Name", "MachineName", "Computer")]
    public string Name { get; set; } = string.Empty;

    /// <summary>DE: CSV-Zuordnung für OperatingSystem. EN: CSV mapping for OperatingSystem.</summary>
    [CsvColumn("OperatingSystem", "OS", "Platform", Optional = true)]
    public string? OperatingSystem { get; set; }

    /// <summary>DE: CSV-Zuordnung für LastSeen. EN: CSV mapping for LastSeen.</summary>
    [CsvColumn("LastSeen", "LastActivity", "LastOnline", Optional = true)]
    public DateTime? LastSeen { get; set; }

    /// <summary>DE: CSV-Zuordnung für IPv4. EN: CSV mapping for IPv4.</summary>
    [CsvColumn("IPv4", "INET", Optional = true)]
    public string? IPv4 { get; set; }

    /// <summary>DE: CSV-Zuordnung für IPv6. EN: CSV mapping for IPv6.</summary>
    [CsvColumn("IPv6", "INET6", Optional = true)]
    public string? IPv6 { get; set; }

    /// <summary>DE: CSV-Zuordnung für FQDN. EN: CSV mapping for FQDN.</summary>
    [CsvColumn("FQDN", "DNS Name", Optional = true)]
    public string? FQDN { get; set; }

    /// <summary>DE: CSV-Zuordnung für Disabled. EN: CSV mapping for Disabled.</summary>
    [CsvColumn("Disabled", "Disable")]
    public bool Disabled { get; set; }

    /// <summary>DE: CSV-Zuordnung für Deprovisioned. EN: CSV mapping for Deprovisioned.</summary>
    [CsvColumn("Deprovisioned", "Deprovision")]
    public bool Deprovisioned { get; set; }

}
