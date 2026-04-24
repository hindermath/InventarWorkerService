# Repository Guidelines

## Project Structure & Module Organization
This repository is a multi-project .NET 10 / C# 14.0 solution (`InventarWorkerService.sln`). Core domain logic lives in `InventarWorkerCommon/` (`Models/`, `Services/`, `Helpers/`). Runtime services are in `InventarWorkerService/` (agent + API), `HarvesterWorkerService/` (collector), and `InventarViewerApp/` (Terminal UI client). Service control utilities are in `CtrlWorkerCommon/`, `CtrlWorkerServiceApp/`, `CtrlWorkerServiceCmdlet/`, and `CtrlWorkerServicePS/`.

Tests are split by scope: `InventarWorkerCommonTest/`, `CtrlWorkerCommonTest/`, and `InventarWorkerServiceIntegrationTest/`. Documentation sources are under `docs/` with DocFX config in `docfx.json`; generated DocFX metadata in `api/` and generated HTML output in `_site/` are local or CI artifacts and MUST remain untracked.

## Build, Test, and Development Commands
- `dotnet restore InventarWorkerService.sln`: restore dependencies for all projects.
- `dotnet build InventarWorkerService.sln`: compile the full solution.
- `dotnet run --project InventarWorkerService/InventarWorkerService.csproj`: run local API/worker service.
- `dotnet run --project HarvesterWorkerService/HarvesterWorkerService.csproj`: run central harvester.
- `dotnet run --project InventarViewerApp/InventarViewerApp.csproj`: start TUI viewer.
- `dotnet test`: execute all unit and integration tests.
- `dotnet test InventarWorkerServiceIntegrationTest/InventarWorkerServiceIntegrationTest.csproj`: run integration tests only.
- `dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"`: run a single test method.
- `dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults`: collect CI-ready coverage artifacts.
- `dotnet list package --outdated`: identify packages not on latest stable versions.
- `pwsh InventarWorkerServiceIntegrationTest/bin/Debug/net10.0/playwright.ps1 install`: install Playwright browsers after first build.
- `docfx docfx.json`: build API and Markdown documentation.

## Coding Style & Naming Conventions
Use C# with 4-space indentation and nullable reference types enabled. Follow existing naming patterns: `PascalCase` for types/methods/properties, `camelCase` for locals/parameters, and `_camelCase` for private fields. Keep platform-specific behavior isolated in service/controller layers; put reusable logic in `InventarWorkerCommon`.

- Imports: Use standard .NET namespaces first, then project-specific namespaces
- Formatting: Follow Microsoft's C# formatting guidelines with 4-space indentation
- Types: Use `string?` for optional reference types, `int?` for optional value types
- Naming: PascalCase for types/methods/properties/constants; camelCase for locals/parameters/_camelCase for private fields
- Error handling: Catch at boundary layers (API controllers return `StatusCode(500, new { error = ... })`; TUI shows `MessageBox.ErrorQuery`). Use `using` statements on DB connections.
- XML doc comments: Mandatory for all public API types and members (`<summary>`, `<param>`, `<returns>`, `<exception>` as applicable)
- Didactic comments: Use bilingual block/line comments for non-public members or variables where XML docs do not apply
- Async: All I/O-bound public service methods return `Task` or `Task<T>`
- Documentation language: Explanatory text MUST be bilingual (German block first, English block second) at CEFR B2 readability

## Testing Guidelines
Tests use MSTest (`[TestClass]`, `[TestMethod]`). Prefer descriptive test names such as `<UnitUnderTest>_<Scenario>_<ExpectedOutcome>`. Keep unit tests deterministic and independent of machine state. Integration tests require `InventarWorkerService` running at `http://localhost:5000`; remote tests may be network-dependent. Coverage in CI MUST stay at least 70% and MUST target 80% or more.

## Commit & Pull Request Guidelines
Recent history follows imperative subjects (for example: `Add ...`, `Update ...`, `Refine ...`). Continue with short, present-tense commit titles and narrow scope per commit.

`main` is protected: create a new branch for every feature/fix and merge changes through a pull request targeting `main`.
- When a dedicated feature branch has implemented the requirements of a Lastenheft, rename that file to `Lastenheft_<Thema>.<feature-branch>.md` so the delivered requirement scope stays traceable in the repository.
Branches may use either the existing topic form or the numbered Spec-Kit form `NNN-short-description` when the Spec-Kit workflow creates the branch.

## Build Versioning

- Repo-wide assembly version fields live in `Directory.Build.props` and MUST keep `Version`, `AssemblyVersion`, and `FileVersion` aligned for all projects.
- The scheme is `Major.Minor.Patch.Build`.
- `Minor` = current Spec-Kit feature/branch number, interpreted numerically as the canonical PR number for versioning (`002` -> `2`) and used immediately even before a GitHub PR exists.
- `Patch` = current commit count in that feature/PR branch after committing the current change.
- `Build` = manual build counter incremented by the bot before every `dotnet build` or `dotnet test`.
- Before any commit or push on a numbered Spec-Kit branch, the repo-wide version fields in `Directory.Build.props` MUST be aligned to this scheme.

PRs should include: purpose, touched projects, test evidence (commands run), and any config/API impact. For UI-related changes in `InventarViewerApp`, include screenshots or terminal captures.

## Project Statistics

- When shared AI-agent guidance, workflow conventions, or statistics methodology changes, review and update `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, and `.github/copilot-instructions.md` together when they are affected.
- Shared guidance must not be updated in only one of these files; any intentional agent-specific divergence must be documented in the same change.
- Maintain `docs/project-statistics.md` as the living statistics ledger for the repository.
- Update the file after each completed Spec-Kit implementation phase, after each agent-driven repository change, or when a refresh is explicitly requested.
- Within the `## Fortschreibungsprotokoll` table, keep entries in strict chronological order: oldest entry at the top, newest and most recently added entry at the bottom; entries with the same date keep their insertion order.
- Keep a final top-level `## Gesamtstatistik` block as the last section of `docs/project-statistics.md`; do not append any later top-level section after it.
- Inside that final `## Gesamtstatistik` block, keep compact ASCII-only diagrams directly below the textual overall summary so the overall artifact mix, documented branch/phase curves, acceleration factors, and the comparison between experienced-developer effort, Thorsten-solo effort, and visible AI-assisted delivery stay readable in plain Markdown.
- Keep each short CEFR-B2 explanation directly adjacent to its matching ASCII diagram group, and keep the statistics block text-first so Braille displays, screen readers, and text browsers do not depend on color or layout alone.
- Each update must record branch or phase, observable work window, production/test/documentation line counts, main work packages, the conservative manual baseline of 80 manually created lines per workday across code, tests, and documentation, and the repo-specific Thorsten-Solo comparison baseline of 100 lines per workday for this native .NET codebase.
- When effort is converted into months, use explicit assumptions such as 21.5 workdays per month and, if applicable, 30 vacation days per year through calendar year 2026 and 31 vacation days per year from calendar year 2027 onward under a TVoeD-style 5-day-week calendar.
- When reporting acceleration, compare both manual references against visible Git active days and label the result as a blended repository speedup rather than a stopwatch measurement.
- When hour values are shown, convert the day-based estimates with the TVoeD working-day baseline of `7.8 hours` (`7h 48m`) per day.

## Copilot Instructions
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

**Key Conventions:**

**Language split:** Explanatory texts in comments/docs MUST be bilingual (German first, then English, CEFR B2). UI labels and log messages remain German.

**Naming:**
- Types/methods/properties/constants: PascalCase
- Local variables, parameters, private fields: camelCase (private fields prefixed `_`)
- Test methods: `<UnitUnderTest>_<Scenario>_<ExpectedOutcome>`
- DB table/column names: PascalCase

**Nullable reference types** are enabled everywhere — use `string?` for optional values.

**Async:** All I/O-bound public service methods return `Task` or `Task<T>`.

**Toolchain:** Use `.NET 10` with `C# 14.0`.

**Serialization:** `System.Text.Json` with camelCase naming policy throughout. Do not use Newtonsoft.Json. All existing usage of `Newtonsoft.Json` MUST be migrated to `System.Text.Json` and the `Newtonsoft.Json` NuGet package MUST be removed from all projects and the solution.

**Dependencies:** Keep NuGet packages on latest stable versions; pinning exceptions must be documented.

**HTTP client:** RestSharp in `InventarViewerApp`; integration tests use Playwright's `APIRequestContext`.

**Data access:** Dapper + `Microsoft.Data.Sqlite`. SQL is written as explicit raw strings with `IF NOT EXISTS` guards, PascalCase identifiers, and indices on frequently queried columns.

**`ServiceStatusWriter`** writes three output types per service: status (JSON), statistics (JSON), log (text). Identified by a service name prefix — default `""` for the agent, `"harvester-service"` for the harvester.

**CSV import:** CsvHelper with explicit class maps (see `InventarWorkerCommon/Services/Csv`).

**Error handling:** Catch at boundary layers (API controllers return `StatusCode(500, new { error = ... })`; TUI shows `MessageBox.ErrorQuery`). Use `using` statements on DB connections.

**XML doc comments** are mandatory on all public API members; CS1591 must not be globally suppressed.

**Didactic comments:** Add bilingual block/line comments for non-public members/variables where XML docs do not apply.

**DocFX sync:** Run `docfx docfx.json` whenever API signatures or XML documentation changes. The generated `api/` metadata and `_site/` HTML output are build artifacts and must not be committed. Published documentation is deployed automatically from `main` to GitHub Pages via `.github/workflows/docs-pages.yml`.

**Test framework:** MSTest. Use `[TestInitialize]`/`[TestCleanup]` for per-test setup. Assert default property values (empty strings, 0, false, null) explicitly in unit tests.

**Where to Put New Code:**

| What | Where |
|---|---|
| Shared domain models | `InventarWorkerCommon/Models/` |
| New services (shared) | `InventarWorkerCommon/Services/` + register in `Program.cs` |
| New API endpoints | `InventarWorkerService/Controllers/` + add integration test |
| New DB tables/views | Extend `SqliteDbService`; add indices; use `IF NOT EXISTS` |
| App-specific persistence/services | Under the app's own `Services/` folder |

## Inclusion & Accessibility

- Explanatory documentation for learner-facing and governance content MUST be bilingual with German first and English second at CEFR-B2 readability.
- Large normative documents such as `Pflichtenheft*.md` and `Lastenheft*.md` may use a synchronized English sidecar with suffix `.EN.md` instead of one oversized inline-bilingual file; the German version remains canonical unless explicitly marked otherwise.
- Follow `Programmierung #include<everyone>`: learner-facing guides, statistics, and generated HTML/API documentation must stay usable on Braille displays, with screen readers, and in text browsers.
- Treat WCAG 2.2 conformance level AA as the practical baseline for generated HTML documentation.
- If `docfx` output is regenerated, the same work item must also run a text-oriented accessibility review with Playwright + `@axe-core/playwright` and `lynx`.
- Recommended A11y toolchain for DocFX-based repos: Node 24 LTS, `npm`, Playwright, `@axe-core/playwright`, and `lynx`.


## Gemeinsame Governance-Ergaenzung / Shared Governance Addendum

- Alle nutzerseitigen Artefakte muessen barrierefrei gedacht und geprueft werden: CLI-Ausgaben, Dokumentation, HTML, UI und generierte Templates; WCAG 2.2 Level AA ist die Standard-Basis, sobald die Kriterien auf das Artefakt anwendbar sind.
- All user-facing artefacts must be designed and reviewed for accessibility: CLI output, documentation, HTML, UI, and generated templates; WCAG 2.2 Level AA is the default baseline wherever the criteria apply.

- Fuer C#/.NET-Repositories gilt standardmaessig eine Thorsten-Solo-Basis von `125` Zeilen/Arbeitstag, sofern das Repo keinen abweichenden, begruendeten Wert dokumentiert.
- The default Thorsten-solo baseline for C#/.NET repositories is `125` lines/workday unless the repository documents a justified deviation.

## Shared Parent Guidance

- The shared parent file `/Users/thorstenhindermann/RiderProjects/AGENTS.md` intentionally stores only repo-spanning baseline rules.
- Keep repository-specific build, test, workflow, architecture, and feature guidance in this repository's own files; when both layers exist, the repository-local files are the more specific authority.

---

## Level-2-Umgebungsregister / Level-2 Environment Registry

- Die zentrale `constitution.md` enthält das verbindliche Level-2 Project Environment Registry.
- Spec-Kit-Pläne und Agentenarbeit in Level-2-Projekten müssen die passende Registry-Zeile als verbindlichen Kontext für Runtime, Build/Test, A11Y, Statistik und Agentenflächen verwenden.
- Änderungen an einer Level-2-Runtime, Toolchain oder Statistik-Basis müssen `constitution.md`, `.specify/memory/constitution.md` und betroffene KI-Agenten-Dateien gemeinsam prüfen.

*The central `constitution.md` contains the binding Level-2 Project Environment Registry. Spec-Kit plans and agent work in Level-2 projects must use the matching registry row as binding context for runtime, build/test, A11Y, statistics, and agent surfaces. Changes to Level-2 runtime, toolchain, or statistics baselines require a joint review of `constitution.md`, `.specify/memory/constitution.md`, and affected AI-agent files.*
## Memory-Safe Languages (MSL) / Speichersichere Sprachen

- Level-2-Projekte SOLLEN eine speichersichere Sprache (Memory-Safe Language, MSL) als primäre Laufzeit verwenden, wenn die Zielplattform es erlaubt.
- Verbindliche MSL-Erlaubnisliste, Regeln und Begründungspflicht: siehe `constitution.md`, Prinzip XI.
- MSL-Kurzliste: Rust, Swift, C#, F#, Java, Kotlin, Scala, Go, Dart, Python, Ruby, JavaScript, TypeScript, Haskell, OCaml, Erlang, Elixir, Ada, SPARK.
- **Nicht** MSL (Begründung im Level-2-`constitution.md` erforderlich): C, C++, klassisches Objective-C, Assembly, `cc65`-C89, Zig (pre-1.0), Nim (manual), D ohne GC.
- In Nicht-MSL-Repositories (z. B. `C64Projects/cc65`) die im Level-2-`constitution.md` hinterlegte Begründung im Plan- und Task-Kontext erwähnen.
- `speckit.constitution` und `speckit.specify` SOLLEN bei Nicht-MSL-Primärsprache einen **nicht blockierenden** Hinweis ausgeben (Tooling-Aufgabe, separate Umsetzung).
- Änderungen an dieser Empfehlung erfordern ein gemeinsames Update in `constitution.md`, `.specify/memory/constitution.md`, `AGENTS.md`, `CLAUDE.md`, `GEMINI.md` und `.github/copilot-instructions.md`.

*Level-2 projects SHOULD use a memory-safe language (MSL) as their primary runtime when the target platform allows. Authoritative rules: `constitution.md`, Principle XI. MSL short list: Rust, Swift, C#/F#, Java/Kotlin/Scala, Go, Dart, Python, Ruby, JavaScript/TypeScript, Haskell, OCaml, Erlang/Elixir, Ada/SPARK. Non-MSL languages (C, C++, Assembly, `cc65`, Zig pre-1.0, …) require a documented justification in the Level-2 `constitution.md`. In non-MSL repositories (e.g. `C64Projects/cc65`), surface the documented justification in plans and tasks. `speckit.constitution` and `speckit.specify` SHOULD emit a non-blocking advisory warning when the primary language is not an MSL — tracked as a separate tooling task. Changes to this recommendation require a joint update across `constitution.md`, `.specify/memory/constitution.md`, and all four agent guidance files.*
## Sichere Code-Erzeugung / Secure Code Generation (ISO 27001/27002 A.8.28)

- KI-generierter Code MUSS den etablierten Secure-Coding-Best-Practices der Zielsprache und des Frameworks folgen. LLMs erzeugen nicht zuverlässig sicheren Code; explizite Durchsetzung ist erforderlich.
- Verbindliche Regeln und sprachspezifische Anforderungen: siehe `constitution.md`, Prinzip XII.
- Sprachspezifische Kurzregeln:
  - **C / C89**: Bounds-Checking, kein `gets()`, kein ungeprüftes `sprintf()`/`strcpy()`, CERT C.
  - **C# / .NET**: parametrisierte Queries, Output-Encoding gegen XSS, Anti-Forgery-Tokens, sichere Deserialisierung, Microsoft Secure Coding Guidelines.
  - **SQL**: nur parametrisierte Statements, kein dynamisches SQL aus nicht vertrauenswürdigem Input.
  - **Bash**: Variable in Anführungszeichen (`"$var"`), kein `eval` auf nicht vertrauenswürdigem Input, `--` End-of-Options.
  - **PowerShell**: `Set-StrictMode -Version Latest`, validierte Parameter, kein `Invoke-Expression` auf nicht vertrauenswürdigem Input.
- Kryptografie: aktuelle Algorithmen (AES-256, RSA >= 3072, SHA-256+, Ed25519); veraltete (MD5, SHA-1 für Signaturen, DES, RC4) nur mit expliziter Risikobegründung.
- Fehlerbehandlung darf keine internen Zustände, Stack-Traces oder Verbindungszeichenketten an Endbenutzer preisgeben.
- Hinzugefügte Abhängigkeiten müssen aktiv gepflegt sein und dürfen keine bekannten kritischen CVEs aufweisen.
- Code-Reviews MÜSSEN eine Sicherheitsperspektive für Eingabeverarbeitung, Authentifizierung, Autorisierung, Kryptografie und Datei-/Netzwerk-I/O enthalten.
- Änderungen an dieser Regel erfordern ein gemeinsames Update in `constitution.md`, `.specify/memory/constitution.md`, `AGENTS.md`, `CLAUDE.md`, `GEMINI.md` und `.github/copilot-instructions.md`.

*AI-generated code MUST follow the secure-coding best practices of the target language and framework. Authoritative rules: `constitution.md`, Principle XII. Language-specific short rules: C/C89 — bounds checking, no `gets()`, CERT C; C#/.NET — parameterised queries, output encoding, anti-forgery tokens, Microsoft Secure Coding Guidelines; SQL — parameterised statements only; Bash — quoted variables, no `eval` on untrusted input, `--` sentinel; PowerShell — `Set-StrictMode`, no `Invoke-Expression` on untrusted input. Cryptography: use current algorithms (AES-256, SHA-256+, Ed25519); deprecated (MD5, SHA-1 for signatures, DES, RC4) only with explicit risk acknowledgement. Error handling must not expose internals. Dependencies must have no known critical CVEs. Code reviews must include a security perspective for input handling, auth, crypto, and I/O. Changes require a joint update across `constitution.md`, `.specify/memory/constitution.md`, and all four agent guidance files.*
## Sichere Software-Architektur / Secure Software Architecture (ISO 27001/27002 A.8.27)

- KI-generierte und menschlich geschriebene Software-Architektur MUSS etablierten sicheren Architekturprinzipien folgen. Sicherer Code (Prinzip XII) ohne sichere Architektur reicht nicht aus — beide Ebenen müssen zusammenwirken.
- Verbindliche Regeln und sprachspezifische Architekturvorgaben: siehe `constitution.md`, Prinzip XIII.
- Verbindliche Architekturprinzipien:
  - **Trust Boundaries**: Explizite Vertrauensgrenzen definieren; alle Eingaben an Vertrauensgrenzen validieren und bereinigen.
  - **Defense in Depth**: Mindestens zwei unabhängige Sicherheitsschichten für kritische Assets.
  - **Least Privilege**: Jede Komponente, jeder Dienst und Prozess arbeitet mit minimalen Berechtigungen.
  - **Fail-Safe Defaults**: Zugriff standardmäßig verweigern, explizit gewähren; Fehlerpfade fallen in sicheren Zustand zurück.
  - **Angriffsfläche reduzieren**: Ungenutzte Endpunkte, Dienste und Debug-Funktionen deaktivieren oder entfernen.
  - **Separation of Concerns**: Authentifizierung, Autorisierung, Logging und Eingabevalidierung als Cross-Cutting Concerns implementieren, nicht ad-hoc verstreuen.
  - **Sichere Konfiguration**: Secrets in plattformgeeigneten Secret-Stores (z. B. Azure Key Vault, macOS Keychain), nie im Quellcode oder in Git-tracked Config-Dateien.
  - **Supply-Chain-Sicherheit**: Abhängigkeiten aus verifizierten Registries; Lock-Files committen; verwundbare Abhängigkeiten vor Release ersetzen.
- Änderungen an dieser Regel erfordern ein gemeinsames Update in `constitution.md`, `.specify/memory/constitution.md`, `AGENTS.md`, `CLAUDE.md`, `GEMINI.md` und `.github/copilot-instructions.md`.

*AI-generated and human-written software architecture MUST follow secure-architecture principles. Authoritative rules: `constitution.md`, Principle XIII. Core principles: trust boundaries (validate all input at system boundaries), defense in depth (at least two independent security layers), least privilege (minimum required permissions), fail-safe defaults (deny by default), attack surface reduction (disable unused features), separation of concerns (auth/logging/validation as cross-cutting concerns), secure configuration (secrets in secret stores, never in code or Git), supply-chain security (verified registries, lock files, no known-vulnerable dependencies). Principles XII + XIII together form the complete secure-development approach: XII = tactical code-level security, XIII = strategic architecture-level security. Changes require a joint update across `constitution.md`, `.specify/memory/constitution.md`, and all four agent guidance files.*
## Sicherheitsdokumentation / Security Documentation (XII/XIII Extensions)

- Jedes Level-2-Projekt MUSS die folgenden Sicherheitsdokumente pflegen, basierend auf den Templates in `.specify/templates/`:
  - **Bedrohungsmodell / Threat Model** (`threat-model-template.md`) — STRIDE-Methodik, Trust Boundaries, Risikobewertung (Prinzip XIII)
  - **Security Architecture Decision Records (S-ADR)** (`adr-template.md`) — architektonische Sicherheitsentscheidungen mit Compliance-Nachweis (Prinzip XIII)
  - **arc42 Section 8 Sicherheits-Querschnittskonzepte** (`arc42-security-template.md`) — Authentifizierung, Autorisierung, Verschlüsselung, Eingabevalidierung, Fehlerbehandlung, Logging, Abhängigkeiten, Deployment (Prinzip XIII)
  - **Sicherheits-Checkliste / Security Checklist** (`security-checklist-template.md`) — sprachspezifische Code-Review-Checkliste (Prinzip XII)
  - **Abhängigkeits-Audit / Dependency Audit** (`dependency-audit-template.md`) — CVE-Tracking, Lizenz-Compliance, Supply-Chain-Sicherheit (Prinzip XII)
  - **Sicherheits-Qualitätsszenarien / Security Quality Scenarios** (`security-quality-scenarios-template.md`) — iSAQB CPSA-F Qualitätsszenario-Methodik (Prinzip XII + XIII, SHOULD)
- Projektspezifische Instanzen werden in `docs/security/` gepflegt; S-ADRs als einzelne Dateien in `docs/security/adr/`.

*Every Level-2 project MUST maintain security documents based on templates in `.specify/templates/`: threat model (STRIDE), S-ADRs, arc42 Section 8 security concepts, security checklist, dependency audit, and security quality scenarios (SHOULD). Project-specific instances live in `docs/security/`; S-ADRs in `docs/security/adr/`. See `constitution.md`, Principles XII and XIII for authoritative requirements.*
## Sicherheitsstandards & Anwendbarkeit / Security Standards & Applicability

- Vor jeder Level-2-Aufgabe die anwendbaren Sicherheitsstandards aus `constitution.md`, Prinzipien XIV-XVIII bestimmen und explizit benennen.
- `NIST SSDF` und `CWE Top 25` gelten immer für Level-2-Arbeit.
- `OWASP ASVS` gilt für Web-, API-, HTTP- und authentifizierte Dienste; der gewählte ASVS-Level muss benannt werden.
- `SBOM` gilt für releasefähige oder verteilbare Artefakte; `VEX`, wenn bekannte Schwachstellen in ausgelieferten oder geprüften Komponenten bewertet werden müssen.
- `SLSA` gilt als Soll-Vorgabe für CI/CD- oder veröffentlichte Artefakte; `Zero Trust` ist für verteilte, servicebasierte, cloudnahe oder remote-verwaltete Systeme explizit zu prüfen.
- `CAPEC` soll in Bedrohungsmodellen für die risikoreichsten Angriffswege verwendet werden; `OWASP SAMM` soll für langlebige Projekte/Workspaces in Verbesserungspläne einfließen.
- `OWASP Cheat Sheet Series`, `OWASP Proactive Controls` und bei öffentlichen OSS-Repositories oder kritischen Abhängigkeiten `OpenSSF Scorecard` sind als ergänzende Referenzen zu berücksichtigen.
- Nichtanwendbarkeit immer als `N/A` mit kurzer Begründung dokumentieren; keine stillschweigende Auslassung.

*At the start of every Level-2 task, determine and name the applicable security standards from `constitution.md`, Principles XIV-XVIII. `NIST SSDF` and `CWE Top 25` always apply. `OWASP ASVS` applies to web/API/HTTP/auth-bearing services; `SBOM` applies to releasable or distributable artefacts; `VEX` applies when known vulnerabilities in shipped/evaluated components need a disposition statement. `SLSA` is the target model for CI/CD and published artefacts; `Zero Trust` must be explicitly evaluated for distributed, service-based, cloud, or remotely managed systems. `CAPEC`, `OWASP SAMM`, `OWASP Cheat Sheet Series`, `OWASP Proactive Controls`, and `OpenSSF Scorecard` are supporting references where relevant. Record non-applicability as `N/A` with justification rather than omitting it silently.*
## Agentischer Security-Workflow / Agentic Security Workflow

- In `spec.md`, `plan.md` und `tasks.md` die anwendbaren Standards samt Evidenzpfad festhalten.
- Bei Bedrohungsmodellen `STRIDE` als Basis und bei risikoreichen Flows zusätzlich relevante `CAPEC`-Patterns verwenden.
- Bei Web/API-Features den `ASVS`-Level und den Verifikationsumfang in `docs/security/` oder gleichwertiger Projektdokumentation ablegen.
- Bei Release-/Artefakt-Arbeit `SBOM`, `VEX`, Provenance/SLSA-Nachweise und gegebenenfalls `OpenSSF Scorecard` in Release- oder Sicherheitsdokumentation einplanen.
- Bei Architekturänderungen `Zero Trust`-Anwendbarkeit und bei langlebigen Projekten `SAMM`-Folgeaktionen prüfen.
- Default-Evidenzpfad: `docs/security/asvs-verification.md`, `docs/security/supply-chain-evidence.md`, `docs/security/zero-trust-applicability.md`, `docs/security/samm-assessment.md`; Abweichungen nur mit lokal dokumentierter Begründung.

*Capture the applicable standards and the evidence path in `spec.md`, `plan.md`, and `tasks.md`. Use `STRIDE` as the base for threat modeling and add relevant `CAPEC` patterns for the highest-risk flows. For web/API work, record the chosen `ASVS` level and verification scope in `docs/security/` or equivalent project documentation. For release and artefact work, plan `SBOM`, `VEX`, provenance/SLSA evidence, and `OpenSSF Scorecard` review where applicable. For architectural changes, evaluate `Zero Trust`; for long-lived projects, consider `OWASP SAMM` follow-up actions. The default evidence path is `docs/security/asvs-verification.md`, `docs/security/supply-chain-evidence.md`, `docs/security/zero-trust-applicability.md`, and `docs/security/samm-assessment.md`, unless the repository documents a justified equivalent location.*
## Hinweise / Notes

- Diese Datei bleibt bewusst kompakt und ergänzt die projektspezifische Dokumentation.
- This file intentionally stays compact and complements the project-specific documentation.

<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan
<!-- SPECKIT END -->
