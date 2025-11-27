namespace InventarWorkerCommon.Models.Settings;

/// <summary>
/// Represents the configuration settings required to connect to a MongoDB database.
/// </summary>
public class MongoDb
{
    /// <summary>
    /// Gets the fully qualified domain name (FQDN) of the MongoDB server.
    /// </summary>
    public string MongoDbFqdn { get; private set; }

    /// <summary>
    /// Gets the port of the MongoDB server.
    /// </summary>
    public string MongoDbPort { get; private set; }

    /// <summary>
    /// Gets the username for the MongoDB connection.
    /// </summary>
    public string MongoDbUser { get; private set; }

    /// <summary>
    /// Gets the password for the MongoDB connection.
    /// </summary>
    public string MongoDbPassword { get; private set; }
}