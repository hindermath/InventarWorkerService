# Repository Guidelines

## Project Structure & Module Organization
This repository is a multi-project .NET 9 solution (`InventarWorkerService.sln`). Core domain logic lives in `InventarWorkerCommon/` (`Models/`, `Services/`, `Helpers/`). Runtime services are in `InventarWorkerService/` (agent + API), `HarvesterWorkerService/` (collector), and `InventarViewerApp/` (Terminal UI client). Service control utilities are in `CtrlWorkerCommon/`, `CtrlWorkerServiceApp/`, `CtrlWorkerServiceCmdlet/`, and `CtrlWorkerServicePS/`.

Tests are split by scope: `InventarWorkerCommonTest/`, `CtrlWorkerCommonTest/`, and `InventarWorkerServiceIntegrationTest/`. Documentation sources are under `docs/` with DocFX config in `docfx.json` and generated output in `_site/`.

## Build, Test, and Development Commands
- `dotnet restore InventarWorkerService.sln`: restore dependencies for all projects.
- `dotnet build InventarWorkerService.sln`: compile the full solution.
- `dotnet run --project InventarWorkerService/InventarWorkerService.csproj`: run local API/worker service.
- `dotnet run --project HarvesterWorkerService/HarvesterWorkerService.csproj`: run central harvester.
- `dotnet run --project InventarViewerApp/InventarViewerApp.csproj`: start TUI viewer.
- `dotnet test`: execute all unit and integration tests.
- `dotnet test InventarWorkerServiceIntegrationTest/InventarWorkerServiceIntegrationTest.csproj`: run integration tests only.
- `pwsh InventarWorkerServiceIntegrationTest/bin/Debug/net9.0/playwright.ps1 install`: install Playwright browsers after first build.
- `docfx docfx.json`: build API and Markdown documentation.

## Coding Style & Naming Conventions
Use C# with 4-space indentation and nullable reference types enabled. Follow existing naming patterns: `PascalCase` for types/methods/properties, `camelCase` for locals/parameters, and `_camelCase` for private fields. Keep platform-specific behavior isolated in service/controller layers; put reusable logic in `InventarWorkerCommon`.

No repository-wide linter config is enforced; keep changes consistent with surrounding files and prefer small, focused diffs.

## Testing Guidelines
Tests use MSTest (`[TestClass]`, `[TestMethod]`). Prefer descriptive test names such as `<UnitUnderTest>_<Scenario>_<ExpectedOutcome>`. Keep unit tests deterministic and independent of machine state. Integration tests require `InventarWorkerService` running at `http://localhost:5000`; remote tests may be network-dependent.

## Commit & Pull Request Guidelines
Recent history follows imperative subjects (for example: `Add ...`, `Update ...`, `Refine ...`). Continue with short, present-tense commit titles and narrow scope per commit.

PRs should include: purpose, touched projects, test evidence (commands run), and any config/API impact. For UI-related changes in `InventarViewerApp`, include screenshots or terminal captures.
