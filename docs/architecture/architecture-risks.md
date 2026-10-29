# Architekturrisiken / Architecture Risks

| ID | Risiko / Risk | Status | Owner / Termin / Trigger |
|---|---|---|---|
| AR-01 | HTTP ohne wirksame AuthZ/TLS / HTTP without effective AuthZ/TLS | Mitigated locally / lokal entschärft | Entwicklung / erneute Prüfung bei Host-, Proxy-, Zertifikats- oder Deploymentänderung |
| AR-02 | Outbound-Ziele und Timeouts / outbound targets and timeouts | Open, High | Entwicklung / 2026-09-30 / neue Zieladresse |
| AR-03 | Provider-Parität ohne gleiche Umgebung / inferred provider parity | Open, Medium | Test / 2026-09-30 / Provideränderung |
| AR-04 | Prozessargumente aus Service-Name / process args from service name | Open, High | Entwicklung / 2026-09-30 / Service-Control change |
| AR-05 | Nicht atomare Status-/Settingswrites / non-atomic writes | Open, Medium | Entwicklung / 2026-09-30 / file format change |
| AR-06 | Paket-CVEs/SBOM-Provenance | Open, High | Release / 2026-09-30 / dependency change |
| AR-07 | Provider-/Remote-Evidenz fehlt lokal | Accepted as local limitation, not Pass | Koordinator / PreMerge / CI available |
| AR-08 | MongoDB-Credentials oder Inventar über Remote-Klartext / MongoDB credentials or inventory over remote plaintext | Mitigated in source; provider proof open | Entwicklung / erneute Prüfung bei MongoDB-, Zertifikats- oder Deploymentänderung |

DE: Keine offene Frage wird still geschlossen. Geringere lokale Reichweite wird als Open oder Follow-up geführt und niemals als Provider-Pass übertragen.

EN: No open question is silently closed. Limited local reach remains Open or Follow-up and is never transferred as a provider pass.

DE: `AR-01` ist lokal durch zwei unabhängige Grenzen entschärft: API-Key-Authentifizierung und die vor dem Listener-Start erzwungene Transportregel. Provider- oder Deployment-Evidenz wird dadurch nicht vorweggenommen.

EN: `AR-01` is locally mitigated by two independent boundaries: API-key authentication and the transport rule enforced before listener startup. This does not pre-claim provider or deployment evidence.

DE: `AR-08` wird im Quellcode an der gemeinsamen MongoDB-Clientgrenze entschärft: Nur ein direkter, eindeutiger Loopback-Endpunkt darf Klartext verwenden; jeder entfernte Seed und jede Topologieerkennung benötigt TLS mit normaler Zertifikatsprüfung. Ein konkretes Serverzertifikat oder Provider-Deployment wird dadurch nicht als geprüft behauptet.

EN: `AR-08` is mitigated in source at the shared MongoDB client boundary: only one direct, explicit loopback endpoint may use plaintext, while every remote seed and topology discovery requires TLS with normal certificate validation. This does not claim proof for a concrete server certificate or provider deployment.
