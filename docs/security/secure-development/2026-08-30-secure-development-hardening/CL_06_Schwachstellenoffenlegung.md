# CL-06: Schwachstellenoffenlegung

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
| <a id="cl-06-01"></a>CL-06-01 | Veröffentlichte CVD-Richtlinie / Published CVD Policy | Open / Not Assessed | `EV-CL0601-REVIEW` | `FIND-092`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-06-02"></a>CL-06-02 | security.txt nach RFC 9116 / security.txt per RFC 9116 | Open / Not Assessed | `EV-CL0602-REVIEW` | `FIND-093`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-06-03"></a>CL-06-03 | Eingangskanal und Postfach / Reporting Channel and Mailbox | Open / Not Assessed | `EV-CL0603-REVIEW` | `FIND-094`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-06-04"></a>CL-06-04 | Triage und Schweregrad / Triage and Severity | Open / Not Assessed | `EV-CL0604-REVIEW` | `FIND-095`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-06-05"></a>CL-06-05 | Reaktionsfristen / Response SLAs | Open / Not Assessed | `EV-CL0605-REVIEW` | `FIND-096`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-06-06"></a>CL-06-06 | Koordinierte Veröffentlichung / Coordinated Disclosure | Open / Not Assessed | `EV-CL0606-REVIEW` | `FIND-097`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-06-07"></a>CL-06-07 | CRA-Pflichtmeldung 24 Stunden / CRA 24-Hour Notification | Open / Not Assessed | `EV-CL0607-REVIEW` | `FIND-098`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-06-08"></a>CL-06-08 | Anwendererklärung und Hinweise / User Notification and Advisories | Open / Not Assessed | `EV-CL0608-REVIEW` | `FIND-099`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-06-09"></a>CL-06-09 | Patch-Bereitstellung / Patch Availability | Open / Not Assessed | `EV-CL0609-REVIEW` | `FIND-100`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-06-10"></a>CL-06-10 | Nachverfolgung und Lessons Learned / Tracking and Lessons Learned | Open / Not Assessed | `EV-CL0610-REVIEW` | `FIND-101`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-06-11"></a>CL-06-11 | Übungen und Tests / Drills and Tests | Open / Not Assessed | `EV-CL0611-REVIEW` | `FIND-102`; fällig 2026-09-30 / due 2026-09-30 |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
