using System.Net;
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

    /// <summary>
    /// Checks if the specified machine is reachable by sending a ping request
    /// to its IP address within the specified timeout duration.
    /// </summary>
    /// <param name="ipAddress">The target IP address to check for connectivity.</param>
    /// <param name="timeout">The maximum time, in milliseconds, to wait for the ping response. Default is 5000ms.</param>
    /// <returns>A boolean value indicating whether the machine is reachable.</returns>
    public static async Task<bool> IsMachineReachableAsync(string ipAddress, int timeout = 5000)
    {
        var result = await PingWithDetailsAsync(ipAddress, timeout);
        return result.IsSuccess;
    }

    /// <summary>
    /// Resolves the given IP address to its host information, including host name, aliases,
    /// and associated IP address list. Provides detailed error information in case of failure.
    /// </summary>
    /// <param name="ipAddress">The IP address to resolve to a host.</param>
    /// <param name="timeout">The maximum time, in milliseconds, to wait for the host resolution process. Default is 5000ms.</param>
    /// <returns>A <see cref="HostInfo"/> object containing host name, aliases, address list, or an error message if the resolution fails.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided IP address is invalid or cannot be parsed.</exception>
    public static async Task<HostInfo> ResolveIpToHostInfoAsync(string ipAddress, int timeout = 5000)
    {
        try
        {
            using var cts = new CancellationTokenSource(timeout);
            //var ipAddr = IPAddress.Parse(ipAddress);
            // Automatische Erkennung von IPv4 und IPv6
            if (!IPAddress.TryParse(ipAddress, out var ipAddr))
            {
                throw new ArgumentException($"Invalid IP address format: {ipAddress}");
            }

            //var hostEntry = await Dns.GetHostEntryAsync(ipAddr);
            var task = Dns.GetHostEntryAsync(ipAddr);
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token));

            if (completedTask == task && task.IsCompletedSuccessfully)
            {
                return new HostInfo
                {
                    HostName = task.Result.HostName,
                    Aliases = task.Result.Aliases,
                    AddressList = task.Result.AddressList.Select(addr => addr.ToString()).ToArray()
                };
            }
            else
            {
                return new HostInfo
                {
                    ErrorMessage = $"Resolving Machine IP {ipAddress} to host information timed out."
                };
            }
        }
        catch (Exception ex)
        {
            return new HostInfo
            {
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Resolves the specified IP address to its corresponding Fully Qualified Domain Name (FQDN).
    /// </summary>
    /// <param name="ipAddress">The IP address to resolve into an FQDN. Can be an IPv4 or IPv6 address.</param>
    /// <returns>A string representing the FQDN of the specified IP address, or null if the resolution fails.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided IP address is in an invalid format.</exception>
    public static async Task<string?> ResolveIpToFqdnAsync(string ipAddress)
    {
        try
        {
            // Automatische Erkennung von IPv4 und IPv6
            if (!IPAddress.TryParse(ipAddress, out var ipAddr))
            {
                throw new ArgumentException($"Invalid IP address format: {ipAddress}");
            }

            var hostEntry = await Dns.GetHostEntryAsync(ipAddr);
            return hostEntry.HostName;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DNS resolution failed for {ipAddress}: {ex.Message}");
            return null;
        }
    }
}