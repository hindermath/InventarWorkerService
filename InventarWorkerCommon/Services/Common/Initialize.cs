using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;

namespace InventarWorkerCommon.Services.Common;

public static class Initialize
{
    public static (ApiService apiService, SqliteDbService dbService, MongoDbService mongoDbService) Services(
        string clientApiFqdn = "localhost",
        string clientApiPort = "5000",
        string mongoDbFqdn = "localhost")
    {
        // Initialize API service
        var apiService = new ApiService($"http://{clientApiFqdn}:{clientApiPort}");
        
        // Initialize database service
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventar.db");
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