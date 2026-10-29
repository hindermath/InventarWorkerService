# Bedrohungsmodell / Threat Model

**Methode / Method:** CIA, STRIDE und CAPEC
**Stand / Date:** 2026-10-29

## Assets und neun Trust Boundaries / Assets and Nine Trust Boundaries

DE: Geschützt werden Inventar, Maschinen-/Nutzer-/Domäneninformationen, Datenbanken, Connection Strings, Konfiguration, Status/Logs, Releases und Governance-Evidenz.

EN: Protected assets are inventory, machine/user/domain information, databases, connection strings, configuration, status/logs, releases, and governance evidence.

| ID | Grenze / Boundary | Hauptrisiko / Main risk | Unabhängige Kontrollen / Independent controls |
|---|---|---|---|
| TB-01 | Client → Worker API | Spoofing, disclosure; CAPEC-115 | API-Key-AuthZ + Fail-fast-Transportgrenze: HTTP nur Loopback, remote HTTPS |
| TB-02 | Client → Viewer API | IDOR, disclosure; CAPEC-122 | API-Key-AuthZ + dieselbe Transportgrenze; Eingabegrenzen separat Open |
| TB-03 | Harvester → Worker | SSRF, timeout; CAPEC-664 | Ziel-Allowlist + Timeout/Cancellation |
| TB-04 | Dienste → SQLite | SQL-Injection, Korruption | Parameter + Transaktion/Recovery |
| TB-05 | Dienste → PostgreSQL | Rechteausweitung | Parameter + Least-Privilege-Rolle |
| TB-06 | Dienste → MongoDB | Offenlegung, unbeabsichtigte Schreibzugriffe, Query-/Credential-Missbrauch | Remote-TLS-Gate + unveränderliches Write-Gate + typisierte Filter + Least Privilege |
| TB-07 | Prozess → Dateien/CSV | Pfad-/Teilwrite; CAPEC-126 | feste Basis + atomarer Ersatz |
| TB-08 | Service-Control → OS | Command Injection; CAPEC-88 | Name-Allowlist + ArgumentList |
| TB-09 | Repository → CI/Release | Supply Chain; CAPEC-438 | gepinnte Tools + Hash/SBOM/Scans |

DE: Credentials haben hohe Vertraulichkeit; Inventar ist mittel. Datenbanken, Status und Releases benötigen hohe Integrität und Verfügbarkeit. Hohe Pfade benötigen zwei Kontrollen. Teilwrites ersetzen das Ziel erst nach Erfolg; lesende Fehler brauchen keinen Rollback.

EN: Credentials have high confidentiality; inventory is medium. Databases, status, and releases need high integrity and availability. High paths need two controls. Partial writes replace the target only after success; read-only failures need no rollback.

DE: Die Transportgrenze wird vor dem Kestrel-Start ausgewertet. Loopback-HTTP bleibt für lokale Clients und einen Reverse Proxy auf demselben Host zulässig. Remote- und Wildcard-Bindungen benötigen HTTPS; eine unsichere oder reine `HTTP_PORTS`-Konfiguration beendet den Start. Weiterleitungen gelten nicht als Schutz für den ersten Request mit API-Key.

EN: The transport boundary is evaluated before Kestrel starts. Loopback HTTP remains available to local clients and a reverse proxy on the same host. Remote and wildcard bindings require HTTPS; an insecure or port-only `HTTP_PORTS` configuration stops startup. Redirects are not treated as protection for the first request carrying an API key.

DE: Die MongoDB-Transportgrenze wird vor dem Erzeugen des Clients ausgewertet. Klartext bleibt nur für genau einen direkten Loopback-Endpunkt zulässig. Sobald ein Seed entfernt ist oder Topologieerkennung benötigt wird, sind TLS und die normale Serverzertifikatsprüfung Pflicht; unsichere TLS-Optionen und Remote-Klartext werden fail-closed abgewiesen. Der lokale Quellnachweis ersetzt keinen konkreten Zertifikats- oder Provider-Nachweis.

EN: The MongoDB transport boundary is evaluated before the client is created. Plaintext remains available only for exactly one direct loopback endpoint. As soon as any seed is remote or topology discovery is needed, TLS and normal server certificate validation are mandatory; insecure TLS options and remote plaintext fail closed. Local source evidence does not replace concrete certificate or provider evidence.
