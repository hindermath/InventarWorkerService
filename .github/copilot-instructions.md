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
```

## Architecture

This is a .NET 9.0 multi-project solution for cross-platform IT hardware/software inventory.

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

**Language split:** Code and comments in English; UI labels and log messages in German.

**Naming:**
- Types/methods/properties/constants: PascalCase
- Local variables, parameters, private fields: camelCase (private fields prefixed `_`)
- Test methods: `<UnitUnderTest>_<Scenario>_<ExpectedOutcome>`
- DB table/column names: PascalCase

**Nullable reference types** are enabled everywhere — use `string?` for optional values.

**Async:** All I/O-bound public service methods return `Task` or `Task<T>`.

**Serialization:** `System.Text.Json` with camelCase naming policy throughout. Do not use Newtonsoft.Json.

**HTTP client:** RestSharp in `InventarViewerApp`; integration tests use Playwright's `APIRequestContext`.

**Data access:** Dapper + `Microsoft.Data.Sqlite`. SQL is written as explicit raw strings with `IF NOT EXISTS` guards, PascalCase identifiers, and indices on frequently queried columns.

**`ServiceStatusWriter`** writes three output types per service: status (JSON), statistics (JSON), log (text). Identified by a service name prefix — default `""` for the agent, `"harvester-service"` for the harvester.

**CSV import:** CsvHelper with explicit class maps (see `InventarWorkerCommon/Services/Csv`).

**Error handling:** Catch at boundary layers (API controllers return `StatusCode(500, new { error = ... })`; TUI shows `MessageBox.ErrorQuery`). Use `using` statements on DB connections.

**XML doc comments** are expected on all public members in service classes.

**Test framework:** MSTest. Use `[TestInitialize]`/`[TestCleanup]` for per-test setup. Assert default property values (empty strings, 0, false, null) explicitly in unit tests.

## Where to Put New Code

| What | Where |
|---|---|
| Shared domain models | `InventarWorkerCommon/Models/` |
| New services (shared) | `InventarWorkerCommon/Services/` + register in `Program.cs` |
| New API endpoints | `InventarWorkerService/Controllers/` + add integration test |
| New DB tables/views | Extend `SqliteDbService`; add indices; use `IF NOT EXISTS` |
| App-specific persistence/services | Under the app's own `Services/` folder |
