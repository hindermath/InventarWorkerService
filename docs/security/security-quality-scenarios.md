# Sicherheits-Qualitätsszenarien / Security Quality Scenarios

| ID | Szenario / Scenario | Messbares Ergebnis / Measurable response |
|---|---|---|
| SQ-01 | Unberechtigter HTTP-Aufruf / unauthorized call | 401/403, keine Daten und kein Stack-Trace / no data or stack trace |
| SQ-02 | Fehlerhafte ID/Name / malformed ID/name | 400, generischer Fehler; Secret-freies Log / generic error; secret-free log |
| SQ-03 | Unerlaubtes Ziel / disallowed target | vor Netzwerkzugriff abgelehnt; endlicher Timeout / rejected before network; finite timeout |
| SQ-04 | Provider-Ausfall / provider failure | keine übertragene Provider-Parität; sichere Teilfehlerinfo / no inferred parity; safe partial failure |
| SQ-05 | Teilwrite / partial write | alte Datei bleibt; Gate erneut / old file remains; rerun gate |
| SQ-06 | Veraltete Evidenz / stale evidence | Status wird Open; Owner/Trigger sichtbar / status becomes Open; owner/trigger visible |
| SQ-07 | Plattform / platform | explizite Plattformevidenz; keine Übertragung / explicit platform evidence; no inference |
| SQ-08 | A11Y | Textstatus, DE/EN, Lynx/Axe ohne anwendbaren Fehler / text status, DE/EN, no applicable failure |

DE: Owner ist die Projektverantwortung; Security-, Architektur- und A11Y-Reviewer prüfen. Offene Szenarien sind bis 2026-09-30 fällig.

EN: The project owner is accountable; security, architecture, and accessibility reviewers review. Open scenarios are due 2026-09-30.
