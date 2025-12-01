namespace InventarWorkerCommon.Models.Settings;

/// <summary>
/// Represents the configuration settings required to connect to a PostgreSQL database.
/// </summary>
public class PgSqlDb
{
    /// <summary>
    /// Gets or sets the fully qualified domain name (FQDN) of the PostgreSQL server.
    /// </summary>
    public string PgSqlDbFqdn { get; set; }
    /// <summary>
    /// Gets the port of the PostgreSQL server.
    /// </summary>
    public string PgSqlDbPort { get; set; }

    /// <summary>
    /// Gets the name of the PostgreSQL database.
    /// </summary>
    public string PgSqlDbName { get; set; }

    /// <summary>
    /// Gets the username for the PostgreSQL connection.
    /// </summary>
    public string PgSqlUser { get; set; }

    /// <summary>
    /// Gets the password for the PostgreSQL connection.
    /// </summary>
    public string PgSqlPassword { get; set; }

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
            var port = string.IsNullOrWhiteSpace(PgSqlDbPort) ? "5432" : PgSqlDbPort;
            var dbName = string.IsNullOrWhiteSpace(PgSqlDbName) ? "postgres" : PgSqlDbName;

            if (string.IsNullOrWhiteSpace(PgSqlUser) is false && (string.IsNullOrWhiteSpace(PgSqlPassword) is false))
            {
                return $"Host={host};Port={port};Database={dbName};Username={PgSqlUser};Password={PgSqlPassword};";
            }

            return $"Host={host};Port={port};Database={dbName};";
        }
    }
}