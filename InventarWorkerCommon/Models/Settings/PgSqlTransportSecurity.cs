using System.Net;
using Npgsql;

namespace InventarWorkerCommon.Models.Settings;

/// <summary>
/// DE: Erzwingt die PostgreSQL-Transportpolicy für lokale Entwicklung und entfernte Bereitstellungen.
/// EN: Enforces the PostgreSQL transport policy for local development and remote deployments.
/// </summary>
public static class PgSqlTransportSecurity
{
    /// <summary>
    /// DE: Liest eine PostgreSQL-Verbindungszeichenfolge und normalisiert sie auf den geforderten Transportschutz.
    /// EN: Parses and normalizes a PostgreSQL connection string to use the required transport protection.
    /// </summary>
    /// <param name="connectionString">DE: Nicht vertrauenswürdige Eingabezeichenfolge. EN: The untrusted connection-string input.</param>
    /// <returns>DE: Normalisierte Zeichenfolge gemäß Transportpolicy. EN: A normalized connection string that follows the transport policy.</returns>
    /// <exception cref="ArgumentException">DE: Wird bei fehlendem oder mehrdeutigem Zielhost ausgelöst. EN: Thrown when the target host is missing or ambiguous.</exception>
    public static string NormalizeConnectionString(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var host = builder.Host;
        if (string.IsNullOrWhiteSpace(host) || host != host.Trim() || host.Contains(',', StringComparison.Ordinal) || host.StartsWith("/", StringComparison.Ordinal))
        {
            throw new ArgumentException("The PostgreSQL host must be one direct hostname or IP address.", nameof(connectionString));
        }

        // DE: Lokale CI-Container benötigen keine übertragene Nutzlast; Remote-Ziele dürfen nie auf Klartext zurückfallen.
        // EN: Local CI containers have no transmitted payload; remote targets must never fall back to plaintext.
        builder.SslMode = IsLoopbackHost(host) ? SslMode.Disable : SslMode.VerifyFull;
#pragma warning disable CS0618 // Npgsql 10 ignores this legacy key; set it explicitly so older providers cannot weaken verification.
        builder.TrustServerCertificate = false;
#pragma warning restore CS0618
        builder.GssEncryptionMode = GssEncryptionMode.Disable;

        return builder.ConnectionString;
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

        if (address.IsIPv4MappedToIPv6)
        {
            address = address.MapToIPv4();
        }

        return IPAddress.IsLoopback(address);
    }
}
