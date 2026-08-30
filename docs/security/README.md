# Sicherheitsdokumentation / Security Documentation

**Repository**: InventarWorkerService (Level-2)
**Constitution-Referenz / Constitution Reference**: Principle XII (A.8.28), Principle XIII (A.8.27), Principle XIV-XVIII

## Zweck / Purpose

Dieses Verzeichnis enthält die projektspezifische Sicherheitsdokumentation
fuer InventarWorkerService. Die Templates stammen aus `.specify/templates/` und
werden hier mit projektspezifischen Inhalten befuellt.

*This directory contains project-specific security documentation for
InventarWorkerService. Templates originate from `.specify/templates/` and are
populated here with project-specific content.*

## Feature-002-Einstieg / Feature 002 Entry

DE: Der kanonische Prüfpfad beginnt beim
[Feature-Assessment](secure-development/2026-08-30-secure-development-hardening/README.md),
führt über [Bedrohungsmodell](threat-model.md),
[arc42-Sicherheitskonzept](arc42-security.md),
[Abhängigkeits-Audit](dependency-audit.md) und
[Supply-Chain-Evidenz](supply-chain-evidence.md) zum
[Barrierefreiheitsnachweis](../accessibility/secure-development-hardening.md).

EN: The canonical review path starts with the
[feature assessment](secure-development/2026-08-30-secure-development-hardening/README.md)
and continues through the [threat model](threat-model.md),
[arc42 security concept](arc42-security.md),
[dependency audit](dependency-audit.md), [supply-chain evidence](supply-chain-evidence.md),
and [accessibility evidence](../accessibility/secure-development-hardening.md).

## Dokumente / Documents

| Dokument / Document | Template-Quelle / Template Source | Status |
|---|---|---|
| [threat-model.md](threat-model.md) | `threat-model-template.md` | Assessed / Bewertet |
| [security-checklist.md](security-checklist.md) | `security-checklist-template.md` | Assessed / Bewertet |
| [arc42-security.md](arc42-security.md) | `arc42-security-template.md` | Assessed / Bewertet |
| [dependency-audit.md](dependency-audit.md) | `dependency-audit-template.md` | Local complete; remote follow-up / Lokal vollständig; Remote-Folgepflicht |
| [security-quality-scenarios.md](security-quality-scenarios.md) | `security-quality-scenarios-template.md` | Assessed / Bewertet |
| [asvs-verification.md](asvs-verification.md) | `asvs-verification-template.md` | ASVS 5.0 L2 assessed / ASVS 5.0 L2 bewertet |
| [supply-chain-evidence.md](supply-chain-evidence.md) | `supply-chain-evidence-template.md` | Local complete; provider evidence pending / Lokal vollständig; Provider-Evidenz offen |
| [zero-trust-applicability.md](zero-trust-applicability.md) | `zero-trust-applicability-template.md` | Assessed / Bewertet |
| [samm-assessment.md](samm-assessment.md) | `samm-assessment-template.md` | Assessed / Bewertet |

## Feature-002-Abschlussstatus / Feature 002 Closeout Status

DE: Die maschinenlesbare Projektbewertung enthält exakt 157 eindeutige
Kontrollen. 126 sind ehrlich `Open / Not Assessed`; 31 sind begründet
`N/A / Not Assessed`. Jeder offene Datensatz besitzt Owner, Reviewer,
Restrisiko, Maßnahme, Priorität über sein Finding, ISO-Termin und
Neubewertungs-Trigger. Dieser Index behauptet deshalb keine vollständige
Normkonformität. Die lokale Implementierung, Vollregression und Coverage sind
belegt; Axe/Chromium, Provider-SBOM/Provenienz/OpenSSF/Scanlogs, historische
Credential-Disposition und unabhängige Remote-Reviews bleiben vor Release
offen.

EN: The machine-readable project assessment contains exactly 157 unique
controls. 126 honestly remain `Open / Not Assessed`; 31 have reasoned
`N/A / Not Assessed` dispositions. Every open record has an owner, reviewer,
residual risk, action, finding priority, ISO due date, and re-evaluation
trigger. This index therefore makes no claim of full standards compliance.
Local implementation, full regression, and coverage are evidenced;
Axe/Chromium, provider SBOM/provenance/OpenSSF/scan logs, historical credential
disposition, and independent remote reviews remain open before release.

## Anwendungshinweise / Usage Notes

- ADRs werden im Verzeichnis `docs/security/adr/` als einzelne Dateien abgelegt.
- Die Templates in `.specify/templates/` sind die kanonische Quelle.
- Default-Evidenzort fuer neue Standards: `docs/security/`.
- Falls dieses Repo einen gleichwertigen Governance-Pfad statt `docs/security/`
  nutzt, muss diese Abweichung hier dokumentiert und verlinkt werden.

*ADRs are stored as individual files in `docs/security/adr/`.
The templates in `.specify/templates/` are the canonical source.
The default evidence location for newly added standards is `docs/security/`.
If this repository uses an equivalent governance path instead, that deviation
must be documented and linked here.*

<!-- EN: docs/security/README.md
[DE-Zusammenfassung: Index der projektspezifischen Sicherheitsdokumentation fuer InventarWorkerService.]
-->
