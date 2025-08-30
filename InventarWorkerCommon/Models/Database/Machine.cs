namespace InventarWorkerCommon.Models.Database;

public record Machine
{
    // Initial properties/fields for the machine dataset
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? OperatingSystem { get; set; }
    public DateTime? LastSeen { get; set; }
    public DateTime CreatedAt { get; set; }
    // Extended properties/fields for the harvester service
    public string? IPv4 { get; set; }
    public string? IPv6 { get; set; }
    public string? FQDN { get; set; }
    public bool Disabled { get; set; }
    public bool Deprovisioned { get; set; }
    public DateTime? LastHarvested { get; set; }
}