using System.Globalization;
using CsvHelper;
using Dapper;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.Software;
using InventarWorkerCommon.Models.SqlDatabase;
using InventarWorkerCommon.Services.Csv;
using InventarWorkerCommon.Services.Database;
using Microsoft.Data.Sqlite;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Deckt die lokale SQLite-Persistenz spiegelbildlich zu den PostgreSQL-Tests ab.
/// EN: Covers the local SQLite persistence layer in a way that mirrors the PostgreSQL tests.
/// </summary>
[TestClass]
[DoNotParallelize]
public class SqliteDbServiceTest
{
    private string _databasePath = string.Empty;
    private SqliteDbService _service = null!;

    /// <summary>
    /// DE: Erstellt pro Test eine frische temporäre SQLite-Datenbank.
    /// EN: Creates a fresh temporary SQLite database for each test.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"inventar-sqlite-{Guid.NewGuid():N}.db");
        _service = new SqliteDbService($"Data Source={_databasePath}");
        _service.InitializeDatabase();
    }

    /// <summary>
    /// DE: Entfernt die temporäre SQLite-Datenbank nach jedem Test.
    /// EN: Removes the temporary SQLite database after each test.
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }

    /// <summary>
    /// DE: Prüft Einfügen, Aktualisieren und Harvester-Felder für Maschinen.
    /// EN: Verifies insert, update and harvester field handling for machines.
    /// </summary>
    [TestMethod]
    public async Task SaveOrUpdateMachineAsync_InsertAndUpdatePaths_WorkAsExpected()
    {
        var firstSeen = DateTime.UtcNow.AddMinutes(-10);
        var machine = CreateMachine("Sqlite-Machine-01", "Windows 10", firstSeen);

        var insertedId = await _service.SaveOrUpdateMachineAsync(machine);
        var insertedMachine = await _service.GetMachineByIdAsync(insertedId);

        Assert.IsNotNull(insertedMachine);
        Assert.AreEqual("Sqlite-Machine-01", insertedMachine.Name);
        Assert.AreEqual("Windows 10", insertedMachine.OperatingSystem);

        var updatedSeen = DateTime.UtcNow;
        var updatedMachine = CreateMachine(
            "Sqlite-Machine-01",
            "Windows 11",
            updatedSeen,
            ipv4: "10.0.0.1",
            ipv6: "::1",
            fqdn: "sqlite-machine.local",
            lastHarvested: updatedSeen);

        var updatedId = await _service.SaveOrUpdateMachineAsync(updatedMachine, isHarvester: true);
        var storedMachine = await _service.GetMachineByIdAsync(updatedId);

        Assert.AreEqual(insertedId, updatedId);
        Assert.IsNotNull(storedMachine);
        Assert.AreEqual("Windows 11", storedMachine.OperatingSystem);
        Assert.AreEqual("10.0.0.1", storedMachine.IPv4);
        Assert.AreEqual("::1", storedMachine.IPv6);
        Assert.AreEqual("sqlite-machine.local", storedMachine.FQDN);
        Assert.AreEqual(updatedSeen, storedMachine.LastHarvested);
    }

    /// <summary>
    /// DE: Prüft Hardware-/Software-Speicherung, Latest-Reader, Exists und Zähler.
    /// EN: Verifies hardware/software persistence, latest readers, exists checks and counters.
    /// </summary>
    [TestMethod]
    public async Task InventoryPersistenceAndCounts_RoundTripSuccessfully()
    {
        var machineId = await _service.SaveOrUpdateMachineAsync(CreateMachine("Sqlite-Inventory-01", "Linux"));

        await _service.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("Sqlite-Inventory-01", 4));
        await _service.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory("SQLite App 1"));
        await _service.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("Sqlite-Inventory-01", 8));
        await _service.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory("SQLite App 2"));

        var latestHardware = await _service.GetLatestHardwareInventoryAsync(machineId);
        var latestSoftware = await _service.GetLatestSoftwareInventoryAsync(machineId);

        Assert.IsTrue(await _service.HasMachineRecordsAsync());
        Assert.IsTrue(await _service.HasHardwareInventoryRecordsAsync());
        Assert.IsTrue(await _service.HasSoftwareInventoryRecordsAsync());
        Assert.AreEqual(1, await _service.GetMachineCountAsync());
        Assert.AreEqual(2, await _service.GetHardwareInventoryCountAsync());
        Assert.AreEqual(2, await _service.GetSoftwareInventoryCountAsync());

        Assert.IsNotNull(latestHardware);
        Assert.IsNotNull(latestSoftware);
        Assert.AreEqual("Sqlite-Inventory-01", latestHardware.ComputerName);
        StringAssert.Contains(latestSoftware.InstalledSoftwareJson, "SQLite App 2");
    }

    /// <summary>
    /// DE: Prüft die Status-Views für aktiv, mit Netzwerk, deaktiviert und deprovisioniert.
    /// EN: Verifies the status views for active, with-network, disabled and deprovisioned machines.
    /// </summary>
    [TestMethod]
    public async Task StatusViews_ReturnExpectedMachines()
    {
        await _service.SaveOrUpdateMachineAsync(CreateMachine("Gamma", "Windows"));
        await _service.SaveOrUpdateMachineAsync(CreateMachine("Alpha", "Windows"));
        await _service.SaveOrUpdateMachineAsync(
            CreateMachine("Alpha", "Windows", ipv4: "192.168.1.10"),
            isHarvester: true);
        await SeedMachineDirectAsync("Beta", disabled: true, deprovisioned: false);
        await SeedMachineDirectAsync("Omega", disabled: true, deprovisioned: true);

        var allMachines = await _service.GetMachinesAsync();
        var activeMachines = await _service.GetAllActiveMachinesAsync();
        var activeWithNetwork = await _service.GetAllActiveMachinesWithNetworkInfoAsync();
        var disabledMachines = await _service.GetAllDisabledMachinesAsync();
        var deprovisionedMachines = await _service.GetAllDeprovisionedMachinesAsync();

        CollectionAssert.AreEqual(new List<string> { "Alpha", "Beta", "Gamma", "Omega" }, allMachines.Select(m => m.Name).ToList());
        CollectionAssert.AreEqual(new List<string> { "Alpha", "Gamma" }, activeMachines.Select(m => m.Name).ToList());
        CollectionAssert.AreEqual(new List<string> { "Alpha" }, activeWithNetwork.Select(m => m.Name).ToList());
        CollectionAssert.AreEqual(new List<string> { "Beta" }, disabledMachines.Select(m => m.Name).ToList());
        CollectionAssert.AreEqual(new List<string> { "Omega" }, deprovisionedMachines.Select(m => m.Name).ToList());

        var machineByName = await _service.GetMachineByNameAsync("Alpha");
        Assert.IsNotNull(machineByName);
        Assert.AreEqual("192.168.1.10", machineByName.IPv4);
    }

    /// <summary>
    /// DE: Prüft, dass alte Inventardaten anhand des UTC-Cutoffs bereinigt werden.
    /// EN: Verifies that old inventory data is cleaned up based on the UTC cutoff.
    /// </summary>
    [TestMethod]
    public async Task CleanupOldRecordsAsync_RemovesOnlyOldInventoryEntries()
    {
        var machineId = await _service.SaveOrUpdateMachineAsync(CreateMachine("Sqlite-Cleanup-01", "Windows"));

        await _service.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("Sqlite-Cleanup-01", 2));
        await _service.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory("Cleanup App Old"));
        await MarkOldestInventoryEntriesAsOldAsync();
        await _service.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("Sqlite-Cleanup-01", 16));
        await _service.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory("Cleanup App New"));

        await _service.CleanupOldRecordsAsync(daysToKeep: 30);

        Assert.AreEqual(1, await _service.GetHardwareInventoryCountAsync());
        Assert.AreEqual(1, await _service.GetSoftwareInventoryCountAsync());

        var latestHardware = await _service.GetLatestHardwareInventoryAsync(machineId);
        var latestSoftware = await _service.GetLatestSoftwareInventoryAsync(machineId);

        Assert.IsNotNull(latestHardware);
        Assert.IsNotNull(latestSoftware);
        StringAssert.Contains(latestSoftware.InstalledSoftwareJson, "Cleanup App New");
    }

    /// <summary>
    /// DE: Prüft Dateinotfound-Guard, Import und Duplikat-Skip des CSV-Pfads.
    /// EN: Verifies the file-not-found guard, import path and duplicate skip for CSV import.
    /// </summary>
    [TestMethod]
    public async Task InitializeMachinesFromCsvAsync_ImportsAndSkipsDuplicates()
    {
        var missingFile = Path.Combine(Path.GetTempPath(), $"sqlite-missing-{Guid.NewGuid():N}.csv");
        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => _service.InitializeMachinesFromCsvAsync(missingFile));

        var csvFile = Path.Combine(Path.GetTempPath(), $"sqlite-import-{Guid.NewGuid():N}.csv");
        await File.WriteAllLinesAsync(
            csvFile,
            [
                "Name,OperatingSystem,IPv4,IPv6,FQDN,Disabled,Deprovisioned",
                "CSV-SQLITE-01,Windows 10,192.168.0.10,,csv-sqlite-01.local,0,0",
                "CSV-SQLITE-02,Windows 11,192.168.0.11,,csv-sqlite-02.local,1,0",
                "CSV-SQLITE-03,Linux,192.168.0.12,,csv-sqlite-03.local,0,1"
            ]);

        var imported = await _service.InitializeMachinesFromCsvAsync(csvFile);
        var reimported = await _service.InitializeMachinesFromCsvAsync(csvFile);
        var importedMachines = await _service.GetMachinesAsync();

        Assert.AreEqual(3, imported);
        Assert.AreEqual(0, reimported);
        Assert.AreEqual(3, importedMachines.Count);
        Assert.IsTrue(importedMachines.Any(m => m.Name == "CSV-SQLITE-02" && m.Disabled));
        Assert.IsTrue(importedMachines.Any(m => m.Name == "CSV-SQLITE-03" && m.Deprovisioned));

        File.Delete(csvFile);
    }

    private async Task SeedMachineDirectAsync(string name, bool disabled, bool deprovisioned)
    {
        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        await connection.ExecuteAsync(
            """
            INSERT INTO Machines (Name, OperatingSystem, LastSeen, CreatedAt, Disabled, Deprovisioned)
            VALUES (@Name, 'Windows', @LastSeen, @CreatedAt, @Disabled, @Deprovisioned)
            """,
            new
            {
                Name = name,
                LastSeen = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Disabled = disabled ? 1 : 0,
                Deprovisioned = deprovisioned ? 1 : 0
            });
    }

    private async Task MarkOldestInventoryEntriesAsOldAsync()
    {
        var oldDate = DateTime.UtcNow.AddDays(-40);
        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            """
            UPDATE HardwareInventories
            SET CreatedAt = @OldDate
            WHERE Id = (SELECT MIN(Id) FROM HardwareInventories)
            """,
            new { OldDate = oldDate });

        await connection.ExecuteAsync(
            """
            UPDATE SoftwareInventories
            SET CreatedAt = @OldDate
            WHERE Id = (SELECT MIN(Id) FROM SoftwareInventories)
            """,
            new { OldDate = oldDate });
    }

    private static Machine CreateMachine(
        string name,
        string operatingSystem,
        DateTime? lastSeen = null,
        string? ipv4 = null,
        string? ipv6 = null,
        string? fqdn = null,
        DateTime? lastHarvested = null)
    {
        return new Machine
        {
            Name = name,
            OperatingSystem = operatingSystem,
            LastSeen = lastSeen ?? DateTime.UtcNow,
            IPv4 = ipv4,
            IPv6 = ipv6,
            FQDN = fqdn,
            LastHarvested = lastHarvested
        };
    }

    private static HardwareInventory CreateHardwareInventory(string machineName, int processorCores)
    {
        return new HardwareInventory
        {
            System = new SystemInfo
            {
                MachineName = machineName,
                Platform = "ThinkCentre M90",
                UserName = "OpenAI",
                Architecture = "x64"
            },
            Cpu = new CpuInfo
            {
                ProcessorName = "AMD Ryzen",
                ProcessorCount = processorCores,
                Architecture = "x64"
            },
            Memory = new MemoryInfo
            {
                TotalPhysicalMemory = 32L * 1024 * 1024 * 1024,
                AvailablePhysicalMemory = 12L * 1024 * 1024 * 1024,
                MemoryUsagePercentage = 62.5
            }
        };
    }

    private static SoftwareInventory CreateSoftwareInventory(string installedSoftwareName)
    {
        return new SoftwareInventory
        {
            InstalledSoftware =
            [
                new SoftwareInfo
                {
                    Name = installedSoftwareName,
                    Version = "1.0.0",
                    Publisher = "OpenAI"
                }
            ],
            RunningProcesses =
            [
                new ProcessInfo
                {
                    ProcessName = "dotnet",
                    ProcessId = 100,
                    WorkingSet = 123456
                }
            ],
            WindowsServices =
            [
                new ServiceInfo
                {
                    ServiceName = "Spooler",
                    DisplayName = "Print Spooler",
                    Status = "Running"
                }
            ],
            EnvironmentVariables = ["PATH=/usr/bin", "HOME=/tmp"],
            StartupPrograms = ["OneDrive"],
            Runtime = new RuntimeInfo
            {
                DotNetVersion = "10.0.0",
                PowerShellVersion = "7.5.0"
            }
        };
    }
}
