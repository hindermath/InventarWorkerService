using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;

namespace InventarViewerApp;

partial class Program
{
    private static (ApiService apiService, SqliteDbService dbService, MongoDbService mongoDbService) InitializeServices()
    {
        // API Service initialisieren
        var apiService = new ApiService("http://localhost:5000");
        
        // Database Service initialisieren
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventar.db");
        var dbService = new SqliteDbService($"Data Source={dbPath}");
        
        // Datenbankschema erstellen wenn nötig
        dbService.InitializeDatabase();
        
        // MongoDB Service initialisieren
        var mongoDbService = new MongoDbService("mongodb://localhost:27017");
        
        // MongoDB Datenbank initialisieren
        mongoDbService.InitializeMongoDatabase();
        
        return (apiService, dbService, mongoDbService);
    }
}