# render-requirements-intake-governance(1)

## NAME

`render-requirements-intake-governance.sh`,
`render-requirements-intake-governance.ps1` - rendert verlinkte
Intake-Reihenfolgen. *Renders linked intake-order views.*

## SYNOPSIS

```bash
bash scripts/render-requirements-intake-governance.sh [--check-only|--dry-run]
bash scripts/render-requirements-intake-governance.sh --write
```

```powershell
pwsh -NoProfile -File scripts/render-requirements-intake-governance.ps1
pwsh -NoProfile -File scripts/render-requirements-intake-governance.ps1 -Write
```

## DESCRIPTION

Der Renderer liest das kanonische Inventar-Serienmanifest und erzeugt aus
einer gemeinsamen typisierten Projektion die Root- und Series-Ansicht. Jede
Zeile enthaelt Position, Status, vollstaendigen Intake-Link, direkte
Abhaengigkeiten und den belegten Spec-Kit-Featurepfad.

*The renderer reads the canonical Inventar series manifest and creates the
root and series views from one shared typed projection. Every row contains the
position, status, complete intake link, direct dependencies, and proven Spec
Kit feature path.*

Der Standard ist ein schreibfreier Check. Der Schreibmodus ersetzt nur die
markierten Rendererbereiche und publiziert mehrere Ausgaben atomar. Unsichere
Pfade, mehrdeutige Nachweise, Drift und Transaktionsfehler werden mit
`LIE001` bis `LIE012` gemeldet.

*The default is a zero-write check. Write mode replaces only renderer-owned
marker sections and publishes multiple outputs atomically. Unsafe paths,
ambiguous evidence, drift, and transaction failures use diagnostics `LIE001`
through `LIE012`.*

## OPTIONS

| Bash | PowerShell | Bedeutung / Meaning |
| --- | --- | --- |
| `--check-only`, `--dry-run` | Standard, `-WhatIf` | Ohne Schreiben pruefen / check without writes |
| `--write` | `-Write` | Markierte Ansichten publizieren / publish marked views |
| `--repo PATH` | `-Repo PATH` | Explizites Git-Repository / explicit Git repository |
| `--manifest PATH` | `-Manifest PATH` | Repositoryrelatives Manifest / repository-relative manifest |
| `--output PATH` | `-OutputPath PATH` | Repositoryrelative Ausgabe / repository-relative output |
| `--help` | `-Help` | Bilinguale Kurzhilfe / bilingual short help |

## SAFETY AND ACCESSIBILITY

Alle Manifest-, Intake-, Feature- und Ausgabepfade bleiben nach logischer und
physischer Aufloesung im Zielrepository. Diagnosen redigieren private Pfade und
zugangsdatenaehnliche Werte. Die Markdown-Tabelle ist textorientiert,
screenreadergeeignet und benoetigt weder Farbe noch grafische Symbole.

*All manifest, intake, feature, and output paths remain inside the target
repository after logical and physical resolution. Diagnostics redact private
paths and credential-shaped values. The Markdown table is text-first,
screen-reader usable, and requires neither color nor graphical symbols.*

## EXIT STATUS

- `0`: Ansicht aktuell oder erfolgreich publiziert. / View current or published successfully.
- `1`-`12`: Passende `LIE`-Vertragsabweichung. / Matching `LIE` contract failure.

## SEE ALSO

`test-requirements-intake-governance(1)`
