# Einführung in die asynchrone Programmierung

In C# wird eine asynchrone Methode mit den Schlüsselwörtern async und await deklariert. Diese ermöglichen es, lang laufende Operationen wie Netzwerkzugriffe oder Dateizugriffe auszuführen, ohne den Hauptthread zu blockieren.
## 🧠 Grundstruktur einer asynchronen Methode
```csharp
public async Task<string> GetContentAsync(string url)
{
    using (var client = new HttpClient())
    {
        var response = await client.GetStringAsync(url);
        return response;
    }
}
```

### 🔍 Erklärung der Bestandteile
- `async`: Kennzeichnet die Methode als asynchron.
- `Task<string>`: Gibt an, dass die Methode einen asynchronen Vorgang ausführt und ein string-Ergebnis zurückliefert. 
Alternativ kann auch Task (ohne Rückgabewert) oder void (nur für Event-Handler) verwendet werden.
- `await`: Wartet auf das Ergebnis eines Tasks, ohne den aktuellen Thread zu blockieren.
## ✅ Typische Einsatzszenarien
- Netzwerkoperationen (z.B. HTTP-Anfragen)
- Datenbankzugriffe
- Dateisystemoperationen
- CPU-intensive Berechnungen (mit `Task.Run`)

Weitere Details und Beispiele findest du im [Microsoft Learn Artikel](https://learn.microsoft.com/de-de/dotnet/csharp/asynchronous-programming/async-scenarios) zur asynchronen Programmierung 
oder im C# async/await Tutorial von [Devware](https://www.devware.de/content-hub/asynchrone-programmierung-in-csharp).
