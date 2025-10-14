using InventarWorkerCommon.Services.Common;
using static InventarViewerApp.API.WebApi;
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
        //var (apiService, dbService, mongoDbService) = Services();

        // Services use...
        // Automatic Disposal by 'using' Statement
        using var serviceContainer = Initialize.Services();
        var apiService = serviceContainer.ApiService;
        var dbService = serviceContainer.DbService;
        var mongoDbService = serviceContainer.MongoDbService;
        // Start Terminal.GUI Application
        TuiApp(apiService, dbService, mongoDbService);
    }
}