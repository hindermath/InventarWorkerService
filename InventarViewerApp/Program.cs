using static InventarViewerApp.WebApi;
using static InventarWorkerCommon.Services.Common.Initialize;

namespace InventarViewerApp;

partial class Program
{
    static async Task Main(string[] args)
    {
        // Prüfen ob Web API Modus gewünscht ist
        if (args.Length > 0 && args[0] == "--api")
        {
            await WebApiAsync(args);
            return;
        }

        // Services initialisieren
        var (apiService, dbService, mongoDbService) = Services();
        
        // Terminal.GUI Anwendung starten
        TuiApp(apiService, dbService, mongoDbService);
    }
}