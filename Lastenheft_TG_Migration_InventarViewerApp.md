# Lastenheft: Terminal.Gui Migration 1.19.x → 2.x — InventarViewerApp

**Dokument-Status:** Entwurf
**Erstellt:** 2026-03-31
**Betrifft:** `InventarViewerApp/`
**Empfohlener PR:** PR B (nach PR A — Elmish-Entscheidung in CtrlWorkerServiceApp)
**Voraussetzung:** `dotnet test InventarWorkerService.sln` vollständig grün

> Dieses Lastenheft ist Teil einer dreiteiligen Lastenheft-Serie für die
> Terminal.Gui-Migration in InventarWorkerService. Überblick:
> [`Lastenheft_TerminalGui_Migration.md`](Lastenheft_TerminalGui_Migration.md)

---

## Ausgangslage

`InventarViewerApp` ist die **komplexeste TUI-Komponente** im Solution: 6 Dateien
mit Terminal.Gui-Code, darunter eine kritische thread-sichere UI-Update-Stelle
(`Application.MainLoop.Invoke`), die in Terminal.Gui 2.x entfernt wurde.

| Datei | Stellen | Hinweis |
|-------|---------|---------|
| `TuiApp.cs` | `Application.Init()`, `Application.Run()`, `Application.Shutdown()` | Standardzyklus |
| `UI/MainWindow.cs` | `Application.Run(dialog)` ×2, `Application.RequestStop()`, `Application.MainLoop.Invoke()` ×5, `Application.Top` | **Kritisch** |
| `UI/SettingsDialog.cs` | `Application.RequestStop()` ×2 | — |
| `UI/HardwareView.cs` | Terminal.Gui-Typen (keine `Application.*`-Aufrufe) | Typen-Aktualisierung |
| `UI/SoftwareView.cs` | Terminal.Gui-Typen (keine `Application.*`-Aufrufe) | Typen-Aktualisierung |
| `UI/StatusView.cs` | Terminal.Gui-Typen (keine `Application.*`-Aufrufe) | Typen-Aktualisierung |

**Gesamt:** 6 Dateien, ~13 relevante Stellen

---

## Kritischer Befund: `Application.MainLoop.Invoke()`

`MainWindow.cs` nutzt fünfmal das Muster:

```csharp
// v1.x — entfernt in v2:
Application.MainLoop.Invoke(() =>
{
    // UI-Update aus Hintergrundthread
});
```

`Application.MainLoop` existiert in Terminal.Gui 2.x **nicht mehr**.
Dies ist die einzige echte Breaking-Change-Stelle in InventarViewerApp.

---

## API-Mapping-Tabelle

| v1.19.x | v2.x | Datei |
|---------|------|-------|
| `Application.MainLoop.Invoke(action)` | `Application.Invoke(action)` | `MainWindow.cs` (5×) |
| `Application.Top` | Explizite `Toplevel`-Instanz | `MainWindow.cs`, `TuiApp.cs` |
| `Key.CtrlMask \| Key.ShiftMask \| Key.Q` | `Key.Ctrl + Key.Shift + Key.Q` | `MainWindow.cs` |
| `Key.CtrlMask \| Key.B` | `Key.Ctrl + Key.B` | `MainWindow.cs` |
| `Key.CtrlMask \| Key.ShiftMask \| Key.W` | `Key.Ctrl + Key.Shift + Key.W` | `MainWindow.cs` |
| `new StatusItem(Key.CtrlMask \| Key.B, ...)` | v2-`StatusItem`-Konstruktor prüfen | `MainWindow.cs` |

---

## Anforderungen

### R-TG-IVA-01: Terminal.Gui in InventarViewerApp auf 2.0.0 anheben

```xml
<!-- InventarViewerApp/InventarViewerApp.csproj -->
<PackageReference Include="Terminal.Gui" Version="2.0.0" />
```

### R-TG-IVA-02: `Application.MainLoop.Invoke()` ersetzen

Alle 5 Vorkommen in `UI/MainWindow.cs`:

```csharp
// Vorher:
Application.MainLoop.Invoke(() => { /* UI-Update */ });

// Nachher:
Application.Invoke(() => { /* UI-Update */ });
```

**Wichtig:** `Application.Invoke()` in v2 verhält sich semantisch identisch —
es stellt sicher, dass der Delegate im UI-Thread ausgeführt wird.

### R-TG-IVA-03: `Application.Top` ablösen

`Application.Top` (statische Eigenschaft, v1.x-Stil) wird in `MainWindow.cs`
und `TuiApp.cs` genutzt. Empfohlenes v2-Muster:

```csharp
// v2-Empfehlung (TuiApp.cs):
Application.Init();
var top = new Toplevel();
Application.Run(top);
Application.Shutdown();
```

### R-TG-IVA-04: Key-Binding-Syntax auf v2 umstellen

Alle `Key.CtrlMask`-Ausdrücke in `MainWindow.cs` gemäß Mapping-Tabelle ersetzen.
`StatusItem`-Konstruktor-Signatur in v2 verifizieren.

### R-TG-IVA-05: Alle bestehenden Tests müssen grün bleiben

```bash
dotnet test InventarWorkerService.sln --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults
```

Coverage-Ziel: ≥ 70% (Minimum), ≥ 80% (Ziel).

### R-TG-IVA-06: FakeDriver-Tests einführen (Folge-PR)

Terminal.Gui 2.x bietet `FakeDriver` für Headless-Tests. Aktuell gibt es
**keine** TUI-Tests in InventarViewerApp. Nach der Migration sollen
FakeDriver-basierte Tests für die kritischsten UI-Pfade ergänzt werden:
mindestens CSV-Import-Dialog, Settings-Dialog und Quit-Shortcut.

Dies ist **kein Pflichtziel dieses Migrations-PRs**, sondern ein eigener Folge-PR.

---

## Abnahmekriterien

| ID | Kriterium |
|----|-----------|
| AK-TG-IVA-01 | `InventarViewerApp.csproj` referenziert Terminal.Gui ≥ 2.0.0 |
| AK-TG-IVA-02 | `Application.MainLoop.Invoke()` vollständig durch `Application.Invoke()` ersetzt (5 Stellen) |
| AK-TG-IVA-03 | `Application.Top` durch explizite `Toplevel`-Instanz ersetzt |
| AK-TG-IVA-04 | Alle `Key.CtrlMask`-Ausdrücke durch v2-Syntax ersetzt |
| AK-TG-IVA-05 | `dotnet test InventarWorkerService.sln` vollständig grün; Coverage ≥ 70% |
| AK-TG-IVA-06 | TUI startet und beendet sich sauber (manueller Smoke-Test) |

---

## Nicht im Scope dieses PRs

- `CtrlWorkerServiceApp` und `CtrlWorkerServiceCmdlet` (eigene Lastenhefte)
- FakeDriver-Tests (Folge-PR, R-TG-IVA-06)
- Neue Features oder Änderungen an der Datenbankschicht

---

## Hinweis für Lernende

**Deutsch:** Das `Application.MainLoop.Invoke()`-Muster zeigt ein klassisches
Problem in UI-Frameworks: **Hintergrundthreads dürfen UI-Elemente nicht direkt
ändern** — die Änderung muss in den UI-Thread „geposted" werden.
Terminal.Gui 2.x hat diesen Mechanismus vereinfacht: `Application.Invoke()`
ersetzt direkt das umständlichere `Application.MainLoop.Invoke()`.
Dasselbe Konzept findet sich in WPF (`Dispatcher.Invoke`), WinForms
(`Control.Invoke`) und Avalonia (`Dispatcher.UIThread.Post`).

**English:** The `Application.MainLoop.Invoke()` pattern illustrates a classic
UI-framework problem: **background threads must not directly modify UI elements**
— changes must be „posted" to the UI thread.
Terminal.Gui 2.x simplified this: `Application.Invoke()` directly replaces the
more cumbersome `Application.MainLoop.Invoke()`.
The same concept appears in WPF (`Dispatcher.Invoke`), WinForms
(`Control.Invoke`), and Avalonia (`Dispatcher.UIThread.Post`).
