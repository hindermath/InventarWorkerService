# Copilot Instructions

## Build & Test

```bash
# Build
dotnet build InventarWorkerService.sln

# Run all tests
dotnet test

# Unit tests only
dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj
dotnet test CtrlWorkerCommonTest/CtrlWorkerCommonTest.csproj

# Single test method
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"

# Integration tests (requires InventarWorkerService running on http://localhost:5000 first)
dotnet run --project InventarWorkerService/InventarWorkerService.csproj
dotnet test InventarWorkerServiceIntegrationTest/InventarWorkerServiceIntegrationTest.csproj

# Collect coverage (CI gate >=70%, target >=80%)
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Check package currency
dotnet list package --outdated

# Regenerate documentation when API/XML docs change
docfx docfx.json
```

The generated DocFX directories `api/` and `_site/` are build artifacts. Keep them out of Git. Published documentation is deployed automatically from `main` via `.github/workflows/docs-pages.yml`.

## Branching Workflow (Mandatory)

- `main` is protected and must not receive direct feature commits.
- Create a new branch for each feature/fix/change.
- Merge to `main` only through pull requests with test evidence.
- When a dedicated feature branch has implemented the requirements of a Lastenheft, rename that file to `Lastenheft_<topic>.<feature-branch>.md` so the delivered scope stays traceable.
- Work branches may use either the existing topic naming or the numbered Spec-Kit form `NNN-short-description`.

`Directory.Build.props` carries the repo-wide `Version`, `AssemblyVersion`, and `FileVersion` values for all projects using `Major.Minor.Patch.Build`:
- `Minor` = current Spec-Kit feature/branch number, interpreted numerically as the canonical PR number for versioning (`002` -> `2`) and used immediately even before a GitHub PR exists
- `Patch` = current commit count in that feature/PR branch (after committing the current change)
- `Build` = manual build counter incremented before every `dotnet build` or `dotnet test`

On numbered Spec-Kit branches, align those three version fields before pushing.

## Architecture

This is a .NET 10 / C# 14.0 multi-project solution for cross-platform IT hardware/software inventory.

**Data flow:**
```
Each machine runs InventarWorkerService (REST agent)
    ↑ queried by HarvesterWorkerService (central collector → SQLite/MongoDB/PostgreSQL)
    ↑ also queried by InventarViewerApp (Terminal.Gui TUI → local SQLite)
```

**Projects:**
- `InventarWorkerService` — ASP.NET Core Worker + REST API on each monitored machine. Endpoints: `GET /api/inventar/hardware|software|full|status`. Swagger at `/swagger` in Development.
- `HarvesterWorkerService` — Central collector; reads machine list from SQLite, calls each agent, writes inventory to SQLite/MongoDB/PostgreSQL.
- `InventarViewerApp` — Terminal.Gui TUI; calls agent API via RestSharp (`ApiService`), persists locally via Dapper+SQLite (`DatabaseService`).
- `InventarWorkerCommon` — Shared domain library. New shared models go here; new services register via DI in the consuming project's `Program.cs`.
- `CtrlWorkerCommon/App/Cmdlet/PS` — Windows Service control utilities and PowerShell cmdlets.

**Worker loop timing:** `30_000ms` in `#if DEBUG`, `86_400_000ms` (24h) in Release.

**Service deployment:** Runs as Windows Service (`AddWindowsService`), systemd (`AddSystemd`), or launchd.

## Key Conventions

**Language split:** Explanatory texts in comments/docs MUST be bilingual (German first, then English, CEFR B2). UI labels and log messages remain German.

**Naming:**
- Types/methods/properties/constants: PascalCase
- Local variables, parameters, private fields: camelCase (private fields prefixed `_`)
- Test methods: `<UnitUnderTest>_<Scenario>_<ExpectedOutcome>`
- DB table/column names: PascalCase

**Nullable reference types** are enabled everywhere — use `string?` for optional values.

**Toolchain:** Use `.NET 10` with `C# 14.0`.

**Async:** All I/O-bound public service methods return `Task` or `Task<T>`.

**Coverage:** CI coverage must be >=70% and must target >=80%.

**Serialization:** `System.Text.Json` with camelCase naming policy throughout. Do not use Newtonsoft.Json. All existing usage of `Newtonsoft.Json` MUST be migrated to `System.Text.Json` and the `Newtonsoft.Json` NuGet package MUST be removed from all projects and the solution.

**Dependencies:** Keep NuGet packages on latest stable versions; document any pinning exceptions.

**HTTP client:** RestSharp in `InventarViewerApp`; integration tests use Playwright's `APIRequestContext`.

**Data access:** Dapper + `Microsoft.Data.Sqlite`. SQL is written as explicit raw strings with `IF NOT EXISTS` guards, PascalCase identifiers, and indices on frequently queried columns.

**`ServiceStatusWriter`** writes three output types per service: status (JSON), statistics (JSON), log (text). Identified by a service name prefix — default `""` for the agent, `"harvester-service"` for the harvester.

**CSV import:** CsvHelper with explicit class maps (see `InventarWorkerCommon/Services/Csv`).

**Error handling:** Catch at boundary layers (API controllers return `StatusCode(500, new { error = ... })`; TUI shows `MessageBox.ErrorQuery`). Use `using` statements on DB connections.

**XML doc comments** are mandatory on all public API members; CS1591 must not be globally suppressed.

**Didactic comments:** Add bilingual block/line comments for non-public members/variables where XML docs do not apply.

**DocFX sync:** Run `docfx docfx.json` whenever API signatures or XML documentation changes. The generated `api/` metadata and `_site/` HTML output must remain untracked. GitHub Pages deployment is handled by `.github/workflows/docs-pages.yml`.

**Test framework:** MSTest. Use `[TestInitialize]`/`[TestCleanup]` for per-test setup. Assert default property values (empty strings, 0, false, null) explicitly in unit tests.

## Where to Put New Code

| What | Where |
|---|---|
| Shared domain models | `InventarWorkerCommon/Models/` |
| New services (shared) | `InventarWorkerCommon/Services/` + register in `Program.cs` |
| New API endpoints | `InventarWorkerService/Controllers/` + add integration test |
| New DB tables/views | Extend `SqliteDbService`; add indices; use `IF NOT EXISTS` |
| App-specific persistence/services | Under the app's own `Services/` folder |

## Project Statistics

- When shared AI-agent guidance, workflow conventions, or statistics methodology changes, review and update `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, and `.github/copilot-instructions.md` together when they are affected.
- Shared guidance must not be updated in only one of these files; any intentional agent-specific divergence must be documented in the same change.
- Maintain `docs/project-statistics.md` as the living statistics ledger for the repository.
- Update the file after each completed Spec-Kit implementation phase, after each agent-driven repository change, or when a refresh is explicitly requested.
- Within the `## Fortschreibungsprotokoll` table, keep entries in strict chronological order: oldest entry at the top, newest and most recently added entry at the bottom; entries with the same date keep their insertion order.
- Keep a final top-level `## Gesamtstatistik` block as the last section of `docs/project-statistics.md`; do not append any later top-level section after it.
- Inside that final `## Gesamtstatistik` block, keep compact ASCII-only diagrams directly below the textual overall summary so the overall artifact mix, documented branch/phase curves, acceleration factors, and the comparison between experienced-developer effort, Thorsten-solo effort, and visible AI-assisted delivery stay readable in plain Markdown.
- Keep each short CEFR-B2 explanation directly adjacent to its matching ASCII diagram group, and keep the statistics block text-first so Braille displays, screen readers, and text browsers do not depend on color or layout alone.
- Each update must capture branch or phase, observable work window, production/test/documentation line counts, main work packages, the conservative manual baseline of 80 manually created lines per workday across code, tests, and documentation, and the repo-specific Thorsten-Solo comparison baseline of 100 lines per workday for this native .NET codebase.
- When effort is converted into months, use explicit assumptions such as 21.5 workdays per month and, if applicable, 30 vacation days per year under a TVoeD-style calendar.
- When reporting acceleration, compare both manual references against visible Git active days and label the result as a blended repository speedup rather than a stopwatch measurement.
- When hour values are shown, convert the day-based estimates with the TVoeD working-day baseline of `7.8 hours` (`7h 48m`) per day.

## Inclusion & Accessibility

- Explanatory documentation for learner-facing and governance content MUST be bilingual with German first and English second at CEFR-B2 readability.
- Large normative documents such as `Pflichtenheft*.md` and `Lastenheft*.md` may use a synchronized English sidecar with suffix `.EN.md` instead of one oversized inline-bilingual file; the German version remains canonical unless explicitly marked otherwise.
- Follow `Programmierung #include<everyone>`: Diese Lernbeispiele richten sich an Azubis (Fachinformatiker AE/SI), die auf Deutsch und Englisch arbeiten, **sowie** an sehbehinderte Lernende, die Braille-Displays, Screen-Reader oder Textbrowser nutzen. Barrierefreiheit ist Pflichtanforderung, kein Nice-to-have. / *These learning examples target apprentices working in German and English, **and** visually impaired learners using Braille displays, screen readers, or text browsers. Accessibility is mandatory.*
- Treat WCAG 2.2 conformance level AA as the practical baseline for generated HTML documentation.
- If `docfx` output is regenerated, the same work item must also run a text-oriented accessibility review with Playwright + `@axe-core/playwright` and `lynx`.
- Recommended A11y toolchain for DocFX-based repos: Node 24 LTS, `npm`, Playwright, `@axe-core/playwright`, and `lynx`.

## Workspace Baseline (vollständig aus `RiderProjects/.github/copilot-instructions.md`)

Diese Regeln gelten für alle Repositories in diesem Workspace. Projektspezifische Regeln in dieser Datei haben Vorrang, wenn sie konkreter sind. GitHub Copilot liest keine übergeordneten `copilot-instructions.md`-Dateien automatisch; daher sind die Workspace-Regeln hier vollständig eingebettet.

### Dokumentation
- Leitprinzip: `Programmierung #include<everyone>` — Diese Lernbeispiele richten sich an Azubis (Fachinformatiker AE/SI), die auf Deutsch und Englisch arbeiten, **sowie** an sehbehinderte Lernende, die Braille-Displays, Screen-Reader oder Textbrowser nutzen. Barrierefreiheit ist Pflichtanforderung, kein Nice-to-have. / *These learning examples target apprentices working in German and English, **and** visually impaired learners using Braille displays, screen readers, or text browsers. Accessibility is mandatory.*
- Deutsch und Englisch zielen beide auf CEFR-B2-Lesbarkeit; Reihenfolge: **Deutsch zuerst, Englisch danach**.
- **Die deutsche Fassung ist kanonisch**, außer dieses Repository markiert eine andere Sprache explizit als primär.
- Große normative Dokumente (`Pflichtenheft*.md`, `Lastenheft*.md`) verwenden eine synchronisierte `.EN.md`-Sidecar-Datei statt einer überlangen Inline-Zweisprachigkeit.
- Bilinguales CEFR-B2-Deliverable ist ein **formales Abnahmekriterium** für learner-facing Dokumentation und aktive Anforderungsartefakte.

### Barrierefreiheit (Accessibility)
- Generiertes HTML-Dokumentation muss **WCAG 2.2 Level AA** erfüllen.
- Semantische Überschriften, Listen, Tabellen und ASCII/Text-First-Diagramme bevorzugen.
- **Wesentliche Bedeutung NICHT nur durch Farbe, Layout oder Maus-only-Affordances kodieren.**
- Guides, Statistiken, Beispiele und generierte API-Dokumentation müssen in text-first Assistive-Setups lesbar bleiben.
- Der dokumentierte **A11Y-Nachweispfad ist ein formales Abnahmekriterium** für learner-facing Dokumentation und aktive Anforderungsartefakte.

### DocFX-Review-Regel
- Wenn ein Repository Dokumentation mit `docfx` neu generiert, muss **dasselbe Work-Item** auch den passenden A11Y-Review ausführen.
- Bevorzugtes Toolchain: **Node 24 LTS**, **`npm`**, **`@axe-core/playwright`**, **`lynx`**.
- Playwright + axe für automatisierte Smoke-Checks verwenden; `lynx` als zusätzlichen Textbrowser-Prüfpfad.

### Statistik-Ledger
- `docs/project-statistics.md` als lebendes Ledger pflegen, wenn diese Datei im Repository existiert.
- Den abschließenden Top-Level-Block `## Gesamtstatistik` als letzten Abschnitt halten.
- ASCII-Diagramme textbrowserfreundlich halten und **kurze CEFR-B2-Erklärungen direkt neben** das jeweilige Diagramm platzieren.
- Dokumentierte **Beschleunigungsfaktoren aus Agentic AI plus Spec-Kit/SDD** einschließen sowie einen Vergleich zwischen experienced-developer-Aufwand, Thorsten-solo-Aufwand und dem sichtbaren AI-assisted-Delivery-Fenster (sofern dieses Repository diese Metriken führt).

### Änderungsdisziplin
- **Nicht davon ausgehen**, dass eine Cross-Repository-Regel projekt-spezifische Build-, Test- oder Release-Anforderungen ersetzt.
- Wenn eine gemeinsame Regel sich ändert und mehrere Repositories betroffen sind, lokale Projektguidance **und** das jeweilige Statistik-Ledger gemeinsam aktualisieren.
- `CODEX_CROSS_REPO_PROMPTS.md` synchron halten, wenn sich übergreifende Prompting-Guidance ändert, damit der wiederverwendbare Prompt mit der aktuellen Baseline übereinstimmt.

<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan
<!-- SPECKIT END -->

## Gemeinsame Governance-Ergaenzung / Shared Governance Addendum

- Alle nutzerseitigen Artefakte muessen barrierefrei gedacht und geprueft werden: CLI-Ausgaben, Dokumentation, HTML, UI und generierte Templates; WCAG 2.2 Level AA ist die Standard-Basis, sobald die Kriterien auf das Artefakt anwendbar sind.
- All user-facing artefacts must be designed and reviewed for accessibility: CLI output, documentation, HTML, UI, and generated templates; WCAG 2.2 Level AA is the default baseline wherever the criteria apply.

- Fuer C#/.NET-Repositories gilt standardmaessig eine Thorsten-Solo-Basis von `125` Zeilen/Arbeitstag, sofern das Repo keinen abweichenden, begruendeten Wert dokumentiert.
- The default Thorsten-solo baseline for C#/.NET repositories is `125` lines/workday unless the repository documents a justified deviation.
