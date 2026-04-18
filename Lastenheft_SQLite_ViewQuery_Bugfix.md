# Lastenheft: Bugfix fehlerhafte View-Abfragen im SqliteDbService

**Dokument-Status:** Entwurf
**Erstellt:** 2026-04-18
**Betrifft:** `InventarWorkerCommon/Services/Database/SqliteDbService.cs`
**Prioritaet:** Mittel (funktionaler Bug, liefert aktuell ungefilterte Daten statt View-Ergebnisse)
**Herkunft:** Review des Lastenhefts "Vervollstaendigung der PostgreSQL-Implementierung", Punkt 1
**Reihenfolge:** 2 von 5

### Umsetzungsreihenfolge aller Lastenhefte

| Nr. | Lastenheft | Abhaengigkeit |
|-----|-----------|---------------|
| 1 | `Lastenheft_PostgreSQL_Implementation.md` | Keine |
| **2** | **`Lastenheft_SQLite_ViewQuery_Bugfix.md`** (dieses Dokument) | Keine (unabhaengig, aber nach Nr. 1 geplant) |
| 3 | `Lastenheft_IDbService_Interface.md` | Setzt Nr. 1 voraus |
| 4 | `Lastenheft_Statistik_View_Lesemethoden.md` | Setzt Nr. 1 voraus |
| 5 | `Lastenheft_MongoDB_Paritaet.md` | Keine direkte, logisch nach Nr. 1 |

---

## Ausgangslage

Im `SqliteDbService` enthalten zwei Methoden fehlerhaftes SQL. Statt die definierten Views `AllDeprovisionedMachinesView` und `AllDisabledMachinesView` abzufragen, wird die Tabelle `Machines` mit einem Alias gelesen. Das fuehrt dazu, dass die WHERE-Bedingungen der Views nicht greifen und alle Maschinen zurueckgegeben werden.

| Methode | Zeile | Fehlerhaftes SQL | Erwartetes SQL |
|---------|-------|-----------------|---------------|
| `GetAllDeprovisionedMachinesAsync` | 496 | `SELECT * FROM Machines AllDeprovisionedMachinesView ORDER BY Name` | `SELECT * FROM AllDeprovisionedMachinesView ORDER BY Name` |
| `GetAllDisabledMachinesAsync` | 511 | `SELECT * FROM Machines AllDisabledMachinesView ORDER BY Name` | `SELECT * FROM AllDisabledMachinesView ORDER BY Name` |

Zum Vergleich: Die Methoden `GetAllActiveMachinesAsync` (Zeile 464) und `GetAllActiveMachinesWithNetworkInfoAsync` (Zeile 482) verwenden korrekt die jeweilige View.

---

## Anforderungen

### R-SQLBUG-01: Korrektur der View-Abfrage in `GetAllDeprovisionedMachinesAsync`

Die SQL-Abfrage in `SqliteDbService.GetAllDeprovisionedMachinesAsync()` muss korrigiert werden:

- **Ist:** `SELECT * FROM Machines AllDeprovisionedMachinesView ORDER BY Name`
- **Soll:** `SELECT * FROM AllDeprovisionedMachinesView ORDER BY Name`

### R-SQLBUG-02: Korrektur der View-Abfrage in `GetAllDisabledMachinesAsync`

Die SQL-Abfrage in `SqliteDbService.GetAllDisabledMachinesAsync()` muss korrigiert werden:

- **Ist:** `SELECT * FROM Machines AllDisabledMachinesView ORDER BY Name`
- **Soll:** `SELECT * FROM AllDisabledMachinesView ORDER BY Name`

### R-SQLBUG-03: Testabdeckung

Fuer beide korrigierten Methoden muessen Unit-Tests existieren, die sicherstellen, dass:
- Nur Maschinen mit `Disabled = 1 AND Deprovisioned = 1` von `GetAllDeprovisionedMachinesAsync` zurueckgegeben werden.
- Nur Maschinen mit `Disabled = 1 AND Deprovisioned = 0` von `GetAllDisabledMachinesAsync` zurueckgegeben werden.
- Maschinen mit abweichenden Flag-Kombinationen nicht im Ergebnis enthalten sind.

---

## Nicht im Scope

- Aenderungen an der PostgreSQL-Implementierung (wird separat im Lastenheft "PostgreSQL-Implementierung" behandelt).
- Aenderungen an den View-Definitionen selbst (diese sind korrekt).
- Refactoring oder Optimierung anderer Methoden im `SqliteDbService`.

---

## Akzeptanzkriterien

| ID | Kriterium |
|----|-----------|
| AK-SQLBUG-01 | `GetAllDeprovisionedMachinesAsync` gibt ausschliesslich Maschinen mit `Disabled = 1 AND Deprovisioned = 1` zurueck. |
| AK-SQLBUG-02 | `GetAllDisabledMachinesAsync` gibt ausschliesslich Maschinen mit `Disabled = 1 AND Deprovisioned = 0` zurueck. |
| AK-SQLBUG-03 | Unit-Tests fuer beide Methoden sind vorhanden und gruen. |
| AK-SQLBUG-04 | `dotnet build` laeuft ohne Warnungen durch. |

---

## Hinweis fuer Lernende

**Deutsch:** Dieser Bug zeigt ein haeufiges SQL-Muster: `SELECT * FROM TabelleA AliasB` ist syntaktisch korrekt und setzt einen Tabellen-Alias. Das Ergebnis ist jedoch voellig anders als `SELECT * FROM ViewB`. Solche Fehler fallen oft erst auf, wenn Filterbedingungen geprueft werden -- der Query laeuft fehlerfrei, liefert aber falsche Daten.

**English:** This bug demonstrates a common SQL pattern: `SELECT * FROM TableA AliasB` is syntactically valid and sets a table alias. The result, however, is completely different from `SELECT * FROM ViewB`. Such errors often go unnoticed until filter conditions are verified -- the query runs without errors but returns incorrect data.
