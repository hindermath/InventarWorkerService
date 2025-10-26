using System.Net.NetworkInformation;

namespace InventarWorkerCommon.Models.Network;

/// <summary>
/// Represents the result of a ping operation, providing information such as
/// success status, round-trip time, target address, and error details (if any).
/// </summary>
public record PingResult
{
    /// <summary>
    /// Indicates whether the ping operation was successful.
    /// This property is set to true if the ping response was received with a status
    /// of <see cref="IPStatus.Success"/>; otherwise, it is set to false.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Represents the status of the ping operation as defined by the <see cref="IPStatus"/> enumeration.
    /// This property indicates the result of the ping attempt, such as success, timeout, or various errors.
    /// </summary>
    public IPStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the round-trip time, in milliseconds, for a ping request and response.
    /// This property represents the duration it takes for the packet to travel to the target
    /// address and back to the sender. A value of 0 indicates that the time was not measured.
    /// </summary>
    public long RoundTripTime { get; set; }

    /// <summary>
    /// Represents the target address that the ping operation was sent to.
    /// This property may include the resolved IP address or null if the address is unavailable.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Provides detailed error information when the ping operation fails.
    /// This property is populated with a descriptive error message if an exception occurs,
    /// or if the operation does not succeed for other reasons.
    /// </summary>
    public string? ErrorMessage { get; set; }
};
