# Lastenheft: Terminal.Gui Migration 1.19.x → 2.x — Überblick (InventarWorkerService)

**Dokument-Status:** Überblick (verweist auf drei Einzel-Lastenhefte)
**Erstellt:** 2026-03-31
**Aktualisiert:** 2026-03-31 — in drei Einzel-Lastenhefte aufgeteilt
**Betrifft:** `InventarViewerApp/`, `CtrlWorkerServiceApp/`, `CtrlWorkerServiceCmdlet/`
**Voraussetzung:** `dotnet test InventarWorkerService.sln` muss vor Beginn vollständig grün sein.

---

## Einzel-Lastenhefte (operative Dokumente)

Dieses Dokument dient als **Überblick und Einstiegspunkt**.
Die drei Einzel-Lastenhefte enthalten die vollständigen Anforderungen,
API-Mappings und Abnahmekriterien für jeweils eine Komponente:

| PR | Komponente | Dokument | Umfang |
|----|-----------|----------|--------|
| **PR A** ⚠️ | **Elmish-Abhängigkeit entfernen** (Vorbedingung für PR D) | [Lastenheft_TG_Elmish_Entscheidung.md](Lastenheft_TG_Elmish_Entscheidung.md) | `Terminal.Gui.Elmish`-Referenz aus `CtrlWorkerServiceApp.csproj` entfernen — 1 Zeile, kein Funktionsverlust |
| **PR B** | **InventarViewerApp** | [Lastenheft_TG_Migration_InventarViewerApp.md](Lastenheft_TG_Migration_InventarViewerApp.md) | 6 Dateien, `MainLoop.Invoke()` (kritischste Stelle) |
| **PR C** | **CtrlWorkerServiceCmdlet** | [Lastenheft_TG_Migration_CtrlWorkerServiceCmdlet.md](Lastenheft_TG_Migration_CtrlWorkerServiceCmdlet.md) | 1 Datei, PSCmdlet-Kontext, Mehrfach-Init-Test |
| **PR D** | **CtrlWorkerServiceApp** | [Lastenheft_TG_Migration_CtrlWorkerServiceApp.md](Lastenheft_TG_Migration_CtrlWorkerServiceApp.md) | 1 Datei, einfachste Komponente (nach PR A) |

> **PR A ist Pflicht-Vorbedingung** für PR D: Solange `Terminal.Gui.Elmish` referenziert ist,
> kann Terminal.Gui in `CtrlWorkerServiceApp` nicht auf 2.x angehoben werden (Paket-Inkompatibilität).
> PR B und PR C sind von PR A **unabhängig** und können parallel oder zuerst gestartet werden.

---

## Ausgangslage

Drei TUI-Komponenten nutzen **Terminal.Gui 1.19.0** (statische API).
Zusammen umfassen sie **8 Dateien** mit Terminal.Gui-Code und
**13 statische `Application.*`-Aufrufe**.

| Projekt | Terminal.Gui-Dateien | Besonderheit |
|---------|:-------------------:|-------------|
| InventarViewerApp | 6 | `Application.MainLoop.Invoke()` (Thread-sicher) |
| CtrlWorkerServiceApp | 1 | `Terminal.Gui.Elmish` Abhängigkeit (ungeklärt) |
| CtrlWorkerServiceCmdlet | 1 | PSCmdlet — TUI eingebettet in PowerShell |

---

## Kritischer Befund: Terminal.Gui.Elmish

`CtrlWorkerServiceApp.csproj` enthält:
```xml
<PackageReference Include="Terminal.Gui.Elmish" Version="2.2.1140" />
```

**Terminal.Gui.Elmish 2.2.x ist ausschließlich für Terminal.Gui 1.x entwickelt
und mit 2.x inkompatibel.**

Code-Analyse: **Kein Elmish-Code im Projekt gefunden** (keine `Program.mkProgram`-,
`ElmishApp.*`- oder `Program.run`-Aufrufe). Die Abhängigkeit ist im Code
**nicht aktiv genutzt** — sie wurde vermutlich für eine geplante, nie umgesetzte
Elmish-Architektur hinzugefügt.

→ **Entscheidung vor Beginn der Migration erforderlich** (R-TG-IWS-02).

---

## Vollständige Stellen-Übersicht

### InventarViewerApp

| Datei | Stellen |
|-------|---------|
| `TuiApp.cs` | `Application.Init()`, `Application.Run()`, `Application.Shutdown()` |
| `UI/MainWindow.cs` | `Application.Run(dialog)` ×2, `Application.RequestStop()`, `Application.MainLoop.Invoke()` ×5, `Application.Top` |
| `UI/SettingsDialog.cs` | `Application.RequestStop()` ×2 |
| `UI/HardwareView.cs` | Terminal.Gui-Typen (keine Application.*-Aufrufe) |
| `UI/SoftwareView.cs` | Terminal.Gui-Typen (keine Application.*-Aufrufe) |
| `UI/StatusView.cs` | Terminal.Gui-Typen (keine Application.*-Aufrufe) |

**Kritisch: `Application.MainLoop.Invoke(action)`** — dieses Muster wird in
`MainWindow.cs` fünfmal verwendet, um UI-Updates thread-sicher aus
Hintergrundthreads auszuführen. In v2 ist `Application.MainLoop` entfernt;
Ersatz: `Application.Invoke(action)`.

### CtrlWorkerServiceApp

| Datei | Stellen |
|-------|---------|
| `Program.cs` | `Application.Init()`, `Application.Top`, `Application.RequestStop()` ×2, `Application.Run()`, `Application.Shutdown()` |

### CtrlWorkerServiceCmdlet

| Datei | Stellen |
|-------|---------|
| `InvokeWorkerServiceControlCmdlet.cs` | `Application.Init()`, `Application.RequestStop()` ×2, `Application.Run()`, `Application.Shutdown()` |

---

## Anforderungen

### R-TG-IWS-01: Terminal.Gui in allen drei Projekten auf 2.0.0 anheben

In `InventarViewerApp.csproj`, `CtrlWorkerServiceApp.csproj` und
`CtrlWorkerServiceCmdlet.csproj` die `PackageReference` aktualisieren:

```xml
<PackageReference Include="Terminal.Gui" Version="2.0.0" />
```

### R-TG-IWS-02: Elmish-Abhängigkeit klären und entfernen

**Vor dem ersten Migrations-PR** ist zu entscheiden:

- **Empfehlung (bevorzugt):** `Terminal.Gui.Elmish`-Referenz aus
  `CtrlWorkerServiceApp.csproj` **entfernen**, da kein aktiver Elmish-Code
  im Projekt vorhanden ist. Die Abhängigkeit war ungenutztes
  Planungs-Artefakt.
- **Alternative:** Wenn Elmish-Architektur für CtrlWorkerServiceApp in einem
  künftigen PR umgesetzt werden soll, auf einen kompatiblen Nachfolger warten
  (zum Stand dieses Lastenhefte existiert kein offizieller
  `Terminal.Gui.Elmish`-Port für v2.x).

Diese Entscheidung ist im zugehörigen PR-Text zu dokumentieren.

### R-TG-IWS-03: `Application.MainLoop.Invoke()` ersetzen

`InventarViewerApp/UI/MainWindow.cs` nutzt fünfmal das Muster:

```csharp
// v1.x — entfernt in v2:
Application.MainLoop.Invoke(() => { /* UI-Update */ });
```

Ersatz in v2:

```csharp
// v2.x:
Application.Invoke(() => { /* UI-Update */ });
```

Alle 5 Vorkommen sind zu ersetzen. Dies ist die **einzige echte Breaking-Change-Stelle**
in InventarViewerApp.

### R-TG-IWS-04: `Application.Top` ablösen

`Application.Top` (statische Eigenschaft, v1.x-Stil) wird in drei Stellen
(MainWindow.cs, CtrlWorkerServiceApp/Program.cs, CtrlWorkerServiceCmdlet) genutzt.
In v2 ist `Application.Top` noch vorhanden, aber das empfohlene Muster ist,
die `Toplevel`-Instanz explizit zu erstellen und zu übergeben:

```csharp
// v2-Empfehlung:
Application.Init();
var top = new Toplevel();
// ... top befüllen ...
Application.Run(top);
Application.Shutdown();
```

### R-TG-IWS-05: Key-Binding-API auf v2-Syntax umstellen

Alle `Key.CtrlMask | Key.X`-Ausdrücke müssen auf v2-Syntax umgestellt werden.
Bekannte Stellen:

| Datei | Alt | Neu |
|-------|-----|-----|
| `MainWindow.cs` | `Key.CtrlMask \| Key.ShiftMask \| Key.Q` | `Key.Ctrl + Key.Shift + Key.Q` |
| `MainWindow.cs` | `Key.CtrlMask \| Key.B` | `Key.Ctrl + Key.B` |
| `MainWindow.cs` | `Key.CtrlMask \| Key.ShiftMask \| Key.W` | `Key.Ctrl + Key.Shift + Key.W` |

`new StatusItem(Key.CtrlMask | Key.B, ...)` → v2-`StatusItem`-Konstruktor prüfen,
da die v2-Signatur abweichen kann.

### R-TG-IWS-06: `Colors.Base` / `Colors.Menu` auf v2-API umstellen

`CtrlWorkerServiceApp` und `CtrlWorkerServiceCmdlet` nutzen:

```csharp
// v1.x:
ColorScheme = Colors.Base;
ColorScheme = Colors.Menu;
```

In v2 lautet der Zugriff:

```csharp
// v2.x:
ColorScheme = Colors.ColorSchemes["Base"];
ColorScheme = Colors.ColorSchemes["Menu"];
```

### R-TG-IWS-07: CtrlWorkerServiceCmdlet — PSCmdlet-Kompatibilität verifizieren

Das `InvokeWorkerServiceControlCmdlet` bettet `Application.Init/Run/Shutdown`
direkt in einem `PSCmdlet`-Aufruf ein. Nach der Migration muss verifiziert werden,
dass Terminal.Gui 2.x korrekt im PowerShell-Prozesskontext startet und beendet.

**Testprozedur:**
```powershell
Import-Module ./CtrlWorkerServiceCmdlet/bin/Release/net10.0/CtrlWorkerServiceCmdlet.dll
Invoke-WorkerServiceControl -Tui
# Erwartung: TUI startet, kann per Keyboard beendet werden, kein Absturz
```

### R-TG-IWS-08: Alle bestehenden Tests müssen grün bleiben

```bash
dotnet test InventarWorkerService.sln --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults
```

Coverage-Ziel: ≥ 70% (Minimum), ≥ 80% (Ziel).

### R-TG-IWS-09: FakeDriver-Tests einführen (mittelfristig)

Terminal.Gui 2.x bietet `FakeDriver` für Headless-Tests. Aktuell gibt es
**keine** TUI-Tests in InventarWorkerService. Nach der Migration sollen
FakeDriver-basierte Tests für die kritischsten UI-Pfade ergänzt werden:
mindestens CSV-Import-Dialog, Settings-Dialog und Quit-Shortcut.
Dies ist **kein Pflichtziel des Migrations-PRs**, sondern ein Folge-PR.

---

## Empfohlene PR-Reihenfolge

1. **PR A — Elmish-Entscheidung:** `CtrlWorkerServiceApp.csproj`: Elmish-Abhängigkeit
   entfernen (minimaler, sicherer Schritt, kein Funktionsverlust).
2. **PR B — InventarViewerApp:** Die komplexeste Komponente zuerst — enthält
   `MainLoop.Invoke()`-Stellen und die meisten TUI-Dateien.
3. **PR C — CtrlWorkerServiceCmdlet:** PSCmdlet mit TUI — nach InventarViewerApp,
   weil dieselben Patterns gelten.
4. **PR D — CtrlWorkerServiceApp:** Einfachste Komponente (1 Datei), nach Elmish-Klärung.

---

## Nicht im Scope

- Migration von `InventarWorkerService` (REST API) oder `HarvesterWorkerService` (kein TUI)
- `ServiceStatusReaderApp` (verwendet kein Terminal.Gui)
- Neue Features oder Datenbankschicht

---

## Akzeptanzkriterien

| ID | Kriterium |
|----|-----------|
| AK-TG-IWS-01 | Alle drei .csproj-Dateien referenzieren Terminal.Gui ≥ 2.0.0 |
| AK-TG-IWS-02 | `Terminal.Gui.Elmish`-Abhängigkeit entfernt oder Entscheidung dokumentiert |
| AK-TG-IWS-03 | `Application.MainLoop.Invoke()` durch `Application.Invoke()` ersetzt (5 Stellen) |
| AK-TG-IWS-04 | `Application.Top` durch explizite `Toplevel`-Instanz ersetzt |
| AK-TG-IWS-05 | Alle `Key.CtrlMask`-Ausdrücke durch v2-Syntax ersetzt |
| AK-TG-IWS-06 | `Colors.Base` / `Colors.Menu` durch `Colors.ColorSchemes[...]` ersetzt |
| AK-TG-IWS-07 | `Invoke-WorkerServiceControl -Tui` startet und beendet sauber |
| AK-TG-IWS-08 | `dotnet test InventarWorkerService.sln` vollständig grün; Coverage ≥ 70% |

---

## Beispiel: Agentic-AI-Dialog (Platzhalter)

Dieser Abschnitt wird während der Umsetzung mit Commit-URLs und Zeitstempeln befüllt.

---

## Hinweis für Lernende

**Deutsch:** Dieses Lastenheft zeigt, dass eine Bibliotheks-Migration in einem
gewachsenen System mehr als ein Versions-Bump ist. Die `Application.MainLoop.Invoke()`-
Stellen sind ein Beispiel für **Thread-Synchronisierung in TUI-Anwendungen** —
ein wichtiges Konzept, das auch in anderen Frameworks (WPF, Avalonia) in ähnlicher
Form auftritt. Die Elmish-Abhängigkeit zeigt, wie ungenutzter Code (`dead dependency`)
Migrationen erschweren kann.

**English:** This requirements document shows that a library migration in a grown system
involves more than a version bump. The `Application.MainLoop.Invoke()` occurrences
illustrate **thread synchronisation in TUI applications** — an important concept that
appears in similar forms in other frameworks (WPF, Avalonia). The Elmish dependency
shows how unused code (`dead dependency`) can complicate migrations.

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
/speckit-specify Nutze Lastenheft_TerminalGui_Migration.md als verbindliche Eingabedatei. Erstelle die Feature-Spezifikation fuer einen Terminal.Gui-Migrations- und Kompatibilitaetslauf im Repository InventarWorkerService.

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
