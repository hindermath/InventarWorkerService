# Deployment-Sicht / Deployment View

| Umgebung / Environment | Prozesse und Ports / Processes and ports | Secrets und Rechte / Secrets and privilege |
|---|---|---|
| Windows | Worker als Service, Viewer/Harvester; Kestrel konfiguriert | Servicekonto mit minimalen Rechten; Credential Store |
| macOS | launchd/Benutzerprozess | Keychain; begrenzte LaunchAgent-Pfade |
| Linux | systemd/Benutzerprozess | systemd credentials/environment; keine Root-Pflicht |
| GitHub Actions | .NET 10 Jobs; Provider-Container | read-only Checkout; kurzlebige Tokens; keine Secrets in Logs |
| Datenbanken | SQLite-Datei, PostgreSQL, MongoDB | eigene Rollen, minimale DB-/Dateirechte |

DE: Externe HTTP-Bindungen benötigen TLS und Zugriffskontrolle. Swagger ist nur in Development oder einer ausdrücklich lokalen administrativen Bindung aktiv. Debug-Funktionen sind in Release aus.

EN: External HTTP bindings require TLS and access control. Swagger is active only in Development or an explicitly local administrative binding. Debug features are disabled in Release.
