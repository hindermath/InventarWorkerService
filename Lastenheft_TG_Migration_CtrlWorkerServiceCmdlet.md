# Lastenheft: Terminal.Gui Migration 1.19.x → 2.x — CtrlWorkerServiceCmdlet

**Dokument-Status:** Entwurf
**Erstellt:** 2026-03-31
**Betrifft:** `CtrlWorkerServiceCmdlet/`
**Empfohlener PR:** PR C (nach PR B — InventarViewerApp)
**Voraussetzung:** `dotnet test InventarWorkerService.sln` vollständig grün

> Dieses Lastenheft ist Teil einer dreiteiligen Lastenheft-Serie für die
> Terminal.Gui-Migration in InventarWorkerService. Überblick:
> [`Lastenheft_TerminalGui_Migration.md`](Lastenheft_TerminalGui_Migration.md)

---

## Ausgangslage

`CtrlWorkerServiceCmdlet` bringt eine **besondere Integrationssituation**: Das
TUI wird direkt innerhalb eines `PSCmdlet` gestartet und beendet. Terminal.Gui
läuft hier **im PowerShell-Prozesskontext**, nicht als eigenständige Anwendung.
Dies erfordert nach der Migration eine explizite PSCmdlet-Verifikation.

| Datei | Stellen | Hinweis |
|-------|---------|---------|
| `InvokeWorkerServiceControlCmdlet.cs` | `Application.Init()`, `Application.RequestStop()` ×2, `Application.Run()`, `Application.Shutdown()` | PSCmdlet-Kontext |

**Gesamt:** 1 Datei, 5 relevante Stellen

---

## Besonderheit: TUI im PSCmdlet-Kontext

```csharp
// Vereinfachtes Beispiel des bestehenden Musters:
[Cmdlet(VerbsLifecycle.Invoke, "WorkerServiceControl")]
public class InvokeWorkerServiceControlCmdlet : PSCmdlet
{
    protected override void ProcessRecord()
    {
        Application.Init();
        // ... TUI aufbauen ...
        Application.Run();
        Application.Shutdown();
    }
}
```

**Risiko:** Terminal.Gui 2.x ändert das interne Threading-Modell.
Es muss verifiziert werden, dass `Application.Init/Run/Shutdown` im
PowerShell-Prozesskontext weiterhin korrekt funktionieren, insbesondere
bei mehrfachem Aufruf des Cmdlets in einer PowerShell-Sitzung.

---

## API-Mapping-Tabelle

| v1.19.x | v2.x | Datei |
|---------|------|-------|
| `Application.Top` | Explizite `Toplevel`-Instanz | `InvokeWorkerServiceControlCmdlet.cs` (falls vorhanden) |
| `Colors.Base` | `Colors.ColorSchemes["Base"]` | `InvokeWorkerServiceControlCmdlet.cs` (falls vorhanden) |
| `Colors.Menu` | `Colors.ColorSchemes["Menu"]` | `InvokeWorkerServiceControlCmdlet.cs` (falls vorhanden) |

> `Application.Init()`, `Application.Run()`, `Application.Shutdown()` und
> `Application.RequestStop()` sind in v2 unverändert. Kein `MainLoop.Invoke()`
> in dieser Datei.

---

## Anforderungen

### R-TG-CSC-01: Terminal.Gui in CtrlWorkerServiceCmdlet auf 2.0.0 anheben

```xml
<!-- CtrlWorkerServiceCmdlet/CtrlWorkerServiceCmdlet.csproj -->
<PackageReference Include="Terminal.Gui" Version="2.0.0" />
```

### R-TG-CSC-02: `Application.Top` ablösen (falls vorhanden)

Vorhandene `Application.Top`-Zugriffe in `InvokeWorkerServiceControlCmdlet.cs`
durch explizite `Toplevel`-Instanz ersetzen:

```csharp
// v2-Empfehlung:
Application.Init();
var top = new Toplevel();
// ... top befüllen ...
Application.Run(top);
Application.Shutdown();
```

### R-TG-CSC-03: `Colors.*`-Zugriffe auf v2-API umstellen (falls vorhanden)

```csharp
// v1.x:
ColorScheme = Colors.Base;

// v2.x:
ColorScheme = Colors.ColorSchemes["Base"];
```

### R-TG-CSC-04: PSCmdlet-Kompatibilität verifizieren

Nach der Migration muss verifiziert werden, dass Terminal.Gui 2.x korrekt
im PowerShell-Prozesskontext startet und beendet.

**Testprozedur:**

```powershell
# Schritt 1: Projekt bauen
dotnet build CtrlWorkerServiceCmdlet/CtrlWorkerServiceCmdlet.csproj \
  --configuration Release

# Schritt 2: Modul laden
Import-Module ./CtrlWorkerServiceCmdlet/bin/Release/net10.0/CtrlWorkerServiceCmdlet.dll

# Schritt 3: Cmdlet aufrufen
Invoke-WorkerServiceControl -Tui

# Erwartung: TUI startet, ist per Keyboard bedienbar, beendet sich sauber
# Kein Absturz, kein hängender PowerShell-Prozess
```

**Zusätzlich prüfen:** Mehrfacher Aufruf in derselben PowerShell-Sitzung
(potenzielle Probleme mit `Application.Init/Shutdown`-Lebenszyklus in v2).

### R-TG-CSC-05: Alle bestehenden Tests müssen grün bleiben

```bash
dotnet test InventarWorkerService.sln --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults
```

Coverage-Ziel: ≥ 70% (Minimum), ≥ 80% (Ziel).

---

## Abnahmekriterien

| ID | Kriterium |
|----|-----------|
| AK-TG-CSC-01 | `CtrlWorkerServiceCmdlet.csproj` referenziert Terminal.Gui ≥ 2.0.0 |
| AK-TG-CSC-02 | `Application.Top` durch explizite `Toplevel`-Instanz ersetzt (falls vorhanden) |
| AK-TG-CSC-03 | `Colors.Base` / `Colors.Menu` durch `Colors.ColorSchemes[...]` ersetzt (falls vorhanden) |
| AK-TG-CSC-04 | `Invoke-WorkerServiceControl -Tui` startet und beendet sich sauber |
| AK-TG-CSC-05 | Mehrfacher Aufruf in einer PS-Sitzung führt zu keinem Absturz |
| AK-TG-CSC-06 | `dotnet test InventarWorkerService.sln` vollständig grün; Coverage ≥ 70% |

---

## Empfohlene PR-Reihenfolge im Gesamtkontext

```
PR A: Elmish entfernen aus CtrlWorkerServiceApp.csproj (Vorbedingung)
PR B: InventarViewerApp migrieren (komplex, MainLoop.Invoke)
PR C: CtrlWorkerServiceCmdlet migrieren (dieses Lastenheft)
PR D: CtrlWorkerServiceApp migrieren (einfachste Komponente)
```

PR C kommt nach PR B, weil dieselben v1→v2-Patterns angewendet werden
und PR B die meiste Erfahrung mit der neuen API liefert.

---

## Nicht im Scope dieses PRs

- `InventarViewerApp` und `CtrlWorkerServiceApp` (eigene Lastenhefte)
- PowerShell-Modul-Signierung oder -Packaging
- Neue Cmdlet-Parameter oder -Features

---

## Hinweis für Lernende

**Deutsch:** Dieses Lastenheft zeigt eine **Sonderform der TUI-Integration**:
Ein GUI-Framework (`Terminal.Gui`) wird in einem Cmdlet-Prozess gestartet,
der selbst kein eigenständiges UI-Programm ist. Solche „eingebetteten" TUI-
Instanzen stellen besondere Anforderungen an den Lebenszyklus des Frameworks.
Das Testen der mehrfachen Initialisierung (`Init` → `Shutdown` → `Init`) ist
ein wichtiger Qualitätspunkt, der in normalen Anwendungen selten auftritt.

**English:** This requirements document illustrates a **special TUI integration
form**: a UI framework (`Terminal.Gui`) is started inside a cmdlet process that
is not itself a standalone UI program. Such „embedded" TUI instances place
special requirements on the framework's lifecycle.
Testing multiple initialisation cycles (`Init` → `Shutdown` → `Init`) is an
important quality point that rarely arises in normal applications.

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
/speckit-specify Nutze Lastenheft_TG_Migration_CtrlWorkerServiceCmdlet.md als verbindliche Eingabedatei. Erstelle die Feature-Spezifikation fuer einen Terminal.Gui-Migrations- und Kompatibilitaetslauf im Repository InventarWorkerService.

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
