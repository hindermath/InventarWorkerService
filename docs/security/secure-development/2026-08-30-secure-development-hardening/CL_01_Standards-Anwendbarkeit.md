# CL-01: Standards Anwendbarkeit

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
| <a id="cl-01-01"></a>CL-01-01 | NIST SSDF und CWE Top 25 / NIST SSDF and CWE Top 25 | Open / Not Assessed | `EV-CL0101-REVIEW` | `FIND-080`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-02"></a>CL-01-02 | OWASP ASVS-Level / OWASP ASVS Level | Open / Not Assessed | `EV-CL0102-REVIEW` | `FIND-081`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-03"></a>CL-01-03 | SBOM / Software Bill of Materials | Open / Not Assessed | `EV-CL0103-REVIEW` | `FIND-082`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-04"></a>CL-01-04 | VEX / Vulnerability Exploitability eXchange | Open / Not Assessed | `EV-CL0104-REVIEW` | `FIND-083`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-05"></a>CL-01-05 | SLSA-Build-Level / SLSA Build Level | Open / Not Assessed | `EV-CL0105-REVIEW` | `FIND-084`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-06"></a>CL-01-06 | Zero Trust / Zero Trust | Open / Not Assessed | `EV-CL0106-REVIEW` | `FIND-085`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-07"></a>CL-01-07 | CAPEC im Bedrohungsmodell / CAPEC in the Threat Model | Open / Not Assessed | `EV-CL0107-REVIEW` | `FIND-086`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-08"></a>CL-01-08 | OWASP SAMM / OWASP SAMM | Open / Not Assessed | `EV-CL0108-REVIEW` | `FIND-087`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-09"></a>CL-01-09 | OpenSSF Scorecard / OpenSSF Scorecard | Open / Not Assessed | `EV-CL0109-REVIEW` | `FIND-088`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-10"></a>CL-01-10 | OWASP Cheat Sheets und Proactive Controls / OWASP Cheat Sheets and Proactive Controls | Open / Not Assessed | `EV-CL0110-REVIEW` | `FIND-089`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-11"></a>CL-01-11 | Nichtanwendbarkeit dokumentieren / Document Non-Applicability | Open / Not Assessed | `EV-CL0111-REVIEW` | `FIND-090`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-01-12"></a>CL-01-12 | Regulatorische Anwendbarkeit / Regulatory Applicability | Open / Not Assessed | `EV-CL0112-REVIEW` | `FIND-091`; fällig 2026-09-30 / due 2026-09-30 |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
