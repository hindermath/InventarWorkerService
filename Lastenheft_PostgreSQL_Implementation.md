# Lastenheft: Vervollständigung der PostgreSQL-Implementierung (PGSQL-Parität)

**Dokument-Status:** Entwurf
**Erstellt:** 2026-04-12
**Betrifft:** `InventarWorkerCommon/Services/Database/PgSqlDbService.cs`, `InventarWorkerCommon/Services/Common/Initialize.cs`, `HarvesterWorkerService/Worker.cs`
**Priorität:** Hoch (Herstellung der funktionalen Parität zur SQLite-Implementierung)

---

## Ausgangslage

Der `InventarWorkerService` nutzt aktuell primär SQLite (`SqliteDbService`) zur lokalen Datenspeicherung im `HarvesterWorkerService`. Eine PostgreSQL-Implementierung (`PgSqlDbService`) wurde begonnen, ist jedoch über die Datenbank-Initialisierung (Tabellen- und View-Erstellung) nicht hinausgekommen. 

Damit PostgreSQL als vollwertige Alternative oder parallele Speicherlösung genutzt werden kann, muss der `PgSqlDbService` die identische API wie der `SqliteDbService` bereitstellen.

| Komponente | Status | Details |
|-----------|--------|---------|
| SQLite-Service | Vollständig | Bietet Methoden für CRUD, CSV-Import, Maintenance und Statistik. |
| PostgreSQL-Service | Rumpf | Nur `InitializeDatabase()` und Hilfsmethoden zur DB-Erstellung vorhanden. |
| Harvester-Worker | SQLite-zentriert | Nutzt aktuell hartverdrahtet den `SqliteDbService`. |

---

## Anforderungen

### R-PGSQL-01: Implementierung der Schreib-Methoden (CRUD)

Der `PgSqlDbService` muss alle Schreiboperationen unterstützen, die auch in SQLite vorhanden sind. Dabei ist auf PostgreSQL-spezifische SQL-Syntax zu achten (z.B. `INSERT ... RETURNING Id` statt `last_insert_rowid()`).

- **R-PGSQL-01.1: `SaveOrUpdateMachineAsync`**
  - Parameter: `Machine machine`, `bool isHarvester = false`
  - Logik: Prüfung auf Existenz via Name, dann Update oder Insert.
- **R-PGSQL-01.2: `SaveHardwareInventoryAsync`**
  - Parameter: `int machineId`, `HardwareInventory hardware`
- **R-PGSQL-01.3: `SaveSoftwareInventoryAsync`**
  - Parameter: `int machineId`, `SoftwareInventory software`
  - JSON-Serialisierung der Unterobjekte (analog zu SQLite).

### R-PGSQL-02: Implementierung der Lese-Methoden (Listen)

Abfrage der in `PgSqlDbService.cs` bereits definierten Views und Tabellen.

- **R-PGSQL-02.1: `GetMachinesAsync()`** (Alle Maschinen)
- **R-PGSQL-02.2: `GetAllActiveMachinesAsync()`** (Via `AllActiveMachinesView`)
- **R-PGSQL-02.3: `GetAllActiveMachinesWithNetworkInfoAsync()`** (Via `AllActiveMachinesWithNetworkInfoView`)
- **R-PGSQL-02.4: `GetAllDisabledMachinesAsync()`** (Via `AllDisabledMachinesView`)
- **R-PGSQL-02.5: `GetAllDeprovisionedMachinesAsync()`** (Via `AllDeprovisionedMachinesView`)

### R-PGSQL-03: Implementierung der Einzelabfragen und Lookups

- **R-PGSQL-03.1: `GetMachineByIdAsync(int id)`**
- **R-PGSQL-03.2: `GetMachineByNameAsync(string machineName)`**
- **R-PGSQL-03.3: `GetLatestHardwareInventoryAsync(int machineId)`**
- **R-PGSQL-03.4: `GetLatestSoftwareInventoryAsync(int machineId)`**

### R-PGSQL-04: Implementierung von Maintenance- und Statistik-Methoden

- **R-PGSQL-04.1: `CleanupOldRecordsAsync(int daysToKeep = 30)`**
- **R-PGSQL-04.2: Existenz-Checks:** `HasMachineRecordsAsync`, `HasHardwareInventoryRecordsAsync`, `HasSoftwareInventoryRecordsAsync`.
- **R-PGSQL-04.3: Zähl-Methoden:** `GetMachineCountAsync`, `GetHardwareInventoryCountAsync`, `GetSoftwareInventoryCountAsync`.

### R-PGSQL-05: Implementierung des CSV-Imports

- **R-PGSQL-05: `InitializeMachinesFromCsvAsync(string csvFilePath)`**
  - Muss identisch zum SQLite-Import funktionieren (Transaktionsschutz, CsvHelper-Integration).

### R-PGSQL-06: Konsistenz und technische Standards

- **Dapper:** Alle SQL-Abfragen müssen weiterhin via `Dapper` ausgeführt werden.
- **Npgsql:** Nutzung von `NpgsqlConnection` für die Verbindung.
- **Async/Await:** Konsequente Nutzung von asynchronen Methoden (Parität zu SQLite).
- **DateTime-Handling:** PostgreSQL erwartet für `timestamptz` in der Regel UTC. Sicherstellen, dass `DateTime.UtcNow` verwendet wird.
- **View-Namen:** Die Namen der Views in PostgreSQL müssen exakt denen in SQLite entsprechen (PascalCase), um die Abfrageroutine identisch halten zu können. Bestehende Abweichungen (z.B. `hardware_inventory_view` vs. `HardwareInventoryView`) müssen korrigiert werden.
- **Dokumentation:** Alle neuen öffentlichen Methoden müssen vollständig mit XML-Kommentaren (zweisprachig oder konsistent zum Projekt) dokumentiert werden.

### R-PGSQL-07: Vorbereitung für Provider-Switching (Optional/Ausblick)

Obwohl dieses Lastenheft primär die Parität des Services betrifft, sollte die Implementierung so sauber sein, dass der `HarvesterWorkerService` in einem nächsten Schritt leicht auf PostgreSQL umgestellt oder beide parallel betrieben werden können.

### R-PGSQL-08: Konfigurations-Integration und Write-Safety

- **R-PGSQL-08.1: Nutzung der Settings:** Der `PgSqlDbService` muss den über `Initialize.cs` bereitgestellten Connection-String (inkl. User/Passwort aus `PgSqlDb.PgSqlConnectionString`) für alle Datenbankverbindungen nutzen.
- **R-PGSQL-08.2: Berücksichtigung von `WriteEnabled`:** Im `HarvesterWorkerService` (oder an zentraler Stelle in `Initialize.cs`) muss sichergestellt werden, dass Schreibzugriffe auf PostgreSQL nur dann erfolgen, wenn `PgSqlDb.WriteEnabled` auf `true` gesetzt ist. Dies verhindert Fehlermeldungen bei unkonfigurierten PostgreSQL-Instanzen.

---

## Nicht im Scope

- Migration bestehender SQLite-Daten nach PostgreSQL.
- Performance-Optimierung (Indizes sind bereits im Schema-Script vorhanden).
- Änderung der Domänenmodelle.

---

## Akzeptanzkriterien

| ID | Kriterium |
|----|-----------|
| AK-PGSQL-01 | `PgSqlDbService` besitzt alle öffentlichen Methoden des `SqliteDbService` mit identischen Signaturen. |
| AK-PGSQL-02 | Alle Schreiboperationen speichern Daten korrekt in der PostgreSQL-Instanz (Verifikation via SQL-Abfrage). |
| AK-PGSQL-03 | Der CSV-Import liest Daten erfolgreich in PostgreSQL ein. |
| AK-PGSQL-04 | Alle Methoden sind asynchron implementiert. |
| AK-PGSQL-05 | XML-Dokumentation für alle öffentlichen Member ist vorhanden. |
| AK-PGSQL-06 | `dotnet build` läuft ohne Warnungen (bezogen auf den neuen Code) durch. |

---

## Hinweis für Lernende

**Deutsch:** Dieses Vorhaben zeigt das Prinzip der "Provider-Parität". In einer professionellen Anwendung möchte man oft flexibel zwischen Datenbanken wechseln können (z.B. SQLite für lokale Entwicklung, PostgreSQL für Produktion). Eine identische API der Service-Klassen ist hierfür die Grundvoraussetzung.

**English:** This project demonstrates the principle of "provider parity". In professional applications, you often want the flexibility to switch between databases (e.g., SQLite for local development, PostgreSQL for production). Having an identical API across service classes is the essential prerequisite for this.
