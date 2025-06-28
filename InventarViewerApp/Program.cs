using System;
using System.IO;
using Terminal.Gui;
using InventarViewerApp.Services;
using InventarViewerApp.UI;


namespace InventarViewerApp;

class Program
{
    static void Main(string[] args)
    {
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