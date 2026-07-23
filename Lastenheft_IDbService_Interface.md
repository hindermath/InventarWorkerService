<!-- intake-authoring:begin -->
# Lastenheft: Einfuehrung des IDbService-Interfaces fuer Provider-Switching

**Dokument-Status:** Entwurf
**Erstellt:** 2026-04-18
**Betrifft:** `InventarWorkerCommon/Services/Database/SqliteDbService.cs`, `InventarWorkerCommon/Services/Database/PgSqlDbService.cs`, `InventarWorkerCommon/Services/Common/Initialize.cs`, `HarvesterWorkerService/Worker.cs`
**Prioritaet:** Mittel (Architektur-Verbesserung, Voraussetzung fuer flexiblen Provider-Wechsel)
**Herkunft:** Review des Lastenhefts "Vervollstaendigung der PostgreSQL-Implementierung", Punkt 6
**Abhaengigkeit:** Setzt die abgeschlossene PostgreSQL-Paritaet voraus (Lastenheft_PostgreSQL_Implementation.md)
**Reihenfolge:** 3 von 5

### Umsetzungsreihenfolge aller Lastenhefte

| Nr. | Lastenheft | Abhaengigkeit |
|-----|-----------|---------------|
| 1 | `Lastenheft_PostgreSQL_Implementation.md` | Keine |
| 2 | `Lastenheft_SQLite_ViewQuery_Bugfix.md` | Keine (unabhaengig, aber nach Nr. 1 geplant) |
| **3** | **`Lastenheft_IDbService_Interface.md`** (dieses Dokument) | Setzt Nr. 1 voraus |
| 4 | `Lastenheft_Statistik_View_Lesemethoden.md` | Setzt Nr. 1 voraus |
| 5 | `Lastenheft_MongoDB_Paritaet.md` | Keine direkte, logisch nach Nr. 1 |

---

## Ausgangslage

Nach Abschluss der PostgreSQL-Paritaet besitzen `SqliteDbService` und `PgSqlDbService` identische oeffentliche Methoden-Signaturen. Es fehlt jedoch ein formales Interface, das diese Parität abbildet. Aktuell referenziert der `ServiceContainer` beide Services als konkrete Typen. Ein Wechsel oder paralleler Betrieb erfordert manuelle Codeaenderungen an allen Aufrufstellen.

### Aktuelle oeffentliche API beider Services (nach PgSQL-Paritaet)

| Methode | Rueckgabe |
|---------|-----------|
| `InitializeDatabase()` | `void` |
| `SaveOrUpdateMachineAsync(Machine, bool)` | `Task<int>` |
| `SaveHardwareInventoryAsync(int, HardwareInventory)` | `Task` |
| `SaveSoftwareInventoryAsync(int, SoftwareInventory)` | `Task` |
| `GetMachinesAsync()` | `Task<List<Machine>>` |
| `GetAllActiveMachinesAsync()` | `Task<List<MachineState>>` |
| `GetAllActiveMachinesWithNetworkInfoAsync()` | `Task<List<MachineState>>` |
| `GetAllDeprovisionedMachinesAsync()` | `Task<List<MachineState>>` |
| `GetAllDisabledMachinesAsync()` | `Task<List<MachineState>>` |
| `GetMachineByIdAsync(int)` | `Task<Machine?>` |
| `GetMachineByNameAsync(string)` | `Task<Machine?>` |
| `GetLatestHardwareInventoryAsync(int)` | `Task<HardwareInventories?>` |
| `GetLatestSoftwareInventoryAsync(int)` | `Task<SoftwareInventories?>` |
| `CleanupOldRecordsAsync(int)` | `Task` |
| `HasMachineRecordsAsync()` | `Task<bool>` |
| `HasHardwareInventoryRecordsAsync()` | `Task<bool>` |
| `HasSoftwareInventoryRecordsAsync()` | `Task<bool>` |
| `GetMachineCountAsync()` | `Task<int>` |
| `GetHardwareInventoryCountAsync()` | `Task<int>` |
| `GetSoftwareInventoryCountAsync()` | `Task<int>` |
| `InitializeMachinesFromCsvAsync(string)` | `Task<int>` |

---

## Anforderungen

### R-IDB-01: Interface-Definition

Ein Interface `IDbService` wird unter `InventarWorkerCommon/Services/Database/` erstellt. Es enthaelt alle oeffentlichen Methoden, die sowohl `SqliteDbService` als auch `PgSqlDbService` anbieten (siehe Tabelle oben).

### R-IDB-02: Implementierung durch beide Services

- `SqliteDbService` implementiert `IDbService`.
- `PgSqlDbService` implementiert `IDbService`.
- Bestehende Funktionalitaet darf nicht veraendert werden.

### R-IDB-03: Anpassung des ServiceContainers

`ServiceContainer` exponiert die Datenbank-Services ueber das Interface:
- `DbService` wird zu `IDbService` (Property-Typ-Aenderung).
- `PgSqlDbService` wird zu `IDbService?` (nullable, da bei fehlendem Settings kein PgSQL-Service existiert).
- Der Konstruktor akzeptiert `IDbService` statt konkreter Typen.

### R-IDB-04: Anpassung der Aufrufer

Alle Stellen, die direkt `SqliteDbService` oder `PgSqlDbService` referenzieren, muessen auf `IDbService` umgestellt werden:
- `HarvesterWorkerService/Worker.cs`
- `InventarWorkerCommon/Services/Common/Initialize.cs`
- Weitere Aufrufer, die bei der Implementierung identifiziert werden.

### R-IDB-05: XML-Dokumentation

Das Interface `IDbService` muss vollstaendig mit XML-Kommentaren dokumentiert werden. Die Dokumentation der Methoden im Interface ist kanonisch; die implementierenden Klassen koennen via `<inheritdoc/>` darauf verweisen.

---

## Nicht im Scope

- Dependency-Injection-Registrierung (z.B. ueber `IServiceCollection`). Die manuelle Instanziierung in `Initialize.cs` bleibt erhalten.
- Automatisches Provider-Switching ueber Konfiguration (z.B. via `appsettings.json`). Der konkrete Provider wird weiterhin in `Initialize.cs` gewaehlt.
- Erweiterung des Interfaces um MongoDB-Methoden (MongoDB hat eine andere API-Oberflaeche).

---

## Akzeptanzkriterien

| ID | Kriterium |
|----|-----------|
| AK-IDB-01 | `IDbService` existiert unter `InventarWorkerCommon/Services/Database/` mit allen Methoden aus der Tabelle oben. |
| AK-IDB-02 | `SqliteDbService` und `PgSqlDbService` implementieren `IDbService`. |
| AK-IDB-03 | `ServiceContainer` nutzt `IDbService` als Property-Typen. |
| AK-IDB-04 | Kein Consumer referenziert mehr direkt `SqliteDbService` oder `PgSqlDbService` (Ausnahme: `Initialize.cs` bei der Instanziierung). |
| AK-IDB-05 | Alle bestehenden Tests kompilieren und laufen unveraendert gruen. |
| AK-IDB-06 | `dotnet build` laeuft ohne Warnungen (bezogen auf den neuen Code) durch. |

---

## Hinweis fuer Lernende

**Deutsch:** Dieses Feature zeigt das "Program to an interface, not an implementation"-Prinzip (SOLID: Dependency Inversion). Durch ein gemeinsames Interface koennen verschiedene Datenbank-Provider ausgetauscht werden, ohne dass die aufrufende Logik geaendert werden muss. In der Praxis ermoeglicht das z.B. SQLite fuer lokale Entwicklung und PostgreSQL fuer Produktion -- der Anwendungscode bleibt identisch.

**English:** This feature demonstrates the "Program to an interface, not an implementation" principle (SOLID: Dependency Inversion). A shared interface allows different database providers to be swapped without changing the calling logic. In practice, this enables e.g. SQLite for local development and PostgreSQL for production -- the application code remains identical.

---

## Spec-Kit-Intake-Reife / Spec Kit Intake Readiness

Dieses Lastenheft ist als Eingabedatei fuer einen spaeteren `/speckit-specify`-Lauf vorgesehen. Vor dem Start muss der aktuelle Repository-Stand geprueft werden, damit bereits erledigte oder ueberholte Punkte nicht erneut umgesetzt werden.

*This requirements document is intended as input for a later `/speckit-specify` run. Before starting, check the current repository state so already completed or superseded items are not implemented again.*

Der spaetere Lauf muss mindestens klassifizieren:

- `Applicable`: gilt fuer diesen Lauf und braucht Umsetzung oder Evidenz.
- `AlreadySatisfied`: ist im aktuellen Stand bereits nachweisbar erledigt.
- `N/A`: gilt fuer diesen Lauf nicht und braucht eine kurze Begruendung.
- `Open`: gilt, ist aber noch nicht ausreichend geklaert oder belegt.
- `FollowUp`: fachlich relevant, aber nicht Teil dieses Laufs.

## Kopierbarer `/speckit-specify`-Prompt / Copyable `/speckit-specify` Prompt

```text
Ersetzter Alt-Prompt: speckit-specify Nutze Lastenheft_IDbService_Interface.md als verbindliche Eingabedatei. Erstelle die Feature-Spezifikation fuer einen Datenbankservice-Interface-Lauf im Repository InventarWorkerService.

Ziel: Pruefe das Lastenheft gegen den aktuellen Repository-Stand und erstelle eine belastbare Spec-Kit-Spezifikation, die fuer Auszubildende, Entwickler*innen, Reviewer und KI-Agenten nachvollziehbar ist.

Pflichtpunkte:
- Lies dieses Lastenheft vollstaendig und uebernehme vorhandene Anforderungen, Scope-Grenzen, Reihenfolgehinweise und Akzeptanzkriterien.
- Pruefe, welche Punkte bereits umgesetzt, ueberholt oder noch offen sind.
- Klassifiziere Anforderungen als `Applicable`, `AlreadySatisfied`, `N/A`, `Open` oder `FollowUp`.
- Plane nur `Applicable`-Punkte fuer diesen Lauf.
- Dokumentiere fuer `N/A` und `FollowUp` jeweils eine kurze Begruendung.
- Beachte `constitution.md`, `.specify/memory/constitution.md`, AGENTS/CLAUDE/GEMINI/Copilot-Guidance, installierte Spec-Kit-Presets, Secure-Development-Basis, A11Y-Regeln, CEFR-B2-Verstaendlichkeit und didaktische Kommentar-Governance.
- Starte keinen weiteren Lastenheft-Lauf und kombiniere mehrere Lastenhefte nur, wenn die Kopplung fachlich begruendet und dokumentiert ist.

Erzeuge eine Spezifikation mit Scope, Nicht-Zielen, Anforderungen, Abhaengigkeiten, Akzeptanzkriterien, Risiken, Teststrategie, Evidenzpfaden und offenen Folgepunkten.
```
<!-- intake-authoring:prompts -->
## Kopierbare Spec-Kit-Prompts / Copy-Ready Spec Kit Prompts

Die folgenden Alternativen starten keinen Lauf automatisch. Der autonome
Prompt ist auf `LocalImplementation` begrenzt und erteilt keine Remote-,
PR-, Merge-, Bypass-, Secret- oder Provider-Berechtigung.

*The alternatives below do not start a run automatically. The autonomous
prompt is limited to `LocalImplementation` and grants no remote,
pull-request, merge, bypass, secret, or provider authority.*

### Specify

<!-- spec-kit-command-id: speckit.specify -->
```text
$speckit-specify Use Lastenheft_IDbService_Interface.md as the binding intake. Preserve its scope, non-goals, ordering, governance, evidence, and acceptance criteria. Create or update only the matching feature specification. Do not implement, commit, push, create a pull request, merge, or start another feature.
```

### Autonomous

<!-- spec-kit-command-id: speckit.autonomous -->
```text
$speckit-autonomous Execute one complete autonomous Spec Kit run using Lastenheft_IDbService_Interface.md as the binding intake. Delivery mode: LocalImplementation. Preserve all scope, ordering, security, accessibility, evidence, and acceptance boundaries. Do not push, create or merge a pull request, use bypass authority, expose secrets, or start a follow-up feature.
```
<!-- intake-authoring:end -->
