# Qualitäts-Szenarien / Quality Scenarios

| Qualität / Quality | Reiz / Stimulus | Messbare Antwort / Measurable response |
|---|---|---|
| Sicherheit | unberechtigter/fehlerhafter Aufruf | 401/403/400; keine Interna |
| Zuverlässigkeit | Provider oder Datei fällt aus | sicherer Restzustand; konkrete Recovery |
| Wartbarkeit | neue Grenze/Dependency | Matrix, Finding und Trigger in einem Review |
| Testbarkeit | Security-Finding | reproduzierbarer RED/GREEN-Test |
| A11Y | Screenreader/Lynx | Status/Entscheidung vollständig als Text |
| Plattformparität | Windows/macOS/Linux | nur ausgeführte Plattform wird positiv belegt |

DE: Ein allgemeiner ADR ist N/A, solange keine nicht-sicherheitsspezifische Strukturentscheidung entsteht. Trigger sind neue Komponenten, Schnittstellen oder Deploymentformen.

EN: A general ADR is N/A while no non-security structural decision is introduced. New components, interfaces, or deployment forms are the trigger.
