# Sicherheits-Checkliste / Security Checklist

**Scope:** InventarWorkerService, Feature 002; NIST SSDF und CWE Top 25
**Stand / Date:** 2026-08-30
**Owner / Reviewer:** Projektverantwortung / Security-Reviewer

## Ergebnis / Result

DE: NIST SSDF PO, PS, PW und RV sind anwendbar. Planung, Threat Model, sichere C#-/SQL-Regeln, Tests, Dependency-Audit und Befundbehandlung bilden die Evidenzkette. Die sichtbaren Paket-CVEs, fehlende API-Zugriffskontrolle, deaktiviertes HTTPS, interne Fehlertexte, unbeschränkte Outbound-Ziele und Prozessargumente sind offene Findings und keine bestandenen Kontrollen.

EN: NIST SSDF PO, PS, PW, and RV apply. Planning, threat modelling, secure C#/SQL rules, tests, dependency review, and finding handling form the evidence chain. Visible package CVEs, missing API access control, disabled HTTPS, internal error text, unrestricted outbound targets, and process arguments are open findings, not passed controls.

| Bereich / Area | Status | Evidenz und Folgeschritt / Evidence and action |
|---|---|---|
| PO – Organisation | Open | Constitution, Feature-002-Artefakte; unabhängiger Review vor Closeout / independent review before closeout |
| PS – Software schützen | Open | Secret-Scans vorhanden; Transport, AuthZ und Abhängigkeiten härten / harden transport, AuthZ, dependencies |
| PW – Sichere Produktion | Open | Parametrisierte Dapper-Pfade prüfen; System.Text.Json erzwingen; Negativtests / verify Dapper paths, enforce System.Text.Json, negative tests |
| RV – Schwachstellen | Open | dotnet list --vulnerable; Findings mit VEX-Entscheidung / findings with VEX disposition |
| CWE Top 25 | Open | CWE-20, 78, 200, 918 und 862 sind relevant; Tests und minimale Korrekturen / tests and minimal fixes |

DE: Owner ist die Projektverantwortung, Reviewer ist der Security-Reviewer, Termin ist 2026-09-30. Neubewertung folgt bei Code-, Paket-, Konfigurations- oder Deploymentänderung.

EN: The project owner is accountable, the security reviewer reviews, and the due date is 2026-09-30. Re-evaluate after code, package, configuration, or deployment changes.
