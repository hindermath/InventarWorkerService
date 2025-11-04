# 🧠 Einführung in die objektorientierte Programmierung mit C#

## Was ist objektorientierte Programmierung?
Die objektorientierte Programmierung (kurz: OOP) ist ein Programmierkonzept, bei dem Software in Objekte unterteilt wird. Diese Objekte sind wie kleine Bausteine, die bestimmte Eigenschaften und Fähigkeiten besitzen. Man kann sich das vorstellen wie Lego-Steine, die zusammen ein größeres System bilden.
In C# basiert alles auf Klassen und Objekten.

🔧 Grundbegriffe der OOP

| Begriff     | Bedeutung                                                                |
|-------------|--------------------------------------------------------------------------|
| Klasse      | Eine Vorlage oder ein Bauplan für ein Objekt                             |
| Objekt      | Eine konkrete Instanz (Ausprägung) einer Klasse                          |
| Eigenschaft | Ein Merkmal eines Objekts (z.B. Name, Farbe, Größe)                      |
| Methode     | Eine Fähigkeit oder Aktion, die ein Objekt ausführen kann                |
| Konstruktor | Eine spezielle Methode, die beim Erstellen eines Objekts aufgerufen wird |


## 🐶 Beispiel: Eine Klasse für Hunde
```csharp
// Definition einer Klasse
public class Hund
{
    // Eigenschaften
    // öffentlich sichtbare Eigenschaft vom Datentyp string (Zeichenkette)
    public string Name; 
    // öffentlich sichtbare Eigenschaft vom Datentyp int (Ganzzahl)
    public int Alter;

    // Methode
    // öffentlich sichtbare Methode, die nichts zurückgibt. 
    // Das wird durch den Typ void definiert.
    public void Bellen()
    {
        // Ausgabe auf der Konsole
        Console.WriteLine($"{Name} bellt: Wuff!");
    }
}
```
Hier haben wir eine Klasse Hund, die zwei Eigenschaften (Name, Alter) und eine Methode (Bellen) besitzt.

## 🐾 Ein Objekt erstellen und verwenden
```csharp
class Program
{
    static void Main(string[] args)
    {
        // Ein Objekt vom Typ Hund erstellen
        // `Hund` ist der Datentyp des Objekts, 
        // genauer gesagt der Klasse `Hund`
        // `meinHund` ist die Zugriffsvariable auf das Objekt `Hund`
        // das rechts vom `=` mit `new Hund()` erstellt wird.
        Hund meinHund = new Hund();
        // Eigenschaften für die Objekt-Instanz `meinHund` festlegen
        meinHund.Name = "Bello";
        meinHund.Alter = 3;

        // Methode aufrufen
        meinHund.Bellen(); // Ausgabe: Bello bellt: Wuff!
    }
}
```

In diesem Beispiel erstellen wir ein Objekt meinHund und geben ihm einen Namen und ein Alter. Dann rufen wir die Methode Bellen() auf.

🧱 Warum OOP?
OOP hilft dabei, Programme:
- übersichtlich zu strukturieren
- wiederverwendbar zu machen
- leicht erweiterbar zu halten

 Statt alles in einer riesigen Datei zu schreiben, teilt man die Logik in kleine, verständliche Einheiten auf – wie Klassen für Hunde, Autos, Kunden usw.

## 🧪 Übung:
👉 Erstelle eine Klasse Auto mit den Eigenschaften Marke, Baujahr und einer Methode Fahren(), die eine passende Meldung ausgibt.

Ein mögliches Beispiel der Klasse Auto:
```csharp
public class Auto
{
    public string Marke;
    public string Modell;
    public int Baujahr;
    public int Kilometerstand;

    public void Fahren(int km)
    {
        Kilometerstand += km;
        Console.WriteLine($"{Marke} {Modell} ist {km} km gefahren.");
    }

    public void Anzeigen()
    {
        Console.WriteLine($"Marke: {Marke}, Modell: {Modell}, Baujahr: {Baujahr}, Kilometerstand: {Kilometerstand}");
    }
}
```