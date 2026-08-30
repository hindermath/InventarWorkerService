# OWASP-ASVS-Verifikation / OWASP ASVS Verification

**Version und Level / Version and level:** OWASP ASVS 5.0 Level 2
**Hosts:** InventarWorkerService und InventarViewerApp Web API

DE: Worker-Scope: hardware, software, full, status. Viewer-Scope: Maschinenliste, ID-/Namensauflösung, Hardware-/Softwarepfade, hardware-overview. Swagger/OpenAPI und statische DocFX-Pfade gehören dazu.

EN: Worker scope: hardware, software, full, status. Viewer scope: machine list, ID/name lookup, hardware/software paths, hardware-overview. Swagger/OpenAPI and static DocFX paths are included.

| Cluster | Status | Finding / Maßnahme |
|---|---|---|
| AuthN/AuthZ | Open | FIND-HTTP-001: keine wirksame Policy; P0 / no effective policy |
| Transport | Open | FIND-HTTP-002: HTTPS-Weiterleitung deaktiviert / redirect disabled |
| Eingabe / Input | Open | FIND-HTTP-003: Namen ohne explizite Grenzen / names lack bounds |
| Fehler/Logging | Open | FIND-HTTP-004: ex.Message außen; generische Fehler / internal message exposed |
| Konfiguration | Open | Secret-, Host- und Swagger-Defaults prüfen / review defaults |
| Swagger/Docs | Open | nur Development oder autorisierte lokale Bindung / Development or authorized local binding only |

DE: CSRF/Anti-Forgery ist für GET-only N/A; Trigger ist ein zustandsändernder Cookie-Endpunkt. Owner Entwicklung, Reviewer Security, Termin 2026-09-30.

EN: CSRF/anti-forgery is N/A for GET-only APIs; a state-changing cookie endpoint is the trigger. Owner development, reviewer security, due 2026-09-30.
