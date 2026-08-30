# CL-12: Agentische KI Sandbox

**Projekt / Project:** InventarWorkerService
**Feature:** 002-secure-development-hardening
**Prüfdatum / Review date:** 2026-08-30
**Baseline / Checklist:** 3.1.0 / 2.2.0
**Owner / Reviewer:** Projektverantwortung / Security-Reviewer

## Bewertungsregel / Assessment Rule

DE: Jeder Prüfpunkt ist gegen den lokalen Feature-Scope bewertet. `Open` ist
kein positives Ergebnis und besitzt Finding, Risiko, Folgeschritt, Termin und
Trigger. `N/A` enthält keine Pass-Behauptung und wird bei Scope-Änderung neu
bewertet. Die vollständigen Frische- und Integritätsfelder stehen in
`assessment-records.json`.

EN: Every checkpoint is assessed against the local feature scope. `Open` is
not a positive result and carries a finding, risk, action, due date, and trigger.
`N/A` makes no pass claim and is re-evaluated when scope changes. Complete
freshness and integrity fields are stored in `assessment-records.json`.

## Prüfpunktmatrix / Checkpoint Matrix

| ID | Prüfpunkt / Checkpoint | Klassifikation / Classification | Evidenz / Evidence | Finding / Disposition |
|---|---|---|---|---|
| <a id="cl-12-01"></a>CL-12-01 | Initialfreigabe der Sandbox / Initial Sandbox Approval | N/A / Not Assessed | `EV-CL1201-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-12-02"></a>CL-12-02 | Begrenzte Host-Mounts / Limited Host Mounts | N/A / Not Assessed | `EV-CL1202-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-12-03"></a>CL-12-03 | Trennung von Agentendaten und Projektcode / Separation of Agent Data and Project Code | N/A / Not Assessed | `EV-CL1203-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-12-04"></a>CL-12-04 | Schutz von Geheimnissen / Secrets Protection | Open / Not Assessed | `EV-CL1204-REVIEW` | `FIND-001`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-12-05"></a>CL-12-05 | Genehmigte und gepinnte Werkzeuge und Modelle / Approved and Pinned Tools and Models | N/A / Not Assessed | `EV-CL1205-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-12-06"></a>CL-12-06 | GitHub Spec Kit und Governance-Presets / GitHub Spec Kit and Governance Presets | Open / Not Assessed | `EV-CL1206-REVIEW` | `FIND-002`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-12-07"></a>CL-12-07 | Menschliche Prüfung / Human Review | Open / Not Assessed | `EV-CL1207-REVIEW` | `FIND-003`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-12-08"></a>CL-12-08 | Audit-Spur und Nachvollziehbarkeit / Audit Trail and Traceability | Open / Not Assessed | `EV-CL1208-REVIEW` | `FIND-004`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-12-09"></a>CL-12-09 | Sandbox-Typologie und Isolationsnachweis / Sandbox Typology and Isolation Evidence | N/A / Not Assessed | `EV-CL1209-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-12-10"></a>CL-12-10 | Netzwerkrestriktion / Network Restriction | N/A / Not Assessed | `EV-CL1210-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-12-11"></a>CL-12-11 | Re-Validierungsstand und Lebenszyklus / Re-Validation Status and Lifecycle | Open / Not Assessed | `EV-CL1211-REVIEW` | `FIND-005`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-12-12"></a>CL-12-12 | Preset-Aktualisierung und Inhaltsabdeckung / Preset Updates and Content Coverage | Open / Not Assessed | `EV-CL1212-REVIEW` | `FIND-006`; fällig 2026-09-30 / due 2026-09-30 |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
