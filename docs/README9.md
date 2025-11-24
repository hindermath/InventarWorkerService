# POCO - Plain Old CLR Object und ORM - Object-Relational Mapper

## Ein POCO in C# bedeutet Plain Old CLR Object.
👉 Das ist ein einfaches Objekt, das nur Daten und eventuell etwas Logik enthält, aber nicht von speziellen Basisklassen erbt und keine komplexen Framework-Abhängigkeiten hat.
Merkmale eines POCO:
- Keine Abhängigkeit von Frameworks
  - Es erbt nicht von Klassen wie EntityObject oder implementiert spezielle Interfaces, die ein Framework erzwingt.
- Nur Eigenschaften und Felder
  - Typischerweise bestehen POCOs aus Properties (z. B. `Id`, `Name`) und optional Methoden, die zur Geschäftslogik gehören.
- Leichtgewichtig
  - Sie sind einfach zu erstellen, zu testen und wiederzuverwenden.
- Flexibel
  - Können in ORMs wie Entity Framework oder NHibernate genutzt werden, ohne dass die Klassen selbst Framework-spezifisch sein müssen.
- Beispiel
  ```csharp
  public class Person
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Geburtsdatum { get; set; }
  }
  ```

## Das ist ein klassisches POCO:
- Keine Vererbung von speziellen Klassen.
- Nur Properties, die Daten beschreiben.
- Kann direkt in Entity Framework als Datenmodell verwendet werden, ohne zusätzliche Attribute oder Basisklassen.
### Warum wichtig?
- Trennung von Geschäftslogik und Infrastruktur: Deine Klassen bleiben unabhängig vom Datenbank- oder Framework-Code.
- Testbarkeit: POCOs sind leicht in Unit Tests einsetzbar.
- Wiederverwendbarkeit: Du kannst sie in verschiedenen Projekten oder Schichten nutzen.

## Praktische Anwendung  
Wenn man sich viel mit Datenanalyse und ORMs wie PostgreSQL beschäftigt, sind POCOs sind genau die Art von Klassen, die man als Datenmodelle in Entity Framework oder Dapper definiert, bevor diese mit SQL-Tabellen verknüpft werden.

## ORM steht für Object-Relational Mapping.
👉 Das ist ein Konzept (und meist eine Bibliothek oder ein Framework), das die Brücke zwischen objektorientierter Programmierung (z. B. in C# oder Java) und relationalen Datenbanken (z. B. PostgreSQL, MSSQL, MySQL) schlägt.

Die Idee dahinter:
- In der objektorientierten Welt arbeitest du mit Klassen und Objekten.
- In der Datenbankwelt gibt es Tabellen, Zeilen und Spalten.
- Ein ORM übersetzt automatisch zwischen diesen beiden Welten:
  - Klassen ↔ Tabellen
  - Objekte ↔ Zeilen
  - Properties ↔ Spalten
### Vorteile
- Weniger SQL schreiben: Du kannst mit Objekten arbeiten, statt manuell SQL-Abfragen zu formulieren.
- Produktivität: CRUD-Operationen (Create, Read, Update, Delete) werden stark vereinfacht.
- Portabilität: Dein Code ist weniger abhängig von einer bestimmten Datenbank.
- Wartbarkeit: Geschäftslogik bleibt klar getrennt von Datenbankzugriff.
  
### Beispiel mit Entity Framework (C#)
```csharp
// POCO-Klasse
public class Person
{
  public int Id { get; set; }
  public string Name { get; set; }
}

// DbContext für ORM
public class MyDbContext : DbContext
{
  public DbSet<Person> Personen { get; set; }
}

// Nutzung
using (var context = new MyDbContext())
{
var neuePerson = new Person { Name = "Thorsten" };
context.Personen.Add(neuePerson);
context.SaveChanges(); // ORM erzeugt automatisch das passende SQL
}
```
Hier musst du kein `INSERT INTO Personen ...` schreiben – das ORM erledigt das für dich.


## Beispiel: POCO + Dapper + SQLite
1. POCO-Klasse
```csharp
 public class Person
 {
   public int Id { get; set; }
   public string Name { get; set; }
 }
```

2. SQLite-Datenbank vorbereiten
Wir legen eine kleine Tabelle Personen an:
```csharp
using System.Data.SQLite;

var connectionString = "Data Source=personen.db;Version=3;";

using (var connection = new SQLiteConnection(connectionString))
{
connection.Open();

    var createTableSql = @"
        CREATE TABLE IF NOT EXISTS Personen (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL
        );";

    using (var cmd = new SQLiteCommand(createTableSql, connection))
    {
        cmd.ExecuteNonQuery();
    }
}
```

3. Dapper verwenden
```csharp
using Dapper;
using Microsoft.Data.SQLite;

// Verbindung öffnen
using (var connection = new SQLiteConnection("Data Source=personen.db;Version=3;"))
{
connection.Open();

    // INSERT mit Dapper
    var neuePerson = new Person { Name = "Thorsten" };
    connection.Execute("INSERT INTO Personen (Name) VALUES (@Name)", neuePerson);

    // SELECT mit Dapper (automatisches Mapping auf POCO)
    var personen = connection.Query<Person>("SELECT Id, Name FROM Personen").ToList();

    foreach (var p in personen)
    {
        Console.WriteLine($"{p.Id}: {p.Name}");
    }
}
```

### Ablauf
- POCO definiert deine Datenstruktur.
- SQLite speichert die Daten persistent.
- Dapper führt SQL aus und mappt die Ergebnisse direkt auf deine POCO-Klasse.
Das ist deutlich schlanker als Entity Framework: SQL selber schreiben, trotzdem komfortables Mapping und Parameterbindung.

## Bekannte ORMs
- Entity Framework Core (C#/.NET)
- NHibernate (C#/.NET)
- Hibernate (Java)
- SQLAlchemy (Python)
- Django ORM (Python)
