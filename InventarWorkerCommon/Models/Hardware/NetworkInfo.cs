namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Represents network interface details including addressing and throughput counters.
/// </summary>
public record NetworkInfo
{
    /// <summary>
    /// Gets or sets the interface name (for example, "eth0" or "Wi-Fi").
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets a descriptive label provided by the OS for the interface.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the interface type (for example, Ethernet, Wireless).
    /// </summary>
    public string NetworkInterfaceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the operational status (for example, Up, Down).
    /// </summary>
    public string OperationalStatus { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the nominal interface speed in bits per second.
    /// </summary>
    public long Speed { get; set; }

    /// <summary>
    /// Gets or sets the list of IP addresses assigned to the interface.
    /// </summary>
    public List<string> IpAddresses { get; set; } = new();

    /// <summary>
    /// Gets or sets the MAC address of the interface.
    /// </summary>
    public string MacAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of bytes sent via the interface.
    /// </summary>
    public long BytesSent { get; set; }

    /// <summary>
    /// Gets or sets the total number of bytes received by the interface.
    /// </summary>
    public long BytesReceived { get; set; }
}
