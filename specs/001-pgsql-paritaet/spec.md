# Feature Specification: PostgreSQL-Parität zum SqliteDbService

**Feature Branch**: `001-pgsql-paritaet`
**Created**: 2026-04-18
**Status**: Draft
**Input**: Lastenheft_PostgreSQL_Implementation.md (Review-Entscheidungen eingearbeitet, Stand 2026-04-18)

---

## Clarifications

### Session 2026-04-18 (Runde 1)

- Q: Soll ein PostgreSQL-Write-Fehler während des Ernte-Zyklus isoliert werden (SQLite läuft weiter) oder als harter Fehler behandelt werden? → A: Isoliert — Fehler wird per `HandleException` geloggt, SQLite/MongoDB-Writes laufen für die aktuelle Maschine normal weiter.
- Q: Wenn PostgreSQL konfiguriert und `WriteEnabled=true`, aber der Server beim Startup nicht erreichbar ist — Exception propagieren (Fail hard) oder graceful degrade? → A: Fail hard — Exception propagiert, Service startet nicht; gibt dem Operator sofortiges Feedback über Fehlkonfiguration.
- Q: Soll `CleanupOldRecordsAsync(daysToKeep=0)` erlaubt sein (alle Einträge löschen) oder durch ein Minimum blockiert werden? → A: Erlaubt — `daysToKeep=0` löscht alle Hardware- und Software-Inventareinträge; kein Mindestwert wird erzwungen.

### Session 2026-04-18 (Runde 2)

- Q: Wie greift Worker.cs auf `PgSqlDb.WriteEnabled` zur Schreibzeit zu, wenn `settings` nach der Initialisierung verworfen ist? → A: `Initialize.Services()` gibt `null` zurück, wenn `WriteEnabled=false` — Worker braucht nur den Null-Check, kein separates Flag-Feld.
- Q: Soll `InitializeDatabase()` die alte View `hardware_inventory_view` explizit droppen, damit bei bestehenden DBs keine verwaisten Views zurückbleiben? → A: Ja — `DROP VIEW IF EXISTS hardware_inventory_view` in `InitializeDatabase()` einfügen; idempotent durch `IF EXISTS`.

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Inventardaten in PostgreSQL schreiben (Priority: P1)

Als **HarvesterWorkerService** möchte ich Maschinen-, Hardware- und Software-Inventardaten
in PostgreSQL speichern können, damit eine vollständige relationale Kopie der Inventardaten
neben SQLite und MongoDB vorhanden ist.

**Why this priority**: Ohne die Schreibmethoden ist PostgreSQL vollständig nutzlos. Alle
weiteren User Stories bauen auf funktionierenden Schreiboperationen auf. Diese Story liefert
den direkten Geschäftswert: ein zweiter, vollfunktionaler Datenbankprovider.

**Independent Test**: Kann unabhängig getestet werden, indem `SaveOrUpdateMachineAsync`,
`SaveHardwareInventoryAsync` und `SaveSoftwareInventoryAsync` gegen eine lokale
PostgreSQL-Instanz aufgerufen werden und die Daten anschliessend per direkter SQL-Abfrage
verifiziert werden.

**Acceptance Scenarios**:

1. **Given** eine konfigurierte PostgreSQL-Instanz und ein `PgSqlDbService`,
   **When** `SaveOrUpdateMachineAsync(machine)` für eine neue Maschine aufgerufen wird,
   **Then** existiert ein Datensatz mit korrektem Namen in der Tabelle `Machines`.

2. **Given** eine Maschine mit bekanntem Namen bereits in PostgreSQL vorhanden,
   **When** `SaveOrUpdateMachineAsync(machine)` erneut für denselben Namen aufgerufen wird,
   **Then** wird der bestehende Datensatz aktualisiert (kein Duplikat).

3. **Given** eine Maschine mit bekanntem Namen,
   **When** `SaveOrUpdateMachineAsync(machine, isHarvester: true)` aufgerufen wird,
   **Then** werden auch `IPv4`, `IPv6`, `FQDN` und `LastHarvested` aktualisiert.

4. **Given** eine `machineId` und ein `HardwareInventory`-Objekt,
   **When** `SaveHardwareInventoryAsync(machineId, hardware)` aufgerufen wird,
   **Then** ist ein Datensatz in `HardwareInventories` mit korrekten Werten vorhanden.

5. **Given** eine `machineId` und ein `SoftwareInventory`-Objekt,
   **When** `SaveSoftwareInventoryAsync(machineId, software)` aufgerufen wird,
   **Then** sind alle JSON-Felder (Prozesse, Software, Services, Umgebung, Autostart, Runtime)
   korrekt serialisiert in `SoftwareInventories` gespeichert.

---

### User Story 2 - HarvesterWorkerService schreibt parallel nach PostgreSQL (Priority: P2)

Als **Systemadministrator** möchte ich, dass der `HarvesterWorkerService` bei aktiviertem
`WriteEnabled` automatisch Inventardaten nach PostgreSQL schreibt — zusätzlich zu SQLite und
MongoDB — damit PostgreSQL als vollwertiger dritter Persistenz-Provider genutzt werden kann.

**Why this priority**: Ohne Worker-Integration kann die PgSQL-Implementierung nicht
end-to-end getestet werden. Der `WriteEnabled`-Schutzschalter ist ausserdem wichtig, um
unkonfigurierte Produktivumgebungen zu schützen.

**Independent Test**: Kann getestet werden, indem `PgSqlDb.WriteEnabled = true` in der
Settings-Datei gesetzt und der `HarvesterWorkerService` gestartet wird. Danach müssen in
PostgreSQL Datensätze in `Machines`, `HardwareInventories` und `SoftwareInventories`
erscheinen.

**Acceptance Scenarios**:

1. **Given** `PgSqlDb.WriteEnabled = true` in den Settings,
   **When** der `HarvesterWorkerService` einen Ernte-Zyklus durchführt,
   **Then** werden Maschinen-, Hardware- und Software-Daten parallel in SQLite, MongoDB
   **und** PostgreSQL gespeichert.

2. **Given** `PgSqlDb.WriteEnabled = false` in den Settings,
   **When** `Initialize.Services(settings)` aufgerufen wird,
   **Then** ist `ServiceContainer.PgSqlDbService` `null`; der Worker führt keine PostgreSQL-Schreibzugriffe durch und wirft keine Exception.

3. **Given** keine Settings-Datei vorhanden (Fallback-Pfad),
   **When** `Initialize.Services()` aufgerufen wird,
   **Then** wird die PostgreSQL-Initialisierung übersprungen; `ServiceContainer.PgSqlDbService`
   ist `null` und keine Exception wird geworfen.

---

### User Story 3 - Inventardaten aus PostgreSQL lesen (Priority: P3)

Als **Entwickler** möchte ich alle Lese-, Lookup- und Wartungs-Methoden des `PgSqlDbService`
nutzen können, die auch im `SqliteDbService` vorhanden sind, damit ich den Provider
transparent wechseln oder parallel betreiben kann.

**Why this priority**: Lesemethoden werden für den laufenden HarvesterWorkerService-Betrieb
nicht unmittelbar benötigt (der Worker liest aus SQLite), sind aber für vollständige
API-Parität und künftiges Provider-Switching erforderlich.

**Independent Test**: Kann getestet werden, indem Testdaten per Schreibmethoden eingefügt
und anschliessend per Lesemethode abgerufen werden. Signaturen müssen identisch zur SQLite-API
sein.

**Acceptance Scenarios**:

1. **Given** mehrere Maschinen in PostgreSQL (aktiv, deaktiviert, deprovisioniert),
   **When** `GetAllActiveMachinesAsync()` aufgerufen wird,
   **Then** werden ausschliesslich aktive Maschinen (`Disabled=0, Deprovisioned=0`) zurückgegeben.

2. **Given** eine Maschine mit `Id = 5` in PostgreSQL,
   **When** `GetMachineByIdAsync(5)` aufgerufen wird,
   **Then** wird die korrekte Maschine zurückgegeben.

3. **Given** eine Maschine mit mehreren Hardware-Inventareinträgen,
   **When** `GetLatestHardwareInventoryAsync(machineId)` aufgerufen wird,
   **Then** wird nur der zeitlich neueste Eintrag zurückgegeben.

4. **Given** `GetAllActiveMachinesWithNetworkInfoAsync()` wird aufgerufen,
   **Then** werden nur Maschinen zurückgegeben, die mindestens einen nicht-leeren
   Netzwerkwert (IPv4, IPv6 oder FQDN) haben.

5. **Given** Inventardaten vorhanden,
   **When** `GetMachineCountAsync()`, `GetHardwareInventoryCountAsync()`,
   `GetSoftwareInventoryCountAsync()` aufgerufen werden,
   **Then** stimmen die Ergebnisse mit den tatsächlichen Datenbankeinträgen überein.

---

### User Story 4 - Maschinen per CSV importieren (Priority: P4)

Als **Systemadministrator** möchte ich eine Liste von Maschinen per CSV-Datei in PostgreSQL
importieren können, damit der Initialbestand ohne manuellen Datenbankzugriff befüllt
werden kann.

**Why this priority**: Der CSV-Import ist für den Erstbetrieb relevant, aber keine
laufende Betriebsfunktion. Er kann nach den Kern-CRUD-Methoden umgesetzt werden.

**Independent Test**: Kann getestet werden, indem eine CSV-Datei im bekannten Format erstellt
und `InitializeMachinesFromCsvAsync(csvFilePath)` aufgerufen wird. Die importierten Maschinen
müssen danach per `GetMachinesAsync()` abrufbar sein.

**Acceptance Scenarios**:

1. **Given** eine gültige CSV-Datei mit 3 Maschinen,
   **When** `InitializeMachinesFromCsvAsync(path)` aufgerufen wird,
   **Then** werden 3 neue Maschinen in PostgreSQL angelegt und die Anzahl importierter
   Datensätze zurückgegeben.

2. **Given** eine CSV-Datei, bei der eine Maschine bereits in der Datenbank existiert,
   **When** `InitializeMachinesFromCsvAsync(path)` aufgerufen wird,
   **Then** wird die bestehende Maschine nicht überschrieben (nur neue werden importiert).

3. **Given** ein Fehler während des Imports,
   **When** eine Exception auftritt,
   **Then** wird die Transaktion vollständig zurückgerollt (kein Teilergebnis in der DB).

4. **Given** eine nicht vorhandene CSV-Datei,
   **When** `InitializeMachinesFromCsvAsync(path)` aufgerufen wird,
   **Then** wird eine `FileNotFoundException` geworfen.

---

### User Story 5 - View-Namen auf PascalCase vereinheitlichen (Priority: P5)

Als **Entwickler** möchte ich, dass alle View-Namen und Spaltenaliase in PostgreSQL mit
den SQLite-Konventionen (PascalCase) übereinstimmen, damit dieselben Abfragen ohne
Anpassung gegen beide Provider funktionieren.

**Why this priority**: Technische Voraussetzung für transparente Parität. Ohne einheitliche
Namen würden identische Abfragen gegen verschiedene Provider fehlschlagen.

**Independent Test**: Kann geprüft werden, indem `SELECT * FROM HardwareInventoryView LIMIT 1`
gegen die PostgreSQL-Instanz ausgeführt wird und PascalCase-Spalten zurückgegeben werden.

**Acceptance Scenarios**:

1. **Given** die initialisierte PostgreSQL-Datenbank,
   **When** `SELECT * FROM HardwareInventoryView LIMIT 1` ausgeführt wird,
   **Then** ist die View unter `HardwareInventoryView` vorhanden (nicht `hardware_inventory_view`).

2. **Given** die View `HardwareInventoryView`,
   **When** die Spaltennamen abgefragt werden,
   **Then** lauten sie `MachineID`, `MachineName`, `Architecture`, `ProcessorCores`,
   `TotalMemoryGB`, `AvailableMemoryGB`, `MemoryUsagePercent` (PascalCase, identisch zu SQLite).

3. **Given** eine Datenbank, die zuvor mit `hardware_inventory_view` initialisiert wurde,
   **When** `InitializeDatabase()` erneut aufgerufen wird,
   **Then** existiert `hardware_inventory_view` nicht mehr; nur `HardwareInventoryView` ist vorhanden.

---

### Edge Cases

- Was passiert, wenn der PostgreSQL-Server beim Startup nicht erreichbar ist (Settings vorhanden, `WriteEnabled=true`)?
  → Exception propagiert, Service startet nicht (Fail hard). Gibt dem Operator sofortiges Feedback über Fehlkonfiguration. Nur wenn keine Settings-Datei vorhanden ist, wird PgSQL übersprungen (FR-014).
- Was passiert, wenn `SaveOrUpdateMachineAsync` mit einem `null`-Maschinenobjekt aufgerufen wird?
  → `ArgumentNullException` erwartet.
- Was passiert, wenn `CleanupOldRecordsAsync` mit `daysToKeep=0` aufgerufen wird?
  → Erlaubt: alle Hardware- und Software-Inventareinträge werden gelöscht. Kein Mindestwert wird erzwungen; der Aufrufer trägt die Verantwortung.
- Was passiert bei Zeitzonenunterschieden? PostgreSQL `timestamptz` erwartet UTC.
  → Alle `DateTime`-Werte MÜSSEN als `DateTime.UtcNow` übergeben werden.
- Was passiert, wenn `PgSqlDbService` im `ServiceContainer` `null` ist (kein Settings-File)?
  → Worker muss vor jedem Aufruf auf `null` prüfen.

---

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: `PgSqlDbService` MUSS `SaveOrUpdateMachineAsync(Machine, bool isHarvester)` implementieren — Parität zu `SqliteDbService`.
- **FR-002**: `PgSqlDbService` MUSS `SaveHardwareInventoryAsync(int machineId, HardwareInventory)` implementieren.
- **FR-003**: `PgSqlDbService` MUSS `SaveSoftwareInventoryAsync(int machineId, SoftwareInventory)` mit JSON-Serialisierung implementieren.
- **FR-004**: `PgSqlDbService` MUSS `GetMachinesAsync()`, `GetAllActiveMachinesAsync()`, `GetAllActiveMachinesWithNetworkInfoAsync()`, `GetAllDisabledMachinesAsync()`, `GetAllDeprovisionedMachinesAsync()` implementieren.
- **FR-005**: `PgSqlDbService` MUSS `GetMachineByIdAsync(int)` und `GetMachineByNameAsync(string)` implementieren.
- **FR-006**: `PgSqlDbService` MUSS `GetLatestHardwareInventoryAsync(int)` und `GetLatestSoftwareInventoryAsync(int)` implementieren.
- **FR-007**: `PgSqlDbService` MUSS `CleanupOldRecordsAsync(int daysToKeep = 30)` implementieren.
- **FR-008**: `PgSqlDbService` MUSS `HasMachineRecordsAsync()`, `HasHardwareInventoryRecordsAsync()`, `HasSoftwareInventoryRecordsAsync()` implementieren.
- **FR-009**: `PgSqlDbService` MUSS `GetMachineCountAsync()`, `GetHardwareInventoryCountAsync()`, `GetSoftwareInventoryCountAsync()` implementieren.
- **FR-010**: `PgSqlDbService` MUSS `InitializeMachinesFromCsvAsync(string csvFilePath)` mit Transaktionsschutz implementieren.
- **FR-011**: PostgreSQL-INSERT MUSS `RETURNING Id` statt `last_insert_rowid()` verwenden.
- **FR-012**: Alle `DateTime`-Werte MÜSSEN als `DateTime.UtcNow` an PostgreSQL übergeben werden.
- **FR-013**: Die View `hardware_inventory_view` MUSS in `HardwareInventoryView` umbenannt und Spaltenaliase auf PascalCase angepasst werden. `InitializeDatabase()` MUSS `DROP VIEW IF EXISTS hardware_inventory_view` ausführen, bevor die neue View angelegt wird, um bei bestehenden Datenbanken keine verwaisten Views zu hinterlassen.
- **FR-014**: `Initialize.Services()` MUSS `null` als `PgSqlDbService` zurückgeben, wenn (a) keine Settings-Datei vorhanden ist oder (b) `PgSqlDb.WriteEnabled = false`. Der Worker prüft ausschliesslich auf `null` — kein separates WriteEnabled-Flag im Worker erforderlich.
- **FR-015**: `HarvesterWorkerService/Worker.cs` MUSS PostgreSQL-Schreiboperationen aufrufen, wenn `PgSqlDbService != null`. Ein Fehler beim PostgreSQL-Write MUSS isoliert werden: per `HandleException` loggen, SQLite/MongoDB-Writes der aktuellen Maschine laufen normal weiter.
- **FR-016**: `InitializeDatabase()` bleibt synchron (Parität zu `SqliteDbService`; keine async-Version).
- **FR-017**: Alle öffentlichen Methoden von `PgSqlDbService` MÜSSEN vollständige XML-Dokumentation haben (zweisprachig DE/EN, CEFR B2).

---

## Constitution Alignment *(mandatory)*

- **CA-001 Branching**: Feature wird auf Branch `001-pgsql-paritaet` implementiert, Merge via PR zu `main`.
- **CA-002 Toolchain**: Betrifft ausschliesslich `InventarWorkerCommon` und `HarvesterWorkerService`; beide sind bereits auf .NET 10 / C# 14.0. Kein Upgrade erforderlich.
- **CA-003 Dependency Currency**: `Npgsql`, `Dapper`, `CsvHelper` auf aktuelle stabile Versionen prüfen. Keine neuen Packages erforderlich.
- **CA-004 Coverage**: Neue Methoden in `PgSqlDbService` benötigen Unit-Tests. CI-Gate ≥70%, Ziel ≥80%. Integrationstests gegen echte PostgreSQL-Instanz werden mit Skip-Attribut oder separatem Profil markiert.
- **CA-005 Layering**: Alle Änderungen in `InventarWorkerCommon/Services/Database/PgSqlDbService.cs` und `InventarWorkerCommon/Services/Common/Initialize.cs`. Worker-Anpassungen in `HarvesterWorkerService/Worker.cs`.
- **CA-006 Linguistic Rules**: XML-Dokumentation zweisprachig (DE zuerst, EN als zweite Sprache), CEFR B2.
- **CA-007 Documentation Enforcement**: `docfx docfx.json` muss nach Abschluss aller API-Änderungen ausgeführt werden.
- **CA-008 Testing Impact**: Red-Green-Refactor für alle 21 neuen Methoden. Unit-Tests sind unabhängig von echter DB (gemockte Verbindung oder In-Memory-Pattern).
- **CA-009 Data Contracts**: `System.Text.Json` für JSON-Serialisierung. Dapper mit expliziten SQL-Strings. PascalCase Tabellen/Spalten-Konvention. View-Umbenennung ist Pflicht.

### Key Entities

- **Machine**: Stammdaten einer verwalteten Maschine (Name, Betriebssystem, Netzwerkinfos, Status-Flags `Disabled`/`Deprovisioned`, Zeitstempel).
- **HardwareInventory**: Hardware-Momentaufnahme einer Maschine (CPU-Details, Speicherwerte, Architektur, Computerhersteller/-modell).
- **SoftwareInventory**: Software-Momentaufnahme als JSON-serialisierte Teillisten (laufende Prozesse, installierte Software, Windows-Dienste, Umgebungsvariablen, Autostart-Programme, Runtime-Info).
- **MachineState**: Projektion der `Machines`-Tabelle für View-Abfragen (Id, Name, Netzwerkinfos, Status-Flags). Wird von allen View-basierten Lesemethoden zurückgegeben.

---

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Alle 21 öffentlichen Methoden des `SqliteDbService` sind im `PgSqlDbService` mit identischen Signaturen implementiert und kompilieren ohne Warnungen.
- **SC-002**: Ein kompletter Ernte-Zyklus des `HarvesterWorkerService` mit `WriteEnabled=true` schreibt nachweislich Daten in PostgreSQL (verifizierbar per direkter SQL-Abfrage auf alle drei Tabellen).
- **SC-003**: Alle Pfade mit `WriteEnabled=false` oder fehlendem Settings-File laufen ohne Exceptions oder Fehlermeldungen durch.
- **SC-004**: Der CSV-Import überträgt eine 10-Einträge-Datei vollständig; bei simuliertem Abbruch nach 5 Einträgen sind 0 Einträge in der Datenbank (Transaktionsschutz verifiziert).
- **SC-005**: CI-Coverage für neue Code-Pfade liegt bei ≥70%; Zielwert ≥80%.
- **SC-006**: `SELECT * FROM HardwareInventoryView` liefert PascalCase-Spalten identisch zur SQLite-View.
- **SC-007**: `dotnet build InventarWorkerService.sln` läuft ohne Warnungen bezogen auf den neuen Code.

---

## Assumptions

- PostgreSQL-Version 14 oder höher (unterstützt `GENERATED BY DEFAULT AS IDENTITY`, `RETURNING Id`, `timestamptz`).
- Eine laufende PostgreSQL-Instanz ist für Integrationstests verfügbar; Unit-Tests laufen ohne echte DB.
- Die bestehenden SQLite-Bugs (`GetAllDeprovisionedMachinesAsync`, `GetAllDisabledMachinesAsync`) werden in einem separaten Feature (`Lastenheft_SQLite_ViewQuery_Bugfix.md`) behoben — nicht Teil dieser Spezifikation.
- Ein formales `IDbService`-Interface wird in diesem Feature nicht eingeführt (separates Lastenheft).
- Lesemethoden für Statistik-Views (`ComputerModelStatisticsView` etc.) sind nicht Teil dieses Features (separates Lastenheft).
- MongoDB-Parität ist nicht Teil dieses Features (separates Lastenheft).
