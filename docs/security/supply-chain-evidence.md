# Supply-Chain-Evidenz / Supply Chain Evidence

## Kandidatenvertrag / Candidate Contract

DE: Der verteilbare Satz besteht aus Worker, Harvester und Viewer unter
`artifacts/release`. Die CI liest die ausgerichtete Repository-Version und
bindet den endgültigen Commit mit
`artifacts/source-sha.txt`, SHA-256-Hashes in `artifacts/release-sha256.txt` und
einer SPDX-3.0-SBOM im `_manifest`-Verzeichnis. Generierte `artifacts/` bleiben
untracked.

EN: The distributable set contains Worker, Harvester, and Viewer under
`artifacts/release`. CI reads the aligned repository version and binds the
final commit through
`artifacts/source-sha.txt`, SHA-256 hashes in `artifacts/release-sha256.txt`,
and an SPDX 3.0 SBOM in `_manifest`. Generated `artifacts/` remain untracked.

## Erzeugung, Validierung und Provenienz / Generation, Validation, and Provenance

```text
dotnet tool run sbom-tool generate -b artifacts/release -bc . -pn InventarWorkerService -pv <version> -ps Thorsten-Hindermann -mi SPDX:3.0 -pm true
dotnet tool run sbom-tool generate -b artifacts/release -bc . -m artifacts/spdx-2.2-validation -pn InventarWorkerService -pv <version> -ps Thorsten-Hindermann -mi SPDX:2.2 -pm true
dotnet tool run sbom-tool validate -b artifacts/release -m artifacts/spdx-2.2-validation/_manifest -o artifacts/sbom-validation.json -mi SPDX:2.2 -n true
find artifacts/release -type f -print0 | sort -z | xargs -0 sha256sum
```

DE: Builder ist der deklarierte GitHub-Actions-Runner `ubuntu-22.04` mit .NET
10, nativer .NET-8-Toolruntime und Toolmanifest. Die CI validiert SPDX 3.0
inhaltlich auf Kontext, Paketpopulation und alle drei Release-Sätze. Da
`sbom-tool validate` 4.1.5 nur SPDX 2.2 unterstützt, prüft der offizielle
Validator zusätzlich eine Spiegeldatei aus demselben Build und wertet sein
JSON-Ergebnis explizit aus. Checkout, Restore, Build, Test, Publish, SBOM,
Validierung und Hashing sind getrennte Schritte. Der aktuelle SLSA-Iststand ist
quellgebundene Build-Evidenz; signierte Provenienz ist das Ziel und bleibt
providerseitiges `FollowUp`. Release-Entscheidung: lokal vorbereitet, aber erst
nach grünen exakten Providerjobs freigegeben.

EN: The builder is the declared GitHub Actions `ubuntu-22.04` runner with .NET
10, a native .NET 8 tool runtime, and the tool manifest. CI validates SPDX 3.0
content for its context, package population, and all three release sets. Since
`sbom-tool validate` 4.1.5 supports only SPDX 2.2, the official validator also
checks a mirror from the same build and its JSON result is evaluated
explicitly. Checkout, restore, build, test, publish, SBOM, validation, and
hashing are separate steps. Current SLSA posture is
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
| System.Security.Cryptography.Xml 10.0.11 | fixed | Beide direkten Pins wurden auf die aktuelle stabile Version `10.0.14` angehoben. Damit liegen die drei High-Advisories (`GHSA-r955-8q5j-3h83`, `GHSA-7hqp-7c5m-4vq6`, `GHSA-c399-9853-6jmj`) außerhalb ihres betroffenen Bereichs bis einschließlich `10.0.12`. Trigger: neuer Advisory-Fund oder Auflösungsdrift. / Both direct pins were raised to the current stable `10.0.14`, outside the three high-severity advisories' affected range through `10.0.12`. Trigger: a new advisory or resolution drift. |
| System.Security.Cryptography.Pkcs 10.0.11 | fixed | `System.Security.Cryptography.Xml 10.0.14` verlangt unter `net10.0` transitiv mindestens `System.Security.Cryptography.Pkcs 10.0.14`; damit liegen `GHSA-846f-f764-76mx` und `GHSA-2fmv-rv2c-59x7` außerhalb ihres betroffenen Bereichs. Ein zusätzlicher direkter Pin ist nicht erforderlich. Trigger: neuer Advisory-Fund oder Auflösungsdrift. / Under `net10.0`, `System.Security.Cryptography.Xml 10.0.14` transitively requires at least `System.Security.Cryptography.Pkcs 10.0.14`, outside both advisories' affected range. No additional direct pin is required. Trigger: a new advisory or resolution drift. |
| Provider-Scorecard und signierte Provenienz / Provider Scorecard and signed provenance | under investigation | Owner: Repository Maintainer; Reviewer: Security Reviewer; Termin/due 2026-09-15; Trigger: erster Providerlauf oder Release. |
| Historische Inventarexporte / Historical inventory exports | under investigation | Vier maschinenlokale CSV-Exporte und eine veraltete SQLite-Builddatei sind aus dem aktuellen Git-Stand entfernt und werden lokal ignoriert. Die Git-Historie bleibt absichtlich unverändert; Commit `028bfdc` enthält weiterhin mögliche Schlüsselwerte. Der zugeordnete Bing-Maps-Schlüssel wurde am 2026-10-29 nach Bestätigung des Repository-Owners deaktiviert. Die zwei historischen OpenAI-Schlüssel sind noch nicht als widerrufen oder ungültig belegt. Aktuelle Produktlogik redigiert Secret-Umgebungswerte. Owner: Security Reviewer; kritisch; Wiedervorlage 2026-11-05; Trigger: OpenAI-Credential-Disposition. / Four machine-local CSV exports and one obsolete SQLite build database are removed from the current Git snapshot and ignored locally. Git history intentionally remains unchanged; commit `028bfdc` still contains potential key values. The mapped Bing Maps key was disabled on 2026-10-29 after confirmation by the repository owner. The two historical OpenAI keys are not yet proven revoked or invalid. Current product logic redacts secret environment values. |

DE: Der erneute `--vulnerable --include-transitive`-Scan nach dem Restore von
`10.0.14` meldet keine bekannten verwundbaren Pakete. Die statische
Reichweitenprüfung fand keine direkte Verwendung von Signed-XML-, `SignedCms`-
oder `EnvelopedCms`-APIs; die Paketaktualisierung beseitigt den Befund dennoch
an der gemeinsamen Abhängigkeitsgrenze.

EN: The repeated `--vulnerable --include-transitive` scan after restoring
`10.0.14` reports no known vulnerable packages. Static reachability review
found no direct use of signed-XML, `SignedCms`, or `EnvelopedCms` APIs; the
package update nevertheless removes the finding at the shared dependency
boundary.

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
