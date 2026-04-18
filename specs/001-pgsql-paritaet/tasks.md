# Tasks: PostgreSQL-Parität zum SqliteDbService

**Branch**: `001-pgsql-paritaet`
**Input**: `specs/001-pgsql-paritaet/` — plan.md, spec.md, data-model.md, contracts/, research.md, quickstart.md
**Prerequisites**: plan.md ✅ · spec.md ✅ · research.md ✅ · data-model.md ✅ · contracts/ ✅ · quickstart.md ✅

**Tests**: Red-Green-Refactor für alle 21 neuen Methoden. Unit-Tests ohne echte DB;
Integrationstests mit `[TestCategory("Integration")]` und `PGSQL_TEST_CONNECTION_STRING`.
Coverage CI-Gate ≥70%; Ziel ≥80% (kombinierter Lauf Unit + Integration).

**Organization**: Tasks sind nach User Stories gruppiert. Jede Story ist unabhängig testbar und lieferbar.

## Format: `[ID] [P?] [Story?] Beschreibung mit Dateipfad`

- **[P]**: Parallelisierbar (andere Methoden/Dateien, keine gegenseitigen Abhängigkeiten)
- **[US1..5]**: User Story gemäß spec.md
- Genaue Dateipfade in jeder Beschreibung angegeben

---

## Phase 1: Setup (Shared Infrastructure)

**Zweck**: Test-Klassen-Skeleton; keine Änderungen an bestehender Projektstruktur nötig
(Branch, .NET 10, Npgsql/Dapper/CsvHelper/MSTest — alles bereits konfiguriert)

- [ ] T001 Erstelle `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — `[TestClass]`-Skeleton mit `ClassInitialize`/`ClassCleanup`, Lesen von Umgebungsvariable `PGSQL_TEST_CONNECTION_STRING` in `ClassInitialize`, `PgSqlDbService _svc`-Instanzfeld; `ClassCleanup` führt `TRUNCATE TABLE SoftwareInventories, HardwareInventories, Machines RESTART IDENTITY CASCADE` auf Test-DB `inventar_test` aus

---

## Phase 2: Foundational (Blocking Prerequisites)

**Zweck**: ServiceContainer nullable + Initialize.cs Guards + InitializeDatabase()-Update.
Diese Phase muss vor allen User-Story-Phasen abgeschlossen sein.

**⚠️ CRITICAL**: Kein US-Task kann beginnen, bevor diese Phase vollständig ist.

- [ ] T002 Update `ServiceContainer` in `InventarWorkerCommon/Services/Common/Initialize.cs` — Property `PgSqlDbService PgSqlDbService { get; }` → `PgSqlDbService? PgSqlDbService { get; }`; Konstruktor: `PgSqlDbService = pgSqlDbService ?? throw new ArgumentNullException(...)` → `PgSqlDbService = pgSqlDbService;` (null erlaubt); `Dispose(bool)`: try-Block für PgSqlDbService hinzufügen (analog zu ApiService/DbService); `DisposeAsyncCore()`: PgSqlDbService-Block in disposeTasks-Liste hinzufügen
- [ ] T003 [P] Update `Initialize.Services(Settings settings)` in `InventarWorkerCommon/Services/Common/Initialize.cs` — vor PgSQL-Init prüfen: `if (!settings.PgSqlDb.WriteEnabled) return new ServiceContainer(apiService, dbService, mongoDbService, pgSqlDbService: null);`; sonst weiter wie bisher
- [ ] T004 [P] Update `Initialize.Services()` (parameterlos) in `InventarWorkerCommon/Services/Common/Initialize.cs` — PgSQL-Initialisierungsblock entfernen; immer `pgSqlDbService: null` an `ServiceContainer` übergeben
- [ ] T005 Update `InitializeDatabase()` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — Zeile `CREATE OR REPLACE VIEW hardware_inventory_view AS ...` ersetzen: (a) `DROP VIEW IF EXISTS hardware_inventory_view;` einfügen, (b) neuen View-Block erstellen: `CREATE OR REPLACE VIEW HardwareInventoryView AS SELECT DISTINCT ON (m.id) m.id AS MachineID, m.name AS MachineName, h.Architecture, h.ProcessorCores, ROUND(h.TotalMemoryGB::numeric/1024/1024/1024,2) AS TotalMemoryGB, ROUND(h.AvailableMemoryGB::numeric/1024/1024/1024,2) AS AvailableMemoryGB, ROUND(h.MemoryUsagePercent::numeric,2) AS MemoryUsagePercent FROM Machines m JOIN HardwareInventories h ON h.MachineId = m.Id ORDER BY m.Id, h.CreatedAt DESC;`
- [ ] T006 [P] Unit-Test: `Services_Parameterless_PgSqlDbServiceIsNull` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — `Initialize.Services()` aufrufen; `Assert.IsNull(container.PgSqlDbService)` — kein DB-Server nötig
- [ ] T007 [P] Unit-Test: `Services_WriteEnabledFalse_PgSqlDbServiceIsNull` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — `Settings` mit `WriteEnabled = false` konstruieren (ohne echte DB); `Assert.IsNull(container.PgSqlDbService)`

**Checkpoint**: ServiceContainer nullable, Initialize.cs Guards aktiv, HardwareInventoryView korrekt definiert — User-Story-Implementierung kann beginnen.

---

## Phase 3: User Story 1 — Inventardaten in PostgreSQL schreiben (P1) 🎯 MVP

**Ziel**: `SaveOrUpdateMachineAsync`, `SaveHardwareInventoryAsync`, `SaveSoftwareInventoryAsync`
**Independent Test**: `dotnet test --filter "TestCategory=Integration&FullyQualifiedName~SaveOrUpdate"` — prüft UPSERT, ID-Sync, Hardware- und Software-Insert gegen echte DB

### Tests für US1 ⚠️ (erst schreiben, dann RED bestätigen, dann implementieren)

- [ ] T008 [P] [US1] Unit-Test: `SaveOrUpdateMachineAsync_NullMachine_ThrowsArgumentNullException` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`
- [ ] T009 [P] [US1] Unit-Test: `SaveOrUpdateMachineAsync_ZeroId_ThrowsArgumentException` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`
- [ ] T010 [P] [US1] Integration-Test `[TestCategory("Integration")]`: `SaveOrUpdateMachineAsync_NewMachine_InsertsWithExplicitId` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — Machine mit Id=42 übergeben; prüfen dass INSERT Id=42 in Machines; zurückgegebene Id == 42
- [ ] T011 [P] [US1] Integration-Test: `SaveOrUpdateMachineAsync_ExistingName_UpdatesRecordNoDuplicate` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`
- [ ] T012 [P] [US1] Integration-Test: `SaveOrUpdateMachineAsync_IsHarvesterTrue_UpdatesNetworkFields` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`
- [ ] T013 [P] [US1] Integration-Test: `SaveHardwareInventoryAsync_ValidData_InsertsRecord` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — prüfen dass CreatedAt UTC ist
- [ ] T014 [P] [US1] Integration-Test: `SaveSoftwareInventoryAsync_ValidData_SerializesAllJsonFields` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`

### Implementierung für US1

- [ ] T015 [US1] Implementiere `SaveOrUpdateMachineAsync(Machine machine, bool isHarvester = false)` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — `ArgumentNullException` wenn machine null; `ArgumentException` wenn machine.Id == 0; `SELECT Id FROM Machines WHERE Name = @Name`; wenn vorhanden: UPDATE OperatingSystem, LastSeen (+ IPv4/IPv6/FQDN/LastHarvested wenn isHarvester), return existingId; wenn neu: `INSERT INTO Machines (Id, Name, OperatingSystem, LastSeen, CreatedAt) VALUES (@Id, ...) RETURNING Id`; alle DateTime = `DateTime.UtcNow`; bilingual XML-Dokumentation DE/EN CEFR-B2
- [ ] T016 [P] [US1] Implementiere `SaveHardwareInventoryAsync(int machineId, HardwareInventory hardware)` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — INSERT INTO HardwareInventories mit allen Feldern aus `hardware.System.*`, `hardware.Cpu.*`, `hardware.Memory.*`; `CreatedAt = DateTime.UtcNow`; bilingual XML-Dokumentation
- [ ] T017 [P] [US1] Implementiere `SaveSoftwareInventoryAsync(int machineId, SoftwareInventory software)` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — INSERT INTO SoftwareInventories; alle Json-Felder via `System.Text.Json.JsonSerializer.Serialize(...)`; `CreatedAt = DateTime.UtcNow`; bilingual XML-Dokumentation

**Checkpoint**: User Story 1 vollständig. `dotnet test --filter "TestCategory=Integration"` — alle US1-Tests grün.

---

## Phase 4: User Story 2 — HarvesterWorkerService schreibt parallel nach PostgreSQL (P2)

**Ziel**: Worker.cs schreibt bei `WriteEnabled=true` nach PostgreSQL; PgSQL-Fehler isoliert; SQLite/MongoDB unberührt
**Independent Test**: `dotnet run --project HarvesterWorkerService/...` mit `WriteEnabled=true` → nach einem Zyklus Datensätze in allen drei DBs verifizieren (quickstart.md §Schritt 3)

### Tests für US2

- [ ] T018 [P] [US2] Unit-Test: `Worker_PgSqlDbServiceIsNull_DoesNotThrow` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — ServiceContainer mit null PgSqlDbService; Worker-Loop-Logik (gemockt oder via direkte Methode); prüfen keine NullReferenceException

### Implementierung für US2

- [ ] T019 [US2] Update `HarvesterWorkerService/Worker.cs` — nach `_machineId = await _sqliteDbService.SaveOrUpdateMachineAsync(machine, isHarvester: true);` einfügen: `machine.Id = _machineId;`; danach den null-geschützten PgSQL-Block hinzufügen: `if (_pgSqlDbService != null) { try { await _pgSqlDbService.SaveOrUpdateMachineAsync(machine, isHarvester: true); await _pgSqlDbService.SaveHardwareInventoryAsync(_machineId, hardwareInventory); await _pgSqlDbService.SaveSoftwareInventoryAsync(_machineId, softwareInventory); } catch (Exception pgException) { HandleException(pgException); } }`

**Checkpoint**: Worker schreibt parallel in alle drei DBs. PgSQL-Exception loggt und setzt Status = Error; SQLite/MongoDB-Writes laufen weiter.

---

## Phase 5: User Story 3 — Inventardaten aus PostgreSQL lesen (P3)

**Ziel**: Alle 16 Lese-, Lookup-, Zähl- und Wartungsmethoden (FR-004 bis FR-009)
**Independent Test**: `dotnet test --filter "TestCategory=Integration&FullyQualifiedName~GetMachines"` nach Einfügen von Testdaten

### Tests für US3

- [ ] T020 [P] [US3] Integration-Tests: `GetMachinesAsync_ReturnsAllOrderedByName`, `GetAllActiveMachinesAsync_ExcludesDisabledAndDeprovisioned`, `GetAllActiveMachinesWithNetworkInfoAsync_RequiresAtLeastOneNetworkValue` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`
- [ ] T021 [P] [US3] Integration-Tests: `GetAllDisabledMachinesAsync_ReturnsOnlyDisabledNotDeprovisioned`, `GetAllDeprovisionedMachinesAsync_ReturnsOnlyDeprovisioned` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`
- [ ] T022 [P] [US3] Integration-Tests: `GetMachineByIdAsync_ExistingId_ReturnsMachine`, `GetMachineByIdAsync_NonExistingId_ReturnsNull`, `GetMachineByNameAsync_ExistingName_ReturnsMachine` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`
- [ ] T023 [P] [US3] Integration-Tests: `GetLatestHardwareInventoryAsync_ReturnsNewestEntry`, `GetLatestSoftwareInventoryAsync_ReturnsNewestEntry` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — je zwei Einträge einfügen; prüfen dass der neuere zurückkommt
- [ ] T024 [P] [US3] Integration-Tests: `HasMachineRecordsAsync_RecordsExist_ReturnsTrue`, `HasMachineRecordsAsync_EmptyTable_ReturnsFalse`, `HasHardwareInventoryRecordsAsync_RecordsExist_ReturnsTrue`, `HasSoftwareInventoryRecordsAsync_RecordsExist_ReturnsTrue` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`
- [ ] T025 [P] [US3] Integration-Tests: `GetMachineCountAsync_ReturnsCorrectCount`, `GetHardwareInventoryCountAsync_ReturnsCorrectCount`, `GetSoftwareInventoryCountAsync_ReturnsCorrectCount` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`
- [ ] T026 [P] [US3] Integration-Test: `CleanupOldRecordsAsync_ZeroDays_DeletesAllInventoryEntries` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — Einträge anlegen; `CleanupOldRecordsAsync(0)` aufrufen; prüfen Count = 0 für Hardware und Software
- [ ] T026b [P] [US3] Unit-Test: `CleanupOldRecordsAsync_ZeroDays_CutoffIsUtcNow` und `CleanupOldRecordsAsync_NegativeDays_CutoffIsInFuture` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — kein DB-Server nötig; Implementierung in `CleanupOldRecordsAsync` muss eine `internal static DateTime CalculateCutoff(int daysToKeep)` Hilfsmethode bereitstellen (z. B. `internal static DateTime CalculateCutoff(int daysToKeep) => DateTime.UtcNow.AddDays(-daysToKeep)`); Test 1: `CalculateCutoff(0)` ist ≤ `DateTime.UtcNow` (cutoff = jetzt, löscht alle); Test 2: `CalculateCutoff(-5)` ist > `DateTime.UtcNow` (cutoff in der Zukunft — verhält sich wie daysToKeep=0); verifiziert plan.md §Test-Strategie und research.md R-07 Randfall negativeWerte
- [ ] T027 [P] [US3] Integration-Test: `CleanupOldRecordsAsync_PositiveDays_KeepsRecentEntries` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs`

### Implementierung für US3

- [ ] T028 [P] [US3] Implementiere `GetMachinesAsync()` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — `SELECT * FROM Machines ORDER BY Name`; bilingual XML-Dokumentation
- [ ] T029 [P] [US3] Implementiere `GetAllActiveMachinesAsync()`, `GetAllActiveMachinesWithNetworkInfoAsync()` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — `SELECT * FROM AllActiveMachinesView ORDER BY Name` bzw. `AllActiveMachinesWithNetworkInfoView`; bilingual XML-Dokumentation
- [ ] T030 [P] [US3] Implementiere `GetAllDisabledMachinesAsync()`, `GetAllDeprovisionedMachinesAsync()` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — SELECT aus AllDisabledMachinesView, AllDeprovisionedMachinesView; bilingual XML-Dokumentation
- [ ] T031 [P] [US3] Implementiere `GetMachineByIdAsync(int id)`, `GetMachineByNameAsync(string machineName)` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — `QuerySingleOrDefaultAsync<Machine>`; bilingual XML-Dokumentation
- [ ] T032 [P] [US3] Implementiere `GetLatestHardwareInventoryAsync(int machineId)`, `GetLatestSoftwareInventoryAsync(int machineId)` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — `SELECT * FROM HardwareInventories WHERE MachineId = @MachineId ORDER BY CreatedAt DESC LIMIT 1` bzw. SoftwareInventories; bilingual XML-Dokumentation
- [ ] T033 [P] [US3] Implementiere `HasMachineRecordsAsync()`, `HasHardwareInventoryRecordsAsync()`, `HasSoftwareInventoryRecordsAsync()` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — `SELECT EXISTS(SELECT 1 FROM ...)` via `QuerySingleAsync<bool>`; bilingual XML-Dokumentation
- [ ] T034 [P] [US3] Implementiere `GetMachineCountAsync()`, `GetHardwareInventoryCountAsync()`, `GetSoftwareInventoryCountAsync()` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — `SELECT COUNT(*) FROM ...` via `QuerySingleAsync<int>`; bilingual XML-Dokumentation
- [ ] T035 [US3] Implementiere `CleanupOldRecordsAsync(int daysToKeep = 30)` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — extrahiere Cutoff-Logik als `internal static DateTime CalculateCutoff(int daysToKeep) => DateTime.UtcNow.AddDays(-daysToKeep);` (benötigt von T026b Unit-Test); nutze `var cutoff = CalculateCutoff(daysToKeep);`; `DELETE FROM HardwareInventories WHERE CreatedAt < @CutoffDate`; `DELETE FROM SoftwareInventories WHERE CreatedAt < @CutoffDate`; bilingual XML-Dokumentation

**Checkpoint**: Alle 16 Lese- und Wartungsmethoden funktionieren unabhängig testbar.

---

## Phase 6: User Story 4 — Maschinen per CSV importieren (P4)

**Ziel**: `InitializeMachinesFromCsvAsync` mit Transaktionsschutz (FR-010, SC-004)
**Independent Test**: `dotnet test --filter "TestCategory=Integration&FullyQualifiedName~InitializeMachinesFromCsv"` — Import, Duplikat-Skip, Rollback auf Test-DB

### Tests für US4

- [ ] T036 [P] [US4] Unit-Test: `InitializeMachinesFromCsvAsync_FileNotFound_ThrowsFileNotFoundException` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — kein DB-Server nötig
- [ ] T037 [P] [US4] Integration-Test: `InitializeMachinesFromCsvAsync_ValidCsv3Machines_Returns3` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — temporäre CSV-Datei mit 3 Einträgen; prüfen Count = 3
- [ ] T038 [P] [US4] Integration-Test: `InitializeMachinesFromCsvAsync_DuplicateMachines_SkipsExisting` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — erste Maschine vorab anlegen; CSV mit 3 Maschinen (1 vorhanden, 2 neu); prüfen importedCount = 2
- [ ] T038b [P] [US4] Integration-Test: `InitializeMachinesFromCsvAsync_ExceptionDuringImport_RollsBackTransaction` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — erste Maschine mit Name `"TestRollback-Existing"` vorab per INSERT anlegen; CSV-Datei mit zwei Einträgen erstellen: Eintrag 1 = neuer Name `"TestRollback-New"`, Eintrag 2 = bereits vorhandener Name `"TestRollback-Existing"` (verletzt UNIQUE-Constraint auf `Machines.Name`); prüfen dass nach dem fehlgeschlagenen Import `SELECT COUNT(*) FROM Machines WHERE Name = 'TestRollback-New'` = 0 (Transaktion vollständig zurückgerollt, auch der erste Eintrag ist weg); verifiziert SC-004 und US4 Acceptance Scenario 3
- [ ] T039 [P] [US4] Integration-Test: `InitializeMachinesFromCsvAsync_DisabledDeprovisioned_MappedAsBool` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — CSV mit "0"/"1" für Disabled/Deprovisioned; prüfen BOOLEAN-Spalten korrekt gesetzt (CsvHelper-Konvertierung verifizieren)

### Implementierung für US4

- [ ] T040 [US4] Implementiere `InitializeMachinesFromCsvAsync(string csvFilePath)` in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` — `if (!File.Exists(csvFilePath)) throw new FileNotFoundException(...)`; `await using var connection = new NpgsqlConnection(_connectionString); await connection.OpenAsync();`; `await using var transaction = await connection.BeginTransactionAsync();`; CsvHelper + `csv.Context.RegisterClassMap<MachineMapFromCsv>()`; `csv.GetRecords<MachineFromCsv>().ToList()` vor der DB-Schleife; pro Maschine: `SELECT Id WHERE Name` — skip wenn vorhanden, sonst INSERT; `await transaction.CommitAsync()`; catch: `await transaction.RollbackAsync(); throw new Exception("Import fehlgeschlagen — Transaktion zurückgerollt.", ex);`; return importedCount; bilingual XML-Dokumentation

**Checkpoint**: CSV-Import funktioniert, Transaktionsschutz verifiziert (SC-004).

---

## Phase 7: User Story 5 — View-Namen auf PascalCase vereinheitlichen (P5)

**Ziel**: `HardwareInventoryView` (PascalCase) existiert, `hardware_inventory_view` (snake_case) nicht mehr (FR-013)
**Implementierung**: bereits in T005 erledigt — diese Phase enthält nur Verifikationstests

### Tests für US5

- [ ] T041 [P] [US5] Integration-Test: `InitializeDatabase_HardwareInventoryViewExists_OldViewDropped` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — `SELECT COUNT(*) FROM information_schema.views WHERE table_schema = 'public' AND table_name = 'hardwareinventoryview'` == 1; `table_name = 'hardware_inventory_view'` == 0
- [ ] T042 [P] [US5] Integration-Test: `HardwareInventoryView_ColumnAliasesArePascalCase` in `InventarWorkerCommonTest/PgSqlDbServiceTest.cs` — `SELECT column_name FROM information_schema.columns WHERE table_name = 'hardwareinventoryview'`; Assert enthält: `machineid`, `machinename`, `architecture`, `processorcores`, `totalmemorygb`, `availablememorygb`, `memoryusagepercent` (PostgreSQL normalisiert Bezeichner auf Lowercase bei SELECT ohne Anführungszeichen)

**Checkpoint**: HardwareInventoryView PascalCase-Aliase verifiziert, alte View nicht mehr vorhanden.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Zweck**: Build-Sauberkeit, Coverage, Dokumentation, Statistiken, Archivierung

- [ ] T043 [P] Build-Check: `dotnet build InventarWorkerService.sln --no-incremental` in Repo-Root — prüfen "Build succeeded, 0 Warning(s)"; alle CS1591-Warnungen (fehlende XML-Docs) und CS8600/CS8602-Nullable-Warnungen in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` und `InventarWorkerCommon/Services/Common/Initialize.cs` beheben
- [ ] T044 [P] Unit-Test-Lauf: `dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj --filter "TestCategory!=Integration"` — alle Unit-Tests müssen grün sein; fehlende Unit-Tests (T006, T007, T008, T009, T036) nachprüfen
- [ ] T045 Integration-Test-Lauf: `export PGSQL_TEST_CONNECTION_STRING="Host=localhost;Port=5432;Database=inventar_test;Username=...;Password=..."; dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj --filter "TestCategory=Integration"` — alle Integration-Tests müssen grün sein
- [ ] T046 [P] Coverage-Report: `dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj --collect:"XPlat Code Coverage" --results-directory ./TestResults` — `reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./TestResults/CoverageReport"`; Coverage ≥70% (kombinierter Lauf); dokumentieren als CI-Evidenz
- [ ] T047 [P] Paket-Aktualität: `dotnet list package --outdated` in Repo-Root ausführen; Ergebnis dokumentieren; sichere Upgrades durchführen; bewusste Pinning-Ausnahmen in Plan kommentieren
- [ ] T048 [P] DocFX: `docfx docfx.json` in Repo-Root ausführen — prüfen keine fehlenden XML-Docs für PgSqlDbService-Methoden; HTML-Output auf CS1591-Lücken prüfen; fix bei Bedarf
- [ ] T049 [P] Version-Sync: `Directory.Build.props` aktualisieren — `Minor = 1` (Branch 001), `Patch` = aktueller Commit-Count auf Branch, `Build` inkrementieren; alle drei Felder konsistent halten
- [ ] T050 [P] Quickstart-Validierung: `specs/001-pgsql-paritaet/quickstart.md` Schritte 1–9 durchführen; Ergebnisse für jeden Schritt (PASS/FAIL) notieren; Schritt 7 SQL-Negativverifikation (SELECT COUNT(*) FROM Machines = 0 bei WriteEnabled=false) bestätigen
- [ ] T051 [P] Statistiken: `docs/project-statistics.md` aktualisieren — Eintrag für `001-pgsql-paritaet` mit Branch-Scope, Produktiv-/Test-/Doku-Zeilenzahlen, Observable-Work-Window, Beschleunigungsfaktor vs. 80 Zeilen/Tag (konservativ) und 100 Zeilen/Tag (Thorsten-Solo-Baseline)
- [ ] T052 Lastenheft archivieren: `bash scripts/rename-lastenheft.sh Lastenheft_PostgreSQL_Implementation.md 001-pgsql-paritaet` (macOS/Linux) — umbenennen zu `Lastenheft_PostgreSQL_Implementation.001-pgsql-paritaet.md`

---

## Dependencies & Execution Order

### Phase-Abhängigkeiten

```
Phase 1 (Setup)
    └── Phase 2 (Foundational) ← BLOCKIERT alle US-Phasen
            ├── Phase 3 (US1 — Write Methods) ── 🎯 MVP
            ├── Phase 4 (US2 — Worker)          ── depends on Phase 3 complete
            ├── Phase 5 (US3 — Read Methods)    ── depends on Phase 3 for test data
            ├── Phase 6 (US4 — CSV Import)       ── independent after Phase 2
            └── Phase 7 (US5 — View Verify)     ── only needs Phase 2 (T005)
                        └── Phase 8 (Polish) ← alle US-Phasen müssen done sein
```

### User-Story-Abhängigkeiten

| Story | Hängt ab von | Kann beginnen nach |
|-------|--------------|-------------------|
| US1 (P1) | Phase 2 | T002–T005 done |
| US2 (P2) | Phase 2 + US1 | T015–T017 done (braucht SaveOrUpdate/Hardware/Software) |
| US3 (P3) | Phase 2 + US1 | T015–T017 done (braucht Testdaten via Write-Methoden) |
| US4 (P4) | Phase 2 | T002–T005 done |
| US5 (P5) | Phase 2 | T005 done |

> **Hinweis T026b / T038b**: Die nachträglich hinzugefügten Tasks folgen denselben Phasen-Regeln wie die übrigen Test-Tasks ihrer jeweiligen Phase. T026b (Unit-Test) gehört zu Phase 5 und kann parallel zu T020–T027 geschrieben werden. T038b (Integration-Test) gehört zu Phase 6 und kann parallel zu T036–T039 geschrieben werden. Beide sind in der Implementierungsreihenfolge vor ihren jeweiligen Impl-Tasks auszuführen (Red-Green-Refactor).

### Innerhalb jeder Phase

1. Tests schreiben → RED bestätigen
2. Methode implementieren → Tests grün
3. XML-Dokumentation vollständig
4. Build ohne Warnungen
5. Commit nach jeder abgeschlossenen Methode oder logischer Gruppe

---

## Parallel Opportunities

### Parallele Ausführung: Phase 2 (Foundational)

```
# Zuerst (blockierend — alle anderen Phase-2-Tasks hängen von T002 ab):
T002  ServiceContainer nullable — MUSS als erstes in Phase 2 implementiert werden

# Danach parallel (nach T002 abgeschlossen):
T003  Update Initialize.Services(Settings) — WriteEnabled-Guard
T004  Update Initialize.Services() parameterlos — immer null
T005  InitializeDatabase() — unabhängig von T003/T004, kann parallel zu diesen laufen
T006  Unit-Test: Services_Parameterless_PgSqlDbServiceIsNull
T007  Unit-Test: Services_WriteEnabledFalse_PgSqlDbServiceIsNull
```

### Parallele Ausführung: Phase 3 (US1)

```
# Tests: alle parallel
T008, T009, T010, T011, T012, T013, T014

# Implementierung nach Tests (T016/T017 parallel, T015 zuerst für ID-Contract):
T015  SaveOrUpdateMachineAsync (zuerst — ID-Sync-Contract muss klar sein)
T016  SaveHardwareInventoryAsync  ←─ parallel
T017  SaveSoftwareInventoryAsync  ←─ parallel
```

### Parallele Ausführung: Phase 5 (US3)

```
# Tests: alle parallel
T020–T027

# Implementierung: alle parallel (verschiedene Methoden in gleicher Datei)
T028–T035
```

---

## Implementation Strategy

### MVP First (User Story 1 + 2)

1. Phase 1 (T001): Test-Skeleton
2. Phase 2 (T002–T007): ServiceContainer + InitializeDatabase
3. Phase 3 (T008–T017): Write-Methoden ← **MVP ready**
4. Phase 4 (T018–T019): Worker-Integration ← **End-to-End ready**
5. **STOP & VALIDIEREN**: `dotnet run HarvesterWorkerService` — Daten in PostgreSQL ✓

### Incremental Delivery

```
Phase 1+2 → Foundation fertig
Phase 3    → US1 done → MVP (Write-Methoden)
Phase 4    → US2 done → Worker schreibt in alle 3 DBs
Phase 5    → US3 done → Vollständige Parität zu SqliteDbService
Phase 6    → US4 done → CSV-Import
Phase 7    → US5 done → View-Verifikation
Phase 8    → Polish → PR-ready
```

---

## Statistik / Statistics

| Phase | Tasks | Unit-Tests | Integration-Tests | Impl-Tasks |
|-------|-------|------------|-------------------|------------|
| Phase 1 Setup | 1 | — | — | 1 |
| Phase 2 Foundational | 6 | 2 | — | 4 |
| Phase 3 US1 | 10 | 2 | 5 | 3 |
| Phase 4 US2 | 2 | 1 | — | 1 |
| Phase 5 US3 | 17 | 1 | 8 | 8 |
| Phase 6 US4 | 6 | 1 | 4 | 1 |
| Phase 7 US5 | 2 | — | 2 | — |
| Phase 8 Polish | 10 | — | — | 10 |
| **Gesamt** | **54** | **7** | **19** | **28** |

---

## Notes

- `[P]`-Tasks = verschiedene Methoden/Dateien oder keine gegenseitigen Abhängigkeiten
- `[US?]`-Label ermöglicht Traceability zu spec.md-User-Stories
- Jede User Story ist unabhängig fertigstellbar und testbar
- Tests müssen RED sein, bevor Implementierung beginnt (Red-Green-Refactor)
- Commit nach jeder Task oder logischer Gruppe — nie uncommittete Änderungen liegenlassen
- Integrationstests brauchen `PGSQL_TEST_CONNECTION_STRING` auf `inventar_test`-Datenbank
- `ClassCleanup` truncatiert Test-DB nach jedem Testlauf
- Jede neue public Methode in PgSqlDbService.cs benötigt bilingual XML-Dokumentation (DE/EN, CEFR-B2)
