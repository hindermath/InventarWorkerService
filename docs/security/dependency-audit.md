# Abhängigkeits-Audit / Dependency Audit

## Entscheidung / Decision

DE: Das Audit vom 30. August 2026 bindet sich an den lokalen Feature-Head
`dc15bc4812245e71c1a5976b8241c7aeb518d4a9`. Direkte und transitive Pakete
wurden mit `dotnet list ... --include-transitive`, `--outdated` und
`--vulnerable --include-transitive` geprüft. Nach den finding-bedingten Updates
meldet NuGet für alle elf Solution-Projekte keine bekannte Schwachstelle.

EN: The audit dated 30 August 2026 is bound to local feature head
`dc15bc4812245e71c1a5976b8241c7aeb518d4a9`. Direct and transitive packages
were checked with `dotnet list ... --include-transitive`, `--outdated`, and
`--vulnerable --include-transitive`. After finding-conditioned updates, NuGet
reports no known vulnerability for all eleven solution projects.

## Herkunft, Lizenzen und Wiederholbarkeit / Provenance, Licences, and Reproducibility

- `nuget.org` ist die erwartete öffentliche Registry. Eine zusätzlich aus der
  Benutzerkonfiguration gelesene DevExpress-Quelle ist `Open`: Sie wird von den
  Projektdateien nicht benötigt und muss vor Release im Providerlauf entfernt
  oder als organisatorisch genehmigt belegt werden. Owner: Repository Maintainer;
  Reviewer: Security Reviewer; Termin: 2026-09-15; Trigger: Restore oder Release.
- Paketlizenzen werden durch die SPDX-3.0-Erzeugung mit aktivierter
  Paketmetadaten-Erkennung erfasst. Fehlende Metadaten blockieren den
  `supply-chain`-Job. Proprietäre oder unbekannte Lizenzen benötigen eine
  dokumentierte Freigabe.
- Die exakten Direktversionen sind in den Projektdateien und die Tools in
  `dotnet-tools.json` gepinnt. NuGet-Lock-Dateien fehlen; dies ist `FollowUp`
  mit mittlerem Risiko, Owner Repository Maintainer, Termin 2026-09-30 und
  Trigger bei der nächsten Release-Vorbereitung.

- `nuget.org` is the expected public registry. An extra DevExpress source read
  from user configuration is `Open`: project files do not require it and the
  provider run must remove it or prove organisational approval before release.
  Owner: Repository Maintainer; reviewer: Security Reviewer; due 2026-09-15;
  trigger: restore or release.
- Package licences are captured by SPDX 3.0 generation with package metadata
  parsing enabled. Missing metadata blocks the `supply-chain` job. Proprietary
  or unknown licences require documented approval.
- Exact direct versions are pinned in project files and tools in
  `dotnet-tools.json`. NuGet lock files are absent; this is a medium-risk
  `FollowUp`, owner Repository Maintainer, due 2026-09-30, triggered by the next
  release preparation.

## Aktualisierungen und Ausnahmen / Updates and Exceptions

DE: Sicherheitsrelevante Microsoft-.NET-Pakete wurden auf `10.0.11`, MongoDB
Driver auf `3.11.1` und nach dem Outdated-Nachweis kompatible Patch-/Minor-
Versionen von Serilog, Swashbuckle, Dapper, Npgsql, PowerShell SDK und
Testwerkzeugen aktualisiert. Terminal.Gui `1.19.0` sowie YamlDotNet `16.3.0`
bleiben wegen Major-Version-Risiko vorerst gepinnt. Owner: Application
Maintainer; Risiko: mittel; Ablaufdatum: 2026-09-30; Trigger: eigener
Migrations-RED/GREEN-Lauf oder neues CVE.

EN: Security-relevant Microsoft .NET packages were updated to `10.0.11`, the
MongoDB Driver to `3.11.1`, and compatible patch/minor versions of Serilog,
Swashbuckle, Dapper, Npgsql, PowerShell SDK, and test tools after the outdated
proof. Terminal.Gui `1.19.0` and YamlDotNet `16.3.0` remain pinned because a
major-version migration needs dedicated evidence. Owner: Application
Maintainer; risk: medium; expiry: 2026-09-30; trigger: dedicated migration
RED/GREEN run or a new CVE.

## Automation und OpenSSF / Automation and OpenSSF

DE: Dependabot ist derzeit nicht konfiguriert; dies bleibt `FollowUp`, weil
Feature 002 keine neue externe Automation autorisiert. Dependency-Track ist
`N/A`, solange kein organisatorischer Ingestionsdienst betrieben wird; Trigger
ist dessen Einführung. Die quellseitig sichtbaren OpenSSF-Kriterien sind
Least-Privilege-Permissions, gepinnte Toolversionen, Secret-Scans und
Security-Tests. Branch-Schutz und providerseitige Scorecard-Werte bleiben
Remote-Closeout-Evidenz und dürfen lokal nicht als bestanden gelten.

EN: Dependabot is not configured and remains `FollowUp` because Feature 002
does not authorise a new external automation service. Dependency-Track is
`N/A` while no organisational ingestion service is operated; its introduction
is the trigger. Source-visible OpenSSF criteria include least-privilege
permissions, pinned tool versions, secret scans, and security tests. Branch
protection and provider-side Scorecard values remain remote-closeout evidence
and are not claimed as a local pass.

## SBOM-Werkzeug / SBOM Tool

DE: `Microsoft.Sbom.DotNetTool` `4.1.5` ist im lokalen Toolmanifest gepinnt und
`dotnet tool restore` war erfolgreich. Das Tool zielt auf .NET 8; auf dem
lokalen .NET-10-only-Runner wird deshalb `DOTNET_ROLL_FORWARD=Major` explizit
gesetzt. Die CI erzeugt und validiert SPDX 3.0 mit `sbom-tool generate` und
`sbom-tool validate`.

EN: `Microsoft.Sbom.DotNetTool` `4.1.5` is pinned in the local tool manifest and
`dotnet tool restore` succeeded. The tool targets .NET 8, so the local .NET
10-only runner explicitly sets `DOTNET_ROLL_FORWARD=Major`. CI generates and
validates SPDX 3.0 with `sbom-tool generate` and `sbom-tool validate`.

## Lokaler Abschluss / Local Close

DE: `dotnet restore InventarWorkerService.sln` war am 30. August 2026 ohne
Paketfehler erfolgreich. Der Release-Build war nach einer finding-bedingten
Importkorrektur mit null Warnungen und null Fehlern grün. Die später ergänzten
Coverlet-Collector bleiben auf der kompatiblen Testplattformversion gepinnt;
ein versuchtes Testframework-Update wurde nach nachgewiesenem Discovery-
Rückschritt zurückgenommen.

EN: `dotnet restore InventarWorkerService.sln` completed on 30 August 2026
without package errors. The release build passed with zero warnings and zero
errors after one finding-conditioned import correction. Added Coverlet
collectors remain pinned to the compatible test-platform version; an attempted
test-framework update was reverted after a proven discovery regression.
