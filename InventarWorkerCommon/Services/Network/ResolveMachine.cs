using System.Net.NetworkInformation;
using InventarWorkerCommon.Models.Network;

namespace InventarWorkerCommon.Services.Network;

/// <summary>
/// Provides functionalities to resolve network machines including
/// sending ping requests and retrieving detailed results.
/// </summary>
public static class ResolveMachine
{
    /// <summary>
    /// Sends a ping request to the specified IP address with the specified timeout
    /// and returns detailed information about the ping result.
    /// </summary>
    /// <param name="ipAddress">The target IP address to which the ping request is sent.</param>
    /// <param name="timeout">The maximum time, in milliseconds, to wait for the ping response. Default is 5000ms.</param>
    /// <returns>A <see cref="PingResult"/> containing details about the ping operation, such as success status, round-trip time, and error message if applicable.</returns>
    public static async Task<PingResult> PingWithDetailsAsync(string ipAddress, int timeout = 5000)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(ipAddress, timeout);

            return new PingResult
            {
                IsSuccess = reply.Status == IPStatus.Success,
                Status = reply.Status,
                RoundTripTime = reply.RoundtripTime,
                Address = reply.Address?.ToString()
            };
        }
        catch (Exception ex)
        {
            return new PingResult
            {
                IsSuccess = false,
                Status = IPStatus.Unknown,
                ErrorMessage = ex.Message
            };
        }
    }
}