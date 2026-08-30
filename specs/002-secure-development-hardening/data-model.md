# Data Model: Security Classification and Evidence

**Feature**: `002-secure-development-hardening`

**Contract**: `contracts/classification-evidence-record.schema.json`

## Purpose

Dieses Datenmodell beschreibt die auditfähige Bewertung der 157 stabilen Secure-Development-Prüfpunkte. Es ist ein Evidenzmodell für Repository-Dokumentation und Tests, kein neues Produkt- oder Datenbankmodell.

*This model describes the audit-ready assessment of the 157 stable secure-development checkpoints. It is an evidence model for repository documentation and tests, not a new product or database model.*

## 0. Two Classification Layers / Zwei Klassifikationsebenen

Die fünf Intake-Zustände und die Prüfpunktbewertung sind getrennte Verträge. Sie dürfen nicht in ein gemeinsames Enum zusammengeführt werden.

*The five intake states and the checkpoint assessment are separate contracts and must not be collapsed into one enum.*

| Layer | Allowed values | Required fields and effect |
|---|---|---|
| Intake requirement | `Applicable`, `AlreadySatisfied`, `N/A`, `Open`, `FollowUp` | `Applicable`: plan/task/evidence path. `AlreadySatisfied`: current evidence and reviewer, no duplicate implementation. `N/A`: rationale, residual risk, owner/reviewer, and trigger. `Open`: owner/reviewer, risk, action, priority, due date, and trigger before planning. `FollowUp`: outside-feature rationale, owner, target or trigger, and traceable destination. |
| Canonical CL control | `Applicable`, `N/A`, `Open` plus `Fulfilled`, `Partly Fulfilled`, `Not Fulfilled`, `Not Assessed` | Exactly one record per stable control under the schema below. Only `Applicable/Fulfilled` is a positive result, and it requires current evidence. |

Only the second layer appears in `assessment-records.json`. The accepted intake classification remains authoritative in `spec.md`; the later Tasks phase selects only its `Applicable` rows and preserves the disposition of the other four states.

## 1. Assessment Record / Prüfpunktbewertung

Jede stabile ID `CL-01-01` bis zum letzten kanonischen Prüfpunkt besitzt genau einen Datensatz.

*Every stable ID has exactly one record.*

| Field | Type | Required | Rule |
|---|---|---:|---|
| `schemaVersion` | string | yes | Exactly `1.0`. |
| `recordId` | string | yes | Stable local identifier, equal to `checkpointId` for this feature. |
| `checkpointId` | string | yes | Pattern `CL-[01][0-9]-[0-9]{2}`; must exist in one canonical checklist. |
| `checklistId` | string | yes | `CL-01` through `CL-12`; must match the checkpoint prefix. |
| `baselineVersion` | string | yes | Version validated from the baseline manifest at assessment time. |
| `checklistVersion` | string | yes | Version validated from the canonical checklist at assessment time. |
| `learningStage` | string | yes | Existing checklist value; preserved for learner traceability. |
| `applicability` | enum | yes | Exactly one of `Applicable`, `N/A`, `Open`. |
| `implementationStatus` | enum | yes | Exactly one of `Fulfilled`, `Partly Fulfilled`, `Not Fulfilled`, `Not Assessed`. |
| `role` | string | yes | Role responsible for assessing/acting on the checkpoint. |
| `owner` | string | yes | Accountable project role; not a mutable provider identity. |
| `reviewer` | string | yes | Independent or designated reviewing role. |
| `rationale` | string | yes | Concrete DE-first/EN-second reasoning; no empty template text. |
| `evidence` | Evidence Record[] | yes | May be empty only when the status contract permits no positive claim. |
| `residualRisk` | string | yes | Remaining risk or explicit `None`/`Keines` with rationale. |
| `nextAction` | string | yes | Concrete action, or no-action-until-trigger statement for N/A. |
| `dueDate` | date or `N/A` | yes | ISO `YYYY-MM-DD` for actionable Open/Not Fulfilled/Partly Fulfilled records. |
| `reevaluationTrigger` | string | yes | Concrete event that invalidates or reopens the decision. |
| `findingIds` | string[] | yes | Links to zero or more findings. |
| `updatedAt` | date-time | yes | UTC evidence decision time. |

### Classification invariants

1. `applicability` and `implementationStatus` are separate and single-valued.
2. `Fulfilled` is allowed only with `Applicable` and at least one current Evidence Record.
3. `N/A` uses `Not Assessed`, no positive claim, a factual rationale, residual risk, no-action-until-trigger statement, and a concrete trigger.
4. `Open` cannot be `Fulfilled`; it requires owner, reviewer, risk, action, ISO due date, and trigger.
5. `Partly Fulfilled` and `Not Fulfilled` require a finding and an ISO due date.
6. Missing, conflicting, stale, stub-only, or non-reproducible evidence cannot support `Fulfilled`.
7. A record becomes stale when any declared invalidation trigger occurs or its review cadence expires.

*The schema enforces the structural part of these rules. Collection-level and repository-current checks are enforced by deterministic tests and reviewer evidence.*

## 2. Evidence Record / Evidenzdatensatz

An Evidence Record proves a defined part of one or more assessment records.

| Field | Type | Required | Rule |
|---|---|---:|---|
| `evidenceId` | string | yes | Stable `EV-...` identifier within the feature evidence set. |
| `kind` | enum | yes | `CommandOutput`, `TestResult`, `SourceReview`, `ConfigurationReview`, `ArchitectureRecord`, `CIRecord`, `ProviderRecord`, `ReleaseArtifact`, or `RiskAcceptance`. |
| `source` | string | yes | Repository-relative file, workflow/job, release asset, or authoritative external reference. |
| `command` | string | no | Exact executed command when the evidence is executable. Never claim an unexecuted command. |
| `scope` | string | yes | Exact endpoint, project, provider, platform, file, control range, or artefact set proven. |
| `observedAt` | date-time | yes | UTC execution/review time. |
| `reviewer` | string | yes | Role that checked the result. |
| `runnerOrPlatform` | string | yes | Concrete local platform or exact CI workflow/job/runner; never infer it from a green display name. |
| `candidateGitSha` | string | yes | Full normalized lowercase 40- or 64-character Git object ID for the reviewed candidate. |
| `result` | enum | yes | `Pass`, `Fail`, `Partial`, or `Informational`. |
| `evidenceReference` | string | yes | Durable repository, CI, PR, release, or security-record reference. |
| `integrity` | object | yes | `sha256` for files/output or `git-sha` for exact-head evidence, with normalized lowercase value. |
| `validUntil` | date-time or `N/A` | yes | Expiry derived from the declared cadence. |
| `freshnessBasis` | string | yes | Explains the time-based cadence or why event-driven invalidation with `validUntil: N/A` is sufficient. |
| `invalidationTriggers` | string[] | yes | At least one concrete scope/code/dependency/config/CI/standard/baseline trigger. |

### Evidence invariants

- `Pass` proves only the declared `scope`.
- Provider evidence never implies parity for a provider not named in scope.
- A source-review `Pass` must identify files and exact Git SHA or SHA-256.
- A command-based `Pass` must contain the exact command, runner/platform, timestamp, result, and integrity reference.
- Every evidence record binds the full candidate Git SHA. `validUntil: N/A` is allowed only with an explicit event-driven freshness basis and concrete invalidation triggers.
- Generated artefacts are evidence only when bound to source, version/hash, generation command, and distribution location.
- A risk acceptance is not a technical pass. It records authority, scope, compensating controls, expiry, and trigger.

## 3. Finding / Befund

A Finding represents an evidenced gap or unresolved contradiction.

| Field | Type | Required | Rule |
|---|---|---:|---|
| `findingId` | string | yes | Stable `FIND-###` identifier. |
| `title` | string | yes | DE-first/EN-second short description. |
| `checkpointIds` | string[] | yes | At least one affected stable CL ID. |
| `boundary` | string | yes | Affected trust boundary or governance/release boundary. |
| `severity` | enum | yes | `Critical`, `High`, `Medium`, `Low`, or `Informational`. |
| `likelihood` | enum | yes | `High`, `Medium`, or `Low`. |
| `impact` | string | yes | Concrete confidentiality, integrity, availability, safety, or compliance effect. |
| `status` | enum | yes | `Open`, `In Progress`, `Mitigated`, `Accepted`, or `Closed`. |
| `owner` | string | yes | Accountable role. |
| `reviewer` | string | yes | Reviewing role. |
| `priority` | enum | yes | `P0`, `P1`, `P2`, or `P3`. |
| `action` | string | yes | Minimal correction or evidence action. |
| `dueDate` | date | yes | Required while status is not `Closed`. |
| `residualRisk` | string | yes | Risk after planned/current controls. |
| `reevaluationTrigger` | string | yes | Event requiring renewed decision. |
| `evidenceIds` | string[] | yes | Proof for the finding and its disposition. |

### Finding transitions

```text
Open -> In Progress -> Mitigated -> Closed
  |                         |
  +-----------------------> Accepted
```

Transitions are not visual-only: `Open`, `In Progress`, `Mitigated`, `Accepted`, and `Closed` are always written as text in the record.

- `Closed` requires current passing evidence at the relevant gate.
- `Accepted` requires authorized risk-acceptance evidence, compensating controls, expiry, and trigger.
- New or stale contradictory evidence returns a record to `Open`.

## 4. Trust Boundary / Vertrauensgrenze

| Field | Meaning |
|---|---|
| `boundaryId` | Stable `TB-##` identifier. |
| `source` / `target` | Actors, hosts, processes, stores, CI, or agent environment. |
| `dataClasses` | Public, internal inventory/status/log data, confidential credentials/connection strings, or stricter CIA class. |
| `entryPoints` | Endpoints, files, environment variables, DB clients, process calls, or repository actions. |
| `identityAndAuthority` | How caller identity and allowed action are established. |
| `validation` | Format/range/length/allow-list and serialization controls. |
| `controls` | Independent preventive/detective/recovery layers. |
| `threats` | STRIDE categories and relevant CAPEC patterns. |
| `residualRisk` | Remaining risk and decision owner. |

## 5. Release Artefact Set / Release-Artefaktsatz

| Field | Meaning |
|---|---|
| `artefactSetId` | Unique release/CI set identifier. |
| `version` | Aligned `Major.Minor.Patch.Build`. |
| `sourceGitSha` | Exact candidate head. |
| `artefacts` | Distributed files with normalized SHA-256. |
| `sbom` | CycloneDX/SPDX format, generator version, path/reference, hash, validation result. |
| `provenance` | Builder, source, steps, SLSA target/current state, integrity result. |
| `vulnerabilitySnapshot` | Tool/source, timestamp, direct/transitive scope, findings. |
| `vexDisposition` | Required for each known finding; otherwise a current no-known-finding decision. |
| `releaseDecision` | Pass, Blocked, or Accepted Risk with authority and evidence. |

## 6. Risk and Follow-up Entry / Risiko- und Folgeeintrag

| Field | Meaning |
|---|---|
| `entryId` | Stable identifier. |
| `type` | `Open Risk`, `Accepted Risk`, or `FollowUp`. |
| `impact` / `likelihood` | Explicit risk basis. |
| `priority` | P0-P3. |
| `owner` / `reviewer` | Accountable and review roles. |
| `decision` | Action, acceptance, or deferral rationale. |
| `dueDate` | Required date. |
| `trigger` | Re-evaluation event. |
| `evidence` | Supporting record IDs and paths. |

## 7. Collection invariants

The implementation evidence set MUST satisfy all rules below:

1. Exactly twelve canonical checklist project-instance documents exist.
2. Exactly 157 assessment records exist after the baseline consistency gate passes.
3. `checkpointId` values are unique and equal the canonical ID set; no extra or missing ID is allowed.
4. Checklist and baseline versions match the validated canonical sources.
5. Every `findingId` and `evidenceId` reference resolves exactly once.
6. Every positive assessment resolves to at least one current Evidence Record whose scope covers the claim.
7. Every CL `Open`/`N/A` decision and every intake `N/A`/`Open`/`FollowUp` disposition contains the fields required by its separate classification layer.
8. Every security standard from the plan maps to at least one assessment record and one canonical evidence path.
9. All user-facing prose is DE-first/EN-second CEFR B2; machine keys and enum values remain stable English contract values.
10. The exact candidate Git SHA and normalized lowercase SHA-256 values bind final evidence.
11. Deterministic positive fixtures are accepted and negative fixtures for stale/stub-only evidence, illegal status combinations, missing fields, broken references, and ID-set drift are rejected by `InventarWorkerCommonTest/SecureDevelopmentEvidenceContractTest.cs` after that later task creates it.

## 8. Persistence and publication

- Machine-readable collection: `docs/security/secure-development/2026-08-30-secure-development-hardening/assessment-records.json`.
- Human-readable project instances: the twelve `CL_*.md` files in the same directory.
- Findings, risks, and closeout: the directory `README.md` and referenced canonical security documents.
- Generated SBOM, scan, coverage, and DocFX outputs are CI/release artefacts unless a source document explicitly declares a versioned evidence file. They are not committed merely because they exist locally.

*The JSON collection supports deterministic completeness checks. The Markdown project instances remain the learner- and reviewer-facing evidence. Generated output stays outside version control unless a specific canonical source decision says otherwise.*
