using InventarViewerApp.Services;
using InventarViewerApp.UI;
using Terminal.Gui;
using static InventarViewerApp.API.WebApi;

namespace InventarViewerApp;

class Program
{
    static async Task Main(string[] args)
    {
        // Prüfen ob Web API Modus gewünscht ist
        if (args.Length > 0 && args[0] == "--api")
        {
            await StartWebApiAsync(args);
            return;
        }

        // Initialisiere Services
        var apiService = new ApiService("http://localhost:5000");
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventar.db");
        var dbService = new DatabaseService($"Data Source={dbPath}");
            
        // Erstelle Datenbankschema wenn nötig
        dbService.InitializeDatabase();
            
        // Starte Terminal.GUI Anwendung
        Application.Init();
            
        var mainWindow = new MainWindow(apiService, dbService);
            
        Application.Top.Add(mainWindow);
            
        Application.Run();
        Application.Shutdown();
    }
    


}