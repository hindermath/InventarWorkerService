# Projektstatistik InventarWorkerService

Stand: 2026-03-30 (aktualisiert inklusive finalem Gesamtstatistik-Block, ASCII-Diagrammen und Governance-Abgleich fuer Bilingualitaet, A11Y und Parent-Baseline)

## Zweck und Pflege

Diese Datei ist das fortlaufende Statistik-Register fuer
`InventarWorkerService`. Sie wird nach jeder abgeschlossenen
Spec-Kit-Implementierungsphase, nach jeder agentischen Aenderung am Repository
und auf explizite Anforderung fortgeschrieben.

## Methodik

- Quellen: Git-Historie, sichtbare Branch-Referenzen und aktueller Dateistand.
- Ausgeschlossen: `.codex/`, `_site/`, `api/`, `bin/`, `obj/`, `TestResults/`
  sowie sonstige generierte Artefakte.
- Produktionscode: produktive `*.cs`-Dateien ausserhalb der Testprojekte.
- Testcode: `CtrlWorkerCommonTest/**/*.cs`,
  `InventarWorkerCommonTest/**/*.cs`,
  `InventarWorkerServiceIntegrationTest/**/*.cs`.
- Dokumentation: Markdown-Dateien in Repository-Wurzel, `docs/`, `.github/`,
  `.specify/` und projektbezogenen Unterordnern.
- Leitsatz fuer diese Datei und die lernrelevanten Dokumente:
  `Programmierung #include<everyone>`. Inhalte muessen fuer Braille-Zeile,
  Screenreader und Textbrowser lesbar bleiben; die ASCII-Diagramme sind
  deshalb bewusst textuell und nicht farbgetragen aufgebaut.
- Fuer erzeugte HTML-Dokumentation gilt WCAG 2.2 Konformitaetsstufe AA als
  konkrete Pruefbasis; besonders wichtig sind Seitensprache,
  Bypass-Mechanismen, Tastaturfokus, Non-Text-Contrast und semantische
  Landmarken.
- Die konservative Handarbeits-Basis in dieser Datei zaehlt Produktionscode,
  Testcode und Dokumentation gemeinsam als manuell zu erstellenden Umfang.
- Die konservative Handarbeits-Basis folgt dem Beitrag
  [Adapt or Disappear: How AI Turned a 2-Year Project into a 1-Week Sprint](https://www.holgerscode.com/blog/2026/02/23/adapt-or-disappear-how-ai-turned-a-2-year-project-into-a-1-week-sprint/#a-note-on-the-orm-29000-lines-you-never-have-to-write):
  maximal 80 manuell erstellte Zeilen pro Arbeitstag fuer einen erfahrenen
  Entwickler.
- Umrechnung in Zeitraeume:
  durchschnittlich 21.5 Arbeitstage pro Monat (Mittel aus 21-22 Arbeitstagen);
  unter TVoeD-Annahme mit 30 Urlaubstagen pro Jahr bis einschliesslich 2026 und
  31 Urlaubstagen pro Jahr ab 2027 (jeweils 5-Tage-Woche) ergeben sich
  `21.5 * 12 - 30 = 228` produktive Arbeitstage pro Jahr fuer Zeitraeume bis
  2026 bzw. `21.5 * 12 - 31 = 227` produktive Arbeitstage pro Jahr ab 2027.
- TVoeD-Stundenbasis in dieser Datei:
  `7.8 Stunden` bzw. `7 Stunden 48 Minuten` pro Arbeitstag fuer zusaetzliche
  Stundenumrechnungen.
- Abgeleitete Formeln in dieser Datei:
  Einzelentwickler `((Produktionscode + Testcode + Dokumentation) / 80)`;
  3er-Team `Einzelentwickler / 3 * 1.2` mit 20 % Koordinationsaufschlag.
- Zusatzannahmen fuer die erfahrungsadjustierte Thorsten-Referenz:
  - Allgemeiner Expertenaufschlag `* 1.25`, weil Thorsten seit Februar 1985
    mehr als 40 Jahre Softwareentwicklungspraxis einbringt und seit 2001 mit
    .NET/C# arbeitet.
  - Kein zusaetzlicher Legacy-Portierungsaufschlag, weil
    `InventarWorkerService` kein Pascal-/Turbo-Vision-Port ist, sondern eine
    native .NET-Loesung.
  - Daraus ergibt sich fuer `InventarWorkerService` eine
    erfahrungsadjustierte Solo-Referenz von `80 * 1.25 = 100` manuell
    erstellten Zeilen pro Arbeitstag.
- Beschleunigungsfaktoren vergleichen Referenz-Arbeitstage mit sichtbaren
  `Git-Aktivtagen`. Sie sind als repo-weiter Output-zu-Aktivtag-Indikator
  formuliert und keine exakte Zeiterfassung.

## Erfahrungsprofil und Beschleunigungsmodell

- Referenzprofil fuer die erfahrungsadjustierte Zweitrechnung:
  - mehr als 40 Jahre Softwareentwicklung seit Februar 1985
  - langjaehrige .NET-/C#-Praxis seit 2001
  - hier ohne zusaetzlichen Pascal-/Turbo-Vision-Domainaufschlag
- Neben der konservativen 80-Zeilen-Referenz fuehrt das Repository daher eine
  zweite Thorsten-Solo-Referenz mit `100 Zeilen/Arbeitstag`.
- Die Beschleunigungsfaktoren beantworten die Frage, wie stark der sichtbare
  Lieferumfang gegenueber einer klassischen, manuell dominierten .NET-
  Entwicklung verdichtet wurde.

## Gesamtstand des Repositories

| Kennzahl | Wert |
|---|---:|
| Beobachtbarer Projektzeitraum | 2025-06-26 bis 2026-03-08 |
| Git-Commits gesamt | 837 |
| Autoren laut Git | 1 |
| Git-Aktivtage | 102 |
| Produktionscode aktuell | 65 Dateien / 9395 Zeilen |
| Testcode aktuell | 9 Dateien / 2094 Zeilen |
| Dokumentation aktuell | 54 Dateien / 6033 Zeilen |
| Davon Spec-Kit-Artefakte | 0 Dateien / 0 Zeilen |
| Davon Governance/Agent-Dateien | 5 Dateien / 617 Zeilen |
| Davon `docs/` | 13 Dateien / 1994 Zeilen |
| Gesamtbasis fuer Handschaetzung (inkl. Dokumentation) | 17522 Zeilen |
| Erfahrener Entwickler, konservative Untergrenze | 219.0 Arbeitstage |
| Erfahrener Entwickler, konservative Untergrenze in Stunden | 1708.2 Stunden (219.0 * 7.8) |
| Erfahrener Entwickler, brutto | 10.2 Arbeitsmonate (21.5 Tage/Monat) |
| Erfahrener Entwickler, TVoeD-Annahme | 11.5 Kalendermonate bzw. 1.0 Jahre |
| Thorsten solo, erfahrungsadjustierte Untergrenze | 175.2 Arbeitstage |
| Thorsten solo, erfahrungsadjustierte Untergrenze in Stunden | 1366.6 Stunden (175.2 * 7.8) |
| Thorsten solo, brutto | 8.1 Arbeitsmonate (21.5 Tage/Monat) |
| Thorsten solo, TVoeD-Annahme | 9.2 Kalendermonate bzw. 0.8 Jahre |
| Kleines Team (3 Personen, +20 % Koordination), Untergrenze | 87.6 Arbeitstage |
| Kleines Team (3 Personen, +20 % Koordination), TVoeD-Annahme | 4.6 Kalendermonate |
| Repo-weiter Beschleunigungsfaktor vs. konservative Referenz | 2.1x (219.0 / 102 Git-Aktivtage) |
| Repo-weiter Beschleunigungsfaktor vs. Thorsten-Referenz | 1.7x (175.2 / 102 Git-Aktivtage) |

## Branch-Ueberblick

| Branch/Ref | Letzte sichtbare Aktivitaet | Einordnung |
|---|---|---|
| `002-spec-kit-versioning` | 2026-03-27 | Arbeitsbranch fuer repo-weite Versionslogik auf Basis nummerierter Spec-Kit-Branches |
| `main` | 2026-03-08 | Integrationsbranch |
| `INV-7` | sichtbar lokal | lokaler Arbeitsbranch |
| `chore/constitution-2.1-compliance` | sichtbar lokal | lokaler Governance-Branch |
| `origin/INV-4`, `origin/INV-5`, `origin/INV-7`, `origin/INV-8`, `origin/INV-9` | sichtbar remote | historische Feature-Branches |
| `origin/constitution-governance-update` | 2026-03-08 | Constitution-Erweiterung |
| `origin/commands-templates-followup` | 2026-03-08 | Spec-Kit-Template-Nacharbeit |
| `origin/ci-workflow-setup` | 2026-03-08 | CI-/Coverage-Workflow |
| `origin/fix/ctrlworker-exception-tests` | 2026-03-08 | Test-/Exception-Fix |
| `origin/chore/constitution-2.1-compliance` | 2026-03-08 | Constitution-2.1-Compliance |

## Rekonstruierte Entwicklungsphasen

### 0. Agent- und Monitoring-Basis

- Status: abgeschlossen und in `main` enthalten
- Beobachtbarer Zeitraum: 2025-06-26 bis 2025-06-28
- Commit-Bild: 67 Commits an 2 Git-Aktivtagen
- Grundlegende Arbeiten: `InventarWorkerService`, Service-Status-Modelle,
  Datei-basiertes Monitoring, Hardware-/Software-Inventarisierung, REST-API und
  Swagger-Grundlage
- Git-Aenderungsvolumen netto:
  - Produktionscode: 2242
  - Testcode: 0
  - Dokumentation: 351
- Konservative Handarbeits-Basis fuer Code und Dokumentation:
  - 2593 Zeilen netto gesamt
  - 32.4 Arbeitstage fuer einen erfahrenen Entwickler
  - 1.5 Arbeitsmonate brutto bzw. 1.7 TVoeD-Kalendermonate
  - 13.0 Arbeitstage fuer ein 3er-Team (+20 % Koordination), entsprechend ca.
    0.7 TVoeD-Kalendermonaten

### 1. Viewer-, Steuerungs- und Portabilitaets-Ausbau

- Status: abgeschlossen und in `main` enthalten
- Beobachtbarer Zeitraum: 2025-06-29 bis 2025-07-15
- Commit-Bild: 139 Commits an 15 Git-Aktivtagen
- Grundlegende Arbeiten: `InventarViewerApp`, SQLite-Persistenz,
  `CtrlWorkerService*`, TUI-Ansichten, Service-Steuerung und
  plattformuebergreifende Setup-/README-Ausweitung
- Git-Aenderungsvolumen netto:
  - Produktionscode: 1893
  - Testcode: 0
  - Dokumentation: 635
- Konservative Handarbeits-Basis fuer Code und Dokumentation:
  - 2528 Zeilen netto gesamt
  - 31.6 Arbeitstage fuer einen erfahrenen Entwickler
  - 1.5 Arbeitsmonate brutto bzw. 1.7 TVoeD-Kalendermonate
  - 12.6 Arbeitstage fuer ein 3er-Team (+20 % Koordination), entsprechend ca.
    0.7 TVoeD-Kalendermonaten

### 2. Datenbank-Backends und Doku-Ausbau

- Status: abgeschlossen und in `main` enthalten
- Beobachtbarer Zeitraum: 2025-07-16 bis 2025-12-16
- Commit-Bild: 567 Commits an 74 Git-Aktivtagen
- Grundlegende Arbeiten: MongoDB- und PostgreSQL-Integration,
  Settings-/Konfigurationsmodell, Schema-/View-Erweiterungen, DocFX-/YAML-Metadaten,
  UI-Ausbau und begleitende Tests
- Git-Aenderungsvolumen netto:
  - Produktionscode: 4583
  - Testcode: 1305
  - Dokumentation: 2190
- Konservative Handarbeits-Basis fuer Code und Dokumentation:
  - 8078 Zeilen netto gesamt
  - 101.0 Arbeitstage fuer einen erfahrenen Entwickler
  - 4.7 Arbeitsmonate brutto bzw. 5.3 TVoeD-Kalendermonate
  - 40.4 Arbeitstage fuer ein 3er-Team (+20 % Koordination), entsprechend ca.
    2.1 TVoeD-Kalendermonaten

### 3. Plattformmetriken und Hardware-Verfeinerung

- Status: abgeschlossen und in `main` enthalten
- Beobachtbarer Zeitraum: 2026-01-14 bis 2026-02-06
- Commit-Bild: 39 Commits an 10 Git-Aktivtagen
- Grundlegende Arbeiten: CPU-/Speicher-Metriken fuer Windows/macOS/Unix,
  PowerShell-Fallbacks, Immutability-Anpassungen und zusaetzliche Modelltests
- Git-Aenderungsvolumen netto:
  - Produktionscode: 521
  - Testcode: 59
  - Dokumentation: 0
- Konservative Handarbeits-Basis fuer Code und Dokumentation:
  - 580 Zeilen netto gesamt
  - 7.2 Arbeitstage fuer einen erfahrenen Entwickler
  - 0.3 Arbeitsmonate brutto bzw. 0.4 TVoeD-Kalendermonate
  - 2.9 Arbeitstage fuer ein 3er-Team (+20 % Koordination), entsprechend ca.
    0.2 TVoeD-Kalendermonaten

### 4. Governance-, CI- und Spec-Kit-Rollout

- Status: abgeschlossen und in `main` enthalten
- Beobachtbarer Zeitraum: 2026-03-08 bis 2026-03-08
- Commit-Bild: 25 Commits an 1 Git-Aktivtag
- Grundlegende Arbeiten: Agent-Dateien, Constitution, Spec-Kit-Templates,
  Coverage-/CI-Workflow, Command-Templates und Constitution-2.1-Compliance
- Git-Aenderungsvolumen netto:
  - Produktionscode: 134
  - Testcode: 730
  - Dokumentation: 5574
- Konservative Handarbeits-Basis fuer Code und Dokumentation:
  - 6438 Zeilen netto gesamt
  - 80.5 Arbeitstage fuer einen erfahrenen Entwickler
  - 3.7 Arbeitsmonate brutto bzw. 4.2 TVoeD-Kalendermonate
  - 32.2 Arbeitstage fuer ein 3er-Team (+20 % Koordination), entsprechend ca.
    1.7 TVoeD-Kalendermonaten

### 5. Branch `002-spec-kit-versioning`

- Status: in Arbeit auf Feature-Branch `002-spec-kit-versioning`
- Beobachtbarer Zeitraum: 2026-03-27 bis 2026-03-27
- Commit-Bild: aktueller Working-Tree-Aenderungssatz vor dem ersten Branch-Commit
- Grundlegende Arbeiten: nummerierte Spec-Kit-Branches als zulaessige
  Arbeitsform ergaenzt, repo-weite Versionslogik in `Directory.Build.props`
  eingefuehrt und die gemeinsame Agent-/Constitution-Governance darauf
  synchronisiert
- Git-/Arbeitsbaum-Aenderungsvolumen fuer den aktuellen Aenderungssatz:
  - Produktionscode: 0 Zeilen
  - Testcode: 0 Zeilen
  - Dokumentation und Governance: 38 Zeilen netto
  - Build-/Versionsmetadaten: 3 Zeilen in `Directory.Build.props`
- Konservative Handarbeits-Basis fuer Code und Dokumentation:
  - 41 Zeilen netto gesamt
  - 0.5 Arbeitstage fuer einen erfahrenen Entwickler
  - 3.9 Stunden auf TVoeD-Basis (`0.5 * 7.8`)
  - 0.0 Arbeitsmonate brutto bzw. 0.0 TVoeD-Kalendermonate
- Thorsten-Solo-Referenz:
  - 0.4 Arbeitstage
  - 3.1 Stunden auf TVoeD-Basis (`0.4 * 7.8`)
  - 0.0 Arbeitsmonate brutto bzw. 0.0 TVoeD-Kalendermonate
- Blended Repository Speedup gegen sichtbare 1 Git-Aktivtag fuer diesen
  Aenderungssatz:
  - 0.5x gegen die konservative 80-Zeilen-Referenz
  - 0.4x gegen die Thorsten-Solo-Referenz mit 100 Zeilen pro Arbeitstag

## Einordnung der KI-/Spec-Kit-Wirkung

- Die beobachtbare manuelle Gesamtbasis liegt bereits bei 17522 Zeilen
  (Produktionscode + Tests + Dokumentation).
- Selbst mit der konservativen Obergrenze von 80 manuell erstellten Zeilen pro
  Arbeitstag ergibt sich bereits eine Untergrenze von 219.0
  Entwickler-Arbeitstagen.
- Unter TVoeD-Annahme mit 30 Urlaubstagen pro Jahr entspricht das fuer einen
  erfahrenen Entwickler ca. 11.5 Kalendermonaten bzw. 1.0 Arbeitsjahren; fuer
  ein 3er-Team mit 20 % Koordinationsaufschlag ca. 4.6 Kalendermonaten.
- Unter Einbezug von Thorstens Erfahrungsprofil sinkt die klassische
  Solo-Referenz fuer `InventarWorkerService` auf ca. 175.2 Arbeitstage bzw.
  9.2 TVoeD-Kalendermonate.
- Gegen die sichtbaren 102 Git-Aktivtage ergibt sich damit ein repo-weiter
  Beschleunigungsfaktor von ca. 2.1x gegen die konservative Referenz und
  ca. 1.7x gegen die erfahrungsadjustierte Thorsten-Referenz.
- Die Historie zeigt einen langen manuellen Aufbau mit spaeter Verdichtung durch
  Governance-/Spec-Kit-Rollout. Die Statistik macht damit sowohl die klassische
  Entwicklungsmasse als auch den spaeteren agentischen Strukturgewinn sichtbar.

## Fortschreibungsprotokoll

| Datum | Ausloeser | Eintrag |
|---|---|---|
| 2026-03-21 | Erstanlage | Basisstatistik fuer `InventarWorkerService` angelegt; Entwicklungsphasen aus der Git-Historie rekonstruiert und Constitution, Templates sowie Agent-Dateien auf Pflegepflicht synchronisiert. |
| 2026-03-22 | Methodik-Update fuer Handarbeits-Schaetzung | Die Statistik rechnet Handarbeit jetzt auf Basis von Produktionscode, Testcode und Dokumentation gemeinsam; zusaetzlich werden Monatswerte auf Basis von 21.5 Arbeitstagen pro Monat sowie TVoeD-Kalenderwerte mit 30 Urlaubstagen pro Jahr ausgewiesen. |
| 2026-03-22 | Governance-Synchronisierung zur Statistiklogik | Constitution sowie die gemeinsamen Agent-Hinweise (`AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md`) wurden auf die neue Statistiklogik synchronisiert: Handarbeits-Schaetzung umfasst nun Code, Tests und Dokumentation gemeinsam; Monats- und TVoeD-Annahmen muessen explizit genannt werden. |
| 2026-03-22 | GitHub-Codex-Spec-Kit-Skills installiert | Die lokale Codex-Skill-Struktur `.agents/skills/` mit den neun `speckit-*`-Skills wurde aus TuiVision in InventarWorkerService uebernommen, damit die Spec-Kit-Kommandos auch in diesem Repository direkt als Skills verfuegbar sind. |
| 2026-03-25 | Erfahrungsadjustierte Beschleunigungsrechnung erweitert | Die Statistik fuehrt jetzt zusaetzlich zur konservativen 80-Zeilen-Referenz eine explizite Thorsten-Solo-Referenz ohne Pascal-Legacy-Aufschlag; dieselbe Methodik wurde in `AGENTS.md`, `CLAUDE.md`, `GEMINI.md` und `.github/copilot-instructions.md` synchronisiert. |
| 2026-03-25 | TVoeD-Stundenbasis ergänzt | Die Statistik weist zusaetzlich Stundenwerte auf Basis von `7,8 Stunden` bzw. `7 Stunden 48 Minuten` pro Arbeitstag aus; dieselbe Umrechnungsregel wurde in die gemeinsamen Agent-Dateien aufgenommen. |
| 2026-03-27 | TVoeD-Urlaubsregel ab 2027 nachgezogen | Die Statistik- und Agentenmethodik wurde auf die neue Stichtagsregel umgestellt: 30 Urlaubstage pro Jahr gelten nur bis einschliesslich 2026, ab dem Kalenderjahr 2027 werden unter TVoeD-Annahme 31 Urlaubstage bei unveraenderter 5-Tage-Woche verwendet. |
| 2026-03-27 | Branch `002-spec-kit-versioning` | Repo-weite Versionslogik fuer nummerierte Spec-Kit-Branches eingefuehrt: `Directory.Build.props` traegt jetzt `Version`, `AssemblyVersion` und `FileVersion`; die gemeinsame Agent-Governance und die Constitution wurden auf `Minor = Spec-Kit-Feature-/Branch-Nummer als kanonische PR-Nummer` synchronisiert. |
| 2026-03-27 | Sortierung des Fortschreibungsprotokolls vereinheitlicht | Die Eintraege im Fortschreibungsprotokoll wurden auf strikt chronologische Reihenfolge gebracht: aeltester Eintrag oben, juengster und zuletzt eingetragener Eintrag unten. Dieselbe Regel wurde in der gemeinsamen Agent-Governance fuer dieses Repository festgeschrieben. |
| 2026-03-28 | Lastenheft-Branch-Suffix-Regel in Agent-Guidance verankert | Die gemeinsamen Agent-Dateien (`AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md`) wurden um die Governance-Regel erweitert, dass ein Lastenheft nach Umsetzung durch einen dedizierten Feature-Branch auf `Lastenheft_<Thema>.<feature-branch>.md` umzubenennen ist, damit die Rueckverfolgbarkeit im Repository erhalten bleibt; Aenderungsumfang dieser Runde vor dieser Ledger-Fortschreibung: 0 Produktionscode-Zeilen, 0 Testcode-Zeilen, +4 Dokumentationszeilen netto im Arbeitsbaum, konservative Handarbeits-Untergrenze 0.1 Arbeitstage bzw. 0.4 Stunden auf TVoeD-Basis, Monatsannahme weiterhin 21.5 Arbeitstage pro Monat. |
| 2026-03-30 | Inklusions-Leitsatz und DocFX-A11y-Baseline verankert | `README.md`, `docs/README.md`, `AGENTS.md`, `CLAUDE.md`, `GEMINI.md` und `.github/copilot-instructions.md` tragen jetzt den Leitsatz `Programmierung #include<everyone>`. Damit ist fuer dieses Repository festgelegt, dass Guides und erzeugte HTML-/API-Dokumentation fuer Braille-Zeile, Screenreader und Textbrowser lesbar bleiben muessen. Fuer DocFX-basierte HTML-Dokumentation gilt WCAG 2.2 AA als praktische Baseline; nach jedem DocFX-Neubau soll ein textorientierter A11y-Review mit Playwright/axe und `lynx` folgen. Diese Runde war reine Governance-/Doku-Arbeit mit `0` Produktionscode-Zeilen, `0` Testcode-Zeilen und ca. `+48` Dokumentationszeilen netto. Konservative Manualreferenz: 80 Zeilen/Tag = `0.6` Tage (ca. `4.7` Stunden); Thorsten-Solo-Referenz: 100 Zeilen/Tag = `0.5` Tage (ca. `3.7` Stunden); sichtbares Arbeitsfenster: 1 kurze Agentensitzung am 2026-03-30. |
| 2026-03-30 | Bilinguale Abschlusspruefung und A11Y-Gate in zentraler Doku verankert | Da fuer `InventarWorkerService` aktuell kein separates Pflichtenheft im Repository vorliegt, wurden die formalen Abschlusspruefpunkte in `docs/README.md` verankert: Lernrelevante Dokumente muessen in Deutsch und Englisch auf CEFR-B2-Niveau vorliegen; grosse normative Dokumente duerfen als synchron gepflegte `.EN.md`-Parallelfassung ausgeliefert werden; fuer DocFX-basierte HTML-Dokumentation gilt die A11Y-Pflicht nach `Programmierung #include<everyone>` mit WCAG 2.2 AA, textorientiertem Review nach `docfx` sowie Nutzbarkeit fuer Braille-Zeile, Screenreader und Textbrowser. Diese Runde war reine Dokumentationsarbeit mit `0` Produktionscode-Zeilen, `0` Testcode-Zeilen und ca. `+8` Dokumentationszeilen netto. Konservative Manualreferenz: 80 Zeilen/Tag = `0.1` Tage (ca. `0.8` Stunden); Thorsten-Solo-Referenz: 100 Zeilen/Tag = `0.1` Tage (ca. `0.6` Stunden); sichtbares Arbeitsfenster: 1 kurze Agentensitzung am 2026-03-30. |
| 2026-03-30 | Parent-Guidance bewusst auf repo-uebergreifende Regeln begrenzt | In den lokalen Guidance-Dateien von `InventarWorkerService` ist jetzt ausdruecklich vermerkt, dass `/Users/thorstenhindermann/RiderProjects/AGENTS.md` nur gemeinsame Basisregeln fuer mehrere Repositories traegt. Repository-spezifische Build-, Test-, Workflow-, Architektur- und Feature-Vorgaben bleiben bewusst in `InventarWorkerService` selbst und sind dort die spezifischere Autoritaet. Diese Runde war reine Dokumentationsarbeit mit `0` Produktionscode-Zeilen, `0` Testcode-Zeilen und ca. `+10` Dokumentationszeilen netto. Konservative Manualreferenz: 80 Zeilen/Tag = `0.1` Tage (ca. `1.0` Stunden); Thorsten-Solo-Referenz: 100 Zeilen/Tag = `0.1` Tage (ca. `0.8` Stunden); sichtbares Arbeitsfenster: 1 kurze Agentensitzung am 2026-03-30. |
| 2026-03-30 | Gemeinsame Governance- und Statistikregeln mit TuiVision abgeglichen | `README.md`, `docs/README.md`, `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md` und diese Statistikdatei wurden nur auf die gemeinsamen Soll-Punkte nachgezogen: CEFR-B2 mit Deutsch zuerst/Englisch danach, `.EN.md` als zulaessige Parallelfassung fuer grosse normative Dokumente, `Programmierung #include<everyone>`, WCAG 2.2 AA, verpflichtender `docfx`-Folgecheck mit Playwright/axe plus `lynx` sowie ein finaler `## Gesamtstatistik`-Block mit ASCII-Diagrammen als letzter Abschnitt. Diese Runde war reine Dokumentationsarbeit mit `0` Produktionscode-Zeilen, `0` Testcode-Zeilen und `+265 / -1` Dokumentationszeilen netto. Konservative Manualreferenz: 80 Zeilen/Tag = `3.3` Tage (ca. `25.8` Stunden); Thorsten-Solo-Referenz: 100 Zeilen/Tag = `2.6` Tage (ca. `20.6` Stunden); sichtbares Arbeitsfenster: 1 kurze Agentensitzung am 2026-03-30. |

## Gesamtstatistik

Basis dieses Schlussblocks sind die aktuell dokumentierten Snapshot- und
Phasenwerte aus den Abschnitten `## Gesamtstand des Repositories` und
`## Rekonstruierte Entwicklungsphasen` weiter oben.

| Kennzahl | Verdichteter Gesamtblick |
|---|---:|
| Artefaktbasis gesamt | `17522` Zeilen |
| Produktions- und Testcode zusammen | `11489` Zeilen (`65.6 %`) |
| Dokumentationsanteil | `6033` Zeilen (`34.4 %`) |
| Spec-Kit-Anteil innerhalb der Doku | `0` Zeilen (`0.0 %`) |
| Governance-/Agent-Anteil innerhalb der Doku | `617` Zeilen (`10.2 %`) |
| `docs/`-Anteil innerhalb der Doku | `1994` Zeilen (`33.1 %`) |
| Beobachtbarer Projektzeitraum | `2025-06-26` bis `2026-03-08` |
| Git-Commits pro sichtbarem Aktivtag | `8.2` (`837 / 102`) |
| Dokumentierte Gesamtzeilen pro sichtbarem Aktivtag | `171.8` (`17522 / 102`) |
| Dokumentierte Gesamtzeilen pro Commit | `20.9` (`17522 / 837`) |
| Konservative Einzelentwickler-Untergrenze | `219.0` Arbeitstage / `1708.2` Stunden |
| Thorsten-Solo-Untergrenze | `175.2` Arbeitstage / `1366.6` Stunden |
| Kleines 3er-Team mit Koordinationsaufschlag | `87.6` Arbeitstage |
| Repo-weiter Speedup gg. 80-Zeilen-Referenz | `2.1x` |
| Repo-weiter Speedup gg. Thorsten-Referenz | `1.7x` |

Kurzfazit:
`InventarWorkerService` zeigt aktuell deutlich mehr Code- als
Dokumentationsvolumen, hat aber gleichzeitig eine sichtbare Governance- und
Dokuschicht, die fuer den Betrieb und die Nachvollziehbarkeit wichtig ist. Der
groesste dokumentierte Volumensprung liegt in Phase `2` (Datenbank-Backends
und Doku-Ausbau), dicht gefolgt von Phase `4` (Governance-, CI- und Spec-Kit-
Rollout). Die Beschleunigungswerte bleiben bewusst konservativ: Sie vergleichen
sichtbaren Lieferumfang mit manuellen Referenzmodellen und messen keine reale
Stoppuhrzeit.

### ASCII-Diagramme

```text
Artefaktmix nach aktuell dokumentiertem Snapshot (Zeilen)
Produktion     | ############################## |  9395 | 53.6 %
Tests          | #######                        |  2094 | 12.0 %
Dokumentation  | ###################            |  6033 | 34.4 %
```

Dieses Diagramm zeigt die aktuelle Verteilung zwischen Produktionscode,
Testcode und Dokumentation. Laengere Balken bedeuten mehr sichtbaren Umfang in
derselben Vergleichsgruppe. So wird schnell lesbar, dass die Codebasis im
Moment groesser ist als die Dokumentation, die Doku aber weiterhin einen
substanziellen Anteil am Repository hat.

This chart shows the current distribution between production code, test code,
and documentation. Longer bars mean more visible scope inside the same
comparison group. It makes clear that the code base is currently larger than
the documentation, while documentation still remains a substantial part of the
repository.

```text
Phasenvolumen nach dokumentierter Netto-Basis (Zeilen)
0 Basis   | ##########               | 2593
1 Ausbau  | ##########               | 2528
2 DB+Doku | ######################## | 8078
3 Metrik  | #                        |  580
4 Gov+CI  | ###################      | 6438
5 002-ver | #                        |   41
```

Dieses Diagramm zeigt die grob sichtbare Netto-Basis der rekonstruierten
Phasen. Es beantwortet die Frage, welche Phase den groessten dokumentierten
Umfang erzeugt hat. Besonders sichtbar sind hier der Datenbank- und Doku-Ausbau
in Phase `2` sowie der spaete Governance- und CI-Rollout in Phase `4`.

This chart shows the rough visible net size of the reconstructed phases. It
answers the question which phase created the largest documented scope. The
database and documentation expansion in phase `2` and the later governance and
CI rollout in phase `4` stand out most clearly.

```text
X/Y-Skizze: dokumentierte Netto-Basis je Phase
Netto-Zeilen
9000 |             *
8000 |
7000 |                         *
6000 |
5000 |
4000 |
3000 | *     *
2000 |
1000 |                   *
   0 +--------------------------------
       0     1     2     3     4    5
```

Die X/Y-Skizze zeigt denselben Phasenverlauf noch einmal als einfachen Trend
ueber die Phasenachse. Sie ist bewusst grob gehalten. Wichtig ist hier nicht
Millimetergenauigkeit, sondern dass der starke Ausschlag bei Phase `2`, der
zweite hohe Ausschlag bei Phase `4` und die kleine Restgroesse der aktuellen
Versionierungsrunde schnell sichtbar werden.

The X/Y sketch shows the same phase progression again as a simple trend over
the phase axis. It is intentionally rough. The goal is not millimetre accuracy,
but quick visibility of the strong peak at phase `2`, the second high peak at
phase `4`, and the very small remaining size of the current versioning round.

```text
Vergleich Referenzaufwand vs. sichtbare Git-Aktivtage
Erfahrener Dev | ############################## | 219.0 d
Thorsten solo  | ########################       | 175.2 d
Git-Aktivtage  | ##############                 | 102.0 d
3er-Team       | ############                   |  87.6 d
```

Dieses Diagramm vergleicht die manuellen Referenzmodelle direkt mit den
sichtbaren Git-Aktivtagen. Damit wird lesbar, wie gross die Luecke zwischen
klassischer Handarbeit und dem dokumentierten Lieferfenster ist. Der Vergleich
ist ein repo-weiter Verdichtungsindikator und keine Aussage ueber echte
Arbeitsstunden.

This chart compares the manual reference models directly with the visible Git
active days. It makes the gap between classical manual delivery and the
documented delivery window easier to read. The comparison is a repository-wide
compression indicator, not a statement about real worked hours.

```text
Dokumentierte Beschleunigungsfaktoren
Repo 80  | ###########              | 2.1x
Repo100  | #########                | 1.7x
Phase 2  | #############            | 1.4x
Phase 4  | ######################## | 80.5x
Phase 5  | #####                    | 0.5x
```

Dieses Diagramm zeigt die dokumentierten Beschleunigungsfaktoren gegen die
verfuegbaren Referenzen und markante Einzelphasen. Die sehr hohe Zahl in Phase
`4` kommt daher, dass ein grosser Governance- und Doku-Block an nur einem
sichtbaren Git-Aktivtag dokumentiert ist. Die kleine Zahl in Phase `5` zeigt
umgekehrt, dass die aktuelle Versionierungsrunde bewusst klein gehalten wurde.

This chart shows the documented acceleration factors against the available
references and selected phases. The very high number in phase `4` appears
because a large governance and documentation package is documented on only one
visible Git active day. The small number in phase `5` shows the opposite case:
the current versioning round was intentionally kept small.
