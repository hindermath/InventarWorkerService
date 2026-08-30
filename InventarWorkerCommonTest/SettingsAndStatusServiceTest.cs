using System.Text.Json;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.Settings;
using InventarWorkerCommon.Services.Common;
using InventarWorkerCommon.Services.Settings;
using InventarWorkerCommon.Services.Status;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Deckt die dateibasierten Settings- und Statusdienste samt Pfadlogik ab.
/// EN: Covers the file-based settings and status services together with the path logic.
/// </summary>
[TestClass]
[DoNotParallelize]
public class SettingsAndStatusServiceTest
{
    private string _serviceStatusRoot = string.Empty;
    private string? _previousServiceStatusDirectory;

    /// <summary>
    /// DE: Richtet pro Test ein isoliertes Service-Status-Verzeichnis ein.
    /// EN: Creates an isolated service status directory for each test.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _previousServiceStatusDirectory = Environment.GetEnvironmentVariable("SERVICESTATUSDIRECTORY");
        _serviceStatusRoot = $"inventar-status-tests-{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable("SERVICESTATUSDIRECTORY", _serviceStatusRoot);
    }

    /// <summary>
    /// DE: Stellt die ursprüngliche Umgebungsvariable wieder her.
    /// EN: Restores the original environment variable.
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        Environment.SetEnvironmentVariable("SERVICESTATUSDIRECTORY", _previousServiceStatusDirectory);
    }

    /// <summary>
    /// DE: Prüft Settings-Writer/-Reader und den asynchronen Dispose-Pfad des ServiceContainers.
    /// EN: Verifies settings writer/reader and the asynchronous dispose path of the service container.
    /// </summary>
    [TestMethod]
    public async Task SettingsServices_ReadWriteAndDisposeAsync_WorkAsExpected()
    {
        var settings = CreateSettings(writeEnabled: false);
        var writer = new SettingsWriter("settings-case");

        writer.WriteSettings(settings);
        writer.WriteSettingsYaml(settings);

        var reader = new SettingsReader("settings-case");
        var readSettings = reader.ReadSettings();

        Assert.IsNotNull(readSettings);
        Assert.AreEqual("api.local", readSettings.ClientApi.ClientApiFqdn);
        Assert.AreEqual("inventar_test", readSettings.PgSqlDb.PgSqlDbName);

        var container = Initialize.Services(settings);
        await container.DisposeAsync();
    }

    /// <summary>
    /// DE: Prüft alle Status-Ausgabeformate, Logs, Statistiken, Metriken und das Einlesen.
    /// EN: Verifies all status output formats, logs, statistics, metrics and reading them back.
    /// </summary>
    [TestMethod]
    public async Task StatusServices_ReadWriteRoundTrip_AllRelevantArtifacts()
    {
        var statusWriter = new ServiceStatusWriter("status-case");
        var statusReader = new ServiceStatusReader("status-case");
        var now = DateTime.Now;
        var status = new ServiceStatus
        {
            State = "Running",
            StartTime = now.AddMinutes(-5),
            LastActivity = now,
            ProcessedItems = 42,
            LastError = string.Empty
        };
        var statistics = new ServiceStatistics
        {
            TotalProcessedItems = 42,
            AverageProcessingTime = 15.5,
            Uptime = TimeSpan.FromMinutes(5),
            MemoryUsage = 1024
        };
        var metrics = new PerformanceMetrics
        {
            CpuUsage = 12.5,
            MemoryUsage = 2048,
            ThreadCount = 4,
            Timestamp = DateTime.UtcNow
        };
        var hardwareInfo = new HardwareInfo
        {
            System = new SystemInfo { MachineName = "StatusMachine", Platform = "macOS", Architecture = "arm64" }
        };

        statusWriter.WriteStatus(status, ServiceStatusOutputFormat.All);
        statusWriter.WriteLog("First entry");
        statusWriter.WriteLog("Second entry");
        statusWriter.WriteStatistics(statistics);
        statusWriter.WritePerformanceMetrics(metrics);
        await statusWriter.WriteHardwareInventory(hardwareInfo);

        var readStatus = statusReader.ReadStatus();
        var readStatistics = statusReader.ReadStatistics();
        var recentLogs = statusReader.ReadRecentLogs(1);

        Assert.IsNotNull(readStatus);
        Assert.AreEqual("Running", readStatus.State);
        Assert.IsTrue(statusReader.IsServiceRunning());
        Assert.IsNotNull(readStatistics);
        Assert.AreEqual(42, readStatistics.TotalProcessedItems);
        CollectionAssert.AreEqual(new List<string> { "Second entry" }, recentLogs.Select(line => line.Split(" - ").Last()).ToList());

        var statusDirectory = Path.Combine(InventarWorkerCommon.Services.Paths.ServicePath.GetServiceStatusPath(), "status-case");
        Assert.IsTrue(File.Exists(Path.Combine(statusDirectory, "status.json")));
        Assert.IsTrue(File.Exists(Path.Combine(statusDirectory, "status.ini")));
        Assert.IsTrue(File.Exists(Path.Combine(statusDirectory, "status.xml")));
        Assert.IsTrue(File.Exists(Path.Combine(statusDirectory, "status.yaml")));
        Assert.IsTrue(File.Exists(Path.Combine(statusDirectory, "statistics.json")));
        Assert.IsTrue(File.Exists(Path.Combine(statusDirectory, "metrics.json")));
        Assert.IsTrue(Directory.GetFiles(statusDirectory, "hardware_inventory_*.json").Length == 1);
    }

    /// <summary>
    /// DE: Prüft die Reader-Fallbacks bei fehlenden Dateien und altem Status.
    /// EN: Verifies reader fallbacks when files are missing and when status is stale.
    /// </summary>
    [TestMethod]
    public void StatusReader_MissingOrStaleFiles_ReturnsSafeFallbacks()
    {
        var missingReader = new ServiceStatusReader("missing-case");

        Assert.IsNull(missingReader.ReadStatus());
        Assert.IsNull(missingReader.ReadStatistics());
        Assert.AreEqual(0, missingReader.ReadRecentLogs().Count);
        Assert.IsFalse(missingReader.IsServiceRunning());

        var statusWriter = new ServiceStatusWriter("stale-case");
        statusWriter.WriteStatus(new ServiceStatus
        {
            State = "Running",
            StartTime = DateTime.Now.AddHours(-1),
            LastActivity = DateTime.Now.AddMinutes(-10),
            ProcessedItems = 1
        });

        var staleReader = new ServiceStatusReader("stale-case");
        Assert.IsFalse(staleReader.IsServiceRunning());
    }

    /// <summary>
    /// DE: Weist Status-Unterverzeichnisse mit Pfadtrennzeichen ab, bevor Dateien geschrieben werden.
    /// EN: Rejects status subdirectories containing path separators before files are written.
    /// </summary>
    [TestMethod]
    public void Writers_PathTraversalName_ThrowsArgumentException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new SettingsWriter("../outside"));
        Assert.ThrowsExactly<ArgumentException>(() => new ServiceStatusWriter("../outside"));
    }

    private static Settings CreateSettings(bool writeEnabled)
    {
        return new Settings
        {
            ClientApi = new ClientApi
            {
                ClientApiFqdn = "api.local",
                ClientApiPort = "5000"
            },
            MongoDb = new MongoDb
            {
                MongoDbFqdn = "localhost",
                MongoDbPort = "27017",
                WriteEnabled = true
            },
            PgSqlDb = new PgSqlDb
            {
                PgSqlDbFqdn = "localhost",
                PgSqlDbPort = "5432",
                PgSqlDbName = "inventar_test",
                PgSqlUser = "inventar",
                PgSqlPassword = "test",
                WriteEnabled = writeEnabled
            }
        };
    }
}
