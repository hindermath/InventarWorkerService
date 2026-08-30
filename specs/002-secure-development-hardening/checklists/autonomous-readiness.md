# Autonomous Run Readiness Checklist

**Feature**: `002-secure-development-hardening`
**Phase**: Tasks generation / Task-Erzeugung
**Evidence date**: 2026-08-30
**Rule**: Nur bereits zum Zeitpunkt der Task-Erzeugung bewiesene Fakten sind markiert. / *Only facts proven at task-generation time are checked.*

## Authority and Scope

- [x] Intake review is `N/A`, or active policy has exactly one current
      `Ready`/human-approved `ReadyWithAcceptedRisks` result.

- [x] Exactly one delivery mode is recorded from current authority.
- [x] Missing remote authority defaults to `LocalImplementation`.
- [x] Accepted scope and explicit exclusions are unchanged.
- [x] Repository constitution, agent guidance, and feature identity agree.
- [x] The feature-local run state validates and agrees with branch, feature
      metadata, checkpoint history, accepted artifacts, tasks, and evidence.
- [ ] `PausedByUser` requires explicit resume; unexpected interruption and any
      uncertain operation have a documented revalidation boundary.

## Artifact Convergence

- [x] Clarify has no remaining material planning ambiguity.
- [x] Requirements and plan-review checklists pass or have accepted dispositions.
- [x] Tasks are dependency-ordered and name exact evidence paths for delivery.
- [x] After preset or governance drift, current mandatory correctness, security,
      permission, and evidence-integrity rules were compared with accepted Plan,
      Tasks, and checklists; applicable missing rules were minimally added and
      efficiency-only preferences did not rewrite accepted artifacts.
- [ ] Analyze has no Critical/High finding; Medium findings are resolved or owned.
- [ ] Every implementation task is complete or conditionally evidenced.

## Proof and Validation

- [ ] The explicit delivery set includes every intended untracked file and no
      unrelated or ignored runtime evidence.
- [x] Every routed phase has a valid structured semantic result; exit zero is
      not treated as sufficient completion evidence.
- [ ] New merge decisions use schema-2.0 `PreMerge` evidence; schema 1.0 is
      historical audit evidence only.

- [x] Evidence existed before the first implementation edit.
- [x] One representative vertical slice has failing and green proof.
- [x] Negative matrices preserve each expected failure and ownership boundary.
- [x] Shared writers were serialized.
- [x] Every mutable validation-token transition maps to one explicit invocation.
- [x] Helpers received an explicit repository root.
- [x] Exit status, required output, and structured/error channels were inspected.
- [x] Changed documentation, evidence, schemas, and status markers were searched
      for executable validator consumers before any test gate was skipped.
- [ ] The exact intended delivery candidate passed `git diff --cached --check`
      or an equivalent non-mutating local-only check.
- [ ] Staged paths were reconciled with untracked and unstaged repository state;
      unrelated work and any prior local-only index state were preserved.
- [ ] Triggered validation passed; skipped gates have an explicit rationale.
- [x] Every acceptance gate was declared before implementation in the reviewed
      gate-requirements artifact with stable ID, scope, and required tokens.

## Remote Delivery

- [x] Remote tasks exist only for the authorized delivery mode.
- [ ] Required review-context checks pass.
- [ ] Every acceptance-specific gate maps to the workflow, job, runner or
      platform, and command that actually executed the required proof.
- [ ] Temporary provider evidence matches the accepted requirements hash and
      exact current reviewed head, and the installed validator passes.
- [ ] Every declared gate has exactly one Primary row; Supplemental evidence
      points to it, and `N/A` includes rationale plus re-evaluation trigger.
- [ ] Executed commands and runners were read from workflow definitions or logs,
      not inferred from green aggregate, workflow, job, or platform-shaped names.
- [ ] Exact-head provider evidence was not committed before merge and therefore
      did not invalidate its own reviewed-head claim.
- [ ] No green aggregate or platform-named tooling job is credited for
      acceptance scope that it did not execute.
- [ ] No actionable review thread remains.
- [ ] Unavailable reviews are recorded as missing, not successful.
- [ ] Duplicate event runs are classified without unauthorized cancellation.
- [ ] Any bypass has separate explicit authority and repository-policy evidence.
- [ ] A causal closeout, if required, was pre-named and is single-commit-capable.
- [ ] No empty feature, retrospective, or closeout pull request is proposed.
- [ ] Merge, cleanup, and default-branch synchronization are proven when required.
- [ ] Schema-1.1 closeout records merge/publication, default-branch sync,
      manifest-declared post-merge actions, and final validation independently.
- [ ] `Completed` is set only after every applicable closeout field is terminal.

## Learning and Finish

- [x] Resume state and the next exact action are recorded.
- [ ] A graceful stop, if requested, preserved work at a safe boundary and did
      not infer commit, push, rollback, merge, or process-kill authority.
- [x] Out-of-scope findings have owner, evidence path, and re-evaluation trigger.
- [ ] Retrospective decisions separate portable rules from project specifics.
- [ ] No empty retrospective branch or pull request was created.
- [x] The next feature was not started implicitly.

## Implementierungsgrenze / Implementation Boundary

DE: Der Implementierungslauf bestätigte Darwin, PowerShell 7, Branch
`002-secure-development-hardening`, Head und Merge-Base
`dc15bc4812245e71c1a5976b8241c7aeb518d4a9`. Der Run-State und alle sechs
gerouteten Vorphasenergebnisse waren gültig; elf Accepted-Artifact-Hashes
stimmten. Nur der Secure-Development-Intake ist im Scope. Ein Parallel-
Campaign-Manifest, Worker-Branch oder zweiter Intake wurde nicht erzeugt;
dieses nicht autorisierte Szenario ist `N/A` und wird bei neuer ausdrücklicher
Parallel-Autorität erneut bewertet.

EN: The implementation run confirmed Darwin, PowerShell 7, branch
`002-secure-development-hardening`, head, and merge base
`dc15bc4812245e71c1a5976b8241c7aeb518d4a9`. Run state and all six routed prior
phase results were valid; eleven accepted-artifact hashes matched. Only the
secure-development intake is in scope. No parallel campaign manifest, worker
branch, or second intake was created; this unauthorised scenario is `N/A` and
is reassessed only after new explicit parallel authority.

DE: Der vollständige lokale Build war grün. Der abschließende Coverage-Lauf
bestand 80 Tests, übersprang 32 explizit provider-/plattformabhängige Tests und
meldete nur 43,0 Prozent Zeilenabdeckung. Damit bleiben „Every implementation
task is complete“, „Triggered validation passed“ und alle Closeout-Aussagen
bewusst offen. Historische Gitleaks-Funde sowie der sandboxbedingt fehlende
Axe-Lauf sind mit Owner, Termin und Trigger dokumentiert.

EN: The complete local build passed. The final coverage run passed 80 tests,
explicitly skipped 32 provider/platform-dependent tests, and reported only
43.0 percent line coverage. Therefore, “Every implementation task is
complete”, “Triggered validation passed”, and all closeout claims intentionally
remain open. Historical Gitleaks findings and the sandbox-blocked Axe run have
an owner, due date, and trigger.

## Task-Generation Evidence / Nachweis der Task-Erzeugung

- `specs/002-secure-development-hardening/autonomous-run-state.json` passed the installed read-only state validator while stage `Tasks` was active.
- All eleven accepted-artifact SHA-256 values matched the state at task-generation time.
- `requirements.md`, `security.md`, and `plan.md` were complete; plan review recorded 36/36 passed items.
- Earlier phase-result files that no longer bind the latest shared payload remain intentionally insufficient for the unchecked routed-phase item; T002 requires coordinator-owned revalidation rather than a false pass.
- No implementation, build, test, source/governance/intake mutation, commit, push, PR, merge, or run-state edit was performed while generating these tasks.
