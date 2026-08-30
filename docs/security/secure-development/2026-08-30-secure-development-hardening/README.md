# Secure-Development-Hardening – Feature 002

## Status / Status

DE: Der lokale Implementierungsumfang bis T096 ist belegt. Der erste
deterministische Baseline-Test lief am 2026-08-30 um 10:52:06 UTC
erwartungsgemäß rot; nach der minimalen Manifestkorrektur bestand derselbe
Vertrag mit zwölf Dateipfaden, konsistenten Versionen und genau 157 eindeutigen
IDs. `FIND-001` ist deshalb geschlossen. Die abschließende Vollregression
bestand 110 Tests, übersprang 2 ausdrücklich plattformabhängige Tests und
erreichte 90,1 Prozent Zeilenabdeckung.

EN: The local implementation scope through T096 is evidenced. The first
deterministic baseline test failed as expected at 2026-08-30 10:52:06 UTC;
after the minimal manifest correction, the same contract passed with twelve
paths, consistent versions, and exactly 157 unique IDs. `FIND-001` is therefore
closed. The final full regression passed 110 tests, explicitly skipped 2
platform-specific tests, and reached 90.1% line coverage.

## Evidence-first RED/Open-Nachweis / Evidence-first RED/Open Proof

| Feld / Field | Wert / Value |
|---|---|
| Finding | `FIND-001` – normative Baseline- und Versionsdrift / normative baseline and version drift |
| Klassifikation / Classification | `Open`; Owner: Projektverantwortung; Reviewer: Security-Reviewer; Priorität: Hoch / High |
| Befehl / Command | `dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj --filter "FullyQualifiedName~SecureDevelopmentEvidenceContractTest.BaselineManifest_CanonicalChecklists_ContainsExactly157UniqueControls"` |
| Runner | lokales Darwin, .NET 10 / local Darwin, .NET 10 |
| Kandidaten-SHA / Candidate SHA | `dc15bc4812245e71c1a5976b8241c7aeb518d4a9` |
| Ergebnis / Result | `Fail`, Exitcode 1; der erste Vergleich meldete widersprüchliche Baseline-Metadaten. Die akzeptierte Forschung grenzt die normative Reparatur auf die Manifest-Versionen von CL-09 und CL-12 ein. / `Fail`, exit code 1; the first comparison exposed conflicting baseline metadata. Accepted research narrows the normative repair to the CL-09 and CL-12 manifest versions. |
| Risiko / Risk | Eine günstig gewählte Quelle könnte 157 Bewertungen mit falscher Versionsbindung erzeugen. / Selecting a favourable source could bind 157 assessments to the wrong versions. |
| Maßnahme / Action | Contract auf den akzeptierten Versionsvertrag begrenzen, Manifest minimal reparieren und denselben Test einmal grün ausführen. / Limit the contract to the accepted version rule, minimally repair the manifest, and run the same test once green. |
| Zieltermin / Due date | 2026-08-30 |
| Frische / Freshness | Gültig für den genannten SHA und Build 29; ungültig bei Baseline-, Checklisten- oder Testvertragsänderung. / Valid for the named SHA and Build 29; invalid after a baseline, checklist, or test-contract change. |

Der Restore-Anteil des Testbefehls meldete zusätzlich bekannte
Abhängigkeitswarnungen. Sie werden als getrennte Supply-Chain-Findings bewertet
und nicht als Beweis für oder gegen `FIND-001` verwendet.

The restore part of the test command also reported known dependency warnings.
They are assessed as separate supply-chain findings and are not used as proof
for or against `FIND-001`.

## T095/T096 Vollregression und Coverage / Full Regression and Coverage

| Feld / Field | Wert / Value |
|---|---|
| Befehl / Command | `PGSQL_TEST_CONNECTION_STRING=<ephemeral-loopback> dotnet test InventarWorkerService.sln --configuration Release --no-build --collect:"XPlat Code Coverage" --results-directory ./TestResults/t095-resume` |
| Runner | macOS-Koordinator außerhalb der gerouteten Sandbox; .NET SDK 10.0.400; Build 65 genau einmal reserviert und verbraucht / macOS coordinator outside the routed sandbox; .NET SDK 10.0.400; Build 65 reserved and consumed exactly once |
| Zeit / Time | 2026-08-30 12:06:31–12:06:43 UTC; ReportGenerator-Ausgabe 12:06:43 UTC / ReportGenerator output at 12:06:43 UTC |
| Testresultat / Test result | Exit 0; `InventarWorkerCommonTest` 84/0/0, `CtrlWorkerCommonTest` 12 bestanden, 2 Plattform-Skips, 0 Fehler; `InventarWorkerServiceIntegrationTest` 14/0/0; gesamt 110 bestanden, 2 Skips, 0 Fehler / exit 0; aggregate 110 passed, 2 platform skips, 0 failed |
| PostgreSQL | Isolierter PostgreSQL-18.4-Cluster nur auf `127.0.0.1:55723`; nach dem Lauf gestoppt und temporäres Verzeichnis wiederherstellbar in den Papierkorb verschoben; kein persistenter Homebrew-Dienst verändert / isolated PostgreSQL 18.4 cluster bound only to `127.0.0.1:55723`; stopped afterwards and temporary directory moved recoverably to Trash; no persistent Homebrew service changed |
| Coverage | 90,1 % Zeilen (1103/1224), 60,0 % Branches (131/218), 91,1 % Methoden (93/102); Pflichtminimum 70 % und Ziel 80 % bestanden / 90.1% lines (1103/1224), 60.0% branches (131/218), 91.1% methods (93/102); mandatory 70% minimum and 80% target passed |
| T096-Disposition | Die 14 Integration-Hosttests liefen bereits innerhalb T095 vollständig grün. Kein zusätzlicher Host-/Testzyklus erforderlich. / All 14 integration-host tests already passed within T095. No extra host/test cycle is required. |

Integritätsbindung / Integrity binding:

- `TestResults/t095-resume/02ef4c3d-1d5e-45e1-9414-f1f405066b55/coverage.cobertura.xml`: `e18abe125b7e8ebf2de4c1e3c581a0c8cf4c58a98b41414790f9dace490d4ae3`
- `TestResults/t095-resume/14910465-ba53-47c3-9f8c-88c15674e521/coverage.cobertura.xml`: `5c1461b4b078c86f515eb5fa2e88eeb17e83c800a9fb4fddbbf2a656210602e0`
- `TestResults/t095-resume/5fd2b87e-05f2-4b51-85ec-c15bdf112977/coverage.cobertura.xml`: `24d882642fb8ea3be045da3f51bfbe0ac193669b3c324002fa9dbc2e3e7949d7`
- `TestResults/t095-resume/CoverageReport/Summary.txt`: `6a14841fdd6f3f1906d395745bff16590dd178aef32ec7ff288382c55f14a1ba`
- `TestResults/t095-resume/CoverageReport/Summary.json`: `a30c6aa131100bd45d8bddd050c8227d653a46878257689ad4ad93ba5dbb17e6`

DE: Alle fünf Dateien wurden vor der Evidenzbindung nochmals ausschließlich
lesend gehasht. ReportGenerator konsumierte nur die drei oben genannten
Cobertura-Dateien. `TestResults/` bleibt generiert und gehört nicht zur
Liefermenge.

EN: All five files were hashed again read-only before binding this evidence.
ReportGenerator consumed only the three Cobertura files named above.
`TestResults/` remains generated and is excluded from the delivery set.

## Ausführbare Konsumenten / Executable Consumers

DE: Vor einem proportionalen Test-Skip wurden die aktuellen Konsumenten mit
`rg` ermittelt. Unbekannte Konsumenten bleiben offen. Die folgende Liste ist
textuell vollständig und benötigt keine Farbinterpretation.

EN: Current consumers were mapped with `rg` before any proportional test skip.
Unknown consumers remain open. The list is complete in text and does not rely
on colour.

- Security-, Architektur- und Assessment-Verträge: `SecureDevelopmentEvidenceContractTest` sowie die geplanten fokussierten MSTest-Methoden.
- Agent-, Constitution- und Template-Parität: `scripts/tests/test_spec_kit_agent_surface_parity.py`, `scripts/install-spec-kit-governance-presets.ps1 -CheckOnly` und `scripts/check-homogeneity.ps1 -DryRun -NoPatch`.
- Documentation Impact: `scripts/validate-documentation-impact.ps1`.
- Intake und Series Lineage: die PowerShell-Validatoren der Presets `intake-authoring-governance` und `intake-sequencing-governance`.
- Projektstatistik: `scripts/render-project-statistics.ps1 -CheckOnly`.
- Autonome Zustands-, Phasen- und Delivery-Verträge: die PowerShell-Validatoren unter `.specify/presets/autonomous-run-governance/scripts/`.
- Provider-/CI-, DocFX-, Axe-, Lynx-, Dependency-, SBOM- und Secret-Scope: die jeweiligen lokalen Commands und die Jobs in `.github/workflows/`.

- Security, architecture, and assessment contracts: `SecureDevelopmentEvidenceContractTest` and the planned focused MSTest methods.
- Agent, constitution, and template parity: `scripts/tests/test_spec_kit_agent_surface_parity.py`, `scripts/install-spec-kit-governance-presets.ps1 -CheckOnly`, and `scripts/check-homogeneity.ps1 -DryRun -NoPatch`.
- Documentation Impact: `scripts/validate-documentation-impact.ps1`.
- Intake and series lineage: the PowerShell validators from the `intake-authoring-governance` and `intake-sequencing-governance` presets.
- Project statistics: `scripts/render-project-statistics.ps1 -CheckOnly`.
- Autonomous state, phase, and delivery contracts: the PowerShell validators under `.specify/presets/autonomous-run-governance/scripts/`.
- Provider/CI, DocFX, Axe, Lynx, dependency, SBOM, and secret scope: the matching local commands and `.github/workflows/` jobs.

## Scope-Grenze / Scope Boundary

DE: Verbindlich ist nur `Lastenheft_Secure-Development-Hardening.md`. Der
Sandbox-Intake bleibt blockiert und wird in diesem Lauf nicht gestartet. Der
autonome Run-State bleibt ausschließlich im Eigentum des äußeren Koordinators.

EN: Only `Lastenheft_Secure-Development-Hardening.md` is binding. The sandbox
intake remains blocked and is not started in this run. The autonomous run state
remains owned exclusively by the outer coordinator.

## Finding-bedingte Härtung / Finding-Conditioned Hardening

DE: Umgesetzt wurden nur belegte Lücken: API-Key-Fallback-Authentifizierung in
beiden HTTP-Hosts, generische Außenfehler mit internem Logging, HTTPS für
entfernte Ziele, 30-Sekunden-Timeout und Cancellation im API-Client,
begrenzte Dienst-/Dateipfade, atomare Status-/Settings-Schreibvorgänge,
`System.Text.Json`-camelCase sowie die Redaction geheimnisverdächtiger
Umgebungsvariablen. SQLite/PostgreSQL waren bereits parametrisiert; MongoDB
verwendet den typisierten Filter-Builder. Dafür wurden keine künstlichen
Provideränderungen erzeugt.

EN: Only evidenced gaps were changed: API-key fallback authentication in both
HTTP hosts, generic external errors with internal logging, HTTPS for remote
targets, a 30-second timeout and cancellation in the API client, bounded
service/file paths, atomic status/settings writes, `System.Text.Json` camelCase,
and redaction of secret-like environment variables. SQLite/PostgreSQL were
already parameterised and MongoDB uses the typed filter builder. No artificial
provider edit was created.

## Lokales Gate-Register / Local Gate Register

| Gate | Ergebnis / Result | Evidenz / Evidence |
|---|---|---|
| `G-TRACE-001` | Pass | Exakt zwölf Presets, 157 eindeutige IDs, fünf Intake-Zustände; gefilterte MSTests grün. / Exactly twelve presets, 157 unique IDs, five intake states; filtered MSTests green. |
| `G-ARCH-001` | Pass | Neun Trust Boundaries, arc42-Sichten, S-ADR und Qualitäts-/Risikoszenarien; Contract-Test grün. / Nine trust boundaries, arc42 views, S-ADR, quality/risk scenarios; contract test green. |
| `G-REGULATORY-001` | Pass | CRA-Screening `Applicable`; NIS2, DORA, EU AI Act, BSI C3A/C5 begründet `N/A`; Contract-Test grün. / CRA screening `Applicable`; reasoned `N/A` for NIS2, DORA, EU AI Act, BSI C3A/C5; contract test green. |
| `G-SEC-001` | Local pass; provider follow-up | Negative/GREEN-Grenzen für HTTP, SSRF/TLS, Prozessnamen, Pfade, Serialisierung und Secret-Redaction; T095 insgesamt 110 Tests grün einschließlich aller 14 Integration-Hosttests. Nicht ausgeführte externe Provider-/CI-Scopeanteile bleiben offen. / Negative/GREEN boundaries for HTTP, SSRF/TLS, process names, paths, serialization, and secret redaction; T095 passed 110 tests including all 14 integration-host tests. Unexecuted external provider/CI scope remains open. |
| `G-BUILD-001` | Pass local; remote matrix pending | Release-Build, .NET 10, 0 Warnungen, 0 Fehler; Vollregression 110 bestanden, 2 Plattform-Skips, 0 Fehler. CI deklariert Ubuntu/macOS/Windows. / Release build, .NET 10, zero warnings/errors; full regression 110 passed, 2 platform skips, 0 failed. CI declares Ubuntu/macOS/Windows. |
| `G-A11Y-001` | Open | DocFX/Postprocessing/Lynx/manuell grün; Axe konnte Chromium in der verwalteten macOS-Sandbox nicht starten. Owner Accessibility Reviewer; hoch; Termin 2026-09-15; Trigger Providerlauf. / DocFX/postprocessing/Lynx/manual green; Axe could not start Chromium in the managed macOS sandbox. Owner Accessibility Reviewer; high; due 2026-09-15; trigger provider run. |
| `G-SUPPLY-001` | Open | NuGet-CVE-Prüfung grün und aktueller Diff/Tracked-Secret-Scan grün. Vollhistorischer Gitleaks-Lauf meldet 16 mögliche Schlüsselwerte aus Commit `028bfdc`/`SoftwareInventories.csv`; Credential-Prüfung oder Widerruf und Providerlogs fehlen. Owner Security Reviewer; kritisch; Termin 2026-09-01. / NuGet CVE review green and current diff/tracked secret scan green. Full-history Gitleaks reports 16 possible key values from commit `028bfdc`/`SoftwareInventories.csv`; credential review or revocation and provider logs are pending. Owner Security Reviewer; critical; due 2026-09-01. |
| `G-AGENT-001` | Pass | Agent-Parität 3/3, Zwölfer-Profil 12/12, Homogenität 29/29. Historische Achtermatrix bleibt absichtlich erhalten. / Agent parity 3/3, managed profile 12/12, homogeneity 29/29. Historical eight-preset matrix intentionally remains. |
| Coverage | Pass | Isolierter T095-Report: 90,1 % Zeilen, 60,0 % Branches, 91,1 % Methoden; Pflichtminimum 70 % und Ziel 80 % bestanden. / Isolated T095 report: 90.1% lines, 60.0% branches, 91.1% methods; mandatory 70% minimum and 80% target passed. |

## T097 Quickstart- und Gate-Abgleich / Quickstart and Gate Reconciliation

DE: Der proportionale Abgleich wurde am 2026-08-30 um 12:15:58 UTC auf dem
lokalen Darwin-Runner ausgeführt. Er bestätigte Branch und Head, den
deaktivierten Auto-Commit-Hook, exakt zwölf aktive Presets, die vollständige
`System.Text.Json`-Migration, zwölf nicht leere CL-Projektinstanzen, exakt 157
eindeutige Assessment-Datensätze, unveränderte T095-Hashes, ausgeschlossene
Generatpfade und einen fehlerfreien unstaged Diff. Die korrigierte
Documentation-Impact-Prüfung
`pwsh -NoProfile -File scripts/validate-documentation-impact.ps1 -Evidence docs/documentation-impact/feature-002-secure-development-hardening.json`
bestand mit einer aktuellen Evidence-Zeile. Der zuvor dokumentierte T094-
Release-Build und der akzeptierte T095-Solution-Lauf werden wiederverwendet;
es gab keine zweite Vollsuite.

EN: The proportional reconciliation ran on the local Darwin runner at
2026-08-30 12:15:58 UTC. It confirmed branch and head, the disabled auto-commit
hook, exactly twelve enabled presets, complete `System.Text.Json` migration,
twelve non-empty CL project instances, exactly 157 unique assessment records,
unchanged T095 hashes, excluded generated paths, and an error-free unstaged
diff. The corrected Documentation Impact command
`pwsh -NoProfile -File scripts/validate-documentation-impact.ps1 -Evidence docs/documentation-impact/feature-002-secure-development-hardening.json`
passed with one current evidence entry. The documented T094 Release build and
accepted T095 solution run are reused; no second full suite was executed.

| Gate | Lokaler Abschluss / Local close | Externe Restgrenze / External remaining boundary |
|---|---|---|
| `G-TRACE-001` | Branch, 12 Presets, 157 IDs und fünf Intake-Zustände bestanden; T095 schützt den unveränderten Contract. / Branch, 12 presets, 157 IDs, and five intake states passed; T095 protects the unchanged contract. | Keine lokale Restpflicht / no local remainder |
| `G-ARCH-001` | Neun Trust Boundaries, arc42-Sichten, S-ADR, Risiken und Qualitätsfälle bestanden den unveränderten Contract. / Nine trust boundaries, arc42 views, S-ADR, risks, and quality cases passed the unchanged contract. | Review-Konvergenz am Kandidaten-Head in T111–T114 / review convergence at candidate head in T111–T114 |
| `G-SEC-001` | 110 Tests einschließlich 14 Integration-Hosttests grün; PostgreSQL tatsächlich isoliert ausgeführt. / 110 tests including 14 integration-host tests passed; PostgreSQL was actually executed in isolation. | Deklarierte CI-/Providerjobs dürfen nur ihren tatsächlich ausgeführten Scope ergänzen. / Declared CI/provider jobs may add only their executed scope. |
| `G-BUILD-001` | Restore und Release-Build grün; T095: 110 bestanden, 2 Plattform-Skips, 0 Fehler, 90,1 % Zeilenabdeckung. / Restore and Release build passed; T095: 110 passed, 2 platform skips, 0 failed, 90.1% line coverage. | Drei-Plattform-Provider-Matrix am exakten Kandidaten-Head / three-platform provider matrix at exact candidate head |
| `G-A11Y-001` | DocFX, Postprocessing, Lynx und manueller text-first Review bestanden. / DocFX, postprocessing, Lynx, and manual text-first review passed. | Axe/Chromium bleibt mit Owner, Termin und Trigger offen, da der lokale Sandboxstart verweigert wurde. / Axe/Chromium remains open with owner, due date, and trigger because local sandbox launch was denied. |
| `G-SUPPLY-001` | Lokale Paket-/CVE-, Toolmanifest-, Workflow-, SBOM-/VEX- und Secret-Scan-Quellen sind vollständig disponiert. / Local package/CVE, tool-manifest, workflow, SBOM/VEX, and secret-scan sources are fully dispositioned. | Provider-SBOM/Provenienz/OpenSSF/Scanlogs und historische Credential-Disposition bleiben ehrlich offen. / Provider SBOM/provenance/OpenSSF/scan logs and historical credential disposition remain explicitly open. |
| `G-AGENT-001` | Bestehende aktuelle Evidenz: Agent-Parität 3/3, Flottenprofil 12/12, Homogenität 29/29; keine spätere Agent-/Template-Mutation. / Current retained evidence: agent parity 3/3, fleet profile 12/12, homogeneity 29/29; no later agent/template mutation. | Kandidatengebundener Providerjob / candidate-bound provider job |
| `G-REGULATORY-001` | CRA-Screening anwendbar; NIS2, DORA, EU AI Act und BSI C3A/C5 mit Annahme, Risiko, Owner und Trigger `N/A`; unveränderter Contract durch T095 grün. / CRA screening applies; reasoned N/A dispositions include assumptions, risk, owner, and trigger; unchanged contract passed T095. | Owner-Entscheidung vor Release bleibt benannt. / Owner decision before release remains named. |

DE: „Lokal abgeschlossen“ bedeutet, dass alle T097-Lokalpflichten ausgeführt
oder mit unveränderter, hashgebundener Evidenz wiederverwendet wurden. Es ist
keine vorgezogene Behauptung über Providerchecks, unabhängige Remote-Reviews,
PreMerge, PR oder Merge.

EN: “Locally closed” means every T097 local obligation was executed or reused
from unchanged hash-bound evidence. It is not an early claim about provider
checks, independent remote reviews, PreMerge, PR, or merge.

## T099 Audit-Abschluss / Audit Closeout

DE: Der Abschlussabgleich am 30. August 2026 bestätigte exakt 157 eindeutige
Assessment-Datensätze und zwölf Projektinstanzen. Die Verteilung ist 126
`Open / Not Assessed` und 31 begründet `N/A / Not Assessed`. Automatisierte
Invarianten fanden keinen offenen Datensatz ohne Owner, Reviewer, Restrisiko,
Maßnahme, ISO-Termin, Finding oder Trigger und keine ungültige N/A-
Kombination. Offene Punkte bleiben bewusst sichtbar; insbesondere werden
G-A11Y und G-SUPPLY ohne Provider-/Credential-Nachweis nicht grün gemeldet.
Die Security-, Architektur-, A11Y-, Dependency-, Supply-Chain- und
Regulierungsdokumente bilden gemeinsam den auditierbaren lokalen
Vor-Kandidaten-Stand.

EN: The closeout reconciliation on 30 August 2026 confirmed exactly 157 unique
assessment records and twelve project instances. The distribution is 126
`Open / Not Assessed` and 31 reasoned `N/A / Not Assessed`. Automated
invariants found no open record without owner, reviewer, residual risk, action,
ISO due date, finding, or trigger and no invalid N/A combination. Open items
remain deliberately visible; in particular, G-A11Y and G-SUPPLY are not marked
green without provider and credential evidence. The security, architecture,
accessibility, dependency, supply-chain, and regulatory documents together
form the auditable local pre-candidate state.

## T100 Anforderungs- und Review-Disposition / Requirements and Review Disposition

| Bereich / Area | Disposition vor Archivierung / Disposition before archival |
|---|---|
| `IR-001`–`IR-009` | Lokaler Feature-Umfang umgesetzt und belegt; externe Release-/Provideranteile bleiben als vollständige Open-/FollowUp-Einträge erhalten. / Local feature scope implemented and evidenced; external release/provider portions remain complete Open/FollowUp entries. |
| `IR-010`–`IR-013` | `AlreadySatisfied` mit aktueller Repository-Evidenz; keine künstliche Doppelimplementierung. / `AlreadySatisfied` with current repository evidence; no artificial duplicate implementation. |
| `IR-014`–`IR-016` | `N/A` mit Annahme, Restrisiko und Trigger: keine KI-Runtime, kein Cloudscope, kein neues/geändertes Skript. / `N/A` with assumption, residual risk, and trigger: no AI runtime, cloud scope, or new/changed script. |
| `IR-017`–`IR-018` | `FollowUp` für externe Prüfung/Zertifizierungsentscheidung und wiederkehrende Monats-/Releaseprüfungen. / `FollowUp` for external assessment/certification decision and recurring monthly/release reviews. |
| Security Review | Lokaler Secure-Coding-/Test-/Evidenzabgleich vollständig; unabhängige Remote-Review-Konvergenz bleibt `Open`, Owner Security Reviewer, Priorität hoch, Termin 2026-09-15, Trigger T111. / Local secure-coding/test/evidence reconciliation complete; independent remote review convergence remains `Open`, owner Security Reviewer, high priority, due 2026-09-15, trigger T111. |
| Architecture Review | arc42-Kontext, Laufzeit, Deployment, Risiken, Qualität und S-ADR lokal vollständig; allgemeiner ADR begründet N/A. Unabhängige Kandidatenreview bleibt `Open`, Owner Architecture Reviewer, Priorität mittel, Termin 2026-09-15, Trigger T111. / Local arc42 context, runtime, deployment, risks, quality, and S-ADR complete; general ADR is reasoned N/A. Independent candidate review remains open, owner Architecture Reviewer, medium priority, due 2026-09-15, trigger T111. |
| Accessibility Review | Manueller text-first-Quellenreview vollständig; Axe/Chromium und exakter Docs-Kandidat bleiben `Open`, Owner Accessibility Reviewer, Priorität hoch, Termin 2026-09-15, Trigger `Docs Pages / build-docs`. / Manual text-first source review complete; Axe/Chromium and exact docs candidate remain open, owner Accessibility Reviewer, high priority, due 2026-09-15, trigger `Docs Pages / build-docs`. |
| Release Review | Lokale Restore-/Build-/Test-/Coverage-/Dependency-Evidenz vollständig. Release-Entscheidung bleibt `Blocked`, bis Providerjobs, historische Credential-Disposition, Supply-Chain-Evidenz und unabhängige Reviews am exakten Head bestanden sind; Owner Release Reviewer, Priorität kritisch, Termin 2026-09-15, Trigger T108–T114. / Local restore/build/test/coverage/dependency evidence complete. Release decision remains `Blocked` until provider jobs, historical credential disposition, supply-chain evidence, and independent reviews pass at the exact head; owner Release Reviewer, critical priority, due 2026-09-15, trigger T108–T114. |

DE: „Archivierungsbereit“ bedeutet hier, dass jede Anforderung und jede
Reviewpflicht eine ehrliche, vollständige Disposition besitzt. Es bedeutet
nicht, dass offene Release-Gates bestanden seien.

EN: “Ready for archival” means every requirement and review obligation has an
honest, complete disposition. It does not mean that open release gates passed.

## T101 berechtigungssichere Umbenennung / Permission-Safe Rename

DE: `scripts/rename-lastenheft.sh` wurde vor der Ausführung geprüft. Das
Skript koppelt `git mv` fest an Staging und Commit; beides ist in dieser Phase
ausdrücklich nicht autorisiert. Deshalb wurde ausschließlich die inhaltlich
gleiche Dateiverschiebung nach
`Lastenheft_Secure-Development-Hardening.002-secure-development-hardening.md`
angewendet. Der SHA-256 blieb
`9eb32515a590aada78a5b33170e00aace834ce87d9b5f5ed9535445c6e50380f`;
der echte Git-Index und `HEAD` blieben unverändert.

EN: `scripts/rename-lastenheft.sh` was inspected before execution. The script
couples `git mv` directly to staging and committing, neither of which is
authorised in this phase. Therefore, only the content-preserving file move to
`Lastenheft_Secure-Development-Hardening.002-secure-development-hardening.md`
was applied. SHA-256 remained
`9eb32515a590aada78a5b33170e00aace834ce87d9b5f5ed9535445c6e50380f`;
the real Git index and `HEAD` remained unchanged.

## Bedingte N/A-Gates / Conditional N/A Gates

DE: `G-SCRIPT-001` bleibt `N/A`, weil der Diff keine `.sh`- oder `.ps1`-Datei
ändert; Trigger ist jede Skriptänderung. `G-AI-SBOM-001` bleibt `N/A`, weil KI
nur Entwicklungswerkzeug und keine ausgelieferte Runtime-/Modell-/Datenkomponente
ist; Trigger ist KI im Produkt. `G-CLOUD-001` bleibt `N/A`, weil kein Cloud-
Provider eingeführt wurde; Trigger ist SaaS/PaaS/IaaS. `G-GENERAL-ADR-001`
bleibt `N/A`, weil die einzige materielle Entscheidung eine Security-S-ADR ist;
Trigger ist eine nicht sicherheitsspezifische Strukturentscheidung.

EN: `G-SCRIPT-001` remains `N/A` because the diff changes no `.sh` or `.ps1`
file; any script change is the trigger. `G-AI-SBOM-001` remains `N/A` because AI
is development tooling, not a released runtime/model/data component; AI in the
product is the trigger. `G-CLOUD-001` remains `N/A` because no cloud provider is
introduced; SaaS/PaaS/IaaS is the trigger. `G-GENERAL-ADR-001` remains `N/A`
because the only material decision is a security S-ADR; a non-security
structural decision is the trigger.

## Stop-Grenze / Stop Boundary

DE: T095 und die bedingte T096 sind vollständig belegt. Die lokale
Implementierung setzt mit T097 fort. Provider-, Commit-, PR-, Merge- und
Closeout-Schritte T108–T122 bleiben beim Koordinator. Der Run-State wurde nicht
verändert.

EN: T095 and conditional T096 are fully evidenced. Local implementation
continues with T097. Provider, commit, PR, merge, and closeout steps T108–T122
remain coordinator-owned. The run state was not modified.
