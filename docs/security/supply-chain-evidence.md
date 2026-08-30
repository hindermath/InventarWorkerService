# Supply-Chain-Evidenz / Supply Chain Evidence

## Kandidatenvertrag / Candidate Contract

DE: Der verteilbare Satz besteht aus Worker, Harvester und Viewer unter
`artifacts/release`. Der lokale Quellkandidat ist
`dc15bc4812245e71c1a5976b8241c7aeb518d4a9` plus aktuellem Arbeitsbaum,
lokale Version `1.2.1.65`. Die CI bindet den endgültigen Commit mit
`artifacts/source-sha.txt`, SHA-256-Hashes in `artifacts/release-sha256.txt` und
einer SPDX-3.0-SBOM im `_manifest`-Verzeichnis. Generierte `artifacts/` bleiben
untracked.

EN: The distributable set contains Worker, Harvester, and Viewer under
`artifacts/release`. The local source candidate is
`dc15bc4812245e71c1a5976b8241c7aeb518d4a9` plus the current working tree,
local version `1.2.1.65`. CI binds the final commit through
`artifacts/source-sha.txt`, SHA-256 hashes in `artifacts/release-sha256.txt`,
and an SPDX 3.0 SBOM in `_manifest`. Generated `artifacts/` remain untracked.

## Erzeugung, Validierung und Provenienz / Generation, Validation, and Provenance

```text
dotnet tool run sbom-tool -- generate -b artifacts/release -bc . -pn InventarWorkerService -pv <version> -ps Thorsten-Hindermann -mi SPDX:3.0
dotnet tool run sbom-tool -- validate -b artifacts/release -mi SPDX:3.0 -n true
find artifacts/release -type f -print0 | sort -z | xargs -0 sha256sum
```

DE: Builder ist der deklarierte GitHub-Actions-Runner `ubuntu-22.04` mit .NET
10 und Toolmanifest. Checkout, Restore, Build, Test, Publish, SBOM, Validierung
und Hashing sind getrennte Schritte. Der aktuelle SLSA-Iststand ist
quellgebundene Build-Evidenz; signierte Provenienz ist das Ziel und bleibt
providerseitiges `FollowUp`. Release-Entscheidung: lokal vorbereitet, aber erst
nach grünen exakten Providerjobs freigegeben.

EN: The builder is the declared GitHub Actions `ubuntu-22.04` runner with .NET
10 and the tool manifest. Checkout, restore, build, test, publish, SBOM,
validation, and hashing are separate steps. Current SLSA posture is
source-bound build evidence; signed provenance is the target and remains a
provider-side `FollowUp`. Release decision: locally prepared, but released only
after green exact provider jobs.

## VEX-artige Disposition / VEX-like Disposition

| Fund / Finding | Status | Begründung und Trigger / Rationale and Trigger |
|---|---|---|
| Microsoft.OpenApi 2.0.0 | fixed | Auf Microsoft-Paketlinie 10.0.11/auflösende Version 2.7.5 aktualisiert; erneutes CVE oder Restore-Drift öffnet den Fund. / Updated through the Microsoft 10.0.11 line/resolved 2.7.5; a new CVE or restore drift reopens it. |
| SharpCompress 0.30.1 | fixed | Transitiv auf 0.48.1 aktualisiert; Trigger ist ein neuer NuGet-Fund. / Transitively updated to 0.48.1; a new NuGet finding is the trigger. |
| Snappier 1.0.0 | fixed | Transitiv auf 1.3.1 aktualisiert. / Transitively updated to 1.3.1. |
| SQLitePCLRaw 2.1.11 | fixed | Transitiv auf 2.1.12 aktualisiert. / Transitively updated to 2.1.12. |
| System.Security.Cryptography.Xml 10.0.6 | fixed | Direkt auf 10.0.11 aktualisiert. / Directly updated to 10.0.11. |
| Provider-Scorecard und signierte Provenienz / Provider Scorecard and signed provenance | under investigation | Owner: Repository Maintainer; Reviewer: Security Reviewer; Termin/due 2026-09-15; Trigger: erster Providerlauf oder Release. |
| Historische Inventar-CSV / Historical inventory CSV | under investigation | Vollhistorischer Gitleaks-Scan meldet 16 mögliche Schlüsselwerte in Commit `028bfdc`. Die aktuelle Produktlogik redigiert Secret-Umgebungswerte; historische Credentials müssen extern geprüft und gegebenenfalls widerrufen werden. Owner: Security Reviewer; kritisch; Termin/due 2026-09-01; Trigger: Credential-Disposition. / Full-history Gitleaks reports 16 possible key values in commit `028bfdc`. Current product logic redacts secret environment values; historical credentials require external review and possible revocation. |

DE: Das aktuelle `--vulnerable --include-transitive`-Ergebnis meldet keine
bekannte Schwachstelle. Diese zeit- und scopegebundene Negativentscheidung gilt
nur für den genannten Restore und ersetzt keine spätere VEX-Neubewertung.

EN: The current `--vulnerable --include-transitive` result reports no known
vulnerability. This time- and scope-bound negative decision applies only to
the named restore and does not replace later VEX reassessment.

## OpenSSF und Secret-Scans / OpenSSF and Secret Scans

DE: Source-seitig sind schmale Workflow-Berechtigungen, Gitleaks,
`scan-agent-secrets.ps1`, eine Drei-Plattform-Matrix und getrennte Security-
Jobs belegt. Provider-Score, Branch-Schutz und die exakten beiden Scan-Logs
bleiben `FollowUp`; sie sind keine lokalen Pass-Behauptungen. `G-SUPPLY-001`
ist lokal vollständig vorbereitet und wird erst mit demselben finalen SHA in
den Providerlogs endgültig geschlossen.

EN: Source-visible evidence covers narrow workflow permissions, Gitleaks,
`scan-agent-secrets.ps1`, a three-platform matrix, and separate security jobs.
The provider score, branch protection, and exact logs from both scans remain
`FollowUp`; they are not local pass claims. `G-SUPPLY-001` is fully prepared
locally and closes only when provider logs bind the same final SHA.
