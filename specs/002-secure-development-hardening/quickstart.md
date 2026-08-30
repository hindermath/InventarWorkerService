# Quickstart: Secure-Development-Hardening Implementation

Diese Anleitung ist für die nachgelagerten Tasks- und Implementierungsphasen. In der Planungsphase wurden die Befehle nicht ausgeführt. Das bereits einmal ausgeführte `setup-plan.ps1` darf nicht erneut ausgeführt werden.

*This guide is for the later tasks and implementation phases. The commands were not run during planning. The already completed one-time `setup-plan.ps1` must not be run again.*

## 1. Revalidate identity and authority

```bash
uname -s
git branch --show-current
git status --short --branch
git rev-parse HEAD
git merge-base HEAD main
shasum -a 256 \
  .specify/feature.json \
  specs/002-secure-development-hardening/spec.md \
  specs/002-secure-development-hardening/checklists/requirements.md \
  specs/002-secure-development-hardening/checklists/security.md
```

Expected branch: `002-secure-development-hardening`. Compare the hashes with `acceptedArtifacts` in `autonomous-run-state.json`. Do not edit that state file; only the outer coordinator owns it.

*Stop on identity, hash, authority, or scope drift. An autonomous resume after interruption requires full revalidation.*

## 2. Confirm hooks and installed preset authority

```bash
git config --get core.hooksPath || true
sed -n '1,220p' .specify/extensions/git/git-config.yml
specify preset list
jq -r '.presets | to_entries[] | select(.value.enabled == true) | [.value.priority, .key, .value.version] | @tsv' .specify/presets/.registry | sort -n
```

The optional Spec-Kit `auto_commit` entries are `false`. They MUST remain unable to create a commit. A repository pre-push check may validate work but does not grant commit, push, merge, or bypass authority.

*The installed twelve-preset registry is authoritative. Historical six-preset intake text remains historical.*

## 3. Run the baseline consistency gate before assessment

The first implementation task creates `InventarWorkerCommonTest/SecureDevelopmentEvidenceContractTest.cs`. Its baseline method is the single deterministic owner of manifest/checklist path, version, twelve-file, and 157-unique-ID validation. Apply the version/build-counter rule in section 6 before invoking it:

```bash
dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj \
  --filter "FullyQualifiedName~SecureDevelopmentEvidenceContractTest.BaselineManifest_CanonicalChecklists_ContainsExactly157UniqueControls"
```

Current research found a manifest/version mismatch for CL-09 and CL-12. The gate must fail until the authoritative baseline is minimally synchronized. Record any additional mismatch as a blocking `Open` finding; never choose whichever source gives the easier result.

*Project assessment starts only after this gate passes.*

## 4. Create and validate the assessment set

Populate:

- `assessment-records.json` with exactly 157 unique checkpoint records;
- twelve DE-first/EN-second project-instance Markdown files;
- complete evidence or complete Open/N/A fields for every record.

Validation rules come from:

```text
specs/002-secure-development-hardening/contracts/classification-evidence-record.schema.json
specs/002-secure-development-hardening/data-model.md
```

Required repository tests must prove exact ID-set equality, schema/status invariants, reference integrity, no empty mandatory fields, and no positive stub-only claim. Do not introduce a general script-shaped tool; prefer MSTest/System.Text.Json tests within the existing test boundary.

The same class contains explicit positive fixtures for `Applicable/Fulfilled` and `N/A/Not Assessed`, plus negative fixtures for missing/stale/stub-only evidence, illegal status combinations, broken IDs/references, missing `runnerOrPlatform`/`candidateGitSha`/`freshnessBasis`, and incomplete bilingual architecture/regulatory evidence. Filtered gates own disjoint methods; only the final full regression reruns the complete class unless an input hash changes.

*The machine-readable collection and human-readable project instances must agree.*

## 5. Apply Red-Green-Refactor per code finding

1. Add a focused failing test for the finding.
2. Confirm it fails for the intended security reason.
3. Implement the smallest secure correction at the owning layer.
4. Run the focused test after incrementing the Build counter.
5. Refactor and rerun focused plus full regression scopes.

*Evidence-only changes use deterministic validators. They do not need artificial code tests, but they cannot be called test-free when an executable consumer or validator is affected.*

## 6. Apply branch versioning before build, test, and commit

Get the current feature count:

```bash
git rev-list --count "$(git merge-base HEAD main)..HEAD"
```

Before the next commit, set `Patch` to that count plus one. Minor is always `2` on this branch. Before every individual command whose first two words are `dotnet build` or `dotnet test`, increment Build once. Keep these equal in `Directory.Build.props`:

```xml
<Version>Major.2.Patch.Build</Version>
<AssemblyVersion>Major.2.Patch.Build</AssemblyVersion>
<FileVersion>Major.2.Patch.Build</FileVersion>
```

Use `apply_patch` or a deliberate editor change; do not add a convenience script. After any later authorized commit, verify:

```bash
git rev-list --count "$(git merge-base HEAD main)..HEAD"
sed -n '1,20p' Directory.Build.props
```

*The stored Patch must equal the post-commit feature count. The Build counter records every build/test invocation, including failed ones.*

Only one task may write `Directory.Build.props`. The first intended feature commit is prepared as `1.2.1.28`; the first coordinator/agent-issued build or test uses Build `29`. Provider CI consumes the committed version without writing a new counter back to the branch. Serialize all other shared writers as defined in `plan.md`.

## 7. Restore, build, and test

Restore does not increment Build:

```bash
dotnet restore InventarWorkerService.sln
```

Increment Build, then build:

```bash
dotnet build InventarWorkerService.sln --configuration Release --no-restore
```

Increment Build before each test command:

```bash
dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj --configuration Release --no-build
dotnet test CtrlWorkerCommonTest/CtrlWorkerCommonTest.csproj --configuration Release --no-build
```

For the HTTP integration scope, first install Playwright after a build, then start InventarWorkerService at the configured integration URL in a separate process and run the integration project after another Build increment:

```bash
pwsh -NoProfile -File InventarWorkerServiceIntegrationTest/bin/Release/net10.0/playwright.ps1 install
dotnet run --project InventarWorkerService/InventarWorkerService.csproj --configuration Release --no-build
dotnet test InventarWorkerServiceIntegrationTest/InventarWorkerServiceIntegrationTest.csproj --configuration Release --no-build
```

Use focused `--filter` scopes for new security tests before the full project. SQLite, MongoDB, and PostgreSQL evidence must name the actual environment. Missing external provider environments are not a pass.

*CI must also change from .NET 9 to the binding .NET 10 registry version.*

### Feature-002-Abschlussnachweis / Feature 002 Final Proof

DE: Der koordinatoreigene T095-Lauf hat die gesamte Solution mit Build 65 und
einer ausschließlich an `127.0.0.1:55723` gebundenen, danach gestoppten
PostgreSQL-18.4-Instanz geprüft. Dabei liefen alle 14 Tests des
Integrationstestprojekts erfolgreich. T096 ist damit gemäß seiner bedingten
Formulierung bereits vollständig belegt; ein zusätzlicher Playwright-, Host-
oder Testzyklus wäre eine unbegründete Wiederholung.

EN: The coordinator-owned T095 run verified the full solution with Build 65
and an isolated PostgreSQL 18.4 instance bound only to `127.0.0.1:55723` and
stopped afterwards. All 14 tests in the integration-test project passed.
T096 is therefore fully covered under its conditional wording; another
Playwright, host, or test cycle would be an unsupported repetition.

## 8. Collect and evaluate coverage

Increment Build before each coverage test command:

```bash
dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj \
  --configuration Release --no-build \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults

dotnet test CtrlWorkerCommonTest/CtrlWorkerCommonTest.csproj \
  --configuration Release --no-build \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults

dotnet tool restore
dotnet reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"TestResults/CoverageReport" \
  -reporttypes:"TextSummary;JsonSummary"
```

Delivery fails below 70% line coverage. Report the >=80% target and the changed security-critical branch coverage. `TestResults/` remains generated evidence and is not committed.

*Coverage is a minimum gate, not proof that security requirements are correct.*

Der einzige abschließende Feature-002-Lauf verwendete bewusst ein isoliertes
Ergebnisverzeichnis und darf bei unveränderten Eingaben nicht wiederholt
werden:

*The single final Feature 002 run deliberately used an isolated results
directory and must not be repeated while inputs remain unchanged:*

```bash
PGSQL_TEST_CONNECTION_STRING=<ephemeral-loopback> \
  dotnet test InventarWorkerService.sln \
  --configuration Release \
  --no-build \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults/t095-resume
```

DE: Ergebnis auf dem macOS-Koordinator mit .NET SDK 10.0.400: 110 bestanden,
2 ausdrücklich plattformabhängig übersprungen, 0 fehlgeschlagen. Der nur aus
`TestResults/t095-resume/**/coverage.cobertura.xml` erzeugte Report meldet
90,1 Prozent Zeilen-, 60,0 Prozent Branch- und 91,1 Prozent Methodenabdeckung.
Damit bestehen das Pflichtminimum von 70 Prozent und das Ziel von 80 Prozent.

EN: Result on the macOS coordinator with .NET SDK 10.0.400: 110 passed, 2
explicit platform skips, and 0 failed. The report generated only from
`TestResults/t095-resume/**/coverage.cobertura.xml` records 90.1% line, 60.0%
branch, and 91.1% method coverage. Both the mandatory 70% minimum and the 80%
target pass.

## 9. Dependency, serialization, and secret gates

```bash
dotnet list InventarWorkerService.sln package --include-transitive
dotnet list InventarWorkerService.sln package --outdated
dotnet list InventarWorkerService.sln package --vulnerable --include-transitive
rg -n "Newtonsoft\.Json" --glob '*.cs' --glob '*.csproj' --glob '!bin/**' --glob '!obj/**'
rg -n "System\.Text\.Json" --glob '*.cs' --glob '*.csproj' --glob '!bin/**' --glob '!obj/**'
gitleaks git --redact --no-banner --no-color --log-level warn --exit-code 2 --pre-commit .
pwsh -NoProfile -File scripts/scan-agent-secrets.ps1 -FailOnHigh -WorkspaceRoot .
```

The CI `agent-secret-scan.yml` and `gitleaks.yml` results remain required provider evidence.

*Remove `Newtonsoft.Json` from product projects and keep only current stable compatible packages. Document every pinning exception.*

## 10. Supply-chain gate

For each distributable artefact set:

1. Publish/build the declared artefacts at the exact candidate SHA.
2. Generate a CycloneDX or SPDX SBOM with the selected audited, pinned tool.
3. Validate the SBOM schema and bind it to version, artefact hashes, source SHA, and generator version.
4. Record SLSA current/target provenance.
5. Record the vulnerability snapshot and either a no-known-finding VEX decision or a full VEX-style disposition.
6. Record OpenSSF Scorecard evidence for the repository and important dependencies.

Pin the audited latest stable `Microsoft.Sbom.DotNetTool` version in `dotnet-tools.json`, restore it with its native .NET 8 runtime, and publish the declared release set to `artifacts/release`. Version 4.1.5 generates SPDX 3.0 but its official validator supports only SPDX 2.2. The workflow therefore checks the SPDX 3.0 graph and generates a same-build SPDX 2.2 mirror for the official file-hash and package validation. PowerShell reads the aligned repository version directly:

```powershell
dotnet tool restore
$releaseVersion = ([xml](Get-Content -LiteralPath "Directory.Build.props" -Raw -Encoding utf8)).Project.PropertyGroup.Version
dotnet tool run sbom-tool generate `
  -b artifacts/release `
  -bc . `
  -pn InventarWorkerService `
  -pv $releaseVersion `
  -ps "Thorsten Hindermann" `
  -nsb "https://github.com/hindermath/InventarWorkerService" `
  -mi SPDX:3.0 `
  -pm true

New-Item -ItemType Directory -Path artifacts/spdx-2.2-validation -Force | Out-Null
dotnet tool run sbom-tool generate `
  -b artifacts/release `
  -bc . `
  -m artifacts/spdx-2.2-validation `
  -pn InventarWorkerService `
  -pv $releaseVersion `
  -ps "Thorsten Hindermann" `
  -mi SPDX:2.2 `
  -pm true
dotnet tool run sbom-tool validate `
  -b artifacts/release `
  -m artifacts/spdx-2.2-validation/_manifest `
  -o artifacts/sbom-validation.json `
  -mi SPDX:2.2 `
  -n true
```

The implementation evidence records the resolved tool and runtime versions, exact expanded commands, release-set SHA-256 values, generated manifest paths, explicit SPDX 3.0 content checks, the SPDX 2.2 validation output, and official CLI reference. Tool installation/version is part of dependency evidence, not an implicit workstation prerequisite.

*Missing SBOM, provenance, vulnerability, or conditional VEX evidence blocks release.*

## 11. Documentation and accessibility gate

```bash
docfx docfx.json
bash .github/scripts/postprocess-docfx-site.sh _site
test -f _site/index.html
lynx -dump _site/index.html | sed -n '1,80p'
```

Run the Axe smoke logic from `.github/workflows/docs-pages.yml` on the generated site and require the GitHub `Docs Pages / build-docs` job on the candidate SHA. Manually review:

- German-first/English-second CEFR-B2 order;
- text-only status, dependencies, decisions, risk, and next action;
- descriptive links and code-block language tags;
- no colour-, image-, or layout-only meaning;
- keyboard/screen-reader-relevant output changes;
- complete bilingual XML documentation if a public API changed.

```bash
git status --short -- api _site
```

`api/` and `_site/` must remain untracked build artefacts.

## 12. Agent, governance, and statistics parity

```bash
python3 -m unittest scripts/tests/test_spec_kit_agent_surface_parity.py
pwsh -NoProfile -File scripts/check-homogeneity.ps1 -TargetDir . -DryRun -NoPatch
pwsh -NoProfile -File scripts/render-project-statistics.ps1 -Repo . -CheckOnly
```

If shared current guidance changes, review atomically:

```text
AGENTS.md
CLAUDE.md
GEMINI.md
.github/copilot-instructions.md
.github/agents/copilot-instructions.md
constitution.md
.specify/memory/constitution.md
affected .specify/templates/*
affected scripts/templates/*
```

Update only current authoritative drift. Preserve historical Lastenhefte, changelog, compatibility notes, and old statistics facts. After all agent-driven repository work, update `docs/project-statistics.md` with its configured renderer and preserve the final `## Gesamtstatistik` section.

## 13. Exact-head autonomous and MergeAndSync closeout

The coordinator validates gate evidence at the candidate head:

```powershell
pwsh -NoProfile -File .specify/presets/autonomous-run-governance/scripts/validate-autonomous-gate-evidence.ps1 `
  -Requirements specs/002-secure-development-hardening/autonomous-run-gate-requirements.json `
  -Evidence .specify/runtime/autonomous-routing/d6d5e58e-0acc-404c-b5b4-4f97eba83c9f/pre-merge-gate-evidence.json `
  -Head (git rev-parse HEAD)
```

Use these pre-named untracked schema-2.0 paths:

```text
.specify/runtime/autonomous-routing/d6d5e58e-0acc-404c-b5b4-4f97eba83c9f/pre-merge-gate-evidence.json
.specify/runtime/autonomous-routing/d6d5e58e-0acc-404c-b5b4-4f97eba83c9f/post-merge-gate-evidence.json
```

The PreMerge file maps every gate to the exact workflow, job, runner/platform, expanded command, timestamp, result, and candidate SHA from definitions or logs. The PostMerge file binds the accepted PreMerge hash and merge commit. Neither file is committed before merge.

Before an authorized commit/merge:

```bash
git diff --cached --check
git status --short --branch
gh pr checks --watch
```

Merge only when technical gates and required review pass. Admin-Bypass is authorized only for a remaining provider-policy restriction and never for failed or missing technical proof. After merge:

```bash
git switch main
git pull --ff-only
git rev-parse HEAD
git rev-parse origin/main
```

Then run the declared delivery-set/post-merge validator and allow only the outer coordinator to update autonomous state.

DE: Nach dem Merge und der exakten `main`-Synchronisierung wendet nur der
äußere Koordinator den in T107 benannten, pfadgenauen Lineage-Stash an. Er
aktualisiert Lastenheft-, Manifest-, Receipt- und Abarbeitungsreihenfolge-
Evidenz auf den kausalen Post-Merge-Stand, wiederholt die vier Intake-
Validatoren und die Liefermengenprüfung und bestätigt, dass kein Nachfolge-
Intake gestartet wurde. Danach aktualisiert er `tasks.md`,
`autonomous-run-state.json` und `docs/project-statistics.md`, staged nur diese
kausalen Closeout-Pfade und führt als letzte Operation
`bash scripts/rename-lastenheft.sh Lastenheft_Secure-Development-Hardening.md
002-secure-development-hardening` aus. Das Skript erzeugt genau einen
verfassungsmäßigen Closeout-Commit auf `main`; dieser enthält keinen eigenen
Commit-Hash, keine Provider-URL und keine selbstreferenzielle Merge-Behauptung.
Der bereits autorisierte schmale Admin-Bypass darf nur die Provider-Policy beim
Push dieses bestandenen Commits überqueren und ersetzt niemals ein technisches
Gate. Danach müssen `main`, `origin/main`, Liefermenge und Run-State ohne
verbleibendes tracked Delta übereinstimmen.

EN: After merge and exact `main` synchronization, only the outer coordinator
applies the path-scoped lineage stash named in T107. It refreshes the
Lastenheft, manifest, receipt, and processing-order evidence to the causal
post-merge state, reruns all four intake validators and the delivery-set check,
and confirms that no successor intake was started. It then updates `tasks.md`,
`autonomous-run-state.json`, and `docs/project-statistics.md`, stages only these
causal closeout paths, and runs
`bash scripts/rename-lastenheft.sh Lastenheft_Secure-Development-Hardening.md
002-secure-development-hardening` as the final operation. The script creates
exactly one constitutional closeout commit on `main`; that commit does not try
to record its own hash, a provider URL, or a self-referential merge claim. The
already authorized narrow Admin-Bypass may cross only the provider-policy
boundary when pushing this proven commit and never substitutes for a technical
gate. Afterwards, `main`, `origin/main`, the delivery set, and the run state
must agree with no residual tracked delta.

*A green provider label, admin permission, or merge result alone is not closeout evidence.*
