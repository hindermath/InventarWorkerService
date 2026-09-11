# Sicherheit verlinkter Intake-Evidence / Linked Intake Evidence Security

## Umfang und Vertrauensgrenze / Scope and trust boundary

Die dependencyfreien Bash- und PowerShell-Renderer lesen ausschließlich das
repositorylokale Inventar-Serienmanifest, dessen Intake-Dateien und explizite
Feature-Abschlussnachweise. Sie erzeugen zwei Markdown-Ansichten und ändern
weder .NET-Produktcode noch API, Datenbank, Netzwerk, Authentifizierung,
Deployment oder Abhängigkeiten. Manifestwerte bleiben Daten und werden nie als
Code oder Befehle ausgeführt.

*The dependency-free Bash and PowerShell renderers read only the repository-
local Inventar series manifest, its intake files, and explicit feature
completion evidence. They create two Markdown views and change no .NET product
code, API, database, network, authentication, deployment, or dependency
surface. Manifest values remain data and are never executed as code or
commands.*

## Bedrohungen und Kontrollen / Threats and controls

| Risiko | CIA-Bezug | Kontrolle und Evidence |
|---|---|---|
| Ungültige oder manipulierte Eingabe | Integrität | Striktes UTF-8, NUL-Ablehnung, Schema-/Typprüfung und SHA-256-Bindung; `LIE001`, `LIE002`, `LIE009`. |
| Pfadtraversal, Option-Injection oder Symlink-Flucht | Vertraulichkeit/Integrität | Ausschließlich repositoryrelative Pfade, physische Containment-Prüfung, Quotes und `--`; `LIE003`–`LIE005`. |
| Doppelte Identität, Kante oder Ausgabe | Integrität | Eindeutige Ziele/Positionen, typisierte Vorwärtskanten, Root- und DAG-Prüfung, Output-Overlap-Verbot; `LIE006`, `LIE007`. |
| Erfundenes Feature-Evidence | Integrität/Nachvollziehbarkeit | Exakte Spec-Bindung oder terminaler Run-State mit akzeptiertem Artefakthash; ältere verfassungsmäßige Renames nur mit genau einem Feature-stempelgleichen Series-Receipt, exakten Endpunkten und aktuellem Hash; `LIE008`. |
| Teilveröffentlichung oder Eingabedrift | Integrität/Verfügbarkeit | Vollständiger Eingabefingerprint, Kandidatenbildung vor Replace, atomarer Multi-Output-Rollback und gemeinsamer Generationshash; `LIE010`–`LIE012`. |
| Datenabfluss über Diagnosen | Vertraulichkeit | Einzeilige bilinguale Diagnosen redigieren private Pfade, Steuerzeichen und zugangsdatenaehnliche Werte. |

## Governance-Disposition

| Standard/Familie | Status | Begründung / Trigger |
|---|---|---|
| MSL-Kontext; Bash; PowerShell 7 | Applicable, erfüllt | .NET-Produkt bleibt MSL; neue Shelllogik nutzt Strict Mode, sichere Pfade und gepaarte Tests. Trigger: Sprach-, Runtime- oder Dependencywechsel. |
| NIST SSDF, CWE Top 25, STRIDE/CIA, CAPEC | Applicable, erfüllt | Eingabe-, Pfad-, Graph-, Proof-, Diagnose- und Publication-Grenzen besitzen positive und negative Tests. Trigger: neue Eingabe oder Trust Boundary. |
| OWASP SAMM | Applicable, erfüllt | Requirement, rote Fixtures, Implementierung und Verifikation sind nachvollziehbar verbunden. Trigger: Prozess- oder Reviewmodell ändert sich. |
| OWASP ASVS | N/A / Not Assessed | Keine Web-, API-, Authentifizierungs- oder Sessionfläche. Trigger: entsprechende Produktänderung. |
| SBOM, VEX, AI-SBOM, SLSA, OpenSSF Scorecard | N/A / Not Assessed | Keine neue Dependency, Paket-, Release- oder Supply-Chain-Auswahl. Trigger: entsprechender Diff. |
| Zero Trust, BSI C3A/C5, NIS2, CRA, EU AI Act, DORA | N/A / Not Assessed | Keine Netzwerk-, Cloud-, AI-System- oder regulierte Produktgrenze. Trigger: entsprechender Scope. |

Owner ist der InventarWorkerService Repository Owner; Reviewer ist die
Feature-032 Security-Rolle. Restrisiko bis zur Lieferung sind native Linux-/
Windows-Parität und unabhängiger PR-Review. Re-Evaluation bei Eingabe-, Pfad-,
Graph-, Dependency-, Netzwerk-, Produkt- oder Trust-Boundary-Änderung.

*The owner is the InventarWorkerService Repository Owner; the reviewer is the
Feature 032 security role. Residual risk until delivery is native Linux and
Windows parity plus independent pull-request review. Re-evaluate on input,
path, graph, dependency, network, product, or trust-boundary changes.*
