# Lastenheft: Lesemethoden fuer Statistik-Views

**Dokument-Status:** Entwurf
**Erstellt:** 2026-04-18
**Betrifft:** `InventarWorkerCommon/Services/Database/SqliteDbService.cs`, `InventarWorkerCommon/Services/Database/PgSqlDbService.cs`
**Prioritaet:** Niedrig (Erweiterung, keine bestehende Funktionalitaet betroffen)
**Herkunft:** Review des Lastenhefts "Vervollstaendigung der PostgreSQL-Implementierung", Punkt 3
**Abhaengigkeit:** Setzt die abgeschlossene PostgreSQL-Paritaet voraus (Lastenheft_PostgreSQL_Implementation.md)
**Reihenfolge:** 4 von 5

### Umsetzungsreihenfolge aller Lastenhefte

| Nr. | Lastenheft | Abhaengigkeit |
|-----|-----------|---------------|
| 1 | `Lastenheft_PostgreSQL_Implementation.md` | Keine |
| 2 | `Lastenheft_SQLite_ViewQuery_Bugfix.md` | Keine (unabhaengig, aber nach Nr. 1 geplant) |
| 3 | `Lastenheft_IDbService_Interface.md` | Setzt Nr. 1 voraus |
| **4** | **`Lastenheft_Statistik_View_Lesemethoden.md`** (dieses Dokument) | Setzt Nr. 1 voraus |
| 5 | `Lastenheft_MongoDB_Paritaet.md` | Keine direkte, logisch nach Nr. 1 |

---

## Ausgangslage

Sowohl der `SqliteDbService` als auch der `PgSqlDbService` erstellen beim Initialisieren mehrere Statistik-Views:

| View | Inhalt |
|------|--------|
| `ComputerModelStatisticsView` | Verteilung nach Computermodell mit Anzahl, Prozentsatz, Zeitraum |
| `ArchitectureStatisticsView` | Verteilung nach Architektur mit Kernen, Speicher, Zeitraum |
| `ModelArchitectureStatisticsView` | Kombinierte Verteilung Modell x Architektur |
| `HardwareStatisticsOverview` | Vereinigte Uebersicht (Modell + Architektur als Kategorien) |

Diese Views existieren in der Datenbank, aber es gibt **keine C#-Methoden**, um sie abzufragen. Die Daten sind derzeit nur ueber direkten SQL-Zugriff erreichbar.

---

## Anforderungen

### R-STAT-01: Lesemethode fuer ComputerModelStatisticsView

- **Methode:** `GetComputerModelStatisticsAsync()`
- **Rueckgabe:** `Task<List<ComputerModelStatistic>>`
- **Neues Modell:** `ComputerModelStatistic` mit Properties: `ComputerModel`, `AnzahlMaschinen`, `EinzigartigeMaschinen`, `Prozentsatz`, `ErsteErfassung`, `LetzteErfassung`

### R-STAT-02: Lesemethode fuer ArchitectureStatisticsView

- **Methode:** `GetArchitectureStatisticsAsync()`
- **Rueckgabe:** `Task<List<ArchitectureStatistic>>`
- **Neues Modell:** `ArchitectureStatistic` mit Properties: `Architecture`, `AnzahlMaschinen`, `EinzigartigeMaschinen`, `Prozentsatz`, `DurchschnittlicheKerne`, `DurchschnittlicherSpeicherGB`, `ErsteErfassung`, `LetzteErfassung`

### R-STAT-03: Lesemethode fuer ModelArchitectureStatisticsView

- **Methode:** `GetModelArchitectureStatisticsAsync()`
- **Rueckgabe:** `Task<List<ModelArchitectureStatistic>>`
- **Neues Modell:** `ModelArchitectureStatistic` mit Properties: `ComputerModel`, `Architecture`, `AnzahlMaschinen`, `EinzigartigeMaschinen`, `Prozentsatz`, `DurchschnittlicheKerne`, `DurchschnittlicherSpeicherGB`, `ErsteErfassung`, `LetzteErfassung`

### R-STAT-04: Lesemethode fuer HardwareStatisticsOverview

- **Methode:** `GetHardwareStatisticsOverviewAsync()`
- **Rueckgabe:** `Task<List<HardwareStatisticOverview>>`
- **Neues Modell:** `HardwareStatisticOverview` mit Properties: `Kategorie`, `Wert`, `Anzahl`, `Prozentsatz`

### R-STAT-05: Provider-Paritaet

Alle vier Methoden muessen sowohl im `SqliteDbService` als auch im `PgSqlDbService` mit identischen Signaturen implementiert werden.

### R-STAT-06: Modelle

Die neuen Modellklassen werden unter `InventarWorkerCommon/Models/SqlDatabase/` abgelegt. Property-Namen entsprechen den Spaltenaliases der Views.

---

## Nicht im Scope

- Aenderung der View-Definitionen (SQL bleibt unveraendert).
- REST-API-Endpunkte fuer Statistiken (kann als separates Feature folgen).
- Aenderung bestehender Methoden oder Modelle.

---

## Akzeptanzkriterien

| ID | Kriterium |
|----|-----------|
| AK-STAT-01 | Vier neue Lesemethoden sind in `SqliteDbService` und `PgSqlDbService` vorhanden. |
| AK-STAT-02 | Alle Methoden geben typisierte Modell-Listen zurueck (kein `dynamic` oder `BsonDocument`). |
| AK-STAT-03 | Vier neue Modellklassen unter `Models/SqlDatabase/` existieren mit vollstaendiger XML-Dokumentation. |
| AK-STAT-04 | Unit-Tests fuer alle Lesemethoden sind vorhanden und gruen. |
| AK-STAT-05 | `dotnet build` laeuft ohne Warnungen (bezogen auf den neuen Code) durch. |

---

## Hinweis fuer Lernende

**Deutsch:** Dieses Feature zeigt, wie bestehende Datenbank-Views ueber typisierte C#-Modelle zugaenglich gemacht werden. Der Vorteil gegenueber rohen SQL-Abfragen: IntelliSense, Compile-Time-Checks und einfachere Testbarkeit. Die Views selbst bleiben unveraendert -- nur die "Bruecke" zwischen Datenbank und Anwendungscode wird gebaut.

**English:** This feature demonstrates how existing database views are made accessible through typed C# models. The advantage over raw SQL queries: IntelliSense, compile-time checks, and easier testability. The views themselves remain unchanged -- only the "bridge" between database and application code is built.
