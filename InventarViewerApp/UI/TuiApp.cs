using InventarViewerApp.Services;
using InventarViewerApp.UI;
using Terminal.Gui;

namespace InventarViewerApp;

partial class Program
{
    private static void StartTerminalGuiApplication(ApiService apiService, DatabaseService dbService)
    {
        // Terminal.GUI initialisieren
        Application.Init();
        
        try
        {
            // Hauptfenster erstellen
            var mainWindow = new MainWindow(apiService, dbService);
            
            // Hauptfenster zur Anwendung hinzufügen
            Application.Top.Add(mainWindow);
            
            // Anwendung starten
            Application.Run();
        }
        finally
        {
            // Anwendung ordnungsgemäß beenden
            Application.Shutdown();
        }
    }
}