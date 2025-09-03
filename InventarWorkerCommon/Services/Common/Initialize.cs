using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;

namespace InventarWorkerCommon.Services.Common;

/// <summary>
/// Provides a static method to initialize and configure essential services used in the application.
/// This includes API service, SQLite database service, and MongoDB service.
/// </summary>
public static class Initialize
{
    /// <summary>
    /// Initializes and configures essential services including API service, SQLite database service,
    /// and MongoDB service with optional parameters for hostnames and port configurations.
    /// </summary>
    /// <param name="clientApiFqdn">The fully qualified domain name (FQDN) or hostname of the client API. Defaults to "localhost".</param>
    /// <param name="clientApiPort">The port number used for the client API. Defaults to "5000".</param>
    /// <param name="mongoDbFqdn">The fully qualified domain name (FQDN) or hostname of the MongoDB service. Defaults to "localhost".</param>
    /// <returns>A tuple containing the initialized services: <c>ApiService</c>, <c>SqliteDbService</c>, and <c>MongoDbService</c>.</returns>
    public static (ApiService apiService, SqliteDbService dbService, MongoDbService mongoDbService) Services(
        string clientApiFqdn = "localhost",
        string clientApiPort = "5000",
        string mongoDbFqdn = "localhost")
    {
        // Initialize API service
        var apiService = new ApiService($"http://{clientApiFqdn}:{clientApiPort}");
        
        // Initialize database service
        //var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventar.db");
        var baseDirectory = OperatingSystem.IsWindows()
            ?
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            :
            OperatingSystem.IsMacOS() ? "/Users/Shared" : "/usr/share";
        var dbPath = Path.Combine(baseDirectory, "inventar.db");
        var dbService = new SqliteDbService($"Data Source={dbPath}");
        
        // Create a database schema if necessary
        dbService.InitializeDatabase();
        
        // Initialize MongoDB Service
        var mongoDbService = new MongoDbService($"mongodb://{mongoDbFqdn}:27017");
        
        // Initialize MongoDB database
        mongoDbService.InitializeMongoDatabase();
        
        return (apiService, dbService, mongoDbService);
    }
}