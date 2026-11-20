# GEMINI.md - Projektkontext für InventarWorkerService

## Einstieg und Detailregeln / Entry point and detailed rules

DE: Lies die Projektkonstitution und vor der Arbeit die [Detailregeln](docs/agent-guidance.md)
für deine Aufgabe. Sie ergänzen die verbindlichen Arbeits-, Sicherheits-, Prüf- und Freigaberegeln.

EN: Read the project constitution and the relevant [detailed rules](docs/agent-guidance.md)
before working. They supplement the binding work, security, verification and approval rules.

Dieses Dokument dient als zentrale Orientierungshilfe für die Arbeit an diesem Repository. Es ergänzt die `README.md` und `CLAUDE.md`.

## 🚀 Projektübersicht
**InventarWorkerService** ist eine plattformübergreifende Inventarisierungslösung für IT-Infrastrukturen, entwickelt mit **.NET 10.0** und **C# 14.0**. Das System erfasst Hardware- und Software-Informationen von Windows-, macOS- und Linux-Systemen.

### Kernkomponenten:
1.  **InventarWorkerService**: Ein ASP.NET Core "Agent", der auf jedem zu überwachenden Rechner läuft. Er erfasst lokale Daten und stellt sie über eine REST-API bereit.
2.  **HarvesterWorkerService**: Der zentrale Dienst, der die Agenten abfragt und die Daten in **SQLite**, **MongoDB** oder **PostgreSQL** konsolidiert.
3.  **InventarViewerApp**: Eine interaktive Terminal-Benutzeroberfläche (TUI) basierend auf `Terminal.Gui` zur Anzeige und Verwaltung des Inventars.
4.  **InventarWorkerCommon**: Die zentrale Bibliothek mit Domänenmodellen, Datenbank-Services und API-Logik.
5.  **Steuerungstools**: Verschiedene Projekte (`CtrlWorker...`) zur Verwaltung der Dienste als Windows-Service oder via PowerShell.

## 🛠 Build & Run
Das Projekt nutzt die Standard .NET-CLI.

- **Gesamte Lösung bauen**: `dotnet build InventarWorkerService.sln`
- **Agent starten**: `dotnet run --project InventarWorkerService/InventarWorkerService.csproj`
- **Sammler starten**: `dotnet run --project HarvesterWorkerService/HarvesterWorkerService.csproj`
- **TUI-App starten**: `dotnet run --project InventarViewerApp/InventarViewerApp.csproj`
- **Coverage messen (CI-Grenze >=70%, Ziel >=80%)**: `dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults`
- **Veraltete NuGet-Pakete prüfen**: `dotnet list package --outdated`
- **Dokumentation neu erzeugen (bei API/XML-Doku-Änderungen)**: `docfx docfx.json`
- Die dabei erzeugten Verzeichnisse `api/` und `_site/` sind Build-Artefakte und bleiben ungetrackt; Verteilung erfolgt ueber GitHub Pages und CI-Artefakte statt ueber Git.

## 🧪 Testing
- **Alle Tests**: `dotnet test`
- **Unit Tests**: `dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj`
- **Integration Tests**: Erfordern einen laufenden Agenten auf Port 5000.
  `dotnet test InventarWorkerServiceIntegrationTest/InventarWorkerServiceIntegrationTest.csproj`

## 🌿 Branching-Workflow (Verbindlich)
- Der Branch `main` ist geschützt und darf nicht direkt für Feature-Commits genutzt werden.
- Für jedes Feature/Fix muss ein neuer Branch erstellt werden.
- Änderungen gelangen ausschließlich per Pull Request nach `main` (inkl. Testnachweis).
- Wenn ein dedizierter Feature-Branch die Anforderungen eines Lastenhefts umgesetzt hat, wird die Datei in `Lastenheft_<Thema>.<feature-branch>.md` umbenannt, damit der gelieferte Umfang im Repository nachvollziehbar bleibt.
- Arbeits-Branches duerfen entweder die bestehende Themenbenennung oder die nummerierte Spec-Kit-Form `NNN-short-description` verwenden.
- `Directory.Build.props` fuehrt die repo-weiten Felder `Version`, `AssemblyVersion` und `FileVersion` als `Major.Minor.Patch.Build`; auf nummerierten Spec-Kit-Branches ist `Minor` die numerisch interpretierte Feature-/Branch-Nummer (`002` -> `2`), `Patch` die Commit-Anzahl im Feature-/PR-Branch nach dem aktuellen Commit und `Build` ein nur vor `dotnet build` oder `dotnet test` erhoehter Zaehler.

## 🏗 Architektur & Konventionen
- **Plattform-Support**: Implementiert als Windows Service, systemd (Linux) und launchd (macOS).
- **Datenhaltung**:
  - Primär: **SQLite** (via Dapper).
  - Sekundär (Harvester): **MongoDB** & **PostgreSQL**.
- **API**: System.Text.Json (camelCase), RestSharp für Clients. Swagger/OpenAPI ist in Development unter `/swagger` verfügbar.
- **Sprache**:
  - Erklärende Texte in Kommentaren/Dokumentation: zweisprachig (Deutsch zuerst, dann Englisch) auf CEFR-B2-Niveau.
  - UI-Labels & Logs: Deutsch.
- **Coding Style**:
  - Toolchain-Basis: `.NET 10` und `C# 14.0`.
  - Nullable Reference Types sind aktiviert.
  - Asynchrone Programmierung (`async/await`) ist Standard für I/O.
  - Test-Namensschema: `<UnitUnderTest>_<Scenario>_<ExpectedOutcome>`.
  - Testabdeckung in CI: mindestens 70%, Zielbereich ab 80%.
  - NuGet-Pakete auf aktuellem stabilen Stand halten; Ausnahmen dokumentieren.
  - XML-Dokumentation ist für öffentliche APIs verpflichtend (CS1591 nicht global unterdrücken).
  - Für nicht-öffentliche Member/Variablen sind an didaktisch relevanten Stellen zweisprachige Block- oder Zeilen-Kommentare zu nutzen.
  - Bei API- oder XML-Doku-Änderungen `docfx docfx.json` ausführen.
- Neue oder geaenderte nicht-triviale Logik muss auf didaktischen Inline-Kommentarbedarf geprueft werden, wenn Lernverstaendnis oder Wartbarkeit betroffen sind.
- Inline-Kommentare erklaeren Warum, Trade-off, Randbedingung, historische Abweichung oder Proof-Grenze; sie wiederholen nicht den offensichtlichen Code.
- Die Kommentarintensitaet bleibt moderat: normalerweise 1 bis 3 Zeilen vor einem nicht-trivialen Block, bei didaktischen Erklaerbloecken Deutsch zuerst und Englisch danach auf CEFR-B2-Niveau.
- **Worker-Intervalle**:
  - Debug: 30 Sekunden.
  - Release: 24 Stunden.

## 📁 Wichtige Verzeichnisse
- `InventarWorkerCommon/Models/`: Domänenmodelle (Hardware, Software, SQL, etc.).
- `InventarWorkerCommon/Services/`: Geschäftslogik für DB, API und Inventarisierung.
- `api/`: Lokal oder in CI erzeugte DocFX-Metadaten; das Verzeichnis bleibt ungetrackt.
- `docs/`: Zusätzliche Dokumentation und PDFs.
- `_site/`: Lokal oder in CI erzeugte HTML-Dokumentation; das Verzeichnis bleibt ungetrackt.

## 👤 Benutzer-Präferenzen (Thorsten)
- **IDE**: JetBrains Rider.
- **Shell**: Bevorzugt PowerShell (plattformübergreifend).
- **Datenbanken**: SQLite, PostgreSQL, MongoDB.
- **Interessen**: C64-Entwicklung (cc65, ACME), VST3-Plugins, Arbeitsrecht (Betriebsrat).
- **Kommunikation**: Deutsch (Du-Form).

## Agentische Skriptausfuehrung / Agentic Script Execution

- Vor jeder Automationsaufgabe zuerst das Betriebssystem pruefen. Wenn ein passendes PowerShell-7-Skript oder Cmdlet vorhanden ist und `pwsh` verfuegbar ist, diese Variante bevorzugen. Fuer strukturierte lokale Automationen ist C# ueber `.NET` oder `mono` ein zulaessiger zweiter Weg, wenn Typisierung, Dateiformate oder Wiederverwendbarkeit dadurch klar besser werden. Erst wenn PowerShell oder C# nicht sinnvoll passen, die OS-nahe vorhandene Repo-Variante nutzen, auf macOS/Linux typischerweise Bash. Keine neue Sprache nur aus Bequemlichkeit einfuehren, wenn ein bestehendes Repo-Skript denselben Zweck erfuellt.
- Detect the operating system before each automation task. If a matching PowerShell 7 script or cmdlet exists and `pwsh` is available, prefer that variant. For structured local automation, C# via `.NET` or `mono` is an acceptable second option when type safety, file formats, or reuse clearly benefit from it. Only when PowerShell or C# is not a good fit, use the existing OS-native repository variant, typically Bash on macOS/Linux. Do not introduce a new language merely for convenience when an existing repository script already solves the task.

## Inclusion & Accessibility

- Erklaerende Lern- und Governance-Dokumentation muss zweisprachig mit Deutsch zuerst und Englisch danach auf CEFR-B2-Niveau vorliegen.
- Grosse normative Dokumente wie `Pflichtenheft*.md` und `Lastenheft*.md` duerfen statt eines uebergrossen Inline-Zweisprachblocks als synchron gepflegte englische Parallelfassung mit Suffix `.EN.md` gefuehrt werden; die deutsche Fassung bleibt kanonisch, solange nichts anderes markiert ist.
*   Programmierung #include<everyone> — Diese Lernbeispiele richten sich an Azubis (Fachinformatiker AE/SI) mit Deutsch und Englisch als Arbeitssprachen sowie an sehbehinderte Lernende, die mit Braille-Displays, Screen-Readern oder Textbrowsern arbeiten. Barrierefreiheit ist kein Nice-to-have, sondern Pflichtanforderung.
- Treat WCAG 2.2 conformance level AA as the practical baseline for generated HTML documentation.
- If `docfx` output is regenerated, the same work item must also run a text-oriented accessibility review with Playwright + `@axe-core/playwright` and `lynx`.
- Recommended A11y toolchain for DocFX-based repos: Node 24 LTS, `npm`, Playwright, `@axe-core/playwright`, and `lynx`.

## Gemeinsame Governance-Ergaenzung / Shared Governance Addendum

- Alle nutzerseitigen Artefakte muessen barrierefrei gedacht und geprueft werden: CLI-Ausgaben, Dokumentation, HTML, UI und generierte Templates; WCAG 2.2 Level AA ist die Standard-Basis, sobald die Kriterien auf das Artefakt anwendbar sind.
- All user-facing artefacts must be designed and reviewed for accessibility: CLI output, documentation, HTML, UI, and generated templates; WCAG 2.2 Level AA is the default baseline wherever the criteria apply.

- Fuer C#/.NET-Repositories gilt standardmaessig eine Thorsten-Solo-Basis von `125` Zeilen/Arbeitstag, sofern das Repo keinen abweichenden, begruendeten Wert dokumentiert.
- The default Thorsten-solo baseline for C#/.NET repositories is `125` lines/workday unless the repository documents a justified deviation.

## Shared Parent Guidance

- Die gemeinsamen Dateien `/Users/thorstenhindermann/RiderProjects/AGENTS.md` und `/Users/thorstenhindermann/RiderProjects/GEMINI.md` speichern die repo-uebergreifenden Basisregeln.
- Diese Projekt-Datei ist die spezifischere Autoritaet fuer projektspezifische Build-Befehle, Workflows, Architektur und Features.

---

## Level-2-Umgebungsregister / Level-2 Environment Registry

- Die zentrale `constitution.md` enthält das verbindliche Level-2 Project Environment Registry.
- Spec-Kit-Pläne und Gemini-Arbeit in Level-2-Projekten müssen die passende Registry-Zeile als verbindlichen Kontext für Runtime, Build/Test, A11Y, Statistik und Agentenflächen verwenden.
- Änderungen an einer Level-2-Runtime, Toolchain oder Statistik-Basis müssen `constitution.md`, `.specify/memory/constitution.md` und betroffene KI-Agenten-Dateien gemeinsam prüfen.

*The central `constitution.md` contains the binding Level-2 Project Environment Registry. Spec-Kit plans and Gemini work in Level-2 projects must use the matching registry row as binding context for runtime, build/test, A11Y, statistics, and agent surfaces. Changes to Level-2 runtime, toolchain, or statistics baselines require a joint review of `constitution.md`, `.specify/memory/constitution.md`, and affected AI-agent files.*

## Allgemeine Architektur-Governance / General Architecture Governance

- Struktur-, Schnittstellen-, Qualitätsattribut-, Laufzeit-, Deployment- oder Wartbarkeitsänderungen müssen iSAQB/arc42-Architekturevidenz prüfen.
- Standardpfad: `docs/architecture/`; ADRs fuer allgemeine Architekturentscheidungen liegen unter `docs/architecture/adr/`.
- Verfügbare Templates: `architecture-vision-template.md`, `context-view-template.md`, `building-block-view-template.md`, `runtime-view-template.md`, `deployment-view-template.md`, `architecture-decision-template.md`, `architecture-risks-template.md`, `quality-scenarios-template.md`.

*Structural, interface, quality-attribute, runtime, deployment, or maintainability changes must evaluate iSAQB/arc42 architecture evidence. Default path: `docs/architecture/`; general architecture ADRs live under `docs/architecture/adr/`. Use the matching templates when the artefact applies.*

## Sicherheitsdokumentation / Security Documentation (XII–XVIII Extensions)

- Jedes Level-2-Projekt MUSS die folgenden Sicherheitsdokumente pflegen, basierend auf den Templates in `.specify/templates/`:
  - **Bedrohungsmodell / Threat Model** (`threat-model-template.md`) — STRIDE-Methodik, Trust Boundaries, Risikobewertung, CAPEC-Referenzen (Prinzip XIII + XVII)
  - **Security Architecture Decision Records (S-ADR)** (`adr-template.md`) — architektonische Sicherheitsentscheidungen mit Compliance-Nachweis (Prinzip XIII)
  - **arc42 Section 8 Sicherheits-Querschnittskonzepte** (`arc42-security-template.md`) — Authentifizierung, Autorisierung, Verschlüsselung, Eingabevalidierung, Fehlerbehandlung, Logging, Abhängigkeiten, Deployment (Prinzip XIII)
  - **Sicherheits-Checkliste / Security Checklist** (`security-checklist-template.md`) — sprachspezifische Code-Review-Checkliste (Prinzip XII)
  - **Abhängigkeits-Audit / Dependency Audit** (`dependency-audit-template.md`) — CVE-Tracking, Lizenz-Compliance, Supply-Chain-Sicherheit (Prinzip XII)
  - **Sicherheits-Qualitätsszenarien / Security Quality Scenarios** (`security-quality-scenarios-template.md`) — iSAQB CPSA-F Qualitätsszenario-Methodik (Prinzip XII + XIII, SHOULD)
  - **ASVS-Verifikation / ASVS Verification** (`asvs-verification-template.md`) — OWASP ASVS Level, Scope und Evidenz (Prinzip XV, Web-/API-Projekte MUST)
  - **Supply-Chain-Evidenz / Supply Chain Evidence** (`supply-chain-evidence-template.md`) — SBOM, AI-SBOM, VEX, SLSA, OpenSSF Scorecard (Prinzip XVI, releasefähige Projekte MUST; AI-SBOM nur bei KI-Runtime-/Produktkomponenten)
  - **Zero-Trust-Anwendbarkeit / Zero Trust Applicability** (`zero-trust-applicability-template.md`) — NIST SP 800-207-Bewertung (Prinzip XVIII, verteilte Systeme SHOULD)
  - **SAMM-Bewertung / SAMM Assessment** (`samm-assessment-template.md`) — OWASP SAMM Reifegrad und Verbesserungsplan (Prinzip XVIII, langlebige Projekte SHOULD)
- Projektspezifische Instanzen werden in `docs/security/` gepflegt; S-ADRs als einzelne Dateien in `docs/security/adr/`.

*Every Level-2 project MUST maintain security documents based on templates in `.specify/templates/`: threat model (STRIDE+CAPEC), S-ADRs, arc42 Section 8 security concepts, security checklist, dependency audit, security quality scenarios (SHOULD), ASVS verification (web/API MUST), supply-chain evidence (release-capable MUST; AI-SBOM when AI runtime/product components apply), Zero Trust applicability note (distributed systems SHOULD), and SAMM assessment (long-lived projects SHOULD). Project-specific instances live in `docs/security/`; S-ADRs in `docs/security/adr/`. See `constitution.md`, Principles XII–XVIII for authoritative requirements.*

## Sicherheitsstandards & Anwendbarkeit / Security Standards & Applicability

- Vor jeder Level-2-Aufgabe die anwendbaren Sicherheitsstandards aus `constitution.md`, Prinzipien XIV-XVIII bestimmen und explizit benennen.
- `NIST SSDF` und `CWE Top 25` gelten immer für Level-2-Arbeit.
- `OWASP ASVS` gilt für Web-, API-, HTTP- und authentifizierte Dienste; der gewählte ASVS-Level muss benannt werden.
- `SBOM` gilt für releasefähige oder verteilbare Artefakte; `VEX`, wenn bekannte Schwachstellen in ausgelieferten oder geprüften Komponenten bewertet werden müssen.
- `AI-SBOM` gilt projektartabhängig bei KI-Modellen, KI-Diensten, Trainings-/Embedding-Daten, Inferenz-Infrastruktur oder KI-Runtime-Komponenten im ausgelieferten oder betriebenen System; reine Entwicklungswerkzeug-Nutzung wird als `N/A` mit Toolchain-Begründung dokumentiert.
- `SLSA` gilt als Soll-Vorgabe für CI/CD- oder veröffentlichte Artefakte; `Zero Trust` ist für verteilte, servicebasierte, cloudnahe oder remote-verwaltete Systeme explizit zu prüfen.
- `CAPEC` soll in Bedrohungsmodellen für die risikoreichsten Angriffswege verwendet werden; `OWASP SAMM` soll für langlebige Projekte/Workspaces in Verbesserungspläne einfließen.
- `OWASP Cheat Sheet Series`, `OWASP Proactive Controls` und bei öffentlichen OSS-Repositories oder kritischen Abhängigkeiten `OpenSSF Scorecard` sind als ergänzende Referenzen zu berücksichtigen.
- Nichtanwendbarkeit immer als `N/A` mit kurzer Begründung dokumentieren; keine stillschweigende Auslassung.

*At the start of every Level-2 task, determine and name the applicable security standards from `constitution.md`, Principles XIV-XVIII. `NIST SSDF` and `CWE Top 25` always apply. `OWASP ASVS` applies to web/API/HTTP/auth-bearing services; `SBOM` applies to releasable or distributable artefacts; `AI-SBOM` applies when AI models, AI services, datasets, inference infrastructure, or AI runtime components are part of the released or operated system; `VEX` applies when known vulnerabilities in shipped/evaluated components need a disposition statement. `SLSA` is the target model for CI/CD and published artefacts; `Zero Trust` must be explicitly evaluated for distributed, service-based, cloud, or remotely managed systems. `CAPEC`, `OWASP SAMM`, `OWASP Cheat Sheet Series`, `OWASP Proactive Controls`, and `OpenSSF Scorecard` are supporting references where relevant. Record non-applicability as `N/A` with justification rather than omitting it silently.*

## Zentrale Verzeichnisse / Key Directories

- `~/scripts/`: Zentrale Automatisierungsskripte (Bootstrap, Secret-Scan, Hook-Installer).
- `~/`: Weitere Workspace-Verzeichnisse werden per `bootstrap-workspace` angelegt und hier eingetragen.
- `~/.gemini/`: Globale Gemini-Konfiguration und persistente Erinnerungen.

## Entwicklungskonventionen / Development Conventions

- **Plattformunabhängigkeit & Dokumentation:** Alle kritischen Skripte müssen sowohl als `.sh` (Bash) als auch als `.ps1` (PowerShell Core) vorliegen. Jedes Skript erfordert eine Unix man-Page (`.sh`, in `docs/man/`), eine vollständige PowerShell-Hilfe (`.ps1`) und muss zusätzlich als PowerShell Cmdlet (Advanced Function) im `Verb-Noun` Format verfügbar sein.
- **Sicherheits-Standard:** Jedes Projekt muss über einen `pre-push` Hook verfügen, der Secret-Scanning in Agenten-Verzeichnissen durchführt.
- **Git-Strategie:** Keine Submodules; stattdessen werden Sub-Repos durch die Baseline-Skripte in der `.gitignore` des übergeordneten Workspaces erfasst.

## Projektstatus / Repository Status

- **Sichtbarkeit:** Öffentliches **Template-Repo** — über „Use this template" nutzbar; kein Fork, keine History-Übertragung
- **Lizenz:** MIT
- **Branch-Schutz:** PR-Pflicht auf `main`; Admin (Eigentümer) kann direkt pushen (`enforce_admins: false`)
- **CI:** ✅ Ubuntu 22.04 · macOS 14 · Windows 2022
- **Compliance-Score:** 100 % (25/25 Checks)

## GitHub/GitLab CLI First / GitHub/GitLab CLI zuerst

Für GitHub-Repositories zuerst die authentifizierte `gh` CLI für mögliche Schreibaktionen und Live-Repository-Operationen verwenden, einschließlich PR-/Issue-Kommentaren, PR-Statusprüfungen, Review-Follow-up, Workflow-Prüfung und Merge-/Statusabfragen. GitHub-Connector-Tools hauptsächlich für strukturierte Read-only-Inspektion oder Fälle nutzen, in denen die CLI nicht geeignet ist.

Für GitLab-Repositories die authentifizierte `glab` CLI zuerst für gleichwertige Aktionen verwenden. Bekanntermaßen fehlschlagende Connector-Schreibwege nicht wiederholt versuchen, wenn `gh`/`glab` die Aufgabe direkt erledigen kann.

For GitHub repositories, use the authenticated `gh` CLI first for feasible write actions and live repository operations, including PR/issue comments, PR status checks, review follow-up, workflow inspection, and merge/status queries. Use GitHub connector tools mainly for structured read-only inspection or when the CLI is not suitable.

For GitLab repositories, use the authenticated `glab` CLI first for equivalent actions. Do not repeatedly try connector write paths that are known to fail when `gh`/`glab` can perform the task directly.

<!-- learner-a11y-baseline:start -->
## Hinweise zur Lernenden- und A11Y-Basis / Notes on the Learner and A11Y Baseline

- Verbindliche Zielgruppen ab dem ersten Ausbildungsjahr sind
  Fachinformatiker*innen, Kaufleute für IT-System-Management und Kaufleute für
  Digitalisierungsmanagement.
- Lern-, Bedien-, Governance- und Spec-Kit-Inhalte stehen auf Deutsch zuerst
  und Englisch danach, verwenden ungefähr CEFR B2 und erklären Fachbegriffe
  beim ersten Auftreten.
- Spec-Kit-Erfahrung wird nicht vorausgesetzt. Befehle, Artefakte, Zustände und
  Übergänge werden beim ersten Gebrauch verständlich eingeführt.
- Abhängigkeiten, Zustände und Entscheidungen erhalten eine vollständige
  textorientierte Erklärung; eine ausschließlich visuelle Darstellung genügt
  nicht.
- `Programmierung #include<everyone>` und WCAG 2.2 Level AA gelten als
  verbindliche Prüfbasis, soweit die Kriterien auf das Artefakt anwendbar sind.

*The binding audience starts in the first training year and includes IT
specialist apprentices and both IT management occupations. Learner, usage,
governance, and Spec Kit content is German-first/English-second at about CEFR
B2, explains technical terms at first use, assumes no prior Spec Kit
experience, and never relies on visual-only dependency, state, or decision
information. `Programmierung #include<everyone>` and WCAG 2.2 Level AA are the
review baseline wherever applicable.*
<!-- learner-a11y-baseline:end -->

<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan
<!-- SPECKIT END -->

## Hinweise / Notes

- Diese Datei bleibt bewusst kompakt und ergänzt die projektspezifische Dokumentation.
- This file intentionally stays compact and complements the project-specific documentation.

<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan
<!-- SPECKIT END -->

<!-- statistics-profile-2-guidance:begin -->
## Statistikprofil 2 / Statistics Profile 2

- Verbindlich sind `docs/project-statistics.config.json` und der markierte Profil-2-Block in `docs/project-statistics.md`; aktualisieren mit `render-project-statistics.*`.
- Profil 2 zeigt exakte KPI, Artefaktmix, 52-Wochen-Aktivitaet, Wochen- und kumulatives Volumen, belastbare Phasen oder Monatsfallback sowie Speedup-Vergleiche.
- Nur ASCII verwenden: Heatmap `0..4`, `-` fuer noch nicht abgelaufene Tage und Gauges `#`/`.`; jedes Textdiagramm bleibt hoechstens 100 Zeichen breit.
- Jede Grafik braucht genaue Zahlen und eine bilinguale CEFR-B2-Textalternative, Deutsch zuerst und Englisch danach.
- Methodik v2 wertet Git-getrackten Text und Bruttoaenderungen aus Nicht-Merge-Commits aus; Ledger, `STATS.md` und Binaerdaten bleiben ausgeschlossen.
- Referenzen dieses Repositories: `80` Zeilen/Arbeitstag konservativ und `100` Zeilen/Arbeitstag Thorsten-Solo. Speedup bleibt Lieferdichte, keine Stoppuhr- oder Personenbewertung.
- Dieser Vertrag ersetzt aeltere Visualisierungsvorgaben; historische Ledger-Eintraege und archivierte Profil-1-Diagramme bleiben unveraendert.
- Gemeinsame Aenderungen werden synchron in `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md` und `.github/agents/copilot-instructions.md` gepflegt.

*Profile 2 is governed by the JSON configuration and generated marker block. Use ASCII `0..4`, `-`, and `#`/`.`, exact values, German-first bilingual CEFR-B2 alternatives, and a 100-character chart limit. Methodology v2 excludes the ledger, `STATS.md`, and binaries. This repository uses manual references of `80` and `100` lines per workday. Speedup describes delivery density, not stopwatch or personal performance. This contract supersedes older visualization rules while retaining historical entries and archived Profile 1 charts.*
<!-- statistics-profile-2-guidance:end -->

## Secure Development Assurance – freigegebenes Projektprofil / Approved Project Profile

- Die unveraendert erhaltene Assurance-Basis ist `secure-development-assurance-thirteen-governance-presets`: die bisherigen zwölf Presets plus `secure-development-assurance-governance` v0.1.10, aktiviert mit Priorität 15. Andere Profile und globale Defaults bleiben unverändert.
- Maßgebliche Produktquellen sind ausschließlich die veröffentlichten eigenständigen GitHub-Preset-Repositories. Lokale Installationen sind Integrationskopien, keine zusätzlichen Produktquellen; Änderungen entstehen im jeweiligen Preset-Repository, nicht in einem Home-Baseline-Scaffold.
- `speckit.secure-development-status` prüft Evidence strikt lesend. `speckit.secure-development-review` benötigt einen ausdrücklich beauftragten Gate-/Kontext-Review; die Installation startet keine Reviews und erteilt keine menschlichen Freigaben.
- Keine C5-Testat-, Zertifizierungs- oder vollständige Readiness-Prüfung; `Ready` gilt nur für den gebundenen Evidence-Kontext. Projektgeführte Baseline-Versionen werden nicht durch die Preset-Version ersetzt.
- Bedienung, Paketbindung, Erhaltungsnachweis und Grenzen: [Integrationsnachweis](docs/maintenance/secure-development-assurance-integration.md), relativ zur Repository-Wurzel.

*The preserved assurance base is the thirteen-preset profile: its existing twelve presets plus Secure Development Assurance v0.1.3, enabled at priority 15. Other profiles and global defaults stay unchanged. Published standalone GitHub repositories are the only product sources; installed copies are integrations, not competing sources. Status is read-only; a gate review requires explicit context-specific authority. Installation starts no reviews and grants no human approvals. This is not a C5 attestation, certification, or full readiness check; `Ready` is evidence-context-specific. The project baseline retains its own version. See the integration record relative to the repository root.*

<!-- BEGIN spec-kit-diagrams-completion -->
## Mermaid und Spec-Kit-Abschlussbericht / Mermaid and Spec Kit completion report

Neue Lastenhefte enthalten bei hilfreichen Abläufen, Zuständen oder Abhängigkeiten
lesbaren Mermaid-Quelltext im Markdown und eine gleichwertige Textalternative.
Bei einfachen Inhalten die Nichtanwendung kurz begründen. Diagramme bilden die
verbindlichen Text-/Manifestquellen ab; Farbe allein trägt keine Bedeutung.
Nach jedem vollständig abgeschlossenen Spec-Kit-Feature-Lauf den vollständigen
Ergebnisbericht im Chat anzeigen und im Feature-Verzeichnis als
`completion-report.md` versionieren. Einzelne Planungs-/Status-/Review-Kommandos
lösen keinen solchen Bericht aus; blockierte oder pausierte Läufe als
Zwischenbericht kennzeichnen. Vorlage: `.specify/templates/completion-report-template.md`;
Regel: `docs/spec-kit-diagrams-and-completion-reports.md`.
Ergebnis, Tests, Dokumentation, Git-gebundene Umfangszahlen, Verlauf und Restpunkte
belegen. Programmlogik von generierter Evidence, Git-Wandzeit von aktiver
Arbeitszeit und bestandene von ausgefallenen Prüfungen unterscheiden. Finale
Merge-/Sync-Evidence im Chat und bestehenden Closeout-Nachweis ergänzen; keine
zusätzlichen Commits allein für selbstreferenzielle Berichts-/Statistikwerte.
Diese Projektregel und lokale Vorlagen bei Spec-Kit-Updates erhalten.

*New intakes use readable Mermaid Markdown and equivalent text alternatives for
useful workflows, states or dependencies; justify omission for simple content.
Diagrams reflect authoritative text/manifests and never rely on color alone.
After each completed feature run, show the full outcome report in chat and
version completion-report.md in the feature directory using the shared template
and rule above. Individual planning/status/review commands do not trigger it;
paused/blocked runs receive interim reports. Evidence outcomes, tests, docs,
Git-bound counts, delivery history and remaining work. Distinguish code from
volume generated as evidence, elapsed from active time, and passed from failed
checks. Add final merge/sync proof in chat and existing closeout evidence, without
commits solely for self-referential counts or IDs. Preserve local rules/templates
across Spec Kit updates.*
<!-- END spec-kit-diagrams-completion -->

<!-- project-statistics-rollout:begin -->
## Statistik-Preset und Pflege / Statistics preset and maintenance

Das aktuelle Projektprofil ist `project-statistics-fourteen-governance-presets`:
bestehende 13er-Basis plus Statistik v0.1.0 bei Prioritaet 90. Globale Defaults
bleiben erhalten; die operative lokale Zuordnung folgt erst nach Lieferung.
`docs/project-statistics/config.json` steuert den getrennten UTC-/52-Wochen-Kontext.
Profil 2 bleibt kanonisch; Referenzmodelle im neuen Kontext bleiben aus.
Nach abgeschlossenem Feature/Implementierungsabschnitt Inhalte zuerst committen,
Update ausdruecklich beauftragen und vorab im Dry-Run pruefen. Beide Statistiken
aus derselben Inhaltsrevision pflegen, Ausgaben committen und lesend verifizieren.
Keine Personen-, Lernleistungs-, Qualitaets-, Sicherheits- oder KI-Produktivitaetsbewertung.
Bedienung: `docs/project-statistics/README.md`; Quellen und Abnahme:
`docs/maintenance/project-statistics-rollout-v010.md`. Kein automatischer
Spec-Kit-Lauf und keine Commit-/Push-/Merge- oder menschliche Freigabe durch das Preset.

The current profile adds statistics v0.1.0 at priority 90 to the unchanged
thirteen-preset base. Preserve global defaults and defer operational assignment
until delivery. Keep legacy Profile 2 authoritative; use the separate UTC context
with reference models disabled. After feature/implementation completion, commit
content first, preview an expressly authorized update, render both contexts,
commit outputs and verify read-only status. Statistics do not rate people,
learning, quality, security or AI productivity. See the repository-relative
usage and rollout records. Installation grants no feature or delivery authority.
<!-- project-statistics-rollout:end -->
