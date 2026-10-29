using System.Globalization;
using System.Net;
using MongoDB.Driver;

namespace InventarWorkerCommon.Services.Database;

/// <summary>
/// DE: Erstellt und prueft MongoDB-Verbindungen an der gemeinsamen Transportgrenze.
/// EN: Creates and validates MongoDB connections at the shared transport boundary.
/// </summary>
public static class MongoDbTransportSecurity
{
    /// <summary>
    /// DE: Erstellt eine treiberkonforme Verbindungszeichenfolge. Entfernte Ziele verwenden TLS.
    /// EN: Creates a driver-compliant connection string. Remote targets use TLS.
    /// </summary>
    /// <param name="host">DE: MongoDB-Hostname oder IP-Literal. EN: MongoDB host name or IP literal.</param>
    /// <param name="port">DE: MongoDB-Port. EN: MongoDB port.</param>
    /// <param name="username">DE: Optionaler Benutzername. EN: Optional user name.</param>
    /// <param name="password">DE: Optionales Passwort. EN: Optional password.</param>
    /// <returns>DE: Gepruefte MongoDB-Verbindungszeichenfolge. EN: Validated MongoDB connection string.</returns>
    public static string CreateConnectionString(
        string? host,
        string? port,
        string? username = null,
        string? password = null)
    {
        var effectiveHost = NormalizeHost(host);
        var effectivePort = ParsePort(port);
        var serverHost = IPAddress.TryParse(effectiveHost, out var address) &&
                         address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
            ? $"[{effectiveHost}]"
            : effectiveHost;

        try
        {
            var builder = new MongoUrlBuilder
            {
                Server = new MongoServerAddress(serverHost, effectivePort),
                UseTls = !IsLoopbackHost(effectiveHost),
                AllowInsecureTls = false,
                DirectConnection = IsLoopbackHost(effectiveHost)
            };

            // DE: Der Treiber escaped Credentials korrekt; partielle Credentials bleiben wie bisher ungenutzt.
            // EN: The driver escapes credentials correctly; partial credentials remain unused as before.
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                builder.Username = username;
                builder.Password = password;
            }

            return builder.ToString();
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or MongoConfigurationException)
        {
            throw new InvalidOperationException(
                "DE: Die MongoDB-Zieladresse ist ungueltig. EN: The MongoDB target address is invalid.");
        }
    }

    /// <summary>
    /// DE: Parst die wirksamen Treibereinstellungen und weist unsichere Remote-Verbindungen ab.
    /// EN: Parses the effective driver settings and rejects insecure remote connections.
    /// </summary>
    /// <param name="connectionString">DE: MongoDB-Verbindungszeichenfolge. EN: MongoDB connection string.</param>
    /// <returns>DE: Sichere Treibereinstellungen. EN: Secure driver settings.</returns>
    public static MongoClientSettings CreateClientSettings(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "DE: Die MongoDB-Verbindungszeichenfolge fehlt. EN: The MongoDB connection string is missing.");
        }

        MongoUrl url;
        MongoClientSettings settings;
        try
        {
            url = new MongoUrl(connectionString);
            settings = MongoClientSettings.FromUrl(url);
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or MongoConfigurationException)
        {
            throw new InvalidOperationException(
                "DE: Die MongoDB-Verbindungszeichenfolge ist ungueltig. " +
                "EN: The MongoDB connection string is invalid.");
        }

        var hasRemoteServer = url.Servers.Any(server => !IsLoopbackHost(server.Host));
        if (!hasRemoteServer)
        {
            if (!settings.UseTls)
            {
                if (url.Servers.Count() != 1 || !string.IsNullOrWhiteSpace(settings.ReplicaSetName))
                {
                    throw new InvalidOperationException(
                        "DE: MongoDB-Topologieerkennung ohne TLS ist nicht zulaessig. " +
                        "EN: MongoDB topology discovery without TLS is not allowed.");
                }

                // DE: Ein direkter Loopback-Client verhindert spaetere Klartextverbindungen zu entdeckten Hosts.
                // EN: A direct loopback client prevents later plaintext connections to discovered hosts.
                settings.DirectConnection = true;
            }

            return settings;
        }

        if (!settings.UseTls)
        {
            throw new InvalidOperationException(
                "DE: Entfernte MongoDB-Ziele muessen TLS verwenden. " +
                "EN: Remote MongoDB targets must use TLS.");
        }

        if (settings.AllowInsecureTls)
        {
            throw new InvalidOperationException(
                "DE: Entfernte MongoDB-Ziele muessen das Serverzertifikat pruefen. " +
                "EN: Remote MongoDB targets must validate the server certificate.");
        }

        return settings;
    }

    private static string NormalizeHost(string? host)
    {
        var effectiveHost = string.IsNullOrWhiteSpace(host) ? "localhost" : host.Trim();
        if (effectiveHost.Length >= 2 && effectiveHost[0] == '[' && effectiveHost[^1] == ']')
        {
            effectiveHost = effectiveHost[1..^1];
        }

        if (effectiveHost.Length == 0)
        {
            throw new InvalidOperationException(
                "DE: Der MongoDB-Host fehlt. EN: The MongoDB host is missing.");
        }

        return effectiveHost;
    }

    private static int ParsePort(string? port)
    {
        var effectivePort = string.IsNullOrWhiteSpace(port) ? "27017" : port.Trim();
        if (!int.TryParse(effectivePort, NumberStyles.None, CultureInfo.InvariantCulture, out var parsedPort) ||
            parsedPort is < 1 or > 65535)
        {
            throw new InvalidOperationException(
                "DE: Der MongoDB-Port muss zwischen 1 und 65535 liegen. " +
                "EN: The MongoDB port must be between 1 and 65535.");
        }

        return parsedPort;
    }

    private static bool IsLoopbackHost(string host)
    {
        if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!IPAddress.TryParse(host, out var address))
        {
            return false;
        }

        if (IPAddress.IsLoopback(address))
        {
            return true;
        }

        return address.IsIPv4MappedToIPv6 && IPAddress.IsLoopback(address.MapToIPv4());
    }
}
