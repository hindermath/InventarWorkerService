# Research: Secure-Development-Hardening

**Feature**: `002-secure-development-hardening`

**Date**: 2026-08-30

**Scope**: Repository-local security assessment, evidence completion, and finding-driven hardening only

## 1. Binding technical baseline

### Decision

Der Level-2-Registry-Eintrag `RiderProjects/InventarWorkerService` ist bindend. Die Lösung bleibt eine .NET-10-/C#-14-Multi-Projekt-Lösung mit MSTest, bedarfsweisem Playwright, DocFX und textorientierter A11Y-Prüfung. C# ist nach Constitution-Prinzip XI eine speichersichere Sprache.

*The `RiderProjects/InventarWorkerService` Level-2 registry row is binding. The solution remains a .NET 10/C# 14 multi-project solution with MSTest, Playwright where needed, DocFX, and text-oriented accessibility review. C# is memory-safe under Constitution Principle XI.*

### Rationale

Alle Produktprojekte zielen bereits auf `net10.0`, und `Directory.Build.props` legt C# 14 fest. Ein Sprach-, Framework- oder Plattformwechsel würde den Intake erweitern und keinen zusätzlichen Sicherheitsnachweis liefern.

*All product projects already target `net10.0`, and `Directory.Build.props` selects C# 14. A language, framework, or platform change would expand the intake without adding relevant security proof.*

### Rejected alternatives

- Neue Sicherheits-Microservices oder ein neues Datenbankprojekt: abgelehnt, weil vorhandene Schichten die betroffenen Grenzen besitzen.
- Nicht speichersichere Sprache für Scanner oder Helfer: abgelehnt, weil keine Plattformgrenze dies verlangt.

## 2. Product and trust-boundary scope

### Decision

Die Sicherheitsbewertung umfasst die vorhandenen Komponenten und keine neue Fachfunktion:

- InventarWorkerService HTTP/API und Serviceprozess
- InventarViewerApp HTTP/API und Terminal.Gui-Oberfläche
- HarvesterWorkerService und ausgehende Worker-Verbindungen
- InventarWorkerCommon einschließlich SQLite, MongoDB, PostgreSQL, CSV, Settings, Status und Dateien
- CtrlWorker-Komponenten und ServiceStatusReaderApp
- CI/CD, Releases, Dokumentation, Spec-Kit und Agentenflächen

*The assessment covers the existing components and introduces no business feature.*

### Trust boundaries

1. API-Client zu InventarWorkerService
2. Client zu Viewer-API
3. Harvester/Viewer zu Worker
4. Dienste zu SQLite, MongoDB und PostgreSQL
5. CSV, Datei und Konfiguration zu Prozess
6. Betriebssysteminventar zu Dienst
7. Nutzer:in zu TUI/CLI
8. CI zu Registry, Build und Release
9. Agent/Runner zu Repository und coordinator-owned Run-State

*Input, identity, authorization, and error handling are re-evaluated at every listed boundary.*

## 3. Assessment record contract

### Decision

Jede stabile CL-ID erhält genau einen Klassifikationsdatensatz mit getrennten Achsen für Anwendbarkeit und Umsetzung. Die verbindliche Struktur steht in `contracts/classification-evidence-record.schema.json`; Sammlungskriterien und Beziehungen stehen in `data-model.md`.

*Each stable CL ID receives exactly one classification record with separate applicability and implementation axes. The normative structure is in the JSON Schema contract, while collection rules and relationships are in `data-model.md`.*

Die fünf Intake-Zustände `Applicable`, `AlreadySatisfied`, `N/A`, `Open` und `FollowUp` bleiben davon getrennt. Nur `Applicable` wird zu Tasks. Der Evidenzdatensatz bindet Reviewer, Runner/Plattform, vollständigen Kandidaten-SHA, Integrität, Gültigkeit, Frischegrund und Invalidierungs-Trigger. Eine spätere Task erstellt `InventarWorkerCommonTest/SecureDevelopmentEvidenceContractTest.cs` als einzigen deterministischen Besitzer der Baseline-, Schema-, Sammlungs-, Architektur- und Regulierungsinvarianten mit positiven und negativen Fixtures.

*The five intake states remain separate and only Applicable enters Tasks. Evidence binds reviewer, runner/platform, full candidate SHA, integrity, validity, freshness basis, and invalidation triggers. One later MSTest contract class owns deterministic positive and negative validation.*

### Rationale

Ein präziser, maschinenlesbarer Vertrag verhindert, dass `Applicable`, `N/A`, `Open` und der Umsetzungsstatus vermischt werden. Er zwingt positive Aussagen zu konkreter Evidenz und N/A/Open zu Begründung, Risiko und Neubewertung.

*A precise machine-readable contract prevents applicability and implementation status from being mixed. It binds positive claims to evidence and N/A/Open decisions to rationale, risk, and re-evaluation.*

### Rejected alternatives

- Reine Ja/Nein-Checkboxen: verlieren Status, Risiko und Aktualität.
- Ein einzelner Freitextbericht: kann Vollständigkeit und Eindeutigkeit für 157 IDs nicht zuverlässig belegen.
- Dateiexistenz als Erfüllung: widerspricht FR-003 und den abgeschlossenen Checklisten.

## 4. Baseline and governance drift

### Observed facts

- `docs/secure-development/baseline-manifest.json` nennt Basis `3.1.0`, CL-09 `2.1.0` und CL-12 `2.1.0`.
- Die kanonischen Dateien CL-09 und CL-12 nennen jeweils `2.2.0`; CL-12 beschreibt bereits das Achterprofil.
- Die zwölf Checklistendateien enthalten aktuell genau 157 eindeutige stabile IDs.
- Der akzeptierte Intake nennt sechs Presets als damalige Teilmenge.
- Die installierte Registry enthält zwölf aktive Presets: acht Kern-Presets plus Model Routing und drei Intake-Prozess-Presets.
- Gepflegte Repository-Templates nennen die aktuellen Kernversionen und das Zwölfer-Flottenprofil; mehrere direkt genutzte Agent-/Constitution-Flächen enthalten noch ältere Versions- oder Achtermatrix-Prosa.
- `.github/workflows/ci.yml` installiert .NET 9, obwohl Registry und Projekte .NET 10 verlangen.

### Decision

Vor der Projektbewertung läuft ein fail-closed Konsistenzgate. Umsetzung repariert nur aktuelle normative Widersprüche, die für dieses Feature relevant sind:

1. Baseline-Manifest, kanonische Checklisten und 157-ID-Zählung synchronisieren.
2. Aktuelle Agenten-/Constitution-Flächen mit installierter Registry und gepflegten Template-Quellen abgleichen.
3. CI auf .NET 10 ausrichten.

Historische Lastenhefte, Changelog- und Statistikzeilen bleiben als Herkunftsnachweis unverändert. Ein zentraler Home-Baseline-Fehler wird nur als separates Follow-up dokumentiert; dieses Feature ändert nichts außerhalb des Repositorys.

*A fail-closed consistency gate runs before project assessment. Implementation repairs only current normative contradictions relevant to this feature. Historical intake, changelog, and statistics text remains provenance. External Home-baseline changes stay a separate follow-up.*

## 5. Secure coding and serialization

### Decision

Produktive JSON-Verarbeitung verwendet ausschließlich `System.Text.Json` mit bestehender camelCase-Konvention. Die noch vorhandene `Newtonsoft.Json`-Paketreferenz in `InventarViewerApp` wird entfernt, sobald eine Repository-Prüfung bestätigt, dass keine erforderliche Nutzung verbleibt. Neue Abstraktionsschichten werden dafür nicht eingeführt.

*Production JSON uses only `System.Text.Json` with the existing camelCase convention. The remaining `Newtonsoft.Json` package reference in `InventarViewerApp` is removed after repository review confirms no required usage remains. No new abstraction layer is introduced for this change.*

### Data-access decision

- SQLite und PostgreSQL verwenden Parameter für Datenwerte.
- Dynamische Identifikatoren werden auf erlaubte Werte begrenzt und sicher gequotet; sie werden nie direkt aus nicht vertrauenswürdigem Input übernommen.
- MongoDB verwendet typisierte Filter oder gleichwertig sichere Builder.
- Providerrechte folgen Least Privilege; positive Evidenz wird nicht zwischen Providern übertragen.

*SQLite and PostgreSQL parameterize values; dynamic identifiers are constrained and safely quoted; MongoDB uses typed or equivalent safe builders; provider permissions follow least privilege.*

## 6. HTTP/API security verification

### Decision

OWASP ASVS 5.0 Level 2 gilt für alle Endpunkte beider Hosts, einschließlich Zugriffskontrolle, Transport, Eingabevalidierung, Fehler, Logging, Konfiguration, Swagger/OpenAPI und statische Dokumentationspfade.

*OWASP ASVS 5.0 Level 2 applies to every endpoint of both hosts, including access control, transport, input validation, errors, logging, configuration, Swagger/OpenAPI, and static documentation paths.*

### Test method

Für jedes implementierte Finding gilt Red-Green-Refactor:

1. Negativ- oder Regressionstest schlägt aus dem erwarteten Sicherheitsgrund fehl.
2. Minimale Änderung behebt genau diesen Grund.
3. Refactoring erhält fokussierte und vollständige grüne Tests.

Repräsentative Tests decken unberechtigten Zugriff, falsche Formate/Bereiche/Längen, Grenzwerte, Secret-Redaktion, interne Fehler, manipulierte Ziele, Zeitüberschreitung, Abbruch und Providerfehler ab.

*Representative tests cover unauthorized access, invalid formats/ranges/lengths, boundaries, secret redaction, internal errors, malicious targets, timeout, cancellation, and provider failures.*

## 7. Security architecture evidence

### Decision

STRIDE und CIA bilden die vollständige Bedrohungsmodell-Basis; CAPEC beschreibt die höchsten Angriffspfade. Hohe Risiken benötigen zwei unabhängige Schutzebenen oder eine ausdrücklich akzeptierte Restrisikoentscheidung. Zero Trust wird für alle verteilten Dienst- und Datenbankflüsse geprüft; Netzwerkstandort allein reicht nicht als Vertrauen.

*STRIDE and CIA form the threat-model base, CAPEC covers the highest-risk paths, and high risks need two independent controls or explicit residual-risk acceptance. Zero Trust is assessed across distributed service and database flows.*

Allgemeine arc42/iSAQB-Nachweise werden für Kontext, Laufzeit, Deployment, Risiken und Qualitätsszenarien geführt. Ein allgemeiner ADR bleibt N/A, solange kein nicht-sicherheitsspezifischer Strukturentscheid entsteht. Signifikante Security-Entscheidungen erhalten S-ADRs.

*General arc42/iSAQB evidence covers context, runtime, deployment, risks, and quality scenarios. General ADR remains N/A unless a non-security structural decision emerges. Material security decisions use S-ADRs.*

## 8. Dependency and supply-chain evidence

### Decision

Der Abhängigkeitsnachweis umfasst direkte und transitive Pakete, bekannte CVEs, Lizenzen, Registry-Herkunft, Aktualität, Reproduzierbarkeit und Ausnahmen. Pakete werden auf die letzte stabile kompatible Version gebracht; jede Pinning-Ausnahme erhält Owner, Risiko, Ablaufdatum und Trigger.

*Dependency evidence covers direct and transitive packages, CVEs, licences, registry provenance, currency, reproducibility, and exceptions. Packages move to the latest stable compatible release, with bounded documented exceptions only.*

Jeder verteilbare Artefaktsatz erhält eine eindeutig an Version und Hash gebundene CycloneDX- oder SPDX-SBOM. SLSA-Evidenz beschreibt Quelle, Builder, Schritte und Integrität. Der VEX-Schritt ist als Gate anwendbar: Ohne bekannten Fund wird die aktuelle Negativentscheidung dokumentiert; mit Fund ist vor Release eine VEX-artige Disposition Pflicht. OpenSSF Scorecard wird für das öffentliche Repository und wesentliche externe Abhängigkeiten ausgewertet.

*Every distributable artefact set receives a version/hash-bound CycloneDX or SPDX SBOM. SLSA evidence records source, builder, steps, and integrity. The VEX gate always records either a current no-known-finding decision or a required disposition. OpenSSF Scorecard covers the public repository and important dependencies.*

### Tooling choice

Bestehende Repository- und CI-Befehle werden bevorzugt. Für die fehlende maschinenlesbare Erzeugung wird `Microsoft.Sbom.DotNetTool` nach Prüfung als aktuelle stabile lokale .NET-Tool-Abhängigkeit gepinnt. Die offiziellen Befehle `sbom-tool generate` und `sbom-tool validate` erzeugen und prüfen SPDX 3.0. Referenz: `https://github.com/microsoft/sbom-tool/blob/main/docs/sbom-tool-cli-reference.md`. Kein separates Allzweck-Skript wird eingeführt.

*Existing repository and CI commands are preferred. The missing machine-readable generation uses an audited latest-stable local `Microsoft.Sbom.DotNetTool` dependency pinned in the tool manifest. The official `sbom-tool generate` and `sbom-tool validate` commands produce and validate SPDX 3.0. No general-purpose script is introduced.*

## 9. AI, cloud, and regulatory applicability

| Area | Decision | Re-evaluation trigger |
|---|---|---|
| AI-SBOM | N/A; AI is development tooling only | Product/operated model, AI service, dataset, inference infrastructure, or runtime |
| EU AI Act | N/A under the same assumption | AI product/runtime scope or regulated AI use |
| BSI C3A/C5 | N/A; no cloud/managed provider is introduced | Provider-dependent hosting, SaaS/PaaS/IaaS, or assurance requirement |
| NIS2 | N/A under current operator/customer assumptions | Essential/important entity or regulated supply chain |
| DORA | N/A | Financial entity or critical ICT-provider scope |
| CRA | Applicable screening | Owner decides market/distribution scope before release |

Diese Entscheidungen sind technische Screenings und keine Rechtsberatung.

*These decisions are technical screenings, not legal advice.*

## 10. Accessibility and documentation

### Decision

Alle betroffenen Governance-, Lern-, Betriebs- und Evidenztexte sind deutsch zuerst und englisch danach auf CEFR-B2-Niveau. Status, Begründung, Evidenz, Risiko und nächste Aktion bleiben als Text verständlich. Beschreibende Links, sprachmarkierte Codeblöcke und Textalternativen werden geprüft.

*All affected governance, learning, operations, and evidence text is German first and English second at CEFR B2. Status, rationale, evidence, risk, and next action remain understandable as text.*

Dokumentationswirkung ist `UpdateRequired`. Da `docs/**` im DocFX-Publishing-Scope liegt, ist DocFX mit Repository-Postprocessing, Axe, Lynx und manueller Textprüfung anwendbar. Öffentliche API-/XML-Änderungen lösen zusätzlich vollständige zweisprachige XML-Dokumentation aus. `api/` und `_site/` bleiben untracked.

*Documentation impact is `UpdateRequired`. Because `docs/**` is in the DocFX publishing scope, DocFX, repository post-processing, Axe, Lynx, and manual text review apply. Public API/XML changes additionally require complete bilingual XML documentation.*

## 11. Testing, coverage, and versioning

### Decision

- Restore, Build und Tests verwenden .NET 10.
- Mindestabdeckung ist 70%; Ziel sind mindestens 80%.
- Externe Provider erhalten nur für tatsächlich ausgeführte Umgebungen positive Evidenz.
- Fehlende MongoDB-/PostgreSQL- oder Plattformumgebungen bleiben sichtbare Blocker oder begrenzte, begründete Findings; sie werden nicht als Pass übernommen.

*Restore, build, and tests use .NET 10; coverage must be at least 70% and targets 80%; provider/platform claims are limited to executed environments.*

Versionsschema ist `Major.Minor.Patch.Build`: Minor `2`; Patch entspricht der Feature-Branch-Commitanzahl nach dem jeweiligen Commit; Build wird vor jedem einzelnen `dotnet build` oder `dotnet test` erhöht. `Version`, `AssemblyVersion` und `FileVersion` bleiben gleich.

*Versioning is `Major.Minor.Patch.Build`: Minor 2; Patch equals the feature-branch commit count after each commit; Build increments before every individual build or test; all three fields remain aligned.*

## 12. Autonomous authority and delivery

### Decision

Der gespeicherte Delivery-Modus ist `MergeAndSync`. Diese Planungsphase erzeugt nur Feature-Planungsartefakte. Commit, Push, PR, Review-Konvergenz, Merge, Default-Branch-Sync, Closeout und Run-State-Änderungen bleiben beim äußeren Koordinator.

*The stored delivery mode is `MergeAndSync`. This planning phase creates feature planning artefacts only. Commit, push, PR, review convergence, merge, default-branch sync, closeout, and run-state changes remain coordinator-owned.*

Die optionalen Spec-Kit-Auto-Commit-Hooks sind deaktiviert und dürfen keine Commits erzeugen. Die Nutzerfreigabe für Admin-Bypass gilt ausschließlich für eine Provider-Policy, nachdem alle technischen Gates am exakten Kandidaten-Head bestanden sind. Sie ersetzt niemals Tests, Review, Security-, Coverage-, A11Y-, Supply-Chain- oder Integritätsnachweis.

*Optional Spec-Kit auto-commit hooks are disabled and may not create commits. User authorization for Admin-Bypass applies only to provider policy after all technical gates pass at the exact candidate head. It never replaces technical proof.*

Gemeinsame Writer werden serialisiert: Baseline/Evidenz, `Directory.Build.props`, Workflows/Toolmanifest, Agenten-/Constitution-/Template-Parität, Statistik sowie PreMerge/PostMerge-Closeout erhalten eine feste Abhängigkeitskette. Die temporären Schema-2.0-Evidenzdateien liegen unter `.specify/runtime/autonomous-routing/d6d5e58e-0acc-404c-b5b4-4f97eba83c9f/` und werden vor dem Merge nicht committet.

*Shared writers are serialized. Temporary schema-2.0 PreMerge/PostMerge evidence stays in the current untracked runtime directory so it does not invalidate the candidate head.*

## 13. Research outcome

Alle technischen Unklarheiten, die den Plan blockieren würden, sind entschieden. Beobachtete Repository-Lücken sind absichtlich keine positiven Aussagen; sie werden im Implementierungslauf durch die definierten fail-closed Gates bewertet und minimal behoben oder als vollständige blockierende Findings geführt.

*All technical questions that could block planning are resolved. Observed repository gaps are deliberately not positive claims; implementation evaluates them through fail-closed gates and either fixes them minimally or records complete blocking findings.*
