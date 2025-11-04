# Wichtige Datenstrukturen in C#

## Class
In C# ist eine Klasse (`class`) eine grundlegende Datenstruktur, die verwendet wird, um Objekte zu definieren. Sie dient als Bauplan, der beschreibt, welche Eigenschaften (`Felder`) und Verhaltensweisen (`Methoden`) ein Objekt haben kann.

### 🧱 Aufbau einer Klasse in C#
```csharp
public class Hund
{
// Eigenschaften (Felder)
public string Name;
public int Alter;

    // Methode
    public void Bellen()
    {
        Console.WriteLine("Wuff! Wuff!");
    }
}
```

### 🔍 Was steckt dahinter?
- Referenztyp: Klassen sind Referenztypen. Das bedeutet, wenn du ein Objekt einer Klasse erstellst, verweist die Variable auf einen Speicherort im Heap, nicht direkt auf die Daten.
- Instanziierung: Du erzeugst ein Objekt mit dem new-Schlüsselwort:
```csharp
  Hund meinHund = new Hund();
  meinHund.Name = "Bello";
  meinHund.Bellen(); // Ausgabe: Wuff! Wuff!
```
- Kapselung: Du kannst Felder privat machen und über Eigenschaften (`Properties`) darauf zugreifen, um Daten zu schützen und zu kontrollieren.
- Konstruktoren: Klassen können spezielle Methoden enthalten, die beim Erstellen eines Objekts ausgeführt werden, z.B.:
```csharp
  public Hund(string name, int alter)
  {
      Name = name;
      Alter = alter;
  }
```

### 🧠 Warum sind Klassen wichtig?
- Sie ermöglichen Modularität und Wiederverwendbarkeit.
- Sie bilden die Grundlage für Objektorientierte Programmierung (OOP) in C#.
- Du kannst mit ihnen komplexe Datenstrukturen modellieren – z.B. Kunden, Fahrzeuge, Tiere, Benutzerkonten usw.

## Interface
In C# ist ein Interface (dt. „Schnittstelle“) eine spezielle Datenstruktur, die einen Vertrag definiert: Sie legt fest, welche Methoden, Eigenschaften, Ereignisse oder Indexer ein Typ bereitstellen muss – ohne deren konkrete Implementierung.
### 📘 Was macht ein Interface aus?
- Es enthält nur die Signaturen von Mitgliedern, keine Logik.
- Eine Klasse oder Struktur, die ein Interface implementiert, muss alle Mitglieder des Interfaces definieren.
- Interfaces ermöglichen Abstraktion und Polymorphismus – zentrale Konzepte der objektorientierten Programmierung. 
### 🧪 Beispiel
```csharp
  // Interface-Definition
  public interface IFahrzeug
  {
  void Fahren();
  int Geschwindigkeit { get; set; }
  }

// Klasse, die das Interface implementiert
public class Auto : IFahrzeug
{
public int Geschwindigkeit { get; set; }

    public void Fahren()
    {
        Console.WriteLine("Das Auto fährt mit " + Geschwindigkeit + " km/h.");
    }
}
```

### 🔍 Warum Interfaces verwenden?
- Flexibilität: Eine Klasse kann mehrere Interfaces implementieren, auch wenn sie nur von einer anderen Klasse erben kann.
- Testbarkeit: Interfaces sind ideal für Unit-Tests und Dependency Injection.
- Erweiterbarkeit: Du kannst neue Klassen hinzufügen, die ein Interface implementieren, ohne bestehende Logik zu ändern.
### 🧠 Denkhilfe: Interface vs. Klasse
  | Merkmal | Interface | Klasse |
  |--------|------------|--------|
  | Implementierung | Keine | Enthält konkrete Logik |
  | Vererbung | Mehrfach möglich | Nur einfache Vererbung erlaubt |
  | Zweck | Vertrag/Definition | Bauplan für Objekte |

## Struct
In C# ist eine struct (Struktur) eine spezielle Datenstruktur, die verwendet wird, um kleine, effiziente Werttypen zu definieren. Sie ähnelt einer Klasse, hat aber einige entscheidende Unterschiede – vor allem in Bezug auf Speicherverhalten und Vererbung.

### 🧱 Was ist eine struct?
Eine `struct` ist ein Wertetyp, der Daten und zugehörige Funktionen kapseln kann. Sie wird direkt auf dem Stack gespeichert (nicht auf dem Heap wie Klassen) und eignet sich besonders für kleine, unveränderliche Datentypen.
```csharp
public struct Punkt
{
public int X;
public int Y;

    public Punkt(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X}, {Y})";
}
```


### 🔍 Eigenschaften von `struct` in C#
- Wertetyp: Wird direkt kopiert bei Zuweisung oder Übergabe an Methoden.
- Keine Vererbung: Kann nicht von anderen Strukturen oder Klassen erben (außer von System.ValueType).
- Kann Interfaces implementieren: Ermöglicht polymorphes Verhalten.
- Standardmäßig unveränderlich: Besonders geeignet für Datentypen wie Koordinaten, Farben, Zeitstempel usw.
- Konstruktoren erlaubt: Müssen alle Felder initialisieren.
- Keine Parameterlose Konstruktoren: Du kannst keinen eigenen parameterlosen Konstruktor definieren – C# stellt einen bereit.

### 🧠 Wann sollte man struct statt class verwenden?
| Verwende `struct` wenn...                             | Verwende `class` wenn...                             |
|-------------------------------------------------------|------------------------------------------------------|
| ...die Datenmenge klein ist                           | ...die Daten komplex oder groß sind                  |
| ...du Wertsemantik brauchst (Kopieren statt Referenz) | ...du Referenzsemantik brauchst (gemeinsame Instanz) |
| ...die Struktur unveränderlich sein soll              | ...du Vererbung oder polymorphe Logik brauchst       |

### 📚 Weitere Infos
Du kannst tiefer eintauchen in Microsofts offizielle Dokumentation zu Strukturtypen.

## Record
Ein `record` in C# ist eine moderne Datenstruktur, die speziell für immutable (unveränderliche) Objekte und wertbasierte Gleichheit entwickelt wurde. Sie wurde mit C# 9.0 eingeführt und ist ideal für Datenmodelle, bei denen der Fokus auf den Inhalt und nicht auf die Identität des Objekts liegt.

### 🧾 Was ist ein `record`?
Ein `record` ist ein Referenztyp (wie eine Klasse), aber mit besonderen Eigenschaften:
- Wertbasierte Gleichheit: Zwei record-Objekte gelten als gleich, wenn ihre Inhalte gleich sind – nicht nur, wenn sie dieselbe Instanz sind.
- Immutability by default: Die Eigenschaften eines record sind standardmäßig nur beim Erstellen festlegbar (init statt set).
- Automatisch generierte Methoden: Der Compiler erstellt automatisch Equals, GetHashCode, ToString und sogar einen Konstruktor.

### 🧪 Beispiel: Ein einfacher `record`
public record Person(string Name, int Alter);

Du kannst ihn so verwenden:
```csharp
var p1 = new Person("Anna", 30);
var p2 = p1 with { Alter = 31 }; // erstellt eine Kopie mit geändertem Alter
Console.WriteLine(p2); // Ausgabe: Person { Name = Anna, Alter = 31 }
```
### 🔍 Vorteile von `record`
- ✅ Kürzere Syntax: Weniger Boilerplate-Code.
- ✅ Sicherer Code: Durch Unveränderlichkeit weniger Seiteneffekte.
- ✅ Ideal für DTOs und Modelle: Perfekt für APIs, Konfigurationen, Datenübertragung.

### 🧠 Vergleich: `record` vs. `class`
| Merkmal | `record` | `class` |
|--|--|--|
| Gleichheit | Wertbasiert (Equals nach Inhalt)| Referenzbasiert (Equals nach Adresse)|
| Mutierbarkeit | Standardmäßig unveränderlich | Standardmäßig veränderlich |
| Syntax | Kürzer, kompakter | Ausführlicher |
| Einsatzgebiet | Datenmodelle, DTOs | Logik, Verhalten, komplexe Objekte |

## Enum
Ein `enum` (kurz für Enumeration) in C# ist eine spezielle Datenstruktur, mit der du eine Gruppe von benannten Konstanten definieren kannst. Sie eignet sich hervorragend, wenn du eine feste Auswahl von Werten brauchst – z. B. Wochentage, Statuscodes oder Farben.

### 🧾 Was ist ein `enum`?
Ein `enum` ist ein Wertetyp, der intern auf einem ganzzahligen Typ basiert (standardmäßig int). Jeder Eintrag bekommt automatisch einen numerischen Wert, beginnend bei 0 – es sei denn, du gibst ihn explizit an.
```csharp
public enum Wochentag
{
    Montag,     // 0
    Dienstag,   // 1
    Mittwoch,   // 2
    Donnerstag, // 3
    Freitag     // 4
}
```

Du kannst dann so damit arbeiten:
```csharp
Wochentag heute = Wochentag.Mittwoch;
Console.WriteLine(heute); // Ausgabe: Mittwoch
Console.WriteLine((int)heute); // Ausgabe: 2
```


### 🧠 Warum `enum` verwenden?
- ✅ Lesbarkeit: Statt kryptischer Zahlen hast du sprechende Namen.
- ✅ Typensicherheit: Du kannst nur gültige Werte verwenden.
- ✅ Fehlervermeidung: Compiler warnt dich bei ungültigen Zuweisungen.
- ✅ Einfaches Mapping: Ideal für Switch-Statements oder Statusabfragen.

### 🧨 Erweiterung: `enum` mit `Flags`
Wenn du mehrere Werte gleichzeitig speichern willst (z.B. Rechte oder Optionen), kannst du das `[Flags]`-Attribut verwenden:
```csharp
[Flags]
public enum Rechte
{
    Keine = 0,
    Lesen = 1,
    Schreiben = 2,
    Ausführen = 4
}

Rechte userRechte = Rechte.Lesen | Rechte.Schreiben;
Console.WriteLine(userRechte); // Ausgabe: Lesen, Schreiben
```

### 🔍 Weitere Details
- Du kannst den zugrunde liegenden Typ ändern:

`  public enum StatusCode : byte { OK = 1, Fehler = 2 }`
- Du kannst mit `Enum.Parse` oder `Enum.TryParse` Strings in Enums umwandeln.
- Du kannst mit `Enum.IsDefined` prüfen, ob ein Wert gültig ist.

## Polymorphismus
### 🧠 Polymorphismus 
ist eines der zentralen Konzepte der objektorientierten Programmierung (OOP), neben Vererbung und Kapselung. Das Wort stammt aus dem Griechischen und bedeutet „Vielgestaltigkeit“. In der Programmierung beschreibt es die Fähigkeit, dass unterschiedliche Klassen auf die gleiche Schnittstelle reagieren können – also dass ein Objekt je nach Kontext unterschiedliche Formen annehmen kann.

### 💡 Bedeutung in OOP
Polymorphismus erlaubt es, Methoden in einer Basisklasse zu definieren und in abgeleiteten Klassen zu überschreiben. Dadurch kann man z. B. eine Methode wie Draw() aufrufen, ohne zu wissen, ob man gerade ein Circle, Rectangle oder Triangle zeichnet – die richtige Methode wird zur Laufzeit automatisch gewählt.

🧪 Beispiel in C#
```csharp
using System;
using System.Collections.Generic;

// Basisklasse
public class Shape
{
    public virtual void Draw()
    {
        Console.WriteLine("Zeichne eine generische Form.");
    }
}

// Abgeleitete Klasse: Kreis
public class Circle : Shape
{
    public override void Draw()
    {
        Console.WriteLine("Zeichne einen Kreis.");
    }
}

// Abgeleitete Klasse: Rechteck
public class Rectangle : Shape
{
    public override void Draw()
    {
        Console.WriteLine("Zeichne ein Rechteck.");
    }
}

// Hauptprogramm
class Program
{
    static void Main()
    {
        List<Shape> shapes = new List<Shape>
        {
            new Circle(),
            new Rectangle(),
            new Shape()
        };

        foreach (Shape shape in shapes)
        {
            shape.Draw(); // Polymorpher Aufruf
        }
    }
}
```

### 🖨️ Ausgabe
>Zeichne einen Kreis.

>Zeichne ein Rechteck.

>Zeichne eine generische Form.

### 🔍 Was passiert hier?
- Die Methode `Draw()` ist in der Basisklasse `Shape` als virtual deklariert.
- Die Klassen `Circle` und `Rectangle` überschreiben diese Methode mit `override`.
- Obwohl die Liste `shapes` nur Shape-Referenzen enthält, wird zur Laufzeit die jeweils passende `Draw()`-Methode aufgerufen – das ist Polymorphismus in Aktion.


## Abstraktion
### 🧠 Abstraktion 
in der objektorientierten Programmierung (OOP) bedeutet, dass man sich auf die wesentlichen Merkmale eines Objekts konzentriert und irrelevante Details ausblendet. Es geht darum, eine vereinfachte Sicht auf komplexe Systeme zu schaffen – ähnlich wie du beim Autofahren das Lenkrad benutzt, ohne zu wissen, wie der Motor intern funktioniert.

### 🔍 Was bedeutet das in C#?
In C# wird Abstraktion oft durch abstrakte Klassen und Interfaces umgesetzt. Eine abstrakte Klasse kann nicht direkt instanziiert werden und dient als Vorlage für andere Klassen. Sie definiert Methoden ohne konkrete Implementierung, die von den abgeleiteten Klassen ausgefüllt werden müssen.

🧪 Beispiel: Tiere und ihre Geräusche
```csharp
using System;

// Abstrakte Klasse
abstract class Animal
{
    public abstract void MakeSound(); // Abstrakte Methode
}

// Konkrete Klasse: Hund
class Dog : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Wuff!");
    }
}

// Konkrete Klasse: Katze
class Cat : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Miau!");
    }
}

// Hauptprogramm
class Program
{
    static void Main()
    {
        Animal myDog = new Dog();
        Animal myCat = new Cat();

        myDog.MakeSound(); // Ausgabe: Wuff!
        myCat.MakeSound(); // Ausgabe: Miau!
    }
}



### 🧩 Was passiert hier?
- `Animal` ist eine abstrakte Klasse, die eine Methode `MakeSound()` definiert, aber nicht implementiert.
- `Dog` und `Cat` erben von `Animal` und implementieren die Methode auf ihre eigene Weise.
- Das Hauptprogramm verwendet nur die abstrakte Sicht auf `Animal`, ohne sich um die Details der konkreten Klassen zu kümmern.

### 🎯 Fazit
Abstraktion hilft dir, komplexe Systeme übersichtlich zu gestalten, indem du nur das zeigst, was für den Benutzer relevant ist. Die interne Logik bleibt verborgen – und das ist oft genau das, was man will.
