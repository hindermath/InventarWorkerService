# CL-04: Bedrohungsmodellierung

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
| <a id="cl-04-01"></a>CL-04-01 | Werte und Schutzbedarf / Assets and Protection Need | Open / Not Assessed | `EV-CL0401-REVIEW` | `FIND-039`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-04-02"></a>CL-04-02 | Vertrauensgrenzen und Datenflüsse / Trust Boundaries and Data Flows | Open / Not Assessed | `EV-CL0402-REVIEW` | `FIND-040`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-04-03"></a>CL-04-03 | STRIDE pro Element / STRIDE per Element | Open / Not Assessed | `EV-CL0403-REVIEW` | `FIND-041`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-04-04"></a>CL-04-04 | CAPEC-Zuordnung / CAPEC Mapping | Open / Not Assessed | `EV-CL0404-REVIEW` | `FIND-042`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-04-05"></a>CL-04-05 | Risikobewertung / Risk Rating | Open / Not Assessed | `EV-CL0405-REVIEW` | `FIND-043`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-04-06"></a>CL-04-06 | Maßnahmen / Mitigations | Open / Not Assessed | `EV-CL0406-REVIEW` | `FIND-044`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-04-07"></a>CL-04-07 | Verbindung zu Anforderungen / Link to Requirements | Open / Not Assessed | `EV-CL0407-REVIEW` | `FIND-045`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-04-08"></a>CL-04-08 | Aktualisierungspflicht / Update Obligation | Open / Not Assessed | `EV-CL0408-REVIEW` | `FIND-046`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-04-09"></a>CL-04-09 | Review / Review | Open / Not Assessed | `EV-CL0409-REVIEW` | `FIND-047`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-04-10"></a>CL-04-10 | Ablage und Auffindbarkeit / Storage and Findability | Open / Not Assessed | `EV-CL0410-REVIEW` | `FIND-048`; fällig 2026-09-30 / due 2026-09-30 |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
