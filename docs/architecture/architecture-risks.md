# Architekturrisiken / Architecture Risks

| ID | Risiko / Risk | Status | Owner / Termin / Trigger |
|---|---|---|---|
| AR-01 | HTTP ohne wirksame AuthZ/TLS / HTTP without effective AuthZ/TLS | Open, High | Entwicklung / 2026-09-30 / Host- oder Deploymentänderung |
| AR-02 | Outbound-Ziele und Timeouts / outbound targets and timeouts | Open, High | Entwicklung / 2026-09-30 / neue Zieladresse |
| AR-03 | Provider-Parität ohne gleiche Umgebung / inferred provider parity | Open, Medium | Test / 2026-09-30 / Provideränderung |
| AR-04 | Prozessargumente aus Service-Name / process args from service name | Open, High | Entwicklung / 2026-09-30 / Service-Control change |
| AR-05 | Nicht atomare Status-/Settingswrites / non-atomic writes | Open, Medium | Entwicklung / 2026-09-30 / file format change |
| AR-06 | Paket-CVEs/SBOM-Provenance | Open, High | Release / 2026-09-30 / dependency change |
| AR-07 | Provider-/Remote-Evidenz fehlt lokal | Accepted as local limitation, not Pass | Koordinator / PreMerge / CI available |

DE: Keine offene Frage wird still geschlossen. Geringere lokale Reichweite wird als Open oder Follow-up geführt und niemals als Provider-Pass übertragen.

EN: No open question is silently closed. Limited local reach remains Open or Follow-up and is never transferred as a provider pass.
