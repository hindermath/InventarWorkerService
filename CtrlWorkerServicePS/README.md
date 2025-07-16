## Wichtige Hinweise zur Verwendung:
1. **Assembly laden**: Das Terminal.Gui-Assembly muss mit `Add-Type -AssemblyName "Terminal.Gui"` geladen werden.
2. **Namespace-Verwendung**: In PowerShell müssen Sie die vollständigen Namespace-Namen verwenden (z.B. `[Terminal.Gui.Application]::Init()`).
3. **Event-Handler**: Event-Handler werden mit `add_EventName` Syntax hinzugefügt.
4. **Objekterstellung**: .NET-Objekte werden mit `[ClassName]::new()` erstellt.
5. **Statische Methoden**: Statische Methoden werden mit `[ClassName]::MethodName()` aufgerufen.

## Alternative: Wenn das NuGet-Paket nicht global verfügbar ist
```powershell
# Paket-Pfad explizit laden
$terminalGuiPath = "$env:HOME/.nuget/packages/terminal.gui/1.19.0/lib/net8.0/Terminal.Gui.dll"
$nStackPath = "$env:HOME/.nuget/packages/nstack.core/1.1.1/lib/netstandard2.0/NStack.dll"
Add-Type -Path $terminalGuiPath
Add-Type -Path $nStackPath
```