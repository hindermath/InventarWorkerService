# S-ADR 002: API-Zugriff standardmäßig verweigern / Deny API Access by Default

**Status:** Accepted for implementation, 2026-08-30

DE: Inventar- und Statusdaten dürfen nicht allein aufgrund einer erreichbaren Netzwerkadresse gelesen werden. Beide Hosts verwenden eine zentrale Autorisierungspolicy. Development-Dokumentation bleibt lokal begrenzt.

EN: Inventory and status data must not be readable merely because a network address is reachable. Both hosts use a central authorization policy. Development documentation remains locally restricted.

**Folge / Consequence:** Nicht authentifizierte Aufrufe erhalten 401/403; Deployment muss Credentials und TLS bereitstellen. / Unauthenticated calls receive 401/403; deployment must provide credentials and TLS.
