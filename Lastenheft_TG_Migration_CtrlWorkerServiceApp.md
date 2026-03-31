# Lastenheft: Terminal.Gui Migration 1.19.x → 2.x — CtrlWorkerServiceApp

**Dokument-Status:** Entwurf
**Erstellt:** 2026-03-31
**Betrifft:** `CtrlWorkerServiceApp/`
**Empfohlener PR:** PR D (nach PR A — Elmish-Entscheidung, und nach PR B + C)
**Voraussetzung:** Elmish-Entscheidung getroffen und umgesetzt (R-TG-CSA-01)

> Dieses Lastenheft ist Teil einer dreiteiligen Lastenheft-Serie für die
> Terminal.Gui-Migration in InventarWorkerService. Überblick:
> [`Lastenheft_TerminalGui_Migration.md`](Lastenheft_TerminalGui_Migration.md)

---

## Ausgangslage

`CtrlWorkerServiceApp` ist die **einfachste TUI-Komponente** im Solution: nur
1 Datei mit Terminal.Gui-Code. Die Migration ist technisch unkompliziert —
sie wird jedoch durch eine ungeklärte Abhängigkeit auf `Terminal.Gui.Elmish`
**blockiert**, die vor dem Versions-Upgrade entschieden werden muss.

| Datei | Stellen | Hinweis |
|-------|---------|---------|
| `Program.cs` | `Application.Init()`, `Application.Top`, `Application.RequestStop()` ×2, `Application.Run()`, `Application.Shutdown()` | Standardzyklus |

**Gesamt:** 1 Datei, 6 relevante Stellen

---

## Kritischer Befund: Terminal.Gui.Elmish (Vorbedingung)

`CtrlWorkerServiceApp.csproj` enthält:

```xml
<PackageReference Include="Terminal.Gui.Elmish" Version="2.2.1140" />
```

**Terminal.Gui.Elmish 2.2.x ist ausschließlich für Terminal.Gui 1.x entwickelt
und mit Terminal.Gui 2.x inkompatibel.**

Code-Analyse: **Kein Elmish-Code im Projekt gefunden** — keine
`Program.mkProgram`-, `ElmishApp.*`- oder `Program.run`-Aufrufe.
Die Abhängigkeit ist im Code **nicht aktiv genutzt**.

→ **Diese Entscheidung muss in einem eigenen PR A vor der Migration getroffen
werden.** Solange `Terminal.Gui.Elmish` referenziert wird, kann Terminal.Gui
nicht auf 2.x angehoben werden (Paket-Inkompatibilität).

---

## API-Mapping-Tabelle

| v1.19.x | v2.x | Datei |
|---------|------|-------|
| `Application.Top` | Explizite `Toplevel`-Instanz | `Program.cs` |
| `Colors.Base` | `Colors.ColorSchemes["Base"]` | `Program.cs` (falls vorhanden) |
| `Colors.Menu` | `Colors.ColorSchemes["Menu"]` | `Program.cs` (falls vorhanden) |

> `Application.Init()`, `Application.Run()`, `Application.Shutdown()` und
> `Application.RequestStop()` sind in v2 unverändert.

---

## Anforderungen

### R-TG-CSA-01: Elmish-Abhängigkeit klären und entfernen (Vorbedingung — PR A)

**Vor diesem Migrations-PR** ist zu entscheiden:

- **Empfehlung (bevorzugt):** `Terminal.Gui.Elmish`-Referenz aus
  `CtrlWorkerServiceApp.csproj` **entfernen**. Da kein aktiver Elmish-Code
  vorhanden ist, entsteht kein Funktionsverlust. Die Abhängigkeit war ein
  ungenutztes Planungs-Artefakt.

  ```xml
  <!-- Zu entfernen aus CtrlWorkerServiceApp.csproj: -->
  <PackageReference Include="Terminal.Gui.Elmish" Version="2.2.1140" />
  ```

- **Alternative:** Wenn Elmish-Architektur künftig umgesetzt werden soll,
  zunächst warten — zum Stand dieses Lastenhefte existiert kein offizieller
  `Terminal.Gui.Elmish`-Port für v2.x.

Diese Entscheidung ist im PR-A-Text zu dokumentieren.

### R-TG-CSA-02: Terminal.Gui in CtrlWorkerServiceApp auf 2.0.0 anheben

```xml
<!-- CtrlWorkerServiceApp/CtrlWorkerServiceApp.csproj -->
<PackageReference Include="Terminal.Gui" Version="2.0.0" />
```

Erst durchführbar, nachdem R-TG-CSA-01 umgesetzt ist.

### R-TG-CSA-03: `Application.Top` ablösen

`Application.Top` in `Program.cs` durch explizite `Toplevel`-Instanz ersetzen:

```csharp
// v2-Empfehlung:
Application.Init();
var top = new Toplevel();
// ... top befüllen ...
Application.Run(top);
Application.Shutdown();
```

### R-TG-CSA-04: `Colors.*`-Zugriffe auf v2-API umstellen

Vorhandene `Colors.Base` / `Colors.Menu`-Zugriffe in `Program.cs` prüfen
und ersetzen:

```csharp
// v1.x:
ColorScheme = Colors.Base;

// v2.x:
ColorScheme = Colors.ColorSchemes["Base"];
```

### R-TG-CSA-05: Alle bestehenden Tests müssen grün bleiben

```bash
dotnet test InventarWorkerService.sln --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults
```

Coverage-Ziel: ≥ 70% (Minimum), ≥ 80% (Ziel).

---

## Abnahmekriterien

| ID | Kriterium |
|----|-----------|
| AK-TG-CSA-01 | `Terminal.Gui.Elmish`-Referenz entfernt oder Entscheidung in PR-Text dokumentiert |
| AK-TG-CSA-02 | `CtrlWorkerServiceApp.csproj` referenziert Terminal.Gui ≥ 2.0.0 |
| AK-TG-CSA-03 | `Application.Top` durch explizite `Toplevel`-Instanz ersetzt |
| AK-TG-CSA-04 | `Colors.Base` / `Colors.Menu` durch `Colors.ColorSchemes[...]` ersetzt |
| AK-TG-CSA-05 | `dotnet test InventarWorkerService.sln` vollständig grün; Coverage ≥ 70% |
| AK-TG-CSA-06 | TUI startet und beendet sich sauber (manueller Smoke-Test) |

---

## Empfohlene PR-Reihenfolge im Gesamtkontext

```
PR A (diese Vorbedingung): Elmish entfernen aus CtrlWorkerServiceApp.csproj
PR B: InventarViewerApp migrieren (komplex, MainLoop.Invoke)
PR C: CtrlWorkerServiceCmdlet migrieren (PSCmdlet-spezifisch)
PR D: CtrlWorkerServiceApp migrieren (dieses Lastenheft)
```

PR D ist bewusst zuletzt, da CtrlWorkerServiceApp die einfachste Komponente
ist und ihre Vorbedingung (PR A) früh abgearbeitet werden kann.

---

## Nicht im Scope dieses PRs

- `InventarViewerApp` und `CtrlWorkerServiceCmdlet` (eigene Lastenhefte)
- Neue Features oder Elmish-Architektur
- Änderungen an der Worker-Service-Logik

---

## Hinweis für Lernende

**Deutsch:** Dieses Lastenheft zeigt das Problem einer **toten Abhängigkeit**
(`dead dependency`): Eine Bibliothek ist als Paket eingebunden, aber im Code
nirgends genutzt. Solche Abhängigkeiten erschweren spätere Migrationen,
weil sie Versionskonflikte erzeugen, obwohl keine Funktionalität dahintersteht.
Das frühzeitige Entfernen ungenutzter Abhängigkeiten ist eine empfohlene
Praxis in der Softwarepflege.

**English:** This requirements document illustrates the problem of a **dead
dependency**: a library is listed as a package reference but never used in code.
Such dependencies complicate future migrations by causing version conflicts
despite providing no functionality. Removing unused dependencies early is a
recommended software maintenance practice.
