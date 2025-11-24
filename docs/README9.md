# POCO - Plain Old CLR Object

## Ein POCO in C# bedeutet Plain Old CLR Object.
👉 Das ist ein einfaches Objekt, das nur Daten und eventuell etwas Logik enthält, aber nicht von speziellen Basisklassen erbt und keine komplexen Framework-Abhängigkeiten hat.
Merkmale eines POCO
- Keine Abhängigkeit von Frameworks
  Es erbt nicht von Klassen wie EntityObject oder implementiert spezielle Interfaces, die ein Framework erzwingt.
- Nur Eigenschaften und Felder
  Typischerweise bestehen POCOs aus Properties (z. B. Id, Name) und optional Methoden, die zur Geschäftslogik gehören.
- Leichtgewichtig
  Sie sind einfach zu erstellen, zu testen und wiederzuverwenden.
- Flexibel
  Können in ORMs wie Entity Framework oder NHibernate genutzt werden, ohne dass die Klassen selbst Framework-spezifisch sein müssen.
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
  Warum wichtig?
- Trennung von Geschäftslogik und Infrastruktur: Deine Klassen bleiben unabhängig vom Datenbank- oder Framework-Code.
- Testbarkeit: POCOs sind leicht in Unit Tests einsetzbar.
- Wiederverwendbarkeit: Du kannst sie in verschiedenen Projekten oder Schichten nutzen.

## Praktische Anwendung  
Wenn man sich viel mit Datenanalyse und ORMs wie PostgreSQL beschäftigt, sind POCOs sind genau die Art von Klassen, die man als Datenmodelle in Entity Framework oder Dapper definiert, bevor diese mit SQL-Tabellen verknüpft werden.
  