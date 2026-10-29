# OWASP-ASVS-Verifikation / OWASP ASVS Verification

**Version und Level / Version and level:** OWASP ASVS 5.0 Level 2
**Hosts:** InventarWorkerService und InventarViewerApp Web API

DE: Worker-Scope: hardware, software, full, status. Viewer-Scope: Maschinenliste, ID-/Namensauflösung, Hardware-/Softwarepfade, hardware-overview. Swagger/OpenAPI und statische DocFX-Pfade gehören dazu.

EN: Worker scope: hardware, software, full, status. Viewer scope: machine list, ID/name lookup, hardware/software paths, hardware-overview. Swagger/OpenAPI and static DocFX paths are included.

| Cluster | Status | Finding / Maßnahme |
|---|---|---|
| AuthN/AuthZ | Local Pass | FIND-HTTP-001: API-Key-Fallback-Policy an beiden Hosts; Provider-/Deploymentnachweis separat |
| Transport | Local Pass | FIND-HTTP-002: Listener-Gate erlaubt HTTP nur auf Loopback und verlangt remote HTTPS; Redirect ist nicht erforderlich |
| Eingabe / Input | Open | FIND-HTTP-003: Namen ohne explizite Grenzen / names lack bounds |
| Fehler/Logging | Open | FIND-HTTP-004: ex.Message außen; generische Fehler / internal message exposed |
| Konfiguration | Partly Fulfilled | Listener-Defaults sind Loopback-only; Zertifikat und API-Key bleiben externe Secrets / listener defaults are loopback-only; certificate and API key remain external secrets |
| Swagger/Docs | Local Pass | Swagger wird nur in Development aktiviert; entfernter Zugriff benötigt HTTPS und API-Key / remote access requires HTTPS and API key |

DE: CSRF/Anti-Forgery ist für GET-only N/A; Trigger ist ein zustandsändernder Cookie-Endpunkt. Owner Entwicklung, Reviewer Security, Termin 2026-09-30.

EN: CSRF/anti-forgery is N/A for GET-only APIs; a state-changing cookie endpoint is the trigger. Owner development, reviewer security, due 2026-09-30.

DE: `Local Pass` gilt für Quellcode, Konfigurationsvertrag und lokale Tests. Ein konkretes Produktionszertifikat, eine Proxy-Konfiguration und Provider-Matrix sind Deployment-Evidenz und werden nicht als lokal ausgeführt behauptet.

EN: `Local Pass` covers source code, the configuration contract, and local tests. A concrete production certificate, proxy configuration, and provider matrix are deployment evidence and are not claimed as locally executed.
