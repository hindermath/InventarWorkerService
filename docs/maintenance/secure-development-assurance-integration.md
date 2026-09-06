# Secure Development Assurance – Projektintegration / Project Integration

## Zweck und freigegebener Umfang / Purpose and Approved Scope

InventarWorkerService verwendet ausdrücklich das Profil
`secure-development-assurance-thirteen-governance-presets`: die bisherigen
zwölf Presets unverändert plus Assurance v0.1.2, aktiviert mit Priorität 15.
Die [13er-Matrix](../../scripts/config/spec-kit-secure-development-assurance-governance-presets.json)
und der [Profilkatalog](../../scripts/config/spec-kit-preset-profiles.json)
dokumentieren diese Entscheidung. Ältere Profile und globale Defaults bleiben
erhalten; die lokale Level-2-Registrierung wird erst nach MergeAndSync umgestellt.

*InventarWorkerService explicitly opts into the thirteen-preset profile. The original
twelve presets remain unchanged; Assurance v0.1.2 is enabled at priority 15.
Older profiles and global defaults are retained. Local registry assignment
follows successful merge and main synchronization.*

## Produktquelle und Paketbindung / Product Source and Package Binding

- [Eigenständiges GitHub-Repository / Standalone GitHub repository](https://github.com/hindermath/spec-kit-preset-secure-development-assurance-governance).
- Tag `v0.1.2`, Commit `02423602592ad0183454e259df628ab940436ba6`.
- [Öffentliches Tag-ZIP / Public tag ZIP](https://github.com/hindermath/spec-kit-preset-secure-development-assurance-governance/archive/refs/tags/v0.1.2.zip).
- SHA-256: `4eb30804bb3c329681e0b7d44187c8daeb3e9e4f250bb6003d5b746c0ad0b656`.
- Voraussetzung / prerequisite: `security-governance >=0.6.1`; vorhanden / installed: `0.6.2`.

Die explizite Versionsbindung gilt unabhängig von „Latest“. Veröffentlichte
GitHub-Preset-Repositories sind alleinige Produktquellen; lokale Installationen
sind Integrationskopien. [Community-Issue #4455](https://github.com/github/spec-kit/issues/4455)
belegt die Einreichung, nicht automatisch die Aufnahme in den Katalog.

*The explicit version pin is independent of “Latest”. Published standalone
GitHub repositories are the only product sources; installed copies are
integrations. The community issue records submission, not catalog acceptance.*

## Bedienung / Usage

Sicherer Einstieg: `$speckit-secure-development-status [<evidence-dir>]`.
Andere Agentenflächen verwenden `speckit.secure-development-status`.
Ohne Verzeichnis wird der lexikografisch neueste Kontext unter
`docs/security/secure-development/` geprüft. Direkter Aufruf ab Projektwurzel:

```bash
bash .specify/presets/secure-development-assurance-governance/scripts/validate-secure-development-assurance.sh status
```

```powershell
pwsh -NoProfile -File .specify/presets/secure-development-assurance-governance/scripts/validate-secure-development-assurance.ps1 -Action Status
```

Der zweite Befehl lautet
`$speckit-secure-development-review <baseline|delta|closure|image-impact> <context-id> <training|mixed|development>`.
Er benötigt einen ausdrücklichen Auftrag für den benannten Kontext. Die
Installation startet keinen Review, keinen autonomen Lauf und keine GSDB- oder
RL-SE-Selbstprüfung. Ausführliche Bedienung und Fehlerbehebung:
[installierte Paket-README](../../.specify/presets/secure-development-assurance-governance/README.md).

*Start with the read-only status command. Review requires explicit authority
for the named context. Installation does not start reviews, autonomous runs,
GSDB or RL-SE assessments. See the installed package README for complete usage
and troubleshooting. Never shorten the installed validator path.*

## Nachweis und Sicherheitsgrenzen / Evidence and Safety Boundaries

Am 2026-09-06 geprüft: öffentliches Archiv mit gebundenem SHA-256,
bytegleich übernommenes Paket, exakte 13er-Matrix und 562 geschützte
Bestandsdateien bytegleich. Die zwölf bisherigen Registereinträge inklusive
Versionen, Aktivierungszuständen und Prioritäten bleiben unverändert.
Vorhandene generierte Befehle bleiben unverändert; ausschließlich die neuen
Status-/Review-Oberflächen werden ergänzt.

Paket-Vertrags-, Negativ-, LF/CRLF/BOM- und Bash-/PowerShell-Paritätstests sowie
Oberflächentests liefen erfolgreich in temporären Projekten. Zusätzlich sind
die temporären Kompositionstests für alle Profile 8 bis 13 bestanden. Kein
produktives Test-Review wurde ausgeführt. Secret-Scan und bestehende PR-CI
bleiben Liefergates; nur die formale Codeowner-Hürde darf nach ausdrücklicher
Freigabe und bestandenen technischen Prüfungen per Admin-Merge übergangen werden.

Lesender Status in beiden Shells: `Blocked` (Exitcode 2), evidence-matrix.md fehlt / evidence-matrix.md missing.
Es liegt damit keine erfolgreiche Assurance-Bewertung vor. Baseline-, Delta-,
Closure- und Image-Impact-Gate sowie `technicalValidation`,
`pilotAuthorization`, `projectAcceptance` und `generalRelease` werden nicht
als erfüllt behauptet. Ohne gültige Evidence gibt es keine verlässlich
auslesbare nächste fachliche Aktion. Eine solche Prüfung bleibt ein separater
Auftrag; die Installation wird deshalb nicht als fachlich `Ready` bezeichnet.

Projektgeführte Baseline 3.1.0, Richtlinien, Checklisten und bestehende
Evidence bleiben erhalten. `CL-02-13 Cloud-Compliance-Assurance` ist ein
C5-Bezug, keine vollständige C5-Kriterien-, Testat- oder Readiness-Prüfung.
`Ready` bezieht sich ausschließlich auf den geprüften Evidence-Kontext und
ersetzt keine menschliche Freigabe oder Zertifizierung.

*The public archive, installed package identity, exact matrix, and
562 protected files were verified. The original twelve registry entries
and existing generated commands are preserved. Isolated contract, negative,
encoding, shell-parity, surface and profile 8–13 tests passed. Secret checks
and existing PR CI remain delivery gates; explicit authority permits bypassing
only the formal Codeowner requirement, never technical failures. Both status
validators fail closed with exit 2: evidence-matrix.md fehlt / evidence-matrix.md missing. No successful gate, human
decision, or next evidence action is inferred. Baseline 3.1.0, policies,
checklists and existing evidence stay unchanged. C5 is a limited relationship,
not attestation, certification, or full readiness. No substantive review starts.*

## Dokumentationsauswirkung / Documentation Impact

`UpdateRequired`, Owner: Thorsten Hindermann. Zielgruppen: Lernende, Maintainer
und KI-Agenten. Leserpfad: README → Integrationsnachweis → Paket-README →
lesender Status. Quelle: gebundenes GitHub-Release; lokale Integration:
Profilkatalog und Matrix. Betroffen: README, dieser Nachweis, fünf gemeinsame
Agenten-Dateien und Statistik-Ledger. Dokumentklasse: Bedienung/Governance;
Deutsch zuerst, Englisch danach, CEFR B2 und text-first. Linkpfade, Befehle und
beide Shells werden geprüft. Keine rein visuelle Bedeutungsübertragung.

Repository-Integration, kein Home-Sync und kein weiterer Flotten-Rollout.
Keine Runtime-, API- oder Verhaltensänderung; beide Constitutions auf Auswirkungen
geprüft. TDD/Changed-Code-Coverage für reine Paketübernahme und Dokumentation
`N/A`, Wiedervorlage bei eigener Produktlogikänderung. NIST SSDF und CWE Top 25
gelten; ASVS, neue Produkt-SBOM, Produkt-AI-SBOM und Zero Trust sind für diesen
Integrationsdelta `N/A`, da keine neue Web-/Auth-, Produkt- oder KI-Laufzeit
entsteht. Projektweit bestehende Pflichten werden dadurch nicht aufgehoben.
Wiedervorlage bei Paket-, Profil-, Baseline- oder Laufzeitänderungen.

*Documentation impact is UpdateRequired, owned by Thorsten Hindermann. The
bilingual, text-first reader path connects README, integration record, package
manual and read-only status. Five agent guides are kept aligned. This is
repository integration without Home sync or wider rollout; no runtime, API or
behavior changes. Both constitutions were checked for impact. Product TDD and
changed-code coverage are N/A for package reuse/documentation and must be
revisited for new product logic. SSDF/CWE apply; the other listed scopes add no
new obligations in this delta, without waiving existing project obligations.
Reevaluate at package, profile, baseline or runtime changes.*
