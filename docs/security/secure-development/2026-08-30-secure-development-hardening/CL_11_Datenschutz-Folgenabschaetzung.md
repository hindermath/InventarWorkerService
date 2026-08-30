# CL-11: Datenschutz Folgenabschaetzung

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
| <a id="cl-11-01"></a>CL-11-01 | Schwellwertanalyse / Threshold Analysis | Open / Not Assessed | `EV-CL1101-REVIEW` | `FIND-019`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-11-02"></a>CL-11-02 | Systematische Beschreibung der Verarbeitung / Systematic Description of Processing | Open / Not Assessed | `EV-CL1102-REVIEW` | `FIND-020`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-11-03"></a>CL-11-03 | Notwendigkeits- und Verhältnismäßigkeitsbewertung / Necessity and Proportionality Assessment | Open / Not Assessed | `EV-CL1103-REVIEW` | `FIND-021`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-11-04"></a>CL-11-04 | Risikobewertung für Betroffene / Risk Assessment for Data Subjects | Open / Not Assessed | `EV-CL1104-REVIEW` | `FIND-022`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-11-05"></a>CL-11-05 | Geplante Abhilfemaßnahmen / Planned Mitigation Measures | Open / Not Assessed | `EV-CL1105-REVIEW` | `FIND-023`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-11-06"></a>CL-11-06 | Konsultation der oder des Datenschutzbeauftragten / Consultation of the Data Protection Officer | N/A / Not Assessed | `EV-CL1106-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-11-07"></a>CL-11-07 | Konsultation der Aufsichtsbehörde / Consultation of the Supervisory Authority | N/A / Not Assessed | `EV-CL1107-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-11-08"></a>CL-11-08 | Verzeichnis der Verarbeitungstätigkeiten / Record of Processing Activities | N/A / Not Assessed | `EV-CL1108-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-11-09"></a>CL-11-09 | Auftragsverarbeitung / Processor Agreements | N/A / Not Assessed | `EV-CL1109-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-11-10"></a>CL-11-10 | Datenschutz durch Technikgestaltung und Voreinstellungen / Privacy by Design and by Default | N/A / Not Assessed | `EV-CL1110-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-11-11"></a>CL-11-11 | Betroffenenrechte / Data Subject Rights | N/A / Not Assessed | `EV-CL1111-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-11-12"></a>CL-11-12 | Fortschreibung und Aktualisierung / Maintenance and Update | Open / Not Assessed | `EV-CL1112-REVIEW` | `FIND-024`; fällig 2026-09-30 / due 2026-09-30 |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
