# Implementation Plan: Secure-Development-Hardening

**Branch**: `002-secure-development-hardening` | **Date**: 2026-08-30 | **Spec**: [spec.md](spec.md)
**Input**: Accepted feature specification and the completed requirements and security checklists in `specs/002-secure-development-hardening/`

## Summary

Dieses Feature erstellt zuerst eine vollständige, ehrliche Sicherheitsbewertung für alle 157 stabilen Prüfpunkt-IDs. Danach werden nur belegte Findings innerhalb der vorhandenen Worker-, API-, Harvester-, Viewer-, Datenbank-, Datei-, Konfigurations-, Prozess-, CI- und Agentengrenzen minimal gehärtet. Jede positive Aussage benötigt aktuelle, reproduzierbare Evidenz. Stubs und bloße Dateiexistenz gelten nicht als Nachweis.

*This feature first creates a complete and honest security assessment for all 157 stable checkpoint IDs. It then applies only evidence-driven hardening within the existing worker, API, harvester, viewer, database, file, configuration, process, CI, and agent boundaries. Every positive claim needs current, reproducible evidence. Stubs and file existence alone do not count as proof.*

Die technische Umsetzung verwendet die bestehende .NET-10-/C#-14-Lösung, `System.Text.Json`, parametrisierte Datenzugriffe und den Red-Green-Refactor-Zyklus. Sicherheits-, Architektur-, A11Y-, Supply-Chain- und Governance-Nachweise werden an den kanonischen Repository-Pfaden vervollständigt. Neue fachliche Inventarfunktionen, neue Provider, Cloud- oder KI-Runtime-Komponenten sind ausgeschlossen.

*The implementation uses the existing .NET 10/C# 14 solution, `System.Text.Json`, parameterized data access, and Red-Green-Refactor. Security, architecture, accessibility, supply-chain, and governance evidence is completed at the canonical repository paths. New inventory features, providers, cloud services, and AI runtime components are excluded.*

## Technical Context

**Language/Version**: C# 14.0 on .NET 10; C# is on the memory-safe-language allow-list

**Primary Dependencies**: ASP.NET Core, MSTest, Playwright, Dapper, Microsoft.Data.Sqlite, MongoDB.Driver, Npgsql, RestSharp, CsvHelper, Serilog, Terminal.Gui; all direct dependencies remain on the latest stable compatible versions, with documented exceptions only

**Storage**: SQLite, MongoDB, PostgreSQL, JSON status/statistics files, text logs, configuration files, and generated CI/release evidence

**Testing**: MSTest unit and integration tests, Playwright HTTP/API tests, Coverlet XPlat coverage, ReportGenerator, repository validators, DocFX, Axe, Lynx, Gitleaks, and agent-secret scanning

**Target Platform**: Windows, macOS, and Linux services and clients; GitHub Actions runners for cross-platform and release evidence

**Project Type**: Multi-project service/API/TUI solution with shared domain and data-access libraries

**Performance Goals**: Preserve current service and UI behavior; security controls use finite HTTP timeouts and cancellation and do not introduce unbounded retries

**Constraints**: Coverage MUST remain at least 70% and target at least 80%; no secret or internal exception disclosure; no unparameterized input-derived SQL; no tracked `api/`, `_site/`, local SBOM/scan output, runner logs, credentials, or autonomous state edits by implementation phases

**Scale/Scope**: 157 stable CL IDs in twelve checklists; two complete HTTP/API surfaces; three database providers; Worker, Harvester, Viewer, shared library, service-control utilities, CI/CD, documentation, and five maintained agent guidance surfaces

## Constitution Check

*GATE: Passed for planning. It MUST be repeated after Phase 1 design and before implementation delivery. Implementation gates remain fail-closed until their evidence exists.*

| Check | Decision | Planned proof |
|---|---|---|
| Level-2 environment | Pass | Bind the `RiderProjects/InventarWorkerService` registry row: .NET 10/C# 14, `InventarWorkerService.sln`, MSTest/Playwright, DocFX text-first A11Y, manual baseline 80 and repository-specific Thorsten-Solo baseline 100 lines/workday. |
| Branch and PR flow | Pass | Work only on `002-secure-development-hardening`; target `main`; delivery is coordinator-owned `MergeAndSync`. No commit, push, PR, merge, or state edit occurs in this plan phase. |
| Versioning | Pass | Keep `Version`, `AssemblyVersion`, and `FileVersion` aligned as `Major.2.Patch.Build`. Before a commit, set `Patch` to the current feature-branch commit count plus one, so it equals the count after that commit. Increment `Build` once immediately before every `dotnet build` or `dotnet test`. |
| MSL | Pass | C# is memory-safe under Principle XI. `docs/security/msl-applicability.md` records that MSL status does not replace API, SQL, I/O, secret, logging, or dependency controls. |
| Secure coding | Pass | Apply Microsoft secure coding guidance, input validation, output encoding where relevant, `System.Text.Json`, parameterized database access, safe error boundaries, finite HTTP timeouts/cancellation, SSRF review, and secret redaction. Remove `Newtonsoft.Json` from every product project. |
| Secure architecture | Pass | Model all listed trust boundaries; use fail-safe defaults, least privilege, defense in depth, attack-surface reduction, separated cross-cutting controls, secure configuration, and supply-chain verification. |
| Project boundaries | Pass | Shared models/services stay in `InventarWorkerCommon`; host-specific HTTP and process controls stay in `InventarWorkerService`, `InventarViewerApp`, `HarvesterWorkerService`, or service-control layers. No new product or provider is introduced. |
| Red-Green-Refactor | Pass | Add a failing security or regression test for each code hardening finding, make the smallest change that passes, then refactor while the focused and full suites remain green. Evidence-only changes use deterministic validators instead of artificial tests. |
| Coverage | Pass | Collect XPlat coverage; fail delivery below 70%; report the 80% target and any justified gap. Security-critical changed paths receive focused branch and negative-case review. |
| Dependencies | Pass | Run direct/transitive inventory, vulnerability and outdated-package checks. Adopt latest stable compatible releases; any pinning exception requires owner, risk, evidence, expiry, and trigger. |
| Serialization and data | Pass | Product JSON uses `System.Text.Json` with camelCase policy. SQLite/PostgreSQL use parameters and constrained identifiers; MongoDB uses typed/equivalent safe filters. Least-privilege and provider-specific failure evidence is not transferred between providers without proof. |
| Security standards | Pass | NIST SSDF, CWE Top 25, OWASP ASVS Level 2, STRIDE/CIA, CAPEC, SBOM, conditional VEX review, SLSA, Zero Trust, SAMM, OpenSSF Scorecard, OWASP Cheat Sheets/Proactive Controls, CRA screening, iSAQB/arc42, and WCAG 2.2 AA are Applicable. |
| Standards marked N/A | Pass | AI-SBOM is N/A because AI is development tooling only. BSI C3A/C5, NIS2, DORA, and EU AI Act are N/A under current non-cloud, non-regulated, non-AI-runtime assumptions. Each decision has a re-evaluation trigger. |
| Security documents | Pass | Complete the canonical `docs/security/` set, twelve project instances, findings/closeout index, and security ADRs only when a material security decision is made. Stubs cannot produce a positive result. |
| Architecture evidence | Pass | Add/update context, runtime, deployment, risk, and quality-scenario evidence under `docs/architecture/`. General ADR is N/A unless a non-security structural decision emerges; material security decisions use `docs/security/adr/`. |
| Documentation impact | `UpdateRequired` | Sources: Constitution and secure-development baseline. Owner: project owner; reviewers: security, architecture, A11Y, and release roles. Update security, architecture, A11Y, learner/navigation, agent, and statistics evidence. Public API/XML changes additionally require bilingual XML documentation and DocFX regeneration. |
| DocFX | Applicable | Repository documentation changes are in the published DocFX scope. Run DocFX, the repository post-processor, Axe, Lynx, and manual text-first review; `api/` and `_site/` remain untracked. |
| A11Y and bilingual delivery | Pass | User/learner evidence is German first, English second, CEFR B2, text-first, and understandable without colour or layout. HTML uses WCAG 2.2 AA as the practical baseline. |
| Statistics | Pass | Update `docs/project-statistics.md` after implementation using its configured renderer. Preserve chronological ordering and the final `Gesamtstatistik` section. |
| Agent parity | Pass | Review all five agent files, both constitutions, affected Spec-Kit templates, and repository template sources together. Change only authoritative current prose that conflicts with the installed stack; preserve historical records. |
| Security-first tracking | Pass | Do not track credentials, connection strings, local agent databases/history/logs, runtime routing logs, generated DocFX output, or unbound scanner/SBOM output. |
| Script parity | N/A | No new or changed script-shaped tool is planned. Trigger: any implementation choice adds or changes a script; then paired Bash/PowerShell, help, man page, dry-run, naming, and parity evidence become mandatory before work continues. |
| Shared writers | Pass after plan repair | Tasks MUST serialize writes to `Directory.Build.props`, assessment/evidence files, `.github/workflows/*`, `docs/project-statistics.md`, the five agent files, both constitutions, affected templates, and coordinator-owned state/evidence boundaries. Parallel work may read these files but may not write them concurrently. |

### Installed Governance Preset Authority

The installed registry is the source for this run. The eight core presets remain the standard matrix; the four locally enabled process presets form the authoritative twelve-preset fleet profile.

| Priority | Preset | Installed version | Plan effect |
|---:|---|---:|---|
| 10 | `security-governance` | 0.6.2 | Secure development and evidence |
| 20 | `architecture-governance` | 0.5.2 | Secure architecture and cloud decisions |
| 30 | `isaqb-architecture-governance` | 0.2.2 | arc42/iSAQB evidence |
| 40 | `a11y-governance` | 0.4.3 | WCAG, bilingual B2, didactics |
| 50 | `cross-platform-governance` | 0.2.2 | Conditional script parity |
| 60 | `agent-parity-governance` | 0.4.2 | Maintained surface parity |
| 61 | `model-routing-governance` | 0.1.4 | Phase-boundary routing only; no model names in feature requirements |
| 64 | `intake-authoring-governance` | 0.3.1 | Intake provenance and write authority |
| 65 | `intake-review-governance` | 0.2.1 | Accepted review gate |
| 66 | `intake-sequencing-governance` | 0.2.3 | No competing intake expansion |
| 70 | `autonomous-run-governance` | 0.4.1 | State, authority, evidence, and closeout |
| 80 | `parallel-autonomous-run-governance` | 0.2.6 | N/A for execution because no campaign was authorized; its installed rules remain part of drift review |

Observed authoritative drift is limited to facts that implementation must repair or disposition:

- The accepted intake records an older six-preset subset as historical context. It MUST remain unchanged.
- Several current agent/constitution surfaces still describe older eight-preset versions, while installed registry and maintained repository templates already describe the newer core versions and twelve-preset fleet profile.
- `docs/secure-development/baseline-manifest.json` declares baseline 3.1.0 and CL-09/CL-12 v2.1.0, while the two canonical checklist files declare v2.2.0; 157 stable IDs remain present and unique.
- `.github/workflows/ci.yml` selects .NET 9 although the binding registry and every product project target .NET 10.

The implementation MUST repair only current authoritative contradictions needed for this feature, atomically across affected surfaces. Historical Lastenhefte, changelogs, statistics entries, and provenance text remain untouched unless they falsely present themselves as current authority.

## Standards and Evidence Plan

| Standard/checkpoint | Applicability | Evidence and acceptance path |
|---|---|---|
| NIST SSDF SP 800-218 | Applicable | `docs/security/security-checklist.md`; map PO, PS, PW, and RV practices to current evidence or findings. |
| CWE Top 25 | Applicable | Same checklist, code review, focused tests, and finding records by trust boundary. |
| OWASP ASVS 5.0 Level 2 | Applicable | `docs/security/asvs-verification.md`; evaluate every applicable L2 control for all endpoints of InventarWorkerService and the Viewer API. |
| STRIDE and CIA | Applicable | `docs/security/threat-model.md`; enumerate assets, classifications, boundaries, threats, controls, and residual risk. |
| CAPEC | Applicable | Reference the most relevant CAPEC patterns for each highest-risk attack path in the threat model. |
| SBOM | Applicable | Machine-readable CycloneDX or SPDX evidence per distributable artefact set, uniquely bound to version/hash and referenced by `docs/security/supply-chain-evidence.md`. |
| VEX | Applicable conditional decision | Every vulnerability scan produces either a no-known-finding record with time/scope or a VEX-style disposition before release. A known finding requires status, rationale, evidence, owner, and trigger. |
| SLSA | Applicable | Record current provenance level, target, builder/source identity, integrity evidence, and gaps in `supply-chain-evidence.md`. |
| Zero Trust | Applicable | `docs/security/zero-trust-applicability.md`; network location alone is never sufficient trust evidence for service/database flows. |
| OWASP SAMM | Applicable | `docs/security/samm-assessment.md`; maturity snapshot, prioritized improvements, owners, and cadence. |
| OpenSSF Scorecard | Applicable | Public repository and high-impact dependencies in `dependency-audit.md` and supply-chain evidence. |
| OWASP Cheat Sheets / Proactive Controls | Applicable | Trace supporting guidance to C# secure-coding and ASVS evidence. |
| CRA | Applicable screening | `docs/security/cra-applicability.md`; technical screening only, with owner decision before release. |
| NIS2 / DORA | N/A | Current project assumptions do not make the operator a regulated entity or financial ICT provider. Trigger: regulated operation, customer, or supply chain. |
| EU AI Act / AI-SBOM | N/A | AI is development tooling only. Trigger: any model, dataset, AI service, inference infrastructure, or runtime becomes part of the product or operated system. |
| BSI C3A / C5 | N/A | No cloud provider or managed service is introduced. Trigger: provider-dependent hosting or cloud assurance scope. |
| iSAQB / arc42 | Applicable | Context, runtime, deployment, risk, and quality-scenario evidence in `docs/architecture/`. |
| WCAG 2.2 Level AA | Applicable | `docs/accessibility/secure-development-hardening.md`, DocFX/Axe/Lynx evidence, manual text and language review. |

## Project Structure

### Planning artefacts for this feature

```text
specs/002-secure-development-hardening/
├── autonomous-run-gate-requirements.json
├── autonomous-run-state.json             # coordinator-owned; never edited by this phase
├── checklists/
│   ├── plan.md
│   ├── requirements.md
│   └── security.md
├── contracts/
│   └── classification-evidence-record.schema.json
├── data-model.md
├── plan.md
├── quickstart.md
├── research.md
└── spec.md
```

`tasks.md` is intentionally absent and is created only by `/speckit.tasks` after plan review.

### Existing implementation boundaries

```text
InventarWorkerCommon/
├── Models/
└── Services/
    ├── Api/
    ├── Csv/
    ├── Database/             # SQLite, MongoDB, PostgreSQL
    ├── Settings/
    └── Status/

InventarWorkerService/
└── Controllers/              # worker HTTP/API surface

HarvesterWorkerService/       # central collector and provider writes

InventarViewerApp/
├── API/                      # hosted viewer API
├── Controllers/              # viewer HTTP/API surface
└── UI/                       # Terminal.Gui user surface

CtrlWorkerCommon/
CtrlWorkerServiceApp/
CtrlWorkerServiceCmdlet/
CtrlWorkerServicePS/
ServiceStatusReaderApp/

InventarWorkerCommonTest/
CtrlWorkerCommonTest/
InventarWorkerServiceIntegrationTest/

.github/workflows/            # build, test, docs/A11Y, secret and release evidence
docs/architecture/            # arc42/iSAQB evidence
docs/security/                # security and supply-chain evidence
docs/accessibility/           # WCAG/text-first evidence
docs/secure-development/      # canonical baseline and checklists
```

### Planned evidence outputs

```text
docs/security/
├── README.md
├── threat-model.md
├── security-checklist.md
├── arc42-security.md
├── dependency-audit.md
├── security-quality-scenarios.md
├── asvs-verification.md
├── supply-chain-evidence.md
├── zero-trust-applicability.md
├── samm-assessment.md
├── msl-applicability.md
├── secure-coding-language-rules.md
├── cra-applicability.md
├── regulatory-applicability.md
├── adr/                      # only material security decisions
└── secure-development/
    └── 2026-08-30-secure-development-hardening/
        ├── README.md         # summary, findings, risks, closeout
        ├── assessment-records.json
        └── CL_01_...md through CL_12_...md

docs/architecture/
├── context-view.md
├── runtime-view.md
├── deployment-view.md
├── architecture-risks.md
└── quality-scenarios.md

docs/accessibility/
└── secure-development-hardening.md
```

The twelve project-instance files use the canonical checklist basenames exactly:

```text
CL_01_Standards-Anwendbarkeit.md
CL_02_Sichere-Softwarearchitektur.md
CL_03_Krypto-Mindestvorgaben.md
CL_04_Bedrohungsmodellierung.md
CL_05_Lieferkette-Build-Integritaet.md
CL_06_Schwachstellenoffenlegung.md
CL_07_CRA-Anwendbarkeit.md
CL_08_Sicherheits-Code-Review.md
CL_09_KI-Codeerzeugung.md
CL_10_Sichere-Entwicklungsumgebung.md
CL_11_Datenschutz-Folgenabschaetzung.md
CL_12_Agentische-KI-Sandbox.md
```

**Structure Decision**: Reuse the current multi-project boundaries. Assessment records and evidence are documentation artefacts, not product domain models. Shared runtime hardening belongs in `InventarWorkerCommon`; HTTP-host decisions stay in their owning hosts; provider-specific evidence and tests stay scoped to the matching provider.

## Classification and Validator Contract

The five-state intake classification and the per-control assessment axes are deliberately separate:

| Layer | Allowed values | Required planning effect |
|---|---|---|
| Intake requirement | `Applicable`, `AlreadySatisfied`, `N/A`, `Open`, `FollowUp` | Only `Applicable` becomes implementation work. `AlreadySatisfied` needs current evidence but no duplicate implementation. `N/A` needs rationale, residual risk, and trigger. `Open` needs owner, risk, next action, and due date before it can become plannable. `FollowUp` stays outside Feature 002 with owner and trigger. |
| Canonical CL control | Applicability: `Applicable`, `N/A`, `Open`; implementation: `Fulfilled`, `Partly Fulfilled`, `Not Fulfilled`, `Not Assessed` | Exactly 157 records map one-to-one to the canonical IDs. Positive status requires current evidence; non-positive status requires the complete finding or N/A fields defined in the contract. |

The later Tasks phase MUST name and create every not-yet-existing path below before first use. Existing paths are consumed in place.

| Purpose | Existing path or later task-created path | Deterministic proof |
|---|---|---|
| Five intake states, exact twelve installed preset manifests, baseline, JSON contract, 157-record collection, references, freshness, exact-head binding, twelve Markdown instances, architecture/regulatory required fields | Create `InventarWorkerCommonTest/SecureDevelopmentEvidenceContractTest.cs` | Positive fixtures for every intake disposition plus `Applicable/Fulfilled` and `N/A/Not Assessed`; negative fixtures for an illegal/missing intake state, preset drift, missing/stale/stub-only evidence, duplicate/missing/extra IDs, broken references, absent runner/head/freshness fields, illegal status combinations, missing bilingual text, and incomplete architecture/regulatory fields. |
| Worker API security | Extend `InventarWorkerServiceIntegrationTest/InventarWorkerServiceIntegrationTests.cs`; create `InventarWorkerServiceIntegrationTest/SecurityBoundaryIntegrationTests.cs` only when separation improves ownership | Focused Playwright/MSTest positive and negative cases for every evidenced Worker endpoint; task-added `.github/workflows/ci.yml` job `security-integration` starts the exact host on `ubuntu-22.04`. |
| Viewer API security | Create `InventarWorkerServiceIntegrationTest/ViewerApiSecurityIntegrationTests.cs` and the minimal host fixture in the same project | Focused positive and negative cases for every evidenced Viewer endpoint; the same `security-integration` job starts and stops the Viewer host explicitly. |
| SQLite/PostgreSQL/MongoDB provider security | Extend `InventarWorkerCommonTest/SqliteDbServiceTest.cs` and `PgSqlDbServiceTest.cs`; create `InventarWorkerCommonTest/MongoDbServiceTest.cs` if MongoDB findings require executable proof | SQLite runs in the three-platform `build-test` matrix. Task-added `.github/workflows/ci.yml` job `provider-security` uses pinned local PostgreSQL and MongoDB service containers on `ubuntu-22.04`; provider-scoped parameterization, boundary, least-privilege/configuration, and failure tests may prove only those environments. |
| Agent parity | Existing `scripts/tests/test_spec_kit_agent_surface_parity.py`, `scripts/check-homogeneity.ps1`, and `.github/workflows/homogeneity-check.yml` | Existing checks plus a later workflow task that invokes the Python parity test on the candidate head. |
| SBOM toolchain | Existing `dotnet-tools.json`, later task-updated after latest-stable/CVE/licence review | Add the audited `Microsoft.Sbom.DotNetTool` pin before first use; `dotnet tool restore`, `sbom-tool generate`, and `sbom-tool validate` must resolve from that committed manifest. |
| Exact-head autonomous proof | Existing validators under `.specify/presets/autonomous-run-governance/scripts/` | Schema-2.0 temporary PreMerge and PostMerge evidence; accepted requirements hash and full candidate SHA must match. |

One contract test class owns the baseline and evidence-document loops. `G-TRACE-001` runs only its baseline methods before assessment; `G-ARCH-001`, `G-REGULATORY-001`, and `G-SEC-001` run their non-overlapping filtered methods. `G-BUILD-001` later runs the full regression suite once. A gate may repeat an earlier method only when its input hash changed; otherwise it reuses the exact-head evidence record. This removes duplicate ad-hoc `rg` and inline parsing loops while preserving a final full-suite regression.

## Shared-Write Serialization

Tasks MUST form one dependency chain for shared writes in this order: baseline/current-governance repair; evidence contract and project instances; code/test findings; workflows and tool manifest; agent/constitution/template parity; statistics; version/candidate preparation; temporary PreMerge evidence; merge; PostMerge evidence and coordinator closeout. A task may run in parallel only when its write set is disjoint from every active task. In particular, no parallel task may write `Directory.Build.props`, `.github/workflows/*`, `docs/project-statistics.md`, shared security/architecture indexes, the five agent surfaces, either constitution, or coordinator-owned autonomous evidence/state.

*Shared writes are serialized by dependency, while read-only reviews may remain parallel. This prevents lost updates and evidence built from mixed repository states.*

## Design and Implementation Phases

### Phase 0 — Research and fail-closed inventory

1. Revalidate feature identity, accepted hashes, branch, authority, and exact installed preset registry.
2. Check manifest, checklist versions, paths, and all 157 IDs before project assessment. Record any drift as `Open`; do not select a more favourable source.
3. Inventory every endpoint, trust boundary, secret/configuration source, outgoing destination, provider, release artefact, workflow, public document, and maintained agent surface.
4. Establish the assessment-record contract from `contracts/classification-evidence-record.schema.json` and create `InventarWorkerCommonTest/SecureDevelopmentEvidenceContractTest.cs` with deterministic accepting and rejecting fixtures before repairing baseline drift.

### Phase 1 — Assessment and architecture evidence

1. Create exactly one complete record for every stable CL ID and complete all twelve project-instance documents.
2. Populate the threat model with CIA, STRIDE, and CAPEC; update arc42/iSAQB context, runtime, deployment, risks, and quality scenarios.
3. Complete ASVS Level 2 for both full HTTP/API surfaces, including Swagger/OpenAPI and static documentation routes.
4. Complete SSDF, CWE, MSL, secure-language, dependency, supply-chain, CRA/regulatory, Zero Trust, SAMM, and A11Y evidence.
5. Convert every missing, stale, conflicting, or stub-only proof into a finding with owner, reviewer, risk, priority, action, due date, and trigger.

### Phase 2 — Red-Green-Refactor hardening

1. For each applicable code finding, add a focused failing unit/integration/negative test first.
2. Implement the smallest secure change at the owning layer. Preserve platform and provider behavior unless the accepted finding requires a narrow change.
3. Refactor after the focused test passes; keep public XML documentation bilingual and review didactic comments for non-trivial logic.
4. Remove `Newtonsoft.Json` package usage from every product project and keep `System.Text.Json` camelCase behavior covered by tests.
5. Fix .NET-10 CI alignment; add the declared three-platform `build-test` matrix plus `security-integration`, `provider-security`, and `supply-chain` jobs to `.github/workflows/ci.yml`; and repair only the authoritative governance drift proven by the inventory. Pin service-container/tool versions and record latest-stable/CVE/licence decisions before use.

### Phase 3 — Verification and closeout

1. Apply versioning before every build/test and before every commit as described below.
2. Execute all Applicable gates at the exact candidate head; record actual command, runner/platform, result, evidence reference, reviewer, and integrity hash.
3. Update navigation, security index, A11Y evidence, changelog if required by implementation, and project statistics.
4. Leave every unresolved issue visible and fail release when a mandatory technical gate lacks proof.

## Versioning Discipline

At the start of implementation, compute the feature commit count relative to `main`:

```bash
git rev-list --count "$(git merge-base HEAD main)..HEAD"
```

- `Major` remains the current repository major.
- `Minor` is `2` immediately on this numbered branch.
- Before preparing each commit, set `Patch` to `current feature count + 1`; after the commit, verify the stored patch equals the new count.
- Before every individual `dotnet build` or `dotnet test`, increment `Build` exactly once and keep all three version fields aligned.
- Restore, documentation, scanner, and read-only list commands do not increment `Build` unless they invoke build/test internally.
- The first feature commit is therefore patch `1`; the first build/test after current build counter `28` uses build `29`. Later values derive from the actual sequence rather than being guessed in tasks.
- The first serialized implementation task aligns the intended first commit to `1.2.1.28` before any agent-issued build/test or commit. Every coordinator/agent-issued `dotnet build` or `dotnet test` then increments `Build` once immediately before that command, including failing commands. Provider CI consumes the committed candidate version and does not write a new counter back to the branch.
- `Directory.Build.props` has exactly one writer. After the final authorized version/candidate commit, no tracked mutation is allowed before temporary exact-head PreMerge evidence is captured. Provider jobs and local read-only validators bind their proof to that committed SHA; self-invalidating PreMerge/PostMerge facts stay in the pre-named runtime evidence files below rather than being committed before merge.

## Gate Requirements Before Implementation

The machine-readable source is `autonomous-run-gate-requirements.json`. The following table explains the stable gates and their exact proof path. Commands shown here are execution requirements for later phases and were not run during planning.

| Gate ID | Applicability | Required command or validator scope | Pass condition |
|---|---|---|---|
| `G-SPEC-001` | Applicable | Validate accepted phase results and accepted hashes with `validate-autonomous-phase-result.ps1` and SHA-256 checks. | Accepted spec/checklists and phase handoffs are valid and unchanged. |
| `G-TRACE-001` | Applicable | `git branch --show-current`; `specify preset list`; filtered intake/preset/baseline methods in task-created `InventarWorkerCommonTest/SecureDevelopmentEvidenceContractTest.cs`. | Correct branch; five intake states are fully disposed; exact twelve enabled manifests; exactly twelve canonical files and 157 unique IDs; versions/paths consistent. Baseline drift blocks assessment rather than passing as an Open exception. |
| `G-ARCH-001` | Applicable | Filtered architecture methods in task-created `InventarWorkerCommonTest/SecureDevelopmentEvidenceContractTest.cs` plus reviewer evidence. | All assets, trust boundaries, CIA/STRIDE/CAPEC risks, independent controls, residual risks, architecture views, and significant decisions are complete; negative incomplete fixtures are rejected. |
| `G-SEC-001` | Applicable | Task-added `.github/workflows/ci.yml` jobs `security-integration` and `provider-security`; focused tests in the exact unit/integration files declared above; filtered evidence-contract methods and manual review. | NIST SSDF, CWE Top 25, ASVS L2, STRIDE/CAPEC, Zero Trust, SAMM, database/HTTP/file/config/process controls, and all positive/negative cases have complete proof on explicitly named environments. |
| `G-BUILD-001` | Applicable | Updated `.github/workflows/ci.yml` job `build-test` on `ubuntu-22.04`, `macos-14`, and `windows-2022`, plus its explicit integration/coverage commands. | .NET 10 on declared platforms; all scoped tests pass; coverage >=70%, target >=80%; provider claims match executed environments. |
| `G-A11Y-001` | Applicable | Existing `.github/workflows/docs-pages.yml` job `build-docs`: `docfx`, post-processor, Lynx, Axe, and manual bilingual/text-first review. | No applicable WCAG 2.2 AA failure; all decisions and statuses remain available as text; generated output stays untracked. |
| `G-SUPPLY-001` | Applicable | Task-added `.github/workflows/ci.yml` job `supply-chain`; existing `Gitleaks / gitleaks` and `Agent Secret Scan / scan-agent-secrets` jobs. | Every distributable artefact set has bound machine-readable SBOM/provenance; every known finding has disposition; missing evidence blocks release. |
| `G-AGENT-001` | Applicable | Existing `.github/workflows/homogeneity-check.yml` job `check`, extended by a later task to run `scripts/tests/test_spec_kit_agent_surface_parity.py`; `specify preset list`. | Five maintained agent files, both constitutions, affected Spec-Kit/template sources, and installed registry contain no conflicting current authority; historical prose stays historical. |
| `G-REGULATORY-001` | Applicable | Filtered regulatory methods in task-created `InventarWorkerCommonTest/SecureDevelopmentEvidenceContractTest.cs` plus reviewer evidence. | CRA screening is complete; every N/A decision contains current assumptions, residual risk, owner/reviewer, and re-evaluation trigger; incomplete negative fixtures fail. |
| `G-CLOSE-001` | Applicable, coordinator-owned | Existing gate-evidence, delivery-set, and run-state validators; staged-diff check; exact provider job logs; PR merge; default-branch fast-forward sync. | Schema-2.0 PreMerge proof covers every Applicable gate at one candidate SHA; MergeAndSync finishes; PostMerge proof binds the PreMerge hash and merge SHA; local `main` matches remote; final validation passes. |
| `G-AI-SBOM-001` | N/A | No command. | AI remains development tooling only. Re-evaluate when an AI model/service/dataset/inference/runtime enters product or operations. |
| `G-CLOUD-001` | N/A | No command. | No cloud/managed provider dependency. Re-evaluate on SaaS/PaaS/IaaS/provider-dependent deployment or assurance need. |
| `G-SCRIPT-001` | N/A | No command. | No script is added/changed. Re-evaluate immediately if implementation selects a script-shaped change. |
| `G-GENERAL-ADR-001` | N/A | No command. | No non-security structural decision is currently selected. Re-evaluate on component/interface/deployment restructuring. |

## MergeAndSync Delivery Contract

Delivery remains owned by the outer autonomous coordinator. The exact path is:

- Temporary PreMerge evidence: `.specify/runtime/autonomous-routing/d6d5e58e-0acc-404c-b5b4-4f97eba83c9f/pre-merge-gate-evidence.json`.
- Temporary causal PostMerge evidence: `.specify/runtime/autonomous-routing/d6d5e58e-0acc-404c-b5b4-4f97eba83c9f/post-merge-gate-evidence.json`.
- Both use schema 2.0 and remain untracked. The installed gate-evidence validator binds the accepted requirements hash, full head, Primary/Supplemental rows, executed command, workflow/job, runner/platform, timestamps, results, and integrity. The coordinator records their validated hashes in its closeout/state transition; this phase never edits that state.

1. Revalidate branch, accepted hashes, scope, authority, clean staging boundary, gate requirements, and candidate-head identity.
2. Commit only intended feature changes with aligned `1.2.Patch.Build` values; optional Spec-Kit auto-commit hooks remain disabled and MUST NOT create commits.
3. Push the feature branch and open/update a PR targeting `main` with purpose, touched projects, executed commands, coverage, security/A11Y/docs impact, and risk evidence.
4. Wait for all technically required workflows and reviews; use the mapping above and derive actual commands/runners from workflow definitions or exact job logs, never from a green aggregate name.
5. Merge only after technical proof passes. The user authorizes Admin-Bypass solely to overcome provider policy after that proof is complete. It MUST NOT replace a failed/missing test, review, security, coverage, A11Y, supply-chain, or exact-head gate.
6. If provider policy still blocks, record provider refusal separately; use the authorized admin path only for that policy condition. Never suppress or relabel technical failure as provider policy.
7. After merge, switch to `main`, run `git pull --ff-only`, verify local/remote head equality, execute declared post-merge validation, and only then allow coordinator closeout/state transitions.
8. Keep state changes coordinator-owned. This plan phase does not edit `autonomous-run-state.json`.

## Complexity Tracking

No Constitution violation or architecture exception is planned. Any later finding that requires a new project, provider, script framework, cloud dependency, or non-security structural ADR exceeds this plan and requires explicit re-planning before implementation.
