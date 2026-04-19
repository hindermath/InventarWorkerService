# Implementation Plan: PostgreSQL-Parität zum SqliteDbService

**Branch**: `001-pgsql-paritaet` | **Date**: 2026-04-18 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-pgsql-paritaet/spec.md`

---

## Summary

`PgSqlDbService` erhält alle 21 öffentlichen Methoden des `SqliteDbService`, sodass PostgreSQL als
vollwertiger paralleler Persistenz-Provider neben SQLite und MongoDB genutzt werden kann. Der
`HarvesterWorkerService` schreibt Inventardaten bei aktiviertem `WriteEnabled` simultan in alle drei
Datenbanken. PostgreSQL-Fehler sind isoliert — SQLite/MongoDB-Writes laufen weiter. Die View
`hardware_inventory_view` wird auf `HardwareInventoryView` (PascalCase) umbenannt.

Technisch: Dapper + Npgsql, `RETURNING Id` statt `last_insert_rowid()`, `DateTime.UtcNow` für
alle `timestamptz`-Spalten, null-Pattern für `PgSqlDbService?` im `ServiceContainer`.

---

## Technical Context

**Language/Version**: C# 14.0 on .NET 10
**Primary Dependencies**: Npgsql 10.0.2, Dapper 2.1.72, CsvHelper 33.1.0,
  MSTest 4.2.1, System.Text.Json (BCL, kein separates Paket)
**Storage**: PostgreSQL 14+ (Ziel / target), SQLite (bestehend), MongoDB (bestehend)
**Testing**: MSTest 4.1.0 + coverage gates (>=70% minimum, target >=80%);
  Unit-Tests ohne echte DB; Integrationstests (`[TestCategory("Integration")]`) mit echter PgSQL-Instanz
**Target Platform**: Cross-platform (Windows Service / systemd / launchd)
**Project Type**: Shared library (`InventarWorkerCommon`) + Background service (`HarvesterWorkerService`)
**Performance Goals**: Schreib-Durchsatz vergleichbar mit SQLite (async I/O); Lese-Latenz < 500 ms für
  Listen-Abfragen bei ~200 Maschinen.
  *Hinweis / Note*: Der Schreib-Durchsatz ist bewusst nicht absolut quantifiziert. Bei Single-Writer,
  ~200 Maschinen und async I/O mit Dapper ist kein absoluter Schwellenwert notwendig. Eine gespeicherte
  SQLite-Basismessung existiert nicht — "vergleichbar mit SQLite" bedeutet: async Dapper-Schreibzugriff
  auf lokaler PostgreSQL-Instanz unter gleicher Last. [CHK004/CHK005: Akzeptierte Unschärfe — MVP-Scope]
**Constraints**: Kein `IDbService`-Interface in diesem Feature (deferred); `InitializeDatabase()`
  bleibt synchron (Parität zu SQLite); PostgreSQL 14+ vorausgesetzt
**Scale/Scope**: ~200 Maschinen, ~10K Inventar-Datensätze pro Tag, Single-Writer-Modell

---

## Constitution Check

*GATE: Alle Gates bestehen vor Phase 0. Re-Check nach Phase 1 unten.*

- **Branching Gate**: ✅ Branch `001-pgsql-paritaet` existiert und ist nicht `main`.
  Merge-Pfad: PR zu `main`. Keine Feature-Commits direkt auf `main`.

- **Toolchain Gate**: ✅ `InventarWorkerCommon` und `HarvesterWorkerService` sind bereits auf
  .NET 10 / C# 14.0 (`LangVersion 14.0`). Kein Upgrade erforderlich.

- **Architecture Gate**: ✅ Alle Kernänderungen in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs`
  und `InventarWorkerCommon/Services/Common/Initialize.cs`. Worker-Anpassungen in
  `HarvesterWorkerService/Worker.cs`. Keine Verletzung der Layer-Grenzen.

- **Documentation Gate**: ✅ FR-017 mandatiert vollständige XML-Dokumentation für alle 21 neuen
  öffentlichen Methoden. Zweisprachig DE/EN, CEFR B2 (constitution Principle I + III).

- **XML/DocFX Gate**: ✅ `docfx docfx.json` wird als letzter Schritt der Polish-Phase ausgeführt,
  nachdem alle API-Signaturen und XML-Kommentare finalisiert sind (CA-007).

- **Testing/Coverage Gate**: ✅ Red-Green-Refactor für alle 21 neuen Methoden definiert.
  Coverage-Plan: Unit-Tests (Logik, null-Checks) + Integrationstests (full method coverage).
  CI-Gate ≥70%; Ziel ≥80%.

- **Dependency Currency Gate**: ✅ Die sichere Patch-/Minor-Welle wurde nachgezogen:
  `.NET 10`-Pakete auf `10.0.6`, `Npgsql` auf `10.0.2`, `MongoDB.Driver` auf `3.7.1`,
  `Swashbuckle.AspNetCore` auf `10.1.7`, `Microsoft.Playwright.MSTest` auf `1.59.0`
  und MSTest / `Microsoft.NET.Test.Sdk` auf `4.2.1` / `18.4.0`.
  Bewusste Pinning-Ausnahme: `YamlDotNet` bleibt vorerst auf `16.3.0`, weil `17.0.1`
  ein Major-Upgrade ist und ausserhalb des PostgreSQL-Paritaets-MVP liegt.
  Der Restbestand nach `dotnet list package --outdated` besteht nur noch aus diesem Pin.

- **Data Contract Gate**: ✅ `System.Text.Json` mit camelCase für JSON-Serialisierung (FR-003).
  Dapper mit expliziten SQL-Strings. PascalCase für Tabellen/Spalten. View-Umbenennung per
  `DROP VIEW IF EXISTS hardware_inventory_view` + `CREATE OR REPLACE VIEW HardwareInventoryView` (FR-013).

- **Statistical Documentation Gate**: ✅ `docs/project-statistics.md` wird als Teil der Polish-Phase
  mit Branch-Scope, Code/Test/Doc-Zeilenzahlen und manuellem Baseline-Eintrag aktualisiert.

**Post-Phase-1 Re-Check**: Alle Gates bestehen nach dem Design. Kein Eintrag in Complexity Tracking
erforderlich.

---

## Project Structure

### Documentation (this feature)

```text
specs/001-pgsql-paritaet/
├── plan.md              # Dieses Dokument / This document
├── spec.md              # Feature-Spezifikation / Feature specification
├── research.md          # Phase 0 — Alle Entscheidungen dokumentiert / All decisions documented
├── data-model.md        # Phase 1 — Datenbankschema und Entitäten / DB schema and entities
├── quickstart.md        # Phase 1 — Validierungsanleitung / Validation guide
├── contracts/
│   └── PgSqlDbService-methods.md  # Alle 21 Methodensignaturen / All 21 method signatures
├── checklists/
│   └── requirements.md  # Vollständig abgehakt / Fully checked
└── tasks.md             # Phase 2 — Wird von /speckit.tasks erstellt / Created by /speckit.tasks
```

### Source Code

```text
InventarWorkerCommon/
├── Services/
│   ├── Database/
│   │   └── PgSqlDbService.cs            # PRIMÄR: 20 neue Methoden + InitializeDatabase()-Erweiterung
│   └── Common/
│       └── Initialize.cs                # WriteEnabled-Guard + Fallback-Pfad; ServiceContainer.PgSqlDbService?
└── Models/
    └── SqlDatabase/                     # Keine neuen Modelle — alle bestehend (Machine, HardwareInventories,
        ├── Machine.cs                   # SoftwareInventories, MachineState, MachineFromCsv)
        ├── HardwareInventories.cs
        ├── SoftwareInventories.cs
        ├── MachineState.cs
        └── MachineFromCsv.cs

HarvesterWorkerService/
└── Worker.cs                            # PgSQL-Schreibaufrufe + null-Check vor jedem PgSQL-Call

InventarWorkerCommonTest/
└── PgSqlDbServiceTest.cs                # NEU: Unit-Tests (Logik) + Integrationstests (full coverage)
```

**Structure Decision**: Multi-project, bestehende Struktur (constitution Principle II). Kein neues
Projekt erforderlich. Alle Shared-Logic-Änderungen in `InventarWorkerCommon`, Runtime-spezifische
Änderungen in `HarvesterWorkerService`.

---

## Complexity Tracking

> Keine Constitution-Verletzungen — kein Eintrag erforderlich.

---

## Implementation Notes

### PgSqlDbService — Kritische Implementierungsdetails

1. **RETURNING Id**: Alle INSERT-Statements in `PgSqlDbService` müssen `RETURNING Id` verwenden.
   Dapper: `await connection.QuerySingleAsync<int>(insertQuery, parameters)`.

2. **DateTime.UtcNow**: Alle DateTime-Werte an PostgreSQL via `CreatedAt = DateTime.UtcNow`,
   `LastSeen = DateTime.UtcNow` etc. Npgsql 6+ lehnt `DateTimeKind.Local` für `timestamptz` ab.

3. **DROP VIEW vor HardwareInventoryView**: In `InitializeDatabase()` muss VOR dem ersten
   `CREATE OR REPLACE VIEW HardwareInventoryView` folgendes stehen:
   ```sql
   DROP VIEW IF EXISTS hardware_inventory_view;
   ```

4. **Null-Pattern für ServiceContainer**: `ServiceContainer.PgSqlDbService` wird zu `PgSqlDbService?`.
   `Initialize.Services(Settings settings)`: gibt `null` zurück wenn `WriteEnabled == false`.
   `Initialize.Services()` (Fallback): gibt immer `null` zurück.
   Konstruktor: `ArgumentNullException` für `PgSqlDbService` entfernen (null ist erlaubt).
   Dispose/DisposeAsync: null-Check vor `PgSqlDbService?.Dispose()` / `PgSqlDbService?.DisposeAsync()` hinzufügen
   (analog zur bestehenden null-sicheren Disposal anderer Services im ServiceContainer).

5. **CSV-Import**: `NpgsqlConnection` + `BeginTransaction()` analog zur SQLite-Implementierung.
   `await using` für Connection und Transaction (IAsyncDisposable).

### Worker.cs — Isoliertes Schreib-Pattern

SQLite ist die **führende Datenbank** (authority). PostgreSQL ist eine optionale parallele Senke.
Die SQLite-Machine-Id wird explizit in `machine.Id` gesetzt, bevor `PgSqlDbService.SaveOrUpdateMachineAsync`
aufgerufen wird — so übernimmt PostgreSQL die SQLite-Id und alle FK-Referenzen in `HardwareInventories`
und `SoftwareInventories` bleiben konsistent. `GENERATED BY DEFAULT AS IDENTITY` erlaubt explizite
Id-Werte; das "BY DEFAULT" ermöglicht genau dieses Muster.

```csharp
// SQLite ist führend — Id aus SQLite wird gesetzt und für PgSQL übernommen:
// SQLite is authoritative — Id from SQLite is set and adopted by PostgreSQL:
_machineId = await _sqliteDbService.SaveOrUpdateMachineAsync(machine, isHarvester: true);
machine.Id = _machineId; // SQLite-Id explizit setzen / Set SQLite Id explicitly

if (_pgSqlDbService != null)
{
    try
    {
        // PgSqlDbService.SaveOrUpdateMachineAsync nutzt machine.Id für den INSERT:
        // PgSqlDbService.SaveOrUpdateMachineAsync uses machine.Id for the INSERT:
        await _pgSqlDbService.SaveOrUpdateMachineAsync(machine, isHarvester: true);
        await _pgSqlDbService.SaveHardwareInventoryAsync(_machineId, hardwareInventory);
        await _pgSqlDbService.SaveSoftwareInventoryAsync(_machineId, softwareInventory);
    }
    catch (Exception pgException)
    {
        HandleException(pgException); // Loggt, setzt Status = Error
        // SQLite/MongoDB-Writes laufen weiter — kein rethrow
    }
}
```

### Test-Strategie

**Unit-Tests** (`InventarWorkerCommonTest/PgSqlDbServiceTest.cs`):
- Null-Check: `Initialize.Services()` gibt `null` für PgSqlDbService zurück
- Null-Check: `Initialize.Services(settings mit WriteEnabled=false)` gibt `null` zurück
- Exception-Verhalten: `SaveOrUpdateMachineAsync(null)` wirft ArgumentNullException
- Exception-Verhalten: `InitializeMachinesFromCsvAsync("nonexistent.csv")` wirft FileNotFoundException
- Logik: `CleanupOldRecordsAsync(0)` generiert korrekten Cutoff-Timestamp (DateTime.UtcNow)

**Integrationstests** (`[TestCategory("Integration")]`):
- Alle 21 Methoden gegen echte PostgreSQL-Instanz
- Transaktion-Rollback-Test für CSV-Import
- View-Umbenennung: `hardware_inventory_view` nicht mehr vorhanden, `HardwareInventoryView` vorhanden
- `daysToKeep=0` löscht alle Einträge

Verbindungsstring für Integrationstests via Umgebungsvariable:
`PGSQL_TEST_CONNECTION_STRING` = `Host=localhost;Port=5432;Database=inventar_test;Username=...`

**CI-Filter-Befehle / CI filter commands**:
- Normaler CI-Lauf (ohne Integration): `dotnet test --filter "TestCategory!=Integration"`
- Nur Integrationstests: `dotnet test --filter "TestCategory=Integration"`

**Test-Datenbank / Test database**: Dedizierte Datenbank `inventar_test` (nicht `inventar`).
Teardown: `ClassCleanup`-Methode führt `TRUNCATE TABLE SoftwareInventories, HardwareInventories, Machines RESTART IDENTITY CASCADE` (oder `DROP TABLE ... CASCADE`) durch. Keine Produktionsdaten werden berührt.

**Coverage-Hinweis / Coverage note**: 5 Unit-Tests allein erreichen ≥70% auf ~800 Zeilen neuem
DB-Code nicht (DB-Methoden haben 0% ohne Integrationstests). Coverage-Gate ≥70% gilt für den
kombinierten Lauf (Unit + Integration). In reinen Unit-Test-CI-Umgebungen ohne PostgreSQL ist
Coverage < 70% akzeptiert und dokumentiert; das Gate gilt für den vollen Testlauf mit Integration.
