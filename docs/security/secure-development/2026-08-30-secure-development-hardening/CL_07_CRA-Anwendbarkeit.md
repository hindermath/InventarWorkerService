# CL-07: CRA Anwendbarkeit

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
| <a id="cl-07-01"></a>CL-07-01 | Produktart bestimmen / Determine Product Class | Open / Not Assessed | `EV-CL0701-REVIEW` | `FIND-007`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-02"></a>CL-07-02 | Kritikalität nach Anhang III / Criticality per Annex III | Open / Not Assessed | `EV-CL0702-REVIEW` | `FIND-008`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-03"></a>CL-07-03 | Kritisch nach Anhang IV / Critical per Annex IV | Open / Not Assessed | `EV-CL0703-REVIEW` | `FIND-009`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-04"></a>CL-07-04 | Sicherheits-Anforderungen Anhang I Teil I / Annex I Part I Requirements | Open / Not Assessed | `EV-CL0704-REVIEW` | `FIND-010`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-05"></a>CL-07-05 | Schwachstellenbehandlung Anhang I Teil II / Annex I Part II Vulnerability Handling | Open / Not Assessed | `EV-CL0705-REVIEW` | `FIND-011`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-06"></a>CL-07-06 | Konformitätsbewertung / Conformity Assessment | Open / Not Assessed | `EV-CL0706-REVIEW` | `FIND-012`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-07"></a>CL-07-07 | CE-Kennzeichnung / CE Marking | Open / Not Assessed | `EV-CL0707-REVIEW` | `FIND-013`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-08"></a>CL-07-08 | Technische Dokumentation / Technical Documentation | Open / Not Assessed | `EV-CL0708-REVIEW` | `FIND-014`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-09"></a>CL-07-09 | Meldepflichten 24 / 72 Stunden / 24- and 72-hour Reports | Open / Not Assessed | `EV-CL0709-REVIEW` | `FIND-015`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-10"></a>CL-07-10 | Lebenszyklus und Support / Lifecycle and Support | Open / Not Assessed | `EV-CL0710-REVIEW` | `FIND-016`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-11"></a>CL-07-11 | Hersteller-Pflichten und -Verträge / Manufacturer Duties and Contracts | Open / Not Assessed | `EV-CL0711-REVIEW` | `FIND-017`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-07-12"></a>CL-07-12 | Nichtanwendbarkeit dokumentieren / Document Non-Applicability | Open / Not Assessed | `EV-CL0712-REVIEW` | `FIND-018`; fällig 2026-09-30 / due 2026-09-30 |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
