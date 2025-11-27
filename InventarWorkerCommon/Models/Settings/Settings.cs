using InventarWorkerCommon.Models.Settings;

namespace InventarWorkerCommon.Models.Settings;

/// <summary>
/// Represents the configuration settings for the application.
/// It includes settings for connecting to external services and databases.
/// </summary>
public class Settings
{
    /// <summary>
    /// Represents the configuration settings related to the Client API.
    /// This property contains details necessary for interacting with the Client API,
    /// such as its fully qualified domain name (FQDN) and the port it uses for communication.
    /// </summary>
    public ClientApi ClientApi { get; set; } = new ClientApi();

    /// <summary>
    /// Represents the configuration settings required for connecting to a MongoDB instance.
    /// This property provides details such as the fully qualified domain name (FQDN),
    /// port number, user credentials, and other essential connection parameters for MongoDB.
    /// </summary>
    public MongoDb MongoDb { get; set; } = new MongoDb();

    /// <summary>
    /// Represents the configuration settings related to the PostgreSQL database.
    /// This property holds the necessary details for connecting to a PostgreSQL database,
    /// such as the database name, port, user credentials, and other connection-specific information.
    /// </summary>
    public PgSqlDb PgSqlDb { get; set; } = new PgSqlDb();
}