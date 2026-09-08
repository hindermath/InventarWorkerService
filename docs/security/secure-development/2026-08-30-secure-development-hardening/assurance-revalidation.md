# Assurance-Revalidierung / Assurance Revalidation

## Ergebnis und Grenze / Result and Boundary

**DE:** Die technische Revalidierung vom 2026-09-08 beseitigt die dokumentierte
Baseline-Versionsabweichung und vervollstaendigt den Assurance-Evidence-Vertrag.
Alle vier technischen Gates sowie der strengste Gesamtstatus sind Ready.
Dieses Ergebnis bewertet die Integritaet des Gate-Satzes, nicht die Wirksamkeit
aller fachlichen Kontrollen. 31 Kontrollen sind als nicht anwendbar und 126 als offen dokumentiert. Es wird keine
Risikoakzeptanz, Zertifizierung, Rechts-, C5-, Pilot-, Projekt- oder
Releasefreigabe abgeleitet.

**EN:** The technical revalidation dated 2026-09-08 resolves the recorded
baseline version drift and completes the Assurance evidence contract. All four
technical gates and the strictest overall status are Ready. This result
assesses gate-set integrity, not the effectiveness of every domain control.
31 controls are recorded as not applicable and 126 as open. No risk acceptance, certification, legal, C5,
pilot, project, or release approval is inferred.

## Aenderung / Change

**DE:**

- Die verwaltete Secure-Development-Baseline wurde aus der Level-0-Quelle
  synchronisiert. Das Repository bindet jetzt Baseline und Richtlinie 3.2.0,
  Sammelband, CL-09 und CL-12 2.2.0, SDLC-Richtlinie 1.2.0 sowie die
  Verzahnungskarte 1.4.0.
- Der frühere blockierte Migrations-Gate-Satz bleibt unter
  archive/2026-09-07-assurance-migration/ als historische Evidence der
  damaligen Abweichung erhalten.
- Die aktiven Gates bewerten nur den technischen Evidence-Vertrag. C5, CRA und
  formale Produktkonformität sind für den aktuellen nichtkommerziellen
  Ausbildungs- und Beispielscope `N/A`; die historische 157-Punkte-Quelle wird
  nicht rückwirkend umgeschrieben oder zu erfüllten Zuständen hochgestuft.

**EN:**

- The managed secure-development baseline was synchronized from the Level-0
  source. The repository now binds baseline and guideline 3.2.0, compendium,
  CL-09, and CL-12 2.2.0, SDLC guideline 1.2.0, and integration map 1.4.0.
- The former blocked migration gate set is retained under
  archive/2026-09-07-assurance-migration/; it remains historical evidence of the earlier drift.
- Active gates assess the technical evidence-contract layer. C5, CRA, and
  formal product conformity are `N/A` for the current non-commercial training
  and example scope. The historical [assessment-records.json](assessment-records.json)
  source is not rewritten retroactively or promoted to fulfilled states.
## Gate-Nachweise / Gate Evidence

| Gate | Ergebnis / Outcome | Evidence-Grenze / Evidence boundary |
|---|---|---|
| Baseline | Ready | Manifestversion, alle gelenkten Dokumentversionen und normalisierte SHA-256-Bindungen / Manifest version, all controlled document versions, and normalized SHA-256 bindings |
| Delta | Ready | Begrenzte Baseline-/Evidence-Korrektur; keine Produkt- oder Runtime-Änderung / Bounded baseline/evidence correction; no product or runtime change |
| Closure | Ready | Nur technische Validierung; drei menschliche Freigabeentscheidungen bleiben Open / Technical validation only; three human approval decisions stay Open |
| Image Impact | Ready | Nur Dokumentations-/Evidence-Delta; keine Image-, Paket-, Netzwerk-, Mount- oder Secret-Änderung / Documentation/evidence-only delta; no image, package, network, mount, or secret change |

**DE:** Die installierten Bash- und PowerShell-Validatoren werden für jedes
Gate und den Gesamtstatus ausgeführt. assurance-validation.json erfasst Befehle,
Exitcodes, Gate-Hashes, Parität und die Read-only-Erhaltungsprüfung.

**EN:** The installed Bash and PowerShell validators are executed for every gate
and for overall status. assurance-validation.json records commands, exit codes,
gate hashes, parity, and the read-only preservation check.

## Dokumentationsauswirkung / Documentation Impact

**DE:** Entscheidung: UpdateRequired. Kanonische Quellen sind die
Secure-Development-Baseline aus Level 0 und die repository-lokale fachliche
Bewertungsquelle. Owner ist die InventarWorkerService-Repository-Maintainerrolle. Betroffen
sind Baseline-Referenzen, aktive Gate-JSONs, Evidence-Matrix,
Revalidierungsbericht, Validierungsreceipt und Projektstatistik. Der Leserpfad
führt von der Matrix über die Gates zur fachlichen Quelle. Die Evidence ist
repository-lokal, DE-zuerst/EN-danach, textorientiert und benötigt keinen
Home-Sync. Nach jeder relevanten Quellen-, Scope-, Produkt- oder
Infrastrukturänderung, spätestens am 2027-09-08, ist neu zu bewerten. Die
regulatorische Scopeentscheidung besitzt zusätzlich die Wiedervorlage
2026-12-31.

**EN:**

- Decision: UpdateRequired.
- Canonical source: Level-0 secure-development baseline plus this repository's
  unchanged domain-assessment source.
- Owner: InventarWorkerService repository maintainer role.
- Affected documents: managed baseline references, active gate JSON files,
  evidence matrix, revalidation report, validation receipt, and project
  statistics.
- Reader paths: maintainer and security reviewer start at evidence-matrix.md,
  continue to the gate JSON files, then follow links to the domain source.
- Navigation and class: context-local governed evidence; no public landing-page
  or product API navigation changes.
- Language and accessibility: German first, English second; status is always
  expressed in text and does not rely on colour or visual-only cues.
- Platform evidence: macOS Bash and PowerShell validator parity locally;
  repository PR checks remain the delivery gate.
- Distribution and Home sync: repository-local Level-2 evidence;
  NoHomeSyncRequired.
- Re-evaluation: immediately after any baseline, evidence, product,
  architecture, dependency, workflow, distribution, image, or scope change,
  and no later than 2027-09-08. The regulatory scope decision is additionally
  reviewed on 2026-12-31.

## Naechste Aktion / Next Action

**DE:** pilotAuthorization, projectAcceptance und generalRelease bleiben
Open, bis ausdrücklich befugte menschliche Evidence vorliegt. Der
projektbezogene v0.1.3-Feldtest empfiehlt `ReleaseAccepted` nur für das Preset;
die zentrale Preset-Abnahme wartet auf alle fünf Projekte und
`github/spec-kit#4455`.

**EN:** Keep pilotAuthorization, projectAcceptance, and generalRelease Open
until explicitly authorised human evidence exists. The project field test
recommends `ReleaseAccepted` for the preset only; the central preset decision
waits for all five projects and `github/spec-kit#4455`.
