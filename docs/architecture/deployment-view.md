# Deployment-Sicht / Deployment View

| Umgebung / Environment | Prozesse und Ports / Processes and ports | Secrets und Rechte / Secrets and privilege |
|---|---|---|
| Windows | Worker als Service, Viewer/Harvester; Kestrel konfiguriert | Servicekonto mit minimalen Rechten; Credential Store |
| macOS | launchd/Benutzerprozess | Keychain; begrenzte LaunchAgent-Pfade |
| Linux | systemd/Benutzerprozess | systemd credentials/environment; keine Root-Pflicht |
| GitHub Actions | .NET 10 Jobs; Provider-Container | read-only Checkout; kurzlebige Tokens; keine Secrets in Logs |
| Datenbanken | SQLite-Datei, PostgreSQL, MongoDB | eigene Rollen, minimale DB-/Dateirechte |

DE: Beide API-Hosts starten standardmäßig nur mit einer Loopback-HTTP-Bindung. Sie unterstützen zwei Betriebsarten: direktes Kestrel-HTTPS mit extern bereitgestelltem Zertifikat oder TLS-Terminierung an einem Reverse Proxy auf demselben Host, der nur an Loopback-HTTP weiterleitet. Remote- und Wildcard-HTTP sowie reine `HTTP_PORTS`-Konfigurationen werden vor dem Listener-Start abgewiesen. Swagger ist nur in Development aktiv. Debug-Funktionen sind in Release aus.

EN: Both API hosts default to a loopback-only HTTP binding. They support two deployment modes: direct Kestrel HTTPS with an externally supplied certificate, or TLS termination at a reverse proxy on the same host that forwards only to loopback HTTP. Remote and wildcard HTTP and port-only `HTTP_PORTS` configurations are rejected before listener startup. Swagger is active only in Development. Debug features are disabled in Release.
