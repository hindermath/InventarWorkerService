# Project Guidelines

This document summarizes conventions and practices in this repository to help contributors write consistent, testable code.

Contents
- Coding conventions in the current codebase
- Code organization and package structure
- Unit and integration testing approaches


## Coding conventions

General
- Language and runtime: C# on .NET 9.0 (nullable reference types are enabled and used).
- Style: Follow standard .NET naming guidelines.
  - Types, methods, properties, events: PascalCase (e.g., DatabaseService, GetMachinesAsync).
  - Local variables, parameters, private fields: camelCase (private fields often prefixed with underscore, e.g., _connectionString).
  - Constants: PascalCase.
- Records and POCOs: Models are primarily declared as record or class types with auto-properties and default initializers.
  - Example: public record Machine { public int Id { get; set; } ... }
- Nullability: Use nullable reference types (e.g., string?) where values may be absent; use value types with nullable wrappers (e.g., DateTime?) as needed.
- Async I/O: Prefer async/await for I/O-bound work. Public service methods are generally Task or Task<T> returning (e.g., GetMachinesAsync).
- Exceptions: Throw specific exceptions with clear messages; catch exceptions at UI or boundary layers to show user-friendly messages. In TUI (Terminal.Gui), exceptions are handled and presented via MessageBox.
- Logging: Utilize standard logging in services/apps when available. Where logging frameworks are not yet wired, ensure meaningful exception messages and return flows.
- Serialization: System.Text.Json is used in tests and services; options frequently set include camelCase naming and case-insensitive property names. Avoid relying on Newtonsoft unless necessary.
- HTTP client usage: RestSharp is used in InventarViewerApp ApiService for calling backend endpoints.
- Data access: Dapper with Microsoft.Data.Sqlite is used in InventarViewerApp for persistence. SQL is written explicitly (triple-quoted raw strings) with IF NOT EXISTS safeguards, indices, and views.
- SQL schema: Use snake-free, PascalCase or CamelCase table and column names consistent with C# naming. Create indexes for common lookups.
- CSV import: CsvHelper is used with explicit class maps for type-safe CSV ingestion.
- Internationalization: Some UI labels/test descriptions are in German; keep consistent language in UI strings within each module.
- Comments and docs: XML documentation comments are used on public members in some services. Prefer concise summaries for public APIs.

Error handling patterns
- Boundary layers (UI, API services) catch exceptions and provide user feedback. Example: HardwareView catches and displays errors via MessageBox.ErrorQuery.
- Database operations should use using statements on connections/commands and fail fast with descriptive error messages.

Performance and scalability
- Use indices on frequently queried columns (e.g., Machines(Name), inventory CreatedAt columns).
- For aggregated views, prefer database-side views and limited projections rather than transferring excessive data to the app.


## Code organization and package structure

Solution overview (top-level projects)
- InventarWorkerService: Backend worker/API service that exposes inventory endpoints (e.g., /api/inventar/*).
- HarvesterWorkerService: Service that collects system/hardware/software information (harvester).
- InventarViewerApp: Terminal.Gui-based TUI client that queries the API and persists data locally via SQLite.
  - API/: API-related utilities (if present)
  - Controllers/: MVC or API controllers (if present)
  - Models/: Application and database models
    - Database/: POCOs for persisted entities (e.g., Machine)
  - Services/: Application services (e.g., ApiService, DatabaseService)
  - UI/: TUI views (e.g., HardwareView)
- ServiceStatusReaderApp: Utility app to read status of services.
- CtrlWorkerCommon, InventarWorkerCommon: Shared libraries
  - Models/: Shared domain models (Hardware, Software, Service info, etc.)
- Test projects
  - InventarWorkerCommonTest: MSTest-based unit tests for model behavior and simple logic
  - InventarWorkerServiceIntegrationTest: MSTest + Microsoft.Playwright.MSTest-based integration/API tests

Data and scripts
- InventarDB.sql: SQL schema and/or helper scripts for DB.
- CSV files (Machines.csv, HardwareInventories.csv, SoftwareInventories.csv): Example or seed data handled by DatabaseService.

Dependency flow
- Apps/services depend on shared libraries: InventarWorkerCommon and CtrlWorkerCommon.
- InventarViewerApp consumes API endpoints from InventarWorkerService and persists selected data locally via Dapper/SQLite.
- Tests reference the corresponding production assemblies to validate behavior.

Notable technologies per area
- Persistence: Microsoft.Data.Sqlite + Dapper; CsvHelper for import; explicit SQL (CREATE TABLE/INDEX/VIEW, SELECT/INSERT/UPDATE).
- HTTP/API: RestSharp client (viewer); Integration tests use Playwright’s APIRequestContext for HTTP calls.
- UI: Terminal.Gui for TUI screens and interactions.
- JSON: System.Text.Json for serialization.


## Unit and integration testing approaches

Test frameworks
- MSTest is used for both unit and integration tests.
  - Attributes: [TestClass], [TestMethod], [TestInitialize], [TestCleanup], [ClassInitialize], [ClassCleanup].
- Integration tests additionally use Microsoft.Playwright.MSTest with PageTest and IAPIRequestContext for HTTP-level API testing.

Unit testing guidelines (InventarWorkerCommonTest)
- Scope: Validate model defaults, property set/get, value semantics, and simple behaviors.
- Structure: Group tests by domain model type (e.g., CpuInfoTests, OsInfoTests, ServiceStatisticsTests, SystemInfoTests, SoftwareInfoTests).
- Patterns:
  - Arrange minimal data; Act by setting properties or invoking simple methods; Assert using MSTest’s Assert class.
  - Use [TestInitialize]/[TestCleanup] for per-test setup/teardown; [ClassInitialize]/[ClassCleanup] for class-wide lifecycle steps.
- Nullability and defaults: Explicitly assert default values for strings (string.Empty), numerical defaults (0/0.0), booleans (false), and nullables (null) where applicable.

Integration testing guidelines (InventarWorkerServiceIntegrationTest)
- Scope: Validate live API behavior of InventarWorkerService endpoints, HTTP status codes, JSON parseability, headers, and basic content checks.
- Tooling: Microsoft.Playwright.MSTest with PageTest to access Playwright’s APIRequest fixture.
- Setup:
  - BaseURL is typically http://localhost:5000 (RemoteUrl examples may also be present).
  - [TestInitialize] creates a new APIRequest context; [TestCleanup] can dispose it.
- Patterns:
  - Use _apiContext.GetAsync("/api/inventar/...", options) with Accept: application/json.
  - Assert response.Ok and that content is non-empty; validate JSON using System.Text.Json (JsonDocument.Parse).
  - Additional tests may check swagger endpoints (/swagger, /swagger/v1/swagger.json), health checks, multiple requests performance, and custom headers.
- Environment prerequisites:
  - Ensure InventarWorkerService is running and listening on the configured BaseURL prior to executing these tests.
  - If ports differ, adjust constants in the test class (BaseUrl, RemoteUrl) or use configuration.

How to run tests
- From the repository root:
  - Run all tests: dotnet test
  - Run only unit tests project: dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj
  - Run only integration tests project: dotnet test InventarWorkerServiceIntegrationTest/InventarWorkerServiceIntegrationTest.csproj
- Integration test setup:
  - Start the InventarWorkerService locally (e.g., dotnet run --project InventarWorkerService/InventarWorkerService.csproj) so it serves http://localhost:5000 before running tests.
  - Optionally adjust test BaseUrl if the service address differs.

Test authoring tips
- Keep tests deterministic: avoid external network and time dependencies where not necessary. For API tests, stabilize the service state before assertions, or mock external integrations at the service layer if added later.
- Use descriptive test method names: <UnitUnderTest>_<Scenario>_<ExpectedOutcome>.
- Prefer verifying the smallest meaningful assertions (e.g., property values, HTTP status, JSON parseability). Add deeper content checks when APIs stabilize.
- For data-access code, prefer testing at the service boundary with an ephemeral or in-memory DB where possible. With SQLite, use temporary DB files or unique connection strings per test run if you add unit/integration tests around DatabaseService.

CI considerations
- Ensure Playwright browsers are installed if UI/browser testing is added; current integration tests use APIRequestContext and do not require a visible browser.
- If tests rely on local ports, make ports configurable via environment variables for CI environments.


## Adding new code
- Place shared domain models in InventarWorkerCommon to maximize reuse.
- Put app-specific persistence and service logic under the corresponding app’s Services folder.
- Follow existing naming and async patterns; add XML summary comments for new public APIs.
- For new endpoints in InventarWorkerService, update integration tests to cover status codes, JSON validity, and basic payload structure.
- For new DB tables or views in InventarViewerApp, extend DatabaseService with clearly separated methods; create indices for query performance; add CSV maps if importing data.
