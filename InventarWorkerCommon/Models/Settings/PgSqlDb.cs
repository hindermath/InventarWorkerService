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
}