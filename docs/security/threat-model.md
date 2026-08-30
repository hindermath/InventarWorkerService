# Bedrohungsmodell / Threat Model

**Methode / Method:** CIA, STRIDE und CAPEC
**Stand / Date:** 2026-08-30

## Assets und neun Trust Boundaries / Assets and Nine Trust Boundaries

DE: Geschützt werden Inventar, Maschinen-/Nutzer-/Domäneninformationen, Datenbanken, Connection Strings, Konfiguration, Status/Logs, Releases und Governance-Evidenz.

EN: Protected assets are inventory, machine/user/domain information, databases, connection strings, configuration, status/logs, releases, and governance evidence.

| ID | Grenze / Boundary | Hauptrisiko / Main risk | Unabhängige Kontrollen / Independent controls |
|---|---|---|---|
| TB-01 | Client → Worker HTTP | Spoofing, disclosure; CAPEC-115 | AuthZ + TLS; aktuell Open / currently open |
| TB-02 | Client → Viewer HTTP | IDOR, disclosure; CAPEC-122 | AuthZ + Eingabegrenzen / input bounds |
| TB-03 | Harvester → Worker | SSRF, timeout; CAPEC-664 | Ziel-Allowlist + Timeout/Cancellation |
| TB-04 | Dienste → SQLite | SQL-Injection, Korruption | Parameter + Transaktion/Recovery |
| TB-05 | Dienste → PostgreSQL | Rechteausweitung | Parameter + Least-Privilege-Rolle |
| TB-06 | Dienste → MongoDB | Query-/Credential-Missbrauch | typisierte Filter + Least Privilege |
| TB-07 | Prozess → Dateien/CSV | Pfad-/Teilwrite; CAPEC-126 | feste Basis + atomarer Ersatz |
| TB-08 | Service-Control → OS | Command Injection; CAPEC-88 | Name-Allowlist + ArgumentList |
| TB-09 | Repository → CI/Release | Supply Chain; CAPEC-438 | gepinnte Tools + Hash/SBOM/Scans |

DE: Credentials haben hohe Vertraulichkeit; Inventar ist mittel. Datenbanken, Status und Releases benötigen hohe Integrität und Verfügbarkeit. Hohe Pfade benötigen zwei Kontrollen. Teilwrites ersetzen das Ziel erst nach Erfolg; lesende Fehler brauchen keinen Rollback.

EN: Credentials have high confidentiality; inventory is medium. Databases, status, and releases need high integrity and availability. High paths need two controls. Partial writes replace the target only after success; read-only failures need no rollback.
