# CL-05: Lieferkette Build Integritaet

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
| <a id="cl-05-01"></a>CL-05-01 | SBOM-Format und -Erzeugung / SBOM Format and Generation | Open / Not Assessed | `EV-CL0501-REVIEW` | `FIND-115`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-02"></a>CL-05-02 | SBOM-Inhalt / SBOM Content | Open / Not Assessed | `EV-CL0502-REVIEW` | `FIND-116`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-03"></a>CL-05-03 | VEX bei bekannten Schwachstellen / VEX for Known Vulnerabilities | Open / Not Assessed | `EV-CL0503-REVIEW` | `FIND-117`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-04"></a>CL-05-04 | SLSA-Provenance / SLSA Provenance | Open / Not Assessed | `EV-CL0504-REVIEW` | `FIND-118`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-05"></a>CL-05-05 | Reproduzierbare Builds / Reproducible Builds | Open / Not Assessed | `EV-CL0505-REVIEW` | `FIND-119`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-06"></a>CL-05-06 | Verifizierte Registries / Verified Registries | Open / Not Assessed | `EV-CL0506-REVIEW` | `FIND-120`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-07"></a>CL-05-07 | Lock-Dateien / Lock Files | Open / Not Assessed | `EV-CL0507-REVIEW` | `FIND-121`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-08"></a>CL-05-08 | Automatisierte Aktualisierungen / Automated Updates | Open / Not Assessed | `EV-CL0508-REVIEW` | `FIND-122`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-09"></a>CL-05-09 | CVE-Überwachung / CVE Monitoring | Open / Not Assessed | `EV-CL0509-REVIEW` | `FIND-123`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-10"></a>CL-05-10 | OpenSSF Scorecard / OpenSSF Scorecard | Open / Not Assessed | `EV-CL0510-REVIEW` | `FIND-124`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-11"></a>CL-05-11 | Lizenz-Compliance / Licence Compliance | Open / Not Assessed | `EV-CL0511-REVIEW` | `FIND-125`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-12"></a>CL-05-12 | Geheimnisse im Build / Secrets in Build | Open / Not Assessed | `EV-CL0512-REVIEW` | `FIND-126`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-05-13"></a>CL-05-13 | KI-Komponenten in der SBOM / AI Components in the SBOM | N/A / Not Assessed | `EV-CL0513-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
