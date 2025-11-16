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
    /// <param name="preferIPv4">Indicates whether IPv4 addresses should be prioritized when resolving the FQDN. Default is true.</param>
    /// <param name="timeout">The maximum time, in milliseconds, to wait for the host resolution process. Default is 5000ms.</param>
    /// <returns>A <see cref="HostInformationResult"/> object containing host name, aliases, address list, or an error message if the resolution fails.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided IP address is invalid or cannot be parsed.</exception>
    public static async Task<HostInformationResult> ResolveIpToHostInfoAsync(string ipAddress, bool preferIPv4 = true, int timeout = 5000)
    {
        try
        {
            using var cts = new CancellationTokenSource(timeout);
            // Automatische Erkennung von IPv4 und IPv6
            if (!IPAddress.TryParse(ipAddress, out var ipAddr))
            {
                throw new ArgumentException($"Invalid IP address format: {ipAddress}");
            }

            var task = Dns.GetHostEntryAsync(ipAddr);
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token));

            if (completedTask == task && task.IsCompletedSuccessfully)
            {
                var ipv4Addresses = task.Result.AddressList
                    .Where(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(addr => addr.ToString())
                    .ToArray();

                var ipv6Addresses = task.Result.AddressList
                    .Where(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    .Select(addr => addr.ToString())
                    .ToArray();


                return new HostInformationResult
                {
                    HostName = task.Result.HostName,
                    AddressList = preferIPv4 ? ipv4Addresses : ipv6Addresses,
                    Aliases = task.Result.Aliases,
                    IPv4Addresses = ipv4Addresses,
                    IPv6Addresses = ipv6Addresses
                };
            }
            else
            {
                return new HostInformationResult
                {
                    ErrorMessage = $"Resolving Machine IP {ipAddress} to host information timed out."
                };
            }
        }
        catch (Exception ex)
        {
            return new HostInformationResult
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
            // Automatic detection of IPv4 and IPv6
            if (!IPAddress.TryParse(ipAddress, out var ipAddr))
            {
                throw new ArgumentException($"Invalid IP address format: {ipAddress}");
            }

            var hostEntry = await Dns.GetHostEntryAsync(ipAddr);
            return hostEntry.HostName;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Resolves the specified fully qualified domain name (FQDN) to its corresponding IPv4 address.
    /// </summary>
    /// <param name="fqdn">The fully qualified domain name (FQDN) to resolve.</param>
    /// <returns>The IPv4 address as a string if the resolution is successful; otherwise, null.</returns>
    public static async Task<string?> ResolveFqdnToIpv4Async(string fqdn)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(fqdn);
            var ipv4Address = addresses.FirstOrDefault(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            return ipv4Address?.ToString();
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Resolves the given FQDN (Fully Qualified Domain Name) to its corresponding IPv6 address, if available.
    /// </summary>
    /// <param name="fqdn">The fully qualified domain name to resolve to an IPv6 address.</param>
    /// <returns>A string representation of the resolved IPv6 address if successful; otherwise, returns null if the resolution fails or no IPv6 address is found.</returns>
    public static async Task<string?> ResolveFqdnToIpv6Async(string fqdn)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(fqdn);
            var ipv6Address = addresses.FirstOrDefault(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6);
            return ipv6Address?.ToString();
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Resolves a fully qualified domain name (FQDN) to detailed host information,
    /// including IP addresses, host name, aliases, and error details if applicable.
    /// </summary>
    /// <param name="fqdn">The fully qualified domain name of the target host to resolve.</param>
    /// <param name="preferIPv4">Indicates whether IPv4 addresses should be prioritized when resolving the FQDN. Default is true.</param>
    /// <param name="timeout">The maximum time, in milliseconds, to wait for the resolution process. Default is 5000ms.</param>
    /// <returns>A <see cref="HostInformationResult"/> object containing details about the host, such as IP addresses, aliases, host name, and any error message if an issue occurred.</returns>
    public static async Task<HostInformationResult> ResolveFqdnToHostInfoAsync(string fqdn, bool preferIPv4 = true,
        int timeout = 5000)
    {
        try
        {
            using var cts = new CancellationTokenSource(timeout);

            //var hostEntry = await Dns.GetHostEntryAsync(fqdn);
            var task = Dns.GetHostEntryAsync(fqdn);
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token));

            if (completedTask == task && task.IsCompletedSuccessfully)
            {
                var ipv4Addresses = task.Result.AddressList
                    .Where(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(addr => addr.ToString())
                    .ToArray();

                var ipv6Addresses = task.Result.AddressList
                    .Where(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    .Select(addr => addr.ToString())
                    .ToArray();

                return new HostInformationResult
                {
                    HostName = task.Result.HostName,
                    AddressList = preferIPv4 ? ipv4Addresses : ipv6Addresses,
                    Aliases = task.Result.Aliases,
                    IPv4Addresses = ipv4Addresses,
                    IPv6Addresses = ipv6Addresses
                };

            }
        }
        catch (Exception ex)
        {
            return new HostInformationResult
            {
                ErrorMessage = ex.Message
            };
        }
        return new HostInformationResult
        {
            ErrorMessage = $"Resolving FQDN {fqdn} to host information timed out."
        };
    }
}