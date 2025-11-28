using static InventarViewerApp.API.WebApi;
using static InventarWorkerCommon.Services.Common.Initialize;
using InventarWorkerCommon.Services.Settings;

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
        // Services use with automatic Disposal by 'using' Statement
        var settingsReader = new SettingsReader();
        var settings = settingsReader.ReadSettings();

        if (settings == null)
        {
            using var serviceContainer = Services();
            var apiService = serviceContainer.ApiService;
            var dbService = serviceContainer.DbService;
            var mongoDbService = serviceContainer.MongoDbService;
            var pgSqlDbService = serviceContainer.PgSqlDbService;
            // Start Terminal.GUI Application
            TuiApp(apiService, dbService, mongoDbService, pgSqlDbService);
        }
        else
        {
            using var serviceContainer = Services();
            var apiService = serviceContainer.ApiService;
            var dbService = serviceContainer.DbService;
            var mongoDbService = serviceContainer.MongoDbService;
            var pgSqlDbService = serviceContainer.PgSqlDbService;
            // Start Terminal.GUI Application
            TuiApp(apiService, dbService, mongoDbService, pgSqlDbService);
        }
    }
}