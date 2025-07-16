## Verwendung
1. **Console-App starten**: `dotnet run`
2. **Web API starten**: `dotnet run -- --api`

## API-Endpunkte
- `GET /api/inventar/machines` - Alle Maschinen
- `GET /api/inventar/machines/{machineName}` - Spezifische Maschine
- `GET /api/inventar/machines/{machineName}/hardware` - Hardware-Inventar
- `GET /api/inventar/machines/{machineName}/software` - Software-Inventar
- `GET /api/inventar/hardware-overview` - Hardware-Übersicht aller Maschinen

## Vorteile dieser Lösung
✅ **Wiederverwendung**: Nutzt die vorhandene -Klasse
✅ **Flexibilität**: Kann sowohl als Console-App als auch als Web API laufen
✅ **Swagger**: Automatische API-Dokumentation
✅ **CORS**: Bereit für Frontend-Integration
✅ **Fehlerbehandlung**: Strukturierte Fehlerantworten `DatabaseService`
Die API ermöglicht es Ihnen, die SQLite-Inventar-Daten über HTTP-Endpunkte abzurufen und in anderen Anwendungen oder Dashboards zu nutzen.
