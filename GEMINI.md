# GEMINI.md - Projektkontext für InventarWorkerService

Dieses Dokument dient als zentrale Orientierungshilfe für die Arbeit an diesem Repository. Es ergänzt die `README.md` und `CLAUDE.md`.

## 🚀 Projektübersicht
**InventarWorkerService** ist eine plattformübergreifende Inventarisierungslösung für IT-Infrastrukturen, entwickelt mit **.NET 10.0** und **C# 14.0**. Das System erfasst Hardware- und Software-Informationen von Windows-, macOS- und Linux-Systemen.

### Kernkomponenten:
1.  **InventarWorkerService**: Ein ASP.NET Core "Agent", der auf jedem zu überwachenden Rechner läuft. Er erfasst lokale Daten und stellt sie über eine REST-API bereit.
2.  **HarvesterWorkerService**: Der zentrale Dienst, der die Agenten abfragt und die Daten in **SQLite**, **MongoDB** oder **PostgreSQL** konsolidiert.
3.  **InventarViewerApp**: Eine interaktive Terminal-Benutzeroberfläche (TUI) basierend auf `Terminal.Gui` zur Anzeige und Verwaltung des Inventars.
4.  **InventarWorkerCommon**: Die zentrale Bibliothek mit Domänenmodellen, Datenbank-Services und API-Logik.
5.  **Steuerungstools**: Verschiedene Projekte (`CtrlWorker...`) zur Verwaltung der Dienste als Windows-Service oder via PowerShell.

## 🛠 Build & Run
Das Projekt nutzt die Standard .NET-CLI.

- **Gesamte Lösung bauen**: `dotnet build InventarWorkerService.sln`
- **Agent starten**: `dotnet run --project InventarWorkerService/InventarWorkerService.csproj`
- **Sammler starten**: `dotnet run --project HarvesterWorkerService/HarvesterWorkerService.csproj`
- **TUI-App starten**: `dotnet run --project InventarViewerApp/InventarViewerApp.csproj`
- **Coverage messen (CI-Grenze >=70%, Ziel >=80%)**: `dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults`
- **Veraltete NuGet-Pakete prüfen**: `dotnet list package --outdated`
- **Dokumentation neu erzeugen (bei API/XML-Doku-Änderungen)**: `docfx docfx.json`

## 🧪 Testing
- **Alle Tests**: `dotnet test`
- **Unit Tests**: `dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj`
- **Integration Tests**: Erfordern einen laufenden Agenten auf Port 5000.
  `dotnet test InventarWorkerServiceIntegrationTest/InventarWorkerServiceIntegrationTest.csproj`

## 🌿 Branching-Workflow (Verbindlich)
- Der Branch `main` ist geschützt und darf nicht direkt für Feature-Commits genutzt werden.
- Für jedes Feature/Fix muss ein neuer Branch erstellt werden.
- Änderungen gelangen ausschließlich per Pull Request nach `main` (inkl. Testnachweis).
- Wenn ein dedizierter Feature-Branch die Anforderungen eines Lastenhefts umgesetzt hat, wird die Datei in `Lastenheft_<Thema>.<feature-branch>.md` umbenannt, damit der gelieferte Umfang im Repository nachvollziehbar bleibt.
- Arbeits-Branches duerfen entweder die bestehende Themenbenennung oder die nummerierte Spec-Kit-Form `NNN-short-description` verwenden.
- `Directory.Build.props` fuehrt die repo-weiten Felder `Version`, `AssemblyVersion` und `FileVersion` als `Major.Minor.Patch.Build`; auf nummerierten Spec-Kit-Branches ist `Minor` die numerisch interpretierte Feature-/Branch-Nummer (`002` -> `2`), `Patch` die Commit-Anzahl im Feature-/PR-Branch nach dem aktuellen Commit und `Build` ein nur vor `dotnet build` oder `dotnet test` erhoehter Zaehler.

## 🏗 Architektur & Konventionen
- **Plattform-Support**: Implementiert als Windows Service, systemd (Linux) und launchd (macOS).
- **Datenhaltung**: 
  - Primär: **SQLite** (via Dapper).
  - Sekundär (Harvester): **MongoDB** & **PostgreSQL**.
- **API**: System.Text.Json (camelCase), RestSharp für Clients. Swagger/OpenAPI ist in Development unter `/swagger` verfügbar.
- **Sprache**: 
  - Erklärende Texte in Kommentaren/Dokumentation: zweisprachig (Deutsch zuerst, dann Englisch) auf CEFR-B2-Niveau.
  - UI-Labels & Logs: Deutsch.
- **Coding Style**:
  - Toolchain-Basis: `.NET 10` und `C# 14.0`.
  - Nullable Reference Types sind aktiviert.
  - Asynchrone Programmierung (`async/await`) ist Standard für I/O.
  - Test-Namensschema: `<UnitUnderTest>_<Scenario>_<ExpectedOutcome>`.
  - Testabdeckung in CI: mindestens 70%, Zielbereich ab 80%.
  - NuGet-Pakete auf aktuellem stabilen Stand halten; Ausnahmen dokumentieren.
  - XML-Dokumentation ist für öffentliche APIs verpflichtend (CS1591 nicht global unterdrücken).
  - Für nicht-öffentliche Member/Variablen sind an didaktisch relevanten Stellen zweisprachige Block- oder Zeilen-Kommentare zu nutzen.
  - Bei API- oder XML-Doku-Änderungen `docfx docfx.json` ausführen.
- **Worker-Intervalle**:
  - Debug: 30 Sekunden.
  - Release: 24 Stunden.

## 📁 Wichtige Verzeichnisse
- `InventarWorkerCommon/Models/`: Domänenmodelle (Hardware, Software, SQL, etc.).
- `InventarWorkerCommon/Services/`: Geschäftslogik für DB, API und Inventarisierung.
- `api/`: Enthält statische API-Dokumentation (DocFX).
- `docs/`: Zusätzliche Dokumentation und PDFs.

## 👤 Benutzer-Präferenzen (Thorsten)
- **IDE**: JetBrains Rider.
- **Shell**: Bevorzugt PowerShell (plattformübergreifend).
- **Datenbanken**: SQLite, PostgreSQL, MongoDB.
- **Interessen**: C64-Entwicklung (cc65, ACME), VST3-Plugins, Arbeitsrecht (Betriebsrat).
- **Kommunikation**: Deutsch (Du-Form).

## 📊 Projektstatistik

- Wenn sich gemeinsam genutzte KI-Agenten-Hinweise, Workflow-Konventionen oder die Statistikmethodik aendern, muessen `AGENTS.md`, `CLAUDE.md`, `GEMINI.md` und `.github/copilot-instructions.md` gemeinsam geprueft und bei Bedarf im selben Change aktualisiert werden.
- Gemeinsame Vorgaben duerfen nicht nur in einer dieser Dateien geaendert werden; beabsichtigte agentenspezifische Abweichungen sind im selben Change ausdruecklich zu dokumentieren.
- `docs/project-statistics.md` ist das fortlaufende Statistik-Register des Repositories.
- Die Datei muss nach jeder abgeschlossenen Spec-Kit-Implementierungsphase, nach jeder agentischen Änderung am Repository und auf explizite Anforderung aktualisiert werden.
- Im `## Fortschreibungsprotokoll` muessen die Tabelleneintraege strikt chronologisch stehen: der aelteste Eintrag oben, der juengste und zuletzt eingetragene Eintrag unten; Eintraege mit demselben Datum behalten ihre Eintragungsreihenfolge.
- Als letzter Top-Level-Block der Datei muss immer ein `## Gesamtstatistik`-Abschnitt stehen; danach darf kein weiterer Top-Level-Abschnitt folgen.
- Innerhalb dieses finalen `## Gesamtstatistik`-Abschnitts muessen kompakte ASCII-only-Diagramme direkt unter der textlichen Gesamtauswertung stehen, damit Artefaktmix, dokumentierte Branch-/Phasenverlaeufe, Beschleunigungsfaktoren und der Vergleich zwischen erfahrener Entwickler-Referenz, Thorsten-Solo-Referenz und sichtbarem KI-Lieferfenster in reinem Markdown lesbar bleiben.
- Jeder kurze CEFR-B2-Erklaertext muss direkt bei seiner ASCII-Diagrammgruppe stehen, und der Statistikblock muss fuer Braille-Zeile, Screenreader und Textbrowser textfreundlich bleiben, ohne auf Farbe oder Layout allein angewiesen zu sein.
- Jeder Eintrag muss Branch oder Phase, beobachtbares Arbeitsfenster, Produktions-, Test- und Doku-Zeilen, die wesentlichen Arbeitspakete, die konservative Handarbeits-Basis von 80 manuell erstellten Zeilen pro Arbeitstag ueber Code, Tests und Dokumentation hinweg sowie die repo-spezifische Thorsten-Solo-Vergleichsbasis von 100 Zeilen pro Arbeitstag fuer diese native .NET-Loesung enthalten.
- Wenn daraus Monatswerte abgeleitet werden, sind die Annahmen explizit zu nennen, zum Beispiel 21,5 Arbeitstage pro Monat sowie 30 Urlaubstage pro Jahr bis einschliesslich 2026 und 31 Urlaubstage pro Jahr ab 2027 in einer TVoeD-aehnlichen Kalenderannahme bei 5-Tage-Woche.
- Beschleunigungsangaben muessen beide Referenzen gegen sichtbare Git-Aktivtage stellen und ausdruecklich als repo-weiten Verdichtungsfaktor statt als Stoppuhrmessung kennzeichnen.
- Wenn Stundenwerte ausgewiesen werden, sind die Tageswerte mit der TVoeD-Arbeitszeit von `7,8 Stunden` bzw. `7 Stunden 48 Minuten` pro Arbeitstag umzurechnen.

## Inclusion & Accessibility

- Erklaerende Lern- und Governance-Dokumentation muss zweisprachig mit Deutsch zuerst und Englisch danach auf CEFR-B2-Niveau vorliegen.
- Grosse normative Dokumente wie `Pflichtenheft*.md` und `Lastenheft*.md` duerfen statt eines uebergrossen Inline-Zweisprachblocks als synchron gepflegte englische Parallelfassung mit Suffix `.EN.md` gefuehrt werden; die deutsche Fassung bleibt kanonisch, solange nichts anderes markiert ist.
*   Programmierung #include<everyone> — Diese Lernbeispiele richten sich an Azubis (Fachinformatiker AE/SI) mit Deutsch und Englisch als Arbeitssprachen sowie an sehbehinderte Lernende, die mit Braille-Displays, Screen-Readern oder Textbrowsern arbeiten. Barrierefreiheit ist kein Nice-to-have, sondern Pflichtanforderung.
- Treat WCAG 2.2 conformance level AA as the practical baseline for generated HTML documentation.
- If `docfx` output is regenerated, the same work item must also run a text-oriented accessibility review with Playwright + `@axe-core/playwright` and `lynx`.
- Recommended A11y toolchain for DocFX-based repos: Node 24 LTS, `npm`, Playwright, `@axe-core/playwright`, and `lynx`.

## Shared Parent Guidance

- Die gemeinsamen Dateien `/Users/thorstenhindermann/RiderProjects/AGENTS.md` und `/Users/thorstenhindermann/RiderProjects/GEMINI.md` speichern die repo-uebergreifenden Basisregeln.
- Diese Projekt-Datei ist die spezifischere Autoritaet fuer projektspezifische Build-Befehle, Workflows, Architektur und Features.
