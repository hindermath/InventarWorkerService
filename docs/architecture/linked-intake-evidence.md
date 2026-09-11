# Architektur verlinkter Intake-Evidence / Linked Intake Evidence Architecture

## Kontext und Datenfluss / Context and data flow

```text
Kanonisches inventar-worker-service-Manifest
  + manifestgebundene Intake-Dateien
  + explizite Spec-/Run-State-/archivierte Closeout-Evidence
  -> UTF-8-, Schema-, Pfad-, Hash-, Graph- und Proof-Validierung
  -> eine typisierte Fünf-Felder-Projektion
  -> Root-Ansicht + Series-Ansicht mit relativem Linkkontext
  -> Check oder atomare Mehrdateien-Publikation mit Recheck/Rollback
```

Das Manifest bleibt die fachliche Quelle für Reihenfolge, Status, Rollen,
Wurzeln und Kanten. Beide Ansichten besitzen dieselben 17 Zeilen und 12
direkten Kanten; nur die relativen Linkziele unterscheiden sich aufgrund des
Dateistandorts. Fehlendes Feature-Evidence wird ausdrücklich angezeigt und
nicht aus Nummer oder Slug geraten.

*The manifest remains the functional source for order, status, roles, roots,
and edges. Both views contain the same 17 rows and 12 direct edges; only their
relative link targets differ by file location. Missing feature evidence is
shown explicitly and is never inferred from a number or slug.*

## Qualitätsziele / Quality goals

| Ziel | Szenario | Messbares Ergebnis |
|---|---|---|
| Integrität | Quelle, Pfad, Hash, Kante oder Proof ist ungültig. | Stabiler `LIE001`–`LIE012`-Blocker; kein Output wird verändert. |
| Determinismus | Unveränderte Eingaben werden erneut verarbeitet. | Beide Dateien bleiben bytegleich; `writes=0`. |
| Semantische Parität | Root- und Series-Datei haben verschiedene Linkbasen. | Fünf Felder sind identisch; nur das sichere relative Ziel ist kontextabhängig. |
| Wiederherstellbarkeit | Publication scheitert oder wird unterbrochen. | In-Prozess-Fehler rollen vollständig zurück; Generationshash deckt externe Teilstände auf. |
| Produktisolation | Governance-Automation wird ergänzt. | Kein Solution-, Projekt-, API-, Runtime-, Paket- oder Dependency-Diff. |
| Portabilität | Dieselben Oberflächen laufen unter Linux und Windows. | Exact-head-Proofs binden Commands, Runner, Exitcodes, Hashes und Write-Zähler. |

## Architekturentscheidung und Kommentarbedarf

Die Änderung fügt repositorynative Standardbibliotheks-Skripte hinzu und
berührt keine ausführbare Produktassembly. ADR/S-ADR und .NET-Produktbuild sind
`N/A / Not Assessed`, solange Solution- und Projektdateien unverändert bleiben;
Trigger sind neue Komponente, öffentliche API, Runtime, Dependency, Deployment
oder Trust Boundary. Kommentare erklären nur nicht offensichtliche Grenzen:
Diagnoseredaktion, historisches Rename-Evidence und atomare Publication.

*The change adds repository-native standard-library scripts and touches no
executable product assembly. ADR/S-ADR and a .NET product build are `N/A / Not
Assessed` while solution and project files stay unchanged; triggers are a new
component, public API, runtime, dependency, deployment, or trust boundary.
Comments explain only non-obvious diagnostic, historic rename-proof, and
atomic-publication boundaries.*

## Documentation Impact: `GeneratedUpdate`

| Feld | Entscheidung |
|---|---|
| Kanonische Quelle | `requirements/intakes/series/inventar-worker-service/manifest.json` plus explizite terminale Feature-Evidence |
| Owner / Reviewer | InventarWorkerService Repository Owner / Feature-032 Documentation Reviewer |
| Zielgruppen und Leserpfad | Lernende, Maintainer, Reviewer: Repository-Root → Reihenfolge → vollständiger Intake oder Feature-Nachweis |
| Navigation und Dokumentklasse | Zwei verlinkte Markdown-Referenzansichten; Security-, Architektur- und A11Y-Nachweise unter `docs/` |
| Sprachpartner | Deutsch zuerst, direkt gefolgt von Englisch; technische Literale bleiben identisch |
| Plattform-/Beispielnachweis | Lokale macOS-Fixtures; exact-head Linux-/Windows-Proofs folgen im Delivery-Checkpoint |
| Distribution / Home Sync | Repositorylokale Source- und Dokumentationsänderung; kein Home Sync |
| Evidence | Positive/negative Fixtures, `LIE001`–`LIE012`, Check/Write/Check und Null-Diff-Write |
| Re-Evaluation | Manifest-, Feld-, Link-, Sprach-, Zielgruppen-, Plattform-, Renderer- oder Distributionsänderung |
