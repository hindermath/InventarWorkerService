# GEMINI.md - Projektkontext für InventarWorkerService

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

## 📊 Projektstatistik

- Wenn sich gemeinsam genutzte KI-Agenten-Hinweise, Workflow-Konventionen oder die Statistikmethodik aendern, muessen `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md` und `.github/agents/copilot-instructions.md` gemeinsam geprueft und bei Bedarf im selben Change aktualisiert werden.
- Gemeinsame Vorgaben duerfen nicht nur in einer dieser Dateien geaendert werden; beabsichtigte agentenspezifische Abweichungen sind im selben Change ausdruecklich zu dokumentieren.
- `docs/project-statistics.md` ist das fortlaufende Statistik-Register des Repositories.
- Die Datei muss nach jeder abgeschlossenen Spec-Kit-Implementierungsphase, nach jeder agentischen Änderung am Repository und auf explizite Anforderung aktualisiert werden.
- Im `## Fortschreibungsprotokoll` muessen die Tabelleneintraege strikt chronologisch stehen: der aelteste Eintrag oben, der juengste und zuletzt eingetragene Eintrag unten; Eintraege mit demselben Datum behalten ihre Eintragungsreihenfolge.
- Als letzter Top-Level-Block der Datei muss immer ein `## Gesamtstatistik`-Abschnitt stehen; danach darf kein weiterer Top-Level-Abschnitt folgen.
- Innerhalb dieses finalen `## Gesamtstatistik`-Abschnitts muessen kompakte ASCII-only-Diagramme direkt unter der textlichen Gesamtauswertung stehen, damit Artefaktmix, dokumentierte Branch-/Phasenverlaeufe, Beschleunigungsfaktoren und der Vergleich zwischen erfahrener Entwickler-Referenz, Thorsten-Solo-Referenz und sichtbarem KI-Lieferfenster in reinem Markdown lesbar bleiben.
- Jeder kurze CEFR-B2-Erklaertext muss direkt bei seiner ASCII-Diagrammgruppe stehen, und der Statistikblock muss fuer Braille-Zeile, Screenreader und Textbrowser textfreundlich bleiben, ohne auf Farbe oder Layout allein angewiesen zu sein.
- Jeder Eintrag muss Branch oder Phase, beobachtbares Arbeitsfenster, Produktions-, Test- und Doku-Zeilen, die wesentlichen Arbeitspakete, die konservative Handarbeits-Basis von 80 manuell erstellten Zeilen pro Arbeitstag ueber Code, Tests und Dokumentation hinweg sowie die repo-spezifische Thorsten-Solo-Vergleichsbasis von 100 Zeilen pro Arbeitstag fuer diese native .NET-Loesung enthalten.
- Wenn daraus Monatswerte abgeleitet werden, sind die Annahmen explizit zu nennen, zum Beispiel 21,5 Arbeitstage pro Monat sowie 30 Urlaubstage pro Jahr bis einschliesslich 2026 und 31 Urlaubstage pro Jahr ab 2027 in einer TVoeD-aehnlichen Kalenderannahme bei 5-Tage-Woche.
- Beschleunigungsangaben muessen beide Referenzen gegen sichtbare Git-Aktivtage stellen und ausdruecklich als repo-weiten Verdichtungsfaktor statt als Stoppuhrmessung kennzeichnen.
- Wenn Stundenwerte ausgewiesen werden, sind die Tageswerte mit der TVoeD-Arbeitszeit von `7,8 Stunden` bzw. `7 Stunden 48 Minuten` pro Arbeitstag umzurechnen.

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
## Memory-Safe Languages (MSL) / Speichersichere Sprachen

- Level-2-Projekte SOLLEN eine speichersichere Sprache (Memory-Safe Language, MSL) als primäre Laufzeit verwenden, wenn die Zielplattform es erlaubt.
- Verbindliche MSL-Erlaubnisliste, Regeln und Begründungspflicht: siehe `constitution.md`, Prinzip XI.
- MSL-Kurzliste: Rust, Swift, C#, F#, Java, Kotlin, Scala, Go, Dart, Python, Ruby, JavaScript, TypeScript, Haskell, OCaml, Erlang, Elixir, Ada, SPARK.
- **Nicht** MSL (Begründung im Level-2-`constitution.md` erforderlich): C, C++, klassisches Objective-C, Assembly, `cc65`-C89, Zig (pre-1.0), Nim (manual), D ohne GC.
- In Nicht-MSL-Repositories (z. B. `C64Projects/cc65`) die im Level-2-`constitution.md` hinterlegte Begründung im Plan- und Task-Kontext erwähnen.
- `speckit.constitution` und `speckit.specify` SOLLEN bei Nicht-MSL-Primärsprache einen **nicht blockierenden** Hinweis ausgeben (Tooling-Aufgabe, separate Umsetzung).
- Änderungen an dieser Empfehlung erfordern ein gemeinsames Update in `constitution.md`, `.specify/memory/constitution.md`, `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md` und `.github/agents/copilot-instructions.md`.

*Level-2 projects SHOULD use a memory-safe language (MSL) as their primary runtime when the target platform allows. Authoritative rules: `constitution.md`, Principle XI. MSL short list: Rust, Swift, C#/F#, Java/Kotlin/Scala, Go, Dart, Python, Ruby, JavaScript/TypeScript, Haskell, OCaml, Erlang/Elixir, Ada/SPARK. Non-MSL languages (C, C++, Assembly, `cc65`, Zig pre-1.0, …) require a documented justification in the Level-2 `constitution.md`. In non-MSL repositories (e.g. `C64Projects/cc65`), surface the documented justification in plans and tasks. `speckit.constitution` and `speckit.specify` SHOULD emit a non-blocking advisory warning when the primary language is not an MSL — tracked as a separate tooling task. Changes to this recommendation require a joint update across `constitution.md`, `.specify/memory/constitution.md`, and all four agent guidance files.*
## Sichere Code-Erzeugung / Secure Code Generation (ISO 27001/27002 A.8.28)

- KI-generierter und menschlich geschriebener Code MUSS den etablierten Secure-Coding-Best-Practices der Zielsprache und des Frameworks folgen. LLMs erzeugen nicht zuverlässig sicheren Code; explizite Durchsetzung ist erforderlich.
- Verbindliche Regeln und sprachspezifische Anforderungen: siehe `constitution.md`, Prinzip XII.
- Sprachspezifische Kurzregeln (Detailprofil: `.specify/templates/secure-coding-language-rules-template.md`):
  - **C / C89**: Bounds-Checking, kein `gets()`, kein ungeprueftes `sprintf()`/`strcpy()`, CERT C.
  - **C# / .NET**: parametrisierte Queries, Output-Encoding gegen XSS, Anti-Forgery-Tokens, sichere Deserialisierung, Microsoft Secure Coding Guidelines.
  - **Rust**: `unsafe` isolieren und begruenden, keine Panic-Pfade aus nicht vertrauenswuerdigem Input, Deserialisierung validieren, `cargo audit` oder gleichwertig verwenden.
  - **Go**: HTTP-/Client-Timeouts setzen, `context` propagieren, SSRF pruefen, `crypto/rand` nutzen, `govulncheck` oder gleichwertig verwenden.
  - **Swift**: keine Force-Unwraps auf nicht vertrauenswuerdigen Daten, dekodierte Eingaben validieren, Keychain/CryptoKit/TLS-Defaults nutzen, Datei-URLs einschraenken.
  - **Java / Kotlin**: DTOs validieren, Persistence-Zugriffe parametrisieren, Deserialisierung beschraenken, Auth/CSRF/CORS/Session-Defaults pruefen.
  - **Python**: Boundary-Input validieren, keine unsichere Deserialisierung oder dynamische Ausfuehrung, `subprocess`/Dateipfade einschraenken, Dependency-Audit nutzen.
  - **TypeScript / JavaScript**: Runtime-Input validieren, XSS/Prototype-Pollution/SSRF pruefen, keine dynamische Code-Ausfuehrung, Lockfiles auditieren.
  - **SQL**: nur parametrisierte Statements, kein dynamisches SQL aus nicht vertrauenswuerdigem Input.
  - **Bash**: Variable in Anfuehrungszeichen (`"$var"`), kein `eval` auf nicht vertrauenswuerdigem Input, `--` End-of-Options.
  - **PowerShell**: `Set-StrictMode -Version Latest`, validierte Parameter, kein `Invoke-Expression` auf nicht vertrauenswuerdigem Input.
- Kryptografie: aktuelle Algorithmen (AES-256, RSA >= 3072, SHA-256+, Ed25519); veraltete (MD5, SHA-1 für Signaturen, DES, RC4) nur mit expliziter Risikobegründung.
- Fehlerbehandlung darf keine internen Zustände, Stack-Traces oder Verbindungszeichenketten an Endbenutzer preisgeben.
- Hinzugefügte Abhängigkeiten müssen aktiv gepflegt sein und dürfen keine bekannten kritischen CVEs aufweisen.
- Code-Reviews MÜSSEN eine Sicherheitsperspektive für Eingabeverarbeitung, Authentifizierung, Autorisierung, Kryptografie und Datei-/Netzwerk-I/O enthalten.
- Änderungen an dieser Regel erfordern ein gemeinsames Update in `constitution.md`, `.specify/memory/constitution.md`, `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md` und `.github/agents/copilot-instructions.md`.

*AI-generated and human-written code MUST follow the secure-coding best practices of the target language and framework. Authoritative rules: `constitution.md`, Principle XII, and `.specify/templates/secure-coding-language-rules-template.md`. Language-specific short rules cover C/C89, C#/.NET, Rust, Go, Swift, Java/Kotlin, Python, TypeScript/JavaScript, SQL, Bash, and PowerShell. MSL status does not replace secure API, I/O, auth, SQL, crypto, logging, or dependency review. Cryptography: use current algorithms (AES-256, SHA-256+, Ed25519); deprecated (MD5, SHA-1 for signatures, DES, RC4) only with explicit risk acknowledgement. Error handling must not expose internals. Dependencies must have no known critical CVEs. Code reviews must include a security perspective for input handling, auth, crypto, and I/O. Changes require a joint update across `constitution.md`, `.specify/memory/constitution.md`, and all four agent guidance files.*
## Sichere Software-Architektur / Secure Software Architecture (ISO 27001/27002 A.8.27)

- KI-generierte und menschlich geschriebene Software-Architektur MUSS etablierten sicheren Architekturprinzipien folgen. Sicherer Code (Prinzip XII) ohne sichere Architektur reicht nicht aus — beide Ebenen müssen zusammenwirken.
- Verbindliche Regeln und sprachspezifische Architekturvorgaben: siehe `constitution.md`, Prinzip XIII.
- Verbindliche Architekturprinzipien:
  - **Trust Boundaries**: Explizite Vertrauensgrenzen definieren; alle Eingaben an Vertrauensgrenzen validieren und bereinigen.
  - **Defense in Depth**: Mindestens zwei unabhängige Sicherheitsschichten für kritische Assets.
  - **Least Privilege**: Jede Komponente, jeder Dienst und Prozess arbeitet mit minimalen Berechtigungen.
  - **Fail-Safe Defaults**: Zugriff standardmäßig verweigern, explizit gewähren; Fehlerpfade fallen in sicheren Zustand zurück.
  - **Angriffsfläche reduzieren**: Ungenutzte Endpunkte, Dienste und Debug-Funktionen deaktivieren oder entfernen.
  - **Separation of Concerns**: Authentifizierung, Autorisierung, Logging und Eingabevalidierung als Cross-Cutting Concerns implementieren, nicht ad-hoc verstreuen.
  - **Sichere Konfiguration**: Secrets in plattformgeeigneten Secret-Stores (z. B. Azure Key Vault, macOS Keychain), nie im Quellcode oder in Git-tracked Config-Dateien.
  - **Supply-Chain-Sicherheit**: Abhängigkeiten aus verifizierten Registries; Lock-Files committen; verwundbare Abhängigkeiten vor Release ersetzen.
- Änderungen an dieser Regel erfordern ein gemeinsames Update in `constitution.md`, `.specify/memory/constitution.md`, `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md` und `.github/agents/copilot-instructions.md`.

*AI-generated and human-written software architecture MUST follow secure-architecture principles. Authoritative rules: `constitution.md`, Principle XIII. Core principles: trust boundaries (validate all input at system boundaries), defense in depth (at least two independent security layers), least privilege (minimum required permissions), fail-safe defaults (deny by default), attack surface reduction (disable unused features), separation of concerns (auth/logging/validation as cross-cutting concerns), secure configuration (secrets in secret stores, never in code or Git), supply-chain security (verified registries, lock files, no known-vulnerable dependencies). Principles XII + XIII together form the complete secure-development approach: XII = tactical code-level security, XIII = strategic architecture-level security. Changes require a joint update across `constitution.md`, `.specify/memory/constitution.md`, and all four agent guidance files.*
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

## Agentischer Security-Workflow / Agentic Security Workflow

- In `spec.md`, `plan.md` und `tasks.md` die anwendbaren Standards samt Evidenzpfad festhalten.
- Bei Bedrohungsmodellen `STRIDE` als Basis und bei risikoreichen Flows zusätzlich relevante `CAPEC`-Patterns verwenden.
- Bei Web/API-Features den `ASVS`-Level und den Verifikationsumfang in `docs/security/` oder gleichwertiger Projektdokumentation ablegen.
- KI-Nutzung explizit klassifizieren: Entwicklungswerkzeug, keine KI im ausgelieferten/betriebenen System, oder KI-Runtime-/Produktkomponente; `AI-SBOM` entsprechend als `N/A` begründen oder in der Supply-Chain-Evidenz dokumentieren.
- Bei Release-/Artefakt-Arbeit `SBOM`, `AI-SBOM`, `VEX`, Provenance/SLSA-Nachweise und gegebenenfalls `OpenSSF Scorecard` in Release- oder Sicherheitsdokumentation einplanen.
- Bei Architekturänderungen `Zero Trust`-Anwendbarkeit und bei langlebigen Projekten `SAMM`-Folgeaktionen prüfen.
- Default-Evidenzpfad: `docs/security/asvs-verification.md`, `docs/security/supply-chain-evidence.md`, `docs/security/zero-trust-applicability.md`, `docs/security/samm-assessment.md`; Abweichungen nur mit lokal dokumentierter Begründung.

*Capture the applicable standards and the evidence path in `spec.md`, `plan.md`, and `tasks.md`. Use `STRIDE` as the base for threat modeling and add relevant `CAPEC` patterns for the highest-risk flows. For web/API work, record the chosen `ASVS` level and verification scope in `docs/security/` or equivalent project documentation. Classify AI usage as development tooling, absent from the released/operated system, or AI runtime/product component; document `AI-SBOM` as `N/A` or as supply-chain evidence accordingly. For release and artefact work, plan `SBOM`, `AI-SBOM`, `VEX`, provenance/SLSA evidence, and `OpenSSF Scorecard` review where applicable. For architectural changes, evaluate `Zero Trust`; for long-lived projects, consider `OWASP SAMM` follow-up actions. The default evidence path is `docs/security/asvs-verification.md`, `docs/security/supply-chain-evidence.md`, `docs/security/zero-trust-applicability.md`, and `docs/security/samm-assessment.md`, unless the repository documents a justified equivalent location.*

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

## Bekannte Fallstricke / Known Pitfalls

### `gh auth login --web` bleibt hängen / `gh auth login --web` Hangs
Browser-Callback kommt in Hintergrundprozessen nicht an.
In **interaktivem Terminal** ausführen.

### `glab auth login --web` bleibt hängen / `glab auth login --web` Hangs
Browser-Callback kommt in Hintergrundprozessen nicht an.
In **interaktivem Terminal** ausführen.

### `gh`-Keyring ungültig (Windows) / `gh` Keyring Invalid (Windows)
Windows Credential Store korrupt.
`gh auth logout` + neu anmelden; danach `gh auth setup-git`.

### `ssh-agent` startet nicht (Windows) / `ssh-agent` Does Not Start (Windows)
Service deaktiviert, Admin nötig.
HTTPS + `gh auth setup-git` verwenden.

### `CursorPosition`-Fehler in PS-Subprocess / `CursorPosition` Error in PowerShell Subprocess
PowerShell-Profil (Oh-My-Posh) lädt im Subprozess.
`-NoProfile` zu `pwsh -File`-Aufrufen hinzufügen.

### `migrate-workspace.*` läuft parallel in Timeouts / `migrate-workspace.*` Times Out in Parallel
Jeder Migrationslauf startet `init-stats.*` und aktualisiert die Level-0/1/2-Statistiken global.
Mehrere parallele Läufe können sich gegenseitig ausbremsen. Erst Vorschau (`-WhatIf`/`--dry-run`),
dann echte Migrationen seriell pro Workspace mit längerem Timeout ausführen.

### `git pull` meldet divergierende Branches (Linux) / `git pull` Reports Divergent Branches (Linux)
Kein globales Rebase-Setup.
`git config --global pull.rebase true`.

### Push rejected: `fetch first` / Push Rejected: `fetch first`
Remote ist neuer als lokal.
`git pull --rebase --autostash && git push`.

### Test-Skript blockiert Pull / Test Script Blocks Pull
Output-Datei wird vor `pull` geschrieben.
`git pull --rebase --autostash origin main`.

### Lastenheft nach Feature-Abschluss nicht umbenannt / Lastenheft Not Renamed After Feature Completion
`tasks.md` enthielt keinen Rename-Schritt (seit constitution v1.1.1 behoben).
`bash scripts/rename-lastenheft.sh <LH-Datei> <branch-name>` oder `pwsh scripts/rename-lastenheft.ps1 -File <LH-Datei> -BranchName <branch-name>`.

### Windows: `$env:HOME` ist leer, nicht `$null` / Windows: `$env:HOME` Is Empty, Not `$null`
```powershell
# Falsch (??-Operator fängt '' nicht ab):
$home = $env:HOME ?? $env:USERPROFILE
# Richtig:
$home = if ($env:HOME) { $env:HOME } else { $env:USERPROFILE }
```

### CI: Scanner-Verzeichnis / CI: Scanner Directory
```bash
# Falsch (CWD = Repo-Root, Dateien nicht gefunden):
bash scripts/check-homogeneity.sh home-baseline
# Richtig (aus dem Parent heraus):
cd "$(dirname "$GITHUB_WORKSPACE")"
bash "$(basename "$GITHUB_WORKSPACE")/scripts/check-homogeneity.sh" "$(basename "$GITHUB_WORKSPACE")"
```

### `.gitignore`-Whitelist / `.gitignore` Whitelist
Jede neue Datei muss explizit als `!DATEINAME` in `.gitignore` eingetragen werden, sonst wird `git add` lautlos ignoriert (z. B. `LICENSE`).

### `bootstrap-workspace`: GitHub-Username / `bootstrap-workspace`: GitHub Username
Früher hardcodiert. Jetzt dynamisch:
```bash
GH_USER=$(gh api user --jq '.login')
```

### Doppelte Überschriften in TOC / Duplicate heading anchors
Gleiche Heading-Texte → GitHub hängt `-1`, `-2` an. TOC-Links für zweite Vorkommen müssen den Suffix enthalten.

### Pflicht für bilinguale Headings / Bilingual Heading Requirement
Format: `## DE / EN` — immer. Nur-Deutsch verletzt WCAG 2.4.6 und bilinguales Konsistenzgebot.
Ausnahme: Eigennamen wie `### Homogeneity Guardian` oder `### Compliance-Check`.

### Code-Blöcke immer mit Sprach-Tag (WCAG 4.1.1) / Code Blocks Must Always Have a Language Tag (WCAG 4.1.1)
Bare ` ``` ` ohne Sprache ist ein A11Y-Fehler. Für ASCII/Dialog/Verzeichnisse: ` ```text `.

### CHANGELOG.md hinzugefügt / CHANGELOG.md Added
Dokumentiert Versionen v0.1.0–v0.4.0. Muss in `.gitignore`-Whitelist (`!CHANGELOG.md`) eingetragen sein.

### ASCII-Box-Drawing-Tabellen: Zeilenbreite / ASCII Box-Drawing Tables: Line Width
Alle Zeilen einer `text`-Code-Block-Tabelle müssen exakt gleich breit sein. Ein überzähliges Leerzeichen vor dem schließenden `│` macht die Zeile 1 Zeichen zu lang.
Prüfen: PowerShell `$line.Length` oder `wc -m` (Bash) für jede Rahmen-Zeile.

### Spec-Kit-Verzeichnis initialisieren / Initialize the Spec-Kit Directory
Nie manuell aus `~/home-baseline-tmp/` kopieren. Stattdessen:
`specify init --here --force --integration {agent}` je Agent für `agy`, `opencode`, `claude`, `copilot` und `codex` ausführen.

### Spec-Kit-Updates repo-weit / Repository-Wide Spec-Kit Updates
Fuer Level 0, Level 1 und Level 2 nicht mehr per Hand in jedem Repo nachziehen.
Stattdessen zuerst `bash scripts/update-spec-kit.sh --dry-run` bzw.
`pwsh scripts/update-spec-kit.ps1 -WhatIf` ausfuehren, danach bei Bedarf
`--commit --push` / `-Commit -Push`.

Das Skript erkennt neue Repos dynamisch ueber `.git` plus `.specify/`, sichert
`.specify/memory/constitution.md`, legt die lokalen Governance-Templates wieder
auf und nimmt `RiderProjects/TuiVision` normal mit. OpenCode wird nur ueber
`.opencode/command/*.md` getrackt; `.opencode`-Caches, Sessions, Logs,
Credentials und lokale Abhaengigkeiten bleiben ausgeschlossen.

Die Standard-Template-Quelle ist das oeffentliche `home-baseline`-Repo, aus dem
das Skript laeuft. Private Repos wie `RiderProjects/TuiVision` duerfen nur
bewusst mit `--template-source` / `-TemplateSource` als Override genutzt werden.

### GitHub-Housekeeping: Archivierung, Sichtbarkeit, Forks und Stars / GitHub Housekeeping: Archiving, Visibility, Forks, and Stars
`archived` bedeutet bei GitHub nur read-only, nicht unsichtbar. Public archived Repos bleiben ohne Anmeldung sichtbar.
Archivierte Repos sind API-seitig read-only; Sichtbarkeit ändern geht deshalb nur über:
`archived=false` → `private=true` → `archived=true`.

Öffentliche Forks lassen sich nicht einfach auf private setzen. Optionen: öffentlich archiviert lassen, löschen, oder als private Mirror-Repos neu anlegen. Vor Löschungen die Repo-Liste eng festlegen; `gh repo delete` benötigt ggf. `gh auth refresh -h github.com -s delete_repo`.

Für Aktivitätsbewertungen `pushedAt` statt `updatedAt` verwenden, weil `updatedAt` durch Metadatenänderungen springt. Stars sind kontogebundene Metadaten und können über `DELETE /user/starred/{owner}/{repo}` entfernt werden; danach `user/starred` gegenprüfen.

## GitHub/GitLab CLI First / GitHub/GitLab CLI zuerst

Für GitHub-Repositories zuerst die authentifizierte `gh` CLI für mögliche Schreibaktionen und Live-Repository-Operationen verwenden, einschließlich PR-/Issue-Kommentaren, PR-Statusprüfungen, Review-Follow-up, Workflow-Prüfung und Merge-/Statusabfragen. GitHub-Connector-Tools hauptsächlich für strukturierte Read-only-Inspektion oder Fälle nutzen, in denen die CLI nicht geeignet ist.

Für GitLab-Repositories die authentifizierte `glab` CLI zuerst für gleichwertige Aktionen verwenden. Bekanntermaßen fehlschlagende Connector-Schreibwege nicht wiederholt versuchen, wenn `gh`/`glab` die Aufgabe direkt erledigen kann.

For GitHub repositories, use the authenticated `gh` CLI first for feasible write actions and live repository operations, including PR/issue comments, PR status checks, review follow-up, workflow inspection, and merge/status queries. Use GitHub connector tools mainly for structured read-only inspection or when the CLI is not suitable.

For GitLab repositories, use the authenticated `glab` CLI first for equivalent actions. Do not repeatedly try connector write paths that are known to fail when `gh`/`glab` can perform the task directly.


## Spec-Kit-Modell-Routing / Spec Kit Model Routing

- Modellwahl ist operative Agenten-Routing-Guidance, keine Feature-Anforderung. Modellnamen nicht in `spec.md`, `plan.md`, `tasks.md` oder einzelne Feature-Specs schreiben; diese Artefakte muessen reproduzierbar bleiben, auch wenn Modellnamen wechseln oder ein anderer KI-Agent verwendet wird.
- Der jeweilige Agent soll diese Empfehlungen auf seine aktuell verfuegbaren Modelle abbilden; keine feste Anbieter- oder Modellbindung ableiten.
- Fuer Spec-Kit-Spezifikation, Klaerung, Planung, Tasks und Analyse (`/speckit-specify`, `/speckit-clarify`, `/speckit-plan`, `/speckit-tasks`, `/speckit-analyze`; je nach Agent auch `/speckit.specify` usw.) das staerkste verfuegbare Frontier-Reasoning-/Coding-Modell bevorzugen.
- Fuer vollstaendige, lang laufende `/speckit-implement`-Laeufe das staerkste verfuegbare Long-Running-Agent-Modell bevorzugen; das Frontier-Modell nutzen, wenn maximale Urteilsguete wichtiger ist als Laufzeitstabilitaet.
- Fuer fokussierte Reviews oder CI-Fixes ein coding-optimiertes Modell bevorzugen.
- Fuer triviale Bereinigung, Formatierung oder risikoarme mechanische Edits ist ein schnelles kleines Coding-Modell akzeptabel.

*Model choice is operational agent-routing guidance, not a feature requirement. Do not pin model names in `spec.md`, `plan.md`, `tasks.md`, or individual feature specs; those artifacts must stay reproducible even when model names change or another AI agent is used. Each agent should map these recommendations to its currently available models; do not derive a fixed vendor or model requirement. For Spec-Kit specification, clarification, planning, task generation, and analysis (`/speckit-specify`, `/speckit-clarify`, `/speckit-plan`, `/speckit-tasks`, `/speckit-analyze`; or `/speckit.specify` etc. depending on the agent surface), prefer the strongest available frontier reasoning/coding model. For complete long-running `/speckit-implement` runs, prefer the strongest available long-running agent model; use the frontier model when maximum judgment quality is more important than runtime stability. For focused review or CI fixes, prefer a coding-optimized model. For trivial cleanup, formatting, or low-risk mechanical edits, a fast small coding model is acceptable.*

## Spec-Kit-Preset-Pflege / Spec Kit Preset Maintenance

- Standard-Preset-Set: `security-governance` v0.6.1 prio 10, `architecture-governance` v0.5.1 prio 20, `isaqb-architecture-governance` v0.2.1 prio 30, `a11y-governance` v0.4.1 prio 40, `cross-platform-governance` v0.2.1 prio 50, `agent-parity-governance` v0.4.0 prio 60, `autonomous-run-governance` v0.3.0 prio 70, `parallel-autonomous-run-governance` v0.2.1 prio 80.
- `autonomous-run-governance` v0.3.0 prio 70 ist Teil der Standard-Achtermatrix. Ein vollständiger autonomer Lauf bleibt ausdrücklich delegationspflichtig; die Installation allein erteilt weder Ausführungsberechtigung noch Remote-, Merge-, Bypass- oder Provider-Rechte und `LocalImplementation` bleibt Default. Dokumentations-, Status-, Schema- oder Evidence-Änderungen gelten erst dann als testfrei, wenn keine ausführbaren Validatoren die geänderten Pfade, Marker, Schemas oder Zustandswerte konsumieren. Vor autorisierten Commits wird der exakt beabsichtigte Kandidat mit `git diff --cached --check` und Statusabgleich geprüft; fremde Änderungen bleiben unberührt. Vor einem Merge wird jeder Acceptance-Gate dem tatsächlich ausgeführten Workflow, Job, Runner beziehungsweise der Plattform und dem Befehl zugeordnet; grüne Namen oder ein Bypass ersetzen keinen technischen Nachweis. Bewusst pausierte Läufe werden als `PausedByUser` gespeichert und nur über `speckit.autonomous-resume` fortgesetzt; `speckit.autonomous-stop` wirkt kooperativ am nächsten sicheren Grenzpunkt, und ein gespeicherter Delivery-Modus ist keine aktuelle Berechtigung. Nach Preset- oder Governance-Drift werden neue zwingende Korrektheits-, Sicherheits-, Berechtigungs- und Evidenzregeln minimal mit akzeptierten Plan-, Task- und Checklist-Artefakten abgeglichen; reine Effizienzpräferenzen lösen keine rückwirkende Neugenerierung aus. Die lesbare Skill-Überschrift `Deliver` ist kein Run-State-Wert; für Remote-Closeout gelten ausschließlich `Publish`, `Review` oder `MergeAndSync`.
- `parallel-autonomous-run-governance` v0.2.1 prio 80 ist Teil der Standard-Achtermatrix. Die Installation startet keine Kampagne und erteilt keine zusaetzlichen Remote-, Merge-, Bypass-, Abbruch-, Secret- oder Provider-Rechte. Kampagnen bleiben ausdruecklich delegationspflichtig, verwenden getrennte Worktrees und maximal drei gleichzeitig aktive Worker. Schema 1.1 erlaubt ein `runnerProfile` je Worker mit Kampagnen-Fallback; Modell und Reasoning-Stufe sind optionale, nicht geheime Metadaten und werden ohne Deklaration nicht erraten. Konsolidierung verlangt exakten Head, aktuelle Review- und Check-Evidenz, ist nach Teilmerges fortsetzbar und setzt `Completed` erst nach Synchronisation, manifestdeklarierten idempotenten Post-Merge-Aktionen und Abschlussvalidierung.
- `a11y-governance` v0.4.1 ergänzt didaktische Inline-Code-Kommentar-Governance für neue oder geaenderte nicht-triviale Logik.
- `security-governance` v0.6.1 fuehrt `AI-SBOM` weiter als bedingt anwendbare Supply-Chain-Evidenz, ergänzt sprachspezifische Secure-Coding-Profile und ergänzt regulatorische Anwendbarkeit für NIS2, CRA, EU AI Act und DORA. Reine Entwicklungswerkzeug-Nutzung bleibt `N/A`; KI-Runtime-/Produktkomponenten benoetigen Evidenz nach G7/BSI AI-SBOM-Clustern; private Ausbildungsprojekte dokumentieren regulatorische Nichtanwendbarkeit mit kurzer Begründung.
- `architecture-governance` v0.5.1 ergänzt `BSI C3A` als bedingte Cloud-Autonomie-Evidenz und `BSI C5` als bedingte Cloud-Compliance-Assurance-Evidenz für Cloud-Service-Auswahl, Provider-Abhängigkeiten, Audit-/Nachweisstand, Shared Responsibility und Betriebsnachweise.
- Alle acht Presets enthalten ab diesem Release-Block audit-ready Spec-Kit-Run-Evidenz: `Applicable` / `N/A` / `Open`, Begründung, Evidenzpfad, Reviewer, Restrisiko und Follow-up muessen im aktuellen Spec-Kit-Lauf dokumentiert werden.
- Die ursprünglichen sechs Presets sind seit 2026-05-04 und `autonomous-run-governance` v0.2.2 ist seit 2026-07-17 im `github/spec-kit` Community-Katalog enthalten und liegen zusätzlich als veröffentlichte Repos unter `https://github.com/hindermath/spec-kit-preset-*`.
- `parallel-autonomous-run-governance` v0.2.1 ist eigenstaendig veroeffentlicht und wurde mit `github/spec-kit#3591` fuer den Community-Katalog eingereicht.
- Registrierte Level-0-, Level-1- und Level-2-Repositories installieren bei vorhandener Spec-Kit-Integration standardmäßig alle acht Presets aus `scripts/config/spec-kit-governance-presets.json`, sofern keine begründete Ausnahme dokumentiert ist.
- Referenz-Rollout für alle acht Presets: `RiderProjects/TinyPl0`, `RiderProjects/TinyCalc`, `RiderProjects/TuiVision`, `RiderProjects/InventarWorkerService`.
- Installation erfolgt bevorzugt mit `install-spec-kit-governance-presets.*` aus der zentralen Matrix; die Skriptlogik enthaelt keine fest eingebauten Versionen. Bei neuen Preset-Releases zuerst die Matrix aktualisieren, dann bestehende Repos bewusst mit `--force` / `-Force` nachziehen.
- Flotten-Rollouts erfassen Level-0, Level-1 und Level-2 explizit. Eine reine Level-2-Registry beweist keine vollstaendige Abdeckung; jeder Zielstatus wird bis Installation, exakter Matrixvalidierung, Commit, Push und Remote-Synchronisation verfolgt.
- Vor dem Staging werden generierte Preset-/Agentenpfade mit dem gesamten Arbeitsbaum abgeglichen. Fremde Aenderungen bleiben unberuehrt; bei Konflikten wird ein sauberer Worktree statt eines erzwungenen Misch-Commits verwendet.
- Aktuelle normative Sechs-/Siebenerangaben werden auf die Achtermatrix migriert. Historische Statistik-, Changelog-, Feldnachweis- und Kompatibilitaetsangaben bleiben erhalten und werden durch einen dokumentierten Allowlist-Scan unterschieden.
- Provider-/Billing-Ablehnung, technischer Gate-Fehler und bestandener Gate sind getrennte Ergebnisse. Bypass oder gruene Sammelnamen ersetzen keinen exakten technischen Nachweis.
- `.specify/presets/` und generierte Agenten-/Command-Dateien committen, wenn Presets Projekt-Policy sind; `.specify/presets/.cache/` nie committen.
- Nach Installation oder Update prüfen: `specify preset list`, mindestens ein `specify preset info <id>`, bei Template-Fragen zusätzlich `specify preset resolve <template>`.
- Die lokale Arbeitskopie der veröffentlichten Preset-Repos liegt unter `~/SpecKitPresetProjects/`; kanonische Scaffolds in diesem Repo liegen unter `specs/spec-kit-presets/` und `specs/spec-kit-preset-repos/`.
- Verbesserungen an Presets zuerst im `home-baseline`-Scaffold einarbeiten, dann in die passenden Repos unter `~/SpecKitPresetProjects/` übertragen, committen, pushen und mit GitHub-ZIP-URL smoke-testen.
- Bei Änderungen an Preset-Regeln immer prüfen, ob `constitution.md`, `.specify/memory/constitution.md`, `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md` und `scripts/templates/*` ebenfalls aktualisiert werden müssen.
- Bei jeder Preset-Version oder Prioritätsänderung zuerst `scripts/config/spec-kit-governance-presets.json` aktualisieren und danach README-Tabellen, Constitution, Agenten-Dateien, `scripts/templates/speckit-workflow-section.md` und Agenten-Templates gemeinsam prüfen.

*Fleet rollouts explicitly cover level 0, level 1, and level 2 and track each
target through installation, exact matrix validation, commit, push, and remote
synchronization. Separate generated paths from unrelated work before staging.
Migrate current normative six/seven references while preserving allowlisted
history and compatibility aliases. Provider refusal, technical gate failure,
and passing evidence are distinct; bypass is not technical proof.*
- Community-/Katalog-Abstimmung läuft über `github/spec-kit#2362`.

*Standard preset set: `security-governance` v0.6.1 prio 10, `architecture-governance` v0.5.1 prio 20, `isaqb-architecture-governance` v0.2.1 prio 30, `a11y-governance` v0.4.1 prio 40, `cross-platform-governance` v0.2.1 prio 50, `agent-parity-governance` v0.4.0 prio 60, `autonomous-run-governance` v0.3.0 prio 70, and `parallel-autonomous-run-governance` v0.2.1 prio 80. `a11y-governance` v0.4.1 adds didactic inline-code-comment governance for new or changed non-trivial logic. `architecture-governance` v0.5.1 adds conditional `BSI C3A` cloud-autonomy evidence and `BSI C5` cloud-compliance assurance evidence for cloud-service selection, provider dependencies, audit/assurance status, shared responsibility, and operational evidence. `security-governance` v0.6.1 keeps conditional `AI-SBOM` evidence, language-specific secure-coding profiles, and regulatory applicability screening for NIS2, CRA, EU AI Act, and DORA: development-tool-only AI usage is `N/A`, AI runtime/product components require G7/BSI AI-SBOM cluster evidence, and private training projects record regulatory `N/A` when no regulated scope exists. All eight presets now include audit-ready Spec-Kit run evidence: `Applicable` / `N/A` / `Open`, rationale, evidence path, reviewer, residual risk, and follow-up must be documented for the current Spec-Kit run. The original six presets have been in the `github/spec-kit` community catalog since 2026-05-04, and `autonomous-run-governance` v0.2.2 was verified there on 2026-07-17. All eight are also published under `https://github.com/hindermath/spec-kit-preset-*`. `parallel-autonomous-run-governance` v0.2.1 was submitted to the community catalog as `github/spec-kit#3591`. Registered level-0, level-1, and level-2 repositories with Spec Kit default to all eight presets from `scripts/config/spec-kit-governance-presets.json` unless a justified exception is documented. Use `install-spec-kit-governance-presets.*` so preset versions stay centralized in the matrix. Commit `.specify/presets/` and generated agent command updates when presets are project policy, but never commit `.specify/presets/.cache/`. Verify installs with `specify preset list`, `specify preset info`, and where relevant `specify preset resolve`. Improve presets in the home-baseline scaffold first, propagate to standalone preset repos, then commit, push, and smoke-test via GitHub ZIP URL. Preset-rule changes and preset version/priority changes require reviewing the central matrix, constitution, README tables/install snippets, all agent guidance files, and relevant templates together. Community/catalog coordination happens in `github/spec-kit#2362`.*

<!-- EN: AGENTS.md placeholder
[DE-Zusammenfassung: AGENTS.md enthält Anweisungen für den Codex Agenten im home-baseline Repository.]
-->

<!-- learner-a11y-baseline:start -->
## Lernenden- und A11Y-Basis / Learner and A11Y Baseline

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
