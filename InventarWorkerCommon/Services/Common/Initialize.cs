using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using InventarWorkerCommon.Services.Paths;

namespace InventarWorkerCommon.Services.Common;

/// <summary>
/// Provides a static method to initialize and configure essential services used in the application.
/// This includes API service, SQLite database service, and MongoDB service.
/// </summary>
public static class Initialize
{
    /// <summary>
    /// Initializes and configures essential services required by the application, including API service,
    /// SQLite database service, and MongoDB service.
    /// </summary>
    /// <param name="clientApiFqdn">The fully qualified domain name of the client API. The default is "localhost".</param>
    /// <param name="clientApiPort">The port on which the client API is running. The default is "80".</param>
    /// <param name="mongoDbFqdn">The fully qualified domain name of the MongoDB server. The default is "localhost".</param>
    /// <param name="mongoDbPort">The port on which the MongoDB server is running. The default is "27017".</param>
    /// <returns>A disposable container with the initialized instances of services.</returns>
    public static ServiceContainer Services(
        string clientApiFqdn = "localhost",
        string clientApiPort = "80",
        string mongoDbFqdn = "localhost",
        string mongoDbPort = "27017")
    {
        // Initialize API service
        var apiService = new ApiService($"http://{clientApiFqdn}:{clientApiPort}");

        // Initialize database service
        var basePath = GetDbBasePath();
        var dbPath = Path.Combine(basePath, "inventar.db");
        var dbService = new SqliteDbService($"Data Source={dbPath}");
        // Create a database schema if necessary
        dbService.InitializeDatabase();

        // Initialize MongoDB Service
        var mongoDbService = new MongoDbService($"mongodb://{mongoDbFqdn}:{mongoDbPort}");
        // Initialize MongoDB database
        mongoDbService.InitializeMongoDatabase();

        return new ServiceContainer(apiService, dbService, mongoDbService);
    }

    /// <summary>
    /// Retrieves the base path for the database file. If the service status path does not exist, it creates
    /// the required path and returns its full name.
    /// If the service status path already exists, it retrieves and returns the existing path.
    /// </summary>
    /// <returns>The base path for the database file as a string.</returns>
    public static string GetDbBasePath()
    {
        if (ServicePath.ExistsServiceStatusPath(ServicePath.GetServiceStatusPath()) is false)
        {
            var directory = ServicePath.CreateServiceStatusPath(ServicePath.GetServiceStatusPath());
            return directory.FullName;
        }
        else
        {
            return ServicePath.GetServiceStatusPath();
        }
    }
}

/// <summary>
/// Container for disposable services implementing the Microsoft Dispose Pattern.
/// </summary>
public sealed class ServiceContainer : IDisposable, IAsyncDisposable
{
    private bool _disposed = false;

    /// <summary>
    /// Gets the API service instance.
    /// </summary>
    public ApiService ApiService { get; }

    /// <summary>
    /// Gets the SQLite database service instance.
    /// </summary>
    public SqliteDbService DbService { get; }

    /// <summary>
    /// Gets the MongoDB service instance.
    /// </summary>
    public MongoDbService MongoDbService { get; }

    /// <summary>
    /// Initializes a new instance of the ServiceContainer class.
    /// </summary>
    /// <param name="apiService">The API service instance.</param>
    /// <param name="dbService">The database service instance.</param>
    /// <param name="mongoDbService">The MongoDB service instance.</param>
    internal ServiceContainer(ApiService apiService, SqliteDbService dbService, MongoDbService mongoDbService)
    {
        ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        DbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        MongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));
    }

    /// <summary>
    /// Releases all resources used by the ServiceContainer.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously releases all resources used by the ServiceContainer.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Core dispose logic for synchronous disposal.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources; false if called from finalizer.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // Dispose managed resources
            try
            {
                if (ApiService is IDisposable apiDisposable)
                {
                    apiDisposable.Dispose();
                }
            }
            catch (Exception ex)
            {
                // Log exception if logging is available
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
                // Log exception if logging is available
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
                // Log exception if logging is available
                System.Diagnostics.Debug.WriteLine($"Error disposing MongoDbService: {ex.Message}");
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Core dispose logic for asynchronous disposal.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    private async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
            return;

        var disposeTasks = new List<Task>();

        // Dispose API Service asynchronously if it implements IAsyncDisposable
        if (ApiService is IAsyncDisposable apiAsyncDisposable)
        {
            disposeTasks.Add(apiAsyncDisposable.DisposeAsync().AsTask());
        }
        else if (ApiService is IDisposable apiDisposable)
        {
            disposeTasks.Add(Task.Run(() => apiDisposable.Dispose()));
        }

        // Dispose Database Service asynchronously if it implements IAsyncDisposable
        if (DbService is IAsyncDisposable dbAsyncDisposable)
        {
            disposeTasks.Add(dbAsyncDisposable.DisposeAsync().AsTask());
        }
        else if (DbService is IDisposable dbDisposable)
        {
            disposeTasks.Add(Task.Run(() => dbDisposable.Dispose()));
        }

        // Dispose MongoDB Service asynchronously if it implements IAsyncDisposable
        if (MongoDbService is IAsyncDisposable mongoAsyncDisposable)
        {
            disposeTasks.Add(mongoAsyncDisposable.DisposeAsync().AsTask());
        }
        else if (MongoDbService is IDisposable mongoDisposable)
        {
            disposeTasks.Add(Task.Run(() => mongoDisposable.Dispose()));
        }

        try
        {
            await Task.WhenAll(disposeTasks).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Log exception if logging is available
            System.Diagnostics.Debug.WriteLine($"Error during async disposal: {ex.Message}");
        }

        _disposed = true;
    }

    /// <summary>
    /// Finalizer to ensure resources are released even if Dispose is not called.
    /// </summary>
    ~ServiceContainer()
    {
        Dispose(disposing: false);
    }

    /// <summary>
    /// Throws an ObjectDisposedException if the container has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the container is disposed.</exception>
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ServiceContainer));
        }
    }
}