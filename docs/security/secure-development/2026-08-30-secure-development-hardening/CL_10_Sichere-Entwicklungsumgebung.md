# CL-10: Sichere Entwicklungsumgebung

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
| <a id="cl-10-01"></a>CL-10-01 | Festplattenverschlüsselung / Full-Disk Encryption | Open / Not Assessed | `EV-CL1001-REVIEW` | `FIND-049`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-02"></a>CL-10-02 | Bildschirmsperre und MFA / Screen Lock and MFA | Open / Not Assessed | `EV-CL1002-REVIEW` | `FIND-050`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-03"></a>CL-10-03 | IDE-Härtung / IDE Hardening | Open / Not Assessed | `EV-CL1003-REVIEW` | `FIND-051`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-04"></a>CL-10-04 | Branch-Schutz / Branch Protection | Open / Not Assessed | `EV-CL1004-REVIEW` | `FIND-052`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-05"></a>CL-10-05 | Signierte Commits / Signed Commits | Open / Not Assessed | `EV-CL1005-REVIEW` | `FIND-053`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-06"></a>CL-10-06 | Secret-Scanning vor Push / Secret Scanning Before Push | Open / Not Assessed | `EV-CL1006-REVIEW` | `FIND-054`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-07"></a>CL-10-07 | Geheimnis-Speicher / Secret Store | Open / Not Assessed | `EV-CL1007-REVIEW` | `FIND-055`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-08"></a>CL-10-08 | CI/CD-Pipeline-Härtung / CI/CD Pipeline Hardening | Open / Not Assessed | `EV-CL1008-REVIEW` | `FIND-056`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-09"></a>CL-10-09 | Reproducible-CI / Reproducible CI | Open / Not Assessed | `EV-CL1009-REVIEW` | `FIND-057`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-10"></a>CL-10-10 | Trennung der Umgebungen / Environment Separation | Open / Not Assessed | `EV-CL1010-REVIEW` | `FIND-058`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-11"></a>CL-10-11 | Schutz von Testdaten / Test Data Protection | Open / Not Assessed | `EV-CL1011-REVIEW` | `FIND-059`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-12"></a>CL-10-12 | Audit-Logs der Plattform / Platform Audit Logs | Open / Not Assessed | `EV-CL1012-REVIEW` | `FIND-060`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-13"></a>CL-10-13 | Zugriffsrezertifizierung / Access Recertification | Open / Not Assessed | `EV-CL1013-REVIEW` | `FIND-061`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-14"></a>CL-10-14 | Endpoint-Schutz / Endpoint Protection | Open / Not Assessed | `EV-CL1014-REVIEW` | `FIND-062`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-15"></a>CL-10-15 | Datensicherung und Wiederherstellung / Backup and Recovery | Open / Not Assessed | `EV-CL1015-REVIEW` | `FIND-063`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-16"></a>CL-10-16 | Schulung / Training | Open / Not Assessed | `EV-CL1016-REVIEW` | `FIND-064`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-10-17"></a>CL-10-17 | Cross-Platform-Skriptparität / Cross-Platform Script Parity | N/A / Not Assessed | `EV-CL1017-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
