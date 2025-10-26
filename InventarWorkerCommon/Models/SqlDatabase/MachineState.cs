namespace InventarWorkerCommon.Models.SqlDatabase;

/// <summary>
/// Represents the state of a machine within the system. This includes general details about
/// the machine's identity, network configuration, and operational status.
/// </summary>
public record MachineState()
{
    // Initial properties/fields for the machine dataset
    /// <summary>
    /// Gets or sets the unique identifier for the machine.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the machine.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the machine was last seen or contacted.
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
};