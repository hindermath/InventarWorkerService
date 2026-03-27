# Projektstatistik InventarWorkerService

Stand: 2026-03-27

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
