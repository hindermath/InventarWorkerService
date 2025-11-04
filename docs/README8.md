# Asynchrone Programmierung

Schnelle Antwort: In C# sind `async` und `await` Schlüsselwörter, die die asynchrone Programmierung vereinfachen, indem sie es Ihnen ermöglichen, nicht blockierenden Code zu schreiben, der wie synchroner Code aussieht. Microsoft Learn has excellent modules that guide you step by step through using Task, async, and await in real-world scenarios.

## 🌐 Zentrale Konzepte
`Task`: Repräsentiert eine Operation, die in der Zukunft abgeschlossen sein kann. Man kann es sich wie ein Versprechen auf ein Ergebnis vorstellen.

`async`: Markiert eine Methode als asynchron. Dadurch darf innerhalb der Methode await verwendet werden.

`await`: Hält die Ausführung der Methode an, bis der angegebene Task abgeschlossen ist – ohne den Thread zu blockieren.

## 🔑 Funktionsweise
### Eine asynchrone Methode deklarieren

```csharp
public async Task<int> GetDataAsync()
{
await Task.Delay(2000); // Simuliert eine Verzögerung von 2 Sekunden
return 42;
}
```
Die Methode gibt ein Task<int> statt eines int zurück.

`await Task.Delay(2000)` unterbricht die Ausführung und gibt den Thread für andere Arbeiten frei.

### Eine asynchrone Methode aufrufen

```csharp
int result = await GetDataAsync();
Console.WriteLine(result);
```
Das Schlüsselwort `await` entpackt das Ergebnis von `Task<int>` zu einem int.

### Fire-and-forget-Tasks

Wenn kein Rückgabewert benötigt wird, verwendet man Task statt Task<T>.

Beispiel:
```csharp
public async Task SaveFileAsync(string path, string content)
{
await File.WriteAllTextAsync(path, content);
}
```

## ⚡ Warum async/await verwenden?
Nicht blockierende Benutzeroberfläche: Anwendungen bleiben reaktionsfähig (z. B. in WPF, WinForms oder MAUI).

Skalierbarkeit: Threads in Webservern (ASP.NET Core) werden freigegeben, um mehr Anfragen zu bearbeiten.

Sauberer Code: Vermeidet „Callback-Hölle“ und lässt asynchronen Code wie synchronen aussehen.

## 📚 Microsoft Learn Ressourcen
Microsoft Learn bietet spezielle Module und Übungen dazu:

* Asynchronous programming with async and await in C#
* Perform I/O-bound work asynchronously with Task and async/await
* Handle multiple tasks and continuations

Diese Module enthalten interaktive Übungen, die direkt im Browser ausprobiert werden können.

# Einfaches Praxisbeispiel

## 🛠 Schritt 1: Ein einfaches Programmgerüst
```csharp
using System;
using System.Threading.Tasks;

class Program
{
static async Task Main(string[] args)
{
Console.WriteLine("Starte Programm...");

        int result = await BerechneWertAsync();

        Console.WriteLine($"Das Ergebnis ist: {result}");
        Console.WriteLine("Programm beendet.");
    }
}
```
👉 Hier siehst du:

`static async Task` Main → auch die Main-Methode kann asynchron sein.

Wir rufen `BerechneWertAsync()` mit `await` auf.

### 🛠 Schritt 2: Die asynchrone Methode
```csharp
static async Task<int> BerechneWertAsync()
{
    Console.WriteLine("Berechnung gestartet...");
    await Task.Delay(2000); // simuliert eine längere Arbeit (2 Sekunden warten)
    Console.WriteLine("Berechnung abgeschlossen!");
    return 42;
}
```
👉 Erklärung:

`Task<int>` bedeutet: Die Methode liefert irgendwann ein int zurück.

`await Task.Delay(2000)` pausiert die Methode, blockiert aber nicht den Thread.

### 🛠 Schritt 3: Ablauf
Wenn du das Programm startest, passiert Folgendes:

- "Starte Programm..." wird sofort ausgegeben.
- "Berechnung gestartet..." erscheint.
- Das Programm wartet 2 Sekunden (ohne Blockierung).
- "Berechnung abgeschlossen!" erscheint.
- Das Ergebnis 42 wird ausgegeben.
- "Programm beendet." wird angezeigt.

✨ Ergebnis: Du hast ein Programm, das asynchron arbeitet, aber trotzdem lesbar und linear aussieht.

Dann machen wir jetzt ein Beispiel mit mehreren parallelen Tasks, damit du siehst, wie man mehrere asynchrone Arbeiten gleichzeitig starten und anschließend auf alle warten kann.

## 🛠 Beispiel: Mehrere Aufgaben parallel starten
```csharp
using System;
using System.Threading.Tasks;

class Program
{
static async Task Main(string[] args)
{
Console.WriteLine("Starte parallele Aufgaben...");

        // Drei Tasks gleichzeitig starten
        Task<int> task1 = BerechneWertAsync(1, 2000);
        Task<int> task2 = BerechneWertAsync(2, 3000);
        Task<int> task3 = BerechneWertAsync(3, 1000);

        // Auf alle warten
        int[] results = await Task.WhenAll(task1, task2, task3);

        Console.WriteLine("Alle Aufgaben abgeschlossen!");
        Console.WriteLine($"Ergebnisse: {string.Join(", ", results)}");
    }

    static async Task<int> BerechneWertAsync(int nummer, int delay)
    {
        Console.WriteLine($"Task {nummer} gestartet...");
        await Task.Delay(delay); // simuliert Arbeit
        Console.WriteLine($"Task {nummer} fertig!");
        return nummer * 10;
    }
}
```
### 🔎 Ablauf
Alle drei Tasks (task1, task2, task3) starten sofort gleichzeitig.

`Task.WhenAll(...)` wartet, bis alle abgeschlossen sind.

Die Ergebnisse werden in einem Array gesammelt und ausgegeben.

👉 Beispielausgabe:

>Code
>>Starte parallele Aufgaben...

>>Task 1 gestartet...

>>Task 2 gestartet...

>>Task 3 gestartet...

>>Task 3 fertig!

>>Task 1 fertig!

>>Task 2 fertig!

>>Alle Aufgaben abgeschlossen!

>>Ergebnisse: 10, 20, 30

### 💡 Wichtige Punkte
Mit `Task.WhenAll` wartest du auf alle Tasks gleichzeitig.

Mit `Task.WhenAny` könntest du auch auf den ersten fertigen Task reagieren.

So lassen sich z.B. mehrere Webanfragen parallel starten und die Ergebnisse effizient verarbeiten.