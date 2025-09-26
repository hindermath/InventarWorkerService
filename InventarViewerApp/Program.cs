using static InventarViewerApp.WebApi;
using static InventarWorkerCommon.Services.Common.Initialize;

namespace InventarViewerApp;

partial class Program
{
    static async Task Main(string[] args)
    {
        // Check if Web API mode is desired
        if (args.Length > 0 && args[0] == "--api")
        {
            await WebApiAsync(args);
            return;
        }

        // Initialize services
        var (apiService, dbService, mongoDbService) = Services();
        
        // Start Terminal.GUI Application
        TuiApp(apiService, dbService, mongoDbService);
    }
}