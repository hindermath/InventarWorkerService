# arc42 Abschnitt 8 – Sicherheitskonzepte / arc42 Section 8 – Security Concepts

DE: Zugriff wird standardmäßig verweigert, sobald eine API mehr als lokal erreichbar ist. Authentifizierung und Autorisierung sind zentraler Host-Scope. TLS wird am Proxy oder Kestrel erzwungen. Eingaben erhalten Format-, Bereichs- und Längengrenzen. Außenfehler bleiben generisch; Diagnosen gehen in Secret-freie Logs. Connection Strings kommen aus sicheren Quellen. SQL-Werte sind parametrisiert. Outbound-Ziele nutzen Allowlist, HTTPS, Timeout und Cancellation. Dateien werden atomar ersetzt.

EN: Access is denied by default whenever an API is reachable beyond localhost. Authentication and authorization are central host concerns. TLS is enforced at proxy or Kestrel. Inputs have format, range, and length limits. External errors stay generic; diagnostics go to secret-free logs. Connection strings come from secure sources. SQL values are parameterized. Outbound targets use allowlist, HTTPS, timeout, and cancellation. Files are replaced atomically.

## S-ADR / Security ADR

DE: Die materielle Entscheidung API-Zugriff standardmäßig verweigern steht in docs/security/adr/002-api-access-default-deny.md. Ein allgemeiner ADR ist N/A, weil keine nicht-sicherheitsspezifische Strukturentscheidung entsteht. Trigger ist eine neue öffentliche API- oder Deploymentstruktur.

EN: The material deny-by-default API decision is stored in docs/security/adr/002-api-access-default-deny.md. A general ADR is N/A because no non-security structural decision is introduced. A new public API or deployment structure is the trigger.
