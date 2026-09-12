<#
.SYNOPSIS
Rendert verlinkte Intake-Reihenfolgen. / Renders linked intake order views.

.DESCRIPTION
Liest ein repositoryrelatives Series-Manifest und validiert UTF-8, Pfade,
Abhaengigkeiten sowie explizite Feature-Nachweise. Jede Zeile folgt dem
gemeinsamen Fuenf-Spalten-Vertrag mit Position, Status, vollstaendigem
Intake-Link, direkten Abhaengigkeiten und Spec-Kit-Feature. Ohne -Write wird
nur geprueft; -WhatIf erzwingt ebenfalls null Schreibvorgaenge.

Reads a repository-relative series manifest and validates UTF-8, paths,
dependencies, and explicit feature evidence. Every row follows the common
five-column contract with position, status, complete intake link, direct
dependencies, and Spec Kit feature. Without -Write the command checks only;
-WhatIf also guarantees zero writes.

.PARAMETER RepositoryRoot
Explizites Git-Repository. Standard ist die Wurzel dieses Skripts.
Explicit Git repository. The default is this script's repository root.

.PARAMETER ManifestPath
Repositoryrelativer Pfad zum kanonischen Series-Manifest.
Repository-relative path to the canonical series manifest.

.PARAMETER OutputPath
Ein oder mehrere repositoryrelative Ausgabepfade. Standard sind Root- und
Series-Ansicht. / One or more repository-relative output paths. The defaults
are the root and series views.

.PARAMETER Write
Veroeffentlicht geaenderte Marker atomar. / Atomically publishes changed markers.

.EXAMPLE
pwsh -NoProfile -File scripts/render-requirements-intake-governance.ps1

.EXAMPLE
pwsh -NoProfile -File scripts/render-requirements-intake-governance.ps1 -Write

.EXAMPLE
pwsh -NoProfile -File scripts/render-requirements-intake-governance.ps1 -Write -WhatIf
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [Alias('Repo')]
    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot),

    [Alias('Manifest')]
    [string]$ManifestPath = 'requirements/intakes/series/inventar-worker-service/manifest.json',

    [Alias('OrderOutput')]
    [string[]]$OutputPath = @(),

    [switch]$Write,

    [switch]$OrderOnly,

    [switch]$AllowDirty,

    [switch]$Help
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-SdhDiagnosticRemediation {
    param([string]$Code)

    switch ($Code) {
        'LIE001' { return 'Datei als gueltiges UTF-8 ohne NUL speichern und erneut pruefen / Save the file as valid UTF-8 without NUL and check again.' }
        'LIE002' { return 'Schema und Pflichtfelder in der kanonischen Quelle korrigieren / Correct the schema and required fields in the canonical source.' }
        'LIE003' { return 'Repository-relativen Pfad ohne Traversal oder Optionskomponente verwenden / Use a repository-relative path without traversal or option components.' }
        'LIE004' { return 'Kanonischen relativen Pfad und erwarteten Typ pruefen / Check the canonical relative path and expected type.' }
        'LIE005' { return 'Symlink und physische Pfadauflosung innerhalb des Repositorys korrigieren / Correct the symlink and physical path resolution inside the repository.' }
        'LIE006' { return 'Kanonische Identitaeten und Positionen eindeutig machen / Make canonical identities and positions unique.' }
        'LIE007' { return 'From, To, Kind und Binding einzeln mit dem Manifest abgleichen / Compare from, to, kind, and binding individually with the manifest.' }
        'LIE008' { return 'Genau einen expliziten vorhandenen Feature-Nachweis bereitstellen / Provide exactly one explicit existing feature proof.' }
        'LIE009' { return 'Kanonische Quelle pruefen und den begrenzten Schreibmodus ausfuehren / Review the canonical source and run the bounded write mode.' }
        'LIE010' { return 'Fehlerursache beheben und die vollstaendige Transaktion erneut ausfuehren / Fix the cause and run the complete transaction again.' }
        'LIE011' { return 'Beide Ausgaben aus derselben typisierten Projektion regenerieren / Regenerate both outputs from the same typed projection.' }
        'LIE012' { return 'Gemeinsame Fixtures, Exitklasse, Diagnose und Ausgabebytes vergleichen / Compare shared fixtures, exit class, diagnostic, and output bytes.' }
        default { return 'Sicheren relativen Eingabekontext pruefen und den Befehl erneut ausfuehren / Check the safe relative input context and run the command again.' }
    }
}

function ConvertTo-SdhPublicDiagnostic {
    param(
        [string]$Message,
        [string[]]$PrivatePath = @()
    )

    $safeMessage = $Message
    foreach ($path in $PrivatePath) {
        if (-not [string]::IsNullOrWhiteSpace($path) -and [IO.Path]::IsPathRooted($path)) {
            $safeMessage = $safeMessage.Replace($path, '[repository]', [StringComparison]::Ordinal)
        }
    }
    # A public diagnostic is one bounded line. This prevents terminal control
    # injection and removes credential-shaped values without hiding ordinary
    # repository-relative context needed for remediation.
    $safeMessage = [regex]::Replace(
        $safeMessage,
        '(?i)\b(token|password|secret|authorization|api[_-]?key)\s*[:=]\s*[^\s;,]+',
        '$1=[redacted]'
    )
    $safeMessage = [regex]::Replace($safeMessage, '(?i)(?:/Users/|/home/)[^\s:;,]+', '[private-path]')
    $safeMessage = [regex]::Replace($safeMessage, '[\x00-\x1f\x7f]', '?')
    if ($safeMessage -match '^(LIE\d{3}):') {
        $remediation = Get-SdhDiagnosticRemediation -Code $Matches[1]
        $safeMessage = "${safeMessage}; Abhilfe / remediation: ${remediation}"
    }
    return $safeMessage
}
function Get-SdhIntakeSeriesManifest {
    param([string]$Repo, [string]$ExplicitManifest = '')

    if ($ExplicitManifest) {
        Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $ExplicitManifest -ExpectedType File
        return Join-Path $Repo $ExplicitManifest
    }

    $preferred = Join-Path $Repo 'requirements/intakes/series/home-baseline-delivery/manifest.json'
    if (Test-Path -LiteralPath $preferred -PathType Leaf) { return $preferred }

    $manifestMatches = @()
    foreach ($seriesRoot in @((Join-Path $Repo 'requirements/intakes/series'), (Join-Path $Repo 'specs/intake-series'))) {
        if (Test-Path -LiteralPath $seriesRoot -PathType Container) {
            $manifestMatches += @(Get-ChildItem -LiteralPath $seriesRoot -Recurse -Depth 1 -File -Filter manifest.json -ErrorAction SilentlyContinue)
        }
    }
    if ($manifestMatches.Count -eq 1) { return $manifestMatches[0].FullName }
    return ''
}

function Assert-SdhSafeRepositoryPath {
    param(
        [string]$Repo,
        [string]$RelativePath,
        [ValidateSet('File', 'Directory')]
        [string]$ExpectedType
    )

    if ([string]::IsNullOrWhiteSpace($RelativePath) `
        -or [IO.Path]::IsPathRooted($RelativePath) `
        -or $RelativePath -match '^[A-Za-z]:[/\\]' `
        -or $RelativePath -match '^[/\\]{2}' `
        -or $RelativePath -match '(^|/)\.\.(/|$)' `
        -or $RelativePath -match '(^|/)-' `
        -or $RelativePath -match '\\' `
        -or $RelativePath -match '[\x00-\x1f]') {
        # Rejected bytes are not reflected because the value may itself carry
        # credentials or terminal control data.
        throw 'LIE003: unsicherer Repositorypfad / unsafe repository path: [redacted]'
    }

    $target = Join-Path $Repo $RelativePath
    $pathType = if ($ExpectedType -eq 'File') { 'Leaf' } else { 'Container' }
    if (-not (Test-Path -LiteralPath $target -PathType $pathType)) {
        throw "LIE004: Ziel fehlt oder hat den falschen Typ / target is missing or has the wrong type: ${RelativePath}"
    }

    $resolvedRepo = Get-SdhPhysicalPath -BasePath $Repo -RelativePath ''
    $resolvedTarget = Get-SdhPhysicalPath -BasePath $Repo -RelativePath $RelativePath
    $prefix = $resolvedRepo + [IO.Path]::DirectorySeparatorChar
    $comparison = if ($IsWindows) { [StringComparison]::OrdinalIgnoreCase } else { [StringComparison]::Ordinal }
    if (-not $resolvedTarget.Equals($resolvedRepo, $comparison) -and -not $resolvedTarget.StartsWith($prefix, $comparison)) {
        throw "LIE005: Pfad verlaesst das Repository / path escapes repository: ${RelativePath}"
    }
}

function Get-SdhPhysicalPath {
    param([string]$BasePath, [string]$RelativePath)

    $baseItem = Get-Item -LiteralPath $BasePath -Force
    $baseLink = $baseItem.ResolveLinkTarget($true)
    $current = if ($baseLink) { $baseLink.FullName } else { $baseItem.FullName }
    foreach ($component in @($RelativePath -split '/' | Where-Object { $_ })) {
        $item = Get-Item -LiteralPath (Join-Path $current $component) -Force
        $link = $item.ResolveLinkTarget($true)
        $current = if ($link) { $link.FullName } else { $item.FullName }
    }
    return [IO.Path]::GetFullPath($current)
}

function Assert-SdhSafeOutputPath {
    param([string]$Repo, [string]$RelativePath)

    if ([string]::IsNullOrWhiteSpace($RelativePath) `
        -or [IO.Path]::IsPathRooted($RelativePath) `
        -or $RelativePath -match '^[A-Za-z]:[/\\]' `
        -or $RelativePath -match '^[/\\]{2}' `
        -or $RelativePath -match '(^|/)\.\.(/|$)' `
        -or $RelativePath -match '(^|/)-' `
        -or $RelativePath -match '\\' `
        -or $RelativePath -match '[\x00-\x1f]') {
        throw 'LIE003: unsicherer Ausgabepfad / unsafe output path: [redacted]'
    }
    $target = Join-Path $Repo $RelativePath
    $parentRelative = [IO.Path]::GetDirectoryName($RelativePath.Replace('/', [IO.Path]::DirectorySeparatorChar))
    $parent = if ($parentRelative) { Join-Path $Repo $parentRelative } else { $Repo }
    if (-not (Test-Path -LiteralPath $parent -PathType Container)) {
        throw "LIE004: Ausgabe-Elternverzeichnis fehlt / output parent is missing: ${parentRelative}"
    }
    if ((Test-Path -LiteralPath $target) -and -not (Test-Path -LiteralPath $target -PathType Leaf)) {
        throw "LIE004: Ausgabe hat den falschen Typ / output has the wrong type: ${RelativePath}"
    }
    if (Test-Path -LiteralPath $target) {
        $item = Get-Item -LiteralPath $target -Force
        if ($item.LinkType) { throw "LIE005: Ausgabe darf kein symbolischer Link sein / output must not be a symbolic link: ${RelativePath}" }
    }
    $resolvedRepo = Get-SdhPhysicalPath -BasePath $Repo -RelativePath ''
    $resolvedParent = Get-SdhPhysicalPath -BasePath $Repo -RelativePath ($parentRelative -replace '\\', '/')
    $prefix = $resolvedRepo.TrimEnd([IO.Path]::DirectorySeparatorChar, [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    $comparison = if ($IsWindows) { [StringComparison]::OrdinalIgnoreCase } else { [StringComparison]::Ordinal }
    if (-not $resolvedParent.Equals($resolvedRepo, $comparison) -and -not $resolvedParent.StartsWith($prefix, $comparison)) {
        throw "LIE005: Ausgabepfad verlaesst das Repository / output path escapes repository: ${RelativePath}"
    }
}

function Read-SdhStrictUtf8File {
    param([string]$Path, [string]$Subject)

    $bytes = [IO.File]::ReadAllBytes($Path)
    if ([Array]::IndexOf[byte]($bytes, 0) -ge 0) {
        throw "LIE001: NUL-Inhalt ist unzulaessig / NUL content is not allowed: ${Subject}"
    }
    try {
        return [Text.UTF8Encoding]::new($false, $true).GetString($bytes)
    } catch [Text.DecoderFallbackException] {
        throw "LIE001: ungueltiges UTF-8 / invalid UTF-8: ${Subject}"
    }
}

function Assert-SdhLinkedIntakeCompletionProof {
    param(
        [string]$Repo,
        [string]$LogicalPath,
        [string]$ResolvedPath,
        [string]$StatePath
    )

    $stateText = Read-SdhStrictUtf8File -Path (Join-Path $Repo $StatePath) -Subject $StatePath
    try { $state = $stateText | ConvertFrom-Json } catch {
        throw "LIE008: ungueltiger Feature-Abschlussnachweis / invalid feature completion proof: ${LogicalPath}"
    }
    $stateProperties = @($state.PSObject.Properties.Name)
    if ('status' -cnotin $stateProperties `
        -or 'closeout' -cnotin $stateProperties `
        -or 'acceptedArtifacts' -cnotin $stateProperties `
        -or $state.status -isnot [string] `
        -or [string]$state.status -cne 'Completed' `
        -or $null -eq $state.closeout `
        -or $state.acceptedArtifacts -isnot [Array]) {
        throw "LIE008: Feature-Abschlussnachweis ist nicht terminal / feature completion proof is not terminal: ${LogicalPath}"
    }
    $closeoutProperties = @($state.closeout.PSObject.Properties.Name)
    if ('mergeOrPublication' -cnotin $closeoutProperties `
        -or 'defaultBranchSync' -cnotin $closeoutProperties `
        -or 'finalValidation' -cnotin $closeoutProperties `
        -or [string]$state.closeout.mergeOrPublication -cne 'Completed' `
        -or [string]$state.closeout.defaultBranchSync -cne 'Completed' `
        -or [string]$state.closeout.finalValidation -cne 'Completed') {
        throw "LIE008: Feature-Closeout ist nicht terminal / feature closeout is not terminal: ${LogicalPath}"
    }

    $resolvedHash = (Get-FileHash -LiteralPath (Join-Path $Repo $ResolvedPath) -Algorithm SHA256).Hash.ToLowerInvariant()
    $accepted = @($state.acceptedArtifacts | Where-Object {
        $null -ne $_ `
            -and 'path' -cin @($_.PSObject.Properties.Name) `
            -and 'sha256' -cin @($_.PSObject.Properties.Name) `
            -and $_.path -is [string] `
            -and $_.sha256 -is [string] `
            -and (([string]$_.path -ceq $LogicalPath) -or ([string]$_.path -ceq $ResolvedPath)) `
            -and [string]$_.sha256 -ceq $resolvedHash
    })
    if ($accepted.Count -eq 1) {
        return
    }

    # A constitutional closeout may bind the renamed intake in its archived
    # series receipt instead of duplicating that path in acceptedArtifacts.
    # Require one exact receipt and retain the terminal run-state requirement.
    $stamp = ($StatePath -replace '^specs/', '') -replace '/autonomous-run-state\.json$', ''
    $archiveRoot = Join-Path $Repo 'specs/intake-authoring-archive/series'
    $receipts = @()
    if (Test-Path -LiteralPath $archiveRoot -PathType Container) {
        $receipts = @(Get-ChildItem -LiteralPath $archiveRoot -Recurse -File -Filter 'series-receipt.json' |
            Where-Object { $_.Directory.Name -ceq $stamp } |
            Sort-Object FullName)
    }
    if ($receipts.Count -eq 1) {
        $receiptPath = [IO.Path]::GetRelativePath($Repo, $receipts[0].FullName).Replace('\', '/')
        Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $receiptPath -ExpectedType File
        $receiptText = Read-SdhStrictUtf8File -Path $receipts[0].FullName -Subject $receiptPath
        try { $receipt = $receiptText | ConvertFrom-Json } catch {
            throw "LIE008: ungueltiger archivierter Abschlussnachweis / invalid archived closeout proof: ${LogicalPath}"
        }
        $receiptProperties = @($receipt.PSObject.Properties.Name)
        $lineageProperties = if ($null -ne $receipt.lineage) { @($receipt.lineage.PSObject.Properties.Name) } else { @() }
        $renameProperties = if ($null -ne $receipt.lineage -and $null -ne $receipt.lineage.rename) { @($receipt.lineage.rename.PSObject.Properties.Name) } else { @() }
        if ('documentType' -cin $receiptProperties `
            -and 'status' -cin $receiptProperties `
            -and 'rename' -cin $lineageProperties `
            -and 'from' -cin $renameProperties `
            -and 'to' -cin $renameProperties `
            -and 'normalizedSha256After' -cin $renameProperties `
            -and [string]$receipt.documentType -ceq 'IntakeSeriesReceipt' `
            -and [string]$receipt.status -ceq 'Ready' `
            -and -not [string]::IsNullOrWhiteSpace([string]$receipt.lineage.rename.from) `
            -and [string]$receipt.lineage.rename.to -ceq $ResolvedPath `
            -and [string]$receipt.lineage.rename.normalizedSha256After -ceq $resolvedHash) {
            return
        }
    }

    throw "LIE008: gestempelter Intake ist nicht durch einen hashgebundenen terminalen Feature-Abschluss belegt / stamped intake is not proven by a hash-bound terminal feature completion: ${LogicalPath}"
}

function Resolve-SdhLinkedIntakePath {
    param([string]$Repo, [string]$LogicalPath)

    if ([string]::IsNullOrWhiteSpace($LogicalPath) `
        -or [IO.Path]::IsPathRooted($LogicalPath) `
        -or $LogicalPath -match '^[A-Za-z]:[/\\]' `
        -or $LogicalPath -match '^[/\\]{2}' `
        -or $LogicalPath -match '(^|/)\.\.(/|$)' `
        -or $LogicalPath -match '(^|/)-' `
        -or $LogicalPath -match '\\' `
        -or $LogicalPath -match '[\x00-\x1f]') {
        throw 'LIE003: unsicherer Repositorypfad / unsafe repository path: [redacted]'
    }
    $logicalTarget = Join-Path $Repo $LogicalPath
    $isActiveLogical = $LogicalPath -cmatch '^requirements/intakes/active/.+\.md$'
    if (-not $isActiveLogical -and (Test-Path -LiteralPath $logicalTarget -PathType Leaf)) {
        Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $LogicalPath -ExpectedType File
        return $LogicalPath
    }
    if (-not $isActiveLogical) {
        throw "LIE004: Ziel fehlt oder hat den falschen Typ / target is missing or has the wrong type: ${LogicalPath}"
    }

    $directoryRelative = [IO.Path]::GetDirectoryName($LogicalPath.Replace('/', [IO.Path]::DirectorySeparatorChar))
    Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath ($directoryRelative.Replace('\', '/')) -ExpectedType Directory
    $baseName = [IO.Path]::GetFileNameWithoutExtension($LogicalPath)
    $pattern = '^' + [regex]::Escape($baseName) + '\.[0-9]{3}-[^/]+\.md$'
    $candidates = @(Get-ChildItem -LiteralPath (Join-Path $Repo $directoryRelative) -File |
        Where-Object { $_.Name -cmatch $pattern } |
        Sort-Object Name)
    if (Test-Path -LiteralPath $logicalTarget -PathType Leaf) {
        Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $LogicalPath -ExpectedType File
        if ($candidates.Count -gt 0) {
            throw "LIE006: Original- und gestempelter Intake existieren gleichzeitig / original and stamped intake both exist: ${LogicalPath}"
        }
        return $LogicalPath
    }
    if ($candidates.Count -eq 0) {
        throw "LIE004: Ziel fehlt oder hat den falschen Typ / target is missing or has the wrong type: ${LogicalPath}"
    }
    if ($candidates.Count -gt 1) {
        throw "LIE006: logischer Intake ist mehrdeutig / logical intake is ambiguous: ${LogicalPath}"
    }

    $resolved = [IO.Path]::GetRelativePath($Repo, $candidates[0].FullName).Replace('\', '/')
    Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $resolved -ExpectedType File
    $archiveMatch = [regex]::Match($candidates[0].Name, '\.([0-9]{3}-[^/]+)\.md$')
    $statePath = if ($archiveMatch.Success) { "specs/$($archiveMatch.Groups[1].Value)/autonomous-run-state.json" } else { '' }
    if ([string]::IsNullOrEmpty($statePath) -or -not (Test-Path -LiteralPath (Join-Path $Repo $statePath) -PathType Leaf)) {
        throw "LIE008: gestempelter Intake hat keinen Feature-Nachweis / stamped intake has no feature proof: ${LogicalPath}"
    }
    Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $statePath -ExpectedType File
    Assert-SdhLinkedIntakeCompletionProof -Repo $Repo -LogicalPath $LogicalPath -ResolvedPath $resolved -StatePath $statePath
    return $resolved
}

function ConvertTo-SdhMarkdownText {
    param([string]$Text)
    # Raw HTML belongs to untrusted data; trusted <br> separators are added by
    # the renderer only after this contextual encoding boundary.
    return $Text.Replace('&', '&amp;').Replace('<', '&lt;').Replace('>', '&gt;').Replace('\', '\\').Replace('|', '\|').Replace('[', '\[').Replace(']', '\]').Replace('(', '\(').Replace(')', '\)').Replace('`', '\`')
}

function Test-SdhLinkedIntakeManifest {
    param([string]$Repo, [string]$ManifestPath)

    Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $ManifestPath -ExpectedType File
    $manifestText = Read-SdhStrictUtf8File -Path (Join-Path $Repo $ManifestPath) -Subject $ManifestPath
    try { $manifest = $manifestText | ConvertFrom-Json } catch { throw "LIE002: ungueltiges Series-Manifest / invalid series manifest: ${ManifestPath}" }
    $required = @('schemaVersion', 'documentType', 'seriesId', 'status', 'orderedTargets', 'roots', 'dependencies')
    foreach ($name in $required) {
        if ($name -cnotin @($manifest.PSObject.Properties.Name)) { throw "LIE002: Pflichtfeld fehlt / required field is missing: ${name}" }
    }
    if ($manifest.schemaVersion -isnot [string] `
        -or [string]$manifest.schemaVersion -cne '1.0' `
        -or $manifest.documentType -isnot [string] `
        -or [string]$manifest.documentType -cne 'IntakeSeriesManifest' `
        -or $manifest.seriesId -isnot [string] `
        -or [string]::IsNullOrWhiteSpace([string]$manifest.seriesId) `
        -or $manifest.status -isnot [string] `
        -or [string]::IsNullOrWhiteSpace([string]$manifest.status) `
        -or $manifest.orderedTargets -isnot [Array] `
        -or @($manifest.orderedTargets).Count -eq 0 `
        -or $manifest.roots -isnot [Array] `
        -or $manifest.dependencies -isnot [Array]) {
        throw "LIE002: ungueltiges Series-Manifest / invalid series manifest: ${ManifestPath}"
    }

    $knownPaths = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $positions = [Collections.Generic.HashSet[int]]::new()
    $positionByPath = [Collections.Generic.Dictionary[string, int]]::new([StringComparer]::Ordinal)
    $index = 0
    foreach ($target in $manifest.orderedTargets) {
        $index++
        if ($null -eq $target `
            -or 'path' -cnotin @($target.PSObject.Properties.Name) `
            -or 'role' -cnotin @($target.PSObject.Properties.Name) `
            -or 'status' -cnotin @($target.PSObject.Properties.Name) `
            -or 'normalizedSha256' -cnotin @($target.PSObject.Properties.Name) `
            -or $target.path -isnot [string] `
            -or $target.role -isnot [string] `
            -or $target.status -isnot [string] `
            -or $target.normalizedSha256 -isnot [string] `
            -or [string]::IsNullOrWhiteSpace([string]$target.path) `
            -or [string]::IsNullOrWhiteSpace([string]$target.role) `
            -or [string]::IsNullOrWhiteSpace([string]$target.status) `
            -or [string]$target.normalizedSha256 -cnotmatch '^[0-9a-f]{64}$') {
            throw 'LIE002: Series-Ziel ist unvollstaendig / series target is incomplete'
        }
        $path = [string]$target.path
        if (-not $knownPaths.Add($path)) { throw 'LIE006: doppelte Intake-Identitaet / duplicate intake identity' }
        $resolvedPath = Resolve-SdhLinkedIntakePath -Repo $Repo -LogicalPath $path
        $null = Read-SdhStrictUtf8File -Path (Join-Path $Repo $resolvedPath) -Subject $resolvedPath
        $actualHash = (Get-FileHash -LiteralPath (Join-Path $Repo $resolvedPath) -Algorithm SHA256).Hash.ToLowerInvariant()
        # A terminal stamped successor is bound by its completed run-state.
        # An unstamped source must still match the manifest directly.
        if ($resolvedPath -ceq $path -and $actualHash -cne [string]$target.normalizedSha256) {
            throw "LIE009: Intake-Hash weicht vom Series-Manifest ab / intake hash differs from series manifest: ${path}"
        }
        $position = Get-SdhDisplayPosition -IntakeFile (Join-Path $Repo $resolvedPath) -ManifestIndex $index
        if ($position -le 0 -or -not $positions.Add($position)) { throw 'LIE006: doppelte oder ungueltige sichtbare Position / duplicate or invalid display position' }
        $positionByPath.Add($path, $position)
    }

    $roots = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($root in $manifest.roots) {
        if ($root -isnot [string] -or [string]::IsNullOrWhiteSpace([string]$root)) { throw 'LIE002: ungueltiger Root / invalid root' }
        if (-not $roots.Add([string]$root)) { throw 'LIE006: doppelte Root-Identitaet / duplicate root identity' }
        if (-not $knownPaths.Contains([string]$root)) { throw "LIE007: unbekannter Root-Endpoint / unknown root endpoint: ${root}" }
    }

    $edges = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $incomingPaths = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($edge in $manifest.dependencies) {
        if ($null -eq $edge `
            -or 'from' -cnotin @($edge.PSObject.Properties.Name) `
            -or 'to' -cnotin @($edge.PSObject.Properties.Name) `
            -or 'kind' -cnotin @($edge.PSObject.Properties.Name) `
            -or 'binding' -cnotin @($edge.PSObject.Properties.Name) `
            -or $edge.from -isnot [string] `
            -or $edge.to -isnot [string] `
            -or $edge.kind -isnot [string] `
            -or [string]::IsNullOrWhiteSpace([string]$edge.from) `
            -or [string]::IsNullOrWhiteSpace([string]$edge.to) `
            -or [string]::IsNullOrWhiteSpace([string]$edge.kind) `
            -or $edge.binding -isnot [bool]) {
            throw 'LIE007: ungueltiges Dependency-Tupel / invalid dependency tuple'
        }
        if (-not $knownPaths.Contains([string]$edge.from)) { throw "LIE007: unbekannter Dependency-Endpoint / unknown dependency endpoint: $($edge.from)" }
        if (-not $knownPaths.Contains([string]$edge.to)) { throw "LIE007: unbekannter Dependency-Endpoint / unknown dependency endpoint: $($edge.to)" }
        $expectedBinding = switch ([string]$edge.kind) {
            'PreferredSerialOrder' { $false }
            { $_ -in @('HardCompletionGate', 'RequirementsGovernanceGate', 'AssessmentBaseline', 'FinalAuditInput') } { $true }
            default { throw "LIE007: unbekannte Dependency-Art / unknown dependency kind: $($edge.kind)" }
        }
        if ([bool]$edge.binding -ne $expectedBinding) {
            throw "LIE007: Dependency-Art und Binding widersprechen sich / dependency kind and binding disagree: $($edge.kind)"
        }
        if ([string]$edge.from -ceq [string]$edge.to) {
            throw 'LIE007: Dependency darf keine Selbstkante enthalten / dependency must not contain a self-edge'
        }
        if ($positionByPath.Item([string]$edge.from) -ge $positionByPath.Item([string]$edge.to)) {
            throw 'LIE007: Dependency-Kante laeuft gegen die sichtbare Reihenfolge / dependency edge runs backward against visible order'
        }
        $null = $incomingPaths.Add([string]$edge.to)
        $identity = '{0}`0{1}`0{2}`0{3}' -f $edge.from, $edge.to, $edge.kind, $edge.binding
        if (-not $edges.Add($identity)) { throw 'LIE007: doppeltes Dependency-Tupel / duplicate dependency tuple' }
    }

    $zeroIndegreePaths = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($knownPath in $knownPaths) {
        if (-not $incomingPaths.Contains($knownPath)) { $null = $zeroIndegreePaths.Add($knownPath) }
    }
    if (-not $roots.SetEquals($zeroIndegreePaths)) {
        throw 'LIE007: Root-Menge entspricht nicht exakt den Zielen ohne eingehende Kante / root set does not exactly equal zero-indegree targets'
    }

    $remainingNodes = [Collections.Generic.HashSet[string]]::new($knownPaths, [StringComparer]::Ordinal)
    $remainingEdges = [Collections.Generic.List[object]]::new()
    foreach ($edge in $manifest.dependencies) { $remainingEdges.Add($edge) }
    while ($remainingNodes.Count -gt 0) {
        $zeroNodes = @($remainingNodes | Where-Object {
            $candidate = $_
            -not ($remainingEdges | Where-Object { [string]$_.to -ceq $candidate } | Select-Object -First 1)
        })
        if ($zeroNodes.Count -eq 0) {
            throw 'LIE007: Dependency-Graph enthaelt einen Zyklus / dependency graph contains a cycle'
        }
        foreach ($zeroNode in $zeroNodes) { $null = $remainingNodes.Remove($zeroNode) }
        $survivingEdges = @($remainingEdges | Where-Object { [string]$_.from -cnotin $zeroNodes })
        $remainingEdges.Clear()
        foreach ($edge in $survivingEdges) { $remainingEdges.Add($edge) }
    }

    $featureEvidence = @()
    if ('featureEvidence' -cin @($manifest.PSObject.Properties.Name)) {
        if ($manifest.featureEvidence -isnot [Array]) {
            throw "LIE002: featureEvidence muss ein Array sein / featureEvidence must be an array: ${ManifestPath}"
        }
        $featureEvidence = @($manifest.featureEvidence)
    }
    foreach ($proof in $featureEvidence) {
        if ($null -eq $proof `
            -or [string]$proof.proofKind -cne 'ReviewedLegacyMapping' `
            -or $proof.reviewed -isnot [bool] `
            -or -not [bool]$proof.reviewed `
            -or -not $knownPaths.Contains([string]$proof.intakePath) `
            -or [string]$proof.featurePath -cnotmatch '^specs/[0-9]{3}-.+') {
            throw "LIE008: ungueltiger Legacy-Feature-Nachweis / invalid legacy feature evidence: $($proof.intakePath)"
        }
        try { Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath ([string]$proof.featurePath) -ExpectedType Directory } catch { throw "LIE008: Feature-Ziel fehlt oder ist unsicher / feature target is missing or unsafe: $($proof.intakePath)" }
    }
    return $manifest
}

function Get-SdhRelativeRepositoryPath {
    param([string]$ViewPath, [string]$TargetPath)

    $viewDirectory = [IO.Path]::GetDirectoryName($ViewPath.Replace('/', [IO.Path]::DirectorySeparatorChar))
    [string[]]$viewParts = @()
    if (-not [string]::IsNullOrEmpty($viewDirectory)) {
        $viewParts = @($viewDirectory -split '[\\/]')
    }
    [string[]]$targetParts = @($TargetPath -split '/')
    $common = 0
    while ($common -lt $viewParts.Count `
        -and $common -lt $targetParts.Count `
        -and $viewParts[$common] -ceq $targetParts[$common]) {
        $common++
    }

    $parts = [System.Collections.Generic.List[string]]::new()
    for ($index = $common; $index -lt $viewParts.Count; $index++) { $parts.Add('..') }
    for ($index = $common; $index -lt $targetParts.Count; $index++) { $parts.Add($targetParts[$index]) }
    return ($parts -join '/')
}

function ConvertTo-SdhEncodedRepositoryPath {
    param([string]$Path)

    $hasTrailingSlash = $Path.EndsWith('/', [StringComparison]::Ordinal)
    $plainPath = if ($hasTrailingSlash) { $Path.Substring(0, $Path.Length - 1) } else { $Path }
    $encoded = @($plainPath -split '/' | ForEach-Object {
        if ($_ -in @('.', '..')) { $_ } else { [Uri]::EscapeDataString($_) }
    }) -join '/'
    if ($hasTrailingSlash) { return $encoded + '/' }
    return $encoded
}

function ConvertTo-SdhMarkdownLabel {
    param([string]$Label)
    return $Label.Replace('&', '&amp;').Replace('<', '&lt;').Replace('>', '&gt;').Replace('\', '\\').Replace('[', '\[').Replace(']', '\]').Replace('|', '\|')
}

function Get-SdhMarkdownLink {
    param(
        [string]$Repo,
        [string]$ViewPath,
        [string]$TargetPath,
        [ValidateSet('File', 'Directory')]
        [string]$ExpectedType
    )

    Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $TargetPath -ExpectedType $ExpectedType
    $label = ConvertTo-SdhMarkdownLabel ([IO.Path]::GetFileName($TargetPath))
    $relative = Get-SdhRelativeRepositoryPath -ViewPath $ViewPath -TargetPath $TargetPath
    if ($ExpectedType -eq 'Directory') { $relative += '/' }
    $encoded = ConvertTo-SdhEncodedRepositoryPath -Path $relative
    return "[${label}](${encoded})"
}

function Get-SdhDisplayPosition {
    param([string]$IntakeFile, [int]$ManifestIndex)

    foreach ($line in Get-Content -LiteralPath $IntakeFile -Encoding UTF8) {
        if ($line -match '^\*\*Reihenfolge:\*\* *(?:sichtbare )?Position ([0-9]+)') { return [int]$Matches[1] }
        if ($line -match '^\*\*Order:\*\* *(?:visible )?[Pp]osition ([0-9]+)') { return [int]$Matches[1] }
        if ($line -match '^Dieser Intake .*Position ([0-9]+)') { return [int]$Matches[1] }
        if ($line -match '^Position ([0-9]+) ') { return [int]$Matches[1] }
    }
    return $ManifestIndex
}

function Get-SdhFeatureCell {
    param([string]$Repo, [string]$ViewPath, [string]$IntakePath, [string]$Status, [object]$Manifest, [string]$ResolvedIntakePath = $IntakePath)

    $candidates = [System.Collections.Generic.List[string]]::new()
    $specsRoot = Join-Path $Repo 'specs'
    if ($Status -ceq 'Completed' -and (Test-Path -LiteralPath $specsRoot -PathType Container)) {
        $needle = ([char]0x60) + $IntakePath + ([char]0x60)
        Get-ChildItem -LiteralPath $specsRoot -Directory -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -match '^\d{3}-.+' } |
            Sort-Object Name |
            ForEach-Object {
                $specFile = Join-Path $_.FullName 'spec.md'
                if (-not (Test-Path -LiteralPath $specFile -PathType Leaf)) { return }
                $specText = Read-SdhStrictUtf8File -Path $specFile -Subject "specs/$($_.Name)/spec.md"
                $matched = [regex]::Split($specText, '\r?\n') | Where-Object {
                    $_ -match '^\*\*(Binding Input|Bindende Eingabe)( / (Binding Input|Bindende Eingabe))?\*\*:' `
                        -and $_.Contains($needle, [StringComparison]::Ordinal)
                }
                if ($matched) { $candidates.Add("specs/$($_.Name)") }
            }

        $archiveMatch = [regex]::Match([IO.Path]::GetFileName($ResolvedIntakePath), '\.([0-9]{3}-[^/]+)\.md$')
        if ($archiveMatch.Success) {
            $archiveFeature = "specs/$($archiveMatch.Groups[1].Value)"
            $stateFile = Join-Path $Repo "${archiveFeature}/autonomous-run-state.json"
            if (Test-Path -LiteralPath $stateFile -PathType Leaf) {
                Assert-SdhLinkedIntakeCompletionProof `
                    -Repo $Repo `
                    -LogicalPath $IntakePath `
                    -ResolvedPath $ResolvedIntakePath `
                    -StatePath "${archiveFeature}/autonomous-run-state.json"
                $candidates.Add($archiveFeature)
            }
        }

        if ($null -ne $Manifest -and 'featureEvidence' -cin @($Manifest.PSObject.Properties.Name)) {
            foreach ($proof in @($Manifest.featureEvidence | Where-Object { [string]$_.intakePath -ceq $IntakePath })) {
                $featurePath = [string]$proof.featurePath
                if (-not (Test-Path -LiteralPath (Join-Path $Repo $featurePath) -PathType Container)) {
                    throw "LIE008: Feature-Ziel fehlt / feature target is missing: ${IntakePath}"
                }
                $candidates.Add($featurePath)
            }
        }
    }

    if ($candidates.Count -eq 0) { return '— (kein Spec-Kit-Feature / no Spec Kit feature)' }
    if ($candidates.Count -gt 1) {
        throw "LIE008: mehrdeutiger Feature-Nachweis / ambiguous feature evidence: ${IntakePath}"
    }
    return Get-SdhMarkdownLink -Repo $Repo -ViewPath $ViewPath -TargetPath $candidates[0] -ExpectedType Directory
}

function Get-SdhLinkedIntakeOrderSection {
    param([string]$Repo, [string]$ManifestPath, [string]$ViewPath)

    $manifestRelative = [IO.Path]::GetRelativePath($Repo, $ManifestPath).Replace('\', '/')
    $manifest = Test-SdhLinkedIntakeManifest -Repo $Repo -ManifestPath $manifestRelative

    $knownPaths = @($manifest.orderedTargets | ForEach-Object { [string]$_.path })
    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add('<!-- secure-development-hardening-order:start -->')
    $lines.Add('## Verlinkte Lastenheft-Reihenfolge / Linked Requirements Order')
    $lines.Add('')
    $lines.Add('Diese Tabelle wird aus dem kanonischen Series-Manifest und ausdruecklicher Feature-Evidence erzeugt. Vollstaendige Dateinamen, direkte eingehende Kanten und sichtbare Positionen bleiben erhalten. Manuelle Abschnitte ausserhalb dieses Markers bleiben unberuehrt.')
    $lines.Add('')
    $lines.Add('*This table is generated from the canonical series manifest and explicit feature evidence. Complete filenames, direct incoming edges, and visible positions are preserved. Manual sections outside this marker remain unchanged.*')
    $lines.Add('')
    $lines.Add('| Position | Status | Lastenheft/Intake | Abhängigkeiten / Dependencies | Spec-Kit-Feature |')
    $lines.Add('|---:|---|---|---|---|')

    $manifestIndex = 0
    foreach ($target in $manifest.orderedTargets) {
        $manifestIndex++
        $path = [string]$target.path
        $status = [string]$target.status
        $role = [string]$target.role
        if ([string]::IsNullOrWhiteSpace($path) -or [string]::IsNullOrWhiteSpace($status) -or [string]::IsNullOrWhiteSpace($role)) {
            throw 'LIE002: Series-Ziel ist unvollstaendig / series target is incomplete'
        }
        $resolvedPath = Resolve-SdhLinkedIntakePath -Repo $Repo -LogicalPath $path
        $intakeLink = Get-SdhMarkdownLink -Repo $Repo -ViewPath $ViewPath -TargetPath $resolvedPath -ExpectedType File
        $displayPosition = Get-SdhDisplayPosition -IntakeFile (Join-Path $Repo $resolvedPath) -ManifestIndex $manifestIndex

        $incoming = @($manifest.dependencies | Where-Object { [string]$_.to -ceq $path })
        $dependencyCells = [System.Collections.Generic.List[string]]::new()
        foreach ($edge in $incoming) {
            $from = [string]$edge.from
            if ($from -cnotin $knownPaths -or [string]::IsNullOrWhiteSpace([string]$edge.kind) -or $edge.binding -isnot [bool]) {
                throw "LIE007: ungueltiges Dependency-Tupel / invalid dependency tuple: ${from}"
            }
            $resolvedFrom = Resolve-SdhLinkedIntakePath -Repo $Repo -LogicalPath $from
            $fromLink = Get-SdhMarkdownLink -Repo $Repo -ViewPath $ViewPath -TargetPath $resolvedFrom -ExpectedType File
            $binding = ([bool]$edge.binding).ToString().ToLowerInvariant()
            $safeKind = ConvertTo-SdhMarkdownText ([string]$edge.kind)
            $dependencyCells.Add("${fromLink} → current (``${safeKind}``, binding: ${binding})")
        }
        $dependencies = if ($dependencyCells.Count -eq 0) {
            '— (Root / keine direkte Abhängigkeit)'
        } else {
            $dependencyCells -join '<br>'
        }
        $featureCell = Get-SdhFeatureCell -Repo $Repo -ViewPath $ViewPath -IntakePath $path -Status $status -Manifest $manifest -ResolvedIntakePath $resolvedPath
        $safeStatus = ConvertTo-SdhMarkdownText $status
        $lines.Add("| ${displayPosition} | ${safeStatus} | ${intakeLink} | ${dependencies} | ${featureCell} |")
    }
    $lines.Add('<!-- secure-development-hardening-order:end -->')
    return ($lines -join "`n")
}

function Get-SdhOrderSection {
    param(
        [string]$Repo,
        [string]$ViewPath = 'Lastenheft_Abarbeitungsreihenfolge.md'
    )

    $manifest = Get-SdhIntakeSeriesManifest -Repo $Repo
    if ($manifest) {
        return Get-SdhLinkedIntakeOrderSection -Repo $Repo -ManifestPath $manifest -ViewPath $ViewPath
    }
    throw 'LIE004: eindeutiges Series-Manifest fehlt / unique series manifest is missing'
}

function New-SdhOrderFileCandidate {
    param([string]$Repo, [string]$ManifestPath, [string]$OutputPath)

    $section = Get-SdhLinkedIntakeOrderSection -Repo $Repo -ManifestPath (Join-Path $Repo $ManifestPath) -ViewPath $OutputPath
    $output = Join-Path $Repo $OutputPath
    if (Test-Path -LiteralPath $output -PathType Leaf) {
        $content = (Read-SdhStrictUtf8File -Path $output -Subject $OutputPath) -replace "`r`n?", "`n"
        if ($content -match '(?s)<!-- secure-development-hardening-order:start -->.*?<!-- secure-development-hardening-order:end -->') {
            $newContent = [regex]::Replace(
                $content,
                '(?s)<!-- secure-development-hardening-order:start -->.*?<!-- secure-development-hardening-order:end -->',
                [Text.RegularExpressions.MatchEvaluator]{ param($match) $section }
            )
        } else {
            $newContent = $content.TrimEnd() + "`n`n" + $section + "`n"
        }
    } else {
        $newContent = @"
# Lastenheft-Abarbeitungsreihenfolge / Requirements Processing Order

Diese Datei haelt die sichtbare Abarbeitungsreihenfolge der vorhandenen Lastenhefte fest. Sie ist eine Vorbereitung fuer spaetere Spec-Kit-Laeufe und startet selbst keinen Lauf.

*This file records the visible processing order of existing requirements documents. It prepares later Spec Kit runs and does not start a run by itself.*

$section
"@
    }
    return $newContent.TrimEnd("`r", "`n") + "`n"
}

function Get-SdhViewSemantics {
    param([string]$Content)

    $lines = [regex]::Split(($Content -replace "`r`n?", "`n"), "`n")
    $table = [Collections.Generic.List[string]]::new()
    $inside = $false
    foreach ($line in $lines) {
        if ($line.StartsWith('| Position | Status | Lastenheft/Intake |', [StringComparison]::Ordinal)) { $inside = $true; continue }
        if ($inside -and $line.StartsWith('|---', [StringComparison]::Ordinal)) { continue }
        if ($inside -and $line.StartsWith('|', [StringComparison]::Ordinal)) {
            $table.Add([regex]::Replace($line, '\]\([^)]*\)', ']'))
            continue
        }
        if ($inside) { break }
    }
    return $table -join "`n"
}

function Restore-SdhLinkedIntakeOutputs {
    param([string]$Repo, [string[]]$OutputPaths, [string]$BackupDirectory)

    for ($index = 0; $index -lt $OutputPaths.Count; $index++) {
        $target = Join-Path $Repo $OutputPaths[$index]
        $backup = Join-Path $BackupDirectory "${index}.file"
        $restoreTemp = Join-Path (Split-Path -Parent $target) (".sdh-restore-{0}-{1}.tmp" -f $PID, $index)
        if (Test-Path -LiteralPath $backup -PathType Leaf) {
            [IO.File]::Copy($backup, $restoreTemp, $true)
            [IO.File]::Move($restoreTemp, $target, $true)
        } else {
            if (Test-Path -LiteralPath $target) { Remove-Item -LiteralPath $target -Force }
            if (Test-Path -LiteralPath $restoreTemp) { Remove-Item -LiteralPath $restoreTemp -Force }
        }
    }
}

function Write-SdhExclusivePublishTemp {
    param([string]$Directory, [string]$Content)

    $bytes = [Text.UTF8Encoding]::new($false).GetBytes($Content)
    for ($attempt = 0; $attempt -lt 10; $attempt++) {
        $path = Join-Path $Directory ('.sdh-publish-{0}.tmp' -f [guid]::NewGuid().ToString('N'))
        $stream = $null
        $created = $false
        try {
            $stream = [IO.File]::Open($path, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::None)
            $created = $true
            $stream.Write($bytes, 0, $bytes.Length)
            $stream.Flush($true)
            $stream.Dispose()
            $stream = $null
            $item = Get-Item -LiteralPath $path -Force
            if ($item.LinkType) {
                throw 'LIE010: unsicherer Publication-Temp abgelehnt / unsafe publication temp rejected'
            }
            return $path
        } catch [IO.IOException] {
            if ($stream) { $stream.Dispose() }
            if ($created -and (Test-Path -LiteralPath $path)) { Remove-Item -LiteralPath $path -Force }
            continue
        } catch {
            if ($stream) { $stream.Dispose() }
            if ($created -and (Test-Path -LiteralPath $path)) { Remove-Item -LiteralPath $path -Force }
            throw
        }
    }
    throw 'LIE010: exklusiver Publication-Temp konnte nicht erzeugt werden / exclusive publication temp could not be created'
}

function Get-SdhLinkedIntakeInputPaths {
    param([string]$Repo, [string]$ManifestPath)

    $manifestText = Read-SdhStrictUtf8File -Path (Join-Path $Repo $ManifestPath) -Subject $ManifestPath
    $manifest = $manifestText | ConvertFrom-Json
    $paths = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $null = $paths.Add($ManifestPath)
    foreach ($target in @($manifest.orderedTargets)) {
        $logicalPath = [string]$target.path
        $null = $paths.Add($logicalPath)
        $null = $paths.Add((Resolve-SdhLinkedIntakePath -Repo $Repo -LogicalPath $logicalPath))
    }
    $specsRoot = Join-Path $Repo 'specs'
    if (Test-Path -LiteralPath $specsRoot -PathType Container) {
        Get-ChildItem -LiteralPath $specsRoot -Directory -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -cmatch '^[0-9]{3}-.+' } |
            ForEach-Object {
                $specFile = Join-Path $_.FullName 'spec.md'
                if (Test-Path -LiteralPath $specFile -PathType Leaf) { $null = $paths.Add("specs/$($_.Name)/spec.md") }
            }
    }
    foreach ($target in @($manifest.orderedTargets)) {
        $resolvedTarget = Resolve-SdhLinkedIntakePath -Repo $Repo -LogicalPath ([string]$target.path)
        $archiveMatch = [regex]::Match([IO.Path]::GetFileName($resolvedTarget), '\.([0-9]{3}-[^/]+)\.md$')
        if ($archiveMatch.Success) {
            $statePath = "specs/$($archiveMatch.Groups[1].Value)/autonomous-run-state.json"
            if (Test-Path -LiteralPath (Join-Path $Repo $statePath) -PathType Leaf) { $null = $paths.Add($statePath) }
            $archiveRoot = Join-Path $Repo 'specs/intake-authoring-archive/series'
            if (Test-Path -LiteralPath $archiveRoot -PathType Container) {
                Get-ChildItem -LiteralPath $archiveRoot -Recurse -File -Filter 'series-receipt.json' |
                    Where-Object { $_.Directory.Name -ceq $archiveMatch.Groups[1].Value } |
                    ForEach-Object { $null = $paths.Add([IO.Path]::GetRelativePath($Repo, $_.FullName).Replace('\', '/')) }
            }
        }
    }
    if ('featureEvidence' -cin @($manifest.PSObject.Properties.Name)) {
        foreach ($proof in @($manifest.featureEvidence)) { $null = $paths.Add([string]$proof.featurePath) }
    }
    return @($paths | Sort-Object)
}

function Get-SdhLinkedIntakeInputFingerprint {
    param([string]$Repo, [string]$ManifestPath)

    $builder = [Text.StringBuilder]::new()
    foreach ($relative in @(Get-SdhLinkedIntakeInputPaths -Repo $Repo -ManifestPath $ManifestPath)) {
        $fullPath = Join-Path $Repo $relative
        if (Test-Path -LiteralPath $fullPath -PathType Leaf) {
            $hash = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
            $null = $builder.Append($relative).Append([char]0).Append('file').Append([char]0).Append($hash).Append("`n")
        } elseif (Test-Path -LiteralPath $fullPath -PathType Container) {
            $null = $builder.Append($relative).Append([char]0).Append("directory`n")
        } else {
            $null = $builder.Append($relative).Append([char]0).Append("missing`n")
        }
    }
    $bytes = [Text.UTF8Encoding]::new($false).GetBytes($builder.ToString())
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant()
}

function Test-SdhFixtureFaultScope {
    param([string]$Repo)

    if (-not (Test-Path -LiteralPath (Join-Path $Repo '.sdh-linked-intake-test-fixture') -PathType Leaf)) { return $false }
    $comparison = if ($IsWindows) { [StringComparison]::OrdinalIgnoreCase } else { [StringComparison]::Ordinal }
    $resolvedRepo = Get-SdhPhysicalPath -BasePath $Repo -RelativePath ''
    $temporaryRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath()).TrimEnd([IO.Path]::DirectorySeparatorChar)
    return $resolvedRepo.StartsWith($temporaryRoot + [IO.Path]::DirectorySeparatorChar, $comparison)
}

function Invoke-SdhLinkedIntakeProjection {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)][string]$Repo,
        [Parameter(Mandatory)][string]$ManifestPath,
        [Parameter(Mandatory)][ValidateSet('Check', 'Write')][string]$Mode,
        [Parameter(Mandatory)][ValidateNotNullOrEmpty()][string[]]$OutputPaths
    )

    $script:SdhRenderResult = 'Failed'
    $script:SdhRenderWriteCount = 0
    $script:SdhRenderAttemptedWrites = 0
    if (-not (Test-Path -LiteralPath (Join-Path $Repo '.git') -PathType Container)) {
        throw 'LIE004: explizites Ziel ist kein Git-Repository / explicit target is not a Git repository'
    }
    if ($OutputPaths.Count -eq 0) { throw 'LIE002: mindestens eine Ausgabe ist erforderlich / at least one output is required' }
    $outputSet = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $null = Test-SdhLinkedIntakeManifest -Repo $Repo -ManifestPath $ManifestPath
    $inputPaths = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($inputPath in @(Get-SdhLinkedIntakeInputPaths -Repo $Repo -ManifestPath $ManifestPath)) { $null = $inputPaths.Add($inputPath) }
    foreach ($outputPath in $OutputPaths) {
        Assert-SdhSafeOutputPath -Repo $Repo -RelativePath $outputPath
        if (-not $outputSet.Add($outputPath)) { throw 'LIE006: doppelter Ausgabepfad / duplicate output path' }
        if ($inputPaths.Contains($outputPath)) { throw "LIE006: Ausgabe ueberlappt kanonische Eingabe / output overlaps canonical input: ${outputPath}" }
    }

    $workDirectory = Join-Path ([IO.Path]::GetTempPath()) ("sdh-linked-intake-{0}" -f [guid]::NewGuid())
    $backupDirectory = Join-Path $workDirectory 'backups'
    $publishTemps = [Collections.Generic.List[string]]::new()
    try {
        $manifestFullPath = Join-Path $Repo $ManifestPath
        $inputFingerprintBefore = Get-SdhLinkedIntakeInputFingerprint -Repo $Repo -ManifestPath $ManifestPath
        $candidates = [Collections.Generic.List[string]]::new()
        foreach ($outputPath in $OutputPaths) {
            $candidates.Add((New-SdhOrderFileCandidate -Repo $Repo -ManifestPath $ManifestPath -OutputPath $outputPath))
        }

        if ($OutputPaths.Count -gt 1) {
            for ($index = 0; $index -lt $OutputPaths.Count; $index++) {
                $leftPath = Join-Path $Repo $OutputPaths[$index]
                if (-not (Test-Path -LiteralPath $leftPath -PathType Leaf)) { continue }
                for ($other = $index + 1; $other -lt $OutputPaths.Count; $other++) {
                    $rightPath = Join-Path $Repo $OutputPaths[$other]
                    if (-not (Test-Path -LiteralPath $rightPath -PathType Leaf)) { continue }
                    $left = Get-SdhViewSemantics (Read-SdhStrictUtf8File -Path $leftPath -Subject $OutputPaths[$index])
                    $right = Get-SdhViewSemantics (Read-SdhStrictUtf8File -Path $rightPath -Subject $OutputPaths[$other])
                    if ($left -cne $right) { throw 'LIE011: Root- und Series-Ansicht widersprechen sich / root and series views disagree' }
                }
            }
        }

        $stale = [Collections.Generic.List[int]]::new()
        for ($index = 0; $index -lt $OutputPaths.Count; $index++) {
            $target = Join-Path $Repo $OutputPaths[$index]
            if (-not (Test-Path -LiteralPath $target -PathType Leaf) `
                -or (Read-SdhStrictUtf8File -Path $target -Subject $OutputPaths[$index]) -cne $candidates[$index]) {
                $stale.Add($index)
            }
        }
        if ($stale.Count -eq 0) {
            $script:SdhRenderResult = 'Current'
            return 'Current writes=0'
        }
        if ($Mode -ceq 'Check') {
            $script:SdhRenderResult = 'Stale'
            throw 'LIE009: erzeugte Ausgabe ist veraltet; Write-Modus ausfuehren / generated output is stale; run write mode'
        }

        New-Item -ItemType Directory -Path $backupDirectory -Force | Out-Null

        $fault = [string]$env:SDH_TEST_FAULT
        if ($fault) {
            if (-not (Test-SdhFixtureFaultScope -Repo $Repo)) {
                throw 'LIE010: Testfehlerinjektion ist nur in isolierten Temp-Fixtures erlaubt / test fault injection is limited to isolated temporary fixtures'
            }
            if ($fault -ceq 'source-drift') {
                [IO.File]::AppendAllText($manifestFullPath, ' ', [Text.UTF8Encoding]::new($false))
            } elseif ($fault -ceq 'input-drift') {
                $vanishPath = [string]$env:SDH_TEST_VANISH_PATH
                if (-not $vanishPath) { throw 'LIE010: Testziel fehlt / test target is missing' }
                Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $vanishPath -ExpectedType File
                [IO.File]::AppendAllText((Join-Path $Repo $vanishPath), "`n", [Text.UTF8Encoding]::new($false))
            } elseif ($fault -ceq 'vanish-target') {
                $vanishPath = [string]$env:SDH_TEST_VANISH_PATH
                if (-not $vanishPath) { throw 'LIE010: Testziel fehlt / test target is missing' }
                Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $vanishPath -ExpectedType File
                Remove-Item -LiteralPath (Join-Path $Repo $vanishPath) -Force
            } elseif ($fault -ceq 'containment-drift') {
                $vanishPath = [string]$env:SDH_TEST_VANISH_PATH
                if (-not $vanishPath) { throw 'LIE010: Testziel fehlt / test target is missing' }
                Assert-SdhSafeRepositoryPath -Repo $Repo -RelativePath $vanishPath -ExpectedType File
                $outsideTarget = Join-Path (Split-Path -Parent $Repo) (".sdh-outside-{0}.md" -f $PID)
                [IO.File]::WriteAllText($outsideTarget, "# outside`n", [Text.UTF8Encoding]::new($false))
                Remove-Item -LiteralPath (Join-Path $Repo $vanishPath) -Force
                New-Item -ItemType SymbolicLink -Path (Join-Path $Repo $vanishPath) -Target $outsideTarget | Out-Null
            } elseif ($fault -cne 'after-first-replace') {
                throw 'LIE010: unbekannte Testfehlerinjektion / unknown test fault injection'
            }
        }

        $inputFingerprintAfter = Get-SdhLinkedIntakeInputFingerprint -Repo $Repo -ManifestPath $ManifestPath
        if ($inputFingerprintBefore -cne $inputFingerprintAfter) {
            throw 'LIE010: kanonische Eingabemenge hat sich vor Publication geaendert / canonical input set changed before publication'
        }
        $null = Test-SdhLinkedIntakeManifest -Repo $Repo -ManifestPath $ManifestPath
        for ($index = 0; $index -lt $OutputPaths.Count; $index++) {
            Assert-SdhSafeOutputPath -Repo $Repo -RelativePath $OutputPaths[$index]
            $recheck = New-SdhOrderFileCandidate -Repo $Repo -ManifestPath $ManifestPath -OutputPath $OutputPaths[$index]
            if ($recheck -cne $candidates[$index]) { throw 'LIE010: Kandidat driftete vor Publication / candidate drifted before publication' }
        }

        for ($index = 0; $index -lt $OutputPaths.Count; $index++) {
            $target = Join-Path $Repo $OutputPaths[$index]
            $backup = Join-Path $backupDirectory "${index}.file"
            if (Test-Path -LiteralPath $target -PathType Leaf) { [IO.File]::Copy($target, $backup, $true) }
            $publishTemp = Write-SdhExclusivePublishTemp -Directory (Split-Path -Parent $target) -Content $candidates[$index]
            $publishTemps.Add($publishTemp)
        }

        $replaced = 0
        try {
            for ($index = 0; $index -lt $OutputPaths.Count; $index++) {
                $target = Join-Path $Repo $OutputPaths[$index]
                if ((Test-Path -LiteralPath $target -PathType Leaf) `
                    -and (Read-SdhStrictUtf8File -Path $target -Subject $OutputPaths[$index]) -ceq $candidates[$index]) {
                    Remove-Item -LiteralPath $publishTemps[$index] -Force
                    continue
                }
                [IO.File]::Move($publishTemps[$index], $target, $true)
                $replaced++
                $script:SdhRenderAttemptedWrites = $replaced
                if ($fault -ceq 'after-first-replace' -and $replaced -eq 1) {
                    throw 'LIE010: simulierte Publication fehlgeschlagen / simulated publication failed'
                }
            }
            for ($index = 0; $index -lt $OutputPaths.Count; $index++) {
                $target = Join-Path $Repo $OutputPaths[$index]
                if ((Read-SdhStrictUtf8File -Path $target -Subject $OutputPaths[$index]) -cne $candidates[$index]) {
                    throw 'LIE010: Post-Write-Verifikation fehlgeschlagen / post-write verification failed'
                }
            }
        } catch {
            Restore-SdhLinkedIntakeOutputs -Repo $Repo -OutputPaths $OutputPaths -BackupDirectory $backupDirectory
            $script:SdhRenderWriteCount = 0
            $message = [string]$_.Exception.Message
            if (-not $message.StartsWith('LIE010:', [StringComparison]::Ordinal)) {
                $message = "LIE010: Publication fehlgeschlagen; Altzustand wiederhergestellt / publication failed; prior state restored: ${message}"
            } else {
                $message += '; vollstaendiger Rollback / complete rollback'
            }
            throw $message
        }
        $script:SdhRenderWriteCount = $replaced
        $script:SdhRenderResult = 'Updated'
        return "Updated writes=${replaced}"
    } finally {
        foreach ($publishTemp in $publishTemps) {
            if (Test-Path -LiteralPath $publishTemp) { Remove-Item -LiteralPath $publishTemp -Force }
        }
        if (Test-Path -LiteralPath $workDirectory) { Remove-Item -LiteralPath $workDirectory -Recurse -Force }
    }
}


function Invoke-RequirementsIntakeGovernanceRender {
    [CmdletBinding(SupportsShouldProcess)]
    param(
        [Parameter(Mandatory)]
        [Alias('Repo')]
        [ValidateScript({ Test-Path -LiteralPath $_ -PathType Container })]
        [string]$RepositoryRoot,

        [Parameter(Mandatory)]
        [Alias('Manifest')]
        [string]$ManifestPath,

        [Alias('OrderOutput')]
        [string[]]$OutputPath = @(),

        [switch]$Write,

        [switch]$AllowDirty
    )

    $repo = (Resolve-Path -LiteralPath $RepositoryRoot).Path
    if (-not (Test-Path -LiteralPath (Join-Path $repo '.git'))) {
        throw 'LIE004: explizites Ziel ist kein Git-Repository / explicit target is not a Git repository'
    }

    $outputs = @($OutputPath)
    if ($outputs.Count -eq 0) {
        $seriesDirectory = [IO.Path]::GetDirectoryName($ManifestPath)
        $seriesOutput = if ([string]::IsNullOrEmpty($seriesDirectory)) {
            'order.md'
        } else {
            $seriesDirectory.Replace('\', '/') + '/order.md'
        }
        $outputs = @(
            'Lastenheft_Abarbeitungsreihenfolge.md',
            $seriesOutput
        )
    }

    $mode = 'Check'
    if ($Write -or $AllowDirty) {
        if ($PSCmdlet.ShouldProcess($repo, 'Publish linked intake order views')) {
            $mode = 'Write'
        }
    }

    $result = Invoke-SdhLinkedIntakeProjection -Repo $repo -ManifestPath $ManifestPath -Mode $mode -OutputPaths $outputs

    [pscustomobject]@{
        Result = [string]$script:SdhRenderResult
        Writes = [int]$script:SdhRenderWriteCount
        Repository = $repo
        Manifest = $ManifestPath
        Outputs = $outputs
    }
}

if ($MyInvocation.InvocationName -ne '.') {
    if ($Help) {
        @'
render-requirements-intake-governance.ps1 — verlinkte Intake-Ansichten rendern

Validiert Manifest, Pfade, Abhaengigkeiten und Feature-Nachweise. Jede Zeile
enthaelt Position, Status, Intake-Link, direkte Abhaengigkeiten und Feature-Nachweis.
Every row follows the common five-column contract. The default mode checks only;
and -Write publishes changed marker sections atomically.

Usage:
  pwsh -NoProfile -File scripts/render-requirements-intake-governance.ps1 [-WhatIf]
  pwsh -NoProfile -File scripts/render-requirements-intake-governance.ps1 -Write
  pwsh -NoProfile -File scripts/render-requirements-intake-governance.ps1 -Repo PATH -Manifest RELATIVE_PATH -OrderOutput RELATIVE_PATH -Write
'@
        exit 0
    }
    try {
        $invokeParameters = @{
            RepositoryRoot = $RepositoryRoot
            ManifestPath = $ManifestPath
            OutputPath = $OutputPath
            Write = $Write
            AllowDirty = $AllowDirty
            WhatIf = [bool]$WhatIfPreference
        }
        Invoke-RequirementsIntakeGovernanceRender @invokeParameters | ConvertTo-Json -Depth 5
    } catch {
        $safeMessage = ConvertTo-SdhPublicDiagnostic -Message ([string]$_.Exception.Message) -PrivatePath @($RepositoryRoot)
        [Console]::Error.WriteLine($safeMessage)
        exit 1
    }
}
