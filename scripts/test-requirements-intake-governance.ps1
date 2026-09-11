<#
.SYNOPSIS
Prueft den Vertrag fuer verlinkte Intake-Reihenfolgen.

Tests the linked intake-order contract.

.DESCRIPTION
Erzeugt aus den akzeptierten positiven und negativen Fixtures isolierte
temporaere Repositories. Geprueft werden die echte Projektion, LIE001-LIE012,
alle Feature-Proof-Arten, Check/Write, Source-/Containment-Recheck, atomarer
Multi-Output-Rollback, Idempotenz, Markdown-Escaping und stabile LF-Ausgabe.
Das Zielrepository wird nicht veraendert.

Creates isolated temporary repositories from the accepted positive and
negative fixtures. It tests the real projection, LIE001-LIE012, all feature-
proof kinds, check/write, source and containment rechecks, atomic multi-output
rollback, idempotence, Markdown escaping, and stable LF output. The target
repository is not changed.

.EXAMPLE
pwsh -NoProfile -File scripts/test-requirements-intake-governance.ps1

.EXAMPLE
Get-Help ./scripts/test-requirements-intake-governance.ps1 -Full
#>

[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Test-RequirementsIntakeGovernance {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [ValidateScript({ Test-Path -LiteralPath $_ -PathType Container })]
        [string]$RepositoryRoot,

        [Parameter(Mandatory)]
        [ValidateScript({ Test-Path -LiteralPath $_ -PathType Container })]
        [string]$FixtureDirectory
    )

    $library = Join-Path $RepositoryRoot 'scripts/render-requirements-intake-governance.ps1'
    if (-not (Test-Path -LiteralPath $library -PathType Leaf)) {
        throw "Bibliothek fehlt / library is missing: ${library}"
    }
    . $library

    function Set-FixtureManifestHashes {
        param([string]$Repo, [string]$ManifestPath)

        $value = Get-Content -LiteralPath $ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
        foreach ($target in $value.orderedTargets) {
            $digest = (Get-FileHash -LiteralPath (Join-Path $Repo ([string]$target.path)) -Algorithm SHA256).Hash.ToLowerInvariant()
            $target.normalizedSha256 = $digest
        }
        [IO.File]::WriteAllText($ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
    }

    $fixtureRepo = Join-Path ([IO.Path]::GetTempPath()) ("linked-intake-{0}" -f [guid]::NewGuid())
    New-Item -ItemType Directory -Path $fixtureRepo | Out-Null
    try {
        $casesFile = Join-Path $FixtureDirectory 'cases.json'
        $expectedFile = Join-Path $FixtureDirectory 'expected-root.md'
        $cases = Get-Content -LiteralPath $casesFile -Raw -Encoding UTF8 | ConvertFrom-Json

        $manifest = [ordered]@{
            schemaVersion = [string]$cases.schemaVersion
            documentType = 'IntakeSeriesManifest'
            seriesId = 'linked-intake-test'
            status = 'Active'
            orderedTargets = @($cases.entries | ForEach-Object {
                [ordered]@{ path = [string]$_.intakePath; role = [string]$_.role; normalizedSha256 = ''; status = [string]$_.status }
            })
            roots = @($cases.entries | Where-Object { @($_.incomingDependencies).Count -eq 0 } | ForEach-Object { [string]$_.intakePath })
            dependencies = @($cases.entries | ForEach-Object { @($_.incomingDependencies) })
        }
        $manifestFile = Join-Path $fixtureRepo 'requirements/intakes/series/home-baseline-delivery/manifest.json'
        New-Item -ItemType Directory -Path (Split-Path -Parent $manifestFile) -Force | Out-Null
        [IO.File]::WriteAllText(
            $manifestFile,
            (($manifest | ConvertTo-Json -Depth 12) + "`n"),
            [Text.UTF8Encoding]::new($false)
        )

        foreach ($entry in $cases.entries) {
            $intakeFile = Join-Path $fixtureRepo ([string]$entry.intakePath)
            New-Item -ItemType Directory -Path (Split-Path -Parent $intakeFile) -Force | Out-Null
            $intakeContent = "# Fixture`n`n**Reihenfolge:** sichtbare Position $($entry.displayPosition)`n"
            [IO.File]::WriteAllText($intakeFile, $intakeContent, [Text.UTF8Encoding]::new($false))
        }
        Set-FixtureManifestHashes -Repo $fixtureRepo -ManifestPath $manifestFile

        $featureDirectory = Join-Path $fixtureRepo 'specs/032-linked-intake-evidence'
        New-Item -ItemType Directory -Path $featureDirectory -Force | Out-Null
        $featureSpec = @'
# Fixture Feature

**Binding Input / Bindende Eingabe**: `Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md`
'@
        [IO.File]::WriteAllText(
            (Join-Path $featureDirectory 'spec.md'),
            ($featureSpec.TrimEnd() + "`n"),
            [Text.UTF8Encoding]::new($false)
        )

        function Get-ContractTable {
            param([string]$Section)

            $lines = [regex]::Split($Section, '\r?\n')
            $headerIndex = [Array]::FindIndex(
                $lines,
                [Predicate[string]] { param($line) $line.StartsWith('| Position | Status | Lastenheft/Intake |', [StringComparison]::Ordinal) }
            )
            $tableLines = [System.Collections.Generic.List[string]]::new()
            if ($headerIndex -ge 0) {
                for ($index = $headerIndex; $index -lt $lines.Count -and $lines[$index].StartsWith('|', [StringComparison]::Ordinal); $index++) {
                    $tableLines.Add($lines[$index])
                }
            }
            if ($tableLines.Count -eq 0) { return '' }
            return ($tableLines -join "`n") + "`n"
        }

        $section = Get-SdhOrderSection -Repo $fixtureRepo
        $actualTable = Get-ContractTable -Section ([string]$section)
        $seriesSection = Get-SdhOrderSection `
            -Repo $fixtureRepo `
            -ViewPath 'requirements/intakes/series/home-baseline-delivery/order.md'
        $actualSeriesTable = Get-ContractTable -Section ([string]$seriesSection)

        $failures = [System.Collections.Generic.List[string]]::new()
        function Assert-ContractText {
            param([string]$Description, [string]$Expected)
            if (-not $actualTable.Contains($Expected, [StringComparison]::Ordinal)) {
                $failures.Add("${Description}: ${Expected}")
            }
        }

        Assert-ContractText 'fuenf Spalten in Vertragsreihenfolge / five columns in contract order' '| Position | Status | Lastenheft/Intake | Abhängigkeiten / Dependencies | Spec-Kit-Feature |'
        Assert-ContractText 'vollstaendiger Intake-Dateiname / complete intake filename' '[Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md](Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md)'
        Assert-ContractText 'Root-Fallback / root fallback' '— (Root / keine direkte Abhängigkeit)'
        Assert-ContractText 'bindende direkte Kante / binding direct edge' '`HardCompletionGate`, binding: true'
        Assert-ContractText 'beratende direkte Kante / advisory direct edge' '`PreferredSerialOrder`, binding: false'
        Assert-ContractText 'eindeutig belegtes Feature / uniquely proven feature' '[032-linked-intake-evidence](specs/032-linked-intake-evidence/)'
        Assert-ContractText 'exakter Feature-Fallback / exact feature fallback' '— (kein Spec-Kit-Feature / no Spec Kit feature)'
        Assert-ContractText 'Metadatenposition 39 trotz Manifestplatz 1 / metadata position 39 despite manifest slot 1' '| 39 | Completed |'

        $expectedTable = Get-Content -LiteralPath $expectedFile -Raw -Encoding UTF8
        if ($actualTable -cne $expectedTable) {
            $failures.Add('gerenderte Root-Tabelle weicht vom positiven Vertrag ab / rendered root table differs from the positive contract')
        }
        $expectedSeriesTable = Get-Content -LiteralPath (Join-Path $FixtureDirectory 'expected-series.md') -Raw -Encoding UTF8
        if ($actualSeriesTable -cne $expectedSeriesTable) {
            $failures.Add('view-relative Series-Tabelle weicht vom positiven Vertrag ab / view-relative series table differs from the positive contract')
        }

        $negativeFixture = Join-Path $RepositoryRoot 'scripts/tests/linked-intake-evidence/negative/cases.json'
        $negativeCases = Get-Content -LiteralPath $negativeFixture -Raw -Encoding UTF8 | ConvertFrom-Json
        $actualCodes = @($negativeCases.cases.expectedDiagnostic | Sort-Object -Unique) -join ' '
        $expectedCodes = 'LIE001 LIE002 LIE003 LIE004 LIE005 LIE006 LIE007 LIE008 LIE009 LIE010 LIE011 LIE012'
        if ($actualCodes -cne $expectedCodes) {
            $failures.Add("Diagnostikfamilien unvollstaendig / diagnostic families incomplete: ${actualCodes}")
        }
        $linearDirectory = Join-Path $RepositoryRoot 'scripts/tests/linked-intake-evidence/accessibility/linearized'
        $linearFiles = @(Get-ChildItem -LiteralPath $linearDirectory -File -Filter '*.txt' | Sort-Object Name)
        if ($linearFiles.Count -ne 5) {
            $failures.Add('linearisierte A11Y-Referenzen sind unvollstaendig / linearized accessibility references are incomplete')
        }
        $orderedLabels = @(
            'Position:',
            'Status:',
            'Lastenheft/Intake:',
            'Abhängigkeiten / Dependencies:',
            'Spec-Kit-Feature:',
            'Nächste Aktion / Next action:'
        )
        foreach ($linearFile in $linearFiles) {
            $linearText = [IO.File]::ReadAllText($linearFile.FullName, [Text.UTF8Encoding]::new($false))
            $previousIndex = -1
            foreach ($label in $orderedLabels) {
                $currentIndex = $linearText.IndexOf($label, [StringComparison]::Ordinal)
                if ($currentIndex -le $previousIndex) {
                    $failures.Add("A11Y-Source-Order fehlt / accessibility source order is missing: $($linearFile.Name) (${label})")
                    break
                }
                $previousIndex = $currentIndex
            }
            if ($linearText.IndexOf([char]0x1b) -ge 0) {
                $failures.Add("A11Y-Referenz enthaelt Farbsteuerung / accessibility reference contains colour control: $($linearFile.Name)")
            }
        }

        $diagnosticPath = Join-Path $RepositoryRoot 'scripts/tests/linked-intake-evidence/accessibility/diagnostics/reference.json'
        $diagnostics = Get-Content -LiteralPath $diagnosticPath -Raw -Encoding UTF8 | ConvertFrom-Json
        $diagnosticCodes = @($diagnostics.cases.code | Sort-Object -Unique) -join ' '
        if ((@($diagnostics.languageOrder) -join ' ') -cne 'de en' -or
            @($diagnostics.cases).Count -ne 12 -or
            $diagnosticCodes -cne $expectedCodes) {
            $failures.Add('bilinguale LIE001-LIE012-Referenzen sind unvollstaendig / bilingual LIE001-LIE012 references are incomplete')
        }
        foreach ($diagnostic in $diagnostics.cases) {
            $combined = @($diagnostic.subject, $diagnostic.de, $diagnostic.en, $diagnostic.remediationDe, $diagnostic.remediationEn) -join ' '
            if ([string]::IsNullOrWhiteSpace([string]$diagnostic.de) -or
                [string]::IsNullOrWhiteSpace([string]$diagnostic.en) -or
                [string]::IsNullOrWhiteSpace([string]$diagnostic.remediationDe) -or
                [string]::IsNullOrWhiteSpace([string]$diagnostic.remediationEn) -or
                (([string]$diagnostic.subject).StartsWith('/', [StringComparison]::Ordinal)) -or
                $combined -match ('(?i)/Users/|token=|pass' + 'word=|secret=|CategoryInfo|ScriptStackTrace')) {
                $failures.Add("Diagnostik ist nicht sicher redigiert / diagnostic is not safely redacted: $($diagnostic.code)")
            }
        }
        $featureMatrix = Get-Content -LiteralPath (Join-Path $RepositoryRoot 'scripts/tests/linked-intake-evidence/feature-proof-matrix.json') -Raw -Encoding UTF8 | ConvertFrom-Json
        if (@($featureMatrix.cases).Count -ne 7) {
            $failures.Add('Feature-Proof-Matrix ist unvollstaendig / feature proof matrix is incomplete')
        }

        function New-TransactionFixture {
            param([string]$CaseId)

            $caseRepo = Join-Path $fixtureRepo "cases/$CaseId"
            $manifestRelative = 'requirements/intakes/series/home-baseline-delivery/manifest.json'
            $manifestPath = Join-Path $caseRepo $manifestRelative
            New-Item -ItemType Directory -Path (Split-Path -Parent $manifestPath) -Force | Out-Null
            New-Item -ItemType Directory -Path (Join-Path $caseRepo '.git') -Force | Out-Null
            [IO.File]::WriteAllText((Join-Path $caseRepo '.sdh-linked-intake-test-fixture'), '', [Text.UTF8Encoding]::new($false))
            $caseManifest = [ordered]@{
                schemaVersion = [string]$cases.schemaVersion
                documentType = 'IntakeSeriesManifest'
                seriesId = 'linked-intake-test'
                status = 'Active'
                orderedTargets = @($cases.entries | ForEach-Object {
                    [ordered]@{ path = [string]$_.intakePath; role = [string]$_.role; normalizedSha256 = ''; status = [string]$_.status }
                })
                roots = @($cases.entries | Where-Object { @($_.incomingDependencies).Count -eq 0 } | ForEach-Object { [string]$_.intakePath })
                dependencies = @($cases.entries | ForEach-Object { @($_.incomingDependencies) })
            }
            [IO.File]::WriteAllText($manifestPath, (($caseManifest | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
            foreach ($entry in $cases.entries) {
                $intakePath = Join-Path $caseRepo ([string]$entry.intakePath)
                New-Item -ItemType Directory -Path (Split-Path -Parent $intakePath) -Force | Out-Null
                [IO.File]::WriteAllText($intakePath, "# Fixture`n`n**Reihenfolge:** sichtbare Position $($entry.displayPosition)`n", [Text.UTF8Encoding]::new($false))
            }
            Set-FixtureManifestHashes -Repo $caseRepo -ManifestPath $manifestPath
            $featureDirectory = Join-Path $caseRepo 'specs/032-linked-intake-evidence'
            New-Item -ItemType Directory -Path $featureDirectory -Force | Out-Null
            [IO.File]::WriteAllText(
                (Join-Path $featureDirectory 'spec.md'),
                "# Fixture Feature`n`n**Binding Input / Bindende Eingabe**: ``Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md```n",
                [Text.UTF8Encoding]::new($false)
            )
            return [pscustomobject]@{
                Repo = $caseRepo
                ManifestRelative = $manifestRelative
                ManifestPath = $manifestPath
                RootOutput = 'Lastenheft_Abarbeitungsreihenfolge.md'
                SeriesOutput = 'requirements/intakes/series/home-baseline-delivery/order.md'
            }
        }

        function Invoke-ProjectionCase {
            param(
                [pscustomobject]$Fixture,
                [ValidateSet('Check', 'Write')][string]$Mode,
                [string[]]$Outputs,
                [string]$Fault = '',
                [string]$VanishPath = ''
            )

            $oldFault = $env:SDH_TEST_FAULT
            $oldVanish = $env:SDH_TEST_VANISH_PATH
            try {
                $env:SDH_TEST_FAULT = $Fault
                $env:SDH_TEST_VANISH_PATH = $VanishPath
                try {
                    $result = Invoke-SdhLinkedIntakeProjection -Repo $Fixture.Repo -ManifestPath $Fixture.ManifestRelative -Mode $Mode -OutputPaths $Outputs
                    return [pscustomobject]@{ ExitClass = 'zero'; ExitCode = 0; Output = [string]$result; Writes = [int]$script:SdhRenderWriteCount }
                } catch {
                    return [pscustomobject]@{ ExitClass = 'nonzero'; ExitCode = 1; Output = [string]$_.Exception.Message; Writes = [int]$script:SdhRenderWriteCount }
                }
            } finally {
                $env:SDH_TEST_FAULT = $oldFault
                $env:SDH_TEST_VANISH_PATH = $oldVanish
            }
        }

        function Assert-ProjectionCase {
            param(
                [string]$CaseId,
                [pscustomobject]$Actual,
                [string]$ExpectedCode,
                [ValidateSet('zero', 'nonzero')][string]$ExpectedExitClass,
                [int]$ExpectedWrites
            )

            if ($Actual.ExitClass -cne $ExpectedExitClass `
                -or $Actual.Writes -ne $ExpectedWrites `
                -or ($ExpectedCode -and -not $Actual.Output.Contains($ExpectedCode, [StringComparison]::Ordinal))) {
                $failures.Add("${CaseId}: expected code=${ExpectedCode}, exit=${ExpectedExitClass}, writes=${ExpectedWrites}; actual exit=$($Actual.ExitCode), writes=$($Actual.Writes), output=$($Actual.Output)")
            }
        }

        if (-not (Get-Command Invoke-SdhLinkedIntakeProjection -CommandType Function -ErrorAction SilentlyContinue)) {
            $failures.Add('oeffentliche Check-/Write-Transaktionsfunktion fehlt / public check/write transaction function is missing')
        } else {
            $transaction = New-TransactionFixture -CaseId transaction
            $actual = Invoke-ProjectionCase -Fixture $transaction -Mode Write -Outputs @($transaction.RootOutput, $transaction.SeriesOutput)
            Assert-ProjectionCase -CaseId 'write-update' -Actual $actual -ExpectedCode '' -ExpectedExitClass zero -ExpectedWrites 2
            $rootSnapshot = Join-Path $fixtureRepo 'powershell-root-before-second-write.md'
            $seriesSnapshot = Join-Path $fixtureRepo 'powershell-series-before-second-write.md'
            [IO.File]::Copy((Join-Path $transaction.Repo $transaction.RootOutput), $rootSnapshot, $true)
            [IO.File]::Copy((Join-Path $transaction.Repo $transaction.SeriesOutput), $seriesSnapshot, $true)
            $actual = Invoke-ProjectionCase -Fixture $transaction -Mode Write -Outputs @($transaction.RootOutput, $transaction.SeriesOutput)
            Assert-ProjectionCase -CaseId 'second-unchanged-write' -Actual $actual -ExpectedCode '' -ExpectedExitClass zero -ExpectedWrites 0
            & git diff --no-index --exit-code -- $rootSnapshot (Join-Path $transaction.Repo $transaction.RootOutput) *> $null
            $rootDiffExit = $LASTEXITCODE
            & git diff --no-index --exit-code -- $seriesSnapshot (Join-Path $transaction.Repo $transaction.SeriesOutput) *> $null
            $seriesDiffExit = $LASTEXITCODE
            if ($rootDiffExit -ne 0 -or $seriesDiffExit -ne 0) {
                $failures.Add('zweiter PowerShell-Write ist nicht byteidentisch / second PowerShell write is not byte-identical')
            }
            $actual = Invoke-ProjectionCase -Fixture $transaction -Mode Check -Outputs @($transaction.RootOutput, $transaction.SeriesOutput)
            Assert-ProjectionCase -CaseId 'check-current' -Actual $actual -ExpectedCode '' -ExpectedExitClass zero -ExpectedWrites 0

            $rootPath = Join-Path $transaction.Repo $transaction.RootOutput
            $rootContent = [IO.File]::ReadAllText($rootPath, [Text.UTF8Encoding]::new($false)).Replace('| 39 | Completed |', '| 39 | Broken |')
            [IO.File]::WriteAllText($rootPath, $rootContent, [Text.UTF8Encoding]::new($false))
            $actual = Invoke-ProjectionCase -Fixture $transaction -Mode Check -Outputs @($transaction.RootOutput)
            Assert-ProjectionCase -CaseId 'stale-check-output' -Actual $actual -ExpectedCode LIE009 -ExpectedExitClass nonzero -ExpectedWrites 0
            $actual = Invoke-ProjectionCase -Fixture $transaction -Mode Write -Outputs @($transaction.RootOutput)
            Assert-ProjectionCase -CaseId 'write-repair' -Actual $actual -ExpectedCode '' -ExpectedExitClass zero -ExpectedWrites 1

            $rootContent = [IO.File]::ReadAllText($rootPath, [Text.UTF8Encoding]::new($false)).Replace('| 39 | Completed |', '| 39 | Broken |')
            [IO.File]::WriteAllText($rootPath, $rootContent, [Text.UTF8Encoding]::new($false))
            $actual = Invoke-ProjectionCase -Fixture $transaction -Mode Write -Outputs @($transaction.RootOutput) -Fault source-drift
            Assert-ProjectionCase -CaseId 'source-hash-recheck' -Actual $actual -ExpectedCode LIE010 -ExpectedExitClass nonzero -ExpectedWrites 0

            $inputDrift = New-TransactionFixture -CaseId input-drift
            $actual = Invoke-ProjectionCase -Fixture $inputDrift -Mode Write -Outputs @($inputDrift.RootOutput) -Fault input-drift -VanishPath 'requirements/intakes/active/Lastenheft_Einzelkante.md'
            Assert-ProjectionCase -CaseId 'complete-input-hash-recheck' -Actual $actual -ExpectedCode LIE010 -ExpectedExitClass nonzero -ExpectedWrites 0

            $inputOverlap = New-TransactionFixture -CaseId input-overlap
            $manifestHashBefore = (Get-FileHash -LiteralPath $inputOverlap.ManifestPath -Algorithm SHA256).Hash
            $actual = Invoke-ProjectionCase -Fixture $inputOverlap -Mode Write -Outputs @($inputOverlap.ManifestRelative)
            Assert-ProjectionCase -CaseId 'canonical-input-output-overlap' -Actual $actual -ExpectedCode LIE006 -ExpectedExitClass nonzero -ExpectedWrites 0
            $manifestHashAfter = (Get-FileHash -LiteralPath $inputOverlap.ManifestPath -Algorithm SHA256).Hash
            if ($manifestHashBefore -cne $manifestHashAfter) {
                $failures.Add('Overlap-Ablehnung veraenderte kanonische Eingabe / overlap rejection changed canonical input')
            }

            $missingLogicalOverlap = New-TransactionFixture -CaseId missing-logical-overlap
            $overlapManifest = Get-Content -LiteralPath $missingLogicalOverlap.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
            $logicalOverlapPath = [string]$overlapManifest.orderedTargets[1].path
            $stampedOverlapPath = $logicalOverlapPath -replace '\.md$', '.123-output-overlap.md'
            Move-Item -LiteralPath (Join-Path $missingLogicalOverlap.Repo $logicalOverlapPath) -Destination (Join-Path $missingLogicalOverlap.Repo $stampedOverlapPath)
            $stampedOverlapHash = (Get-FileHash -LiteralPath (Join-Path $missingLogicalOverlap.Repo $stampedOverlapPath) -Algorithm SHA256).Hash.ToLowerInvariant()
            $overlapStateDirectory = Join-Path $missingLogicalOverlap.Repo 'specs/123-output-overlap'
            New-Item -ItemType Directory -Path $overlapStateDirectory -Force | Out-Null
            $overlapState = [ordered]@{
                status = 'Completed'
                closeout = [ordered]@{ mergeOrPublication = 'Completed'; defaultBranchSync = 'Completed'; finalValidation = 'Completed' }
                acceptedArtifacts = @([ordered]@{ path = $stampedOverlapPath; sha256 = $stampedOverlapHash })
            }
            [IO.File]::WriteAllText(
                (Join-Path $overlapStateDirectory 'autonomous-run-state.json'),
                (($overlapState | ConvertTo-Json -Depth 8) + "`n"),
                [Text.UTF8Encoding]::new($false)
            )
            $actual = Invoke-ProjectionCase -Fixture $missingLogicalOverlap -Mode Write -Outputs @($logicalOverlapPath)
            Assert-ProjectionCase -CaseId 'missing-logical-input-output-overlap' -Actual $actual -ExpectedCode LIE006 -ExpectedExitClass nonzero -ExpectedWrites 0
            if (Test-Path -LiteralPath (Join-Path $missingLogicalOverlap.Repo $logicalOverlapPath)) {
                $failures.Add('Overlap-Ablehnung erzeugte den fehlenden logischen Intake / overlap rejection created the missing logical intake')
            }

            $vanished = New-TransactionFixture -CaseId vanished
            $actual = Invoke-ProjectionCase -Fixture $vanished -Mode Write -Outputs @($vanished.RootOutput) -Fault vanish-target -VanishPath 'requirements/intakes/active/Lastenheft_Einzelkante.md'
            Assert-ProjectionCase -CaseId 'vanished-target-recheck' -Actual $actual -ExpectedCode LIE004 -ExpectedExitClass nonzero -ExpectedWrites 0

            $containment = New-TransactionFixture -CaseId containment
            $actual = Invoke-ProjectionCase -Fixture $containment -Mode Write -Outputs @($containment.RootOutput) -Fault containment-drift -VanishPath 'requirements/intakes/active/Lastenheft_Einzelkante.md'
            Assert-ProjectionCase -CaseId 'target-containment-recheck' -Actual $actual -ExpectedCode LIE005 -ExpectedExitClass nonzero -ExpectedWrites 0

            $rollback = New-TransactionFixture -CaseId rollback
            $actual = Invoke-ProjectionCase -Fixture $rollback -Mode Write -Outputs @($rollback.RootOutput, $rollback.SeriesOutput)
            Assert-ProjectionCase -CaseId 'rollback-baseline-write' -Actual $actual -ExpectedCode '' -ExpectedExitClass zero -ExpectedWrites 2
            $rootBefore = (Get-FileHash -LiteralPath (Join-Path $rollback.Repo $rollback.RootOutput) -Algorithm SHA256).Hash
            $seriesBefore = (Get-FileHash -LiteralPath (Join-Path $rollback.Repo $rollback.SeriesOutput) -Algorithm SHA256).Hash
            $rollbackManifest = Get-Content -LiteralPath $rollback.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
            $rollbackManifest.orderedTargets[0].status = 'Eligible'
            [IO.File]::WriteAllText($rollback.ManifestPath, (($rollbackManifest | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
            $actual = Invoke-ProjectionCase -Fixture $rollback -Mode Write -Outputs @($rollback.RootOutput, $rollback.SeriesOutput) -Fault after-first-replace
            Assert-ProjectionCase -CaseId 'simulated-publish-failure' -Actual $actual -ExpectedCode LIE010 -ExpectedExitClass nonzero -ExpectedWrites 0
            $rootAfter = (Get-FileHash -LiteralPath (Join-Path $rollback.Repo $rollback.RootOutput) -Algorithm SHA256).Hash
            $seriesAfter = (Get-FileHash -LiteralPath (Join-Path $rollback.Repo $rollback.SeriesOutput) -Algorithm SHA256).Hash
            if ($rootBefore -cne $rootAfter -or $seriesBefore -cne $seriesAfter) {
                $failures.Add('atomarer Rollback stellte den vollstaendigen Altzustand nicht her / atomic rollback did not restore the complete prior state')
            }
            if (@(Get-ChildItem -LiteralPath $rollback.Repo -Recurse -File -Filter '.sdh-*-*.tmp' -ErrorAction SilentlyContinue).Count -gt 0) {
                $failures.Add('Rollback hinterliess renderer-owned temporaere Dateien / rollback left renderer-owned temporary files')
            }

            $drift = New-TransactionFixture -CaseId root-series-drift
            $actual = Invoke-ProjectionCase -Fixture $drift -Mode Write -Outputs @($drift.RootOutput, $drift.SeriesOutput)
            Assert-ProjectionCase -CaseId 'root-series-drift-baseline' -Actual $actual -ExpectedCode '' -ExpectedExitClass zero -ExpectedWrites 2
            $seriesPath = Join-Path $drift.Repo $drift.SeriesOutput
            $seriesContent = [IO.File]::ReadAllText($seriesPath, [Text.UTF8Encoding]::new($false)).Replace('| 39 | Completed |', '| 39 | Broken |')
            [IO.File]::WriteAllText($seriesPath, $seriesContent, [Text.UTF8Encoding]::new($false))
            $actual = Invoke-ProjectionCase -Fixture $drift -Mode Check -Outputs @($drift.RootOutput, $drift.SeriesOutput)
            Assert-ProjectionCase -CaseId 'root-series-drift' -Actual $actual -ExpectedCode LIE011 -ExpectedExitClass nonzero -ExpectedWrites 0

            $escaping = New-TransactionFixture -CaseId escaping
            $escapingManifest = Get-Content -LiteralPath $escaping.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
            $escapingManifest.orderedTargets[0].status = '<status data-safe="yes">Com|pleted & Ready</status> [x](y)\z'
            [IO.File]::WriteAllText($escaping.ManifestPath, (($escapingManifest | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
            $actual = Invoke-ProjectionCase -Fixture $escaping -Mode Write -Outputs @($escaping.RootOutput, $escaping.SeriesOutput)
            Assert-ProjectionCase -CaseId 'markdown-escaping' -Actual $actual -ExpectedCode '' -ExpectedExitClass zero -ExpectedWrites 2
            $escapedRoot = [IO.File]::ReadAllText((Join-Path $escaping.Repo $escaping.RootOutput), [Text.UTF8Encoding]::new($false))
            if (-not $escapedRoot.Contains('&lt;status data-safe="yes"&gt;Com\|pleted &amp; Ready&lt;/status&gt; \[x\]\(y\)\\z', [StringComparison]::Ordinal) `
                -or -not $escapedRoot.Contains('<br>', [StringComparison]::Ordinal) `
                -or $escapedRoot.Contains('<status', [StringComparison]::Ordinal) `
                -or $escapedRoot.Contains('<kind>', [StringComparison]::Ordinal)) {
                $failures.Add('nicht vertrauenswuerdige Tabellenfelder sind nicht vollstaendig escaped / untrusted table fields are not fully escaped')
            }

            $publicCli = Join-Path $RepositoryRoot 'scripts/render-requirements-intake-governance.ps1'
            function Invoke-PublicCliCase {
                param([string[]]$Arguments)

                $startInfo = [Diagnostics.ProcessStartInfo]::new()
                $startInfo.FileName = (Get-Command pwsh -ErrorAction Stop).Source
                $startInfo.UseShellExecute = $false
                $startInfo.RedirectStandardOutput = $true
                $startInfo.RedirectStandardError = $true
                foreach ($argument in @('-NoProfile', '-File', $publicCli) + $Arguments) {
                    $null = $startInfo.ArgumentList.Add($argument)
                }
                $process = [Diagnostics.Process]::new()
                $process.StartInfo = $startInfo
                $null = $process.Start()
                $stdoutTask = $process.StandardOutput.ReadToEndAsync()
                $stderrTask = $process.StandardError.ReadToEndAsync()
                $process.WaitForExit()
                return [pscustomobject]@{
                    ExitCode = $process.ExitCode
                    Stdout = $stdoutTask.GetAwaiter().GetResult()
                    Stderr = $stderrTask.GetAwaiter().GetResult()
                }
            }

            function Assert-PublicDiagnostic {
                param(
                    [string]$CaseId,
                    [pscustomobject]$Actual,
                    [string]$ExpectedCode,
                    [string]$ExpectedSafeSubject,
                    [string]$ForbiddenValue = ''
                )

                $combined = $Actual.Stdout + "`n" + $Actual.Stderr
                if ($Actual.ExitCode -eq 0 `
                    -or -not $Actual.Stderr.Contains($ExpectedCode, [StringComparison]::Ordinal) `
                    -or -not $Actual.Stderr.Contains($ExpectedSafeSubject, [StringComparison]::Ordinal) `
                    -or -not $Actual.Stderr.Contains('Abhilfe / remediation:', [StringComparison]::Ordinal) `
                    -or $Actual.Stdout.Contains($ExpectedCode, [StringComparison]::Ordinal) `
                    -or ($ForbiddenValue -and $combined.Contains($ForbiddenValue, [StringComparison]::Ordinal)) `
                    -or $combined -match '(?i)/Users/|/home/|CategoryInfo|FullyQualifiedErrorId|ScriptStackTrace|at .*prepare-secure-development-hardening' `
                    -or $combined -match '[\x00-\x08\x0b\x0c\x0e-\x1f\x7f]') {
                    $failures.Add("oeffentliche PowerShell-Diagnostik ist nicht sicher: ${CaseId} / public PowerShell diagnostic is unsafe: ${CaseId}")
                }
            }

            $hostileManifest = ('/Us' + "ers/synthetic-private/credentials/token=not-a-real-secret`nmanifest.json")
            $publicResult = Invoke-PublicCliCase -Arguments @(
                '-Repo', $escaping.Repo, '-OrderOnly', '-Manifest', $hostileManifest,
                '-OrderOutput', $escaping.RootOutput, '-WhatIf'
            )
            Assert-PublicDiagnostic -CaseId public-lie003 -Actual $publicResult `
                -ExpectedCode LIE003 -ExpectedSafeSubject repository-relative -ForbiddenValue 'token=not-a-real-secret'

            $publicResult = Invoke-PublicCliCase -Arguments @(
                '-Repo', $escaping.Repo, '-OrderOnly', '-Manifest', 'missing-manifest.md',
                '-OrderOutput', $escaping.RootOutput, '-WhatIf'
            )
            Assert-PublicDiagnostic -CaseId public-lie004-safe -Actual $publicResult `
                -ExpectedCode LIE004 -ExpectedSafeSubject missing-manifest.md

            $publicResult = Invoke-PublicCliCase -Arguments @(
                '-Repo', $escaping.Repo, '-OrderOnly', '-Manifest', 'token=fixture-secret-value.md',
                '-OrderOutput', $escaping.RootOutput, '-WhatIf'
            )
            Assert-PublicDiagnostic -CaseId public-lie004-credential -Actual $publicResult `
                -ExpectedCode LIE004 -ExpectedSafeSubject 'token=[redacted]' -ForbiddenValue fixture-secret-value

            $publicLie007Safe = New-TransactionFixture -CaseId public-lie007-safe
            $manifestObject = Get-Content -LiteralPath $publicLie007Safe.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
            $manifestObject.dependencies[0].from = 'requirements/intakes/active/missing.md'
            [IO.File]::WriteAllText($publicLie007Safe.ManifestPath, (($manifestObject | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
            $publicResult = Invoke-PublicCliCase -Arguments @(
                '-Repo', $publicLie007Safe.Repo, '-OrderOnly', '-Manifest', $publicLie007Safe.ManifestRelative,
                '-OrderOutput', $publicLie007Safe.RootOutput, '-WhatIf'
            )
            Assert-PublicDiagnostic -CaseId public-lie007-safe -Actual $publicResult `
                -ExpectedCode LIE007 -ExpectedSafeSubject requirements/intakes/active/missing.md

            $publicLie007Credential = New-TransactionFixture -CaseId public-lie007-credential
            $manifestObject = Get-Content -LiteralPath $publicLie007Credential.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
            $manifestObject.dependencies[0].from = 'token=fixture-secret-value.md'
            [IO.File]::WriteAllText($publicLie007Credential.ManifestPath, (($manifestObject | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
            $publicResult = Invoke-PublicCliCase -Arguments @(
                '-Repo', $publicLie007Credential.Repo, '-OrderOnly', '-Manifest', $publicLie007Credential.ManifestRelative,
                '-OrderOutput', $publicLie007Credential.RootOutput, '-WhatIf'
            )
            Assert-PublicDiagnostic -CaseId public-lie007-credential -Actual $publicResult `
                -ExpectedCode LIE007 -ExpectedSafeSubject 'token=[redacted]' -ForbiddenValue fixture-secret-value
            $publicHelpOutput = @(& pwsh -NoProfile -File $publicCli -Help 2>&1 | ForEach-Object { [string]$_ })
            $publicHelpExit = $LASTEXITCODE
            $publicHelpText = $publicHelpOutput -join "`n"
            if ($publicHelpExit -ne 0 `
                -or -not $publicHelpText.Contains('Position, Status', [StringComparison]::Ordinal) `
                -or -not $publicHelpText.Contains('five-column', [StringComparison]::Ordinal) `
                -or $publicHelpText.Contains('/Users/', [StringComparison]::Ordinal) `
                -or $publicHelpText -match '(?i)CategoryInfo|FullyQualifiedErrorId|ScriptStackTrace') {
                $failures.Add('oeffentliche PowerShell-Hilfe ist unvollstaendig oder enthaelt private Pfad-/Stackdaten / public PowerShell help is incomplete or contains private path or stack data')
            }
            foreach ($lineEndingPath in @((Join-Path $escaping.Repo $escaping.RootOutput), (Join-Path $escaping.Repo $escaping.SeriesOutput))) {
                if ([Array]::IndexOf[byte]([IO.File]::ReadAllBytes($lineEndingPath), 13) -ge 0) {
                    $failures.Add('Ausgabe enthaelt CR statt stabiler LF-Zeilenenden / output contains CR instead of stable LF line endings')
                }
            }

            $manual = New-TransactionFixture -CaseId manual-content
            $actual = Invoke-ProjectionCase -Fixture $manual -Mode Write -Outputs @($manual.RootOutput)
            Assert-ProjectionCase -CaseId 'manual-content-baseline' -Actual $actual -ExpectedCode '' -ExpectedExitClass zero -ExpectedWrites 1
            $manualPath = Join-Path $manual.Repo $manual.RootOutput
            $generated = [IO.File]::ReadAllText($manualPath, [Text.UTF8Encoding]::new($false))
            [IO.File]::WriteAllText($manualPath, "# Manueller Prefix`rPrefix bleibt`r${generated}`r## Manueller Suffix`rSuffix bleibt`r", [Text.UTF8Encoding]::new($false))
            $actual = Invoke-ProjectionCase -Fixture $manual -Mode Write -Outputs @($manual.RootOutput)
            Assert-ProjectionCase -CaseId 'manual-content-normalization' -Actual $actual -ExpectedCode '' -ExpectedExitClass zero -ExpectedWrites 1
            $manualResult = [IO.File]::ReadAllText($manualPath, [Text.UTF8Encoding]::new($false))
            foreach ($expectedManualText in @('# Manueller Prefix', 'Prefix bleibt', '## Manueller Suffix', 'Suffix bleibt')) {
                if (-not $manualResult.Contains($expectedManualText, [StringComparison]::Ordinal)) {
                    $failures.Add("manueller Text fehlt nach CR-Normalisierung / manual text missing after CR normalization: ${expectedManualText}")
                }
            }
            if ($manualResult.Contains("`r", [StringComparison]::Ordinal)) {
                $failures.Add('manueller Inhalt enthaelt CR statt stabiler LF-Zeilenenden / manual content contains CR instead of stable LF line endings')
            }

            foreach ($negativeCase in $negativeCases.cases) {
                $caseId = [string]$negativeCase.id
                if ($caseId -in @('stale-check-output', 'simulated-publish-failure', 'root-series-drift', 'cross-implementation-parity')) { continue }
                $fixture = New-TransactionFixture -CaseId $caseId
                if ($caseId -ceq 'invalid-utf8') {
                    [IO.File]::WriteAllBytes($fixture.ManifestPath, [byte[]](0xC3, 0x28))
                } elseif ($caseId -ceq 'embedded-nul') {
                    [IO.File]::WriteAllBytes($fixture.ManifestPath, [byte[]](0x7B, 0x00, 0x7D))
                } elseif ($caseId -ceq 'missing-required-field') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.PSObject.Properties.Remove('schemaVersion')
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'wrong-type') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.orderedTargets = [pscustomobject]@{}
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -in @('absolute-posix', 'absolute-drive', 'absolute-unc', 'parent-traversal', 'option-path', 'missing-target', 'wrong-filesystem-type', 'symlink-escape')) {
                    $badPath = [string]$negativeCase.path
                    if ($caseId -ceq 'symlink-escape') {
                        $outside = Join-Path $fixtureRepo 'outside-powershell.md'
                        [IO.File]::WriteAllText($outside, "# outside`n", [Text.UTF8Encoding]::new($false))
                        New-Item -ItemType Directory -Path (Join-Path $fixture.Repo 'fixtures') -Force | Out-Null
                        New-Item -ItemType SymbolicLink -Path (Join-Path $fixture.Repo $badPath) -Target $outside | Out-Null
                    }
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.orderedTargets[0].path = $badPath
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'logical-and-stamped-both-present') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $logicalPath = [string]$value.orderedTargets[1].path
                    $stampedPath = $logicalPath.Substring(0, $logicalPath.Length - 3) + '.123-shadow.md'
                    [IO.File]::WriteAllText((Join-Path $fixture.Repo $stampedPath), "# Stamped shadow`n", [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'duplicate-identity') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.orderedTargets = @($value.orderedTargets) + @($value.orderedTargets[0])
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'duplicate-position') {
                    [IO.File]::WriteAllText((Join-Path $fixture.Repo 'requirements/intakes/active/Lastenheft_Überblick.md'), "# Fixture`n`n**Reihenfolge:** sichtbare Position 39`n", [Text.UTF8Encoding]::new($false))
                    Set-FixtureManifestHashes -Repo $fixture.Repo -ManifestPath $fixture.ManifestPath
                } elseif ($caseId -ceq 'unknown-endpoint') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.dependencies = @($value.dependencies) + @([pscustomobject]@{
                        from = 'requirements/intakes/active/missing.md'
                        to = [string]$value.orderedTargets[0].path
                        kind = 'HardCompletionGate'
                        binding = $true
                    })
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'missing-normalized-hash') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.orderedTargets[0].PSObject.Properties.Remove('normalizedSha256')
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'target-hash-mismatch') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.orderedTargets[0].normalizedSha256 = ('0' * 64) -join ''
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'non-string-target-field') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.orderedTargets[0].status = 7
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'non-string-edge-field') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.dependencies[0].kind = 7
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'unknown-edge-kind') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.dependencies[0].kind = 'UnknownGate'
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'binding-mismatch') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    foreach ($edge in $value.dependencies) { if ([string]$edge.kind -ceq 'PreferredSerialOrder') { $edge.binding = $true } }
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'backward-edge') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.dependencies[-1].from = [string]$value.orderedTargets[3].path
                    $value.dependencies[-1].to = [string]$value.orderedTargets[1].path
                    $value.roots = @([string]$value.orderedTargets[2].path, [string]$value.orderedTargets[3].path)
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'self-edge') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.dependencies = @($value.dependencies) + @([pscustomobject]@{
                        from = [string]$value.orderedTargets[1].path
                        to = [string]$value.orderedTargets[1].path
                        kind = 'HardCompletionGate'
                        binding = $true
                    })
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'dependency-cycle') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.dependencies = @($value.dependencies) + @([pscustomobject]@{
                        from = [string]$value.orderedTargets[0].path
                        to = [string]$value.orderedTargets[1].path
                        kind = 'HardCompletionGate'
                        binding = $true
                    })
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'incorrect-root-set') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.roots = @($value.roots) + @([string]$value.orderedTargets[0].path)
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'missing-nested-directory') {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value.orderedTargets[0].path = 'requirements/intakes/missing/deep/Lastenheft.md'
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -in @('nonterminal-archive-proof', 'archive-hash-mismatch')) {
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $logicalPath = [string]$value.orderedTargets[1].path
                    $stampedPath = $logicalPath.Substring(0, $logicalPath.Length - 3) + '.123-archive-proof.md'
                    Move-Item -LiteralPath (Join-Path $fixture.Repo $logicalPath) -Destination (Join-Path $fixture.Repo $stampedPath)
                    New-Item -ItemType Directory -Path (Join-Path $fixture.Repo 'specs/123-archive-proof') -Force | Out-Null
                    $stampedHash = (Get-FileHash -LiteralPath (Join-Path $fixture.Repo $stampedPath) -Algorithm SHA256).Hash.ToLowerInvariant()
                    $proofStatus = if ($caseId -ceq 'nonterminal-archive-proof') { 'Active' } else { 'Completed' }
                    $proofHash = if ($caseId -ceq 'archive-hash-mismatch') { ('0' * 64) -join '' } else { $stampedHash }
                    $state = [ordered]@{
                        status = $proofStatus
                        closeout = [ordered]@{ mergeOrPublication = 'Completed'; defaultBranchSync = 'Completed'; finalValidation = 'Completed' }
                        acceptedArtifacts = @([ordered]@{ path = $stampedPath; sha256 = $proofHash })
                    }
                    [IO.File]::WriteAllText((Join-Path $fixture.Repo 'specs/123-archive-proof/autonomous-run-state.json'), (($state | ConvertTo-Json -Depth 6) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($caseId -ceq 'multiple-feature-candidates') {
                    $second = Join-Path $fixture.Repo 'specs/033-second-proof'
                    New-Item -ItemType Directory -Path $second -Force | Out-Null
                    [IO.File]::WriteAllText((Join-Path $second 'spec.md'), "# Second`n`n**Binding Input**: ``Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md```n", [Text.UTF8Encoding]::new($false))
                }
                $actual = Invoke-ProjectionCase -Fixture $fixture -Mode Check -Outputs @($fixture.RootOutput)
                Assert-ProjectionCase -CaseId $caseId -Actual $actual -ExpectedCode ([string]$negativeCase.expectedDiagnostic) -ExpectedExitClass nonzero -ExpectedWrites 0
            }

            foreach ($featureCase in $featureMatrix.cases) {
                $featureId = [string]$featureCase.id
                $fixture = New-TransactionFixture -CaseId "feature-$featureId"
                $intake = 'Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md'
                if ($featureId -ceq 'archived-closeout-binding') {
                    Remove-Item -LiteralPath (Join-Path $fixture.Repo 'specs/032-linked-intake-evidence') -Recurse -Force
                    $intake = 'requirements/intakes/active/Lastenheft_Überblick.md'
                    $archived = 'requirements/intakes/active/Lastenheft_Überblick.123-archived-proof.md'
                    New-Item -ItemType Directory -Path (Join-Path $fixture.Repo 'specs/123-archived-proof') -Force | Out-Null
                    Remove-Item -LiteralPath (Join-Path $fixture.Repo $intake) -Force
                    [IO.File]::WriteAllText((Join-Path $fixture.Repo $archived), "# Fixture`n`n**Reihenfolge:** sichtbare Position 1`n", [Text.UTF8Encoding]::new($false))
                    $archivedHash = (Get-FileHash -LiteralPath (Join-Path $fixture.Repo $archived) -Algorithm SHA256).Hash.ToLowerInvariant()
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    foreach ($target in $value.orderedTargets) { if ([string]$target.path -ceq $intake) { $target.status = 'Completed' } }
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                    $state = [ordered]@{
                        status = 'Completed'
                        closeout = [ordered]@{ mergeOrPublication = 'Completed'; defaultBranchSync = 'Completed'; finalValidation = 'Completed' }
                        acceptedArtifacts = @([ordered]@{ path = $archived; sha256 = $archivedHash })
                    }
                    [IO.File]::WriteAllText((Join-Path $fixture.Repo 'specs/123-archived-proof/autonomous-run-state.json'), (($state | ConvertTo-Json -Depth 6) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($featureId -ceq 'reviewed-legacy-mapping') {
                    Remove-Item -LiteralPath (Join-Path $fixture.Repo 'specs/032-linked-intake-evidence') -Recurse -Force
                    New-Item -ItemType Directory -Path (Join-Path $fixture.Repo 'specs/124-reviewed-legacy') -Force | Out-Null
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value | Add-Member -NotePropertyName featureEvidence -NotePropertyValue @([ordered]@{
                        intakePath = $intake
                        featurePath = 'specs/124-reviewed-legacy'
                        proofKind = 'ReviewedLegacyMapping'
                        reviewed = $true
                    })
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($featureId -ceq 'no-evidence') {
                    Remove-Item -LiteralPath (Join-Path $fixture.Repo 'specs') -Recurse -Force
                } elseif ($featureId -ceq 'ambiguous') {
                    $second = Join-Path $fixture.Repo 'specs/033-second-proof'
                    New-Item -ItemType Directory -Path $second -Force | Out-Null
                    [IO.File]::WriteAllText((Join-Path $second 'spec.md'), "# Second`n`n**Binding Input**: ``${intake}```n", [Text.UTF8Encoding]::new($false))
                } elseif ($featureId -ceq 'invalid') {
                    Remove-Item -LiteralPath (Join-Path $fixture.Repo 'specs/032-linked-intake-evidence') -Recurse -Force
                    $value = Get-Content -LiteralPath $fixture.ManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
                    $value | Add-Member -NotePropertyName featureEvidence -NotePropertyValue @([ordered]@{
                        intakePath = $intake
                        featurePath = 'specs/125-missing'
                        proofKind = 'ReviewedLegacyMapping'
                        reviewed = $true
                    })
                    [IO.File]::WriteAllText($fixture.ManifestPath, (($value | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
                } elseif ($featureId -ceq 'similarity-is-not-proof') {
                    Remove-Item -LiteralPath (Join-Path $fixture.Repo 'specs/032-linked-intake-evidence') -Recurse -Force
                    New-Item -ItemType Directory -Path (Join-Path $fixture.Repo 'specs/126-similar-name') -Force | Out-Null
                }
                $actual = Invoke-ProjectionCase -Fixture $fixture -Mode Write -Outputs @($fixture.RootOutput)
                if ([string]$featureCase.expectedState -in @('Ambiguous', 'Invalid')) {
                    Assert-ProjectionCase -CaseId "feature-$featureId" -Actual $actual -ExpectedCode LIE008 -ExpectedExitClass nonzero -ExpectedWrites 0
                } else {
                    Assert-ProjectionCase -CaseId "feature-$featureId" -Actual $actual -ExpectedCode '' -ExpectedExitClass zero -ExpectedWrites 1
                    $featureOutput = [IO.File]::ReadAllText((Join-Path $fixture.Repo $fixture.RootOutput), [Text.UTF8Encoding]::new($false))
                    $expectedFeature = switch ($featureId) {
                        'feature-binding' { '[032-linked-intake-evidence](specs/032-linked-intake-evidence/)' }
                        'archived-closeout-binding' { '[123-archived-proof](specs/123-archived-proof/)' }
                        'reviewed-legacy-mapping' { '[124-reviewed-legacy](specs/124-reviewed-legacy/)' }
                        default { '— (kein Spec-Kit-Feature / no Spec Kit feature)' }
                    }
                    if (-not $featureOutput.Contains($expectedFeature, [StringComparison]::Ordinal)) {
                        $failures.Add("feature-${featureId}: erwartete Zelle fehlt / expected cell is missing: ${expectedFeature}")
                    }
                }
            }
        }

        if ($failures.Count -gt 0) {
            foreach ($failure in $failures) {
                [Console]::Error.WriteLine("FEHLER / FAIL: ${failure}")
            }
            throw "$($failures.Count) fehlgeschlagene Assertion(en) / failed assertion(s)"
        }

        Write-Output 'PASS: PowerShell-Vertrag fuer verlinkte Intake-Evidence ist erfuellt.'
        Write-Output 'PASS: PowerShell contract for linked intake evidence is satisfied.'
    }
    finally {
        if (Test-Path -LiteralPath $fixtureRepo) {
            Remove-Item -LiteralPath $fixtureRepo -Recurse -Force
        }
    }
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$fixtureDirectory = Join-Path $PSScriptRoot 'tests/linked-intake-evidence/positive'
Test-RequirementsIntakeGovernance -RepositoryRoot $repositoryRoot -FixtureDirectory $fixtureDirectory
