# Laufzeitsicht / Runtime View

## Positiver Ablauf / Positive Flow

DE: Client → autorisierter HTTP-Host → Controller → Inventardienst oder parametrisierter Datenbankdienst → camelCase-JSON. Harvester → erlaubtes HTTPS-Ziel → endlicher Request → providergetrennte Speicherung.

EN: Client → authorized HTTP host → controller → inventory service or parameterized database service → camelCase JSON. Harvester → allowed HTTPS target → finite request → provider-specific storage.

## Negativ- und Teilfehler / Negative and Partial Failure

DE: Fehlende Identität endet 401/403 vor Fachlogik. Fehlerhafte IDs/Namen enden 400. Interne Ausnahmen werden geloggt und außen generisch beantwortet. Ein unerlaubtes Ziel wird vor DNS/Netzwerk abgelehnt. Providerfehler werden nur für den betroffenen Provider gemeldet. Dateiänderungen schreiben temporär und ersetzen atomar; bei Fehler bleibt die alte Datei. Lesende Abläufe benötigen keinen Rollback.

EN: Missing identity ends with 401/403 before domain logic. Invalid IDs/names return 400. Internal exceptions are logged and external responses stay generic. A disallowed target is rejected before DNS/network. Provider failures are reported only for that provider. File changes write temporarily and replace atomically; the old file remains after failure. Read-only flows need no rollback.

## Evidenzlauf / Evidence Flow

DE: Accepted hashes → RED/Open → minimale Korrektur → fokussiertes GREEN → einmalige Vollregression → exact intended delivery. Eine Inputänderung invalidiert nur abhängige Evidenz.

EN: Accepted hashes → RED/Open → minimal correction → focused GREEN → one full regression → exact intended delivery. An input change invalidates only dependent evidence.
