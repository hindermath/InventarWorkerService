# Wichtige Konstrukte in C#

In C# hat der Operator ?: zwei unterschiedliche Bedeutungen, je nach Kontext:

## 🧠 1. Ternärer Operator (Conditional Operator)
Der ternäre Operator ist eine Kurzform für if-else und hat die Struktur:
Bedingung ? Ausdruck_wenn_true : Ausdruck_wenn_false;


### Beispiel:
```csharp
int zahl = 10;
string ergebnis = (zahl > 5) ? "Größer als 5" : "Kleiner oder gleich 5";
```

➡️ Wenn `zahl > 5` wahr ist, wird `"Größer als 5"` zugewiesen, sonst `"Kleiner oder gleich 5"`.

## 🧩 2. Null-Coalescing Operator (??) und Null-Conditional Operator (?.)
### 2.1 Null-Coalescing Operator (??)
Obwohl du nach ?: gefragt hast, wird das oft mit anderen ähnlichen Operatoren verwechselt:
- ?? → Null-Coalescing Operator: Gibt den rechten Wert zurück, wenn der linke null ist.

`  string name = eingabe ?? "Standardname";`
### 2.2 Null-Conditional Operator (?.)
- ?. → Null-Conditional Operator: Führt eine Methode oder Eigenschaft nur aus, wenn das Objekt nicht null ist.

`  int? laenge = text?.Length;`

### Fazit
Der ?:-Operator ist der ternäre Operator und dient als kompakte if-else-Alternative. Er ist besonders nützlich für einfache Entscheidungen direkt in einer Zuweisung oder Rückgabe.

## 3. Optionale Parameter
In C# ermöglichen optionale Parameter das Aufrufen von Methoden, ohne alle Argumente explizit anzugeben. Sie sind besonders nützlich, um Methoden flexibler und übersichtlicher zu gestalten.

### 🧩 Syntax für optionale Parameter
Du kannst einem Parameter einen Standardwert zuweisen:
```csharp
void Begrüßen(string name = "Gast")
{
    Console.WriteLine($"Hallo, {name}!");
}
```

### 🔹 Aufrufmöglichkeiten:
```csharp
Begrüßen();           // Ausgabe: Hallo, Gast!
Begrüßen("Thorsten"); // Ausgabe: Hallo, Thorsten!
```


📌 Regeln für optionale Parameter
- Sie müssen am Ende der Parameterliste stehen.
- Du kannst mehrere optionale Parameter definieren.
- Du kannst benannte Argumente verwenden, um bestimmte Parameter zu setzen:
```csharp
  void ZeigeInfo(string name = "Unbekannt", int alter = 0)
  {
  Console.WriteLine($"{name} ist {alter} Jahre alt.");
  }
ZeigeInfo(alter: 42); // Ausgabe: Unbekannt ist 42 Jahre alt.
```

### 🛠️ Anwendung in Konstruktoren und Interfaces
Optionale Parameter funktionieren auch in Konstruktoren und Interface-Methoden:
```csharp
interface IBeispiel
{
    void MachWas(int x = 10);
}
```

### 🧠 Alternative: Method Overloading
Statt optionaler Parameter kannst du auch Methoden überladen, z.B.:
```csharp
void Begrüßen() => Begrüßen("Gast");
void Begrüßen(string name) => Console.WriteLine($"Hallo, {name}!");
```

Das ist manchmal besser, wenn du komplexe Logik oder unterschiedliche Typen brauchst.
