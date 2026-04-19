using System.Reflection;
using System.Text.Json;
using Dapper;
using HarvesterWorkerService;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.Settings;
using InventarWorkerCommon.Models.Software;
using InventarWorkerCommon.Models.SqlDatabase;
using InventarWorkerCommon.Services.Common;
using InventarWorkerCommon.Services.Database;
using Npgsql;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Enthält Unit- und Integrationstests für <see cref="PgSqlDbService"/> und den dazugehörigen Initialisierungsfluss.
/// EN: Contains unit and integration tests for <see cref="PgSqlDbService"/> and the related initialization flow.
/// </summary>
[TestClass]
[DoNotParallelize]
public class PgSqlDbServiceTest
{
    private const string IntegrationCategory = "Integration";
    private static string? _integrationConnectionString;
    private static string _serviceStatusDirectory = string.Empty;
    private PgSqlDbService _svc = null!;

    /// <summary>
    /// DE: Liest die optionale PostgreSQL-Testverbindung und richtet das temporäre Service-Verzeichnis ein.
    /// EN: Reads the optional PostgreSQL test connection and sets up the temporary service directory.
    /// </summary>
    /// <param name="context">
    /// DE: MSTest-Kontext für die Klasseninitialisierung.
    /// EN: MSTest context for class initialization.
    /// </param>
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        _ = context;
        _integrationConnectionString = Environment.GetEnvironmentVariable("PGSQL_TEST_CONNECTION_STRING");
        _serviceStatusDirectory = Path.Combine(Path.GetTempPath(), $"inventar-pgsql-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_serviceStatusDirectory);
        Environment.SetEnvironmentVariable("SERVICESTATUSDIRECTORY", _serviceStatusDirectory);
    }

    /// <summary>
    /// DE: Bereinigt die Testdatenbank nach Abschluss aller Tests und entfernt das temporäre Verzeichnis.
    /// EN: Cleans the test database after all tests and removes the temporary directory.
    /// </summary>
    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        if (!string.IsNullOrWhiteSpace(_integrationConnectionString))
        {
            var cleanupService = new PgSqlDbService(_integrationConnectionString);
            cleanupService.InitializeDatabase();
            await ResetDatabaseAsync(_integrationConnectionString);
        }

        Environment.SetEnvironmentVariable("SERVICESTATUSDIRECTORY", null);

        if (!string.IsNullOrWhiteSpace(_serviceStatusDirectory) && Directory.Exists(_serviceStatusDirectory))
        {
            Directory.Delete(_serviceStatusDirectory, recursive: true);
        }
    }

    /// <summary>
    /// DE: Initialisiert die Standardinstanz für Tests und setzt das temporäre Service-Verzeichnis pro Test zurück.
    /// EN: Initializes the default test instance and reapplies the temporary service directory per test.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        Environment.SetEnvironmentVariable("SERVICESTATUSDIRECTORY", _serviceStatusDirectory);
        _svc = new PgSqlDbService(_integrationConnectionString ?? "Host=localhost;Port=5432;Database=inventar_test;Username=inventar;Password=test;");
    }

    /// <summary>
    /// DE: Prüft, dass der parameterlose Initialisierungsweg keinen PostgreSQL-Dienst erzeugt.
    /// EN: Verifies that the parameterless initialization path does not create a PostgreSQL service.
    /// </summary>
    [TestMethod]
    public void Services_Parameterless_PgSqlDbServiceIsNull()
    {
        using var container = Initialize.Services();
        Assert.IsNull(container.PgSqlDbService);
    }

    /// <summary>
    /// DE: Prüft, dass bei deaktiviertem PostgreSQL-Schreiben kein PostgreSQL-Dienst erzeugt wird.
    /// EN: Verifies that no PostgreSQL service is created when PostgreSQL writing is disabled.
    /// </summary>
    [TestMethod]
    public void Services_WriteEnabledFalse_PgSqlDbServiceIsNull()
    {
        var settings = CreateSettings(writeEnabled: false);

        using var container = Initialize.Services(settings);

        Assert.IsNull(container.PgSqlDbService);
    }

    /// <summary>
    /// DE: Prüft die Guard-Exception für ein fehlendes Maschinenobjekt.
    /// EN: Verifies the guard exception for a missing machine object.
    /// </summary>
    [TestMethod]
    public async Task SaveOrUpdateMachineAsync_NullMachine_ThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => _svc.SaveOrUpdateMachineAsync(null!));
    }

    /// <summary>
    /// DE: Prüft die Guard-Exception für eine fehlende SQLite-Id.
    /// EN: Verifies the guard exception for a missing SQLite Id.
    /// </summary>
    [TestMethod]
    public async Task SaveOrUpdateMachineAsync_ZeroId_ThrowsArgumentException()
    {
        var machine = CreateMachine(0, "ZeroIdMachine");

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => _svc.SaveOrUpdateMachineAsync(machine));
    }

    /// <summary>
    /// DE: Prüft, dass der Worker-Helfer bei einem fehlenden PostgreSQL-Dienst keinen Fehler wirft.
    /// EN: Verifies that the worker helper does not throw when the PostgreSQL service is missing.
    /// </summary>
    [TestMethod]
    public async Task Worker_PgSqlDbServiceIsNull_DoesNotThrow()
    {
        var method = typeof(Worker).GetMethod("WriteToPgSqlAsync", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);

        var machine = CreateMachine(21, "WorkerNullMachine");
        var hardware = CreateHardwareInventory("WorkerNullMachine");
        var software = CreateSoftwareInventory();

        var task = method.Invoke(
            null,
            new object[]
            {
                null!,
                21,
                machine,
                hardware,
                software,
                new Action<Exception>(_ => { })
            }) as Task;

        Assert.IsNotNull(task);
        await task;
    }

    /// <summary>
    /// DE: Prüft den UTC-Cutoff für <c>daysToKeep = 0</c>.
    /// EN: Verifies the UTC cutoff for <c>daysToKeep = 0</c>.
    /// </summary>
    [TestMethod]
    public void CleanupOldRecordsAsync_ZeroDays_CutoffIsUtcNow()
    {
        var method = typeof(PgSqlDbService).GetMethod("CalculateCutoff", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);

        var before = DateTime.UtcNow;
        var cutoff = (DateTime)method.Invoke(null, new object[] { 0 })!;
        var after = DateTime.UtcNow;

        Assert.AreEqual(DateTimeKind.Utc, cutoff.Kind);
        Assert.IsTrue(cutoff >= before.AddSeconds(-1));
        Assert.IsTrue(cutoff <= after.AddSeconds(1));
    }

    /// <summary>
    /// DE: Prüft, dass negative Tage einen Cutoff in der Zukunft ergeben.
    /// EN: Verifies that negative day values produce a cutoff in the future.
    /// </summary>
    [TestMethod]
    public void CleanupOldRecordsAsync_NegativeDays_CutoffIsInFuture()
    {
        var method = typeof(PgSqlDbService).GetMethod("CalculateCutoff", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);

        var cutoff = (DateTime)method.Invoke(null, new object[] { -5 })!;

        Assert.AreEqual(DateTimeKind.Utc, cutoff.Kind);
        Assert.IsTrue(cutoff > DateTime.UtcNow);
    }

    /// <summary>
    /// DE: Prüft die Dateinotfound-Guard des CSV-Imports.
    /// EN: Verifies the file-not-found guard of the CSV import.
    /// </summary>
    [TestMethod]
    public async Task InitializeMachinesFromCsvAsync_FileNotFound_ThrowsFileNotFoundException()
    {
        var missingFile = Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid():N}.csv");
        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => _svc.InitializeMachinesFromCsvAsync(missingFile));
    }

    /// <summary>
    /// DE: Prüft, dass eine neue Maschine mit expliziter SQLite-Id eingefügt wird.
    /// EN: Verifies that a new machine is inserted with an explicit SQLite Id.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task SaveOrUpdateMachineAsync_NewMachine_InsertsWithExplicitId()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machine = CreateMachine(42, "PgSql-NewMachine");

        var returnedId = await svc.SaveOrUpdateMachineAsync(machine);
        var storedMachine = await svc.GetMachineByIdAsync(42);

        Assert.AreEqual(42, returnedId);
        Assert.IsNotNull(storedMachine);
        Assert.AreEqual(42, storedMachine.Id);
        Assert.AreEqual("PgSql-NewMachine", storedMachine.Name);
    }

    /// <summary>
    /// DE: Prüft, dass ein erneutes Speichern nach Name keinen Duplikateintrag erzeugt.
    /// EN: Verifies that saving the same name again does not create a duplicate row.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task SaveOrUpdateMachineAsync_ExistingName_UpdatesRecordNoDuplicate()
    {
        var svc = await CreateIntegrationServiceAsync();
        await svc.SaveOrUpdateMachineAsync(CreateMachine(5, "PgSql-DuplicateMachine", operatingSystem: "Windows 10"));

        var returnedId = await svc.SaveOrUpdateMachineAsync(CreateMachine(99, "PgSql-DuplicateMachine", operatingSystem: "Windows 11"));
        var count = await QueryScalarAsync<int>("SELECT COUNT(*) FROM Machines WHERE Name = @Name", new { Name = "PgSql-DuplicateMachine" });
        var storedMachine = await svc.GetMachineByIdAsync(5);

        Assert.AreEqual(5, returnedId);
        Assert.AreEqual(1, count);
        Assert.IsNotNull(storedMachine);
        Assert.AreEqual("Windows 11", storedMachine.OperatingSystem);
    }

    /// <summary>
    /// DE: Prüft, dass der Harvester-Pfad auch Netzwerkinformationen aktualisiert.
    /// EN: Verifies that the harvester path also updates network information.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task SaveOrUpdateMachineAsync_IsHarvesterTrue_UpdatesNetworkFields()
    {
        var svc = await CreateIntegrationServiceAsync();
        await svc.SaveOrUpdateMachineAsync(CreateMachine(7, "PgSql-HarvesterMachine", operatingSystem: "Windows 10"));

        var updatedMachine = CreateMachine(88, "PgSql-HarvesterMachine", operatingSystem: "Windows 11", ipv4: "10.0.0.7", ipv6: "::7", fqdn: "pgsql-harvester.local");
        var returnedId = await svc.SaveOrUpdateMachineAsync(updatedMachine, isHarvester: true);
        var storedMachine = await svc.GetMachineByIdAsync(7);

        Assert.AreEqual(7, returnedId);
        Assert.IsNotNull(storedMachine);
        Assert.AreEqual("10.0.0.7", storedMachine.IPv4);
        Assert.AreEqual("::7", storedMachine.IPv6);
        Assert.AreEqual("pgsql-harvester.local", storedMachine.FQDN);
        Assert.IsNotNull(storedMachine.LastHarvested);
    }

    /// <summary>
    /// DE: Prüft, dass ein Hardware-Inventar erfolgreich gespeichert wird und UTC nutzt.
    /// EN: Verifies that a hardware inventory is saved successfully and uses UTC.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task SaveHardwareInventoryAsync_ValidData_InsertsRecord()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machineId = await svc.SaveOrUpdateMachineAsync(CreateMachine(11, "PgSql-HardwareMachine"));

        await svc.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("PgSql-HardwareMachine"));
        var latestHardware = await svc.GetLatestHardwareInventoryAsync(machineId);

        Assert.IsNotNull(latestHardware);
        Assert.AreEqual("PgSql-HardwareMachine", latestHardware.ComputerName);
        Assert.AreEqual(DateTimeKind.Utc, latestHardware.CreatedAt.Kind);
    }

    /// <summary>
    /// DE: Prüft, dass alle Software-JSON-Felder korrekt serialisiert gespeichert werden.
    /// EN: Verifies that all software JSON fields are stored with correct serialization.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task SaveSoftwareInventoryAsync_ValidData_SerializesAllJsonFields()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machineId = await svc.SaveOrUpdateMachineAsync(CreateMachine(12, "PgSql-SoftwareMachine"));
        var software = CreateSoftwareInventory();

        await svc.SaveSoftwareInventoryAsync(machineId, software);
        var latestSoftware = await svc.GetLatestSoftwareInventoryAsync(machineId);

        Assert.IsNotNull(latestSoftware);
        Assert.AreEqual(JsonSerializer.Serialize(software.RunningProcesses), latestSoftware.ProcessesJson);
        Assert.AreEqual(JsonSerializer.Serialize(software.InstalledSoftware), latestSoftware.InstalledSoftwareJson);
        Assert.AreEqual(JsonSerializer.Serialize(software.WindowsServices), latestSoftware.ServicesJson);
        Assert.AreEqual(JsonSerializer.Serialize(software.EnvironmentVariables), latestSoftware.EnvironmentJson);
        Assert.AreEqual(JsonSerializer.Serialize(software.StartupPrograms), latestSoftware.StartupProgramsJson);
        Assert.AreEqual(JsonSerializer.Serialize(software.Runtime), latestSoftware.RuntimeJson);
    }

    /// <summary>
    /// DE: Prüft die Sortierung aller Maschinen und den Filter aktiver Maschinen.
    /// EN: Verifies ordering of all machines and filtering of active machines.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetMachinesAsync_ReturnsAllOrderedByName()
    {
        var svc = await CreateIntegrationServiceAsync();
        await SeedStatusMachinesAsync(svc);

        var machines = await svc.GetMachinesAsync();

        CollectionAssert.AreEqual(
            new[] { "Alpha", "Beta", "Gamma", "Omega" },
            machines.Select(machine => machine.Name).ToArray());
    }

    /// <summary>
    /// DE: Prüft, dass aktive Maschinen deaktivierte und deprovisionierte Einträge ausschließen.
    /// EN: Verifies that active machines exclude disabled and deprovisioned entries.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetAllActiveMachinesAsync_ExcludesDisabledAndDeprovisioned()
    {
        var svc = await CreateIntegrationServiceAsync();
        await SeedStatusMachinesAsync(svc);

        var machines = await svc.GetAllActiveMachinesAsync();

        CollectionAssert.AreEqual(new[] { "Alpha", "Gamma" }, machines.Select(machine => machine.Name).ToArray());
    }

    /// <summary>
    /// DE: Prüft, dass aktive Maschinen mit Netzwerkinformationen mindestens einen Netzwerkwert haben.
    /// EN: Verifies that active machines with network information have at least one network value.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetAllActiveMachinesWithNetworkInfoAsync_RequiresAtLeastOneNetworkValue()
    {
        var svc = await CreateIntegrationServiceAsync();
        await SeedStatusMachinesAsync(svc);

        var machines = await svc.GetAllActiveMachinesWithNetworkInfoAsync();

        CollectionAssert.AreEqual(new[] { "Alpha" }, machines.Select(machine => machine.Name).ToArray());
    }

    /// <summary>
    /// DE: Prüft, dass nur deaktivierte, aber nicht deprovisionierte Maschinen zurückgegeben werden.
    /// EN: Verifies that only disabled but not deprovisioned machines are returned.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetAllDisabledMachinesAsync_ReturnsOnlyDisabledNotDeprovisioned()
    {
        var svc = await CreateIntegrationServiceAsync();
        await SeedStatusMachinesAsync(svc);

        var machines = await svc.GetAllDisabledMachinesAsync();

        CollectionAssert.AreEqual(new[] { "Beta" }, machines.Select(machine => machine.Name).ToArray());
    }

    /// <summary>
    /// DE: Prüft, dass nur deprovisionierte Maschinen zurückgegeben werden.
    /// EN: Verifies that only deprovisioned machines are returned.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetAllDeprovisionedMachinesAsync_ReturnsOnlyDeprovisioned()
    {
        var svc = await CreateIntegrationServiceAsync();
        await SeedStatusMachinesAsync(svc);

        var machines = await svc.GetAllDeprovisionedMachinesAsync();

        CollectionAssert.AreEqual(new[] { "Omega" }, machines.Select(machine => machine.Name).ToArray());
    }

    /// <summary>
    /// DE: Prüft den Lookup einer vorhandenen Maschine per Id.
    /// EN: Verifies lookup of an existing machine by Id.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetMachineByIdAsync_ExistingId_ReturnsMachine()
    {
        var svc = await CreateIntegrationServiceAsync();
        await svc.SaveOrUpdateMachineAsync(CreateMachine(13, "LookupById"));

        var machine = await svc.GetMachineByIdAsync(13);

        Assert.IsNotNull(machine);
        Assert.AreEqual("LookupById", machine.Name);
    }

    /// <summary>
    /// DE: Prüft den Lookup einer nicht vorhandenen Maschine per Id.
    /// EN: Verifies lookup of a non-existing machine by Id.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetMachineByIdAsync_NonExistingId_ReturnsNull()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machine = await svc.GetMachineByIdAsync(9999);
        Assert.IsNull(machine);
    }

    /// <summary>
    /// DE: Prüft den Lookup einer vorhandenen Maschine per Name.
    /// EN: Verifies lookup of an existing machine by name.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetMachineByNameAsync_ExistingName_ReturnsMachine()
    {
        var svc = await CreateIntegrationServiceAsync();
        await svc.SaveOrUpdateMachineAsync(CreateMachine(14, "LookupByName"));

        var machine = await svc.GetMachineByNameAsync("LookupByName");

        Assert.IsNotNull(machine);
        Assert.AreEqual(14, machine.Id);
    }

    /// <summary>
    /// DE: Prüft, dass stets der neueste Hardware-Eintrag zurückgegeben wird.
    /// EN: Verifies that the newest hardware entry is returned.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetLatestHardwareInventoryAsync_ReturnsNewestEntry()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machineId = await svc.SaveOrUpdateMachineAsync(CreateMachine(15, "HardwareLatestMachine"));

        await svc.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("HardwareLatestMachine", processorCores: 4));
        await Task.Delay(20);
        await svc.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("HardwareLatestMachine", processorCores: 8));

        var latestHardware = await svc.GetLatestHardwareInventoryAsync(machineId);

        Assert.IsNotNull(latestHardware);
        Assert.AreEqual(8, latestHardware.ProcessorCores);
    }

    /// <summary>
    /// DE: Prüft, dass stets der neueste Software-Eintrag zurückgegeben wird.
    /// EN: Verifies that the newest software entry is returned.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetLatestSoftwareInventoryAsync_ReturnsNewestEntry()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machineId = await svc.SaveOrUpdateMachineAsync(CreateMachine(16, "SoftwareLatestMachine"));

        await svc.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory("FirstApp"));
        await Task.Delay(20);
        await svc.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory("SecondApp"));

        var latestSoftware = await svc.GetLatestSoftwareInventoryAsync(machineId);

        Assert.IsNotNull(latestSoftware);
        StringAssert.Contains(latestSoftware.InstalledSoftwareJson ?? string.Empty, "SecondApp");
    }

    /// <summary>
    /// DE: Prüft den Existenzcheck für Maschinen bei vorhandenen Datensätzen.
    /// EN: Verifies the machine existence check when records are present.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task HasMachineRecordsAsync_RecordsExist_ReturnsTrue()
    {
        var svc = await CreateIntegrationServiceAsync();
        await svc.SaveOrUpdateMachineAsync(CreateMachine(17, "MachineExists"));
        Assert.IsTrue(await svc.HasMachineRecordsAsync());
    }

    /// <summary>
    /// DE: Prüft den Existenzcheck für Maschinen bei leerer Tabelle.
    /// EN: Verifies the machine existence check when the table is empty.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task HasMachineRecordsAsync_EmptyTable_ReturnsFalse()
    {
        var svc = await CreateIntegrationServiceAsync();
        Assert.IsFalse(await svc.HasMachineRecordsAsync());
    }

    /// <summary>
    /// DE: Prüft den Existenzcheck für Hardware-Datensätze.
    /// EN: Verifies the existence check for hardware records.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task HasHardwareInventoryRecordsAsync_RecordsExist_ReturnsTrue()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machineId = await svc.SaveOrUpdateMachineAsync(CreateMachine(18, "HardwareExists"));
        await svc.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("HardwareExists"));

        Assert.IsTrue(await svc.HasHardwareInventoryRecordsAsync());
    }

    /// <summary>
    /// DE: Prüft den Existenzcheck für Software-Datensätze.
    /// EN: Verifies the existence check for software records.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task HasSoftwareInventoryRecordsAsync_RecordsExist_ReturnsTrue()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machineId = await svc.SaveOrUpdateMachineAsync(CreateMachine(19, "SoftwareExists"));
        await svc.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory());

        Assert.IsTrue(await svc.HasSoftwareInventoryRecordsAsync());
    }

    /// <summary>
    /// DE: Prüft den Maschinenzähler.
    /// EN: Verifies the machine count.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetMachineCountAsync_ReturnsCorrectCount()
    {
        var svc = await CreateIntegrationServiceAsync();
        await svc.SaveOrUpdateMachineAsync(CreateMachine(20, "CountMachine1"));
        await svc.SaveOrUpdateMachineAsync(CreateMachine(21, "CountMachine2"));

        Assert.AreEqual(2, await svc.GetMachineCountAsync());
    }

    /// <summary>
    /// DE: Prüft den Hardware-Zähler.
    /// EN: Verifies the hardware inventory count.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetHardwareInventoryCountAsync_ReturnsCorrectCount()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machineId = await svc.SaveOrUpdateMachineAsync(CreateMachine(22, "HardwareCountMachine"));
        await svc.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("HardwareCountMachine"));
        await svc.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("HardwareCountMachine", processorCores: 16));

        Assert.AreEqual(2, await svc.GetHardwareInventoryCountAsync());
    }

    /// <summary>
    /// DE: Prüft den Software-Zähler.
    /// EN: Verifies the software inventory count.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task GetSoftwareInventoryCountAsync_ReturnsCorrectCount()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machineId = await svc.SaveOrUpdateMachineAsync(CreateMachine(23, "SoftwareCountMachine"));
        await svc.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory("AppOne"));
        await svc.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory("AppTwo"));

        Assert.AreEqual(2, await svc.GetSoftwareInventoryCountAsync());
    }

    /// <summary>
    /// DE: Prüft, dass der Aufräumlauf mit null Tagen alle Inventareinträge löscht.
    /// EN: Verifies that cleanup with zero days deletes all inventory entries.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task CleanupOldRecordsAsync_ZeroDays_DeletesAllInventoryEntries()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machineId = await svc.SaveOrUpdateMachineAsync(CreateMachine(24, "CleanupAllMachine"));
        await svc.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("CleanupAllMachine"));
        await svc.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory());
        await Task.Delay(20);

        await svc.CleanupOldRecordsAsync(0);

        Assert.AreEqual(0, await svc.GetHardwareInventoryCountAsync());
        Assert.AreEqual(0, await svc.GetSoftwareInventoryCountAsync());
    }

    /// <summary>
    /// DE: Prüft, dass positive Aufbewahrungstage neuere Einträge erhalten.
    /// EN: Verifies that positive retention keeps newer entries.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task CleanupOldRecordsAsync_PositiveDays_KeepsRecentEntries()
    {
        var svc = await CreateIntegrationServiceAsync();
        var machineId = await svc.SaveOrUpdateMachineAsync(CreateMachine(25, "CleanupRecentMachine"));

        await svc.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("CleanupRecentMachine", processorCores: 2));
        await svc.SaveHardwareInventoryAsync(machineId, CreateHardwareInventory("CleanupRecentMachine", processorCores: 4));
        await svc.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory("OldApp"));
        await svc.SaveSoftwareInventoryAsync(machineId, CreateSoftwareInventory("NewApp"));

        await MarkOldestInventoryEntriesAsOldAsync();
        await svc.CleanupOldRecordsAsync(30);

        Assert.AreEqual(1, await svc.GetHardwareInventoryCountAsync());
        Assert.AreEqual(1, await svc.GetSoftwareInventoryCountAsync());
        Assert.AreEqual(4, (await svc.GetLatestHardwareInventoryAsync(machineId))?.ProcessorCores);
        StringAssert.Contains((await svc.GetLatestSoftwareInventoryAsync(machineId))?.InstalledSoftwareJson ?? string.Empty, "NewApp");
    }

    /// <summary>
    /// DE: Prüft den erfolgreichen CSV-Import mit drei Maschinen.
    /// EN: Verifies successful CSV import with three machines.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task InitializeMachinesFromCsvAsync_ValidCsv3Machines_Returns3()
    {
        var svc = await CreateIntegrationServiceAsync();
        var csvFile = CreateCsvFile(
            "Name,OperatingSystem,IPv4,IPv6,FQDN,Disabled,Deprovisioned",
            "CsvMachine01,Windows 10,192.168.1.10,,csv-01.local,0,0",
            "CsvMachine02,Windows 11,192.168.1.11,,csv-02.local,0,0",
            "CsvMachine03,Linux,192.168.1.12,,csv-03.local,0,0");

        try
        {
            var importedCount = await svc.InitializeMachinesFromCsvAsync(csvFile);
            Assert.AreEqual(3, importedCount);
            Assert.AreEqual(3, await svc.GetMachineCountAsync());
        }
        finally
        {
            File.Delete(csvFile);
        }
    }

    /// <summary>
    /// DE: Prüft, dass bestehende Maschinen beim CSV-Import übersprungen werden.
    /// EN: Verifies that existing machines are skipped during CSV import.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task InitializeMachinesFromCsvAsync_DuplicateMachines_SkipsExisting()
    {
        var svc = await CreateIntegrationServiceAsync();
        await svc.SaveOrUpdateMachineAsync(CreateMachine(26, "CsvMachineExisting"));
        var csvFile = CreateCsvFile(
            "Name,OperatingSystem,IPv4,IPv6,FQDN,Disabled,Deprovisioned",
            "CsvMachineExisting,Windows 10,192.168.1.20,,csv-existing.local,0,0",
            "CsvMachineNew01,Windows 11,192.168.1.21,,csv-new01.local,0,0",
            "CsvMachineNew02,Linux,192.168.1.22,,csv-new02.local,0,0");

        try
        {
            var importedCount = await svc.InitializeMachinesFromCsvAsync(csvFile);
            Assert.AreEqual(2, importedCount);
            Assert.AreEqual(3, await svc.GetMachineCountAsync());
        }
        finally
        {
            File.Delete(csvFile);
        }
    }

    /// <summary>
    /// DE: Prüft, dass ein Importfehler einen vollständigen Rollback auslöst.
    /// EN: Verifies that an import failure triggers a full rollback.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task InitializeMachinesFromCsvAsync_ExceptionDuringImport_RollsBackTransaction()
    {
        var svc = await CreateIntegrationServiceAsync();
        var csvFile = CreateCsvFile(
            "Name,OperatingSystem,IPv4,IPv6,FQDN,Disabled,Deprovisioned",
            "TestRollback-New,Windows 11,192.168.2.10,,rollback-new.local,0,0",
            ",Windows 11,192.168.2.11,,rollback-invalid.local,0,0");

        try
        {
            await Assert.ThrowsExactlyAsync<Exception>(() => svc.InitializeMachinesFromCsvAsync(csvFile));
            Assert.AreEqual(0, await QueryScalarAsync<int>("SELECT COUNT(*) FROM Machines WHERE Name = @Name", new { Name = "TestRollback-New" }));
        }
        finally
        {
            File.Delete(csvFile);
        }
    }

    /// <summary>
    /// DE: Prüft das Mapping von 0/1 auf BOOLEAN beim CSV-Import.
    /// EN: Verifies the mapping from 0/1 to BOOLEAN during CSV import.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task InitializeMachinesFromCsvAsync_DisabledDeprovisioned_MappedAsBool()
    {
        var svc = await CreateIntegrationServiceAsync();
        var csvFile = CreateCsvFile(
            "Name,OperatingSystem,IPv4,IPv6,FQDN,Disabled,Deprovisioned",
            "CsvBoolMachine01,Windows 10,192.168.3.10,,csv-bool-01.local,1,0",
            "CsvBoolMachine02,Windows 11,192.168.3.11,,csv-bool-02.local,1,1");

        try
        {
            await svc.InitializeMachinesFromCsvAsync(csvFile);

            var rows = (await QueryAsync<(string Name, bool Disabled, bool Deprovisioned)>(
                "SELECT Name, Disabled, Deprovisioned FROM Machines ORDER BY Name")).ToList();

            Assert.AreEqual(2, rows.Count);
            Assert.IsTrue(rows[0].Disabled);
            Assert.IsFalse(rows[0].Deprovisioned);
            Assert.IsTrue(rows[1].Disabled);
            Assert.IsTrue(rows[1].Deprovisioned);
        }
        finally
        {
            File.Delete(csvFile);
        }
    }

    /// <summary>
    /// DE: Prüft, dass nur die neue PascalCase-View vorhanden ist.
    /// EN: Verifies that only the new PascalCase view exists.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task InitializeDatabase_HardwareInventoryViewExists_OldViewDropped()
    {
        var svc = await CreateIntegrationServiceAsync();
        _ = svc;

        var newViewCount = await QueryScalarAsync<int>(
            "SELECT COUNT(*) FROM information_schema.views WHERE table_schema = 'public' AND table_name = 'hardwareinventoryview'");
        var oldViewCount = await QueryScalarAsync<int>(
            "SELECT COUNT(*) FROM information_schema.views WHERE table_schema = 'public' AND table_name = 'hardware_inventory_view'");

        Assert.AreEqual(1, newViewCount);
        Assert.AreEqual(0, oldViewCount);
    }

    /// <summary>
    /// DE: Prüft die PascalCase-Aliase der Hardware-View.
    /// EN: Verifies the PascalCase aliases of the hardware view.
    /// </summary>
    [TestMethod]
    [TestCategory(IntegrationCategory)]
    public async Task HardwareInventoryView_ColumnAliasesArePascalCase()
    {
        var svc = await CreateIntegrationServiceAsync();
        _ = svc;

        var columns = (await QueryAsync<string>(
            "SELECT column_name FROM information_schema.columns WHERE table_name = 'hardwareinventoryview' ORDER BY ordinal_position")).ToList();

        CollectionAssert.AreEqual(
            new[] { "machineid", "machinename", "architecture", "processorcores", "totalmemorygb", "availablememorygb", "memoryusagepercent" },
            columns);
    }

    private static Settings CreateSettings(bool writeEnabled)
    {
        return new Settings
        {
            ClientApi = new ClientApi { ClientApiFqdn = "localhost", ClientApiPort = "5000" },
            MongoDb = new MongoDb { MongoDbFqdn = "localhost", MongoDbPort = "27017" },
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

    private static Machine CreateMachine(
        int id,
        string name,
        string operatingSystem = "Windows 11",
        bool disabled = false,
        bool deprovisioned = false,
        string? ipv4 = null,
        string? ipv6 = null,
        string? fqdn = null)
    {
        return new Machine
        {
            Id = id,
            Name = name,
            OperatingSystem = operatingSystem,
            LastSeen = DateTime.UtcNow,
            Disabled = disabled,
            Deprovisioned = deprovisioned,
            IPv4 = ipv4,
            IPv6 = ipv6,
            FQDN = fqdn,
            LastHarvested = DateTime.UtcNow
        };
    }

    private static HardwareInventory CreateHardwareInventory(string machineName, int processorCores = 8)
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

    private static SoftwareInventory CreateSoftwareInventory(string installedSoftwareName = "Visual Studio Code")
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

    private static async Task SeedStatusMachinesAsync(PgSqlDbService svc)
    {
        await svc.SaveOrUpdateMachineAsync(CreateMachine(1, "Gamma"));
        await svc.SaveOrUpdateMachineAsync(CreateMachine(2, "Alpha", ipv4: "192.168.10.10"));
        await svc.SaveOrUpdateMachineAsync(CreateMachine(3, "Beta", disabled: true));
        await svc.SaveOrUpdateMachineAsync(CreateMachine(4, "Omega", disabled: true, deprovisioned: true));
    }

    private static async Task<PgSqlDbService> CreateIntegrationServiceAsync()
    {
        var connectionString = RequireIntegrationConnectionString();
        var svc = new PgSqlDbService(connectionString);
        svc.InitializeDatabase();
        await ResetDatabaseAsync(connectionString);
        return svc;
    }

    private static string RequireIntegrationConnectionString()
    {
        if (string.IsNullOrWhiteSpace(_integrationConnectionString))
        {
            Assert.Inconclusive("PGSQL_TEST_CONNECTION_STRING is not configured.");
        }

        return _integrationConnectionString!;
    }

    private static async Task ResetDatabaseAsync(string connectionString)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync("TRUNCATE TABLE SoftwareInventories, HardwareInventories, Machines RESTART IDENTITY CASCADE");
    }

    private static async Task<T> QueryScalarAsync<T>(string sql, object? parameters = null)
    {
        await using var connection = new NpgsqlConnection(RequireIntegrationConnectionString());
        await connection.OpenAsync();
        return await connection.QuerySingleAsync<T>(sql, parameters);
    }

    private static async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        await using var connection = new NpgsqlConnection(RequireIntegrationConnectionString());
        await connection.OpenAsync();
        return await connection.QueryAsync<T>(sql, parameters);
    }

    private static async Task MarkOldestInventoryEntriesAsOldAsync()
    {
        var oldDate = DateTime.UtcNow.AddDays(-40);
        await using var connection = new NpgsqlConnection(RequireIntegrationConnectionString());
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

    private static string CreateCsvFile(params string[] lines)
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"pgsql-import-{Guid.NewGuid():N}.csv");
        File.WriteAllLines(filePath, lines);
        return filePath;
    }
}
