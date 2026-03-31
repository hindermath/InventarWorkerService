# Lastenheft: Elmish-Abhängigkeit entfernen (CtrlWorkerServiceApp) — PR A

**Dokument-Status:** Entwurf
**Erstellt:** 2026-03-31
**Betrifft:** `CtrlWorkerServiceApp/CtrlWorkerServiceApp.csproj`
**Empfohlener PR:** PR A — vor allen Terminal.Gui-Migrations-PRs
**Aufwand:** Minimal (1 Zeile entfernen)

> **Einordnung:** Dieses Lastenheft beschreibt ausschließlich die Entfernung
> einer ungenutzten Paket-Abhängigkeit. Es ist **keine Terminal.Gui-Migration**
> und enthält **keine Code-Änderungen**. Die eigentliche Migration von
> `CtrlWorkerServiceApp` folgt in PR D:
> [`Lastenheft_TG_Migration_CtrlWorkerServiceApp.md`](Lastenheft_TG_Migration_CtrlWorkerServiceApp.md)

---

## Hintergrund und Befund

Code-Analyse vom 2026-03-31 ergab:

`CtrlWorkerServiceApp.csproj` enthält folgende Paket-Referenz:

```xml
<PackageReference Include="Terminal.Gui.Elmish" Version="2.2.1140" />
```

**Befund:** Im gesamten Quellcode von `CtrlWorkerServiceApp` existiert
**kein Elmish-Code**. Geprüfte Muster ohne Treffer:

| Suchmuster | Ergebnis |
|-----------|----------|
| `Program.mkProgram` | nicht gefunden |
| `ElmishApp.` | nicht gefunden |
| `Program.run` | nicht gefunden |
| `open Elmish` | nicht gefunden |

**Schlussfolgerung:** Die Abhängigkeit ist ein **ungenutztes Planungs-Artefakt**
(`dead dependency`) — vermutlich für eine nie umgesetzte Elmish-Architektur
hinzugefügt.

---

## Warum ein eigenes Lastenheft?

`Terminal.Gui.Elmish 2.2.x` ist **ausschließlich mit Terminal.Gui 1.x kompatibel**
und mit Terminal.Gui 2.x **inkompatibel**. Solange diese Referenz vorhanden ist,
kann Terminal.Gui in `CtrlWorkerServiceApp` nicht auf Version 2.x angehoben werden.

Die Entfernung muss als **eigenständiger, atomarer PR A** vor allen
Migrations-PRs erfolgen, damit:

1. Die Entscheidung explizit dokumentiert und nachvollziehbar ist
2. Kein Agentic-AI-Lauf die Entfernung mit der Migration vermischt
3. Der `dotnet build`-Erfolg nach der Entfernung isoliert verifiziert werden kann
4. Ein Rückgängigmachen (falls doch Elmish-Architektur geplant) sauber möglich bleibt

---

## Anforderungen

### R-ELM-01: `Terminal.Gui.Elmish`-Referenz entfernen

In `CtrlWorkerServiceApp/CtrlWorkerServiceApp.csproj` folgende Zeile entfernen:

```xml
<!-- Zu entfernen: -->
<PackageReference Include="Terminal.Gui.Elmish" Version="2.2.1140" />
```

Die Datei darf nach der Änderung **keine weiteren Anpassungen** enthalten —
ausschließlich diese eine Zeile wird gelöscht.

### R-ELM-02: Kein Elmish-Code im Projekt

Verifizieren, dass im Anschluss kein Elmish-Code in `CtrlWorkerServiceApp`
vorhanden ist (Suchprüfung):

```bash
grep -r "Elmish\|mkProgram\|ElmishApp" \
  InventarWorkerService/CtrlWorkerServiceApp/ \
  --include="*.cs" --include="*.fsx"
# Erwartung: kein Treffer
```

### R-ELM-03: Entscheidung im PR-Text dokumentieren

Der PR-Text muss folgende Aussage enthalten:

> „`Terminal.Gui.Elmish` wurde als ungenutzte Abhängigkeit entfernt.
> Im gesamten Quellcode von CtrlWorkerServiceApp wurde kein Elmish-Code gefunden.
> Falls eine Elmish-Architektur in einem künftigen PR eingeführt werden soll,
> muss zunächst ein kompatibler Elmish-Port für Terminal.Gui 2.x verfügbar sein."

### R-ELM-04: Build und Tests müssen grün bleiben

```bash
dotnet restore InventarWorkerService.sln
dotnet build InventarWorkerService.sln --configuration Release
dotnet test InventarWorkerService.sln --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults
```

Alle Schritte müssen **ohne Fehler oder Warnungen** bzgl. Elmish abschließen.
Coverage-Ziel: ≥ 70% (Minimum), ≥ 80% (Ziel).

---

## Abnahmekriterien

| ID | Kriterium |
|----|-----------|
| AK-ELM-01 | `CtrlWorkerServiceApp.csproj` enthält keine `Terminal.Gui.Elmish`-Referenz mehr |
| AK-ELM-02 | `grep`-Prüfung findet keinen Elmish-Code in `CtrlWorkerServiceApp/` |
| AK-ELM-03 | PR-Text enthält die dokumentierte Entscheidungsbegründung (R-ELM-03) |
| AK-ELM-04 | `dotnet build InventarWorkerService.sln` erfolgreich ohne Elmish-Warnungen |
| AK-ELM-05 | `dotnet test InventarWorkerService.sln` vollständig grün; Coverage ≥ 70% |

---

## Explizit NICHT im Scope dieses PRs

- Keine Terminal.Gui-API-Änderungen (kein Versions-Bump auf 2.x)
- Keine Änderungen an `.cs`-Dateien
- Keine Einführung einer Ersatzarchitektur für Elmish
- Keine Änderungen an anderen Projekten im Solution

---

## Nächster Schritt nach diesem PR

Nach Abschluss von PR A ist `CtrlWorkerServiceApp` für die Terminal.Gui-Migration
freigegeben. Die Migration selbst erfolgt in **PR D**:
[`Lastenheft_TG_Migration_CtrlWorkerServiceApp.md`](Lastenheft_TG_Migration_CtrlWorkerServiceApp.md)

PRs B und C (`InventarViewerApp` und `CtrlWorkerServiceCmdlet`) sind von diesem
PR **unabhängig** und können parallel oder vor PR A gestartet werden.

---

## Hinweis für Lernende

**Deutsch:** Eine **tote Abhängigkeit** (`dead dependency`) ist eine
Bibliothek, die als Paket eingebunden, aber im Code nirgends genutzt wird.
Sie erhöht die Buildzeit, verursacht potenzielle Sicherheits-Updates ohne
Nutzen und — wie hier — kann sie zukünftige Upgrades blockieren.
Das regelmäßige Entfernen ungenutzter Abhängigkeiten (z. B. mit
`dotnet list package --deprecated` oder `dotnet outdated`) ist eine
empfohlene Wartungspraxis.

**English:** A **dead dependency** is a library listed as a package reference
but never used in code. It increases build time, triggers unnecessary security
updates, and — as here — can block future upgrades.
Regularly removing unused dependencies (e.g., with
`dotnet list package --deprecated` or `dotnet outdated`) is a recommended
maintenance practice.
