using System.Text.Json;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.Settings;
using InventarWorkerCommon.Services.Common;
using InventarWorkerCommon.Services.Settings;
using InventarWorkerCommon.Services.Status;
using InventarWorkerCommon.Services.Paths;

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
        BasePath.UnixBasePathOverride = Path.Combine(Path.GetTempPath(), $"inventar-base-tests-{Guid.NewGuid():N}");
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
        var testBasePath = BasePath.UnixBasePathOverride;
        BasePath.UnixBasePathOverride = null;
        // DE: Dispose gibt SQLite-Verbindungen in den Pool zurueck; vor dem Loeschen Handles schliessen.
        // EN: Dispose returns SQLite connections to the pool; close handles before deleting files.
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (!string.IsNullOrWhiteSpace(testBasePath) && Directory.Exists(testBasePath))
        {
            Directory.Delete(testBasePath, recursive: true);
        }
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

        var settingsDirectory = Path.Combine(
            InventarWorkerCommon.Services.Paths.ServicePath.GetServiceStatusPath(),
            "settings-case");
        var jsonPath = Path.Combine(settingsDirectory, "settings.json");
        var yamlPath = Path.Combine(settingsDirectory, "settings.yaml");

        Assert.IsTrue(File.Exists(jsonPath));
        Assert.IsTrue(File.Exists(yamlPath));

        if (!OperatingSystem.IsWindows())
        {
            const UnixFileMode ownerReadWrite = UnixFileMode.UserRead | UnixFileMode.UserWrite;
            var legacyMode = ownerReadWrite | UnixFileMode.GroupRead | UnixFileMode.OtherRead;

            Assert.AreEqual(ownerReadWrite, File.GetUnixFileMode(jsonPath));
            Assert.AreEqual(ownerReadWrite, File.GetUnixFileMode(yamlPath));

            File.SetUnixFileMode(jsonPath, legacyMode);
            File.SetUnixFileMode(yamlPath, legacyMode);

            writer.WriteSettings(settings);
            writer.WriteSettingsYaml(settings);

            Assert.AreEqual(ownerReadWrite, File.GetUnixFileMode(jsonPath));
            Assert.AreEqual(ownerReadWrite, File.GetUnixFileMode(yamlPath));

            File.SetUnixFileMode(jsonPath, legacyMode);
            File.SetUnixFileMode(yamlPath, legacyMode);
        }

        var legacyJsonTemporaryPath = Path.Combine(settingsDirectory, $".settings.json.{Guid.NewGuid():N}.tmp");
        var legacyYamlTemporaryPath = Path.Combine(settingsDirectory, $".settings.yaml.{Guid.NewGuid():N}.tmp");
        File.WriteAllText(legacyJsonTemporaryPath, "legacy-json-marker");
        File.WriteAllText(legacyYamlTemporaryPath, "legacy-yaml-marker");

        var reader = new SettingsReader("settings-case");
        var readSettings = reader.ReadSettings();

        Assert.IsFalse(File.Exists(legacyJsonTemporaryPath));
        Assert.IsFalse(File.Exists(legacyYamlTemporaryPath));
        Assert.AreEqual(0, Directory.GetFiles(settingsDirectory, ".settings.*.tmp").Length);
        if (!OperatingSystem.IsWindows())
        {
            const UnixFileMode ownerReadWrite = UnixFileMode.UserRead | UnixFileMode.UserWrite;
            Assert.AreEqual(ownerReadWrite, File.GetUnixFileMode(jsonPath));
            Assert.AreEqual(ownerReadWrite, File.GetUnixFileMode(yamlPath));
        }

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
        Assert.ThrowsExactly<ArgumentException>(() => new SettingsReader("../outside"));
        Assert.ThrowsExactly<ArgumentException>(() => new ServiceStatusReader("../outside"));
    }

    /// <summary>
    /// DE: Lehnt eine verwurzelte oder traversierende Zustandsstamm-Umgebung ab, bevor ein Dienstpfad gebildet wird.
    /// EN: Rejects a rooted or traversing state-root environment value before a service path is formed.
    /// </summary>
    [TestMethod]
    public void ServicePath_UnsafeEnvironmentDirectoryName_ThrowsArgumentException()
    {
        Environment.SetEnvironmentVariable("SERVICESTATUSDIRECTORY", "../outside");

        Assert.ThrowsExactly<ArgumentException>(() => ServicePath.GetServiceStatusPath());
    }

    /// <summary>
    /// DE: Lehnt einen vorab platzierten symbolischen Link anstelle des Dienstzustandsstamms ab.
    /// EN: Rejects a pre-positioned symbolic link in place of the service-state root.
    /// </summary>
    [TestMethod]
    public void ServicePath_SymlinkedStateRootOnUnix_ThrowsUnauthorizedAccessException()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var stateRoot = ServicePath.GetServiceStatusPath();
        var targetDirectory = Path.Combine(Path.GetTempPath(), $"inventar-state-target-{Guid.NewGuid():N}");
        Directory.CreateDirectory(BasePath.GetBasePath());
        Directory.CreateDirectory(targetDirectory);
        Directory.CreateSymbolicLink(stateRoot, targetDirectory);

        try
        {
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => new ServiceStatusWriter("status-case"));
        }
        finally
        {
            Directory.Delete(stateRoot);
            Directory.Delete(targetDirectory);
        }
    }

    /// <summary>
    /// DE: Lehnt einen symbolischen Link für ein vorhersehbares Statusartefakt ab und verändert dessen Ziel nicht.
    /// EN: Rejects a symbolic link for a predictable status artifact and leaves its target unchanged.
    /// </summary>
    [TestMethod]
    public void ServiceStatusWriter_SymlinkedStatusFileOnUnix_ThrowsUnauthorizedAccessException()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var writer = new ServiceStatusWriter("status-symlink-case");
        var statusDirectory = Path.Combine(ServicePath.GetServiceStatusPath(), "status-symlink-case");
        var targetFile = Path.Combine(Path.GetTempPath(), $"inventar-status-target-{Guid.NewGuid():N}.json");
        var statusFile = Path.Combine(statusDirectory, "status.json");
        File.WriteAllText(targetFile, "sentinel");
        File.CreateSymbolicLink(statusFile, targetFile);

        try
        {
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => writer.WriteStatus(new ServiceStatus { State = "Running" }));
            Assert.AreEqual("sentinel", File.ReadAllText(targetFile));
        }
        finally
        {
            File.Delete(statusFile);
            File.Delete(targetFile);
        }
    }

    /// <summary>
    /// DE: Lehnt auch einen hängenden Log-Symlink ab, bevor das Ziel erstellt oder beschrieben werden kann.
    /// EN: Rejects even a dangling log symlink before its target can be created or written.
    /// </summary>
    [TestMethod]
    public void ServiceStatusWriter_DanglingLogSymlinkOnUnix_ThrowsUnauthorizedAccessException()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var writer = new ServiceStatusWriter("log-symlink-case");
        var statusDirectory = Path.Combine(ServicePath.GetServiceStatusPath(), "log-symlink-case");
        var targetFile = Path.Combine(Path.GetTempPath(), $"inventar-missing-log-target-{Guid.NewGuid():N}.log");
        var logFile = Path.Combine(statusDirectory, "service.log");
        File.CreateSymbolicLink(logFile, targetFile);

        try
        {
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => writer.WriteLog("must not be written"));
            Assert.IsFalse(File.Exists(targetFile));
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    /// DE: Liest keine Settings über einen symbolischen Link und ändert dessen Zielrechte nicht.
    /// EN: Does not read settings through a symbolic link or change its target permissions.
    /// </summary>
    [TestMethod]
    public void SettingsReader_SymlinkedSettingsFileOnUnix_ReturnsNullWithoutChangingTarget()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var writer = new SettingsWriter("settings-symlink-case");
        var settingsDirectory = Path.Combine(ServicePath.GetServiceStatusPath(), "settings-symlink-case");
        var targetFile = Path.Combine(Path.GetTempPath(), $"inventar-settings-target-{Guid.NewGuid():N}.json");
        var settingsFile = Path.Combine(settingsDirectory, "settings.json");
        File.WriteAllText(targetFile, "sentinel");
        var targetMode = File.GetUnixFileMode(targetFile);
        File.CreateSymbolicLink(settingsFile, targetFile);

        try
        {
            var reader = new SettingsReader("settings-symlink-case");

            Assert.IsNull(reader.ReadSettings());
            Assert.AreEqual("sentinel", File.ReadAllText(targetFile));
            Assert.AreEqual(targetMode, File.GetUnixFileMode(targetFile));
        }
        finally
        {
            File.Delete(settingsFile);
            File.Delete(targetFile);
        }
    }

    /// <summary>
    /// DE: Verhindert Settings-Schreibzugriffe in Verzeichnissen, in denen andere Unix-Benutzer Dateinamen austauschen könnten.
    /// EN: Prevents settings writes in directories where other Unix users could replace file names.
    /// </summary>
    [TestMethod]
    public void SettingsWriter_GroupWritableDirectoryOnUnix_ThrowsUnauthorizedAccessException()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var writer = new SettingsWriter("settings-unsafe-case");
        var settingsDirectory = Path.Combine(
            InventarWorkerCommon.Services.Paths.ServicePath.GetServiceStatusPath(),
            "settings-unsafe-case");
        var originalMode = File.GetUnixFileMode(settingsDirectory);

        try
        {
            File.SetUnixFileMode(settingsDirectory, originalMode | UnixFileMode.GroupWrite);

            Assert.ThrowsExactly<UnauthorizedAccessException>(() => writer.WriteSettings(CreateSettings(writeEnabled: false)));
            Assert.AreEqual(0, Directory.GetFiles(settingsDirectory, ".settings.*.tmp").Length);
        }
        finally
        {
            File.SetUnixFileMode(settingsDirectory, originalMode);
        }
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
