## Wichtige Hinweise zur Verwendung:
1. **Assembly laden**: Das Terminal.Gui-Assembly muss mit `Add-Type -AssemblyName "Terminal.Gui"` geladen werden.
2. **Namespace-Verwendung**: In PowerShell müssen Sie die vollständigen Namespace-Namen verwenden (z.B. `[Terminal.Gui.Application]::Init()`).
3. **Event-Handler**: Event-Handler werden mit `add_EventName` Syntax hinzugefügt.
4. **Objekterstellung**: .NET-Objekte werden mit `[ClassName]::new()` erstellt.
5. **Statische Methoden**: Statische Methoden werden mit `[ClassName]::MethodName()` aufgerufen.

## Alternative: Wenn das NuGet-Paket nicht global verfügbar ist
```powershell
# Paket-Pfad explizit laden
$terminalGuiPath = "C:\Users\$env:USERNAME\.nuget\packages\terminal.gui\1.x.x\lib\net6.0\Terminal.Gui.dll"
Add-Type -Path $terminalGuiPath
```