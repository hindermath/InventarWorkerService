# Feature Specification: Secure-Development-Hardening

**Feature Branch**: `002-secure-development-hardening`
**Created**: 2026-08-30
**Status**: Draft
**Input**: `Lastenheft_Secure-Development-Hardening.md` als verbindlicher, bereits geprüfter und manifestfähiger Intake / as the binding, already reviewed and manifest-eligible intake

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Sicherheitslage vollständig und ehrlich bewerten (Priority: P1)

Als Projektverantwortung oder Security-Reviewer möchte ich jeden stabilen Prüfpunkt der zwölf Secure-Development-Checklisten gegen reale Repository-Evidenz bewerten, damit erfüllte, offene und nicht anwendbare Anforderungen auditfähig unterschieden werden.

*As a project owner or security reviewer, I want every stable checkpoint in the twelve secure-development checklists to be assessed against real repository evidence so that fulfilled, open, and non-applicable requirements are distinguished in an audit-ready way.*

**Why this priority**: Ohne eine vollständige und belegte Ausgangsbewertung wären spätere Härtungen selektiv und positive Aussagen nicht verlässlich.

*Without a complete, evidence-backed baseline assessment, later hardening would be selective and positive compliance statements would not be reliable.*

**Independent Test**: Die ausgefüllten Projektinstanzen enthalten für alle 157 stabilen CL-IDs eine zulässige Anwendbarkeits- und Umsetzungsbewertung, Rolle, Begründung, Evidenz oder Folgeschritt, Restrisiko und Neubewertungs-Trigger. Keine positive Aussage stützt sich nur auf einen Stub oder eine Dateiexistenz.

*The completed project instances contain an allowed applicability and implementation assessment, role, rationale, evidence or follow-up, residual risk, and re-evaluation trigger for all 157 stable CL IDs. No positive statement relies only on a stub or file existence.*

**Acceptance Scenarios**:

1. **Given** eine kanonische Checklisten-ID, **When** die Projektbewertung gelesen wird, **Then** besitzt sie genau eine nachvollziehbare Klassifikation und einen konkreten Evidenz- oder Folgepfad.
2. **Given** ein vorhandener Sicherheits-Stub, **When** keine projektspezifische Prüfung nachgewiesen ist, **Then** wird der Punkt nicht als erfüllt gewertet.
3. **Given** ein nicht anwendbarer Prüfpunkt, **When** der Bericht geprüft wird, **Then** nennt er eine kurze Begründung und einen Neubewertungs-Trigger.

*1. **Given** a canonical checklist ID, **When** its project assessment is read, **Then** it has exactly one traceable classification and a concrete evidence or follow-up path.
2. **Given** an existing security stub, **When** no project-specific review is evidenced, **Then** the item is not treated as fulfilled.
3. **Given** a non-applicable checkpoint, **When** the report is reviewed, **Then** it states a short rationale and a re-evaluation trigger.*

---

### User Story 2 - Verteilte Inventardienste sicher betreiben (Priority: P2)

Als Systemadministrator möchte ich, dass API-, Datenbank-, Datei-, Konfigurations- und Prozessgrenzen nach sicheren Standardwerten geschützt und überprüfbar sind, damit Inventar- und Zugangsdaten nicht durch offene Schnittstellen, ungeprüfte Eingaben oder interne Fehlermeldungen offengelegt werden.

*As a system administrator, I want API, database, file, configuration, and process boundaries to be protected by secure defaults and to be verifiable so that inventory and credential data are not exposed through open interfaces, unvalidated input, or internal error messages.*

**Why this priority**: Die Worker-, Harvester- und Viewer-Komponenten tauschen schützenswerte Inventardaten über mehrere Vertrauensgrenzen aus.

*The worker, harvester, and viewer components exchange sensitive inventory data across several trust boundaries.*

**Independent Test**: Repräsentative positive und negative Szenarien prüfen Zugriffskontrolle, Transport, Eingabevalidierung, Fehlerbehandlung, Protokollierung, ausgehende Verbindungen und Datenbankrechte an jeder betroffenen Grenze.

*Representative positive and negative scenarios verify access control, transport, input validation, error handling, logging, outbound connections, and database privileges at every affected boundary.*

**Acceptance Scenarios**:

1. **Given** ein nicht berechtigter Zugriff auf schützenswerte Inventardaten, **When** die Anfrage eine HTTP-Grenze erreicht, **Then** wird sie sicher abgewiesen oder eine ausdrücklich akzeptierte, eng begrenzte Ausnahme mit Kompensationsmaßnahmen ist dokumentiert.
2. **Given** fehlerhafte externe Eingaben, **When** sie API-, Import-, Konfigurations- oder Datenbankgrenzen erreichen, **Then** werden sie vor sicherheitsrelevanter Verarbeitung abgewiesen und erzeugen keine sensiblen Ausgaben.
3. **Given** ein interner Fehler, **When** ein Endnutzer oder API-Client die Antwort erhält, **Then** enthält sie keine Stack-Traces, Connection Strings, Zugangsdaten oder ungefilterten internen Ausnahmeinformationen.
4. **Given** ein nicht erreichbarer oder manipulativ adressierter externer Dienst, **When** ein Client-Verbindungsversuch erfolgt, **Then** verhindern Zielprüfung, Zeitgrenzen und sichere Fehlerbehandlung ein unbegrenztes oder unerlaubtes Verhalten.
5. **Given** ein Teilfehler nach begonnener zustandsändernder Verarbeitung oder ein unterbrochener Evidenzlauf, **When** die Wiederherstellung beginnt, **Then** sind sicherer Restzustand, Wiederholbarkeit oder Rollback, Bedienhinweis und erneute Gate-Prüfung eindeutig festgelegt.

*1. **Given** an unauthorized request for protected inventory data, **When** it reaches an HTTP boundary, **Then** it is denied safely or an explicitly accepted, narrowly scoped exception with compensating controls is documented.
2. **Given** malformed external input, **When** it reaches API, import, configuration, or database boundaries, **Then** it is rejected before security-relevant processing and produces no sensitive output.
3. **Given** an internal failure, **When** an end user or API client receives the response, **Then** it contains no stack traces, connection strings, credentials, or unfiltered internal exception details.
4. **Given** an unavailable or maliciously addressed external service, **When** a client connection is attempted, **Then** destination checks, time limits, and safe error handling prevent unbounded or unauthorized behaviour.
5. **Given** a partial failure after state-changing processing has started or an interrupted evidence run, **When** recovery begins, **Then** the safe residual state, retry or rollback, operator guidance, and renewed gate verification are unambiguously defined.*

---

### User Story 3 - Lieferkette und Releases nachvollziehbar absichern (Priority: P3)

Als Release-Reviewer möchte ich für jedes verteilbare Artefakt Abhängigkeiten, Schwachstellenstatus und Herkunft nachvollziehen können, damit Releases nur mit transparentem Komponentenbestand und begründeten Risikodezisionen freigegeben werden.

*As a release reviewer, I want dependencies, vulnerability status, and provenance to be traceable for every distributable artefact so that releases are approved only with a transparent component inventory and reasoned risk decisions.*

**Why this priority**: Sichere Implementierung allein schützt nicht vor verwundbaren oder manipulierten Abhängigkeiten und Build-Artefakten.

*Secure implementation alone does not protect against vulnerable or tampered dependencies and build artefacts.*

**Independent Test**: Ein reproduzierbarer Release-Kandidat besitzt eine maschinenlesbare SBOM, eine aktuelle Abhängigkeitsbewertung, nachvollziehbare Build-Herkunft und bei bekannten Schwachstellen eine VEX-artige Disposition.

*A reproducible release candidate has a machine-readable SBOM, a current dependency assessment, traceable build provenance, and a VEX-style disposition when known vulnerabilities exist.*

**Acceptance Scenarios**:

1. **Given** ein verteilbarer Release-Kandidat, **When** die Freigabeevidenz geprüft wird, **Then** ist sein vollständiger Komponentenbestand maschinenlesbar verfügbar und dem Artefakt eindeutig zugeordnet.
2. **Given** eine bekannte Schwachstelle in einer ausgelieferten oder bewerteten Komponente, **When** die Release-Entscheidung erfolgt, **Then** ist der Status als betroffen, nicht betroffen, behoben oder in Prüfung dokumentiert.
3. **Given** ein Build aus der CI/CD-Kette, **When** seine Herkunft geprüft wird, **Then** sind Quelle, Build-Schritte und Integritätsstatus im vorgesehenen SLSA-Zielpfad nachvollziehbar.

*1. **Given** a distributable release candidate, **When** its release evidence is reviewed, **Then** its complete component inventory is available in machine-readable form and uniquely linked to the artefact.
2. **Given** a known vulnerability in a shipped or assessed component, **When** the release decision is made, **Then** its status is recorded as affected, not affected, remediated, or under investigation.
3. **Given** a CI/CD build, **When** its origin is reviewed, **Then** source, build steps, and integrity status are traceable along the intended SLSA path.*

---

### User Story 4 - Nachweise barrierearm lernen und prüfen (Priority: P4)

Als auszubildende Person, Entwickler:in oder Reviewer möchte ich Sicherheitsentscheidungen in klarer, deutsch-erster und englisch-zweiter Sprache ohne visuelle Abhängigkeit verstehen, damit ich Prüfungen selbstständig nachvollziehen und wiederholen kann.

*As an apprentice, developer, or reviewer, I want to understand security decisions in clear German-first and English-second language without visual dependency so that I can independently follow and repeat the checks.*

**Why this priority**: Governance-Evidenz ist nur dauerhaft nutzbar, wenn neue Teammitglieder und assistive Technologien Status, Begründung und nächste Schritte gleichwertig erfassen können.

*Governance evidence remains useful only when new team members and assistive technologies can understand status, rationale, and next actions equally.*

**Independent Test**: Betroffene Dokumentation und textbasierte Ausgaben bestehen die festgelegte WCAG-2.2-AA-orientierte Prüfung, bleiben in einem Textbrowser verständlich und erklären Fachbegriffe bei der ersten Verwendung auf CEFR-B2-Niveau.

*Affected documentation and text output pass the defined WCAG 2.2 AA-oriented review, remain understandable in a text browser, and explain specialist terms at first use at CEFR B2 level.*

**Acceptance Scenarios**:

1. **Given** eine Person ohne Spec-Kit-Vorerfahrung, **When** sie eine Prüfentscheidung liest, **Then** erkennt sie Status, Begründung, Evidenz, Risiko und nächsten Schritt ohne Rückfrage.
2. **Given** ein Screenreader oder Textbrowser, **When** er eine Ergebnisübersicht verarbeitet, **Then** bleiben alle Informationen ohne Farbe, Diagrammposition oder Bildinhalt vollständig.
3. **Given** geänderte öffentliche Dokumentation oder generiertes HTML, **When** die A11Y-Prüfung läuft, **Then** sind die anwendbaren WCAG-2.2-AA-Kriterien nachgewiesen oder verbleibende Abweichungen als offene Risiken dokumentiert.

*1. **Given** a person with no prior Spec Kit experience, **When** they read a review decision, **Then** they can identify status, rationale, evidence, risk, and next action without asking for clarification.
2. **Given** a screen reader or text browser, **When** it processes a result summary, **Then** all information remains complete without colour, diagram position, or image content.
3. **Given** changed public documentation or generated HTML, **When** the accessibility review runs, **Then** applicable WCAG 2.2 AA criteria are evidenced or remaining deviations are recorded as open risks.*

### Edge Cases

- Ein vorhandener Scanner oder eine vorhandene Dokumentdatei beweist nur den geprüften Teilumfang; nicht geprüfte Kontrollen bleiben `Open` oder `Not Assessed`.
- Unterschiedliche Ergebnisse zwischen SQLite, MongoDB und PostgreSQL werden je Provider belegt; ein positives Ergebnis wird nicht ungeprüft übertragen.
- Wenn kein bekannter Schwachstellenfund vorliegt, bleibt die VEX-Erzeugung bedingt; die Supply-Chain-Evidenz dokumentiert den Prüfzeitpunkt und den Trigger für eine spätere Disposition.
- AI-SBOM bleibt `N/A`, solange KI ausschließlich Entwicklungswerkzeug ist. Eine spätere KI-Runtime-, Modell-, Daten- oder Inferenzkomponente löst eine Neubewertung aus.
- BSI C3A und BSI C5 bleiben `N/A`, solange dieser Lauf keinen Cloud-Anbieter, Managed Service oder providerabhängiges Deployment einführt.
- Widersprüchliche oder fehlende Evidenz darf nicht durch eine Annahme geschlossen werden; sie erzeugt einen benannten offenen Befund mit Owner, Risiko, Priorität und Termin.
- Eine Härtung darf Windows-, macOS- oder Linux-Betrieb nicht stillschweigend verschlechtern. Plattformabweichungen benötigen Evidenz und eine begründete Entscheidung.
- Generierte Dateien wie DocFX-Ausgabe, SBOM und Scanberichte dürfen nicht allein wegen lokaler Existenz als versionierte Quelle gelten; ihr kanonischer Ursprung und ihre Verteilung müssen benannt sein.

*Existing scanners or documents prove only their reviewed scope; provider-specific results are not transferred without evidence; missing or conflicting evidence creates a named finding; conditional VEX, AI-SBOM, cloud, platform, and generated-artefact decisions are re-evaluated when their stated triggers occur.*

## Scope und Nicht-Ziele / Scope and Non-Goals

### In Scope

- Worker-, API-, Harvester-, Viewer-, Shared-Library- und Service-Control-Grenzen sowie deren Datenflüsse.
- SQLite-, MongoDB- und PostgreSQL-Zugriffe, Import/Export, Views, Abfragen, Migrationen, Dateien, Konfiguration, Secrets, Logging und Prozesszugriffe.
- HTTP/API-Schutz für InventarWorkerService und die Viewer-API mit OWASP ASVS Level 2 als Verifikationsniveau.
- Die zwölf kanonischen Checklisten unter `docs/secure-development/checklisten/` und ihre 157 stabilen CL-IDs.
- Projektspezifische Sicherheits-, Architektur-, Barrierefreiheits-, Test-, CI-, Dependency-, SBOM-, VEX-, SLSA- und Abschlussnachweise.
- Synchronisierte Agenten- und Spec-Kit-Governance, soweit der aktuelle installierte Preset-Stack oder gemeinsame Regeln betroffen sind.

*The scope covers all runtime and data boundaries, both HTTP/API surfaces at OWASP ASVS Level 2, all twelve canonical secure-development checklists and their 157 stable IDs, project-specific evidence, and synchronized local governance where current installed presets or shared rules are affected.*

### Out of Scope

- Neue fachliche Inventarfunktionen, neue Datenbankprovider oder ein allgemeiner Plattformumbau ohne direkten Härtungsbefund.
- Cloud-Migration, Auswahl eines Cloud-Anbieters, KI-Runtime- oder KI-Produktfunktionen.
- Formale Zertifizierung, Rechtsberatung, externe Penetrationstests oder eine pauschale Zusage vollständiger Normkonformität.
- Änderungen außerhalb dieses Repositorys; erkannter Home-Baseline-Bedarf wird als getrenntes Follow-up dokumentiert.
- Commit, Push, Pull Request, Merge oder Änderung von `autonomous-run-state.json` innerhalb der Specify-Phase.

*New business features, cloud migration, AI runtime/product functions, formal certification, legal advice, external penetration testing, changes outside this repository, and coordinator-owned delivery or run-state transitions are out of scope for this specification phase.*

## Intake-Klassifikation / Intake Classification

Nur Einträge mit `Applicable` werden in Plan und Aufgaben dieses Features übernommen. `AlreadySatisfied` wird erneut belegt, aber nicht umgesetzt. `N/A` wird nicht geplant und besitzt einen Trigger. `Open` benötigt Owner, Risiko, Folgeschritt und Termin, bevor es planbar wird. `FollowUp` bleibt außerhalb dieses Features.

*Only `Applicable` entries enter this feature's plan and tasks. `AlreadySatisfied` is re-evidenced but not implemented. `N/A` is excluded from planning and has a trigger. `Open` needs an owner, risk, next action, and date before it becomes plannable. `FollowUp` remains outside this feature.*

| ID | Intake-Anforderung / Intake requirement | Klassifikation | Begründung und Evidenz / Rationale and evidence |
|---|---|---|---|
| IR-001 | Verbindlichen Scope und aktuellen Repository-Stand prüfen / Review binding scope and current repository state | Applicable | Aktuelle Code-, CI-, Dokumentations- und Governance-Evidenz muss gegen den Intake bewertet werden. / Current code, CI, documentation, and governance evidence must be assessed against the intake. |
| IR-002 | Alle relevanten Secure-Development-Prüfpunkte klassifizieren / Classify all relevant secure-development checkpoints | Applicable | Der breite Härtungsscope macht alle zwölf Checklisten und 157 CL-IDs prüfrelevant; einzelne Ergebnisse dürfen `N/A` sein. / The broad hardening scope makes all twelve checklists and 157 CL IDs review-relevant; individual outcomes may be `N/A`. |
| IR-003 | Projektspezifische Sicherheitsnachweise vervollständigen / Complete project-specific security evidence | Applicable | `docs/security/README.md` weist die Kernnachweise überwiegend als Stubs aus. / The security index identifies most core evidence files as stubs. |
| IR-004 | API-, Datenbank-, Datei-, Konfigurations- und Prozessgrenzen härten / Harden API, database, file, configuration, and process boundaries | Applicable | Diese Grenzen verarbeiten Inventar-, System- oder Zugangsdaten und benötigen belegte sichere Standardwerte. / These boundaries process inventory, system, or credential data and need evidenced secure defaults. |
| IR-005 | Build-, Test-, CI- und Provider-Parität belegen / Evidence build, test, CI, and provider parity | Applicable | Der Level-2-Registry-Pfad verlangt Restore, Build, Tests und bedarfsweise Playwright; Security-Negativfälle und Provider-Parität fehlen als vollständige Härtungsevidenz. / The Level-2 registry requires restore, build, tests, and Playwright where needed; complete hardening evidence for negative cases and provider parity is missing. |
| IR-006 | Dependency- und Supply-Chain-Nachweise herstellen / Establish dependency and supply-chain evidence | Applicable | Releasefähige Artefakte, externe Pakete und CI/CD machen Audit, SBOM, SLSA und bedingtes VEX erforderlich. / Release-capable artefacts, external packages, and CI/CD require audit, SBOM, SLSA, and conditional VEX. |
| IR-007 | A11Y- und didaktische Governance prüfen / Review accessibility and didactic governance | Applicable | Dokumentation, TUI/CLI/API-Ausgaben und mögliche Logikänderungen sind nutzer- oder lernrelevant. / Documentation, TUI/CLI/API output, and possible logic changes are user- or learner-facing. |
| IR-008 | Agentenflächen und Governance-Presets abgleichen / Align agent surfaces and governance presets | Applicable | Das Lastenheft nennt eine ältere Sechser-Teilmenge; der installierte Zwölfer-Stack ist heute maßgeblich. / The intake lists an older six-preset subset; the installed twelve-preset stack is now authoritative. |
| IR-009 | Ergebnisübersicht, Restrisiken und Folgeaufgaben liefern / Deliver result summary, residual risks, and follow-ups | Applicable | Der Intake verlangt einen auditfähigen Abschluss. / The intake requires an audit-ready closeout. |
| IR-010 | Wiederverwendbare Secure-Development-Basis bereitstellen / Provide reusable secure-development baseline | AlreadySatisfied | Richtlinie, Sammelband, Baseline-Manifest und CL-01 bis CL-12 sind unter `docs/secure-development/` vorhanden; dies beweist noch keine Projekterfüllung. / Guideline, compendium, baseline manifest, and CL-01 through CL-12 exist; this does not yet prove project compliance. |
| IR-011 | Memory-Safe-Language-Auswahl / Memory-safe-language selection | AlreadySatisfied | C# ist nach Constitution-Prinzip XI eine speichersichere Sprache; der Registry-Eintrag bestätigt .NET 10/C# 14. / C# is memory-safe under Principle XI, and the registry confirms .NET 10/C# 14. |
| IR-012 | Sicherheitsnachweis-Verzeichnis und Grundgerüste / Security evidence directory and scaffolds | AlreadySatisfied | `docs/security/` und die kanonisch benannten Grunddateien existieren; die inhaltliche Befüllung bleibt `Applicable`. / The directory and canonically named files exist; populating them remains `Applicable`. |
| IR-013 | Vorhandene Secret- und A11Y-CI-Grundkontrollen / Existing secret and accessibility CI baseline controls | AlreadySatisfied | Gitleaks, Agent-Secret-Scan sowie DocFX-, Axe- und Lynx-Schritte sind vorhanden; Reichweite und aktuelle Wirksamkeit werden dennoch geprüft. / Gitleaks, agent secret scanning, and DocFX, Axe, and Lynx steps exist; scope and current effectiveness are still reviewed. |
| IR-014 | AI-SBOM / AI SBOM | N/A | KI wird nur als Entwicklungswerkzeug verwendet; keine KI-Komponente wird ausgeliefert oder betrieben. Trigger: Modell, KI-Dienst, Datensatz, Inferenz-Infrastruktur oder KI-Runtime wird Produktbestandteil. / AI is development tooling only; no AI component is shipped or operated. Trigger: an AI model, service, dataset, inference infrastructure, or runtime becomes part of the product. |
| IR-015 | BSI C3A/C5 Cloud-Nachweise / BSI C3A/C5 cloud evidence | N/A | Der Intake führt keine Cloud- oder Managed-Service-Auswahl ein. Trigger: providerabhängiges Hosting, SaaS/PaaS/IaaS oder Cloud-Assurance. / The intake introduces no cloud or managed-service selection. Trigger: provider-dependent hosting, SaaS/PaaS/IaaS, or cloud assurance. |
| IR-016 | Neues skriptförmiges Werkzeug / New script-shaped tool | N/A | Die Spezifikation fordert kein neues oder geändertes Skript; vorhandene Befehle und CI können die Nachweise tragen. Trigger: Der Plan führt ein Skript ein oder ändert eines. Dann gelten Bash/PowerShell-Parität, Manpage, deutsch-erste/englisch-zweite PowerShell-Hilfe, `--dry-run`/`-WhatIf` und ein zulässiger `Verb-Noun`-Name. / The specification requires no new or changed script. Trigger: the plan introduces or changes one; all cross-platform parity requirements then apply. |
| IR-017 | Externer Penetrationstest oder formale Zertifizierung / External penetration test or formal certification | FollowUp | Nach Bedrohungsmodell und Restrisikobewertung entscheidet die Projektverantwortung separat über externen Prüfbedarf; dieser Lauf beansprucht keine Zertifizierung. / After threat modelling and residual-risk assessment, the project owner separately decides whether external review is needed; this run claims no certification. |
| IR-018 | Wiederkehrende Monats- und Release-Prüfungen / Recurring monthly and release reviews | FollowUp | Dieses Feature richtet Evidenz und Auslöser ein; zukünftige Monats- und Release-Ausführungen sind laufender Betrieb mit Projektverantwortung als Owner. / This feature establishes evidence and triggers; future monthly and release executions are ongoing operations owned by the project owner. |

**Open-Status im Specify-Zeitpunkt / Open status at specify time**: Es bleibt keine scopebestimmende Unklarheit offen. Neue technische Befunde dürfen während der Umsetzung als `Open` mit Owner, Priorität, Risiko, Zieltermin und Neubewertungs-Trigger erfasst werden.

*No scope-defining ambiguity remains at specification time. New technical findings may be recorded during implementation as `Open` with owner, priority, risk, due date, and re-evaluation trigger.*

### Ausgewählte Secure-Development-Checklisten / Selected Secure-Development Checklists

Alle zwölf Checklisten sind für die Bewertung `Applicable`, weil der Intake den vollständigen Entwicklungs-, Betriebs-, Lieferketten-, Datenschutz- und Agentenscope umfasst. Diese Auswahl bedeutet nicht, dass jeder einzelne Prüfpunkt anwendbar ist; die Projektinstanz darf einzelne CL-IDs mit Begründung als `N/A` bewerten.

*All twelve checklists are applicable to the assessment because the intake covers the complete development, operations, supply-chain, privacy, and agent scope. This selection does not make every individual checkpoint applicable; the project instance may classify individual CL IDs as N/A with rationale.*

| Checkliste / Checklist | Auswahlbegründung / Selection rationale | Projektinstanz / Project instance |
|---|---|---|
| CL-01 Standards-Anwendbarkeit / Standards Applicability | Level-2-Standards und regulatorische Trigger müssen vollständig entschieden werden. / Level-2 standards and regulatory triggers need complete decisions. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_01_Standards-Anwendbarkeit.md` |
| CL-02 Sichere Softwarearchitektur / Secure Software Architecture | Verteilte Dienste, Datenbanken und mehrere Trust Boundaries sind betroffen. / Distributed services, databases, and several trust boundaries are affected. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_02_Sichere-Softwarearchitektur.md` |
| CL-03 Krypto-Mindestvorgaben / Cryptographic Minimum Requirements | Transport, Secret-Speicherung und Zugangsdaten benötigen eine belegte Krypto- und TLS-Entscheidung. / Transport, secret storage, and credentials need evidenced cryptographic and TLS decisions. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_03_Krypto-Mindestvorgaben.md` |
| CL-04 Bedrohungsmodellierung / Threat Modelling | STRIDE, CIA und CAPEC sind für die benannten Grenzen verpflichtend. / STRIDE, CIA, and CAPEC are required for the named boundaries. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_04_Bedrohungsmodellierung.md` |
| CL-05 Lieferkette und Build-Integrität / Supply Chain and Build Integrity | Pakete, CI/CD, Releases, SBOM, SLSA, VEX und OpenSSF sind im Scope. / Packages, CI/CD, releases, SBOM, SLSA, VEX, and OpenSSF are in scope. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_05_Lieferkette-Build-Integritaet.md` |
| CL-06 Schwachstellenoffenlegung / Vulnerability Disclosure | Der Release- und VEX-Scope braucht Melde-, Dispositions- und Reaktionswege. / The release and VEX scope needs disclosure, disposition, and response paths. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_06_Schwachstellenoffenlegung.md` |
| CL-07 CRA-Anwendbarkeit / CRA Applicability | Verteilbare Artefakte erfordern eine dokumentierte CRA-Screening-Entscheidung. / Distributable artefacts require a documented CRA screening decision. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_07_CRA-Anwendbarkeit.md` |
| CL-08 Sicherheits-Code-Review / Security Code Review | API-, SQL-, Datei-, Konfigurations-, Fehler- und Serialisierungspfade werden gehärtet. / API, SQL, file, configuration, error, and serialization paths are hardened. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_08_Sicherheits-Code-Review.md` |
| CL-09 KI-Codeerzeugung / AI Code Generation | KI ist Entwicklungswerkzeug; sichere Erzeugung, Review und AI-SBOM-N/A müssen trotzdem belegt werden. / AI is development tooling; secure generation, review, and AI-SBOM N/A still need evidence. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_09_KI-Codeerzeugung.md` |
| CL-10 Sichere Entwicklungsumgebung / Secure Development Environment | Lokale Toolchains, Secrets, Plattformen, Scanner und CI-Grenzen sind prüfrelevant. / Local toolchains, secrets, platforms, scanners, and CI boundaries require review. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_10_Sichere-Entwicklungsumgebung.md` |
| CL-11 Datenschutz-Folgenabschätzung / Data Protection Impact Assessment | Inventar-, Maschinen- und mögliche Nutzerbezüge benötigen ein dokumentiertes Datenschutz-Screening. / Inventory, machine, and possible user references need a documented privacy screening. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_11_Datenschutz-Folgenabschaetzung.md` |
| CL-12 Agentische KI-Sandbox / Agentic AI Sandbox | Der autonome Lauf und mehrere Agentenflächen benötigen Authority-, Secret-, Sandbox- und Evidenzgrenzen. / The autonomous run and multiple agent surfaces need authority, secret, sandbox, and evidence boundaries. | `docs/security/secure-development/2026-08-30-secure-development-hardening/CL_12_Agentische-KI-Sandbox.md` |

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Der Lauf MUSS alle 157 stabilen IDs aus CL-01 bis CL-12 in einer projektspezifischen Evidenzinstanz bewerten. / *The run MUST assess all 157 stable IDs from CL-01 through CL-12 in a project-specific evidence instance.*
- **FR-002**: Jeder Prüfpunkt MUSS Anwendbarkeit (`Applicable`, `N/A` oder `Open`) und Umsetzungsstatus (`Fulfilled`, `Partly Fulfilled`, `Not Fulfilled` oder `Not Assessed`) getrennt führen und Rolle, Begründung, Evidenz, Restrisiko, Trigger sowie nächste Maßnahme enthalten. / *Each checkpoint MUST keep applicability and implementation status separate and include role, rationale, evidence, residual risk, trigger, and next action.*
- **FR-003**: Positive Erfüllungsaussagen MÜSSEN auf konkrete, aktuelle und prüfbare Evidenz mit Quelle oder Befehl, exaktem Scope, Zeitpunkt, Reviewer und Integritätsbezug verweisen; Stubs, Dateiexistenz oder nicht ausgeführte Vorlagen reichen nicht aus. Evidenz wird bei einer einschlägigen Scope-, Code-, Abhängigkeits-, Konfigurations-, CI-, Standard- oder Baseline-Änderung sowie nach Ablauf ihres erklärten Prüfrhythmus ungültig und MUSS vor einer positiven Aussage erneuert werden. / *Positive compliance statements MUST reference concrete, current, verifiable evidence with source or command, exact scope, time, reviewer, and integrity reference; stubs, file existence, or unexecuted templates are insufficient. Evidence becomes stale after a relevant scope, code, dependency, configuration, CI, standard, or baseline change or when its declared review cadence expires, and MUST be renewed before a positive claim.*
- **FR-004**: Die projektspezifischen Sicherheitsdokumente unter `docs/security/` MÜSSEN für den festgestellten Scope vervollständigt und im Index mit ehrlichem Status geführt werden. / *Project-specific security documents under `docs/security/` MUST be completed for the assessed scope and indexed with an honest status.*
- **FR-005**: Das Bedrohungsmodell MUSS Assets mit CIA-Einstufung, alle relevanten Trust Boundaries, STRIDE-Kategorien, die risikoreichsten CAPEC-Muster, Schutzmaßnahmen und Restrisiken enthalten. / *The threat model MUST include assets with CIA ratings, all relevant trust boundaries, STRIDE categories, the highest-risk CAPEC patterns, controls, and residual risks.*
- **FR-006**: Beide HTTP/API-Flächen MÜSSEN gegen OWASP ASVS Level 2 für Zugriffskontrolle, Transport, Eingaben, Fehler, Protokollierung, Konfiguration und veröffentlichte Dokumentationsendpunkte verifiziert werden. / *Both HTTP/API surfaces MUST be verified against OWASP ASVS Level 2 for access control, transport, input, errors, logging, configuration, and published documentation endpoints.*
- **FR-007**: Jede schützenswerte HTTP-Funktion MUSS eine Fail-Safe-Zugriffsentscheidung besitzen; unauthentifizierte oder weit erreichbare Ausnahmen benötigen enge Begrenzung, begründete Risikoakzeptanz und mindestens eine unabhängige Kompensationsmaßnahme. / *Every protected HTTP function MUST have a fail-safe access decision; unauthenticated or broadly reachable exceptions require narrow scope, reasoned risk acceptance, and at least one independent compensating control.*
- **FR-008**: Nutzer- und API-Fehlerausgaben DÜRFEN keine internen Ausnahmen, Stack-Traces, Connection Strings, Secrets oder sicherheitsrelevante Pfadinformationen offenlegen; interne Diagnose bleibt getrennt und berechtigungsbegrenzt. / *User and API error output MUST NOT expose internal exceptions, stack traces, connection strings, secrets, or security-relevant paths; internal diagnostics remain separate and access-limited.*
- **FR-009**: Eingaben an HTTP-, Import-, Datei-, Konfigurations-, Prozess- und Datenbankgrenzen MÜSSEN vor Nutzung auf Format, Bereich, Länge und zulässige Werte geprüft werden. / *Input at HTTP, import, file, configuration, process, and database boundaries MUST be checked for format, range, length, and allowed values before use.*
- **FR-010**: Datenbankzugriffe MÜSSEN für SQLite, MongoDB und PostgreSQL parametrisierte oder gleichwertig sichere Abfragen, begrenzte Identifikatoren, rollenbezogene Minimalrechte und belegte Fehlerpfade verwenden. / *Database access for SQLite, MongoDB, and PostgreSQL MUST use parameterized or equivalently safe queries, constrained identifiers, role-specific least privilege, and evidenced error paths.*
- **FR-011**: Secrets und Connection Strings MÜSSEN außerhalb versionierter Konfiguration liegen, sicher zusammengesetzt werden und in UI, Logs, Exceptions, Tests und Artefakten redigiert bleiben. / *Secrets and connection strings MUST remain outside versioned configuration, be composed safely, and stay redacted in UI, logs, exceptions, tests, and artefacts.*
- **FR-012**: Die bestehende Serialisierungsregel MUSS vollständig erfüllt werden: produktive JSON-Verarbeitung nutzt den festgelegten sicheren Standard, und die abweichende Alt-Abhängigkeit wird aus allen Produktprojekten entfernt. / *The existing serialization rule MUST be fully met: production JSON processing uses the declared secure standard, and the divergent legacy dependency is removed from all product projects.*
- **FR-013**: Ausgehende HTTP- und Dienstverbindungen MÜSSEN erlaubte Ziele, sichere Transportannahmen, endliche Zeitgrenzen, Abbruch und SSRF-Risiken nachvollziehbar behandeln. / *Outbound HTTP and service connections MUST address allowed destinations, secure transport assumptions, finite time limits, cancellation, and SSRF risks in a traceable way.*
- **FR-014**: Restore, Build, Unit-, Integrations- und Security-Negativtests MÜSSEN zur .NET-10/C#-14-Registry passen und für betroffene Provider und Plattformen reproduzierbar sein; CI-Abweichungen werden behoben oder als blockierendes Risiko geführt. / *Restore, build, unit, integration, and negative security tests MUST match the .NET 10/C# 14 registry and be reproducible for affected providers and platforms; CI drift is remediated or recorded as a blocking risk.*
- **FR-015**: Der Abhängigkeitsnachweis MUSS direkte und transitive Pakete, bekannte CVEs, Lizenzen, Registry-Herkunft, Aktualität, Lock-/Wiederholbarkeit und risikobasierte Entscheidungen abdecken. / *Dependency evidence MUST cover direct and transitive packages, known CVEs, licences, registry provenance, currency, locking/reproducibility, and risk-based decisions.*
- **FR-016**: Jeder verteilbare Artefaktsatz MUSS eine eindeutig zugeordnete, maschinenlesbare SBOM besitzen; `docs/security/supply-chain-evidence.md` MUSS Format, Erzeugungszeitpunkt, Ablage und Verifikation nennen. / *Every distributable artefact set MUST have a uniquely associated machine-readable SBOM; the supply-chain evidence MUST state format, generation point, storage, and verification.*
- **FR-017**: Wenn eine bekannte Schwachstelle in einer ausgelieferten oder bewerteten Komponente erkannt wird, MUSS eine VEX-artige Disposition mit Status, Begründung, Evidenz, Owner und Neubewertungs-Trigger vorliegen. / *When a known vulnerability is found in a shipped or assessed component, a VEX-style disposition MUST record status, rationale, evidence, owner, and re-evaluation trigger.*
- **FR-018**: Für CI/CD-gebaute oder veröffentlichte Artefakte MUSS ein dokumentierter SLSA-Zielzustand mit nachweisbarer Herkunft und Integrität festgelegt werden; Abweichungen bleiben priorisierte Risiken. / *For CI/CD-built or published artefacts, a documented SLSA target with verifiable provenance and integrity MUST be defined; gaps remain prioritized risks.*
- **FR-019**: Die verteilten Worker-, Harvester-, Viewer- und Datenbankflüsse MÜSSEN nach NIST SP 800-207 auf Zero-Trust-Anwendbarkeit geprüft werden; Netzwerkstandort allein darf kein ausreichender Vertrauensnachweis sein. / *Distributed worker, harvester, viewer, and database flows MUST be reviewed for Zero Trust applicability under NIST SP 800-207; network location alone is not sufficient trust evidence.*
- **FR-020**: Der OWASP-SAMM-Nachweis MUSS einen aktuellen Reifegrad, priorisierte Verbesserungen, Owner und Wiederholungsrhythmus enthalten. / *OWASP SAMM evidence MUST contain a current maturity snapshot, prioritized improvements, owners, and review cadence.*
- **FR-021**: CRA-, NIS2-, DORA- und EU-AI-Act-Anwendbarkeit MUSS dokumentiert werden; Nichtanwendbarkeit braucht sachliche Annahmen und einen Trigger, und CRA-relevante Ergebnisse müssen Vulnerability- und Release-Nachweise beeinflussen. / *CRA, NIS2, DORA, and EU AI Act applicability MUST be documented; non-applicability needs factual assumptions and a trigger, and CRA-relevant outcomes must influence vulnerability and release evidence.*
- **FR-022**: Nutzer- und lernbezogene Inhalte MÜSSEN deutsch zuerst, englisch danach auf CEFR-B2-Niveau bereitstehen und Status, Abhängigkeiten, Entscheidungen sowie nächste Schritte ohne reine Farb-, Bild- oder Layoutinformation vermitteln. / *User- and learner-facing content MUST be German first and English second at CEFR B2 and convey status, dependencies, decisions, and next actions without relying only on colour, images, or layout.*
- **FR-023**: Wenn öffentliche API-Dokumentation oder DocFX-Inhalte geändert werden, MÜSSEN die generierten Inhalte textorientiert sowie mit WCAG-2.2-AA-orientierter automatisierter und manueller Evidenz geprüft werden; generierte Verzeichnisse bleiben untracked. / *When public API documentation or DocFX content changes, generated content MUST receive text-oriented and WCAG 2.2 AA-oriented automated and manual evidence; generated directories remain untracked.*
- **FR-024**: Änderungen an gemeinsamen Regeln MÜSSEN `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md` und `.github/agents/copilot-instructions.md` gemeinsam prüfen und betroffene Spec-Kit-Templates oder die Memory-Constitution synchron behandeln. / *Changes to shared rules MUST jointly review all five maintained agent surfaces and synchronize affected Spec Kit templates or the memory constitution.*
- **FR-025**: Der Lauf MUSS genau die in CR-013 festgelegte Dokumentationsauswirkung umsetzen und Navigation, Sprachpartner, Owner, Plattformnachweis, Distribution und Home-Sync-Entscheidung belegen. / *The run MUST implement exactly the documentation impact decision in CR-013 and evidence navigation, language partner, owner, platform proof, distribution, and Home-sync decision.*
- **FR-026**: Der Abschluss MUSS alle offenen Risiken, akzeptierten Restrisiken und Folgeaufgaben mit Priorität, Owner, Zieltermin und Trigger zusammenfassen; ein Gate darf nur bei vollständiger Evidenz als bestanden gelten. / *Closeout MUST summarize all open risks, accepted residual risks, and follow-ups with priority, owner, due date, and trigger; a gate may pass only with complete evidence.*
- **FR-027**: Falls der Plan entgegen IR-016 ein Skript einführt oder ändert, MUSS er vor Umsetzung Bash-/PowerShell-Parität, Manpage, deutsch-erste/englisch-zweite PowerShell-Hilfe, `--dry-run`/`-WhatIf` und einen zulässigen `Verb-Noun`-Namen ergänzen. / *If the plan introduces or changes a script despite IR-016, it MUST add all required cross-platform parity evidence before implementation.*
- **FR-028**: Nach der agentischen Repository-Änderung MUSS `docs/project-statistics.md` gemäß Projektmethodik aktualisiert werden, ohne den abschließenden `Gesamtstatistik`-Block zu verletzen. / *After the agent-driven repository change, project statistics MUST be updated according to repository methodology without violating the final overall-statistics block.*
- **FR-029**: Geänderte öffentliche APIs MÜSSEN vollständige deutsch-erste/englisch-zweite XML-Dokumentation und synchronisierte DocFX-Quellen besitzen. / *Changed public APIs MUST have complete German-first/English-second XML documentation and synchronized DocFX sources.*
- **FR-030**: Die Umsetzung MUSS die Feature-Identität, den verbindlichen Intake und die autonome Zustandsgrenze wahren; `autonomous-run-state.json` wird ausschließlich durch den äußeren Koordinator verändert. / *Implementation MUST preserve feature identity, the binding intake, and the autonomous state boundary; only the outer coordinator changes the autonomous run state.*
- **FR-031**: Anforderungen für Teilfehler und Wiederherstellung MÜSSEN für zustandsändernde Datenbank-, Import-, Datei-, Migrations- und Evidenzabläufe den sicheren Restzustand, Idempotenz oder Rollback, begrenzte Wiederholung, Bedienhinweis und erneute Gate-Prüfung festlegen; wenn kein Zustand verändert wird, MUSS Rollback mit Begründung als `N/A` geführt werden. / *Partial-failure and recovery requirements MUST define the safe residual state, idempotency or rollback, bounded retry, operator guidance, and renewed gate verification for state-changing database, import, file, migration, and evidence flows; where no state changes, rollback MUST be recorded as N/A with rationale.*
- **FR-032**: Der Lauf MUSS vor der Projektbewertung Baseline-Manifest, zwölf kanonische Checklisten, Versionsangaben und die 157 eindeutigen stabilen IDs auf gegenseitige Konsistenz prüfen; Drift wird als `Open` mit Owner, Risiko, Folgeschritt und Zieltermin erfasst und darf nicht durch die Wahl der günstigeren Quelle verdeckt werden. / *Before project assessment, the run MUST check the baseline manifest, twelve canonical checklists, version declarations, and 157 unique stable IDs for mutual consistency; drift is recorded as Open with owner, risk, next action, and due date and MUST NOT be hidden by selecting the more favourable source.*

### Constitution Requirements *(mandatory)*

- **CR-001 — Level-2 Registry**: Bindend ist der Eintrag `RiderProjects/InventarWorkerService`: .NET 10/C# 14, Multi-Projekt-Lösung, `dotnet restore/build/test`, MSTest und bedarfsweise Playwright; DocFX und Lerninhalte benötigen textorientierte A11Y-Evidenz. / *The matching Level-2 registry entry is binding.*
- **CR-002 — A11Y**: WCAG 2.2 Level AA gilt für betroffene Dokumentation und HTML; TUI, CLI, API-JSON und Statusausgaben benötigen gleichwertige textbasierte Zustände und verständliche Fehler. Evidenzpfad: `docs/accessibility/secure-development-hardening.md`. / *WCAG 2.2 Level AA applies to affected documentation and HTML, with equivalent text-first treatment for terminal and API output.*
- **CR-003 — Sprache und Lernniveau**: Governance- und Lerntexte sind deutsch zuerst und englisch danach, CEFR B2, mit erklärten Fachbegriffen und ohne vorausgesetzte Spec-Kit-Erfahrung. Große normative Dokumente dürfen einen synchronen `.EN.md`-Partner nutzen; Standard bleibt inline zweisprachig. / *Governance and learner prose is German first, English second, CEFR B2, and suitable for first-time Spec Kit users.*
- **CR-004 — Statistik und Agentenflächen**: `docs/project-statistics.md` benötigt ein Update. Die fünf gemeinsamen Agentenflächen und betroffene Spec-Kit-Surfaces benötigen einen gemeinsamen Drift-Review; nur tatsächlich betroffene Inhalte werden geändert. / *Project statistics require an update, and all maintained agent and affected Spec Kit surfaces require a joint drift review.*
- **CR-005 — MSL**: Primärsprache ist C# auf .NET 10. C# steht auf der MSL-Allowlist; Hardware oder Laufzeit erzwingen keine nicht speichersichere Sprache. Sichere C#/.NET-, SQL- und Skriptregeln bleiben trotzdem verbindlich. Evidenz: `docs/security/msl-applicability.md`. / *C# on .NET 10 is memory-safe; this does not replace secure API, I/O, SQL, logging, configuration, or dependency review.*
- **CR-006 — Standards**: NIST SSDF und CWE Top 25 sind immer `Applicable`. Ebenfalls `Applicable` sind OWASP ASVS Level 2, SBOM, SLSA, bedingtes VEX, Zero Trust, CAPEC, SAMM, OpenSSF Scorecard, OWASP Cheat Sheets/Proactive Controls, iSAQB/arc42, CRA-Screening und WCAG 2.2 AA. N/A-Entscheidungen stehen in der Standardsmatrix. / *The named standards are explicitly applicable, with conditional and N/A decisions recorded below.*
- **CR-007 — ASVS**: OWASP ASVS Level 2 deckt alle HTTP-Endpunkte des InventarWorkerService und der Viewer-API einschließlich Zugriffskontrolle, Transport, Validierung, Fehler, Logging, Konfiguration, Swagger/OpenAPI und statische Dokumentationspfade ab. Evidenz: `docs/security/asvs-verification.md`. / *ASVS Level 2 covers both complete HTTP/API surfaces.*
- **CR-008 — Supply Chain**: SBOM und SLSA gelten für verteilbare Artefakte. VEX gilt, sobald eine bekannte Schwachstelle disponiert werden muss. Evidenz: `docs/security/dependency-audit.md` und `docs/security/supply-chain-evidence.md`; SBOM/VEX werden als eindeutig zugeordnete Release- oder CI-Evidenz referenziert. / *SBOM and SLSA apply; VEX is triggered by a known vulnerability requiring disposition.*
- **CR-009 — AI Classification**: KI ist ausschließlich Entwicklungswerkzeug und nicht Teil des ausgelieferten oder betriebenen Systems. `AI-SBOM`: `N/A`. Trigger ist jede spätere KI-Runtime-, Modell-, Daten-, Inferenz- oder Produktkomponente. / *AI is development tooling only, so AI-SBOM is N/A with a clear re-evaluation trigger.*
- **CR-010 — Trust Boundaries**: STRIDE und CIA sind die Basis; CAPEC wird für die höchsten Risiken an HTTP-, Datenbank-, Datei-, Konfigurations-, Prozess-, CI- und Agentengrenzen verwendet. Zero Trust wird für die verteilten Dienste ausdrücklich bewertet. / *STRIDE/CIA, CAPEC, and Zero Trust apply to the listed trust boundaries.*
- **CR-011 — Evidence Location**: Standardpfad bleibt `docs/security/`; erforderlich sind mindestens `threat-model.md`, `security-checklist.md`, `arc42-security.md`, `dependency-audit.md`, `security-quality-scenarios.md`, `asvs-verification.md`, `supply-chain-evidence.md`, `zero-trust-applicability.md`, `samm-assessment.md`, `msl-applicability.md`, `secure-coding-language-rules.md`, `cra-applicability.md` und `regulatory-applicability.md` sowie bedarfsweise S-ADRs unter `docs/security/adr/`. / *The canonical security evidence path and required files are explicit.*
- **CR-012 — Installed Governance**: Der aktuelle installierte Stack aus zwölf Presets ist maßgeblich. Die im Intake genannte ältere Sechser-Teilmenge bleibt historischer Scope-Hinweis und begrenzt den heutigen Stack nicht. Zusätzliche Prozess-Presets erweitern keine Produktfunktion. / *The installed twelve-preset stack is authoritative; the intake's older six-preset subset remains historical context and does not narrow current governance or expand product scope.*
- **CR-013 — Documentation Impact**: `UpdateRequired`. Betroffene Zielgruppen sind Auszubildende, Entwickler:innen, Security-/Architektur-/A11Y-Reviewer, Operatoren, Release-Verantwortliche und KI-Agenten. Dokumentfamilien sind `docs/security/`, `docs/architecture/`, `docs/accessibility/`, Secure-Development-Projektinstanzen, Agenten-Guidance, DocFX/API-Dokumentation bei Signaturänderung und `docs/project-statistics.md`. Leserpfade führen von `docs/security/README.md`, `docs/secure-development/README.md` und dieser Spec zu den Nachweisen. Kanonische Quellen sind Constitution und Secure-Development-Baseline; Owner ist die Projektverantwortung, fachlicher Reviewer die Security-Rolle. Navigation wird in den betroffenen Indizes und bei neuen veröffentlichten Seiten in DocFX aktualisiert. Dokumentklasse: Governance-, Sicherheits-, Architektur-, A11Y-, Betriebs- und Release-Evidenz. Sprachstrategie: deutsch zuerst, englisch danach im selben Dokument; `.EN.md` nur für begründete große normative Partner. Plattformbeweis: textbasierte Markdown-Prüfung sowie bei DocFX Axe, Lynx und unterstützte Plattformtests. Distribution: versionierte Quellen im Repository, generierte HTML-/SBOM-/Scanartefakte nur über dokumentierte CI-/Release-Pfade. Home-Sync: in diesem Feature `No`; ein nachgewiesener zentraler Matrixfehler wird als separates Follow-up an die Home-Baseline übergeben. Evidenz: geänderte Indizes, Checklisten, Test-/A11Y-Berichte und Abschlussübersicht. Neubewertungs-Trigger: neue öffentliche API, neues Dokumentformat, neue Plattform, Preset- oder Constitution-Drift, Cloud-/KI-Runtime-Einführung oder geänderte Distribution. / *The single documentation-impact decision is UpdateRequired with audiences, families, reader paths, sources, ownership, navigation, class, language partner, platform proof, distribution, Home-sync decision, evidence, and triggers fully declared.*

### Key Entities

- **Prüfpunkt / Assessment Checkpoint**: Eine stabile CL-ID mit Anwendbarkeit, Umsetzungsstatus, Lernstufe, Rollen, Begründung, Evidenz, Restrisiko, Trigger und nächster Maßnahme. / *A stable CL ID with all audit fields.*
- **Befund / Finding**: Eine belegte Lücke oder Abweichung mit Schwere, betroffener Grenze, Risiko, Owner, Priorität, Zieltermin und Status. / *An evidenced gap with severity, affected boundary, risk, owner, priority, due date, and status.*
- **Evidenzdatensatz / Evidence Record**: Ein aktueller, reproduzierbarer Nachweis mit Quelle, Scope, Zeitpunkt, Prüfer:in, Ergebnis und Integritätsbezug. / *A current, reproducible proof with source, scope, time, reviewer, outcome, and integrity reference.*
- **Trust Boundary / Vertrauensgrenze**: Übergang zwischen Akteuren, Diensten, Prozessen, Dateien, Konfigurationen, Datenbanken, CI oder externen Quellen, an dem Eingaben und Berechtigungen neu bewertet werden. / *A boundary where input and authority must be re-evaluated.*
- **Release-Artefaktsatz / Release Artefact Set**: Zusammengehörige verteilbare Dateien mit Version, SBOM, Herkunft, Schwachstellenstatus und Freigabeentscheidung. / *A set of distributable files with version, SBOM, provenance, vulnerability status, and release decision.*
- **Risiko- und Folgeeintrag / Risk and Follow-up Entry**: Offener oder akzeptierter Punkt mit Auswirkung, Wahrscheinlichkeit, Maßnahme, Owner, Zieltermin und Neubewertungs-Trigger. / *An open or accepted item with impact, likelihood, action, owner, due date, and trigger.*

## Test- und Evidenzstrategie / Test and Evidence Strategy

1. **Baseline- und Traceability-Prüfung**: Manifest, zwölf Einzelchecklisten und 157 IDs werden auf Vollständigkeit und eindeutige Zuordnung geprüft. / *Verify baseline, all checklists, and all stable IDs for completeness and traceability.*
2. **Sicherheitsreview**: Code-, Konfigurations-, Datenfluss- und Architekturprüfung deckt NIST SSDF, CWE Top 25, ASVS Level 2, STRIDE/CIA, CAPEC und Zero Trust ab. / *Review code, configuration, flows, and architecture against the declared standards.*
3. **Negative Tests**: Unberechtigter Zugriff, fehlerhafte Eingaben, Grenzwerte, Ausnahmen, Secret-Redaktion, nicht erreichbare Ziele, SSRF-nahe Ziele und Providerfehler werden reproduzierbar geprüft. / *Run reproducible negative cases for access, input, errors, redaction, outbound targets, and provider failures.*
4. **Build- und Regressionsevidenz**: Registry-konforme Wiederherstellung, Kompilierung, Unit- und Integrationstests sowie Coverage-Gates werden ausgeführt; die drei Datenbankprovider werden nur mit belegter Reichweite gleichgestellt. / *Run registry-aligned restore, build, tests, coverage, and provider-scoped evidence.*
5. **Supply Chain**: Abhängigkeits- und CVE-Prüfung, SBOM-Validierung, SLSA-Herkunft und bedingte VEX-Disposition werden einem Release-Kandidaten zugeordnet. / *Associate dependency, CVE, SBOM, provenance, and conditional VEX evidence with a release candidate.*
6. **A11Y und Dokumentation**: Textprüfung, Link- und Sprachprüfung sowie bei DocFX Axe- und Lynx-Evidenz werden erzeugt; Inhalte bleiben ohne Farbe oder Diagramm verständlich. / *Produce text, link, language, Axe, and Lynx evidence where applicable.*
7. **Governance-Parität**: Installierter Preset-Stack, Constitution, Memory-Constitution, Agentenflächen und betroffene Templates werden auf widerspruchsfreie aktuelle Aussagen geprüft. / *Review installed presets and all maintained governance surfaces for current, consistent statements.*
8. **Abschlussgate**: Kein Pflichtbefund bleibt ohne Status, Owner, Risiko, Evidenz oder Folgeschritt; Build/Test/A11Y/Security-Gates müssen ihren jeweiligen Scope bestehen. / *No mandatory finding remains without complete fields, and all scoped gates must pass.*

## Standards- und Evidenzmatrix / Standards and Evidence Matrix

| Standard oder Checkpoint | Status | Geplanter Evidenzpfad und Trigger / Planned evidence path and trigger |
|---|---|---|
| NIST SSDF SP 800-218 | Applicable | `spec.md`, `plan.md`, `tasks.md`, `docs/security/security-checklist.md`; PO/PS/PW/RV-Abdeckung. / PO, PS, PW, and RV coverage. |
| CWE Top 25 | Applicable | `docs/security/security-checklist.md`, Tests und Befundregister; relevante Schwächen je Grenze. / Relevant weaknesses per boundary. |
| OWASP ASVS Level 2 | Applicable | `docs/security/asvs-verification.md`; vollständiger HTTP/API-Scope beider Hosts. / Complete HTTP/API scope of both hosts. |
| SBOM | Applicable | `docs/security/supply-chain-evidence.md` plus maschinenlesbares CI-/Release-Artefakt je verteilbarem Satz. / Machine-readable CI/release artefact per distributable set. |
| SLSA | Applicable | `docs/security/supply-chain-evidence.md`; Zielniveau und Provenance-Lücken. / Target level and provenance gaps. |
| VEX | Applicable, conditional | Gleicher Supply-Chain-Pfad; Trigger ist ein bekannter Fund in ausgelieferter oder bewerteter Komponente. / Triggered by a known finding in a shipped or assessed component. |
| AI-SBOM | N/A | Entwicklungswerkzeug-only; Trigger siehe CR-009. / Development-tool-only; trigger in CR-009. |
| STRIDE und CIA | Applicable | `docs/security/threat-model.md`. |
| CAPEC | Applicable | `docs/security/threat-model.md` für die höchsten Angriffswege. / For the highest-risk attack paths. |
| NIST Zero Trust SP 800-207 | Applicable | `docs/security/zero-trust-applicability.md`; verteilte Dienste und Datenbanken. / Distributed services and databases. |
| OWASP SAMM | Applicable | `docs/security/samm-assessment.md`; Reifegrad und Verbesserungsbacklog. / Maturity and improvement backlog. |
| OpenSSF Scorecard | Applicable | `docs/security/dependency-audit.md` und `supply-chain-evidence.md`; öffentliches Repository beziehungsweise wichtige externe Abhängigkeiten. / Public repository and/or high-impact external dependencies. |
| OWASP Cheat Sheets / Proactive Controls | Applicable | Referenz in Secure-Coding- und ASVS-Evidenz. / Referenced by secure-coding and ASVS evidence. |
| CRA | Applicable screening | `docs/security/cra-applicability.md`; formale Markt-/Distributionsentscheidung durch Owner vor Release. / Formal market/distribution decision by the owner before release. |
| NIS2 | N/A on current assumptions | `docs/security/regulatory-applicability.md`; Trigger: Betrieb als wesentliche/wichtige Einrichtung oder regulierte Lieferkette. / Trigger: operation as an essential/important entity or regulated supply chain. |
| DORA | N/A on current assumptions | Gleicher Pfad; Trigger: Finanzunternehmen oder dessen kritischer ICT-Dienstleister. / Trigger: financial entity or its critical ICT provider. |
| EU AI Act | N/A | Keine KI-Runtime-/Produktkomponente; Trigger siehe CR-009. / No AI runtime/product component. |
| BSI C3A / BSI C5 | N/A | Cloud-Nachweise dokumentieren die Nichtanwendbarkeit; Trigger siehe IR-015. / Cloud evidence records non-applicability. |
| iSAQB / arc42 | Applicable | `docs/architecture/` für Kontext, Laufzeit, Deployment, Risiken und Qualitätsszenarien. / Context, runtime, deployment, risks, and quality scenarios. |
| WCAG 2.2 Level AA | Applicable | `docs/accessibility/secure-development-hardening.md` und bedarfsweise DocFX-Testevidenz. / Accessibility evidence and DocFX tests where needed. |
| Datenschutz-/DPIA-Screening | Applicable | Ausgefüllte CL-11-Projektinstanz; Ergebnis kann begründet `N/A` sein. / Completed CL-11 project instance; outcome may be reasoned N/A. |

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100 % der 157 stabilen CL-IDs besitzen genau eine vollständige, zulässige und nachvollziehbare Projektbewertung; kein Feld mit Pflichtcharakter bleibt leer. / *All 157 stable CL IDs have exactly one complete, valid, traceable project assessment with no mandatory field empty.*
- **SC-002**: 100 % der positiven Sicherheits- und Compliance-Aussagen verweisen auf aktuelle Evidenz; 0 Aussagen stützen sich ausschließlich auf Stubs oder Dateiexistenz. / *All positive security and compliance statements reference current evidence; none relies only on stubs or file existence.*
- **SC-003**: Alle identifizierten Trust Boundaries und Assets sind im Bedrohungsmodell erfasst; jeder hohe Risiko-Pfad besitzt mindestens zwei unabhängige Schutzebenen oder eine ausdrücklich akzeptierte Restrisikoentscheidung. / *All identified trust boundaries and assets are modelled; every high-risk path has at least two independent controls or an explicit residual-risk acceptance.*
- **SC-004**: Der definierte OWASP-ASVS-Level-2-Scope beider HTTP/API-Flächen ist zu 100 % bewertet; jeder nicht erfüllte Kontrollpunkt hat Owner, Priorität, Risiko und Zieltermin. / *The complete ASVS Level 2 scope for both HTTP/API surfaces is assessed, and every unmet control has owner, priority, risk, and due date.*
- **SC-005**: Repräsentative Negativtests liefern in 100 % der geprüften Fehlerfälle keine Secrets, Connection Strings, Stack-Traces oder ungefilterten internen Ausnahmeinformationen an Nutzer oder API-Clients. / *Representative negative tests expose no secrets, connection strings, stack traces, or unfiltered internal exception details.*
- **SC-006**: Jeder geprüfte Release-Artefaktsatz besitzt eine validierbare SBOM und Herkunftsevidenz; jeder bekannte Schwachstellenfund besitzt vor Freigabe eine VEX-artige Disposition. / *Every reviewed release artefact set has a valid SBOM and provenance evidence; every known vulnerability has a VEX-style disposition before release.*
- **SC-007**: Alle Registry-konformen Build-, Unit-, Integrations- und relevanten A11Y-Gates bestehen; die Testabdeckung bleibt mindestens 70 % und zielt auf mindestens 80 %. / *All registry-aligned build, unit, integration, and relevant accessibility gates pass; coverage remains at least 70% and targets at least 80%.*
- **SC-008**: 100 % der betroffenen Governance- und Lerndokumente sind deutsch zuerst und englisch danach auf CEFR-B2-Niveau; alle Status- und Entscheidungsinformationen bleiben textuell verständlich. / *All affected governance and learner documents are German first and English second at CEFR B2, and all status and decision information remains understandable as text.*
- **SC-009**: Der installierte Zwölfer-Preset-Stack, die lokale Constitution, die Memory-Constitution, alle fünf Agentenflächen und betroffene Templates enthalten nach dem Drift-Review keine widersprüchlichen maßgeblichen Aussagen. / *After drift review, the installed twelve-preset stack and all maintained local governance surfaces contain no conflicting authoritative statements.*
- **SC-010**: Die Abschlussübersicht enthält für 100 % der offenen und akzeptierten Risiken Owner, Priorität, Entscheidung, Zieltermin und Neubewertungs-Trigger; kein Pflichtgate wird ohne vollständige Evidenz als bestanden gemeldet. / *The closeout summary gives complete ownership and decision fields for every open or accepted risk, and no mandatory gate passes without complete evidence.*
- **SC-011**: 100 % der betrachteten zustandsändernden Teilfehler besitzen eine eindeutige Recovery-Entscheidung mit sicherem Restzustand und erneuter Gate-Prüfung; jede nicht anwendbare Rollback-Anforderung ist begründet. / *Every assessed state-changing partial failure has an unambiguous recovery decision with a safe residual state and renewed gate verification; every non-applicable rollback requirement has a rationale.*
- **SC-012**: Baseline-Manifest und zwölf kanonische Checklisten weisen vor der Projektbewertung genau 157 eindeutige stabile IDs und widerspruchsfreie Versionsangaben aus, oder jede Abweichung bleibt als vollständiger `Open`-Befund plan- und abschlussblockierend. / *Before project assessment, the baseline manifest and twelve canonical checklists expose exactly 157 unique stable IDs with consistent version declarations, or every discrepancy remains a complete Open finding that blocks planning and closeout.*

## Assumptions

- Der geprüfte Intake bleibt die einzige fachliche Quelle dieses Features; andere Lastenhefte werden nicht kombiniert oder gestartet. / *The reviewed intake remains the sole business input for this feature; no other intake is combined or started.*
- Das Repository bleibt ein Level-2-Projekt mit .NET 10/C# 14 und den im Registry-Eintrag genannten Projekten, Datenbanken und Testflächen. / *The repository remains a Level-2 .NET 10/C# 14 project with the registered components and test surfaces.*
- Inventar- und Systemdaten werden mindestens als intern, Zugangsdaten als vertraulich behandelt; eine strengere Klassifikation aus dem Bedrohungsmodell hat Vorrang. / *Inventory and system data are at least internal, and credentials are confidential; stricter threat-model classification takes precedence.*
- Releasefähige oder verteilbare Artefakte entstehen weiterhin über die vorhandene Build-/Release-Kette. / *Release-capable or distributable artefacts continue to be produced by the existing build/release chain.*
- Regulatorische Entscheidungen sind technische Anwendbarkeits-Screenings und keine Rechtsberatung. / *Regulatory decisions are technical applicability screenings, not legal advice.*
- Bestehende Nutzerfunktionen und unterstützte Plattformen bleiben erhalten, soweit ein ausdrücklich dokumentiertes Sicherheitsrisiko keine eng begrenzte Änderung verlangt. / *Existing user capabilities and supported platforms remain unless an explicitly documented security risk requires a narrowly scoped change.*

## Abhängigkeiten, Risiken und Follow-ups / Dependencies, Risks, and Follow-ups

### Dependencies

- Aktuelle Constitution und `.specify/memory/constitution.md`, Level-2-Registry sowie der installierte Preset-Stack.
- `docs/secure-development/baseline-manifest.json`, Richtlinie, Sammelband und CL-01 bis CL-12.
- Verfügbare .NET-, MSTest-, Playwright-, DocFX-, Axe-, Lynx-, Dependency-, SBOM- und Secret-Scan-Werkzeuge gemäß Repository-Governance.
- Zugängliche Testumgebungen für die tatsächlich geprüften Datenbank- und HTTP-Flächen; fehlende externe Umgebungen werden nicht als positives Ergebnis gewertet.

*Dependencies are the current governance sources, canonical secure-development baseline, governed verification tools, and accessible test environments for each claimed scope.*

### Risks

- **R-001 Hoch / High**: Offene oder weit erreichbare HTTP-Flächen können Inventardaten offenlegen. Owner: Projektverantwortung; Trigger: ASVS-/Threat-Model-Befund; Maßnahme: fail-safe Zugriffsschutz oder dokumentierte enge Ausnahme.
- **R-002 Hoch / High**: Interne Exception-Texte oder Connection Strings können Geheimnisse und Strukturinformationen offenlegen. Owner: Entwicklung; Trigger: Negativtest; Maßnahme: getrennte interne Diagnose und generische Außenantwort.
- **R-003 Hoch / High**: Unvollständige project-specific Stubs können fälschlich als Compliance gelten. Owner: Security-Reviewer; Trigger: Nachweis ohne Prüfung; Maßnahme: vollständige Evidenzinstanzen und ehrlicher Indexstatus.
- **R-004 Mittel / Medium**: Härtungen können Provider- oder Plattformparität brechen. Owner: Entwicklung/Test; Trigger: abweichender Test; Maßnahme: scoped parity evidence and regression correction.
- **R-005 Mittel / Medium**: Abhängigkeits- oder CI-Drift kann Build, SBOM oder Provenance unzuverlässig machen. Owner: Release-Verantwortung; Trigger: Audit-/Build-Abweichung; Maßnahme: priorisierte Korrektur oder blockierte Freigabe.
- **R-006 Mittel / Medium**: Umfangreiche Evidenz kann für Lernende unübersichtlich werden. Owner: Dokumentationsverantwortung; Trigger: A11Y-/B2-Review; Maßnahme: text-first Indizes, klare Begriffe und kurze DE/EN-Erklärungen.

*The principal risks are exposed HTTP data, information leakage, false compliance from stubs, provider/platform regressions, supply-chain drift, and inaccessible evidence. Each has a named owner, trigger, and control.*

### Follow-ups

- Wiederkehrender monatlicher Dependency-Audit und Release-Audit nach Abschluss dieses Features.
- Externe Penetrationstest- oder Zertifizierungsentscheidung nach Bedrohungsmodell und Restrisikobewertung.
- Separater Home-Baseline-Follow-up nur, wenn der lokale Drift-Review einen Fehler in der zentralen Governance-Quelle belegt.
- Neubewertung von AI-SBOM, BSI C3A/C5, NIS2, DORA oder EU AI Act ausschließlich bei ihrem dokumentierten Trigger.

*Recurring audits, possible external assessment, evidenced Home-baseline corrections, and conditional standard re-evaluations remain explicit follow-ups outside the current implementation scope.*

## Autonomous-run Applicability

- **Delivery mode and authority**: Der feature-lokale Run-State deklariert `MergeAndSync`; diese Specify-Phase besitzt jedoch nur Autorität für lokale Spezifikationsartefakte. Commit, Push, PR, Merge, Closeout und Zustandsübergänge bleiben beim äußeren Koordinator. / *The feature-local run state declares MergeAndSync, while this specify phase is authorized only for local specification artefacts; all delivery and state transitions remain coordinator-owned.*
- **Identity and accepted input**: Feature `002-secure-development-hardening`, Branch `002-secure-development-hardening`, verbindlicher Intake `Lastenheft_Secure-Development-Hardening.md`, Run-State `specs/002-secure-development-hardening/autonomous-run-state.json`. / *The feature, branch, intake, and state paths are fixed.*
- **Autonomy boundary**: Keine zusätzliche Fachfunktion, kein weiteres Lastenheft, keine Remote-Aktion, kein Secret-/Provider-Adminzugriff und keine Änderung des Run-State durch die Phase. / *Autonomy cannot expand scope or authority.*
- **Causal closeout**: Applicable für den Gesamtlauf, weil Härtungsbefunde, Evidenz, ggf. Codeänderungen und Delivery kausal verbunden werden müssen; Durchführung ausschließlich durch den Koordinator. / *Causal closeout applies to the full run and remains coordinator-owned.*
- **Mutable validation tokens**: `N/A`; dieses Feature definiert keine veränderlichen Provider-, Review- oder Secret-Tokens. Trigger: Der Koordinator führt später einen externen, mutablen Gate-Token ein. / *No mutable validation tokens are defined.*
- **Retrospective boundary**: Wiederverwendbare Lernpunkte dürfen nur nach belegtem Abschluss extrahiert werden; projektspezifische Risiken und sensible Konfiguration bleiben lokal. / *Portable learning is extracted only after evidenced completion; project-specific risk and sensitive configuration remain local.*
- **Stop and recovery**: Der feature-lokale Run-State ist die Quelle für kooperativen Stopp. Nach Unterbrechung oder Drift sind Branch, Intake, Hashes, Autorität, Artefakte und Gates vollständig neu zu validieren; expliziter Pause-Status benötigt Resume. / *The feature-local state governs cooperative stop and full recovery revalidation.*

| Gate ID | Status | Scope und Befehlstoken / Scope and command token | Bestehensbedingung und Trigger / Pass condition and trigger |
|---|---|---|---|
| G-SPEC-001 | Applicable | `speckit.specify`; `spec.md` und `checklists/requirements.md` | Beide vollständig, keine Klärungsmarker, Qualitätscheck bestanden. / Both complete, no clarification markers, checklist passed. |
| G-TRACE-001 | Applicable | Intake-, Branch-, Preset- und Registry-Abgleich / Intake, branch, preset, and registry review | Scope, Klassifikation und heutige Governance sind nachvollziehbar. / Scope, classification, and current governance are traceable. |
| G-SEC-001 | Applicable | Nachgelagerter Plan/Tasks/Implement-Lauf; Security-Evidenz / Downstream plan, tasks, implementation, and security evidence | Alle anwendbaren Security-Anforderungen besitzen vollständige Evidenz. / All applicable security requirements have complete evidence. |
| G-BUILD-001 | Applicable | Registry-konforme Restore-/Build-/Test-Tokens / Registry-aligned restore, build, and test tokens | Alle betroffenen Gates bestehen; Trigger sind Code-, Paket-, Build- oder Teständerungen. / All affected gates pass. |
| G-A11Y-001 | Applicable | Text-/DocFX-/A11Y-Prüftokens / Text, DocFX, and accessibility review tokens | Betroffene Artefakte bestehen ihren erklärten Scope. / Affected artefacts pass their declared scope. |
| G-SUPPLY-001 | Applicable | Dependency-, SBOM-, Provenance- und bedingte VEX-Prüfung / Dependency, SBOM, provenance, and conditional VEX review | Release-Evidenz ist vollständig oder die Freigabe bleibt blockiert. / Release evidence is complete or release remains blocked. |
| G-CLOSE-001 | Applicable, coordinator-owned | Autonomer Evidence-/Closeout-Validator / Autonomous evidence and closeout validator | Erst nach vollständigen Tasks, Gates, kausaler Evidenz und autorisiertem Delivery-Abschluss. / Only after complete tasks, gates, causal evidence, and authorized delivery closeout. |

## Agent Parity Applicability

`Applicable`: Das Feature prüft Drift in gemeinsamen Regeln, Preset-Versionen und Spec-Kit-Governance. Gemeinsam zu prüfen sind `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`, `.github/copilot-instructions.md`, `.github/agents/copilot-instructions.md`, `.specify/memory/constitution.md` und betroffene Dateien unter `.specify/templates/`. Provider- oder modellspezifische Laufzeitwerte werden nicht in Anforderungen geschrieben. Eine absichtliche Abweichung ist derzeit nicht vorgesehen; jede spätere Abweichung benötigt Begründung im selben Change.

*Applicable: shared rules, preset versions, and Spec Kit governance require a joint drift review across all maintained agent, memory, and affected template surfaces. No provider-specific runtime values or intentional divergence are planned.*

## Cross-Platform Applicability

`N/A` gemäß IR-016: Dieses Feature verlangt derzeit kein neues oder geändertes skriptförmiges Werkzeug. Zielplattformen der Produktprüfung bleiben macOS, Linux und Windows. Geplanter Bash-Name, PowerShell-`Verb-Noun`-Name, Manpage, Hilfe und Dry-Run-Parität sind deshalb `N/A`. Neubewertung vor Planfreigabe, sobald ein Skript als Umsetzungsweg gewählt wird; dann gilt FR-027 vollständig.

*N/A under IR-016: no script-shaped tool is currently required. Product review still covers macOS, Linux, and Windows. All script-specific names and parity artefacts become mandatory if the plan selects a script.*

## Accessibility Applicability

`Applicable`: Betroffen sind Governance- und Lerndokumentation, Sicherheits- und Architekturindizes, mögliche TUI-/CLI-/API-Fehlerausgaben, Status-JSON, Changelog und bei öffentlichen API-Änderungen DocFX-HTML. WCAG 2.2 Level AA gilt für HTML und anwendbare Dokumentkriterien; Terminal- und JSON-Ausgaben benötigen klare Textparität. Zielgruppe umfasst Erstnutzer:innen von Spec Kit und Auszubildende ohne Security-Spezialwissen. Deutsch-erste/englisch-zweite CEFR-B2-Texte, beschreibende Links, Sprachkennzeichnung für Codeblöcke, Textalternativen und statusunabhängige Farbcodierung sind Pflicht. Neue oder geänderte nicht triviale Logik benötigt einen moderaten Review didaktischer zweisprachiger Kommentare; ohne Logikänderung ist dieser Teil `N/A`. Evidenz: `docs/accessibility/secure-development-hardening.md`.

*Applicable: all listed user-facing artefacts require German-first/English-second CEFR B2 delivery, text-first state, accessible links and alternatives, applicable WCAG 2.2 AA evidence, and a didactic-comment review when non-trivial logic changes.*

## Architecture Applicability

`Applicable`: Das Feature betrifft Systemkontext, HTTP- und Datenbankschnittstellen, Laufzeitflüsse, Deployment-Sicherheit, Qualitätsattribute und vorhandene technische Security-Schulden. Architekturziele sind sichere Standardwerte, minimale Rechte, Defense in Depth, nachvollziehbare Fehler, Verfügbarkeit bei Teilfehlern, Testbarkeit, Plattformparität und wartbare Evidenz. Erwartet werden aktualisierte Kontext-, Laufzeit-, Deployment-, Risiko- und Qualitätsszenario-Nachweise unter `docs/architecture/`. Ein allgemeiner ADR ist `N/A`, solange Entscheidungen ausschließlich sicherheitsspezifisch sind; sicherheitsrelevante Entscheidungen erhalten S-ADRs unter `docs/security/adr/`. Trigger für einen allgemeinen ADR ist eine strukturelle, nicht nur sicherheitsbezogene Änderung an Komponenten, Schnittstellen oder Deployment.

*Applicable: context, interfaces, runtime flows, deployment security, quality attributes, and technical debt are affected. General architecture evidence is required; general ADRs are N/A unless a non-security structural decision emerges, while security decisions use S-ADRs.*

## Architecture Governance Applicability

- **MSL constraints**: Keine Hardware- oder Plattformgrenze erzwingt eine nicht speichersichere Sprache; C#/.NET bleibt geeignet. / *No runtime or hardware constraint displaces C#/.NET as the memory-safe primary language.*
- **Trust boundaries**: API-Client zu Worker-API; Client zu Viewer-API; Harvester zu Worker; Dienste zu SQLite/MongoDB/PostgreSQL; CSV/Datei und Konfiguration zu Prozess; Betriebssysteminventar zu Dienst; CI zu Registry/Release; Agent/Runner zu Repository und Run-State. / *All named client, service, data, file, OS, CI, and agent boundaries are in scope.*
- **Data classes**: Öffentliche Dokumentation; interne Inventar-, Status- und Logdaten; vertrauliche Zugangsdaten und Connection Strings; strengere Einstufungen aus der CIA-Bewertung gelten vorrangig. / *Public documentation, internal inventory/status/log data, and confidential credentials are distinguished.*
- **Threat model**: STRIDE+CIA und CAPEC sind `Applicable`; Pfad `docs/security/threat-model.md`. / *STRIDE+CIA and CAPEC apply.*
- **S-ADR and arc42 Section 8**: `Applicable` für Zugriffsschutz, Transport, Secret-Behandlung oder andere signifikante Security-Entscheidungen; Pfade `docs/security/adr/` und `docs/security/arc42-security.md`. / *Applicable for significant security decisions and cross-cutting concepts.*
- **Security quality scenarios**: `Applicable`; Pfad `docs/security/security-quality-scenarios.md`. / *Applicable.*
- **Zero Trust and SAMM**: Beide `Applicable`; Pfade `docs/security/zero-trust-applicability.md` und `docs/security/samm-assessment.md`. / *Both apply.*
- **BSI C3A/C5**: `N/A` mit Trigger aus IR-015; Nichtanwendbarkeit wird in den vorgesehenen Cloud-Nachweisen oder im Regulierungsindex dokumentiert. / *N/A with the cloud trigger in IR-015.*

## Security Governance Applicability

Primärsprache ist die speichersichere Sprache C# auf .NET 10. Die Prüfung wendet Microsoft Secure Coding Guidelines, parametrisierte SQL-Zugriffe, sichere Deserialisierung, Output-Encoding, Zugriffskontrolle, Zeitgrenzen und SSRF-Prüfung an. NIST SSDF und CWE Top 25 sind immer anwendbar. OWASP ASVS Level 2 gilt für den HTTP/API-Scope. SBOM und SLSA gelten für verteilbare beziehungsweise veröffentlichte Artefakte. VEX gilt bei einem bekannten Fund. Zero Trust, CAPEC, SAMM und OpenSSF gelten nach der Standardsmatrix. AI-SBOM, BSI C3A/C5, NIS2, DORA und EU AI Act sind unter den dokumentierten aktuellen Annahmen `N/A`; CRA wird ausdrücklich geprüft. Erforderliche Evidenz umfasst MSL-Anwendbarkeit, Security-Checkliste, sprachspezifische Regeln, Dependency-Audit, ASVS-Verifikation, Supply-Chain-Evidenz, CRA-/Regulierungsentscheidungen und die ausgefüllten Secure-Development-Projektinstanzen.

*The memory-safe C#/.NET profile and all declared standards, conditional evidence, N/A rationales, triggers, and canonical security evidence paths are binding.*

## Audit Evidence Applicability

Jeder nachfolgende Governance-Checkpoint ist `Applicable`, sofern nicht ausdrücklich `N/A` angegeben. Die Umsetzungsinstanzen führen zusätzlich `Fulfilled`, `Partly Fulfilled`, `Not Fulfilled` oder `Not Assessed`. Ein späterer `Open`-Befund benötigt immer Owner, Reviewer, Restrisiko, Maßnahme, Zieltermin und Trigger.

*Each governance checkpoint below is `Applicable` unless explicitly marked `N/A`. Implementation instances also record the allowed implementation status. Any later `Open` finding always needs an owner, reviewer, residual risk, action, due date, and trigger.*

| Checkpoint | Status | Markdown-Evidenz / Markdown evidence | Owner / Reviewer | Restrisiko und Neubewertung / Residual risk and re-evaluation |
|---|---|---|---|---|
| Secure-Development-Baseline und 157 CL-IDs / baseline and 157 CL IDs | Applicable | `docs/security/secure-development/2026-08-30-secure-development-hardening/` | Projektverantwortung / Security-Reviewer | Fehlende oder veraltete Evidenz blockiert den Abschluss; Neubewertung bei Baseline- oder Scope-Änderung. / Missing or stale evidence blocks closeout; re-evaluate on baseline or scope change. |
| Security-, ASVS-, Threat-, Supply-Chain- und Regulierungsnachweise / security evidence set | Applicable | Kanonische Dateien aus CR-011 / Canonical files from CR-011 | Entwicklung und Release-Verantwortung / Security-Reviewer | Nicht erfüllte Kontrollen bleiben priorisierte Befunde; Neubewertung bei Code-, Paket-, Release- oder Deployment-Änderung. / Unmet controls remain prioritized findings; re-evaluate on code, package, release, or deployment change. |
| Allgemeine und sichere Architektur / general and secure architecture | Applicable | `docs/architecture/`, `docs/security/arc42-security.md`, `docs/security/adr/` | Architekturverantwortung / Security-Reviewer | Unbelegte Trust-Boundary- oder Qualitätsentscheidungen blockieren betroffene Gates; Neubewertung bei Struktur-, Schnittstellen- oder Laufzeitänderung. / Unsupported boundary or quality decisions block affected gates; re-evaluate on structural, interface, or runtime change. |
| Barrierefreiheit und Didaktik / accessibility and didactics | Applicable | `docs/accessibility/secure-development-hardening.md` | Dokumentationsverantwortung / A11Y-Reviewer | Unverständliche oder visuell abhängige Evidenz bleibt offen; Neubewertung bei Nutzeroberflächen-, Ausgabe- oder Dokumentationsänderung. / Inaccessible evidence remains open; re-evaluate on UI, output, or documentation change. |
| Cross-Platform-Skriptparität / cross-platform script parity | N/A | IR-016 und FR-027 / IR-016 and FR-027 | Entwicklung / Plattform-Reviewer | Kein aktuelles Restrisiko aus neuem Skript; sofortige Neubewertung, wenn der Plan ein Skript ändert oder anlegt. / No current new-script risk; re-evaluate immediately if the plan changes or adds a script. |
| Agenten- und Spec-Kit-Parität / agent and Spec Kit parity | Applicable | Fünf Agentenflächen, Memory-Constitution und betroffene Templates / Five agent surfaces, memory constitution, and affected templates | Projektverantwortung / Governance-Reviewer | Drift kann widersprüchliche Agentenentscheidungen erzeugen; Neubewertung bei Regel-, Preset- oder Template-Änderung. / Drift can cause conflicting agent decisions; re-evaluate on rule, preset, or template change. |
| Autonome Zustands- und Gate-Evidenz / autonomous state and gate evidence | Applicable | `specs/002-secure-development-hardening/` und coordinator-owned Runtime-Evidenz / feature path and coordinator-owned runtime evidence | Äußerer Koordinator / Delivery-Reviewer | Unklare Autorität oder unvollständige Gate-Kausalität blockiert Delivery; Neubewertung nach Unterbrechung, Drift oder Zustandswechsel. / Unclear authority or incomplete gate causality blocks delivery; re-evaluate after interruption, drift, or state change. |

## Installed Governance Preset Authority

Das Lastenheft vom 2026-06-17 nennt als damalige Teilmenge sechs Presets. Der am 2026-08-30 durch `specify preset list` festgestellte Stack umfasst zwölf aktivierte Presets und ist für diesen Lauf maßgeblich:

*The intake dated 2026-06-17 lists an older subset of six presets. The twelve enabled presets resolved on 2026-08-30 are authoritative for this run:*

| Priorität | Preset | Installierte Version | Scope-Wirkung / Scope effect |
|---:|---|---:|---|
| 10 | `security-governance` | 0.6.2 | Produkt- und Evidenzanforderungen / Product and evidence requirements |
| 20 | `architecture-governance` | 0.5.2 | Sichere Architektur und Cloud-N/A / Secure architecture and cloud N/A |
| 30 | `isaqb-architecture-governance` | 0.2.2 | Allgemeine Architektur-Evidenz / General architecture evidence |
| 40 | `a11y-governance` | 0.4.3 | A11Y, DE/EN, B2 und Didaktik / Accessibility, language, readability, didactics |
| 50 | `cross-platform-governance` | 0.2.2 | Bedingte Skriptparität / Conditional script parity |
| 60 | `agent-parity-governance` | 0.4.2 | Gemeinsame Agentenflächen / Shared agent surfaces |
| 61 | `model-routing-governance` | 0.1.4 | Agentenneutrale lokale Phasenbindung / Agent-neutral local phase routing |
| 64 | `intake-authoring-governance` | 0.3.1 | Intake-Herkunft und Grenzen / Intake provenance and boundaries |
| 65 | `intake-review-governance` | 0.2.1 | Akzeptierter Review-Gate-Kontext / Accepted review-gate context |
| 66 | `intake-sequencing-governance` | 0.2.3 | Keine konkurrierende Intake-Ausweitung / No competing intake expansion |
| 70 | `autonomous-run-governance` | 0.4.1 | Zustands-, Evidenz- und Autoritätsgrenzen / State, evidence, and authority boundaries |
| 80 | `parallel-autonomous-run-governance` | 0.2.6 | Kampagnenregeln nur bei explizitem Start / Campaign rules only when explicitly started |

Die sechs zusätzlichen Prozess-Presets ändern nicht den fachlichen Härtungsscope. Bei Konflikten gilt die aktuell installierte, zusammengesetzte Governance; historische Intake-Angaben bleiben zur Rückverfolgbarkeit erhalten.

*The six additional process presets do not expand the product hardening scope. Current composed governance wins in conflicts, while historical intake statements remain traceable.*
