<!--
Sync Impact Report
- Version change: 2.5.0 -> 2.6.0
- Bump rationale:
  - MINOR: Added workspace-baseline alignment guidance from the repository root `constitution.md` without removing repository-specific principles.
- Modified principles:
  - None
- Added sections:
  - Workspace Baseline Alignment / Programmierung #include<everyone> — Inclusion & Accessibility By Default
  - Workspace Baseline Alignment / DE-First / EN-Second Delivery
- Removed sections:
  - None
- Templates requiring updates:
  - .specify/templates/plan-template.md: pending review
  - .specify/templates/spec-template.md: pending review
  - .specify/templates/tasks-template.md: pending review
  - .specify/templates/commands/constitution.md: pending review
- Follow-up TODOs:
  - Review template and runtime-guidance wording for repository-specific propagation where needed.
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
and merged through a pull request targeting `main`. Branches MAY use either the
existing topic naming or the numbered Spec-Kit form `NNN-short-description`. Pull
requests MUST state:
- Purpose and which projects are touched.
- Test evidence (coverage report or CI link).
- Config/API impact if applicable.
- UI-impacting changes in `InventarViewerApp` MUST include a screenshot or terminal
  capture.
- Sample console output when user-visible output changes.
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
- `Directory.Build.props` MUST keep repo-wide `Version`, `AssemblyVersion`, and
  `FileVersion` aligned as `Major.Minor.Patch.Build`. On numbered Spec-Kit
  branches, `Minor` = numerically interpreted feature/branch number as
  canonical PR number for versioning (`002` -> `2`), `Patch` = commit count in
  that feature/PR branch after the current change is committed, and `Build` =
  manual build counter incremented before every `dotnet build` or `dotnet test`.
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

### Commit Message Format

Every commit MUST use Conventional Commits format:
`<type>: <short imperative subject line>`

Allowed types: `feat:`, `fix:`, `chore:`, `docs:`, `refactor:`, `test:`, `ci:`.
The subject line MUST be lowercase and imperative (e.g., `feat: add PgSqlDbService write methods`).

Every commit that involves AI-assisted work MUST include a `Co-authored-by` trailer
identifying the active AI agent, for example:
```
Co-Authored-By: Claude Opus 4.6 <noreply@anthropic.com>
```
The exact identity token depends on the agent used in the session. The trailer MUST be
the last line of the commit message body.

Rationale: uniform commit history enables automated changelog generation and makes
AI-assisted sessions auditable.

### Statistical Documentation

`docs/project-statistics.md` is the mandatory, living statistical ledger for the
repository. It MUST be updated whenever one of the following happens:

1. A Spec-Kit implementation phase is completed or materially re-scoped.
2. An agent-driven work package changes repository content (code, tests, specs,
   plans, tasks, governance, or operational docs).
3. A contributor explicitly requests a statistics refresh.

Within the `## Fortschreibungsprotokoll` section, table rows MUST remain in strict
chronological order: oldest entry first, newest and most recently added entry last,
while rows with the same date keep their insertion order.

Every update MUST record, at minimum:

- branch or phase identifier and current status,
- observable git-based work window (first and last date, commit days where possible),
- current or change-based counts for production code, test code, and documentation,
- the main work packages or delivered artefacts,
- whether the numbers come from committed history, the working tree, or both,
- a conservative manual-effort baseline using **80 manually created lines per
  workday** for an experienced developer across production code, test code, and
  documentation,
- when time spans are derived, the assumptions for monthly conversion
  (21.5 workdays/month) and TVöD-style annual leave (30 vacation days per year
  through end of 2026, 31 days from 2027 onwards under a 5-day-week calendar),
- when hour values are shown, convert day-based estimates using the TVöD working-day
  baseline of **7.8 hours (7h 48m)** per day.

Manual-effort estimates for a small team MAY be derived from that baseline, but
the formula and assumptions MUST be stated explicitly.

## Workspace Baseline Alignment

This Spec-Kit constitution inherits the binding workspace-family governance from `constitution.md` in the repository root. Project-specific rules remain in force; where both apply, the stricter rule wins.

### A. Programmierung #include<everyone> — Inclusion & Accessibility By Default

`Programmierung #include<everyone>` is a binding repository-wide principle. All user-facing artefacts — including CLI output, documentation and Markdown, HTML and generated websites, graphical user interfaces, and generated templates or scaffolding — MUST follow WCAG 2.2 Level AA wherever the criteria are applicable. They MUST remain usable with keyboard-only interaction, screen readers, Braille displays, and text browsers. Accessibility review is part of completion, not post-processing.

### B. DE-First / EN-Second Delivery

German is the canonical first language for user-facing governance and documentation in this repository family; English follows directly after it. User-facing and learner-facing guidance MUST remain bilingual at approximately CEFR-B2 readability, and materially changed guidance MUST update both language tracks in the same change.

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

### Lastenheft Archivierung (Feature Completion Archive)

When a feature's implementation is fully merged, the corresponding `Lastenheft_*.md`
MUST be renamed to stamp the delivering branch name onto the filename:

```bash
# macOS/Linux
bash scripts/rename-lastenheft.sh <LH-Datei> <branch-name>

# Windows
pwsh scripts/rename-lastenheft.ps1 -File <LH-Datei> -BranchName <branch-name>
```

Example: `Lastenheft_PostgreSQL_Implementation.md` + branch `008-pgsql-parity`
→ `Lastenheft_PostgreSQL_Implementation.008-pgsql-parity.md`.

This rename MUST be included in the final tasks.md as the last task of the Polish
phase. Omitting it leaves the Lastenheft in an ambiguous delivered/undelivered state
and breaks traceability.

Use `docs/project-statistics.md` for the living project-statistics ledger and
manual-effort baseline tracking.

**Version**: 2.6.0 | **Ratified**: 2026-03-08 | **Last Amended**: 2026-04-20
