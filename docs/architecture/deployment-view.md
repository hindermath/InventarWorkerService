# Deployment-Sicht / Deployment View

| Umgebung / Environment | Prozesse und Ports / Processes and ports | Secrets und Rechte / Secrets and privilege |
|---|---|---|
| Windows | Worker als Service, Viewer/Harvester; Kestrel konfiguriert | Servicekonto mit minimalen Rechten; Credential Store |
| macOS | launchd-LaunchDaemon mit dediziertem Dienstkonto | Keychain; `/Library/Application Support/InventarWorkerService` gehört dem Dienstkonto und hat Modus 0700 |
| Linux | systemd-Systemdienst mit dediziertem Dienstkonto | `StateDirectory=InventarWorkerService`; systemd erzeugt `/var/lib/InventarWorkerService` dienstkontobesessen mit Modus 0700 |
| GitHub Actions | .NET 10 Jobs; Provider-Container | read-only Checkout; kurzlebige Tokens; keine Secrets in Logs |
| Datenbanken | SQLite-Datei, PostgreSQL, MongoDB | eigene Rollen, minimale DB-/Dateirechte |

DE: Beide API-Hosts starten standardmäßig nur mit einer Loopback-HTTP-Bindung. Sie unterstützen zwei Betriebsarten: direktes Kestrel-HTTPS mit extern bereitgestelltem Zertifikat oder TLS-Terminierung an einem Reverse Proxy auf demselben Host, der nur an Loopback-HTTP weiterleitet. Remote- und Wildcard-HTTP sowie reine `HTTP_PORTS`-Konfigurationen werden vor dem Listener-Start abgewiesen. Swagger ist nur in Development aktiv. Debug-Funktionen sind in Release aus.

EN: Both API hosts default to a loopback-only HTTP binding. They support two deployment modes: direct Kestrel HTTPS with an externally supplied certificate, or TLS termination at a reverse proxy on the same host that forwards only to loopback HTTP. Remote and wildcard HTTP and port-only `HTTP_PORTS` configurations are rejected before listener startup. Swagger is active only in Development. Debug features are disabled in Release.

DE: MongoDB darf ohne TLS nur über genau einen direkten Loopback-Endpunkt angesprochen werden. Topologieerkennung sowie entfernte Einzel-, Multi-Seed- und SRV-Ziele benötigen TLS mit normaler Serverzertifikatsprüfung. Private Zertifizierungsstellen werden über den System-Truststore bereitgestellt; unsichere TLS-Optionen sind keine Deployment-Lösung.

EN: MongoDB may be contacted without TLS only through exactly one direct loopback endpoint. Topology discovery and remote single-seed, multi-seed, and SRV targets require TLS with normal server certificate validation. Private certificate authorities are provisioned through the system trust store; insecure TLS options are not a deployment solution.

DE: PostgreSQL ohne TLS ist ausschließlich für einen direkten Loopback-Host (`localhost`, `127.0.0.1` oder `::1`) erlaubt, damit lokale CI-Container ohne Netzwerkübertragung funktionieren. Jeder entfernte Host erzwingt `SSL Mode=VerifyFull`; unsichere oder optionale TLS-Modi, GSS-Fallback und deaktivierte Zertifikatsprüfung werden zentral überschrieben. Private CAs werden über den System-Truststore oder den geschützten Settings-Wert `PgSqlRootCertificatePath` bereitgestellt.

EN: PostgreSQL without TLS is allowed only for one direct loopback host (`localhost`, `127.0.0.1`, or `::1`) so local CI containers can run without network transport. Every remote host enforces `SSL Mode=VerifyFull`; insecure or optional TLS modes, GSS fallback, and disabled certificate verification are overridden centrally. Private CAs are provided through the system trust store or the protected `PgSqlRootCertificatePath` settings value.

DE: Die Worker-Endpunkte für Live-Inventare erlauben genau einen aktiven Scan ohne Warteschlange. Parallel eintreffende authentifizierte Anfragen erhalten HTTP 429, statt zusätzliche lokale Prozesse oder Systemabfragen zu starten.

EN: Worker endpoints for live inventory allow exactly one active scan without a queue. Concurrent authenticated requests receive HTTP 429 rather than starting additional local processes or system queries.

DE: Persistenter Dienstzustand liegt auf Unix nicht in benutzerprofil- oder umgebungsvariablenabhängigen Verzeichnissen. Linux-Units MÜSSEN `StateDirectory=InventarWorkerService` und einen dedizierten, nicht privilegierten Benutzer verwenden. macOS-LaunchDaemons MÜSSEN den Zustand unter `/Library/Application Support/InventarWorkerService` mit Eigentümer des Dienstkontos und Modus `0700` bereitstellen. `SERVICESTATUSDIRECTORY` benennt nur den einzelnen Dienststamm; Pfadsyntax, Links sowie fremde oder zu offene Verzeichnisse werden abgewiesen.

EN: Persistent Unix service state does not use user-profile- or environment-dependent directories. Linux units MUST use `StateDirectory=InventarWorkerService` and a dedicated unprivileged user. macOS LaunchDaemons MUST provision state under `/Library/Application Support/InventarWorkerService`, owned by the service account with mode `0700`. `SERVICESTATUSDIRECTORY` names only the single service root; path syntax, links, and foreign-owned or overly permissive directories are rejected.

DE: Der Harvester sendet Inventar-API-Schlüssel nur an bereits aufgelöste Literal-IPs, die in `HARVESTER_ALLOWED_WORKER_TARGETS` explizit als IP oder CIDR freigegeben sind, zum Beispiel `10.20.0.0/16;2001:db8:42::/64`. Ohne Allowlist werden Ziele vor dem Netzaufruf abgewiesen. Dadurch können CSV-, Datenbank- oder DNS-Werte keine neuen Credential-Empfänger schaffen.

EN: The harvester sends inventory API keys only to already resolved literal IPs explicitly approved as an IP or CIDR in `HARVESTER_ALLOWED_WORKER_TARGETS`, for example `10.20.0.0/16;2001:db8:42::/64`. Without an allowlist, targets are rejected before the network call. CSV, database, or DNS values therefore cannot create new credential recipients.
