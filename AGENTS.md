# Repository Guidelines

## Project Structure & Module Organization
This repository is a multi-project .NET 10 / C# 14.0 solution (`InventarWorkerService.sln`). Core domain logic lives in `InventarWorkerCommon/` (`Models/`, `Services/`, `Helpers/`). Runtime services are in `InventarWorkerService/` (agent + API), `HarvesterWorkerService/` (collector), and `InventarViewerApp/` (Terminal UI client). Service control utilities are in `CtrlWorkerCommon/`, `CtrlWorkerServiceApp/`, `CtrlWorkerServiceCmdlet/`, and `CtrlWorkerServicePS/`.

Tests are split by scope: `InventarWorkerCommonTest/`, `CtrlWorkerCommonTest/`, and `InventarWorkerServiceIntegrationTest/`. Documentation sources are under `docs/` with DocFX config in `docfx.json` and generated output in `_site/`.

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

PRs should include: purpose, touched projects, test evidence (commands run), and any config/API impact. For UI-related changes in `InventarViewerApp`, include screenshots or terminal captures.

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

**Serialization:** `System.Text.Json` with camelCase naming policy throughout. Do not use Newtonsoft.Json.

**Dependencies:** Keep NuGet packages on latest stable versions; pinning exceptions must be documented.

**HTTP client:** RestSharp in `InventarViewerApp`; integration tests use Playwright's `APIRequestContext`.

**Data access:** Dapper + `Microsoft.Data.Sqlite`. SQL is written as explicit raw strings with `IF NOT EXISTS` guards, PascalCase identifiers, and indices on frequently queried columns.

**`ServiceStatusWriter`** writes three output types per service: status (JSON), statistics (JSON), log (text). Identified by a service name prefix — default `""` for the agent, `"harvester-service"` for the harvester.

**CSV import:** CsvHelper with explicit class maps (see `InventarWorkerCommon/Services/Csv`).

**Error handling:** Catch at boundary layers (API controllers return `StatusCode(500, new { error = ... })`; TUI shows `MessageBox.ErrorQuery`). Use `using` statements on DB connections.

**XML doc comments** are mandatory on all public API members; CS1591 must not be globally suppressed.

**Didactic comments:** Add bilingual block/line comments for non-public members/variables where XML docs do not apply.

**DocFX sync:** Run `docfx docfx.json` whenever API signatures or XML documentation changes.

**Test framework:** MSTest. Use `[TestInitialize]`/`[TestCleanup]` for per-test setup. Assert default property values (empty strings, 0, false, null) explicitly in unit tests.

**Where to Put New Code:**

| What | Where |
|---|---|
| Shared domain models | `InventarWorkerCommon/Models/` |
| New services (shared) | `InventarWorkerCommon/Services/` + register in `Program.cs` |
| New API endpoints | `InventarWorkerService/Controllers/` + add integration test |
| New DB tables/views | Extend `SqliteDbService`; add indices; use `IF NOT EXISTS` |
| App-specific persistence/services | Under the app's own `Services/` folder |
