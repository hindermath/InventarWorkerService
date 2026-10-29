using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Settings;
using InventarWorkerCommon.Models.Software;
using InventarWorkerCommon.Services.Common;
using InventarWorkerCommon.Services.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Prüft die MongoDB-Schreibberechtigung an der gemeinsamen Persistenzgrenze.
/// EN: Verifies the MongoDB write capability at the shared persistence boundary.
/// </summary>
[TestClass]
[DoNotParallelize]
public class MongoDbServiceTest
{
    private const string DatabaseIntegrationCategory = "DatabaseIntegration";
    private static string _serviceStatusDirectory = string.Empty;
    private static string? _integrationConnectionString;

    /// <summary>
    /// DE: Richtet eine isolierte SQLite-Basis für die Factory-Tests ein.
    /// EN: Configures an isolated SQLite base directory for the factory tests.
    /// </summary>
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        _ = context;
        _integrationConnectionString = Environment.GetEnvironmentVariable("MONGODB_TEST_CONNECTION_STRING");
        _serviceStatusDirectory = Path.Combine(Path.GetTempPath(), $"inventar-mongodb-gate-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_serviceStatusDirectory);
        Environment.SetEnvironmentVariable("SERVICESTATUSDIRECTORY", _serviceStatusDirectory);
    }

    /// <summary>
    /// DE: Entfernt die isolierte Testbasis.
    /// EN: Removes the isolated test base directory.
    /// </summary>
    [ClassCleanup]
    public static void ClassCleanup()
    {
        Environment.SetEnvironmentVariable("SERVICESTATUSDIRECTORY", null);
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (Directory.Exists(_serviceStatusDirectory))
        {
            Directory.Delete(_serviceStatusDirectory, recursive: true);
        }
    }

    /// <summary>
    /// DE: Beweist, dass beide Schreibsenken bei deaktivierter Berechtigung vor jedem Datenbankzugriff zurückkehren.
    /// EN: Proves that both write sinks return before database access when the capability is disabled.
    /// </summary>
    [TestMethod]
    public async Task DisabledWrites_DoNotRequireInitializedDatabases()
    {
        var service = new MongoDbService("mongodb://127.0.0.1:1", writeEnabled: false);

        await service.SaveSoftwareInventoryAsync(7, new SoftwareInventory());
        await service.SaveHardwareInventoryAsync(7, new HardwareInventory());

        Assert.IsFalse(service.WriteEnabled);
    }

    /// <summary>
    /// DE: Prüft, dass konfigurierte Schreibberechtigungen unverändert in den MongoDB-Dienst übernommen werden.
    /// EN: Verifies that configured write capabilities are propagated unchanged to the MongoDB service.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ConfiguredFactory_PropagatesMongoDbWriteEnabled(bool writeEnabled)
    {
        var settings = CreateSettings(writeEnabled);

        using var container = Initialize.Services(settings);

        Assert.AreEqual(writeEnabled, container.MongoDbService.WriteEnabled);
    }

    /// <summary>
    /// DE: Der Fallback ohne lesbare Settings bleibt schreibgeschützt.
    /// EN: The fallback without readable settings remains write-disabled.
    /// </summary>
    [TestMethod]
    public void ParameterlessFactory_DisablesMongoDbWrites()
    {
        using var container = Initialize.Services();

        Assert.IsFalse(container.MongoDbService.WriteEnabled);
    }

    /// <summary>
    /// DE: Der bestehende Ein-Parameter-Konstruktor bleibt für explizite direkte Nutzung schreibfähig.
    /// EN: The existing one-parameter constructor remains write-enabled for explicit direct use.
    /// </summary>
    [TestMethod]
    public void CompatibleConstructor_RemainsWriteEnabled()
    {
        var service = new MongoDbService("mongodb://127.0.0.1:1");

        Assert.IsTrue(service.WriteEnabled);
    }

    /// <summary>
    /// DE: Ein deaktivierter Dienst erzeugt auch gegen eine echte MongoDB keine Dokumente; Lesezugriffe bleiben möglich.
    /// EN: A disabled service creates no documents against a real MongoDB while reads remain available.
    /// </summary>
    [TestMethod]
    [TestCategory(DatabaseIntegrationCategory)]
    public async Task DisabledWrites_RealProvider_LeaveCollectionsEmptyAndReadsAvailable()
    {
        var connectionString = RequireIntegrationConnectionString();
        var machineId = Random.Shared.Next(100_000_000, int.MaxValue);
        var service = new MongoDbService(connectionString, writeEnabled: false);
        service.InitializeSoftwareMongoDatabase();
        service.InitializeHardwareMongoDatabase();

        await service.SaveSoftwareInventoryAsync(machineId, new SoftwareInventory());
        await service.SaveHardwareInventoryAsync(machineId, new HardwareInventory());
        var softwareMatches = await service.FindSoftwareByNameAsync(machineId, "not-present");

        var client = new MongoClient(connectionString);
        var softwareCount = await CountDocumentsAsync(client, "SoftwareInventory", machineId);
        var hardwareCount = await CountDocumentsAsync(client, "HardwareInventory", machineId);

        Assert.AreEqual(0L, softwareCount);
        Assert.AreEqual(0L, hardwareCount);
        Assert.AreEqual(0, softwareMatches.Count);
    }

    /// <summary>
    /// DE: Eine explizit aktivierte Instanz bewahrt das bestehende Schreibverhalten beider Senken.
    /// EN: An explicitly enabled instance preserves the existing write behavior of both sinks.
    /// </summary>
    [TestMethod]
    [TestCategory(DatabaseIntegrationCategory)]
    public async Task EnabledWrites_RealProvider_InsertBothDocuments()
    {
        var connectionString = RequireIntegrationConnectionString();
        var machineId = Random.Shared.Next(100_000_000, int.MaxValue);
        var service = new MongoDbService(connectionString, writeEnabled: true);
        service.InitializeSoftwareMongoDatabase();
        service.InitializeHardwareMongoDatabase();

        try
        {
            await service.SaveSoftwareInventoryAsync(machineId, new SoftwareInventory());
            await service.SaveHardwareInventoryAsync(machineId, new HardwareInventory());

            var client = new MongoClient(connectionString);
            Assert.AreEqual(1L, await CountDocumentsAsync(client, "SoftwareInventory", machineId));
            Assert.AreEqual(1L, await CountDocumentsAsync(client, "HardwareInventory", machineId));
        }
        finally
        {
            var client = new MongoClient(connectionString);
            await client.GetDatabase("SoftwareInventory").DropCollectionAsync(machineId.ToString());
            await client.GetDatabase("HardwareInventory").DropCollectionAsync(machineId.ToString());
        }
    }

    private static Settings CreateSettings(bool writeEnabled)
    {
        return new Settings
        {
            ClientApi = new ClientApi
            {
                ClientApiFqdn = "localhost",
                ClientApiPort = "80"
            },
            MongoDb = new MongoDb
            {
                MongoDbFqdn = "127.0.0.1",
                MongoDbPort = "1",
                WriteEnabled = writeEnabled
            },
            PgSqlDb = new PgSqlDb
            {
                WriteEnabled = false
            }
        };
    }

    private static string RequireIntegrationConnectionString()
    {
        if (string.IsNullOrWhiteSpace(_integrationConnectionString))
        {
            Assert.Inconclusive("MONGODB_TEST_CONNECTION_STRING is not configured.");
        }

        return _integrationConnectionString!;
    }

    private static async Task<long> CountDocumentsAsync(IMongoClient client, string databaseName, int machineId)
    {
        var collection = client
            .GetDatabase(databaseName)
            .GetCollection<BsonDocument>(machineId.ToString());
        return await collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);
    }
}
