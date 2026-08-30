# CL-09: KI Codeerzeugung

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
| <a id="cl-09-01"></a>CL-09-01 | Genehmigte KI-Werkzeuge / Approved AI Tools | Open / Not Assessed | `EV-CL0901-REVIEW` | `FIND-065`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-02"></a>CL-09-02 | Pflicht zur menschlichen Überprüfung / Mandatory Human Review | Open / Not Assessed | `EV-CL0902-REVIEW` | `FIND-066`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-03"></a>CL-09-03 | Vier-Augen-Prinzip bei kritischer Logik / Four-Eyes Rule for Critical Logic | Open / Not Assessed | `EV-CL0903-REVIEW` | `FIND-067`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-04"></a>CL-09-04 | CVE-Prüfung für vorgeschlagene Abhängigkeiten / CVE Check for Suggested Dependencies | Open / Not Assessed | `EV-CL0904-REVIEW` | `FIND-068`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-05"></a>CL-09-05 | Schutz vor halluzinierten Paketen / Protection Against Hallucinated Packages | Open / Not Assessed | `EV-CL0905-REVIEW` | `FIND-069`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-06"></a>CL-09-06 | Lizenz-Klärung / Licence Clearance | Open / Not Assessed | `EV-CL0906-REVIEW` | `FIND-070`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-07"></a>CL-09-07 | Datenschutz in Prompts / Data Protection in Prompts | Open / Not Assessed | `EV-CL0907-REVIEW` | `FIND-071`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-08"></a>CL-09-08 | Keine eigene Krypto durch KI / No Custom Crypto from AI | Open / Not Assessed | `EV-CL0908-REVIEW` | `FIND-072`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-09"></a>CL-09-09 | Tests für KI-erzeugten Code / Tests for AI-Generated Code | Open / Not Assessed | `EV-CL0909-REVIEW` | `FIND-073`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-10"></a>CL-09-10 | Markierung im PR / Mark in the PR | Open / Not Assessed | `EV-CL0910-REVIEW` | `FIND-074`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-11"></a>CL-09-11 | Werkzeug-Konfiguration und Telemetrie / Tool Configuration and Telemetry | Open / Not Assessed | `EV-CL0911-REVIEW` | `FIND-075`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-12"></a>CL-09-12 | Schulung / Training | Open / Not Assessed | `EV-CL0912-REVIEW` | `FIND-076`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-13"></a>CL-09-13 | Audit-Spur der Werkzeugnutzung / Audit Trail of Tool Usage | Open / Not Assessed | `EV-CL0913-REVIEW` | `FIND-077`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-14"></a>CL-09-14 | Ausnahmen und Risikoakzeptanz / Exceptions and Risk Acceptance | Open / Not Assessed | `EV-CL0914-REVIEW` | `FIND-078`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-09-15"></a>CL-09-15 | KI-Lieferkettentransparenz / AI Supply-Chain Transparency | N/A / Not Assessed | `EV-CL0915-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-09-16"></a>CL-09-16 | KI-Regulierungs-Screening / AI Regulatory Screening | N/A / Not Assessed | `EV-CL0916-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-09-17"></a>CL-09-17 | Didaktische Kommentare bei nichttrivialer Logik / Didactic Comments for Non-Trivial Logic | Open / Not Assessed | `EV-CL0917-REVIEW` | `FIND-079`; fällig 2026-09-30 / due 2026-09-30 |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
