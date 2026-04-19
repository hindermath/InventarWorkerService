# Lastenheft: Vervollständigung der PostgreSQL-Implementierung (PGSQL-Parität)

**Dokument-Status:** Entwurf (Review-Entscheidungen eingearbeitet)
**Erstellt:** 2026-04-12
**Letzte Überarbeitung:** 2026-04-18
**Betrifft:** `InventarWorkerCommon/Services/Database/PgSqlDbService.cs`, `InventarWorkerCommon/Services/Common/Initialize.cs`, `HarvesterWorkerService/Worker.cs`
**Priorität:** Hoch (Herstellung der funktionalen Parität zur SQLite-Implementierung)
**Reihenfolge:** 1 von 5 (dieses Lastenheft wird zuerst umgesetzt)

### Umsetzungsreihenfolge aller Lastenhefte

| Nr. | Lastenheft | Abhaengigkeit |
|-----|-----------|---------------|
| **1** | **`Lastenheft_PostgreSQL_Implementation.md`** (dieses Dokument) | Keine |
| 2 | `Lastenheft_SQLite_ViewQuery_Bugfix.md` | Keine (unabhaengig, aber nach Nr. 1 geplant) |
| 3 | `Lastenheft_IDbService_Interface.md` | Setzt Nr. 1 voraus |
| 4 | `Lastenheft_Statistik_View_Lesemethoden.md` | Setzt Nr. 1 voraus |
| 5 | `Lastenheft_MongoDB_Paritaet.md` | Keine direkte, logisch nach Nr. 1 |

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
- **Async/Await:** Konsequente Nutzung von asynchronen Methoden (Parität zu SQLite). **Ausnahme:** `InitializeDatabase()` bleibt synchron, da die Methode einmalig beim Startup aufgerufen wird und in SQLite ebenfalls synchron ist.
- **DateTime-Handling:** PostgreSQL erwartet für `timestamptz` in der Regel UTC. Sicherstellen, dass `DateTime.UtcNow` verwendet wird.
- **View-Namen:** Die Namen der Views in PostgreSQL müssen exakt denen in SQLite entsprechen (PascalCase), um die Abfrageroutine identisch halten zu können. Die bestehende Abweichung `hardware_inventory_view` muss im Rahmen dieses Features auf `HardwareInventoryView` umbenannt werden. Ebenso müssen die Spalten-Aliase dieser View an die SQLite-Konvention angeglichen werden (z.B. `machine_id` → `MachineID`, `machine_name` → `MachineName`).
- **Dokumentation:** Alle neuen öffentlichen Methoden müssen vollständig mit XML-Kommentaren (zweisprachig oder konsistent zum Projekt) dokumentiert werden.

### R-PGSQL-07: Vorbereitung für Provider-Switching (Ausblick)

Die Implementierung muss so sauber sein, dass der `HarvesterWorkerService` in einem späteren Schritt leicht auf PostgreSQL umgestellt oder beide parallel betrieben werden können. Ein formales `IDbService`-Interface ist **nicht** Teil dieses Vorhabens (siehe "Nicht im Scope"), die identischen Methoden-Signaturen bilden jedoch die Grundlage für ein späteres Interface-Refactoring.

### R-PGSQL-08: Konfigurations-Integration und Write-Safety

- **R-PGSQL-08.1: Nutzung der Settings:** Der `PgSqlDbService` muss den über `Initialize.cs` bereitgestellten Connection-String (inkl. User/Passwort aus `PgSqlDb.PgSqlConnectionString`) für alle Datenbankverbindungen nutzen.
- **R-PGSQL-08.2: Berücksichtigung von `WriteEnabled`:** Im `HarvesterWorkerService` muss sichergestellt werden, dass Schreibzugriffe auf PostgreSQL nur dann erfolgen, wenn `PgSqlDb.WriteEnabled` auf `true` gesetzt ist. Dies verhindert Fehlermeldungen bei unkonfigurierten PostgreSQL-Instanzen. Die Worker-Integration erfolgt analog zu den bestehenden MongoDB-Aufrufen in `Worker.cs`.
- **R-PGSQL-08.3: Fallback-Pfad ohne Settings:** Wenn keine Settings-Datei vorhanden ist (parameterloser Aufruf von `Initialize.Services()`), wird die PostgreSQL-Initialisierung übersprungen. Der Connection-String ohne Credentials kann keine authentifizierte Verbindung aufbauen. `ServiceContainer.PgSqlDbService` darf in diesem Fall `null` sein; alle Aufrufer müssen darauf prüfen.

---

## Nicht im Scope

- Migration bestehender SQLite-Daten nach PostgreSQL.
- Performance-Optimierung (Indizes sind bereits im Schema-Script vorhanden).
- Änderung der Domänenmodelle.
- Lesemethoden für Statistik-Views (`ComputerModelStatisticsView`, `ArchitectureStatisticsView`, `ModelArchitectureStatisticsView`, `HardwareStatisticsOverview`). Diese Views werden zwar beim Initialisieren erstellt, C#-Abfragemethoden sind jedoch weder in SQLite noch in PostgreSQL vorhanden und können als eigenes Feature nachgeliefert werden.
- Einführung eines formalen `IDbService`-Interfaces für Provider-Switching. Die identischen Methoden-Signaturen bereiten dies vor, das Interface-Refactoring ist ein eigenständiges Feature mit Auswirkung auf alle Consumer.
- Herstellung der MongoDB-Parität. Der `MongoDbService` besitzt aktuell nur Schreibmethoden und eine einzelne Abfrage. Die Erweiterung des MongoDB-Services ist nicht Gegenstand dieses Vorhabens.
- Behebung bestehender SQLite-Bugs (z.B. fehlerhafte View-Abfragen in `GetAllDeprovisionedMachinesAsync` und `GetAllDisabledMachinesAsync`). Diese werden in einem separaten Bug-Fix adressiert; die PgSQL-Implementierung verwendet von Anfang an die korrekten View-Abfragen.

---

## Akzeptanzkriterien

| ID | Kriterium |
|----|-----------|
| AK-PGSQL-01 | `PgSqlDbService` besitzt alle öffentlichen Methoden des `SqliteDbService` mit identischen Signaturen. |
| AK-PGSQL-02 | Alle Schreiboperationen speichern Daten korrekt in der PostgreSQL-Instanz (Verifikation via SQL-Abfrage). |
| AK-PGSQL-03 | Der CSV-Import liest Daten erfolgreich in PostgreSQL ein. |
| AK-PGSQL-04 | Alle Methoden sind asynchron implementiert (Ausnahme: `InitializeDatabase()` bleibt synchron, Parität zu SQLite). |
| AK-PGSQL-05 | XML-Dokumentation für alle öffentlichen Member ist vorhanden. |
| AK-PGSQL-06 | `dotnet build` läuft ohne Warnungen (bezogen auf den neuen Code) durch. |
| AK-PGSQL-07 | Der `HarvesterWorkerService` schreibt bei `WriteEnabled = true` parallel zu SQLite und MongoDB auch nach PostgreSQL. |
| AK-PGSQL-08 | Bei `WriteEnabled = false` oder fehlender Settings-Datei werden keine PostgreSQL-Schreibzugriffe ausgeführt und keine Exceptions geworfen. |
| AK-PGSQL-09 | Die View `HardwareInventoryView` in PostgreSQL verwendet PascalCase-Namen und -Spaltenaliase, identisch zur SQLite-Variante. |

---

## Hinweis für Lernende

**Deutsch:** Dieses Vorhaben zeigt das Prinzip der "Provider-Parität". In einer professionellen Anwendung möchte man oft flexibel zwischen Datenbanken wechseln können (z.B. SQLite für lokale Entwicklung, PostgreSQL für Produktion). Eine identische API der Service-Klassen ist hierfür die Grundvoraussetzung.

**English:** This project demonstrates the principle of "provider parity". In professional applications, you often want the flexibility to switch between databases (e.g., SQLite for local development, PostgreSQL for production). Having an identical API across service classes is the essential prerequisite for this.
