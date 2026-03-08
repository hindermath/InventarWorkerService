# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Build the entire solution
dotnet build InventarWorkerService.sln

# Run a specific service
dotnet run --project InventarWorkerService/InventarWorkerService.csproj
dotnet run --project HarvesterWorkerService/HarvesterWorkerService.csproj
dotnet run --project InventarViewerApp/InventarViewerApp.csproj
```

## Test Commands

```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj

# Run only ctrl worker common tests
dotnet test CtrlWorkerCommonTest/CtrlWorkerCommonTest.csproj

# Run integration tests (requires InventarWorkerService running on http://localhost:5000 first)
dotnet run --project InventarWorkerService/InventarWorkerService.csproj
dotnet test InventarWorkerServiceIntegrationTest/InventarWorkerServiceIntegrationTest.csproj

# Run a single test method
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"

# Regenerate documentation when API/XML docs change
docfx docfx.json
```

## Branching Workflow (Mandatory)

- `main` is protected and must not receive direct feature commits.
- Create a new branch for each feature/fix/change.
- Merge to `main` only through a pull request with test evidence.

## Architecture Overview

This is a multi-project .NET 9.0 solution for hardware/software inventory management across machines.

### Data Flow

```
HarvesterWorkerService  →  InventarWorkerService (HTTP API)  →  HarvesterWorkerService stores to DB
InventarWorkerService   →  collects local machine inventory, exposes via REST
InventarViewerApp (TUI) →  queries InventarWorkerService API  →  persists in local SQLite
```

### Projects

**InventarWorkerService** — ASP.NET Core Worker + REST API running on each monitored machine
- Runs a background `Worker` loop (DEBUG: 30s, Release: 24h) collecting local hardware & software
- Exposes `GET /api/inventar/hardware|software|full|status`
- Swagger UI available in Development at `/swagger`
- Default port: `80` (production), configurable via `appsettings.json` Kestrel section

**HarvesterWorkerService** — Central collector service
- Queries the SQLite DB for all active machines with network info
- Resolves each machine by IPv4/IPv6/FQDN, calls the target machine's `InventarWorkerService` API
- Saves collected hardware/software inventory to SQLite, MongoDB, and PostgreSQL

**InventarViewerApp** — Terminal.Gui TUI client
- Connects to InventarWorkerService API via RestSharp (`ApiService`)
- Persists data locally via Dapper + SQLite (`DatabaseService`)
- UI views: `HardwareView`, `SoftwareView`, `StatusView`, `SettingsDialog`

**InventarWorkerCommon** — Shared library (the core domain)
- `Models/`: Hardware, Software, Network, Service, Settings, SqlDatabase domain models
- `Services/Hardware/`: `HardwareInventoryService`, `SoftwareInventoryService`
- `Services/Database/`: `SqliteDbService`, `MongoDbService`, `PgSqlDbService`
- `Services/Api/`: `ApiService` (RestSharp client for calling remote InventarWorkerService)
- `Services/Status/`: `ServiceStatusWriter` (writes status/stats/logs to files)
- `Services/Settings/`: `SettingsReader` (reads from INI config files via ini-parser)
- `Helpers/`: `AverageProcessingTime`, custom exceptions (e.g., `HostResolutionException`)

**CtrlWorkerCommon / CtrlWorkerServiceApp / CtrlWorkerServiceCmdlet / CtrlWorkerServicePS** — Windows Service control utilities and PowerShell cmdlets

### Key Technical Conventions

- **Nullable reference types** are enabled throughout; use `string?` where values may be absent
- **Async/await** for all I/O-bound operations; public service methods return `Task` or `Task<T>`
- **System.Text.Json** with camelCase naming policy — avoid Newtonsoft.Json
- **Dapper + Microsoft.Data.Sqlite** for local persistence; SQL written as raw strings with `IF NOT EXISTS`, indices on frequently queried columns
- **ServiceStatusWriter** writes three output types: status (JSON), statistics (JSON), log (text) — identified by service name prefix (default `""`, harvester uses `"harvester-service"`)
- Worker loop delay: `30_000ms` in `#if DEBUG`, `86_400_000ms` (24h) in Release
- **Test naming**: `<UnitUnderTest>_<Scenario>_<ExpectedOutcome>`
- **Language clarity**: Explanatory text in comments/docs must be bilingual (German block first, English block second) at CEFR B2 level
- **UI language**: German strings in UI labels and log messages
- **XML docs**: Public API members require complete XML documentation; do not suppress CS1591 globally
- **Didactic comments**: Add bilingual block/line comments for non-public members/variables where XML docs do not apply
- **DocFX**: Run `docfx docfx.json` when API signatures or XML docs change

### Database

- SQLite is the primary local store (path configured via settings or defaults)
- MongoDB and PostgreSQL are additional targets in HarvesterWorkerService
- Schema uses PascalCase table/column names; views and indices defined explicitly in SQL

### Adding New Code

- New shared domain models → `InventarWorkerCommon/Models/`
- New API endpoints in `InventarWorkerService` → add controller action + integration test
- New DB tables/views → extend `SqliteDbService` with clearly separated methods + add indices
- New services → place under `InventarWorkerCommon/Services/` and register via DI in `Program.cs`
