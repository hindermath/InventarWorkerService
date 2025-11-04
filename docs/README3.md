# 📚 Datenstrukturen und Algorithmen in C#

## 🧠 Begriffsdefinition

**Datenstruktur**: Eine Datenstruktur ist eine spezielle Art, Daten zu organisieren, zu speichern und zu verwalten, sodass sie effizient genutzt und verarbeitet werden können.

**Algorithmus**: Ein Algorithmus ist eine endliche Folge von klar definierten Anweisungen zur Lösung eines Problems oder zur Durchführung einer Aufgabe.

---

## 🗂️ Datenstrukturen – Beschreibung

Datenstrukturen sind essenziell für die effiziente Programmierung. Sie bestimmen, wie Daten gespeichert und abgerufen werden. Häufige Typen sind:

- **Arrays**: Feste Größe, schneller Zugriff über Index.
- **Listen**: Dynamisch erweiterbar, ideal für flexible Datenmengen.
- **Stacks**: LIFO-Prinzip (Last In, First Out).
- **Queues**: FIFO-Prinzip (First In, First Out).
- **Dictionaries**: Schlüssel-Wert-Paare für schnellen Zugriff.
- **Bäume und Graphen**: Für komplexe Beziehungen und Hierarchien.

### 🧪 Beispiel: Liste in C#

```csharp
using System; // Importiert den System-Namespace für grundlegende Klassen wie Console
using System.Collections.Generic; // Importiert den Namespace für generische Sammlungen wie List<T>

class Program // Definiert eine Klasse namens "Program" als Container für unseren Code
{ // Öffnende geschweifte Klammer - Beginn der Klassendefinition
    static void Main() // Definiert die Main-Methode - der Einstiegspunkt des Programms
    { // Öffnende geschweifte Klammer - Beginn der Main-Methode
        // Erstellt eine neue Instanz einer generischen Liste, die String-Elemente aufnehmen kann
        // List<string> = der Datentyp (eine Liste von Zeichenketten)
        // namen = der Variablenname für unsere Liste
        // new List<string>() = erzeugt eine neue, leere Liste im Arbeitsspeicher
        List<string> namen = new List<string>();
        
        // Fügt das String-Element "Anna" zur Liste hinzu
        // Add() ist eine Methode der List-Klasse, die ein neues Element am Ende anfügt
        namen.Add("Anna");
        
        // Fügt das String-Element "Ben" zur Liste hinzu
        // Die Liste enthält jetzt: ["Anna", "Ben"]
        namen.Add("Ben");
        
        // Fügt das String-Element "Clara" zur Liste hinzu  
        // Die Liste enthält jetzt: ["Anna", "Ben", "Clara"]
        namen.Add("Clara");

        // Startet eine foreach-Schleife, die über alle Elemente der Liste iteriert
        // foreach = Schleifen-Schlüsselwort für das Durchlaufen von Sammlungen
        // var = automatische Typenerkennung (hier wird es zu 'string')
        // name = Schleifenvariable, die in jedem Durchlauf das aktuelle Element enthält  
        // in = Schlüsselwort, das die Datenquelle für die Schleife angibt
        // namen = die Liste, über die wir iterieren
        foreach (var name in namen)
        { // Öffnende geschweifte Klammer - Beginn des Schleifenkörpers
            // Gibt das aktuelle Element (name) auf der Konsole aus
            // Console.WriteLine() ist eine statische Methode zur Textausgabe
            // In jedem Schleifendurchlauf wird ein anderer Name ausgegeben
            Console.WriteLine(name);
        } // Schließende geschweifte Klammer - Ende des Schleifenkörpers
    } // Schließende geschweifte Klammer - Ende der Main-Methode
} // Schließende geschweifte Klammer - Ende der Klassendefinition
```
## 🔍 **Zusätzliche Erklärungen zum Code:**
### **📋 Was ist List?**
- **List** ist eine **generische Sammlung** (Collection) in C#
- **Generisch** bedeutet: Du gibst beim Erstellen den Datentyp an (hier: `string`)
- **Dynamisch erweiterbar:** Die Größe passt sich automatisch an
- **Typsicher:** Kann nur Elemente des angegebenen Typs aufnehmen

### **🔄 Wie funktioniert die foreach-Schleife?**
1. **Erste Iteration:** `name = "Anna"` → gibt "Anna" aus
2. **Zweite Iteration:** `name = "Ben"` → gibt "Ben" aus
3. **Dritte Iteration:** `name = "Clara"` → gibt "Clara" aus
4. **Ende:** Keine weiteren Elemente → Schleife beendet

### **💡 Alternative Schreibweisen:**
```csharp
// Statt 'var' könnte man auch explizit 'string' verwenden:
foreach (string name in namen)

// Oder die Liste direkt bei der Erstellung befüllen:
List<string> namen = new List<string> { "Anna", "Ben", "Clara" };
```

## ⚙️ Algorithmen – Beschreibung

Algorithmen sind das Herzstück jeder Software. Sie definieren, wie Daten verarbeitet werden. Wichtige Kategorien:

*  Sortieralgorithmen: z.B. Bubble Sort, Merge Sort
*  Suchalgorithmen: z.B. Binäre Suche
*  Rekursionsverfahren: z.B. Fakultätsberechnung
*  Graphalgorithmen: z.B. Dijkstra, Tiefensuche

### 🔍 Beispiel: Binäre Suche in C#

```csharp
using System; // Importiert den System-Namespace, der grundlegende Klassen wie Console enthält

class Program // Definiert eine öffentliche Klasse namens "Program" - der Container für unseren Code
{
    // Definiert eine statische Methode für die binäre Suche
    // 'static' bedeutet: Die Methode gehört zur Klasse, nicht zu einer Objektinstanz
    // 'int' ist der Rückgabetyp: Die Methode gibt eine Ganzzahl zurück (den Index)
    // 'BinaereSuche' ist der Methodenname
    // Parameter: int[] array (das zu durchsuchende Array), int ziel (der gesuchte Wert)
    static int BinaereSuche(int[] array, int ziel)
    {
        int links = 0; // Initialisiert die linke Grenze des Suchbereichs mit Index 0 (erstes Element)
        int rechts = array.Length - 1; // Setzt die rechte Grenze auf den letzten gültigen Index des Arrays

        // Schleife läuft solange der linke Index kleiner oder gleich dem rechten Index ist
        // Wenn links > rechts wird, ist der Suchbereich leer und das Element wurde nicht gefunden
        while (links <= rechts)
        {
            // Berechnet den mittleren Index zwischen links und rechts
            // Division durch 2 gibt uns die Mitte des aktuellen Suchbereichs
            int mitte = (links + rechts) / 2;

            // Prüft, ob das Element in der Mitte genau unser gesuchtes Ziel ist
            if (array[mitte] == ziel)
                return mitte; // Erfolg! Gibt den Index zurück, wo das Element gefunden wurde
            // Prüft, ob das mittlere Element kleiner ist als unser Ziel
            else if (array[mitte] < ziel)
                links = mitte + 1; // Das Ziel muss in der rechten Hälfte sein - verschiebe linke Grenze
            else
                rechts = mitte - 1; // Das Ziel muss in der linken Hälfte sein - verschiebe rechte Grenze
        }

        return -1; // Element wurde nicht gefunden - konventioneller Rückgabewert für "nicht gefunden"
    }

    // Die Main-Methode - Einstiegspunkt des Programms
    // 'static void' bedeutet: statische Methode ohne Rückgabewert
    static void Main()
    {
        // Erstellt ein sortiertes Array von Ganzzahlen
        // WICHTIG: Binäre Suche funktioniert nur bei sortierten Arrays!
        int[] zahlen = { 1, 3, 5, 7, 9, 11 };
        
        // Ruft die BinaereSuche-Methode auf, um nach der Zahl 7 zu suchen
        // Das Ergebnis (Index) wird in der Variable 'index' gespeichert
        int index = BinaereSuche(zahlen, 7);
        
        // Verwendet einen ternären Operator (Kurzform von if-else)
        // Wenn index >= 0: gibt "Gefunden an Index {index}" aus
        // Sonst: gibt "Nicht gefunden" aus
        // Der $-Operator ermöglicht String-Interpolation (Einsetzen von Variablenwerten)
        Console.WriteLine(index >= 0 ? $"Gefunden an Index {index}" : "Nicht gefunden");
    }
}
```

## 🔍 **Zusätzliche Erklärungen zum Algorithmus:**
**Binäre Suche - Das Prinzip:**
- **Voraussetzung:** Das Array muss sortiert sein
- **Strategie:** "Teile und herrsche" - halbiert den Suchbereich in jedem Schritt
- **Effizienz:** O(log n) - sehr schnell auch bei großen Datenmengen
- **Funktionsweise:** Vergleicht das mittlere Element mit dem Ziel und entscheidet, in welcher Hälfte weitergesucht werden muss

**Warum funktioniert das?** Da das Array sortiert ist, wissen wir: Wenn das mittlere Element größer als unser Ziel ist, können alle Elemente rechts davon ebenfalls größer sein. Somit können wir die rechte Hälfte komplett ignorieren und nur in der linken Hälfte weitersuchen.

## 🧭 Fazit

Datenstrukturen und Algorithmen sind die Grundlage jeder effizienten Softwareentwicklung. Wer sie versteht, kann Programme schreiben, die nicht nur funktionieren, sondern auch skalierbar und performant sind.
