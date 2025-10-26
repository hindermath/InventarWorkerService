namespace InventarWorkerCommon.Models.Network;

/// <summary>
/// Represents information about a host, including its name, associated aliases, address list,
/// and potential error messages when resolving host information. This class is commonly used
/// for network-related tasks where host resolution is required.
/// </summary>
public record HostInformationResult()
{
    /// <summary>
    /// Gets or sets the hostname of a network host.
    /// </summary>
    /// <remarks>
    /// This property represents the resolved hostname of a machine or network device.
    /// It can be used to identify the host within a network environment, typically in conjunction with an IPv4 or IPv6 address.
    /// The value is optional and may be null if the hostname resolution fails or if the hostname is not determined.
    /// </remarks>
    public string? HostName { get; set; }

    /// <summary>
    /// Gets or sets the collection of aliases associated with the host.
    /// </summary>
    /// <remarks>
    /// This property represents alternative names or aliases for the host as resolved during a DNS lookup.
    /// Alias names are typically provided by the DNS server and can include alternative hostnames used
    /// to refer to the same host or machine within a network environment. The collection may be empty if no aliases are resolved
    /// or if the DNS server does not provide alias information.
    /// </remarks>
    public string[] Aliases { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the list of IP addresses associated with a network host.
    /// </summary>
    /// <remarks>
    /// This property contains an array of IP addresses representing the resolved addresses for a specific host.
    /// It may include both IPv4 and IPv6 addresses, depending on the network configuration and resolution process.
    /// The value is populated during host resolution and can be empty if no addresses are resolved or the resolution fails.
    /// </remarks>
    public string[] AddressList { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the list of IPv4 addresses associated with the host.
    /// </summary>
    /// <remarks>
    /// This property contains an array of IPv4 addresses resolved for the specified host.
    /// It provides an easy way to retrieve all IPv4 addresses linked to the host's network identity.
    /// The values in this list may be empty if no IPv4 addresses are resolved or the host does not
    /// have IPv4 configurations.
    /// </remarks>
    public string[] IPv4Addresses { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the list of IPv6 addresses associated with the host.
    /// </summary>
    /// <remarks>
    /// This property contains an array of IP addresses in IPv6 format that are linked to the host.
    /// It is useful when dealing with hosts that operate in an IPv6-enabled network environment.
    /// The value is optional and can be empty if no IPv6 addresses are associated with the host or
    /// if the information is unavailable.
    /// </remarks>
    public string[] IPv6Addresses { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the error message indicating any issues that occurred during the resolution of host information.
    /// </summary>
    /// <remarks>
    /// This property holds a textual description of errors encountered while trying to resolve host information.
    /// It can be used to identify problems such as timeouts, invalid inputs, or resolution failures.
    /// The value is null or empty if no errors occurred during the resolution process.
    /// </remarks>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Indicates whether the host information was successfully resolved.
    /// </summary>
    /// <remarks>
    /// This property returns true if the host resolution was successful, meaning the host name
    /// is not null or empty, and there is no associated error message. A false value indicates
    /// that either the host name could not be resolved or an error occurred during the resolution process.
    /// </remarks>
    public bool IsSuccess => string.IsNullOrEmpty(HostName) is false && string.IsNullOrEmpty(ErrorMessage);
};