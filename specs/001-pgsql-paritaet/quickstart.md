# Quickstart: PostgreSQL-Parität zum SqliteDbService

**Branch**: `001-pgsql-paritaet` | **Date**: 2026-04-18

Diese Anleitung beschreibt, wie die Implementierung nach Fertigstellung validiert werden kann.

---

## Voraussetzungen / Prerequisites

### 1. PostgreSQL-Instanz

```bash
# macOS — PostgreSQL via Homebrew
brew install postgresql@16
brew services start postgresql@16

# Oder Docker / Or Docker:
docker run --name inventar-pgsql \
  -e POSTGRES_PASSWORD=test \
  -e POSTGRES_USER=inventar \
  -e POSTGRES_DB=inventar \
  -p 5432:5432 \
  -d postgres:16
```

### 2. .NET 10 SDK

```bash
dotnet --version
# Erwartet / Expected: 10.x.x
```

### 3. Solution bauen / Build solution

```bash
cd /path/to/InventarWorkerService
dotnet restore InventarWorkerService.sln
dotnet build InventarWorkerService.sln
# Erwartetes Ergebnis / Expected: Build succeeded, 0 Warning(s)
```

---

## Schritt 1: Datenbank initialisieren / Initialize Database

DE: Stelle sicher, dass `InitializeDatabase()` ohne Fehler durchläuft.
EN: Verify that `InitializeDatabase()` runs without errors.

```bash
# Umgebungsvariable setzen / Set environment variable:
export PGSQL_TEST_CONNECTION_STRING="Host=localhost;Port=5432;Database=inventar_test;Username=inventar;Password=test;"

# Integrationstests ausführen / Run integration tests:
dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj \
  --filter "TestCategory=Integration"

# Normaler CI-Lauf ohne Integrationstests / Normal CI run without integration tests:
dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj \
  --filter "TestCategory!=Integration"
```

> **Hinweis / Note**: Integrationstests verwenden eine dedizierte Test-Datenbank `inventar_test`
> (nicht `inventar`), die per `ClassCleanup` zwischen Testläufen bereinigt wird.

Manuell verifizieren / Verify manually:

```sql
-- Nach InitializeDatabase(): / After InitializeDatabase():
\dt         -- Zeigt / Shows: Machines, HardwareInventories, SoftwareInventories
\dv         -- Zeigt / Shows: HardwareInventoryView, AllActiveMachinesView etc.
             -- NICHT mehr / NOT anymore: hardware_inventory_view
```

---

## Schritt 2: User Story 1 validieren — Schreib-Methoden / Validate Write Methods

```sql
-- PostgreSQL-Shell vorbereiten / Prepare PostgreSQL shell:
psql -h localhost -U inventar -d inventar
```

```csharp
// C# Smoke Test (in einem Testprojekt / in a test project):
var svc = new PgSqlDbService("Host=localhost;Port=5432;Database=inventar;Username=inventar;Password=test;");
svc.InitializeDatabase();

var machine = new Machine { Name = "TEST-PC-01", OperatingSystem = "Windows 11" };
var machineId = await svc.SaveOrUpdateMachineAsync(machine, isHarvester: true);
// Erwartet / Expected: machineId > 0

var hardware = new HardwareInventory { /* ... */ };
await svc.SaveHardwareInventoryAsync(machineId, hardware);

var software = new SoftwareInventory { /* ... */ };
await svc.SaveSoftwareInventoryAsync(machineId, software);
```

```sql
-- Verifizieren / Verify:
SELECT * FROM Machines;               -- 1 Zeile / row
SELECT COUNT(*) FROM HardwareInventories;  -- 1
SELECT COUNT(*) FROM SoftwareInventories;  -- 1
```

---

## Schritt 3: User Story 2 validieren — HarvesterWorkerService / Validate Worker Integration

DE: Settings-Datei mit `WriteEnabled = true` vorbereiten.
EN: Prepare settings file with `WriteEnabled = true`.

Typischer Speicherort / Typical settings file location:
- Windows: `%ProgramData%\InventarWorkerService\settings.ini`
- macOS/Linux: `/var/lib/inventar/settings.ini`

```ini
[PgSqlDb]
PgSqlDbFqdn = localhost
PgSqlDbPort = 5432
PgSqlDbName = inventar
PgSqlUser = inventar
PgSqlPassword = test
WriteEnabled = true
```

```bash
# HarvesterWorkerService starten / Start HarvesterWorkerService:
dotnet run --project HarvesterWorkerService/HarvesterWorkerService.csproj
# Im Debug-Modus läuft der Loop alle 30 Sekunden.
# In debug mode the loop runs every 30 seconds.
```

Nach einem Ernte-Zyklus prüfen / After one harvest cycle, verify:

```sql
SELECT COUNT(*) FROM Machines;
SELECT COUNT(*) FROM HardwareInventories;
SELECT COUNT(*) FROM SoftwareInventories;
-- Alle drei Tabellen sollten Datensätze enthalten.
-- All three tables should contain records.
```

---

## Schritt 4: User Story 3 validieren — Lese-Methoden / Validate Read Methods

```csharp
var machines = await svc.GetMachinesAsync();
// Expected: Alle Maschinen / All machines

var activeMachines = await svc.GetAllActiveMachinesAsync();
// Expected: Nur aktive (Disabled=0, Deprovisioned=0)

var count = await svc.GetMachineCountAsync();
// Expected: Übereinstimmung mit GetMachinesAsync().Count

var hasRecords = await svc.HasMachineRecordsAsync();
// Expected: true

var latest = await svc.GetLatestHardwareInventoryAsync(machineId);
// Expected: nicht null, CreatedAt ist der neueste Timestamp
```

---

## Schritt 5: User Story 4 validieren — CSV-Import / Validate CSV Import

Test-CSV erstellen / Create test CSV:

```csv
Name,OperatingSystem,IPv4,IPv6,FQDN,Disabled,Deprovisioned
CSV-MACHINE-01,Windows 10,192.168.1.10,,csv-01.local,0,0
CSV-MACHINE-02,Windows 11,192.168.1.11,,csv-02.local,0,0
CSV-MACHINE-03,Linux,192.168.1.12,,csv-03.local,0,0
```

> **Hinweis / Note**: `MachineFromCsv.Disabled` und `.Deprovisioned` sind C# `bool`.
> CsvHelper konvertiert `"0"` → `false` und `"1"` → `true` automatisch per `BooleanConverter`.
> Die PostgreSQL-Spalten sind `BOOLEAN NOT NULL DEFAULT FALSE` —
> Npgsql mappt `bool` ↔ `BOOLEAN` direkt, keine manuelle Konvertierung nötig.

```csharp
var importedCount = await svc.InitializeMachinesFromCsvAsync("/path/to/test-machines.csv");
// Expected: importedCount == 3

// Erneuter Import / Re-import same file:
var importedCount2 = await svc.InitializeMachinesFromCsvAsync("/path/to/test-machines.csv");
// Expected: importedCount2 == 0 (Duplikate werden übersprungen / duplicates skipped)
```

---

## Schritt 6: User Story 5 validieren — PascalCase Views / Validate PascalCase Views

```sql
-- View-Name prüfen / Check view name:
SELECT * FROM HardwareInventoryView LIMIT 1;
-- Expected: Funktioniert / Works

SELECT * FROM hardware_inventory_view LIMIT 1;
-- Expected: ERROR:  relation "hardware_inventory_view" does not exist

-- Spaltenaliase prüfen / Check column aliases:
SELECT column_name FROM information_schema.columns
WHERE table_name = 'hardwareinventoryview';
-- Expected: MachineID, MachineName, Architecture, ProcessorCores,
--           TotalMemoryGB, AvailableMemoryGB, MemoryUsagePercent
```

---

## Schritt 7: WriteEnabled=false-Pfad / WriteEnabled=false Path

```ini
[PgSqlDb]
WriteEnabled = false
```

```bash
dotnet run --project HarvesterWorkerService/HarvesterWorkerService.csproj
# Expected: Kein Fehler, keine PgSQL-Datensätze, SQLite/MongoDB laufen normal.
# Expected: No error, no PgSQL records, SQLite/MongoDB run normally.
```

SQL-Negativverifikation — bestätigt, dass tatsächlich KEINE Daten nach PostgreSQL geschrieben wurden:
/ SQL negative verification — confirms that NO data was written to PostgreSQL:

```sql
-- Mit dem PostgreSQL-Server verbinden / Connect to the PostgreSQL server:
psql -h localhost -U inventar -d inventar

-- Prüfen, dass keine Datensätze vorhanden sind / Verify no records were written:
SELECT COUNT(*) FROM Machines;           -- Erwartet / Expected: 0
SELECT COUNT(*) FROM HardwareInventories;  -- Erwartet / Expected: 0
SELECT COUNT(*) FROM SoftwareInventories;  -- Erwartet / Expected: 0
```

> Wenn `WriteEnabled = false` korrekt funktioniert, liefern alle drei Abfragen 0.
> Jeder Wert > 0 zeigt an, dass der `WriteEnabled`-Guard nicht greift.
> / If `WriteEnabled = false` works correctly, all three queries return 0.
> Any value > 0 indicates the `WriteEnabled` guard is not working.

---

## Schritt 8: Unit-Tests & Coverage / Unit Tests & Coverage

```bash
dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults

# Coverage-Report generieren / Generate coverage report:
dotnet tool run reportgenerator \
  -reports:"./TestResults/**/coverage.cobertura.xml" \
  -targetdir:"./TestResults/CoverageReport"

# CI-Gate: >=70% erforderlich, >=80% angestrebt
# CI gate: >=70% required, >=80% targeted
```

---

## Schritt 9: Abschluss-Validierung / Final Validation

```bash
# Solution ohne Warnungen bauen / Build solution without warnings:
dotnet build InventarWorkerService.sln --no-incremental
# Expected: Build succeeded, 0 Warning(s)

# Paket-Aktualität prüfen / Check package currency:
dotnet list package --outdated

# DocFX generieren (nach API-Änderungen) / Generate DocFX (after API changes):
docfx docfx.json
```

---

## Validierungsprotokoll / Validation Log

Stand / Snapshot: `2026-04-19`

Die folgenden Ergebnisse dokumentieren die reale Ausfuehrung der Quickstart-Schritte
gegen den aktuellen Branch-Stand. Fuer die Worker-Laufschritte wurde bewusst eine
isolierte Umgebung mit `SERVICESTATUSDIRECTORY=InventarWorkerServiceQuickstart`
und der PostgreSQL-Testdatenbank `inventar_test` verwendet, damit keine regulaeren
lokalen Daten ueberschrieben werden.

The following results document the real execution of the quickstart steps against
the current branch state. For the worker runtime steps, an isolated environment
with `SERVICESTATUSDIRECTORY=InventarWorkerServiceQuickstart` and the PostgreSQL
test database `inventar_test` was used on purpose so that no regular local data
was overwritten.

| Schritt | Status | Ergebnis / Evidence |
|--------|--------|---------------------|
| 1 | PASS | Integrationstests (`30/30`) und Non-Integration-Tests (`28/28`) liefen gruen; manuelle `psql`-Pruefung zeigte die Tabellen `machines`, `hardwareinventories`, `softwareinventories` sowie die Views inklusive `hardwareinventoryview` und ohne `hardware_inventory_view`. |
| 2 | PASS | Temporaerer Smoke-Lauf via `PgSqlDbService` erfolgreich: `machineId=1001`, `machinesTableRows=4`, `hardwareTableRows=1`, `softwareTableRows=1`. |
| 3 | PASS | Isolierter Harvester-Lauf mit `pgSqlDb.writeEnabled=true` und lokalem Agenten auf Port `80` lief erfolgreich (`Collecting inventory completed successfully: 1 runs`); anschliessend PostgreSQL-Zaehler jeweils `1` fuer `machines`, `hardwareinventories`, `softwareinventories`. |
| 4 | PASS | Smoke-Lauf bestaetigte die Lese-Methoden: `machines=1`, `activeMachines=1`, `machineCount=1`, `hasMachineRecords=True`, `latestHardwareExists=True`. |
| 5 | PASS | CSV-Import im Smoke-Lauf: `csvImported=3`, erneuter Import `csvReimported=0`. |
| 6 | PASS | View-Verifikation im Smoke-Lauf: `pascalCaseViewCount=1`, `snakeCaseViewCount=0`, Spalten `machineid,machinename,architecture,processorcores,totalmemorygb,availablememorygb,memoryusagepercent`. |
| 7 | PASS | Isolierter Harvester-Lauf mit `pgSqlDb.writeEnabled=false` lief erfolgreich; anschliessende SQL-Negativverifikation ergab `0` Datensaetze in `machines`, `hardwareinventories`, `softwareinventories`. |
| 8 | FAIL | Coverage-Lauf und Reportgenerator liefen technisch erfolgreich, aber das Gate bleibt rot: Cobertura `line-rate="0.2266"` (`22.66 %`). Das entspricht dem weiterhin offenen Task `T046`. |
| 9 | PASS | `dotnet build InventarWorkerService.sln --no-incremental` erfolgreich mit `0 Warnung(en)` und `0 Fehler`; `dotnet list ... --outdated` zeigt nur noch die bewusst gepinnte Ausnahme `YamlDotNet 16.3.0 -> 17.0.1`; `docfx docfx.json` lief erfolgreich mit `0 warning(s)` und `0 error(s)`. |
