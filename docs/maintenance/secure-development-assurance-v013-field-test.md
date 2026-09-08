# Secure Development Assurance v0.1.3 – InventarWorkerService-Feldtest

## Ergebnis und Scope / Result and Scope

**Empfehlung: `ReleaseAccepted`.** InventarWorkerService bestätigt das
unveränderte Preset `secure-development-assurance-governance` v0.1.3 im
dokumentierten Projektfeldtest. Die Empfehlung betrifft nur die
Preset-Funktion im nichtkommerziellen Ausbildungs- und Beispielprojekt. Sie ist
keine Produkt-, Pilot-, Projekt-, Risiko-, C5-, Konformitäts- oder
Zertifizierungsfreigabe.

*Recommendation: `ReleaseAccepted`. InventarWorkerService confirms the
unchanged v0.1.3 preset in this documented project field test. This covers
preset behavior in a non-commercial training and example project only. It is
not a product, pilot, project, risk, C5, conformity, or certification approval.*

Test-Owner und technischer Reviewer ist `@hindermath`. Kontext:
`docs/security/secure-development/2026-08-30-secure-development-hardening`,
Modus `development`. Produktcode, APIs, Runtime, Abhängigkeiten, Images und
Deployment wurden nicht geändert.

## Paket- und Lieferbindung / Package and Delivery Binding

| Feld / Field | Nachweis / Evidence |
|---|---|
| Release | `v0.1.3`, Pre-Release |
| Tag-Commit | `0d03aa9ebe8f74a26e331815bca5609fb48d7a14` |
| ZIP SHA-256 | `9023b442b4d82e25bee5a7fe9b73efb7f591a4f265f54061ae6e4a56b9b5c75f` |
| Spec Kit | `0.12.8` |
| Security Governance | `0.6.2`, Priorität 10 |
| Assurance-Preset | `0.1.3`, Priorität 15 |
| Profil | 13 Presets, exakt |
| Host | macOS 26.6.2, Apple Silicon; Bash 3.2.57; PowerShell 7.6.5 |
| Delivery | InventarWorkerService PR #67; Evidence-Commit wird nach der Git-Bindung ergänzt |

## Technische Prüfung / Technical Validation

| Test | Ergebnis | Exitcode |
|---|---|---:|
| Release-ZIP und SHA-256 | bestanden | 0 |
| 13-Preset-`CheckOnly`, Bash und PowerShell | bestanden | 0 |
| `preset list`/`info`/`resolve`; `specify check` | bestanden | 0 |
| Status in Bash und PowerShell | `Ready`, fachlich gleich | 0 |
| Vier Einzelreviews je Shell | alle `Ready` | 0 |
| Roh-Hash-Snapshot | 7 von 7 Evidence-Dateien unverändert | 0 |
| Vertrags-, Negativ-, LF-/CRLF-/BOM- und Shell-Parität | bestanden | 0 |
| Acht erzeugte Agenten-/Command-Flächen | bestanden; fehlende Evidence blockiert geregelt | 0 |
| Temporäre Komposition: 13, Disable, Enable, Remove, gültige 12, Reinstall | bestanden | 0 |
| Historische Bewertungsmatrix | 157 eindeutige IDs; 31 N/A, 126 Open | 0 |

Die Negativsuite bestätigt alle vorgesehenen Fail-closed-Pfade mit dem
erwarteten Exitcode 2; der Gesamttest endet mit 0. Der synthetische
C5-/Zertifizierungsfall prüft ausschließlich sichere Blockierung und ist keine
C5-Prüfung des Projekts. Die historische Fachbewertung vom 2026-08-30 wurde
nicht rückwirkend als erfüllt umgedeutet.

*The negative suite confirms the required fail-closed paths with exit code 2;
the complete suite returns 0. The synthetic C5/certification case tests safe
blocking only and is not a project C5 assessment. The historical 2026-08-30
domain assessment is not retroactively promoted to fulfilled.*

## Grenzen und Wiedervorlage / Boundaries and Review Dates

- Baseline, Delta, Closure und Image Impact sind `Ready`;
  `technicalValidation` ist `Fulfilled`.
- `pilotAuthorization`, `projectAcceptance` und `generalRelease` bleiben
  `Open`.
- Technische Evidence-Wiedervorlage: `2027-09-08`.
- C5 sowie CRA und formale Produktkonformität: `N/A` im aktuellen
  nichtkommerziellen Ausbildungs-/Beispielscope.
- Regulatorische Scope-Wiedervorlage: `2026-12-31`, früher bei kommerzieller
  Nutzung, Marktbereitstellung, Kundenübergabe, Supportvertrag, Cloud-Runtime,
  Provider-Assurance oder geänderter Hersteller-/Steward-Rolle.
- Kein Risiko wurde akzeptiert. Restrisiko ist eine unbemerkte Scopeänderung.

## Abschluss und Dokumentationsauswirkung / Closeout and Documentation Impact

Es besteht keine fachliche Bash-/PowerShell- oder
LF-/CRLF-/UTF-8-BOM-Abweichung. Offene Punkte dieses Projektfeldtests: keine.
Die Community-Einreichung `github/spec-kit#4455`, alle fünf Projektberichte und
die spätere zentrale v0.1.3-Entscheidung werden abgewartet.

`UpdateRequired`. Owner: Thorsten Hindermann. Zielgruppen: Maintainer,
technische Reviewer und Lernende. Leserpfad: v0.1.3-Adoption → Feldbericht →
Security-Scope → maschinenlesbare Evidence. Deutsch zuerst, Englisch danach,
textorientiert, `sourceOnly`, kein Home-Sync. Neu bewerten bei Preset-,
Baseline-, Produkt-, Delivery-, Cloud- oder Scopeänderung.

*There is no substantive shell or line-ending variance and this project field
test has no open finding. Community issue `github/spec-kit#4455`, all five
project reports, and the later central decision remain pending. Documentation
impact is UpdateRequired, source-only, bilingual, text-first, and requires no
Home sync.*
