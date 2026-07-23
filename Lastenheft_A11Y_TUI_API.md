<!-- intake-authoring:begin -->
# Lastenheft: Barrierefreiheit InventarWorkerService (A11Y)

**Dokument-Status:** Entwurf
**Erstellt:** 2026-03-31
**Betrifft:** `InventarViewerApp/`, `CtrlWorkerServiceApp/`, `ServiceStatusReaderApp/`,
`InventarWorkerService/` (REST API), `InventarWorkerCommon/`
**Empfohlener Durchführungszeitraum:** Parallel zur nächsten Feature-Runde; die
Kurzfrist-Anforderungen (R-A11Y-IWS-01 bis R-A11Y-IWS-03) können sofort beginnen.
Die Terminal.Gui-Migration (R-A11Y-IWS-05) ist als eigener PR geplant.
**Grundlage:** `../docs/tui-a11y-assessment.md` im RiderProjects-Workspace

---

## Ausgangslage

InventarWorkerService besteht aus mehreren Komponenten mit unterschiedlichen UI-Typen:

| Komponente | UI-Typ | Framework |
|-----------|--------|-----------|
| InventarViewerApp | TUI | Terminal.Gui **1.19.0** + Elmish |
| CtrlWorkerServiceApp | TUI + CLI | Terminal.Gui **1.19.0** + Elmish |
| CtrlWorkerServiceCmdlet | PowerShell-Modul | Terminal.Gui **1.19.0** |
| ServiceStatusReaderApp | Console CLI | Nur .NET Console |
| InventarWorkerService | REST API | ASP.NET Core + Swagger 10.1.5 |
| HarvesterWorkerService | Worker Service | Kein UI |

Die TUI-Komponenten nutzen noch **Terminal.Gui 1.19.0** (statische API) —
wie TinyCalc. Die REST API exponiert eine Swagger-UI unter `/swagger`.
Screen-Reader-Nutzer können den sichtbaren Terminal-Text lesen, erkennen aber
keine semantischen UI-Rollen.

---

## Anforderungen

### R-A11Y-IWS-01: InventarViewerApp — vollständige Shortcut-Legende in der StatusBar

Die StatusBar von `InventarViewerApp` zeigt bereits einige Shortcuts (`Ctrl+B`, `Ctrl+T`,
`Ctrl+W`). Die Legende muss auf **alle** wichtigen Shortcuts erweitert werden:

| Shortcut | Aktion |
|----------|--------|
| F1 | Über das Programm |
| F3 | CSV importieren |
| F5 | Daten aktualisieren |
| F8 | Einstellungen |
| F12 | Aktivitätsprotokoll |
| Ctrl+Shift+Q | Beenden |
| Ctrl+Shift+W | WebAPI ein/aus |

Da die StatusBar platzbegrenzt ist, ist eine kontextabhängige Rotation oder eine
zweite StatusBar-Zeile zulässig.

### R-A11Y-IWS-02: InventarViewerApp — Text-Bestätigung nach jeder Aktion

Nach jeder benutzerinitierten Aktion muss eine Meldung als sichtbarer Text
in einer Statuszeile oder einem Log-Bereich erscheinen. Reine Farb- oder
Fokusänderungen genügen nicht:

Beispiele:
- F5 Refresh → `„Daten aktualisiert: <N> Maschinen, <Datum/Uhrzeit>"`
- F3 CSV-Import → `„Import erfolgreich: <N> Datensätze eingelesen"` oder Fehlertext
- Ctrl+Shift+W → `„WebAPI gestartet"` / `„WebAPI gestoppt"`

### R-A11Y-IWS-03: CtrlWorkerServiceApp — Hilfe-Text und Keyboard-Navigation

Der `--help`-Output von `CtrlWorkerServiceApp` muss vollständig und strukturiert sein:
alle Befehle (`start`, `stop`, `--tui`, `--help`) mit kurzer Beschreibung und
erwarteten Exit-Codes. Im TUI-Modus (Flag `--tui`) gelten dieselben Anforderungen
wie für `InventarViewerApp` (R-A11Y-IWS-01 und R-A11Y-IWS-02).

### R-A11Y-IWS-04: ServiceStatusReaderApp — farb-unabhängige Ausgabe

`ServiceStatusReaderApp` gibt Status-Informationen farbig aus (`[OK]` grün,
`[FEHLER]` rot usw.). Diese Unterscheidungen müssen **auch ohne Farbe** erkennbar sein:

- Statusanzeigen als Text-Präfix: `[OK]`, `[FEHLER]`, `[WARNUNG]` — nicht nur als Farbe
- `NO_COLOR`-Umgebungsvariable und `--no-color`-Flag müssen respektiert werden
- Tabellenausgabe muss als Plain-Text lesbar sein (keine Box-Drawing-Characters
  als einzige Strukturierungsebene)

### R-A11Y-IWS-05: Migration Terminal.Gui 1.19.0 → 2.0.0

Alle drei TUI-Komponenten (InventarViewerApp, CtrlWorkerServiceApp,
CtrlWorkerServiceCmdlet) verwenden noch Terminal.Gui 1.19.0.
Die Migration auf 2.0.0 (instanzbasiertes Lifecycle-Modell) ist Voraussetzung für:

- Besseres `ColorScheme`-API für High-Contrast-Modus
- `FakeDriver`-Unterstützung für Headless-Tests
- Langfristige Wartbarkeit (Parität mit TinyPl0)

Die Migration soll als eigenständiger PR mit vollständigem Regressionstest
(`dotnet test InventarWorkerService.sln`) durchgeführt werden.

### R-A11Y-IWS-06: Swagger-UI — Hinweise für Screen-Reader-Nutzer

Die Swagger-UI unter `/swagger` wird von der Bibliothek `Swashbuckle` gerendert
und kann nicht vollständig kontrolliert werden. Dennoch sind folgende Maßnahmen möglich:

- XML-Dokumentationskommentare auf allen API-Endpunkten und DTOs vollständig
  pflegen — diese werden in der OpenAPI-Spec als `description`-Felder ausgegeben
  und sind maschinenlesbar
- Eine Hinweiszeile in der API-Dokumentation (DocFX oder README) ergänzen:
  „Die Swagger-UI ist eingeschränkt barrierefrei. Die vollständige API-Spezifikation
  steht als maschinenlesbare OpenAPI-JSON unter `/swagger/v1/swagger.json` bereit."
- Alternativ: OpenAPI-JSON via `docfx` in die statische Dokumentation integrieren

### R-A11Y-IWS-07: A11Y-Tests für Dokumentations-HTML (Playwright+axe)

Die mit `docfx` generierte Dokumentation des InventarWorkerService muss durch
Playwright-Tests mit `@axe-core/playwright` auf WCAG 2.2 AA-Konformität geprüft
werden — analog zu TuiVision. Zusätzlich: `lynx`-Validierung.
Integration in `ci.yml` als eigener CI-Job.

### R-A11Y-IWS-08: Farbkontrast WCAG 2.2 AA (nach Migration)

Nach der Terminal.Gui-Migration (R-A11Y-IWS-05) müssen alle
`ColorScheme`-Definitionen in allen TUI-Komponenten einen Mindestkontrast
von **4,5:1** (WCAG 2.2 AA, Normaltext) einhalten.

---

## Nicht im Scope

- Vollständige Screen-Reader-Semantik (Terminal.Gui bietet keine UI-Automation-Integration)
- Barrierefreiheit der HarvesterWorkerService-Hintergrundprozesse (kein UI)
- Vollständige Überarbeitung der Swagger-UI (liegt bei Swashbuckle/OpenAPI)
- Änderungen an der Datenbankschicht (SQLite/MongoDB/PostgreSQL)

---

## Akzeptanzkriterien

| ID | Kriterium |
|----|-----------|
| AK-A11Y-IWS-01 | InventarViewerApp StatusBar zeigt alle 7 Shortcuts vollständig |
| AK-A11Y-IWS-02 | Jede Aktion erzeugt sichtbaren Statustext in InventarViewerApp |
| AK-A11Y-IWS-03 | `--help`-Output strukturiert; CtrlWorkerServiceApp TUI-Modus mit Shortcut-Legende |
| AK-A11Y-IWS-04 | ServiceStatusReaderApp: `NO_COLOR` respektiert; Status als Text-Präfix |
| AK-A11Y-IWS-05 | `dotnet test InventarWorkerService.sln` nach Terminal.Gui-Migration vollständig grün |
| AK-A11Y-IWS-06 | Alle API-Endpunkte und DTOs haben vollständige XML-Dokumentation |
| AK-A11Y-IWS-07 | Playwright+axe-Tests für DocFX-HTML ohne serious/critical Violations; in CI |
| AK-A11Y-IWS-08 | Alle ColorScheme-Kombinationen nach Migration ≥ 4,5:1 Kontrast |

---

## Beispiel: Agentic-AI-Dialog (Platzhalter für spätere Durchführung)

Dieser Abschnitt wird während der Umsetzung mit Agentic-AI plus Spec-Kit/SDD
befüllt — jeder Schritt mit Commit-URL und Zeitstempel.

---

## Hinweis für Lernende

**Deutsch:** InventarWorkerService zeigt, dass „Barrierefreiheit" nicht nur eine Frage
des UI-Frameworks ist. Eine REST API ist barrierefrei, wenn ihre Endpunkte vollständig
dokumentiert sind. Ein CLI-Tool ist barrierefrei, wenn es farb-unabhängig lesbar bleibt.
Ein TUI ist barrierefrei, wenn alle Aktionen als Text rückgemeldet werden.

**English:** InventarWorkerService illustrates that accessibility is not just a UI-framework
concern. A REST API is accessible when its endpoints are fully documented. A CLI tool is
accessible when output remains readable without colour. A TUI is accessible when all actions
are confirmed as visible text.

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
Ersetzter Alt-Prompt: speckit-specify Nutze Lastenheft_A11Y_TUI_API.md als verbindliche Eingabedatei. Erstelle die Feature-Spezifikation fuer einen Barrierefreiheits- und Nutzbarkeitslauf im Repository InventarWorkerService.

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
$speckit-specify Use Lastenheft_A11Y_TUI_API.md as the binding intake. Preserve its scope, non-goals, ordering, governance, evidence, and acceptance criteria. Create or update only the matching feature specification. Do not implement, commit, push, create a pull request, merge, or start another feature.
```

### Autonomous

<!-- spec-kit-command-id: speckit.autonomous -->
```text
$speckit-autonomous Execute one complete autonomous Spec Kit run using Lastenheft_A11Y_TUI_API.md as the binding intake. Delivery mode: LocalImplementation. Preserve all scope, ordering, security, accessibility, evidence, and acceptance boundaries. Do not push, create or merge a pull request, use bypass authority, expose secrets, or start a follow-up feature.
```
<!-- intake-authoring:end -->
