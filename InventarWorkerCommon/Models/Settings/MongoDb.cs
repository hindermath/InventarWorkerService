namespace InventarWorkerCommon.Models.Settings;

/// <summary>
/// Represents the configuration settings required to connect to a MongoDB database.
/// </summary>
public class MongoDb
{
    /// <summary>
    /// Gets the fully qualified domain name (FQDN) of the MongoDB server.
    /// </summary>
    public string MongoDbFqdn { get; set; } = string.Empty;

    /// <summary>
    /// Gets the port of the MongoDB server.
    /// </summary>
    public string MongoDbPort { get; set; } = string.Empty;

    /// <summary>
    /// Gets the username for the MongoDB connection.
    /// </summary>
    public string MongoDbUser { get; set; } = string.Empty;

    /// <summary>
    /// Gets the password for the MongoDB connection.
    /// </summary>
    public string MongoDbPassword { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the application should write data to MongoDB.
    /// </summary>
    public bool WriteEnabled { get; set; }

    /// <summary>
    /// Gets the connection string used to connect to the MongoDB database, including authentication
    /// credentials (if provided) and host/port information.
    /// </summary>
    public string MongoDbConnectionString
    {
        get
        {
            var host = string.IsNullOrWhiteSpace(MongoDbFqdn) ? "localhost" : MongoDbFqdn;
            var port = string.IsNullOrWhiteSpace(MongoDbPort) ? "27017" : MongoDbPort;

            if (string.IsNullOrWhiteSpace(MongoDbUser) is false && string.IsNullOrWhiteSpace(MongoDbPassword) is false)
            {
                return $"mongodb://{MongoDbUser}:{MongoDbPassword}@{host}:{port}";
            }

            return $"mongodb://{host}:{port}";
        }
    }
}