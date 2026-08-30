using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using InventarWorkerCommon.Services.Paths;

namespace InventarWorkerCommon.Services.Common;

/// <summary>
/// DE: Stellt Hilfsmethoden bereit, um die gemeinsam genutzten Dienste der Anwendung zu erzeugen.
/// EN: Provides helper methods that create the application's shared services.
/// </summary>
public static class Initialize
{
    /// <summary>
    /// DE: Initialisiert API-, SQLite- und MongoDB-Dienste fuer den Fallback-Pfad ohne Settings-Datei.
    /// PostgreSQL wird in diesem Pfad bewusst nicht initialisiert.
    /// EN: Initializes API, SQLite, and MongoDB services for the fallback path without a settings file.
    /// PostgreSQL is intentionally not initialized on this path.
    /// </summary>
    /// <param name="clientApiFqdn">DE: FQDN der Client-API. EN: FQDN of the client API.</param>
    /// <param name="clientApiPort">DE: Port der Client-API. EN: Port of the client API.</param>
    /// <param name="mongoDbFqdn">DE: FQDN des MongoDB-Servers. EN: FQDN of the MongoDB server.</param>
    /// <param name="mongoDbPort">DE: Port des MongoDB-Servers. EN: Port of the MongoDB server.</param>
    /// <param name="pgSqlDbFqdn">DE: Unbenutzt im Fallback-Pfad, fuer API-Kompatibilitaet beibehalten. EN: Unused on the fallback path, kept for API compatibility.</param>
    /// <param name="pgSqlDbPort">DE: Unbenutzt im Fallback-Pfad, fuer API-Kompatibilitaet beibehalten. EN: Unused on the fallback path, kept for API compatibility.</param>
    /// <param name="pgSqlDbName">DE: Unbenutzt im Fallback-Pfad, fuer API-Kompatibilitaet beibehalten. EN: Unused on the fallback path, kept for API compatibility.</param>
    /// <returns>
    /// DE: Ein Container mit initialisierten Diensten; <see cref="ServiceContainer.PgSqlDbService"/> ist <c>null</c>.
    /// EN: A container with initialized services; <see cref="ServiceContainer.PgSqlDbService"/> is <c>null</c>.
    /// </returns>
    public static ServiceContainer Services(
        string clientApiFqdn = "localhost",
        string clientApiPort = "80",
        string mongoDbFqdn = "localhost",
        string mongoDbPort = "27017",
        string pgSqlDbFqdn = "localhost",
        string pgSqlDbPort = "5432",
        string pgSqlDbName = "inventar")
    {
        var isLoopback = string.Equals(clientApiFqdn, "localhost", StringComparison.OrdinalIgnoreCase) ||
                         System.Net.IPAddress.TryParse(clientApiFqdn, out var address) && System.Net.IPAddress.IsLoopback(address);
        var scheme = isLoopback ? "http" : "https";
        var apiService = new ApiService(
            $"{scheme}://{clientApiFqdn}:{clientApiPort}",
            Environment.GetEnvironmentVariable("INVENTORY_API_KEY"));

        var basePath = GetDbBasePath();
        var dbPath = Path.Combine(basePath, "inventar.db");
        var dbService = new SqliteDbService($"Data Source={dbPath}");
        dbService.InitializeDatabase();

        var mongoDbService = new MongoDbService($"mongodb://{mongoDbFqdn}:{mongoDbPort}");
        mongoDbService.InitializeSoftwareMongoDatabase();
        mongoDbService.InitializeHardwareMongoDatabase();

        _ = pgSqlDbFqdn;
        _ = pgSqlDbPort;
        _ = pgSqlDbName;

        return new ServiceContainer(apiService, dbService, mongoDbService, pgSqlDbService: null);
    }

    /// <summary>
    /// DE: Initialisiert API-, SQLite-, MongoDB- und optional PostgreSQL-Dienste anhand der Einstellungen.
    /// Wenn <c>WriteEnabled</c> fuer PostgreSQL deaktiviert ist, bleibt der PostgreSQL-Dienst <c>null</c>.
    /// EN: Initializes API, SQLite, MongoDB, and optionally PostgreSQL services based on settings.
    /// When PostgreSQL <c>WriteEnabled</c> is disabled, the PostgreSQL service stays <c>null</c>.
    /// </summary>
    /// <param name="settings">
    /// DE: Anwendungseinstellungen fuer API- und Datenbankverbindungen.
    /// EN: Application settings for API and database connections.
    /// </param>
    /// <returns>
    /// DE: Ein Container mit allen verfuegbaren Diensten.
    /// EN: A container with all available services.
    /// </returns>
    public static ServiceContainer Services(InventarWorkerCommon.Models.Settings.Settings settings)
    {
        var apiService = new ApiService(
            settings.ClientApi.ClientApiUrl,
            Environment.GetEnvironmentVariable("INVENTORY_API_KEY"));

        var basePath = GetDbBasePath();
        var dbPath = Path.Combine(basePath, "inventar.db");
        var dbService = new SqliteDbService($"Data Source={dbPath}");
        dbService.InitializeDatabase();

        var mongoDbService = new MongoDbService(settings.MongoDb.MongoDbConnectionString);
        mongoDbService.InitializeSoftwareMongoDatabase();
        mongoDbService.InitializeHardwareMongoDatabase();

        if (!settings.PgSqlDb.WriteEnabled)
        {
            return new ServiceContainer(apiService, dbService, mongoDbService, pgSqlDbService: null);
        }

        var pgSqlDbService = new PgSqlDbService(settings.PgSqlDb.PgSqlConnectionString);
        pgSqlDbService.InitializeDatabase();

        return new ServiceContainer(apiService, dbService, mongoDbService, pgSqlDbService);
    }

    /// <summary>
    /// DE: Liefert das Basisverzeichnis fuer die lokale SQLite-Datenbank und erzeugt es bei Bedarf.
    /// EN: Returns the base directory for the local SQLite database and creates it when needed.
    /// </summary>
    /// <returns>
    /// DE: Vollstaendiger Pfad zum Datenbank-Basisverzeichnis.
    /// EN: Full path to the database base directory.
    /// </returns>
    public static string GetDbBasePath()
    {
        if (ServicePath.ExistsServiceStatusPath(ServicePath.GetServiceStatusPath()) is false)
        {
            var directory = ServicePath.CreateServiceStatusPath(ServicePath.GetServiceStatusPath());
            return directory.FullName;
        }

        return ServicePath.GetServiceStatusPath();
    }
}

/// <summary>
/// DE: Kapselt die gemeinsam erzeugten Dienste und entsorgt sie nach dem Microsoft-Dispose-Pattern.
/// EN: Wraps the shared services and disposes them according to the Microsoft dispose pattern.
/// </summary>
public sealed class ServiceContainer : IDisposable, IAsyncDisposable
{
    private bool _disposed;

    /// <summary>
    /// DE: API-Dienst fuer REST-Zugriffe.
    /// EN: API service for REST access.
    /// </summary>
    public ApiService ApiService { get; }

    /// <summary>
    /// DE: SQLite-Dienst fuer lokale relationale Persistenz.
    /// EN: SQLite service for local relational persistence.
    /// </summary>
    public SqliteDbService DbService { get; }

    /// <summary>
    /// DE: MongoDB-Dienst fuer dokumentenbasierte Persistenz.
    /// EN: MongoDB service for document-based persistence.
    /// </summary>
    public MongoDbService MongoDbService { get; }

    /// <summary>
    /// DE: Optionaler PostgreSQL-Dienst; ist <c>null</c>, wenn PostgreSQL nicht initialisiert wurde.
    /// EN: Optional PostgreSQL service; is <c>null</c> when PostgreSQL was not initialized.
    /// </summary>
    public PgSqlDbService? PgSqlDbService { get; }

    /// <summary>
    /// DE: Erstellt einen neuen Container fuer die gemeinsam genutzten Dienste.
    /// EN: Creates a new container for the shared services.
    /// </summary>
    /// <param name="apiService">DE: API-Dienst. EN: API service.</param>
    /// <param name="dbService">DE: SQLite-Dienst. EN: SQLite service.</param>
    /// <param name="mongoDbService">DE: MongoDB-Dienst. EN: MongoDB service.</param>
    /// <param name="pgSqlDbService">DE: Optionaler PostgreSQL-Dienst. EN: Optional PostgreSQL service.</param>
    internal ServiceContainer(
        ApiService apiService,
        SqliteDbService dbService,
        MongoDbService mongoDbService,
        PgSqlDbService? pgSqlDbService)
    {
        ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        DbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        MongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));
        PgSqlDbService = pgSqlDbService;
    }

    /// <summary>
    /// DE: Gibt alle synchron entsorgbaren Ressourcen frei.
    /// EN: Releases all synchronously disposable resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// DE: Gibt alle asynchron entsorgbaren Ressourcen frei.
    /// EN: Releases all asynchronously disposable resources.
    /// </summary>
    /// <returns>
    /// DE: Asynchroner Dispose-Vorgang.
    /// EN: Asynchronous dispose operation.
    /// </returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing)
        {
            return;
        }

        try
        {
            if (ApiService is IDisposable apiDisposable)
            {
                apiDisposable.Dispose();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error disposing ApiService: {ex.Message}");
        }

        try
        {
            if (DbService is IDisposable dbDisposable)
            {
                dbDisposable.Dispose();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error disposing DbService: {ex.Message}");
        }

        try
        {
            if (MongoDbService is IDisposable mongoDisposable)
            {
                mongoDisposable.Dispose();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error disposing MongoDbService: {ex.Message}");
        }

        try
        {
            if (PgSqlDbService is IDisposable pgSqlDisposable)
            {
                pgSqlDisposable.Dispose();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error disposing PgSqlDbService: {ex.Message}");
        }

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
        {
            return;
        }

        var disposeTasks = new List<Task>();

        if (ApiService is IAsyncDisposable apiAsyncDisposable)
        {
            disposeTasks.Add(apiAsyncDisposable.DisposeAsync().AsTask());
        }
        else if (ApiService is IDisposable apiDisposable)
        {
            disposeTasks.Add(Task.Run(apiDisposable.Dispose));
        }

        if (DbService is IAsyncDisposable dbAsyncDisposable)
        {
            disposeTasks.Add(dbAsyncDisposable.DisposeAsync().AsTask());
        }
        else if (DbService is IDisposable dbDisposable)
        {
            disposeTasks.Add(Task.Run(dbDisposable.Dispose));
        }

        if (MongoDbService is IAsyncDisposable mongoAsyncDisposable)
        {
            disposeTasks.Add(mongoAsyncDisposable.DisposeAsync().AsTask());
        }
        else if (MongoDbService is IDisposable mongoDisposable)
        {
            disposeTasks.Add(Task.Run(mongoDisposable.Dispose));
        }

        if (PgSqlDbService is IAsyncDisposable pgSqlAsyncDisposable)
        {
            disposeTasks.Add(pgSqlAsyncDisposable.DisposeAsync().AsTask());
        }
        else if (PgSqlDbService is IDisposable pgSqlDisposable)
        {
            disposeTasks.Add(Task.Run(pgSqlDisposable.Dispose));
        }

        try
        {
            await Task.WhenAll(disposeTasks).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during async disposal: {ex.Message}");
        }

        _disposed = true;
    }

    /// <summary>
    /// DE: Finalizer als Sicherheitsnetz, falls <see cref="Dispose()"/> nicht aufgerufen wurde.
    /// EN: Finalizer as a safety net when <see cref="Dispose()"/> was not called.
    /// </summary>
    ~ServiceContainer()
    {
        Dispose(disposing: false);
    }
}
