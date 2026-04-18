# Research: PostgreSQL-Parität zum SqliteDbService

**Branch**: `001-pgsql-paritaet` | **Date**: 2026-04-18
**Input**: spec.md (clarified), existing codebase (PgSqlDbService.cs, SqliteDbService.cs, Initialize.cs, Worker.cs)

---

## Entscheidungen / Decisions

### R-01: INSERT mit RETURNING Id (statt last_insert_rowid())

**Decision**: `INSERT INTO Machines (...) VALUES (...) RETURNING Id` mit Dapper `QuerySingleAsync<int>()`.

**Rationale**: PostgreSQL kennt kein `last_insert_rowid()` (SQLite-spezifisch). `RETURNING Id` ist der
PostgreSQL-native Weg, die ID einer neu eingefügten Zeile zurückzugeben. Dapper unterstützt dieses
Muster nativ — `QuerySingleAsync<int>()` mappt den zurückgegebenen Skalar direkt.

**Alternatives considered**:
- `SELECT currval(pg_get_serial_sequence('Machines','Id'))` — umständlich und erfordert separate Abfrage
- `LASTVAL()` — session-spezifisch, aber thread-safety-Risiko bei Verbindungs-Pooling
- `RETURNING Id` — klarste und sicherste Option ✅

---

### R-02: DateTime.UtcNow für alle timestamptz-Spalten

**Decision**: Alle `DateTime`-Werte werden als `DateTime.UtcNow` an Npgsql übergeben.

**Rationale**: PostgreSQL `timestamptz` speichert intern immer UTC und konvertiert beim Auslesen in die
Session-Zeitzone. Npgsql 6+ erfordert `DateTimeKind.Utc` für `timestamptz`-Parameter, sonst Exception
`Cannot write DateTime with Kind=Local to PostgreSQL type 'timestamptz'`. Der bestehende SQLite-Code
verwendet bereits `DateTime.UtcNow` — Parität ist hier direkt möglich.

**Alternatives considered**:
- `DateTimeOffset.UtcNow` — wäre korrekt, erfordert aber Modell-Anpassungen
- Timezone-Konvertierung per Npgsql-TypeMapping — unnötige Komplexität
- `DateTime.UtcNow` — einfachste, bereits etablierte Lösung im Codebase ✅

---

### R-03: WriteEnabled=null-Pattern für ServiceContainer.PgSqlDbService

**Decision**: `Initialize.Services(Settings settings)` gibt `null` als `PgSqlDbService` zurück,
wenn `settings.PgSqlDb.WriteEnabled == false`. `ServiceContainer.PgSqlDbService` wird zu `PgSqlDbService?`
(nullable). Der Worker prüft vor jedem PgSQL-Call auf `_pgSqlDbService != null`.

**Rationale**: Der Worker hat nach der Initialisierung keinen Zugriff mehr auf die settings-Variable.
Ein separates `WriteEnabled`-Flag im Worker würde die Verantwortlichkeiten mischen. Das Null-Pattern
ist idiomatisch in C# und erfordert keine neuen Felder oder Properties.

**Impact on ServiceContainer**:
- `PgSqlDbService PgSqlDbService { get; }` → `PgSqlDbService? PgSqlDbService { get; }`
- Constructor: ArgumentNullException für PgSqlDbService entfernen (null ist erlaubt)
- Dispose-Methoden: null-Check vor Dispose von PgSqlDbService

**Alternatives considered**:
- Separates `bool PgSqlWriteEnabled`-Property im ServiceContainer — unnötige Duplizierung
- Exception bei `WriteEnabled=false` — falsches Fehler-Modell für eine opt-in-Konfiguration
- Null-Return-Pattern ✅

---

### R-04: Fallback-Pfad überspringt PostgreSQL vollständig

**Decision**: `Initialize.Services()` (parameterloser Overload, kein Settings-File) überspringt
PostgreSQL-Initialisierung komplett und setzt `PgSqlDbService = null`.

**Rationale**: Ohne Settings-File gibt es keine Zugangsdaten für PostgreSQL. Der parameterlose Overload
baut Connection Strings mit Defaults (localhost, Port 5432, DB "inventar") — ohne Username/Password
würde die Verbindung in den meisten Produktivumgebungen scheitern. Der Fallback-Pfad ist für
unkonfigurierte Umgebungen gedacht, in denen PostgreSQL nicht verfügbar ist.

**Alternatives considered**:
- Verbindungsversuch mit Defaults, bei Fehler graceful degrade — schlechte UX (verborgene Fehler)
- Exception im parameterlosem Overload — bricht bestehende Logik
- PgSQL überspringen = konservativste und sicherste Lösung ✅

---

### R-05: Fail-hard beim Startup, wenn PgSQL konfiguriert aber nicht erreichbar

**Decision**: Wenn `WriteEnabled=true` und Settings-File vorhanden, aber PgSQL-Server nicht
erreichbar, propagiert `InitializeDatabase()` die Npgsql-Exception. Der Service startet nicht.

**Rationale**: Dieses Verhalten gibt dem Operator sofortiges Feedback über Fehlkonfiguration.
Ein graceful degrade würde die Fehlkonfiguration verschleiern — Daten würden stilleherweise nur
in SQLite/MongoDB landen, ohne dass der Operator weiß, dass PgSQL-Writes fehlschlagen.
Fail-fast ist konsistent mit dem Prinzip "fail loudly on misconfiguration".

**Alternatives considered**:
- Graceful degrade mit Log-Warnung — Konfigurationsfehler werden nicht sofort sichtbar
- Exception + automatischer Retry-Mechanismus — zu komplex für MVP
- Exception propagiert = sauberste Lösung ✅

---

### R-06: DROP VIEW IF EXISTS hardware_inventory_view in InitializeDatabase()

**Decision**: `InitializeDatabase()` führt `DROP VIEW IF EXISTS hardware_inventory_view` aus,
bevor `CREATE OR REPLACE VIEW HardwareInventoryView AS ...` ausgeführt wird.

**Rationale**: `CREATE OR REPLACE VIEW` ersetzt nur Views mit demselben Namen. Die alte View heißt
`hardware_inventory_view` (snake_case), die neue `HardwareInventoryView` (PascalCase). Ein einfaches
`CREATE OR REPLACE` würde die alte View nicht entfernen. `DROP VIEW IF EXISTS` ist idempotent — bei
frischen Installationen ohne alte View ist der Befehl ein No-op.

**Alternatives considered**:
- Manuelle Migration durch Operator — schlechte DevEx, fehleranfällig
- Beide Views parallel beibehalten — Konfusion und Naming-Inkonsistenz
- DROP IF EXISTS + CREATE = idempotente, sichere Lösung ✅

---

### R-07: CleanupOldRecords(daysToKeep=0) ohne Mindestgrenze erlaubt

**Decision**: `daysToKeep=0` ist gültiger Eingabewert. Cutoff = `DateTime.UtcNow.AddDays(0)` = jetzt.
Alle Einträge (CreatedAt < jetzt) werden gelöscht. Kein Minimum wird erzwungen.

**Rationale**: Der Aufrufer trägt die Verantwortung für den gewählten Wert. Datenbankwartungs-
operationen müssen deterministische Semantik haben — eine hidden Mindestgrenze wäre überraschend.
Der Wert 0 als "alles löschen" ist eine bewusste, legitime Entscheidung des Aufrufers (z. B. für
Test-Cleanup oder Migrations-Szenarien).

**Randfall negative Werte / Edge case negative values**: `daysToKeep = -5` ergibt
`DateTime.UtcNow.AddDays(-(-5)) = DateTime.UtcNow.AddDays(5)` (Cutoff 5 Tage in der Zukunft).
`DELETE WHERE CreatedAt < cutoff_zukunft` löscht ALLE Einträge, da alle `CreatedAt`-Werte in
der Vergangenheit liegen. Negative Werte verhalten sich damit wie `daysToKeep = 0`. Dies ist
mathematisch korrekt und dokumentiertes Verhalten — kein Sonderfall, keine Ausnahme. Der Aufrufer
ist für die Wahl des Wertes verantwortlich.
/ `daysToKeep = -5` results in `DateTime.UtcNow.AddDays(5)` as cutoff (5 days in the future).
`DELETE WHERE CreatedAt < future_cutoff` removes ALL records since all `CreatedAt` values are
in the past. Negative values behave equivalently to `daysToKeep = 0`. Mathematically correct;
no special handling needed; caller is responsible for the chosen value.

**Alternatives considered**:
- `ArgumentOutOfRangeException` bei daysToKeep < 1 — bricht legitime Use Cases
- `Math.Max(1, daysToKeep)` — verschleiert Kaller-Intent
- Keine Einschränkung = konsistenteste Lösung ✅

---

### R-08: Test-Strategie für PgSqlDbService

**Decision**: Zweistufige Test-Strategie:

1. **Unit-Tests** (MSTest, kein externer Service):
   - Testen: null-Check-Logik, ArgumentNullException-Verhalten, Fallback-Pfade in Initialize.cs
   - Werkzeug: SQLite in-memory wo SQL-kompatibel (`Data Source=:memory:`), Fake-ServiceContainer
   - Einschränkung: PostgreSQL-spezifische SQL-Syntax (RETURNING, DISTINCT ON) kann nicht per
     SQLite-Unit-Test validiert werden

2. **Integrationstests** (MSTest + `[TestCategory("Integration")]`):
   - Testen: alle 21 Methoden end-to-end gegen echte PostgreSQL-Instanz
   - Voraussetzung: lokale oder CI-PostgreSQL-Instanz (Verbindungsstring via Umgebungsvariable)
   - Skip in normaler CI-Pipeline; explizit ausführbar via `dotnet test --filter TestCategory=Integration`
   - Ausschluss aus normalem CI: `dotnet test --filter "TestCategory!=Integration"`

**Trennlinie Unit ↔ Integration / Dividing line unit ↔ integration**:
- Unit-testbar (kein PostgreSQL-SQL): null-Checks in `ServiceContainer`, `ArgumentNullException`-
  Logik in `SaveOrUpdateMachineAsync`, `FileNotFoundException`-Check in
  `InitializeMachinesFromCsvAsync`, `CleanupOldRecordsAsync`-Cutoff-Berechnung,
  `WriteEnabled`-Fallback-Pfade in `Initialize.Services()`.
- Integration-Test erforderlich (PostgreSQL-SQL): alle 21 DB-Methoden, die tatsächlich SQL
  ausführen (RETURNING Id, DISTINCT ON, timestamptz-Vergleiche, VIEW-Abfragen).
  Coverage-Gate ≥70% setzt Integration-Tests voraus — Unit-Tests allein decken zu wenig ab.
/ Unit-testable (no PostgreSQL SQL): null checks in `ServiceContainer`, `ArgumentNullException`
  logic, `FileNotFoundException` check, cleanup cutoff calculation, `WriteEnabled` fallback.
  Integration required (PostgreSQL SQL): all 21 DB methods that execute actual SQL.

**Rationale**: Die Dapper-Pattern und SQL-Queries können nicht sinnvoll ohne echte DB geprüft werden,
da PostgreSQL-spezifische Syntax (RETURNING Id, DISTINCT ON, timestamptz) von SQLite nicht unterstützt
wird. Integrationstests mit echter PostgreSQL sind für vollständige Methodenabdeckung notwendig.

**Alternatives considered**:
- Nur Unit-Tests mit Mocks (Moq auf IDbConnection) — zu aufwendig für 21 Methoden, fragile
- Nur Integrationstests — schließt CI ohne PostgreSQL aus
- Zweistufige Strategie (Unit + optional Integration) = pragmatischster Ansatz ✅

---

### R-09: CSV-Import mit NpgsqlTransaction

**Decision**: `InitializeMachinesFromCsvAsync` verwendet `NpgsqlConnection.BeginTransaction()` und
übergibt die Transaktion an alle Dapper-Calls via `transaction`-Parameter.

**Rationale**: Identisches Pattern wie SQLite-Implementierung (`SqliteConnection.BeginTransaction()`).
Npgsql unterstützt dasselbe ADO.NET-Transaktionsmodell. Bei Exception: `transaction.Rollback()` vor
`throw`. Bei Erfolg: `transaction.Commit()`.

**Alternatives considered**:
- `TransactionScope` — funktioniert mit Npgsql, aber erfordert `using System.Transactions` und
  verteilte Transaktionen; unnötige Komplexität
- Kein Transaktionsschutz — verletzt FR-010 und SC-004
- `BeginTransaction()` = direktes Parity-Pattern zur SQLite-Implementierung ✅

---

### R-10: Vorhandene SQL Views in PgSqlDbService.InitializeDatabase()

**Decision**: Die Statistik-Views (`ComputerModelStatisticsView`, `ArchitectureStatisticsView`,
`ModelArchitectureStatisticsView`, `HardwareStatisticsOverview`) sind bereits im SQL-Block von
`InitializeDatabase()` definiert. C# Lesemethoden für diese Views werden in einem separaten
Lastenheft implementiert (nicht in diesem Feature).

**Rationale**: Die View-Definitionen sind bereits vorhanden — kein Änderungsbedarf an `InitializeDatabase()`.
Die C# Read-Methoden sind per Assumption aus spec.md explizit ausgeklammert.

**Alternatives considered**:
- C# Lesemethoden in diesem Feature mitliefern — würde den Scope erheblich erweitern; kein Bedarf für laufenden HarvesterWorkerService-Betrieb
- Statistik-Views aus `InitializeDatabase()` entfernen — würde Schema-Fragmentierung erzeugen; Views gehören logisch zur DB-Initialisierung
- Views als eigenes Artefakt in einem Migration-Skript — unnötige Komplexität; `CREATE OR REPLACE VIEW` in `InitializeDatabase()` ist idempotent ✅

---

### R-11: SQLite als führende Datenbank — ID-Synchronisierung mit PostgreSQL

**Decision**: SQLite ist die autoritäre Datenbank. Vor dem Aufruf von
`PgSqlDbService.SaveOrUpdateMachineAsync` setzt der Worker `machine.Id = _machineId` (SQLite-Id).
Die PgSQL-Implementierung führt einen INSERT mit expliziter Id durch. `_machineId` aus SQLite
kann danach für alle FK-Referenzen in `HardwareInventories` und `SoftwareInventories` in
PostgreSQL verwendet werden.

**Rationale**: PostgreSQL ist eine optionale, sekundäre Senke für Lernzwecke (Azubis sollen
einen echten SQL-Server kennenlernen). SQLite ist immer vorhanden und ist das einzige System,
das der `HarvesterWorkerService` aktiv verwaltet. Da beide DBs immer gemeinsam initialisiert
und betrieben werden, ist die Maschinen-ID in SQLite die kanonische Referenz.
`GENERATED BY DEFAULT AS IDENTITY` erlaubt explizite Id-Werte — "BY DEFAULT" bedeutet: die
Sequenz wird nur verwendet, wenn kein expliziter Wert angegeben wird.

**Voraussetzung / Prerequisite**: Beide Datenbanken werden immer gemeinsam initialisiert und
betrieben. Ein unabhängiges Zurücksetzen von PostgreSQL (ohne entsprechendes Reset von SQLite)
würde zu ID-Divergenz führen und muss als unsupported Szenario dokumentiert sein.

**Alternatives considered**:
- Separate `pgMachineId`-Variable im Worker — unabhängige IDs, mehr Worker-Code, kein SQLite-Führungs-Konzept
- Keine explizite Id-Übergabe (PgSQL auto-assign) — IDs divergieren bei unterschiedlicher Initialisierung
- ID-Sync über expliziten INSERT = einfachster Ansatz für das "SQLite is master"-Modell ✅

---

## Offene Punkte / Open Points

Keine. Alle NEEDS CLARIFICATION-Marker aus spec.md sind aufgelöst.
Alle research-Entscheidungen sind dokumentiert und direkt in data-model.md und contracts/ übertragen.
