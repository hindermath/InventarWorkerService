using InventarViewerApp.Services;

namespace InventarViewerApp;

partial class Program
{
    private static (ApiService apiService, DatabaseService dbService) InitializeServices()
    {
        // API Service initialisieren
        var apiService = new ApiService("http://localhost:5000");
        
        // Database Service initialisieren
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventar.db");
        var dbService = new DatabaseService($"Data Source={dbPath}");
        
        // Datenbankschema erstellen wenn nötig
        dbService.InitializeDatabase();
        
        return (apiService, dbService);
    }
}