<!--
Sync Impact Report
- Version change: 2.0.0 -> 2.1.0
- Modified principles:
  - IV. Testability and TDD Discipline
    -> IV. Testability, TDD, and Coverage Discipline
  - Added VII. Toolchain and Dependency Currency
- Added sections:
  - None
- Removed sections:
  - None
- Templates requiring updates:
  - .specify/templates/plan-template.md: ✅ updated
  - .specify/templates/spec-template.md: ✅ updated
  - .specify/templates/tasks-template.md: ✅ updated
  - .specify/templates/commands/*.md: ✅ updated
- Runtime guidance documents reviewed:
  - AGENTS.md: ✅ updated
  - CLAUDE.md: ✅ updated
  - GEMINI.md: ✅ updated
  - .github/copilot-instructions.md: ✅ updated
  - README.md: ✅ updated
- Follow-up TODOs:
  - None
-->
# InventarWorkerService Constitution

## Core Principles

### I. Didaktische und sprachliche Klarheit (Pedagogical and Linguistic Clarity)
All explanatory text in source comments, XML documentation, API docs, guides, and
examples MUST be bilingual: German text block first, English text block second. Both
language blocks MUST target CEFR B2 readability. Explanations MUST focus on why
decisions exist (trade-offs, constraints, and intent), not only what code does.
Implementation choices that obscure learning flow are prohibited.
Rationale: the project is used to train Fachinformatiker trainees and must stay
understandable for German and non-native speakers.

### II. Shared Domain and Layer Boundaries (NON-NEGOTIABLE)
Shared logic MUST be implemented in `InventarWorkerCommon` (`Models/`, `Services/`,
`Helpers/`). Runtime-specific behavior MUST remain in the owning application layer
(`InventarWorkerService`, `HarvesterWorkerService`, `InventarViewerApp`, or
`CtrlWorker*`). New API endpoints MUST be added in
`InventarWorkerService/Controllers/` and MUST include integration test coverage in
`InventarWorkerServiceIntegrationTest/`.
Rationale: strict boundaries prevent duplicated logic and reduce cross-project
regressions.

### III. Documentation Completeness and Learning-First Standards (NON-NEGOTIABLE)
Every public type and member MUST include complete XML documentation with
`<summary>`, `<param>`, `<returns>`, and `<exception>` where applicable. `<remarks>`
and `<example>` MUST be added when they improve learner understanding. Missing XML
documentation for public API members MUST be treated as a build failure; warning CS1591
MUST NOT be suppressed globally. When API signatures or XML comments change,
`docfx docfx.json` MUST be run and generated output MUST be committed in the same
change set. Runtime guidance in `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, and
`.github/copilot-instructions.md` MUST be kept consistent with this constitution.
Didactically relevant non-public members, variables, and complex control paths MUST
carry bilingual block or line comments where XML documentation is not applicable.
Rationale: documentation is a first-class training artifact and must stay executable.

### IV. Testability, TDD, and Coverage Discipline
Tests MUST use MSTest attributes and descriptive method names in the
`<UnitUnderTest>_<Scenario>_<ExpectedOutcome>` pattern. New feature work MUST start
with failing tests (Red), then implementation to passing tests (Green), then cleanup
(Refactor), unless an explicit exception is documented in the plan's complexity section.
Unit tests MUST be deterministic and independent of machine-specific state. Any API
contract change, new endpoint, or cross-service integration behavior MUST include or
update integration tests. Coverage for changed code paths MUST be at least 70% in CI.
Coverage of 80% or higher MUST be actively targeted; if a PR lands between 70% and 80%,
the PR MUST include an explicit follow-up item with owner and due date.
Rationale: explicit Red-Green-Refactor behavior and coverage gates reduce regression
risk while keeping improvements measurable.

### V. Data, Serialization, and Persistence Consistency
JSON serialization MUST use `System.Text.Json` with camelCase naming policy; new
usage of `Newtonsoft.Json` is prohibited. Data access MUST use Dapper with explicit SQL
strings, `IF NOT EXISTS` guards for schema creation, PascalCase identifiers for
tables/columns, and indices on frequently queried columns. Status output behavior MUST
remain compatible with the `ServiceStatusWriter` model (status JSON, statistics JSON,
log text).
Rationale: uniform contracts prevent drift between agent, harvester, viewer, and docs.

### VI. Branching and Pull Request Governance (NON-NEGOTIABLE)
The `main` branch is protected and MUST NOT receive direct feature commits. Every
feature, fix, or constitutional amendment MUST be implemented on a newly created branch
and merged through a pull request targeting `main`. Pull requests MUST state purpose,
touched projects, test evidence, and config/API impact; UI-impacting changes in
`InventarViewerApp` MUST include a screenshot or terminal capture.
Rationale: branch protection and documented review gates are mandatory for controlled
integration.

### VII. Toolchain and Dependency Currency
Repository work MUST target .NET 10 and C# 14.0 for new or migrated projects. NuGet
packages MUST be kept on latest stable versions as part of regular delivery. If a
package must stay behind latest stable due compatibility or vendor issues, the PR MUST
document package name, pinned version, rationale, and next review date.
Rationale: current toolchains and dependencies reduce security exposure and maintenance
cost.

## Implementation Constraints

- C# naming conventions (`PascalCase`, `camelCase`, `_camelCase`) and nullable
  reference types MUST remain enabled.
- Toolchain baseline MUST be `.NET 10` with `LangVersion` set to `14.0`.
- Runtime model MUST stay cross-platform: Windows Service (`AddWindowsService`),
  systemd (`AddSystemd`), and launchd compatibility.
- Worker loop timing MUST remain `30_000ms` in debug and `86_400_000ms` in release,
  unless explicitly amended through this constitution.
- New shared models belong in `InventarWorkerCommon/Models/`; new shared services belong
  in `InventarWorkerCommon/Services/` and MUST be registered in the consuming
  `Program.cs`.
- Error handling MUST be performed at system boundaries (API: HTTP 500 payload;
  TUI: `MessageBox.ErrorQuery`), with deterministic resource handling (`using` where
  applicable).

## Development Workflow and Quality Gates

1. Create a new working branch before implementation. Work on `main` for feature
   development is prohibited.
2. Define or update feature specification, plan, and tasks with a constitution check
   covering bilingual B2 documentation, XML completeness, TDD, coverage, layering, and
   dependency currency.
3. Implement code in the project-specific location defined by Principle II.
4. Run validation commands at minimum:
   `dotnet restore InventarWorkerService.sln`,
   `dotnet build InventarWorkerService.sln`,
   `dotnet test` with coverage collection,
   `dotnet list package --outdated`,
   and `docfx docfx.json` whenever API signatures/XML docs or documentation content
   changed.
5. Open a pull request to `main` with required evidence and ensure constitution
   compliance is reviewed before merge.
6. Perform a final documentation and coverage compliance review before merge.

## Governance

This constitution is authoritative for repository engineering practices and supersedes
conflicting local guidance. Amendment process: submit a pull request on a non-`main`
branch that includes (a) constitution changes, (b) propagated template/runtime updates,
and (c) a Sync Impact Report at the top of this file. Versioning policy follows semantic
versioning for governance:
- MAJOR: incompatible principle removals or redefinitions.
- MINOR: new principles/sections or materially expanded mandates.
- PATCH: clarifications, wording improvements, and non-semantic refinements.
Compliance review is mandatory in planning and code review; unresolved violations MUST
be documented in the implementation plan's complexity tracking section.

**Version**: 2.1.0 | **Ratified**: 2026-03-08 | **Last Amended**: 2026-03-08
