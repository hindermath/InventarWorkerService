# Abhaengigkeiten pflegen / Dependency Maintenance

## Deutsch

Verbindliche Konfiguration: [Dependabot](dependabot.yml). Owner: Repository-Maintainer.
Versionsupdates werden montags ab 06:50 Uhr (Europe/Berlin) geprueft; die tatsaechliche Startzeit kann abweichen.
Minor- und Patch-Updates sind je Oekosystem gebuendelt, Major-Updates bleiben einzeln.
Je Oekosystem sind hoechstens fuenf Versions-PRs gleichzeitig offen.
Sicherheitsupdates verwenden eigene Gruppen und warten nicht auf den Wochenlauf.
Vorhandene SHA-Pins bleiben erhalten. Es wird kein Auto-Merge eingerichtet.

Kopierte Maintenance-TUI-Dateien und zentral verteilte Wartungsworkflows werden in home-baseline gepflegt. Ihre Aktualisierung erfolgt ueber den bestehenden Propagationsprozess.
Historische Specs, Archive und Evidence-Fixtures werden nicht automatisch umgeschrieben.
Aktive Produkt-, Beispiel- und Testabhaengigkeiten bleiben erfasst.
Nicht in Manifesten erfasste Downloads und Toolversionen benoetigen weiterhin ihren eigenen Wartungsprozess.

Naechster Schritt: [Dependabot-Laeufe](https://github.com/hindermath/InventarWorkerService/actions/workflows/dependabot/dependabot-updates)
und [Sicherheitsmeldungen](https://github.com/hindermath/InventarWorkerService/security/dependabot) pruefen.
Bei Fehlern den betroffenen Job oeffnen und Manifestpfad, Registry-Zugriff und bestehende Pins kontrollieren.
Neue PRs anhand von Changelog, CI und Projektvorgaben pruefen und regulär freigeben.
Gruene Checks sind keine Produkt-, Plattform- oder Barrierefreiheitsabnahme.

## English

Authoritative configuration: [Dependabot](dependabot.yml). Owner: repository maintainer.
Version checks run on Mondays from 06:50 (Europe/Berlin); actual start times may vary.
Minor and patch updates are grouped per ecosystem; major updates stay separate.
Each ecosystem allows at most five open version PRs.
Security updates use independent groups and do not wait for the weekly run.
Existing SHA pins remain intact. This change configures no auto-merge.

Copied Maintenance TUI files and centrally distributed maintenance workflows are maintained in home-baseline and updated through the existing propagation process.
Historical specs, archives and evidence fixtures are not rewritten automatically.
Active product, example and test dependencies remain covered.
Downloads and tool versions outside supported manifests still need their existing maintenance process.

Next, inspect [Dependabot runs](https://github.com/hindermath/InventarWorkerService/actions/workflows/dependabot/dependabot-updates)
and [security alerts](https://github.com/hindermath/InventarWorkerService/security/dependabot).
For failed jobs, check manifest paths, registry access and existing pins in the job log.
Review new PRs using their changelog, CI and project requirements before a regular approval.
Passing checks do not establish product, platform or accessibility acceptance.

## Abdeckung / Coverage

| Oekosystem / Ecosystem | Pfad / Path |
|---|---|
| `github-actions` | `/` |
| `nuget` | `/` |
| `dotnet-sdk` | `/` |

## Dokumentationsentscheidung / Documentation Decision

UpdateRequired, 2026-11-03. Zielgruppe / audience: Maintainer und / and agents.
Leserpfad / reader path: configuration -> this guide -> Dependabot run or alert -> reviewed PR.
Quelle / source: repository-local configuration. Klasse / class: ActiveSemantic.
Sprache / language: German first, equivalent English in the same file.
Source-only; kein / no Home Sync, runtime distribution or package installation.
Nachweis / evidence: YAML round-trip, Dependabot schema and manifest coverage checks;
hosted checks are recorded on the configuration PR before merge.
Risiko / risk: update proposals still require compatibility and security review.
Neu pruefen bei / reevaluate on: new manifests, moved paths, changed pins or maintenance ownership.

## Claude-GitHub-Integration entfernt / Claude GitHub Integration Retired

DE: Die Claude-GitHub-App und ihre automatischen PR-/Issue-Workflows wurden
auf ausdruecklichen Ownerwunsch entfernt. Fuer dieses Repository starten keine
Claude-Code-GitHub-Actions mehr. Lokale Claude-Werkzeuge und CLAUDE.md bleiben
unabhaengige Entwicklungswerkzeuge. Technische Checks und menschliche Reviews
gelten weiter; Abhaengigkeits-PRs erhalten kein Auto-Merge.

EN: The owner explicitly retired the Claude GitHub App and its automatic
PR/issue workflows. This repository no longer starts Claude Code GitHub Actions.
Local Claude tools and CLAUDE.md remain independent development tools.
Technical checks and human reviews remain required; dependency PRs receive
no auto-merge.

## Terminal.Gui: 1.x bis zur Produktmigration / 1.x until product migration

DE: InventarViewerApp, CtrlWorkerServiceApp und CtrlWorkerServiceCmdlet bleiben
bis zu einer ausdruecklich beauftragten und geprueften Migration bei Terminal.Gui
1.x. Am 3. November 2026 ist 1.19.0 die neueste stabile 1.x-Version auf NuGet und
bereits in allen drei Produktmanifesten eingetragen. Dependabot bietet weiterhin
Minor-/Patch-Updates innerhalb von 1.x an; regulaere Major-Versionsupdates fuer
genau das Paket Terminal.Gui sind vorerst ausgenommen. Terminal.Gui.Elmish hat
eine eigene Versionsfolge und wird von dieser Regel nicht erfasst.

Sicherheitsmeldungen und Sicherheitsupdate-Gruppen bleiben aktiv. Die Ausnahme
verwendet nur `update-types`, keinen pauschalen Versionsbereich: Ein erforderlicher
Sicherheits-PR kann daher weiterhin einen Major-Wechsel vorschlagen. Auch dieser
PR darf erst nach der notwendigen Produktmigration, Kompatibilitaetspruefung und
Freigabe gemergt werden; es gibt kein Auto-Merge. Die Grenze nach der gelieferten
2.x-Migration erneut pruefen und die Ausnahme dann bewusst entfernen.

EN: InventarViewerApp, CtrlWorkerServiceApp and CtrlWorkerServiceCmdlet remain on
Terminal.Gui 1.x until an explicitly commissioned and reviewed product migration.
On 3 November 2026, 1.19.0 is the latest stable 1.x NuGet release and is already
used by all three product manifests. Dependabot continues minor/patch updates
within 1.x; regular major version updates for the exact Terminal.Gui package are
temporarily ignored. Terminal.Gui.Elmish follows its own version sequence and
is outside this rule.

Security alerts and security-update groups remain active. The exception uses
only `update-types`, without a blanket version range, so a required security PR
may still propose a major change. Such a PR also needs product migration,
compatibility validation and approval before merge; no auto-merge is configured.
Reevaluate and deliberately remove the exception after delivering the 2.x migration.

Owner: Repository-Maintainer. UpdateRequired; Source-only, kein / no Home Sync.
Leserpfad / reader path: Dependabot configuration -> this policy -> reviewed
migration PR. Quellen / sources: product manifests, [NuGet version index](https://api.nuget.org/v3-flatcontainer/terminal.gui/index.json),
[Dependabot options](https://docs.github.com/en/code-security/reference/supply-chain-security/dependabot-options-reference#ignore).

## CI-Runner / CI runners

DE: Technische macOS-Pruefungen verwenden macos-15, da GitHub macos-14 am
2. November 2026 eingestellt hat. Pruefumfang, Berechtigungen und SHA-Pins bleiben erhalten.
EN: Technical macOS checks use macos-15 because GitHub retired macos-14 on
2 November 2026. Test coverage, permissions and SHA pins are retained.

Quelle / Source: https://github.blog/changelog/2026-10-01-github-actions-macos-14-runner-image-retirement/

## Native Contract-Laufzeit / Native contract runtime

DE: Der vollstaendige Linked-Intake-Nachweis darf unter Windows hoechstens 45 Minuten
laufen, unter Linux weiterhin 20 Minuten. Beide Shell-Suiten und die Pruefung auf
Repository-Schreibzugriffe bleiben Pflicht. Hintergrund ist ein beobachteter Abbruch
an der bisherigen Windows-Grenze von 20 Minuten.

EN: The complete linked-intake proof has a 45-minute Windows budget and retains
the 20-minute Linux budget. Both shell suites and the zero-repository-writes check
remain mandatory. This addresses an observed cancellation at the previous
20-minute Windows limit.

## Vollstaendige Windows-Inventarisierung / Full Windows inventory

DE: Der Software-Inventar-Integrationstest erlaubt unter Windows hoechstens fuenf
Minuten fuer die vollstaendige lokale Inventarisierung eines gehosteten Runners.
Die Standardgrenze anderer Requests und Plattformen bleibt unveraendert.
HTTP-Erfolg, nicht leerer Inhalt, gueltiges JSON und API-Authentifizierung werden
weiterhin geprueft; dies aendert keinen Produkt-Timeout und kein Antwortzeit-SLO.

EN: On Windows, the software inventory integration test allows at most five
minutes for full local inventory of a hosted runner. Other requests and platforms
retain their existing limit. HTTP success, nonempty content, valid JSON and API
authentication remain checked; no product timeout or response-time SLO changes.
