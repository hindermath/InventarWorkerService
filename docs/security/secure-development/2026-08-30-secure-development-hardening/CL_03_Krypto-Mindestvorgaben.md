# CL-03: Krypto Mindestvorgaben

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
| <a id="cl-03-01"></a>CL-03-01 | Symmetrische Verschlüsselung / Symmetric Encryption | N/A / Not Assessed | `EV-CL0301-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-02"></a>CL-03-02 | Asymmetrische Verschlüsselung und Signatur / Asymmetric Encryption and Signature | N/A / Not Assessed | `EV-CL0302-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-03"></a>CL-03-03 | Hash-Funktionen / Hash Functions | N/A / Not Assessed | `EV-CL0303-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-04"></a>CL-03-04 | Passwort-Hashing / Password Hashing | N/A / Not Assessed | `EV-CL0304-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-05"></a>CL-03-05 | Message Authentication Code (MAC) / MAC | N/A / Not Assessed | `EV-CL0305-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-06"></a>CL-03-06 | Verbotene oder eingeschränkte Verfahren / Forbidden or Restricted Algorithms | N/A / Not Assessed | `EV-CL0306-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-07"></a>CL-03-07 | TLS-Konfiguration / TLS Configuration | Open / Not Assessed | `EV-CL0307-REVIEW` | `FIND-036`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-03-08"></a>CL-03-08 | Zufallszahlen / Random Numbers | N/A / Not Assessed | `EV-CL0308-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-09"></a>CL-03-09 | Schlüsselverwaltung / Key Management | N/A / Not Assessed | `EV-CL0309-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-10"></a>CL-03-10 | Schlüsselrotation / Key Rotation | N/A / Not Assessed | `EV-CL0310-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-11"></a>CL-03-11 | Keine eigene Krypto / No Custom Crypto | Open / Not Assessed | `EV-CL0311-REVIEW` | `FIND-037`; fällig 2026-09-30 / due 2026-09-30 |
| <a id="cl-03-12"></a>CL-03-12 | Hardware-Unterstützung / Hardware Support | N/A / Not Assessed | `EV-CL0312-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-13"></a>CL-03-13 | Krypto-Agilität / Crypto Agility | N/A / Not Assessed | `EV-CL0313-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-14"></a>CL-03-14 | Vorbereitung auf Post-Quanten-Krypto / Preparing for Post-Quantum Crypto | N/A / Not Assessed | `EV-CL0314-REVIEW` | N/A; kein lokaler Schritt bis zum Trigger / no local action until trigger |
| <a id="cl-03-15"></a>CL-03-15 | Audit der Krypto-Nutzung / Crypto Usage Audit | Open / Not Assessed | `EV-CL0315-REVIEW` | `FIND-038`; fällig 2026-09-30 / due 2026-09-30 |


## Restrisiko und Trigger / Residual Risk and Trigger

DE: Offene Punkte dürfen erst nach aktueller, scopegenauer Evidenz positiv
bewertet werden. Nichtanwendbare Punkte werden bei Änderungen an Produkt,
Deployment, Organisation, Cloud, KI-Runtime, Datenschutz, Kryptografie oder
Skripten erneut geprüft.

EN: Open items may become positive only after current, scope-specific evidence.
N/A items are re-evaluated after changes to product, deployment, organisation,
cloud, AI runtime, privacy, cryptography, or scripts.
