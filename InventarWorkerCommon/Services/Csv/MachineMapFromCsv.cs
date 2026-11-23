using CsvHelper;
using InventarWorkerCommon.Models.SqlDatabase;

namespace InventarWorkerCommon.Services.Csv;

/// <summary>
/// Maps the existing machine CSV header aliases and optional columns with CsvHelper's source generator.
/// </summary>
[CsvRecordMap(typeof(MachineFromCsv))]
public partial class MachineMapFromCsv
{
    /// <summary>Gets or sets the CSV mapping for Name.</summary>
    [CsvColumn("Name", "MachineName", "Computer")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the CSV mapping for OperatingSystem.</summary>
    [CsvColumn("OperatingSystem", "OS", "Platform", Optional = true)]
    public string? OperatingSystem { get; set; }

    /// <summary>Gets or sets the CSV mapping for LastSeen.</summary>
    [CsvColumn("LastSeen", "LastActivity", "LastOnline", Optional = true)]
    public DateTime? LastSeen { get; set; }

    /// <summary>Gets or sets the CSV mapping for IPv4.</summary>
    [CsvColumn("IPv4", "INET", Optional = true)]
    public string? IPv4 { get; set; }

    /// <summary>Gets or sets the CSV mapping for IPv6.</summary>
    [CsvColumn("IPv6", "INET6", Optional = true)]
    public string? IPv6 { get; set; }

    /// <summary>Gets or sets the CSV mapping for FQDN.</summary>
    [CsvColumn("FQDN", "DNS Name", Optional = true)]
    public string? FQDN { get; set; }

    /// <summary>Gets or sets the CSV mapping for Disabled.</summary>
    [CsvColumn("Disabled", "Disable")]
    public bool Disabled { get; set; }

    /// <summary>Gets or sets the CSV mapping for Deprovisioned.</summary>
    [CsvColumn("Deprovisioned", "Deprovision")]
    public bool Deprovisioned { get; set; }

}
