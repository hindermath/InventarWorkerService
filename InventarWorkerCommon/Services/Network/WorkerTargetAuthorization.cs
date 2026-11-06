using System.Net;
using System.Net.Sockets;

namespace InventarWorkerCommon.Services.Network;

/// <summary>
/// DE: Autorisiert die vom Harvester erreichbaren Worker-IP-Adressen anhand einer expliziten Betreiber-Allowlist.
/// EN: Authorizes Worker IP addresses reachable by the harvester using an explicit operator allowlist.
/// </summary>
public static class WorkerTargetAuthorization
{
    /// <summary>DE: Name der Allowlist-Umgebungsvariablen. EN: Allowlist environment-variable name.</summary>
    public const string AllowedTargetsEnvironmentVariable = "HARVESTER_ALLOWED_WORKER_TARGETS";

    /// <summary>
    /// DE: Prüft eine bereits aufgelöste Literal-IP gegen die Betreiber-Allowlist.
    /// EN: Validates an already resolved literal IP against the operator allowlist.
    /// </summary>
    /// <param name="resolvedAddress">DE: Aufgelöste IPv4- oder IPv6-Adresse. EN: Resolved IPv4 or IPv6 address.</param>
    /// <param name="reason">DE: Fehlersichere Ablehnungsbegründung. EN: Safe rejection reason.</param>
    /// <returns>DE: <c>true</c> nur für explizit erlaubte Ziele. EN: <c>true</c> only for explicitly allowed targets.</returns>
    public static bool IsAuthorized(string resolvedAddress, out string reason)
    {
        if (!IPAddress.TryParse(resolvedAddress, out var address))
        {
            reason = "DE: Die Worker-Zieladresse ist keine Literal-IP. EN: The Worker target address is not a literal IP.";
            return false;
        }

        address = Normalize(address);
        if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any) || address.Equals(IPAddress.Broadcast) || address.IsIPv6Multicast || IsIpv4Multicast(address))
        {
            reason = "DE: Nicht routbare oder Multicast-Zieladressen sind nicht zulässig. EN: Non-routable or multicast target addresses are not allowed.";
            return false;
        }

        var configuredTargets = Environment.GetEnvironmentVariable(AllowedTargetsEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(configuredTargets))
        {
            reason = "DE: Keine Worker-Allowlist ist konfiguriert. EN: No Worker allowlist is configured.";
            return false;
        }

        foreach (var entry in configuredTargets.Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (MatchesNetwork(address, entry))
            {
                reason = string.Empty;
                return true;
            }
        }

        reason = "DE: Die Worker-Zieladresse ist nicht freigegeben. EN: The Worker target address is not approved.";
        return false;
    }

    private static bool MatchesNetwork(IPAddress address, string entry)
    {
        var parts = entry.Split('/', StringSplitOptions.TrimEntries);
        if (parts.Length is < 1 or > 2 || !IPAddress.TryParse(parts[0], out var networkAddress)) return false;
        networkAddress = Normalize(networkAddress);
        if (address.AddressFamily != networkAddress.AddressFamily) return false;

        var maximumPrefixLength = address.AddressFamily == AddressFamily.InterNetwork ? 32 : 128;
        var prefixLength = maximumPrefixLength;
        if (parts.Length == 2 && (!int.TryParse(parts[1], out prefixLength) || prefixLength < 0 || prefixLength > maximumPrefixLength)) return false;

        var addressBytes = address.GetAddressBytes();
        var networkBytes = networkAddress.GetAddressBytes();
        var wholeBytes = prefixLength / 8;
        var remainingBits = prefixLength % 8;
        if (!addressBytes.AsSpan(0, wholeBytes).SequenceEqual(networkBytes.AsSpan(0, wholeBytes))) return false;
        if (remainingBits == 0) return true;

        var mask = (byte)(0xFF << (8 - remainingBits));
        return (addressBytes[wholeBytes] & mask) == (networkBytes[wholeBytes] & mask);
    }

    private static IPAddress Normalize(IPAddress address) => address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;

    private static bool IsIpv4Multicast(IPAddress address) =>
        address.AddressFamily == AddressFamily.InterNetwork && address.GetAddressBytes()[0] is >= 224 and <= 239;
}
