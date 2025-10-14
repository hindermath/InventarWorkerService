using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using InventarWorkerCommon.Services.Paths;

namespace InventarWorkerCommon.Services.Common;

/// <summary>
/// Provides a static method to initialize and configure essential services used in the application.
/// This includes API service, SQLite database service, and MongoDB service.
/// </summary>
public static class Initialize
{
    /// <summary>
    /// Initializes and configures essential services required by the application, including API service,
    /// SQLite database service, and MongoDB service.
    /// </summary>
    /// <param name="clientApiFqdn">The fully qualified domain name of the client API. The default is "localhost".</param>
    /// <param name="clientApiPort">The port on which the client API is running. The default is "5000".</param>
    /// <param name="mongoDbFqdn">The fully qualified domain name of the MongoDB server. The default is "localhost".</param>
    /// <param name="mongoDbPort">The port on which the MongoDB server is running. The default is "27017".</param>
    /// <returns>A tuple containing the initialized instances of <see cref="ApiService"/>, <see cref="SqliteDbService"/>, and <see cref="MongoDbService"/>.</returns>
    public static (ApiService apiService, SqliteDbService dbService, MongoDbService mongoDbService) Services(
        string clientApiFqdn = "localhost",
        string clientApiPort = "5000",
        string mongoDbFqdn = "localhost",
        string mongoDbPort = "27017")
    {
        // Initialize API service
        var apiService = new ApiService($"http://{clientApiFqdn}:{clientApiPort}");
        
        // Initialize database service
        var basePath = GetDbBasePath(); //BasePath.GetBasePath();
        var dbPath = Path.Combine(basePath, "inventar.db");
        var dbService = new SqliteDbService($"Data Source={dbPath}");
        
        // Create a database schema if necessary
        dbService.InitializeDatabase();
        
        // Initialize MongoDB Service
        var mongoDbService = new MongoDbService($"mongodb://{mongoDbFqdn}:{mongoDbPort}");
        
        // Initialize MongoDB database
        mongoDbService.InitializeMongoDatabase();
        
        return (apiService, dbService, mongoDbService);
    }

    /// <summary>
    /// Retrieves the base path for the database file. If the service status path does not exist, it creates
    /// the required path and returns its full name.
    /// If the service status path already exists, it retrieves and returns the existing path.
    /// </summary>
    /// <returns>The base path for the database file as a string.</returns>
    public static string GetDbBasePath()
    {
        if (ServicePath.ExistsServiceStatusPath(ServicePath.GetServiceStatusPath()) is false)
        {
            var directory = ServicePath.CreateServiceStatusPath(ServicePath.GetServiceStatusPath());
            return directory.FullName;
        }
        else
        {
            return ServicePath.GetServiceStatusPath();
        }
    }
}