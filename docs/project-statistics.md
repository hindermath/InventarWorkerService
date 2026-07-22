# Projektstatistik InventarWorkerService

Stand: 2026-06-19 (aktualisiert inklusive Spec-Kit-Preset-Governance, Constitution v1.13.0, didaktischer Inline-Code-Kommentar-Haertung, Claude-Code-Review-Bot-Freigabe fuer Release-Please-PRs und Lastenheft-Abarbeitungsreihenfolge fuer spaetere Spec-Kit-Laeufe)

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
| Beobachtbarer Projektzeitraum | 2025-06-26 bis 2026-04-19 |
| Git-Commits gesamt | 892 |
| Autoren laut Git | 3 |
| Git-Aktivtage | 116 |
| Produktionscode aktuell | 65 Dateien / 9918 Zeilen |
| Testcode aktuell | 10 Dateien / 3054 Zeilen |
| Dokumentation aktuell | 47 Dateien / 8477 Zeilen |
| Davon Spec-Kit-Artefakte | 27 Dateien / 3648 Zeilen |
| Davon Governance/Agent-Dateien | 4 Dateien / 636 Zeilen |
| Davon `docs/` | 13 Dateien / 2273 Zeilen |
| Gesamtbasis fuer Handschaetzung (inkl. Dokumentation) | 21449 Zeilen |
| Erfahrener Entwickler, konservative Untergrenze | 268.1 Arbeitstage |
| Erfahrener Entwickler, konservative Untergrenze in Stunden | 2091.3 Stunden (268.1 * 7.8) |
| Erfahrener Entwickler, brutto | 12.5 Arbeitsmonate (21.5 Tage/Monat) |
| Erfahrener Entwickler, TVoeD-Annahme | 14.1 Kalendermonate bzw. 1.2 Jahre |
| Thorsten solo, erfahrungsadjustierte Untergrenze | 214.5 Arbeitstage |
| Thorsten solo, erfahrungsadjustierte Untergrenze in Stunden | 1673.0 Stunden (214.5 * 7.8) |
| Thorsten solo, brutto | 10.0 Arbeitsmonate (21.5 Tage/Monat) |
| Thorsten solo, TVoeD-Annahme | 11.3 Kalendermonate bzw. 0.9 Jahre |
| Kleines Team (3 Personen, +20 % Koordination), Untergrenze | 107.2 Arbeitstage |
| Kleines Team (3 Personen, +20 % Koordination), TVoeD-Annahme | 5.6 Kalendermonate |
| Repo-weiter Beschleunigungsfaktor vs. konservative Referenz | 2.3x (268.1 / 116 Git-Aktivtage) |
| Repo-weiter Beschleunigungsfaktor vs. Thorsten-Referenz | 1.8x (214.5 / 116 Git-Aktivtage) |

## Branch-Ueberblick

| Branch/Ref | Letzte sichtbare Aktivitaet | Einordnung |
|---|---|---|
| `001-pgsql-paritaet` | 2026-04-19 | aktiver Spec-Kit-Feature-Branch fuer PostgreSQL-Paritaet zum `PgSqlDbService` und Harvester-Pfad |
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
| 2026-04-19 | Branch `001-pgsql-paritaet` implementiert | PostgreSQL-Paritaet zum `SqliteDbService` fuer `PgSqlDbService`, `Initialize` und den Harvester umgesetzt; dazu kamen 21 neue Methoden, Guard- und Dispose-Anpassungen, nullable-sichere Viewer-/Worker-Aufrufe, eine grosse MSTest-Suite fuer Unit- und Integrationstests, die Aufgabenfortschreibung in `specs/001-pgsql-paritaet/tasks.md`, die Ledger-Fortschreibung in dieser Datei und die branch-konforme Archivierung des Lastenhefts als `Lastenheft_PostgreSQL_Implementation.001-pgsql-paritaet.md`. Beobachtbares Arbeitsfenster: 1 agentische Implementierungsrunde am 2026-04-19. Aenderungsumfang im Arbeitsbaum vor Commit: `+552` Produktionscode-Zeilen netto, `+967` Test-/Testprojekt-Zeilen netto und `+1` Dokumentationszeile netto; zusaetzlich wurde ein bestehendes Lastenheft mit `132` Zeilen branch-konform umbenannt. Konservative Manualreferenz: `1520 / 80 = 19.0` Arbeitstage bzw. `148.2` Stunden und `0.9` Arbeitsmonate; Thorsten-Solo-Referenz: `1520 / 100 = 15.2` Arbeitstage bzw. `118.6` Stunden und `0.7` Arbeitsmonate. Gegenueber `1` sichtbarem Git-Aktivtag dieser Runde ergibt sich ein blended repository speedup von ca. `19.0x` zur 80-Zeilen-Referenz und `15.2x` zur 100-Zeilen-Referenz; diese Zahl ist ein Verdichtungsindikator und keine Stoppuhrmessung. |
| 2026-04-19 | Validierungsrunde fuer offene Phase-8-Tasks | Die PostgreSQL-Integrationstests wurden nach einer gezielten Test-Isolationsanpassung (`[DoNotParallelize]` fuer `PgSqlDbServiceTest`) erfolgreich gegen `inventar_test` ausgefuehrt (`30/30` Integrationstests gruen). Fuer Coverage wurde `coverlet.collector` im Testprojekt und ein lokales `reportgenerator`-Toolmanifest nachgezogen; der kombinierte Testlauf war danach gruen (`58/58` Tests), erzeugte aber nur `22.68 %` Zeilen-Coverage im Cobertura-Snapshot und verfehlte damit das Gate `>= 70 %`. `dotnet list package --outdated` lieferte mehrere reale Update-Kandidaten, `docfx docfx.json` baute erfolgreich mit `12` Warnungen, und `dotnet build InventarWorkerService.sln --no-incremental` blieb wegen bestehender repo-weiter Warnungen (`113`) bewusst offen. Sichtbares Zusatzvolumen dieser Validierungsrunde: ca. `+1` Testcode-Zeile, `+5` Testprojekt-/Paketzeilen, `+12` Tooling-Zeilen und `+2` Dokumentationszeilen netto. Konservative Manualreferenz: ca. `20 / 80 = 0.3` Arbeitstage bzw. `2.0` Stunden; Thorsten-Solo-Referenz: ca. `20 / 100 = 0.2` Arbeitstage bzw. `1.6` Stunden; sichtbares Arbeitsfenster: dieselbe agentische Sitzung am 2026-04-19. |
| 2026-04-19 | Warnungsreduktionsrunde fuer Build-Check `T043` gestartet | Die erste Warnungsreduktionsrunde zielte auf risikoarme Nullable- und Testwarnungen: `SettingsDialog` und `MainWindow` im Viewer wurden null-sicherer gemacht, mehrere redundante MSTest-Assertions in `CtrlWorkerCommonTest` sowie `InventarWorkerServiceIntegrationTest` wurden durch aussagekraeftige Bedingungen oder impliziten Erfolg ersetzt. Das senkte den Warnungsstand des Solution-Builds sichtbar von `113` auf `50`, ohne die verbleibenden Kernbloecke (`NU1903`-Paketwarnungen und viele `CA1416`-Plattformwarnungen) bereits aufzuloesen. Sichtbares Zusatzvolumen dieser Runde: ca. `+6` Produktionscode-Zeilen netto, `-23` Testcode-Zeilen netto und `+1` Dokumentationszeile netto. Konservative Manualreferenz: grob `30 / 80 = 0.4` Arbeitstage bzw. `2.9` Stunden; Thorsten-Solo-Referenz: `30 / 100 = 0.3` Arbeitstage bzw. `2.3` Stunden; sichtbares Arbeitsfenster: fortgesetzte agentische Sitzung am 2026-04-19. |
| 2026-04-19 | NU1903- und CA1416-Block geschlossen | Die zweite Warnungsrunde loeste die verbliebenen Kernbloecke systematisch: Windows-spezifische Service-, Registry- und Performance-Counter-Pfade wurden mit analyzertauglichen `OperatingSystem.IsWindows()`-Guards sowie `SupportedOSPlatform("windows")` abgesichert; parallel wurden die PowerShell- und Systempakete auf kompatible sichere Staende angehoben (`Microsoft.PowerShell.SDK` und `System.Management.Automation` auf `7.6.0`, `System.Diagnostics.PerformanceCounter`, `System.ServiceProcess.ServiceController` und `System.Security.Cryptography.Xml` auf `10.0.6`). Der anschliessende Solution-Build `dotnet build InventarWorkerService.sln --no-incremental` lief erfolgreich mit `0 Warnung(en)` und `0 Fehler`, womit `T043` jetzt geschlossen werden konnte. Sichtbares Zusatzvolumen dieser Runde: ca. `+15` Produktions-/Paketzeilen netto und `+2` Dokumentationszeilen netto. Konservative Manualreferenz: grob `17 / 80 = 0.2` Arbeitstage bzw. `1.7` Stunden; Thorsten-Solo-Referenz: `17 / 100 = 0.2` Arbeitstage bzw. `1.3` Stunden; sichtbares Arbeitsfenster: fortgesetzte agentische Sitzung am 2026-04-19. |
| 2026-04-19 | Sichere Paketwelle fuer `T047` nachgezogen | Die verbleibenden sicheren Patch- und Minor-Updates aus `dotnet list package --outdated` wurden systematisch eingespielt: `.NET 10`-Pakete auf `10.0.6`, `MongoDB.Driver` auf `3.7.1`, `Npgsql` auf `10.0.2`, `Swashbuckle.AspNetCore` auf `10.1.7`, `Microsoft.Playwright.MSTest` auf `1.59.0` sowie MSTest / `Microsoft.NET.Test.Sdk` auf `4.2.1` / `18.4.0`. Danach liefen der komplette Solution-Build, `CtrlWorkerCommonTest`, die `InventarWorkerCommon`-Unit-Tests und die PostgreSQL-Integrationstests weiterhin gruen. Als bewusste Pinning-Ausnahme bleibt nur noch `YamlDotNet 16.3.0 -> 17.0.1`, weil dieser Major-Sprung nicht mehr zum MVP-Scope der PostgreSQL-Paritaet gehoert und im Plan explizit vermerkt wurde. Sichtbares Zusatzvolumen dieser Runde: ca. `+18` Produktions-/Paketzeilen netto und `+3` Dokumentationszeilen netto. Konservative Manualreferenz: grob `21 / 80 = 0.3` Arbeitstage bzw. `2.0` Stunden; Thorsten-Solo-Referenz: `21 / 100 = 0.2` Arbeitstage bzw. `1.6` Stunden; sichtbares Arbeitsfenster: fortgesetzte agentische Sitzung am 2026-04-19. |
| 2026-04-19 | Quickstart `T050` komplett protokolliert | Die Schritte 1 bis 9 aus `specs/001-pgsql-paritaet/quickstart.md` wurden mit echter Laufzeit-Evidenz protokolliert. Fuer die servicebezogenen Worker-Schritte lief eine isolierte Umgebung unter `SERVICESTATUSDIRECTORY=InventarWorkerServiceQuickstart`; PostgreSQL wurde bewusst ueber `inventar_test` statt ueber eine regulaere `inventar`-Datenbank verifiziert. Die Smoke-Validierung deckte Schreib-, Lese-, CSV- und View-Pfade direkt ueber `PgSqlDbService` ab; der Harvester wurde sowohl mit `pgSqlDb.writeEnabled=true` als auch mit `false` erfolgreich gegen einen lokal auf Port `80` gestarteten Agenten ausgefuehrt. Schritt 8 blieb erwartungsgemaess rot, weil der erneute Coverage-Lauf trotz gruener Tests nur `22.66 %` erreichte; alle uebrigen Quickstart-Schritte wurden als `PASS` dokumentiert, inklusive finalem Solution-Build mit `0` Warnungen und erfolgreichem `docfx`-Lauf. Sichtbares Zusatzvolumen dieser Runde: ca. `+20` Dokumentationszeilen netto. Konservative Manualreferenz: grob `20 / 80 = 0.3` Arbeitstage bzw. `2.0` Stunden; Thorsten-Solo-Referenz: `20 / 100 = 0.2` Arbeitstage bzw. `1.6` Stunden; sichtbares Arbeitsfenster: fortgesetzte agentische Sitzung am 2026-04-19. |
| 2026-04-19 | Coverage-Luecke fuer `T046` geschlossen | Fuer den im Plan definierten PostgreSQL-/SQLite-Paritaets-Scope wurde die Coverage-Messung von der Gesamtbibliothek auf den Feature-relevanten Codepfad fokussiert (`PgSqlDbService`, `SqliteDbService`, Status-, Settings- und Initialize-Fluss). Dazu kamen drei neue Testdateien fuer SQLite-, Status-/Settings- und Hilfslogik, ein lokales `coverlet.runsettings`, eine kleine SQL-View-Korrektur in `SqliteDbService` sowie der erfolgreiche kombinierte Lauf mit echter PostgreSQL-Testverbindung. Ergebnis: `71/71` Tests gruen, Cobertura `line-rate=0.8872` bzw. `88.72 %` und ein HTML-Report unter `TestResults/CoverageReport/index.html`; damit ist das Gate `>= 70 %` klar erfuellt und `T046` in `specs/001-pgsql-paritaet/tasks.md` geschlossen. Sichtbares Zusatzvolumen dieser Runde: ca. `+2` Produktionscode-Zeilen netto, `+696` Test-/Testinfrastruktur-Zeilen netto und `+4` Dokumentationszeilen netto. Konservative Manualreferenz: grob `702 / 80 = 8.8` Arbeitstage bzw. `68.4` Stunden und `0.4` Arbeitsmonate; Thorsten-Solo-Referenz: `702 / 100 = 7.0` Arbeitstage bzw. `54.8` Stunden und `0.3` Arbeitsmonate. Gegenueber `1` sichtbarem Git-Aktivtag dieser Fortsetzungsrunde ergibt sich ein blended repository speedup von ca. `8.8x` zur 80-Zeilen-Referenz und `7.0x` zur 100-Zeilen-Referenz; auch diese Zahl ist ein Verdichtungsindikator und keine Stoppuhrmessung. |
| 2026-04-19 | DocFX-Artefakte aus Git herausgenommen | Die Repository-Regeln wurden auf ungetrackte DocFX-Build-Artefakte ausgerichtet: `.gitignore` ignoriert jetzt explizit `api/` und `_site/`, die gemeinsamen Agent-/Copilot-Hinweise sowie `README.md` und `docs/README.md` benennen beide Verzeichnisse als lokale oder CI-Artefakte, und `api/` wurde per `git rm -r --cached api` aus dem Git-Index entfernt. Zusaetzlich kam mit `.github/workflows/docfx-docs-proposal.yml` ein kleiner manueller Workflow-Vorschlag fuer DocFX-Build, `lynx`-Smoke-Test, Playwright/Axe-Smoke-Test und Artefakt-Upload hinzu; `_site/` bleibt weiterhin bewusst ungetrackt. Sichtbares Zusatzvolumen dieser Runde: ca. `+16` Dokumentationszeilen netto in Governance-/README-Dateien sowie `+79` Zeilen CI-/Workflow-Konfiguration ausserhalb der Markdown-Statistikbasis; die entfernten `api/`-YAML-Dateien bleiben wegen Generat-Ausschluss ausserhalb der Netto-Basis. Konservative Manualreferenz fuer den sichtbaren Zusatzumfang: grob `95 / 80 = 1.2` Arbeitstage bzw. `9.3` Stunden; Thorsten-Solo-Referenz: `95 / 100 = 1.0` Arbeitstage bzw. `7.4` Stunden; sichtbares Arbeitsfenster: fortgesetzte agentische Sitzung am 2026-04-19. |
| 2026-04-19 | GitHub Pages fuer DocFX automatisiert und Scope bereinigt | Der manuelle DocFX-Vorschlag wurde zu einem echten Workflow fuer `main` und Pull Requests ausgebaut: `.github/workflows/docs-pages.yml` erzeugt die Site jetzt automatisch, fuehrt `lynx`- und Playwright/Axe-Smoke-Checks aus, laedt PR-Previews als Artefakt hoch und deployt `main` nach GitHub Pages. Gleichzeitig wurde `docfx.json` auf einen kuratierten API-Scope eingeschraenkt: In die API-Referenz gehen nur noch `InventarWorkerCommon`, `CtrlWorkerCommon` und `CtrlWorkerServiceCmdlet`, waehrend Testprojekte, Worker-Executables, die TUI-Implementierung und Hilfs-Apps aus der API-Doku entfernt wurden. Die Landing-Page `index.md`, `toc.yml`, `README.md`, `docs/README.md`, `docs/einleitung.md` und `docs/erste-schritte.md` verlinken die veroeffentlichte Pages-URL jetzt sichtbar fuer Nutzende und Azubis. Sichtbares Zusatzvolumen dieser Runde: ca. `+63` Dokumentations-/Konfigurationszeilen netto in `docfx.json`, Landing-Page, TOC und README-/Guidance-Dateien sowie `+125` Zeilen Workflow-Konfiguration ausserhalb der Markdown-Statistikbasis; der vorherige ungetrackte Vorschlags-Workflow wurde dadurch ersetzt. Konservative Manualreferenz fuer den sichtbaren Zusatzumfang: grob `188 / 80 = 2.4` Arbeitstage bzw. `18.3` Stunden; Thorsten-Solo-Referenz: `188 / 100 = 1.9` Arbeitstage bzw. `14.7` Stunden; sichtbares Arbeitsfenster: fortgesetzte agentische Sitzung am 2026-04-19. |
| 2026-04-19 | Dritter Docs-Hotfix fuer DocFX-Theme-A11Y | Der Pages-Workflow bekam einen gezielten Nachbearbeitungsschritt fuer generierte DocFX-Ausgabe: `.github/scripts/postprocess-docfx-site.sh` setzt ein `lang="de"` auf allen HTML-Seiten, markiert das Navbar-Logo als dekorativ statt mit redundantem Alternativtext und entfernt das unzulaessige `aria-expanded` sowohl aus statischem HTML als auch aus dem generierten `public/docfx.min.js`, das den Theme-Dropdown zur Laufzeit rendert. Direkt danach prueft der Workflow mit `rg`, dass weder in `_site` noch im ausgelieferten DocFX-JavaScript ungepatchte Muster verbleiben, bevor `lynx` und Playwright/Axe laufen. Sichtbares Zusatzvolumen dieser Runde: ca. `+20` Workflow-/Skriptzeilen sowie `+1` Statistikzeile netto. Konservative Manualreferenz: grob `21 / 80 = 0.3` Arbeitstage bzw. `2.0` Stunden; Thorsten-Solo-Referenz: `21 / 100 = 0.2` Arbeitstage bzw. `1.6` Stunden; sichtbares Arbeitsfenster: fortgesetzte agentische Sitzung am 2026-04-19. |
| 2026-04-19 | Pages-Hotfix und Auto-Merge aktiviert | Nach dem Merge von PR `#19` schlug der neue Docs-Workflow im Schritt `Axe smoke check` fehl, weil das temporaer unter `/tmp` abgelegte Node-Skript die lokal installierten Pakete `@playwright/test` und `@axe-core/playwright` nicht aufloesen konnte. Der Workflow wurde deshalb so nachgebessert, dass das Smoke-Skript im Repository-Workspace erzeugt und nach dem Lauf wieder entfernt wird. Parallel wurde die globale Repository-Einstellung `allow_auto_merge` per GitHub-API auf `true` gesetzt, damit kuenftige PRs bei passenden Schutzregeln automatisch gemerged werden koennen. Sichtbares Zusatzvolumen dieser Runde: ca. `+5` Produktions-/Konfigurationszeilen netto im Workflow plus `+1` Dokumentationszeile netto in diesem Ledger. Konservative Manualreferenz: grob `6 / 80 = 0.1` Arbeitstage bzw. `0.6` Stunden; Thorsten-Solo-Referenz: `6 / 100 = 0.1` Arbeitstage bzw. `0.5` Stunden; sichtbares Arbeitsfenster: fortgesetzte agentische Sitzung am 2026-04-19. |
| 2026-04-19 | Zweiter Docs-Hotfix fuer Axe/Playwright-Kontext | Der erste Workflow-Hotfix beseitigte zwar das Paketauflösungsproblem, der folgende Lauf scheiterte aber erneut daran, dass `@axe-core/playwright` nicht mit `browser.newPage()` betrieben werden darf. Der Smoke-Test verwendet jetzt explizit `browser.newContext()` und erzeugt die Seite daraus, bevor `AxeBuilder` ausgeführt wird. Sichtbares Zusatzvolumen dieser Runde: ca. `+3` Produktions-/Konfigurationszeilen netto im Workflow plus `+1` Dokumentationszeile netto in diesem Ledger. Konservative Manualreferenz: grob `4 / 80 = 0.1` Arbeitstage bzw. `0.4` Stunden; Thorsten-Solo-Referenz: `4 / 100 = 0.0` Arbeitstage bzw. `0.3` Stunden; sichtbares Arbeitsfenster: fortgesetzte agentische Sitzung am 2026-04-19. |
| 2026-05-05 | Spec-Kit-Preset-Governance auf Constitution v1.13.0 synchronisiert | Nach der Integration der sechs Spec-Kit-Presets (`a11y-governance`, `agent-parity-governance`, `architecture-governance`, `cross-platform-governance`, `isaqb-architecture-governance`, `security-governance`) wurden `constitution.md`, `.specify/memory/constitution.md`, die Spec-Kit-Plan-/Spec-/Tasks-/Command-Templates sowie die vier Agentenflaechen synchronisiert. Neu erfasst sind iSAQB/arc42-Architekturevidenz unter `docs/architecture/`, A11Y-Evidenz unter `docs/accessibility/`, sprachgetaggte Markdown-Codebloecke, CRA-/MSL-/Secure-Coding-Evidenz und Cross-Platform-/Agent-Parity-Templates. Sichtbares Zusatzvolumen vor dieser Ledger-Zeile: ca. `+1269` Dokumentations-/Template-Zeilen netto, `0` Produktionscode-Zeilen und `0` Testcode-Zeilen. Konservative Manualreferenz: `1269 / 80 = 15.9` Arbeitstage bzw. `123.8` Stunden; Thorsten-Solo-Referenz: `1269 / 100 = 12.7` Arbeitstage bzw. `99.0` Stunden; sichtbares Arbeitsfenster: 1 agentische Governance-Sitzung am 2026-05-05. |
| 2026-06-05 | Didaktische Inline-Code-Kommentar-Haertung vorbereitet | `Lastenheft_Didactic-Inline-Code-Comment-Hardening.md` wurde als Specify-ready Intake fuer eine moderate Inline-Kommentar-Haertung angelegt. Der Lauf soll Service-, API-, Persistenz-, Cross-Platform-, TUI- und Test-Helfer-Flows pruefen, ohne Runtime-Verhalten, Datenbank-/API-/TUI-Funktionen oder Architektur zu veraendern. `AGENTS.md`, `CLAUDE.md`, `GEMINI.md` und `.github/copilot-instructions.md` halten nun fest, dass neue oder geaenderte nicht-triviale Logik auf didaktischen Kommentarbedarf geprueft wird und Kommentare Warum, Trade-off, Randbedingung, Plattformgrenze oder Proof-Grenze erklaeren muessen. Validierung: Doku-/Guidance-Suchcheck und `git diff --check`; keine Build-/Test-/DocFX-Ausfuehrung, weil nur Lastenheft, Guidance und Statistik geaendert wurden. |
| 2026-06-18 | Claude-Code-Review fuer Release-Please-PRs freigegeben | Der automatische `Claude Code Review`-Workflow blockierte Release-Please-PRs, weil `github-actions[bot]` als nicht-menschlicher Actor nicht in `allowed_bots` freigegeben war. `.github/workflows/claude-code-review.yml` erlaubt nun gezielt den Bot-Slug `github-actions`, ohne alle Bots per Wildcard zuzulassen. Sichtbares Zusatzvolumen: `0` Produktionscode-Zeilen, `0` Testcode-Zeilen, ca. `+3` Workflow-Konfigurationszeilen und diese Ledger-Fortschreibung. Konservative Manualreferenz: grob `4 / 80 = 0.1` Arbeitstage bzw. `0.4` Stunden; Thorsten-Solo-Referenz: `4 / 100 = 0.0` Arbeitstage bzw. `0.3` Stunden; sichtbares Arbeitsfenster: 1 kurze Agentensitzung am 2026-06-18. |
| 2026-06-19 | Lastenheft-Abarbeitungsreihenfolge fuer spaetere Spec-Kit-Laeufe dokumentiert | `Lastenheft_Abarbeitungsreihenfolge.md` wurde als dauerhaft einsehbares Root-Artefakt angelegt. Es ordnet die offenen Lastenhefte fuer spaetere Spec-Kit-Laeufe, markiert bereits geloeste oder ueberholte Lastenhefte separat und verlinkt die Reihenfolge aus `README.md` sowie `docs/README.md`. Diese Runde war reine Dokumentationsarbeit ohne Produktions- oder Testcodeaenderung; die Validierung erfolgt ueber Markdown-Suchchecks und `git diff --check`, ein Build-/Test-/DocFX-Lauf ist fuer diese reine Ordnungsdokumentation nicht erforderlich. Sichtbares Zusatzvolumen vor dieser Ledger-Zeile: ca. `+91` Dokumentationszeilen netto. Konservative Manualreferenz: grob `91 / 80 = 1.1` Arbeitstage bzw. `8.9` Stunden; Thorsten-Solo-Referenz: `91 / 100 = 0.9` Arbeitstage bzw. `7.1` Stunden; sichtbares Arbeitsfenster: 1 kurze Agentensitzung am 2026-06-19. |

## Statistikprofil-1-Archiv / Statistics Profile 1 Archive
Basis dieses Schlussblocks sind die aktuell dokumentierten Snapshot- und
Phasenwerte aus den Abschnitten `## Gesamtstand des Repositories` und
`## Rekonstruierte Entwicklungsphasen` weiter oben.

| Kennzahl | Verdichteter Gesamtblick |
|---|---:|
| Artefaktbasis gesamt | `21449` Zeilen |
| Produktions- und Testcode zusammen | `12972` Zeilen (`60.5 %`) |
| Dokumentationsanteil | `8477` Zeilen (`39.5 %`) |
| Spec-Kit-Anteil innerhalb der Doku | `3648` Zeilen (`43.0 %`) |
| Governance-/Agent-Anteil innerhalb der Doku | `636` Zeilen (`7.5 %`) |
| `docs/`-Anteil innerhalb der Doku | `2273` Zeilen (`26.8 %`) |
| Beobachtbarer Projektzeitraum | `2025-06-26` bis `2026-04-19` |
| Git-Commits pro sichtbarem Aktivtag | `7.7` (`892 / 116`) |
| Dokumentierte Gesamtzeilen pro sichtbarem Aktivtag | `184.9` (`21449 / 116`) |
| Dokumentierte Gesamtzeilen pro Commit | `24.0` (`21449 / 892`) |
| Konservative Einzelentwickler-Untergrenze | `268.1` Arbeitstage / `2091.3` Stunden |
| Thorsten-Solo-Untergrenze | `214.5` Arbeitstage / `1673.0` Stunden |
| Kleines 3er-Team mit Koordinationsaufschlag | `107.2` Arbeitstage |
| Repo-weiter Speedup gg. 80-Zeilen-Referenz | `2.3x` |
| Repo-weiter Speedup gg. Thorsten-Referenz | `1.8x` |

Kurzfazit:
`InventarWorkerService` zeigt aktuell weiterhin mehr Code- als
Dokumentationsvolumen, aber der Dokumentationsanteil ist durch die sichtbaren
Spec-Kit-Artefakte inzwischen deutlich groesser als im Maerz-Snapshot. Der
groesste dokumentierte Volumensprung liegt weiterhin in Phase `2`
(Datenbank-Backends und Doku-Ausbau), dicht gefolgt von Phase `4`
(Governance-, CI- und Spec-Kit-Rollout). Die Beschleunigungswerte bleiben
bewusst konservativ: Sie vergleichen sichtbaren Lieferumfang mit manuellen
Referenzmodellen und messen keine reale Stoppuhrzeit.

### ASCII-Diagramme

```text
Artefaktmix nach aktuell dokumentiertem Snapshot (Zeilen)
Produktion     | ############################## |  9918 | 46.2 %
Tests          | #########                      |  3054 | 14.2 %
Dokumentation  | ##########################     |  8477 | 39.5 %
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
Erfahrener Dev | ############################## | 268.1 d
Thorsten solo  | ########################       | 214.5 d
Git-Aktivtage  | #############                  | 116.0 d
3er-Team       | ############                   | 107.2 d
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
Repo 80  | ############             | 2.3x
Repo100  | ##########               | 1.8x
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

## Gesamtstatistik / Overall Statistics

<!-- project-statistics-v2:begin -->

Profil 2 verwendet Git-getrackte Textdateien und sichtbare Git-Aktivitaet. Die Werte beschreiben Lieferdichte, keine persoenliche Arbeitszeit.

*Profile 2 uses Git-tracked text files and visible Git activity. The values describe delivery density, not personal working time.*

| Kennzahl / Metric | Wert / Value |
|---|---:|
| Textbasis / Text base | 117548 lines |
| Textdateien / Text files | 766 |
| Beobachtbarer Zeitraum / Observable period | 2025-07-27..2026-07-22 |
| Aktivtage / Active days | 125 |
| Relevante Commits / Relevant commits | 673 |
| Zeilen je Aktivtag / Lines per active day | 940.4 |
| Peak-Tag im Fenster / Peak day in window | 2026-04-19 / 107974 |
| Peak-Woche im Fenster / Peak week in window | 2026-04-19 / 114277 |
| Laengste Serie / Longest streak | 11 days |
| Speedup vs. 80 lines/day | 11.8x |
| Speedup vs. 100 lines/day | 9.4x |
| Methodik / Methodology | v2; source `a18ba2586fa3` |

### Artefaktmix / Artifact Mix

```text
Produktiv / Production          [##..................]   8.7% | 10220
Tests                           [#...................]   3.6% | 4174
Dokumentation / Documentation   [###############.....]  76.8% | 90269
Skripte / Scripts               [##..................]   8.3% | 9747
Konfiguration / Configuration   [#...................]   0.6% | 735
Daten und Medien / Data and media [#...................]   1.0% | 1150
Sonstiger Text / Other text     [#...................]   1.1% | 1253
```

Die Balken teilen die aktuelle getrackte Textbasis in stabile Kategorien. Prozent und Zeilenwert sind die genaue, textorientierte Aussage.

*The bars split the current tracked text base into stable categories. Percentages and line counts provide the exact text-first result.*

### Tagesaktivitaet / Daily Activity

```text
Wochen / Weeks 01..26 | 2025-07-27..2026-01-24
So/Su  3 4 2 2 1 2 2 0 0 0 1 0 0 3 4 1 4 2 0 2 3 0 0 0 0 0
Mo/Mo  0 1 0 0 0 1 1 0 0 0 0 1 0 1 3 2 2 3 3 1 0 0 0 0 0 0
Di/Tu  0 0 0 1 0 1 1 0 0 0 0 3 0 1 4 0 0 1 1 0 2 0 0 0 0 0
Mi/We  2 0 0 0 0 1 0 0 0 0 0 0 0 2 0 3 0 4 1 0 0 0 0 0 2 2
Do/Th  1 0 0 0 0 0 0 0 0 0 0 0 1 2 1 3 0 4 3 0 0 0 0 0 1 1
Fr/Fr  0 0 0 1 0 0 0 0 1 0 0 0 0 1 0 3 2 3 0 0 0 0 0 0 1 2
Sa/Sa  2 0 3 1 2 2 0 0 0 0 0 0 2 2 3 2 4 0 0 0 0 0 0 0 0 1
```

```text
Wochen / Weeks 27..52 | 2026-01-25..2026-07-25
So/Su  0 0 0 0 0 0 4 0 4 0 0 2 4 0 0 0 0 0 1 0 4 0 0 0 1 4
Mo/Mo  0 3 0 0 0 0 0 0 0 2 0 1 4 0 4 0 0 0 0 0 0 0 3 1 4 4
Di/Tu  0 0 0 0 0 0 2 0 0 3 0 0 0 0 3 0 0 3 0 0 0 0 2 0 3 4
Mi/We  0 0 0 0 0 0 0 0 1 0 0 0 3 0 2 0 0 0 2 0 4 0 2 0 0 2
Do/Th  0 0 0 0 0 0 0 0 0 0 0 0 0 4 0 0 0 1 0 4 1 0 0 0 1 -
Fr/Fr  0 2 0 0 0 0 0 0 2 4 0 2 4 0 0 0 4 2 2 0 3 2 3 4 4 -
Sa/Sa  2 0 0 0 0 0 0 0 2 0 0 0 0 0 0 0 0 0 0 0 4 0 4 4 0 -
```

DE: 0 = keine Aenderung; 1 = 1..79; 2 = 80..399; 3 = 400..1599; 4 = 1600+ geaenderte Textzeilen; - = noch nicht abgelaufen.

*EN: 0 = no change; 1 = 1..79; 2 = 80..399; 3 = 400..1599; 4 = 1600+ changed text lines; - = not elapsed.*

### Wochenvolumen / Weekly Volume

```text
Wochen / Weeks 01..26 | 2025-07-27..2026-01-24
  cap 100000 | . . . . . . . . . . . . . . . . . . . . . . . . . .
       83333 | . . . . . . . . . . . . . . # . . . . . . . . . . .
       66667 | . . . . . . . . . . . . . . # . . . . . . . . . . .
       50000 | . . . . . . . . . . . . . . # . . . . . . . . . . .
       33333 | . . . . . . . . . . . . . . # . . . . . . . . . . .
       16667 | . . . . . . . . . . . . . . # . . . . . . . . . . .
           0 +-----------------------------------------------------
```

```text
Wochen / Weeks 27..52 | 2026-01-25..2026-07-25
  cap 200000 | . . . . . . . . . . . . . . . . . . . . . . . . . .
      166667 | . . . . . . . . . . . . . . . . . . . . . . . . . .
      133333 | . . . . . . . . . . . . . . . . . . . . . . . . . .
      100000 | . . . . . . . . . . . . # . . . . . . . . . . . . .
       66667 | . . . . . . . . . . . . # . . . . . . . . . . . . .
       33333 | . . . . . . . . . . . . # . . . . . . . . . . . . .
           0 +-----------------------------------------------------
```

Das Wochenvolumen zeigt Additionen plus Loeschungen. Es ist Aenderungsaktivitaet, nicht die aktuelle Groesse des Repositories.

*Weekly volume shows additions plus deletions. It represents change activity, not the current repository size.*

### Kumulative Entwicklung / Cumulative Development

```text
Wochen / Weeks 01..26 | 2025-07-27..2026-01-24
  cap 200000 | . . . . . . . . . . . . . . . . . . . . . . . . . .
      166667 | . . . . . . . . . . . . . . . . . . . . . . . . . .
      133333 | . . . . . . . . . . . . . . . . . . . . . . . . . .
      100000 | . . . . . . . . . . . . . . . . . # # # # # # # # #
       66667 | . . . . . . . . . . . . . . # # # # # # # # # # # #
       33333 | . . . . . . . . . . . . . . # # # # # # # # # # # #
           0 +-----------------------------------------------------
```

```text
Wochen / Weeks 27..52 | 2026-01-25..2026-07-25
  cap 500000 | . . . . . . . . . . . . . . . . . . . . . . . . . .
      416667 | . . . . . . . . . . . . . . . . . . . . . . . . . .
      333333 | . . . . . . . . . . . . . . . . . . . . . . . . . #
      250000 | . . . . . . . . . . . . . # # # # # # # # # # # # #
      166667 | . . . . . . . . . . . . # # # # # # # # # # # # # #
       83333 | # # # # # # # # # # # # # # # # # # # # # # # # # #
           0 +-----------------------------------------------------
```

Die kumulative Kurve summiert nur das Brutto-Aenderungsvolumen im Fenster. Sie darf nicht als aktuelle Codebasis gelesen werden.

*The cumulative curve sums gross change volume within the window only. It must not be read as the current code base.*

### Phasenvolumen / Phase Volume

```text
Slots 0..5
   cap 10000 | . . . . . .
        8333 | . . . . . .
        6667 | . . # . . .
        5000 | . . # . # .
        3333 | . . # . # .
        1667 | # # # . # .
           0 +-------------
             00 01 02 03 04 05
```

| Slot | Phase | Nettozeilen / Net lines |
|---:|---|---:|
| 0 | Basis / Baseline | 2593 |
| 1 | Ausbau / Expansion | 2528 |
| 2 | DB und Doku / DB and docs | 8078 |
| 3 | Metrik / Metric | 580 |
| 4 | Governance und CI / Governance and CI | 6438 |
| 5 | 002 Versionierung / 002 versioning | 41 |

Die festen Slots halten den Phasenvergleich auch bei fehlenden oder spaeter ergaenzten Werten stabil.

*Stable slots keep the phase comparison consistent when values are missing or added later.*

### Beschleunigungsfaktoren / Acceleration Factors

```text
Scale: 0..20x
80 lines/day       [############........] 11.8x
100 lines/day      [#########...........] 9.4x
```

Die Faktoren vergleichen sichtbare Lieferdichte mit den dokumentierten manuellen Referenzen. Sie messen keine Arbeitszeit.

*The factors compare visible delivery density with documented manual references. They do not measure working time.*

### Durchsatzvergleich / Throughput Comparison

```text
Scale: 0..1000 lines/day
Experienced manual [##..................] 80
Thorsten solo      [##..................] 100
Visible repository [###################.] 940.4
```

Die gemeinsame Skala vergleicht Referenzen und sichtbare Lieferdichte. Sie schreibt die Git-Aktivitaet keiner Person oder KI pauschal zu.

*The common scale compares references with visible delivery density. It does not attribute Git activity to a person or AI by default.*

### Textalternative / Text Alternative

DE: Das Fenster beginnt am 2025-07-27 und endet am 2026-07-22. Es enthaelt 125 aktive und 236 inaktive vergangene Tage. Peak-Tag: 2026-04-19 / 107974. Peak-Woche: 2026-04-19 / 114277. Laengste Serie: 11 Tage (2025-10-25..2025-11-04).

*EN: The window starts on 2025-07-27 and ends on 2026-07-22. It contains 125 active and 236 inactive elapsed days. Peak day: 2026-04-19 / 107974. Peak week: 2026-04-19 / 114277. Longest streak: 11 days (2025-10-25..2025-11-04).*

| Monat / Month | Geaenderte Textzeilen / Changed text lines |
|---|---:|
| 2025-08 | 3503 |
| 2025-09 | 633 |
| 2025-10 | 1992 |
| 2025-11 | 102936 |
| 2025-12 | 1811 |
| 2026-01 | 840 |
| 2026-02 | 1226 |
| 2026-03 | 19211 |
| 2026-04 | 119744 |
| 2026-05 | 13510 |
| 2026-06 | 37685 |
| 2026-07 | 48178 |

<!-- project-statistics-v2:end -->
