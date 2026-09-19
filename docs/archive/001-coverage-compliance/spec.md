# Feature Specification: Testabdeckung auf Constitution Prinzip IV anheben

**Feature Branch**: `001-coverage-compliance`  
**Created**: 2026-03-10  
**Status**: Draft  
**Input**: User description: "Testabdeckung für InventarWorkerCommon und CtrlWorkerCommon auf >= 70% (CI-Gate) und >= 80% (Ziel) gemäß Verfassungsprinzip IV"

## Compliance-Analyse (Ist-Stand)

### Prinzip III – XML-Dokumentation
✅ **Compliant**: CS1591 ist via `Directory.Build.targets` als Build-Error erzwungen (nur für Nicht-Testprojekte). Der Build enthält 0 Fehler und 0 Warnungen → alle öffentlichen Typen und Member sind XML-dokumentiert.

### Prinzip IV – Testabdeckung
❌ **Nicht compliant**: CI-Gate ≥70%, Ziel ≥80%.

| Projekt              | Zeilen abgedeckt | Zeilenabdeckung | Status        |
|----------------------|-----------------|-----------------|---------------|
| `InventarWorkerCommon` | 40 / 2.871      | **1,4 %**       | ❌ Weit unter Gate |
| `CtrlWorkerCommon`   | 46 / 100        | **46,0 %**      | ❌ Unter Gate |

**Klassen mit 0 % Abdeckung (Top-Lücken in InventarWorkerCommon):**
- `HardwareInventoryService`: 1.218 ungetestete Zeilen
- `SoftwareInventoryService`: 1.158 ungetestete Zeilen
- `PgSqlDbService`: 592 ungetestete Zeilen
- `SqliteDbService`: 512 ungetestete Zeilen
- `ServiceStatusWriter`: 280 ungetestete Zeilen
- `ServiceContainer`: 122 ungetestete Zeilen
- `ServiceStatusReader`: 104 ungetestete Zeilen
- `SettingsWriter`, `SettingsReader`, `ResolveMachine`: weitere Lücken

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 – Unit-Tests für Status-Services (Priority: P1)

Als Entwickler möchte ich, dass `ServiceStatusWriter` und `ServiceStatusReader` durch Unit-Tests
abgedeckt sind, damit Statusdateien zuverlässig und regressionssicher geschrieben und gelesen werden.

As a developer I want `ServiceStatusWriter` and `ServiceStatusReader` covered by unit tests
so that status file I/O is reliable and regression-safe.

**Why this priority**: Diese Klassen sind einfach mit einem temporären Verzeichnis testbar (keine externe Infrastruktur), liefern sofort messbaren Coverage-Zuwachs und sind kritischer Bestandteil aller Dienste.

**Independent Test**: Vollständig testbar durch temporäre Dateisystemoperationen ohne Datenbank oder Netzwerk. Delivers verifiable status-file contract.

**Acceptance Scenarios**:

1. **Given** ein temporäres Verzeichnis existiert, **When** `ServiceStatusWriter.WriteStatus()` aufgerufen wird, **Then** wird eine gültige JSON-Statusdatei am erwarteten Pfad erstellt.
2. **Given** eine gültige Statusdatei existiert, **When** `ServiceStatusReader.ReadStatus()` aufgerufen wird, **Then** wird das korrekte Status-Objekt zurückgegeben.
3. **Given** keine Statusdatei existiert, **When** `ServiceStatusReader.ReadStatus()` aufgerufen wird, **Then** wird `null` zurückgegeben (keine Exception).
4. **Given** ein temporäres Verzeichnis existiert, **When** `ServiceStatusWriter.WriteStatistics()` und `WriteLog()` aufgerufen werden, **Then** werden Statistik-JSON und Log-Textdatei korrekt erstellt.
5. **Given** ein temporäres Verzeichnis existiert, **When** `ServiceStatusWriter.WritePerformanceMetrics()` aufgerufen wird, **Then** wird eine gültige Metrics-JSON-Datei am erwarteten Pfad erstellt.
6. **Given** ein temporäres Verzeichnis und ein `HardwareInfo`-Objekt, **When** `ServiceStatusWriter.WriteHardwareInventory()` aufgerufen wird, **Then** wird eine gültige Hardware-Inventar-JSON-Datei am erwarteten Pfad erstellt.

---

### User Story 2 – Unit-Tests für Settings-Services (Priority: P1)

Als Entwickler möchte ich, dass `SettingsReader` und `SettingsWriter` durch Unit-Tests abgedeckt sind,
damit Konfigurationsdateien korrekt deserialisiert und persistiert werden.

As a developer I want `SettingsReader` and `SettingsWriter` covered by unit tests so that
configuration round-trips are validated and regression-safe.

**Why this priority**: Settings-Services sind einfach testbar (kein I/O außer Dateisystem), sichern kritische Konfigurationslogik ab und liefern direkten Coverage-Beitrag.

**Independent Test**: Testbar durch temporäres Verzeichnis mit test-INI/JSON-Dateien. Liefert sofortigen Wert als eigenständige Verifikation des Konfigurations-Kontrakts.

**Acceptance Scenarios**:

1. **Given** eine gültige Settings-JSON-Datei, **When** `SettingsReader.ReadSettings()` aufgerufen wird, **Then** wird ein korrektes Settings-Objekt zurückgegeben.
2. **Given** keine Settings-Datei existiert, **When** `SettingsReader.ReadSettings()` aufgerufen wird, **Then** wird `null` zurückgegeben.
3. **Given** ein Settings-Objekt, **When** `SettingsWriter.WriteSettings()` aufgerufen wird, **Then** wird die Datei am korrekten Pfad geschrieben und die gelesenen Werte stimmen überein (Round-Trip).

---

### User Story 3 – Unit-Tests für SqliteDbService (Priority: P2)

Als Entwickler möchte ich, dass `SqliteDbService` durch Unit-Tests mit einer In-Memory-SQLite-Datenbank
abgedeckt ist, damit Datenbankoperationen korrekt funktionieren.

As a developer I want `SqliteDbService` covered by unit tests using an in-memory SQLite database
so that database operations are verified without external infrastructure.

**Why this priority**: Größter einzelner Coverage-Beitrag nach den Hardware-Services, vollständig ohne externe Infrastruktur testbar (In-Memory SQLite), direkter Einfluss auf das Erreichen des 70 %-Gates.

**Independent Test**: Testbar mit `Microsoft.Data.Sqlite` In-Memory-Datenbank (`Data Source=:memory:`). Delivers gesamte CRUD-Abdeckung für Maschinen, Hardware- und Software-Inventar.

**Acceptance Scenarios**:

1. **Given** eine neue In-Memory-Datenbank, **When** `InitializeDatabase()` aufgerufen wird, **Then** werden alle Tabellen und Indizes angelegt (idempotent, `IF NOT EXISTS`).
2. **Given** eine initialisierte Datenbank, **When** `SaveOrUpdateMachineAsync()` mit einer neuen Maschine aufgerufen wird, **Then** kann die Maschine via `GetMachinesAsync()` abgerufen werden.
3. **Given** eine vorhandene Maschine, **When** `SaveHardwareInventoryAsync()` aufgerufen wird, **Then** wird das Inventar gespeichert und via `GetLatestHardwareInventoryAsync()` abrufbar.
4. **Given** eine vorhandene Maschine, **When** `SaveSoftwareInventoryAsync()` aufgerufen wird, **Then** wird das Inventar gespeichert und via `GetLatestSoftwareInventoryAsync()` abrufbar.
5. **Given** CSV-Daten im erwarteten Format, **When** `InitializeMachinesFromCsvAsync()` aufgerufen wird, **Then** werden die Maschinen korrekt importiert.

---

### User Story 4 – Unit-Tests für CtrlWorkerCommon (Priority: P2)

Als Entwickler möchte ich, dass `CrossPlatformServiceController` weitere Branches abgedeckt hat,
damit die Service-Steuerungslogik auf 70 %+ kommt.

As a developer I want more branches covered in `CrossPlatformServiceController` to bring
`CtrlWorkerCommon` above the 70% CI gate.

**Why this priority**: CtrlWorkerCommon ist mit 46% relativ nah am Gate; wenige zusätzliche Tests reichen, um das 70%-Gate zu erreichen.

**Independent Test**: Kann unabhängig von InventarWorkerCommon getestet werden — plattformspezifische Tests werden per `[TestCategory("Windows")]` / Skip-Logik ausgeführt.

**Acceptance Scenarios**:

1. **Given** ein NSubstitute-Mock von `IWindowsServiceController` (Windows-only Wrapper), **When** Fehlerpfade in `CrossPlatformServiceController` getriggert werden, **Then** werden die korrekten Exceptions geworfen.
2. **Given** alle vorhandenen und neuen Tests, **When** `dotnet test CtrlWorkerCommonTest` ausgeführt wird, **Then** ist die Zeilenabdeckung ≥ 70%.

---

### User Story 6 – Unit-Tests für SoftwareInventoryService (Priority: P2 — **Pflicht für 70%-Gate**)

Als Entwickler möchte ich, dass die plattformunabhängigen Teile von `SoftwareInventoryService`
durch Unit-Tests abgedeckt sind, damit das 70%-Coverage-Gate für `InventarWorkerCommon` erreichbar ist.

As a developer I want the platform-independent parts of `SoftwareInventoryService` covered
by unit tests so that the 70% CI gate for `InventarWorkerCommon` is achievable.

**Why this priority**: Ohne `SoftwareInventoryService` (813 Zeilen) deckt der Plan nur ~23% ab — weit unter dem 70%-Gate. P1+P2+Story6 zusammen sind mathematisch notwendig, um SC-001 zu erfüllen.

**Independent Test**: Teile ohne OS-Aufrufe (Parsing, Filterung, Aggregationslogik, Modell-Transformation) sind direkt testbar. OS-Calls werden über Interfaces abstrahiert oder mit `RuntimeInformation.IsOSPlatform()` geprüft.

**Acceptance Scenarios**:

1. **Given** Roh-Prozessdaten im erwarteten Format, **When** `GetInstalledSoftware()` / Parsing-Logik aufgerufen wird, **Then** werden korrekte typisierte `SoftwareInfo`-Objekte zurückgegeben.
2. **Given** bekannte plattformunabhängige Hilfsmethoden, **When** diese mit gültigen und ungültigen Eingaben aufgerufen werden, **Then** verhalten sie sich wie spezifiziert.
3. **Given** alle vorhandenen und neuen Tests inklusive Story 6, **When** `dotnet test InventarWorkerCommonTest` ausgeführt wird, **Then** ist die Zeilenabdeckung ≥ 70%.

---

### User Story 5 – Mock-basierte Tests für HardwareInventoryService (Priority: P3 — **strikt optional, kein Gate-Blocking**)

Als Entwickler möchte ich, dass die plattformunabhängigen Teile von `HardwareInventoryService`
durch Unit-Tests abgedeckt sind.

As a developer I want the platform-independent parts of `HardwareInventoryService` covered
by unit tests, keeping platform-specific I/O behind mockable interfaces.

**Why this priority**: HardwareInventoryService hat mit 1.218 Zeilen das größte Coverage-Potential, aber auch das höchste Mock-Aufwand. **Strikt optional** — wird nur implementiert wenn Zeit verbleibt, nachdem P1+P2+Story4+Story6 abgeschlossen sind. P3 blockiert weder das 70%-Gate noch das 80%-Ziel.

**Independent Test**: Teile ohne OS-Aufrufe (z. B. Parsing, Formatierung, Aggregationslogik) sind direkt testbar. OS-Calls werden über Interfaces abstrahiert oder mit `RuntimeInformation` geprüft.

**Acceptance Scenarios**:

1. **Given** Roh-Daten im erwarteten Format, **When** Parsing-Methoden aufgerufen werden, **Then** liefern sie korrekte typisierte Objekte.
2. **Given** plattformunabhängige Hilfsmethoden, **When** diese mit gültigen und ungültigen Eingaben aufgerufen werden, **Then** verhalten sie sich wie spezifiziert.

---

### Edge Cases

- **Ausgabeverzeichnis für Statusdateien fehlt**: `ServiceStatusWriter` legt das Verzeichnis automatisch an (bestehende Produktionslogik via `ServicePath.CreateServiceStatusPath()`). Tests übergeben ein GUID-basiertes Temp-Verzeichnis als Konstruktorparameter.
- **SQLite korrupt / kein Schreibrecht**: Außerhalb des Scope dieser Phase — Tests verwenden ausschließlich In-Memory-Datenbanken.
- **CSV-Importdaten mit fehlerhaften Zeilen**: Fehlerhafte Zeilen werden übersprungen, der Rest wird importiert (partial import). Tests verifizieren dieses Verhalten mit gemischten gültig/ungültig-CSV-Daten.
- **`ServiceStatusReader` liest ungültiges JSON**: Gibt `null` zurück — Fehler wird still geschluckt (robust). Tests verifizieren `null`-Rückgabe bei korrumpierter Datei.
- **Parallele SQLite-Schreibzugriffe (Concurrency)**: Außerhalb des Scope — In-Memory-Tests sind single-threaded.

---

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: `InventarWorkerCommonTest` MUSS nach Abschluss eine Zeilenabdeckung von ≥ 70% für `InventarWorkerCommon` erreichen.
- **FR-002**: `CtrlWorkerCommonTest` MUSS nach Abschluss eine Zeilenabdeckung von ≥ 70% für `CtrlWorkerCommon` erreichen.
- **FR-003**: Alle neuen Tests MÜSSEN dem Namensschema `<UnitUnderTest>_<Scenario>_<ExpectedOutcome>` folgen.
- **FR-004**: Alle neuen Tests MÜSSEN deterministisch und unabhängig von maschinellen Zustand sein (kein echtes Netzwerk, keine echte Windows-Service-Registry, keine produktive Datenbank).
- **FR-005**: Tests für `SqliteDbService` MÜSSEN eine In-Memory-SQLite-Datenbank (`Data Source=:memory:`) verwenden.
- **FR-006**: Tests für `ServiceStatusWriter` und `ServiceStatusReader` MÜSSEN ein temporäres Verzeichnis verwenden (`Path.GetTempPath()` + GUID) und dieses nach dem Test bereinigen.
- **FR-007**: Der Build MUSS nach Abschluss weiterhin 0 Fehler und 0 Warnungen erzeugen.
- **FR-008**: Die bestehenden 31 passing Tests DÜRFEN nicht gebrochen werden.
- **FR-009**: Plattformspezifische Tests (Windows-only) MÜSSEN per `[TestCategory]` oder `[Ignore]` mit Begründung markiert sein, sodass sie auf macOS/Linux übersprungen werden.
- **FR-010**: Bilingual (Deutsch/Englisch, CEFR B2) kommentierte Testklassen und XML-Dokumentation auf allen öffentlichen Testmethoden wo zutreffend.
- **FR-011**: `CtrlWorkerCommon` MUSS ein Interface `IWindowsServiceController` erhalten (Windows-only Wrapper für `System.ServiceProcess.ServiceController`; Methoden: `Start`, `Stop`, `WaitForStatus`). `CrossPlatformServiceController` konsumiert dieses Interface via DI-Konstruktor; der bestehende `(string serviceName)`-Konstruktor bleibt erhalten.
- **FR-012**: `ServiceStatusReader.ReadStatus()` MUSS bei existierender Datei mit ungültigem/korrumpiertem JSON `null` zurückgeben — keine Exception (robust/defensive).
- **FR-013**: `InitializeMachinesFromCsvAsync()` MUSS fehlerhafte CSV-Zeilen überspringen und den Rest der gültigen Zeilen importieren (partial import).
- **FR-014**: `SettingsReader`, `ServiceStatusReader` und `ServiceStatusWriter` MÜSSEN einen internen Konstruktor erhalten, der einen vollständigen Verzeichnispfad direkt akzeptiert (kein `ServicePath`-Lookup). Die Testprojekte erhalten `[assembly: InternalsVisibleTo(...)]`-Attribute, um diesen Konstruktor zu nutzen.
- **FR-015**: `SoftwareInventoryService` MUSS durch Unit-Tests abgedeckt sein (plattformunabhängige Teile). Diese Story ist Pflichtbestandteil für das Erreichen des 70%-Gates (SC-001).
- **FR-016**: Die Klassen `PgSqlDbService`, `MongoDbService`, `Initialize`, `ServiceContainer`, `ResolveMachine` und `ApiService` MÜSSEN mit `[ExcludeFromCodeCoverage]` annotiert werden. Sie sind bewusst aus dem Scope ausgeschlossen (externe Infrastruktur / OS-Abhängigkeit / HTTP-Clients ohne Mock-Server) und dürfen den Coverage-Nenner nicht negativ beeinflussen.

---

## Constitution Alignment *(mandatory)*

- **CA-001 Branching**: Feature wird auf Branch `001-coverage-compliance` entwickelt und via PR in `main` gemergt.
- **CA-002 Toolchain**: Neue Tests verwenden .NET 10 / C# 14.0; keine Änderungen an bestehenden Produktionsprojekten außer ggf. minimaler Interface-Extraktion für Testbarkeit.
- **CA-003 Dependency Currency**: `coverlet.collector` wird zu beiden Testprojekten hinzugefügt (bereits für `InventarWorkerCommonTest` vorhanden). **`NSubstitute`** wird als Mocking-Framework zu beiden Testprojekten hinzugefügt. Keine weiteren neuen Pakete erwartet.
- **CA-004 Coverage**: Dieses Feature IST die Coverage-Compliance-Maßnahme. Ziel: ≥70% CI-Gate für beide Projekte, ≥80% angestrebt. User Story 6 (`SoftwareInventoryService`) ist mathematisch notwendig für SC-001. `[ExcludeFromCodeCoverage]` wird auf nicht testbaren Klassen (`PgSqlDbService`, `MongoDbService`, `Initialize`, `ServiceContainer`, `ResolveMachine`, `ApiService`) gesetzt, damit der Coverage-Nenner nur testbare Klassen enthält.
- **CA-005 Layering**: Neue Tests liegen in `InventarWorkerCommonTest/` und `CtrlWorkerCommonTest/`. **Minimale Produktionscode-Änderungen sind erlaubt**: (1) `IWindowsServiceController`-Interface in `CtrlWorkerCommon`; (2) interne Konstruktoren für `SettingsReader` und `ServiceStatusWriter` mit direktem Pfad-Parameter; (3) ggf. Interface-Extraktion für `SoftwareInventoryService` OS-Calls. Keine bestehende Funktionalität wird verändert.
- **CA-006 Linguistic Rules**: Testklassen und Kommentare bilingual (Deutsch zuerst, Englisch zweite, CEFR B2).
- **CA-007 Documentation Enforcement**: Keine API-Signatur-Änderungen an öffentlichen Produktions-APIs erwartet; `docfx` muss nur ausgeführt werden, falls doch.
- **CA-008 Testing Impact**: Red-Green-Refactor-Zyklus wird eingehalten. Keine bestehenden Tests werden verändert, außer zur Behebung von Konflikten.
- **CA-009 Data Contracts**: `SqliteDbService`-Tests verwenden In-Memory-SQLite; keine Produktionsdaten werden berührt.

---

## Assumptions

- `HardwareInventoryService` und `SoftwareInventoryService` werden für P3 (Story 5) nur in den plattformunabhängigen Teilen getestet. Die vollständige Mock-Abstraktion der OS-Layer ist aufwändig und für das 70%-Gate nicht erforderlich.
- `PgSqlDbService` und `MongoDbService` werden in dieser Feature-Phase NICHT mit Live-Datenbank-Tests abgedeckt (externe Infrastruktur). Ihre Abdeckung wird optional mit In-Memory/Mock-Strategien angegangen, falls das 80%-Ziel es erfordert.
- **`ServiceStatusReader`** erhält ebenfalls einen internen Konstruktor mit `InternalsVisibleTo` — gleiche ServicePath-Kopplung wie SettingsReader und ServiceStatusWriter (FR-014).
- **`[ExcludeFromCodeCoverage]`** wird auf `PgSqlDbService`, `MongoDbService`, `Initialize`, `ServiceContainer`, `ResolveMachine` und `ApiService` gesetzt — externe Infrastruktur / OS-abhängige / HTTP-Klassen werden vom Nenner ausgeschlossen (FR-016).
- **NSubstitute** ist das gewählte Mocking-Framework für alle Test-Doubles.
- **P3 (User Story 5, HardwareInventoryService)** ist strikt optional und kein Gate-Blocking — wird nur implementiert wenn nach Abschluss von P1+P2+Story4+Story6 noch Kapazität besteht.
- **User Story 6 (SoftwareInventoryService)** ist P2 Pflicht — ohne sie ist das 70%-Gate für `InventarWorkerCommon` mathematisch nicht erreichbar.
- **Minimale Produktionscode-Änderungen sind erlaubt**: `IWindowsServiceController`-Interface (Windows-only), interne Konstruktoren mit `InternalsVisibleTo` für `SettingsReader`/`ServiceStatusWriter`.
- **`IWindowsServiceController`** ist ein Windows-only Wrapper-Interface für `System.ServiceProcess.ServiceController` (Methoden: `Start`, `Stop`, `WaitForStatus`); kein cross-platform Scope.
- **`ServiceStatusReader`** gibt `null` zurück bei ungültigem JSON (kein Throw).
- **CSV-Import** überspringt fehlerhafte Zeilen (partial import), wirft keine Exception.

---

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: `dotnet test InventarWorkerCommonTest --collect:"XPlat Code Coverage"` liefert eine Zeilenabdeckung von **≥ 70%** für `InventarWorkerCommon`.
- **SC-002**: `dotnet test CtrlWorkerCommonTest --collect:"XPlat Code Coverage"` liefert eine Zeilenabdeckung von **≥ 70%** für `CtrlWorkerCommon`.
- **SC-003**: Angestrebte Zeilenabdeckung beider Projekte liegt bei **≥ 80%**.
- **SC-004**: `dotnet build InventarWorkerService.sln` liefert weiterhin **0 Fehler, 0 Warnungen**.
- **SC-005**: Alle bestehenden **31 passing Tests** bleiben grün; 2 macOS-Skip-Tests bleiben übersprungen.
- **SC-006**: Neue Tests sind vollständig deterministisch — kein Test schlägt bei wiederholter Ausführung inkonsistent fehl.
