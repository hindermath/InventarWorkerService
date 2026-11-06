using Npgsql;

namespace InventarWorkerCommon.Models.Settings;

/// <summary>
/// Represents the configuration settings required to connect to a PostgreSQL database.
/// </summary>
public class PgSqlDb
{
    /// <summary>
    /// Gets or sets the fully qualified domain name (FQDN) of the PostgreSQL server.
    /// </summary>
    public string PgSqlDbFqdn { get; set; } = string.Empty;
    /// <summary>
    /// Gets the port of the PostgreSQL server.
    /// </summary>
    public string PgSqlDbPort { get; set; } = string.Empty;

    /// <summary>
    /// Gets the name of the PostgreSQL database.
    /// </summary>
    public string PgSqlDbName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the username for the PostgreSQL connection.
    /// </summary>
    public string PgSqlUser { get; set; } = string.Empty;

    /// <summary>
    /// Gets the password for the PostgreSQL connection.
    /// </summary>
    public string PgSqlPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional PEM root certificate used to verify a private PostgreSQL CA.
    /// </summary>
    public string PgSqlRootCertificatePath { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the application should write data to PostgreSQL.
    /// </summary>
    public bool WriteEnabled { get; set; }

    /// <summary>
    /// Gets the connection string required to connect to the PostgreSQL database.
    /// The connection string is dynamically constructed using the database host, port, name,
    /// user credentials, and other related properties.
    /// </summary>
    public string PgSqlConnectionString
    {
        get
        {
            var host = string.IsNullOrWhiteSpace(PgSqlDbFqdn) ? "localhost" : PgSqlDbFqdn;
            var dbName = string.IsNullOrWhiteSpace(PgSqlDbName) ? "postgres" : PgSqlDbName;
            var port = string.IsNullOrWhiteSpace(PgSqlDbPort) ? 5432 : ParsePort(PgSqlDbPort);
            var connectionBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Database = dbName
            };

            if (string.IsNullOrWhiteSpace(PgSqlUser) is false && (string.IsNullOrWhiteSpace(PgSqlPassword) is false))
            {
                connectionBuilder.Username = PgSqlUser;
                connectionBuilder.Password = PgSqlPassword;
            }

            if (string.IsNullOrWhiteSpace(PgSqlRootCertificatePath) is false)
            {
                connectionBuilder.RootCertificate = PgSqlRootCertificatePath;
            }

            return PgSqlTransportSecurity.NormalizeConnectionString(connectionBuilder.ConnectionString);
        }
    }

    private static int ParsePort(string value)
    {
        if (!int.TryParse(value, out var port) || port is < 1 or > 65535)
        {
            throw new InvalidOperationException("The PostgreSQL port must be between 1 and 65535.");
        }

        return port;
    }
}
