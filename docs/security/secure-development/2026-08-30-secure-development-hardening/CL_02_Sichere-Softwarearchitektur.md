# CL-02: Sichere Softwarearchitektur

**Projekt / Project:** InventarWorkerService
**Feature:** 002-secure-development-hardening
**Prüfdatum / Review date:** 2026-08-30
**Baseline / Checklist:** 3.1.0 / 2.0.0
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
| <a id="cl-02-01"></a>CL-02-01 | Vertrauensgrenzen / Trust Boundaries | Open / Not Assessed | `EV-CL0201-REVIEW` | `FIND-025`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-02"></a>CL-02-02 | Tiefenverteidigung / Defense in Depth | Open / Not Assessed | `EV-CL0202-REVIEW` | `FIND-026`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-03"></a>CL-02-03 | Geringste Rechte / Least Privilege | Open / Not Assessed | `EV-CL0203-REVIEW` | `FIND-027`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-04"></a>CL-02-04 | Sichere Voreinstellungen / Fail-Safe Defaults | Open / Not Assessed | `EV-CL0204-REVIEW` | `FIND-028`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-05"></a>CL-02-05 | Angriffsfläche reduzieren / Reduce Attack Surface | Open / Not Assessed | `EV-CL0205-REVIEW` | `FIND-029`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-06"></a>CL-02-06 | Trennung von Belangen / Separation of Concerns | Open / Not Assessed | `EV-CL0206-REVIEW` | `FIND-030`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-07"></a>CL-02-07 | Sichere Konfiguration / Secure Configuration | Open / Not Assessed | `EV-CL0207-REVIEW` | `FIND-031`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-08"></a>CL-02-08 | Lieferketten-Sicherheit / Supply-Chain Security | Open / Not Assessed | `EV-CL0208-REVIEW` | `FIND-032`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-09"></a>CL-02-09 | Sprachspezifische Architekturhinweise / Language-Specific Notes | Open / Not Assessed | `EV-CL0209-REVIEW` | `FIND-033`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-10"></a>CL-02-10 | S-ADR-Pflicht / S-ADR Obligation | Open / Not Assessed | `EV-CL0210-REVIEW` | `FIND-034`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-11"></a>CL-02-11 | arc42 Abschnitt 8 / arc42 Section 8 | Open / Not Assessed | `EV-CL0211-REVIEW` | `FIND-035`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-02-12"></a>CL-02-12 | Cloud-Autonomie und digitale Souveränität / Cloud Autonomy and Digital Sovereignty | N/A / Not Assessed | `EV-CL0212-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-02-13"></a>CL-02-13 | Cloud-Compliance-Assurance / Cloud Compliance Assurance | N/A / Not Assessed | `EV-CL0213-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
