# Kontextsicht / Context View

DE: InventarWorkerService sammelt lokale Hardware- und Softwaredaten. Harvester und Viewer lesen sie über HTTP. Viewer und Harvester speichern in SQLite, PostgreSQL oder MongoDB. Betriebssystemdienste, Dateien, CI und Dokumentationsnutzer bilden externe Kontexte.

EN: InventarWorkerService collects local hardware and software data. Harvester and Viewer read it over HTTP. Viewer and Harvester store it in SQLite, PostgreSQL, or MongoDB. Operating-system services, files, CI, and documentation readers are external contexts.

| Akteur/System / Actor/System | Datenklasse / Data class | Trust Boundary |
|---|---|---|
| Administrator:in / administrator | Credentials, Konfiguration | TB-07/TB-08 |
| Worker-Client / client | Inventar, Status | TB-01 |
| Harvester | Inventar, Provider-Credentials | TB-03–TB-06 |
| Viewer/API-Client | Inventar, Maschinenbezug | TB-02/TB-04 |
| CI/Release | Source, Pakete, SBOM | TB-09 |
| Dokumentationsleser:in / reader | öffentliche Evidenz / public evidence | DocFX/A11Y |

DE: Authentifizierung, Autorisierung, TLS, Validierung, Secret-Schutz und Recovery werden an der jeweiligen Grenze geprüft; keine Grenze erbt einen Pass von einer anderen.

EN: Authentication, authorization, TLS, validation, secret protection, and recovery are reviewed at each boundary; no boundary inherits a pass from another.
