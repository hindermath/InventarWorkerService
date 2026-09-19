# Coverage-Entwurf: Abgleich und Folgeaufgabe / Coverage draft reconciliation

## Deutsch

Stand: 2026-09-19. Owner: Repository-Maintainer.
Geprüfte Inhaltsrevision: `0adaae0fa93873a68f10a697d1e3e9c456ece396`.
Status: **Abgleich abgeschlossen; Folgeaufgabe offen. Keine Coverage-Abnahme.**

Der historische [Entwurf](../archive/001-coverage-compliance/spec.md) und seine
[Checkliste](../archive/001-coverage-compliance/checklists/requirements.md)
stammen aus `001-coverage-compliance`, Commit
`9fc4a23d33543c0fb243d807f32694f2471a0adb`. Beide Kopien sind bytegleich.
Sie bleiben historische Dokumente, keine aktuelle Implementierungsanweisung.
Der Branch wird nach verifizierter Vollhistorien-Sicherung entfernt; seine
sechs ursprünglichen Commits werden nicht nach main übernommen.

### Nachweisgrenze

Coverage bezeichnet den durch Tests ausgeführten Anteil des instrumentierten Codes.
Die lokalen Cobertura-Berichte vom 10. März 2026 zeigen 1,39 % für
InventarWorkerCommon und 46 % für CtrlWorkerCommon. Sie stammen aus
`TestResults/8a19de85-0e84-4783-bfc1-a6b818bdff0d/` und
`TestResults/bbac8d42-474c-42cc-bc4e-05b378887551/`.
Das sind **keine aktuellen Werte**.

[Feature-002-Quickstart](https://github.com/hindermath/InventarWorkerService/blob/0adaae0fa93873a68f10a697d1e3e9c456ece396/specs/002-secure-development-hardening/quickstart.md)
dokumentiert später 110 bestandene Tests, zwei Plattform-Skips und 90,1 %
Zeilenabdeckung. Das ist historische, aggregierte Evidence, kein aktueller
Nachweis von mindestens 70 % für jede der beiden Bibliotheken.
[CI](https://github.com/hindermath/InventarWorkerService/blob/0adaae0fa93873a68f10a697d1e3e9c456ece396/.github/workflows/ci.yml) prüft einen aggregierten Summary-Wert.
[Runsettings](https://github.com/hindermath/InventarWorkerService/blob/0adaae0fa93873a68f10a697d1e3e9c456ece396/InventarWorkerCommonTest/coverlet.runsettings), gebunden im
[Testprojekt](https://github.com/hindermath/InventarWorkerService/blob/0adaae0fa93873a68f10a697d1e3e9c456ece396/InventarWorkerCommonTest/InventarWorkerCommonTest.csproj),
begrenzen die einbezogenen Klassen. Eine hohe Quote dieses Ausschnitts
belegt nicht die Abdeckung der gesamten Bibliothek.

Kein neuer Build oder Testlauf wurde für diesen Dokumentationsabgleich ausgeführt.
Bestehende Tests berühren teilweise Plattformdienste, Prozessinventar und
dateibasierte Statuspfade. Ein erneuter Lauf ist erst isoliert und mit
geprüften Abhängigkeiten durchzuführen; keine Host-Dienste starten/stoppen.

### Anforderungen gegen aktuellen Quellstand

| Alte Anforderung | Ergebnis der statischen Prüfung |
| --- | --- |
| FR-001/002, SC-001/002/003: je Bibliothek 70 %, Ziel 80 % | Aktuell nicht nachgewiesen; CI prüft nur Gesamtwert. |
| FR-003/010: Namen und bilinguale XML-Kommentare | In SettingsAndStatusServiceTest und SqliteDbServiceTest sichtbar; kein vollständiger Dokumentations-Audit. |
| FR-004: unabhängig von Maschinenzustand | Nicht erfüllt nachgewiesen: Controller-Plattformtests und Softwareinventar verwenden reale Plattformpfade. Isolation bleibt offen. |
| FR-005: SQLite ausschließlich im Speicher | Abweichung: SqliteDbServiceTest verwendet eine temporäre Datei, mit Cleanup. Nutzen der Dateitests erhalten; In-Memory-Vertrag bewusst neu entscheiden. |
| FR-006: Statusdateien temporär und entfernt | GUID-basierter Statusname vorhanden; Cleanup stellt nur die Umgebungsvariable wieder her, löscht nicht das Testverzeichnis. |
| FR-007/008, SC-004/005/006: warnungsfreier Build, Regression, Wiederholbarkeit | Ohne neuen Lauf nicht bestätigt. Historische 31 Tests sind keine aktuelle Suite-Größe. |
| FR-009: Plattformtests markieren/überspringen | Plattformbezogene Testklasse vorhanden; Skips je Betriebssystem erneut nachweisen. |
| FR-011: IWindowsServiceController und injizierbarer Wrapper | Das benannte Interface ist in CtrlWorkerCommon nicht vorhanden; Testbarkeit bleibt Folgearbeit. |
| FR-012: beschädigtes JSON ergibt null | ReadStatus fängt Ausnahmen und gibt null zurück; gezielter Korruptionsfall fehlt in SettingsAndStatusServiceTest. |
| FR-013: fehlerhafte CSV-Zeilen überspringen | Aktueller SQLite-Test prüft gültige Importe, Duplikate und fehlende Datei; Mischfall gültig/ungültig bleibt unbewiesen. |
| FR-014: interne Pfadkonstruktoren, InternalsVisibleTo | Alter Ansatz nicht nachgewiesen; aktuelle Tests verwenden die Umgebungsvariable. Isolierte Pfadübergabe prüfen. |
| FR-015: SoftwareInventoryService | SoftwareInventorySecurityTest prüft Geheimnis-Redaktion, aber mit realem Inventarlauf; unabhängige Parsing-/Fehlerfalltests und Coverage bleiben offen. |
| FR-016: sechs Klassen pauschal ausschließen | Nicht übernehmen: heutige Tests schließen z. B. PostgreSQL, Initialize und ServiceContainer ausdrücklich ein. Jede Ausnahme braucht aktuelle Begründung. |
| Story 5: Hardwareinventar | Historisch optional; kein neues Abschluss-Gate durch diese Archivierung. |

### Aktuelle Folgeaufgabe: belastbare projektbezogene Coverage

Priorität: nächster gezielter Test-/Coverage-Arbeitsschritt; kein automatischer
Spec-Kit-Start und keine Änderung der Intake-Reihenfolge. Owner: Repository-Maintainer.
Abnahme und Implementierung sind offen:

- [ ] Messumfang pro Bibliothek und alle Filter/Ausschlüsse dokumentieren.
      Alte pauschale Ausschlüsse nicht übernehmen; sicherheitsrelevante Pfade erhalten.
- [ ] Isolierten Testlauf vorbereiten: keine produktive DB, kein reales
      Start/Stop von Diensten, temporäre Dateien zuverlässig im finally/Cleanup entfernen.
- [ ] Controller-Test-Doubles, beschädigtes Status-JSON, fehlende Settings,
      gemischtes CSV und unabhängige Softwareinventar-Tests ergänzen oder
      vorhandene gleichwertige Nachweise zuordnen. SQLite-Datei versus In-Memory
      ausdrücklich entscheiden, nicht pauschal bestehende Tests ersetzen.
- [ ] Frisch je Bibliothek ausführbare/gedeckte Zeilen, Prozentsatz, Filter,
      Commit, SDK, OS, Testzahlen und Skips erfassen; Mindestziel 70 %, Ziel 80 %
      aus dem Altentwurf gegen heutige Governance bestätigen.
- [ ] Nach bestätigtem Messvertrag CI je Bibliothek statt nur aggregiert prüfen.
      Fehlende Bibliotheken und fehlende Berichte müssen fehlschlagen.
- [ ] Wiederholung und Plattformnachweise sowie Build-Warnungen prüfen.
      Erst dann die Folgeaufgabe als abgeschlossen kennzeichnen.

### Archiv und Dokumentationsauswirkung

Vollständiges lokales Git-Bundle:
`~/.home-baseline/branch-archive-20260919.jbhLcw/InventarWorkerService.bundle`.
SHA-256: `2b57ebe6c23db4c1727b47aead79bfcb519e8493a967cd7eba31d4f7a841a9d9`.
Es ist mit `git bundle verify` geprüft. Wiederherstellung im Repository:

```bash
git fetch /Users/thorstenhindermann/.home-baseline/branch-archive-20260919.jbhLcw/InventarWorkerService.bundle refs/heads/001-coverage-compliance:refs/heads/001-coverage-compliance
```

Documentation Impact: `UpdateRequired`, umgesetzt durch diesen kanonischen
Wartungsnachweis und das verlinkte Archiv. Leser: Maintainer und Testverantwortliche.
Dokumentklasse: Wartungsabgleich mit offenem Backlog; Sprachpartner unten.
Quelle: alter Entwurf und oben verlinkter aktueller Quellstand.
Textorientiert, ohne farbabhängige Darstellung; keine API-/Runtime-Änderung,
kein Home-Sync. Erneut prüfen bei Beginn der Folgeaufgabe. Statistik-Renderer
bleibt gemäß aktuellem Projektvertrag bis zum ausdrücklich beauftragten
Statistik-Update nach Inhaltscommit unberührt.
NIST SSDF/CWE Top 25 sind für Nachweisqualität und Testisolation relevant.
Keine neue Sicherheitsabnahme: ASVS/SBOM/SLSA-/Architekturnachweise bleiben
unverändert; keine Release- oder Laufzeitänderung. KI wird nur als
Entwicklungswerkzeug verwendet, keine neue AI-SBOM-Pflicht.

## English

Date: 2026-09-19. Owner: repository maintainer. Reviewed content revision:
`0adaae0fa93873a68f10a697d1e3e9c456ece396`.
Reconciliation is complete; follow-up is open. This is not coverage acceptance.

The linked original specification and checklist are byte-identical historical
copies from `9fc4a23d33543c0fb243d807f32694f2471a0adb`.
They do not reactivate the old implementation plan. The six old commits are
archived, not merged. The full-history bundle, SHA-256 and restore command
above preserve the deleted branch.

Coverage is the share of instrumented code exercised by tests. Local March 10
reports show 1.39% for InventarWorkerCommon and 46% for CtrlWorkerCommon.
They are stale. Feature 002 later recorded 110 passed tests, two platform
skips and 90.1% aggregate line coverage. Neither proves current coverage of
each library. CI checks the aggregate summary; test-project runsettings
restrict included classes. No fresh build or test run was performed.

Requirement disposition mirrors the German table:

- FR-001/002 and SC-001/002/003: fresh per-library 70% minimum / 80% target evidence missing.
- FR-003/010: naming and bilingual XML comments visible in inspected tests, not fully audited.
- FR-004: machine-independent isolation not established; real platform/inventory calls remain.
- FR-005: SQLite tests use cleaned-up temporary files, not in-memory databases.
- FR-006: status cleanup restores the environment variable but does not remove its directory.
- FR-007/008 and SC-004/005/006: current build, regression and repeatability unverified.
- FR-009: platform tests exist; verify OS-specific skip behavior.
- FR-011: the named injectable Windows controller interface is absent.
- FR-012: reader catches errors and returns null; targeted corrupt-JSON test missing in the inspected class.
- FR-013: valid CSV and duplicate tests exist; mixed malformed/valid input remains unproven.
- FR-014: current tests use environment configuration, not the old internal-path-constructor approach.
- FR-015: secret-redaction test exists but invokes real inventory; isolated parsing/error coverage remains open.
- FR-016: reject blanket historical exclusions; current tests deliberately include PostgreSQL,
  initialization and the service container. Justify exclusions individually.
- Hardware inventory remains historically optional.

Current follow-up, owned by the maintainer: define per-library denominators and
filters; prepare isolated execution with no production DB or host service
mutations; add or map controller doubles, corrupt JSON, missing settings,
mixed CSV and independent inventory tests; decide file-backed versus in-memory
SQLite intentionally. Record fresh covered/executable lines, percentages,
filters, commit, SDK, OS, test totals and skips. Confirm the inherited 70%/80%
contract against current governance, then enforce it per library in CI,
failing on missing reports/libraries. Verify repeatability, platforms and
build warnings before closing. No Spec Kit run or intake reordering starts here.

Documentation Impact is UpdateRequired, fulfilled by this source-only
maintenance record and linked archive. Audience: maintainers and test owners.
Reevaluate when follow-up starts. No API/runtime changes or Home sync.
Statistics await an expressly authorized post-content-commit update.
NIST SSDF/CWE Top 25 inform evidence and isolation; this is no new security,
ASVS, SBOM, SLSA or architecture acceptance. AI is development tooling only.
