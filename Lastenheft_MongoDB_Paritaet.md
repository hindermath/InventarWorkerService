# Lastenheft: Herstellung der MongoDB-Paritaet zum SqliteDbService

**Dokument-Status:** Entwurf
**Erstellt:** 2026-04-18
**Betrifft:** `InventarWorkerCommon/Services/Database/MongoDbService.cs`
**Prioritaet:** Niedrig (Erweiterung, aktuelle Schreibfunktionalitaet ist nicht betroffen)
**Herkunft:** Review des Lastenhefts "Vervollstaendigung der PostgreSQL-Implementierung", Punkt 7
**Reihenfolge:** 5 von 5

### Umsetzungsreihenfolge aller Lastenhefte

| Nr. | Lastenheft | Abhaengigkeit |
|-----|-----------|---------------|
| 1 | `Lastenheft_PostgreSQL_Implementation.md` | Keine |
| 2 | `Lastenheft_SQLite_ViewQuery_Bugfix.md` | Keine (unabhaengig, aber nach Nr. 1 geplant) |
| 3 | `Lastenheft_IDbService_Interface.md` | Setzt Nr. 1 voraus |
| 4 | `Lastenheft_Statistik_View_Lesemethoden.md` | Setzt Nr. 1 voraus |
| **5** | **`Lastenheft_MongoDB_Paritaet.md`** (dieses Dokument) | Keine direkte, logisch nach Nr. 1 |

---

## Ausgangslage

Der `MongoDbService` verfuegt aktuell nur ueber drei Methoden:

| Methode | Typ |
|---------|-----|
| `InitializeSoftwareMongoDatabase()` | Initialisierung |
| `InitializeHardwareMongoDatabase()` | Initialisierung |
| `SaveSoftwareInventoryAsync(int, SoftwareInventory)` | Schreiben |
| `SaveHardwareInventoryAsync(int, HardwareInventory)` | Schreiben |
| `FindSoftwareByNameAsync(int, string)` | Lesen (eingeschraenkt) |

Im Vergleich zum `SqliteDbService` (und nach Abschluss der PgSQL-Paritaet auch zum `PgSqlDbService`) fehlen alle Lese-, Lookup-, Maintenance- und Statistik-Methoden.

### Besonderheiten der MongoDB-Architektur

MongoDB verwendet eine andere Datenstruktur als die relationalen Services:
- **Zwei separate Datenbanken:** `SoftwareInventory` und `HardwareInventory`
- **Collections pro Maschine:** Jede Maschine hat eine eigene Collection (benannt nach `machineId`)
- **Dokumentenbasiert:** Daten werden als BSON-Dokumente gespeichert, nicht in relationalen Tabellen
- **Kein `Machines`-Management:** MongoDB speichert keine Maschinenstammdaten -- diese kommen aus SQLite/PostgreSQL

Diese Unterschiede bedeuten, dass nicht alle SQLite-Methoden 1:1 uebertragbar sind. Insbesondere Maschinen-CRUD und View-basierte Abfragen haben in MongoDB keine direkte Entsprechung.

---

## Anforderungen

### R-MONGO-01: Lesemethoden fuer Inventardaten

- **R-MONGO-01.1: `GetLatestHardwareInventoryAsync(int machineId)`**
  - Rueckgabe: Das neueste Hardware-Dokument der Collection `{machineId}` in der `HardwareInventory`-Datenbank.
  - Sortierung nach `_id` oder `CreatedAt` absteigend, Limit 1.

- **R-MONGO-01.2: `GetLatestSoftwareInventoryAsync(int machineId)`**
  - Rueckgabe: Das neueste Software-Dokument der Collection `{machineId}` in der `SoftwareInventory`-Datenbank.

- **R-MONGO-01.3: `FindHardwareByModelAsync(int machineId, string computerModel)`**
  - Analog zu `FindSoftwareByNameAsync`, aber fuer Hardware-Dokumente nach Computermodell.

### R-MONGO-02: Zaehler- und Existenz-Methoden

- **R-MONGO-02.1: `GetHardwareInventoryCountAsync(int machineId)`**
  - Rueckgabe: Anzahl der Hardware-Dokumente in der Collection des angegebenen Rechners.

- **R-MONGO-02.2: `GetSoftwareInventoryCountAsync(int machineId)`**
  - Rueckgabe: Anzahl der Software-Dokumente in der Collection des angegebenen Rechners.

- **R-MONGO-02.3: `HasHardwareInventoryRecordsAsync(int machineId)`**
  - Rueckgabe: `bool`, ob mindestens ein Hardware-Dokument existiert.

- **R-MONGO-02.4: `HasSoftwareInventoryRecordsAsync(int machineId)`**
  - Rueckgabe: `bool`, ob mindestens ein Software-Dokument existiert.

**Hinweis:** Die Signatur weicht bewusst von SQLite ab (`machineId`-Parameter), da MongoDB Collections pro Maschine verwendet. Falls ein `IDbService`-Interface spaeter eingefuehrt wird, muss fuer MongoDB ein separates Interface oder eine Adapter-Schicht gewaehlt werden.

### R-MONGO-03: Maintenance-Methode

- **R-MONGO-03.1: `CleanupOldRecordsAsync(int machineId, int daysToKeep = 30)`**
  - Loescht Dokumente aelter als `daysToKeep` Tage aus beiden Datenbanken fuer die angegebene `machineId`.

### R-MONGO-04: Auflistung vorhandener Collections

- **R-MONGO-04.1: `GetMachineIdsWithDataAsync()`**
  - Rueckgabe: `Task<List<int>>` -- Liste aller `machineId`-Werte, fuer die Collections existieren.
  - Ermoeglicht die Abfrage, welche Maschinen ueberhaupt Inventardaten in MongoDB haben.

---

## Nicht im Scope

- Maschinen-CRUD in MongoDB (Stammdaten bleiben in SQLite/PostgreSQL).
- View-Aequivalente (MongoDB hat keine Views im relationalen Sinne; Aggregation Pipelines koennen spaeter hinzugefuegt werden).
- Einfuehrung eines gemeinsamen Interfaces mit den relationalen Services (siehe separates Lastenheft `IDbService`).
- Aenderung der bestehenden Collection-Struktur (eine Collection pro `machineId`).

---

## Akzeptanzkriterien

| ID | Kriterium |
|----|-----------|
| AK-MONGO-01 | Lesemethoden fuer Hardware- und Software-Inventar geben korrekte Dokumente zurueck. |
| AK-MONGO-02 | Zaehler- und Existenz-Methoden liefern korrekte Ergebnisse. |
| AK-MONGO-03 | `CleanupOldRecordsAsync` loescht ausschliesslich Dokumente aelter als die angegebene Schwelle. |
| AK-MONGO-04 | `GetMachineIdsWithDataAsync` gibt alle Collection-Namen zurueck, die gueltige `machineId`-Werte darstellen. |
| AK-MONGO-05 | Bestehende Schreibmethoden funktionieren unveraendert. |
| AK-MONGO-06 | Unit-Tests fuer alle neuen Methoden sind vorhanden und gruen. |
| AK-MONGO-07 | `dotnet build` laeuft ohne Warnungen (bezogen auf den neuen Code) durch. |

---

## Hinweis fuer Lernende

**Deutsch:** Dieses Feature zeigt die Unterschiede zwischen dokumentenbasierten (MongoDB) und relationalen (SQLite/PostgreSQL) Datenbanken. Waehrend relationale Services eine einheitliche Tabellen-API haben, erfordert MongoDB ein anderes Abfragemuster: Collections statt Tabellen, Filter-Builder statt SQL WHERE-Klauseln, und Dokument-Projektion statt SELECT-Spalten. "Paritaet" bedeutet hier nicht identische Signaturen, sondern aequivalente Funktionalitaet.

**English:** This feature highlights the differences between document-based (MongoDB) and relational (SQLite/PostgreSQL) databases. While relational services share a uniform table API, MongoDB requires different query patterns: collections instead of tables, filter builders instead of SQL WHERE clauses, and document projection instead of SELECT columns. "Parity" here means equivalent functionality, not identical signatures.
