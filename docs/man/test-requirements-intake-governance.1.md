# test-requirements-intake-governance(1)

## NAME

`test-requirements-intake-governance.sh`,
`test-requirements-intake-governance.ps1` - prueft den Vertrag verlinkter
Intake-Reihenfolgen. *Tests the linked intake-order contract.*

## SYNOPSIS

```bash
bash scripts/test-requirements-intake-governance.sh
```

```powershell
pwsh -NoProfile -File scripts/test-requirements-intake-governance.ps1
Get-Help ./scripts/test-requirements-intake-governance.ps1 -Full
```

## DESCRIPTION

Beide Tests erzeugen isolierte temporaere Git-Repositories und pruefen die
echten Rendereroberflaechen. Abgedeckt sind die fuenf Spalten, sichere Links,
Abhaengigkeitskanten, Feature-Nachweise, `LIE001` bis `LIE012`, atomarer
Rollback, Idempotenz, Markdown-Escaping, LF-Ausgabe sowie Bash-/PowerShell-
Paritaet.

*Both tests create isolated temporary Git repositories and exercise the real
renderer surfaces. Coverage includes the five columns, safe links, dependency
edges, feature evidence, `LIE001` through `LIE012`, atomic rollback,
idempotence, Markdown escaping, LF output, and Bash/PowerShell parity.*

## SAFETY

Die Tests schreiben nur in betriebssystemseitig erzeugte
Temporaerverzeichnisse und entfernen diese wieder. Das Arbeitsrepository,
sein Index und seine Ansichten bleiben unveraendert.

*The tests write only to operating-system-created temporary directories and
remove them afterwards. The working repository, its index, and its views stay
unchanged.*

## EXIT STATUS

- `0`: Alle Vertragsassertions sind erfuellt. / All contract assertions pass.
- `1`: Mindestens eine Assertion ist fehlgeschlagen. / At least one assertion failed.
- `2`: Der Bash-Aufruf enthielt eine unbekannte Option. / The Bash invocation used an unknown option.

## SEE ALSO

`render-requirements-intake-governance(1)`
