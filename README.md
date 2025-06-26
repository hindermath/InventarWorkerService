# C#/.NET Core für Windows Services und Linux Daemons
## 🖥️ Windows Services mit .NET:
Du kannst die Worker Service-Projektvorlage verwenden (dotnet new worker).
Durch Registrierung mit `UseWindowsService()` im HostBuilder kann der Dienst als Windows-Dienst installiert werden.
Mithilfe von sc create oder PowerShell kannst du den Service auf deinem System einrichten.

## 🐧 Linux Daemons:

Auf Linux nutzt du ebenfalls den Worker Service.
Statt über Windows-Dienste läuft der Prozess hier als systemd-Service.
Du erstellst dazu eine .service-Datei und platzierst sie unter `/etc/systemd/system/`.

### Beispiel für systemd-Datei:
```ini
[Unit]
Description=Mein .NET Worker
[Service]
ExecStart=/usr/bin/dotnet /pfad/zu/meinerApp.dll
Restart=always
User=meinuser
[Install]
WantedBy=multi-user.target
```
Einmal eingerichtet, kannst du sie mit sudo systemctl start|stop|enable meineApp steuern.
Das Coole ist: Du schreibst deine Anwendung einmal in C#, und durch das plattformunabhängige Hosting-Modell von .NET läuft sie auf beiden Systemen stabil und performant. Wenn du willst, zeige ich dir gerne, wie du das mit einem kleinen Worker-Projekt praktisch umsetzt.


Sie sagten
kann man mit C#/.Net Core Windows Services und Linux Deamons oder auch macOS Deamons erstellen?
Copilot sagte
Absolut — mit C# und .NET Core (bzw. dem modernen .NET ab Version 5+) kannst du nicht nur Windows Services und Linux Daemons, sondern auch macOS Daemons bauen. Das Ganze läuft über das flexible Hosting-Modell von .NET, das plattformübergreifend funktioniert.

🛠️ Was ist möglich?
Windows Services: Mit UseWindowsService() kannst du deine App als Dienst registrieren. Installation per sc.exe oder PowerShell.

Linux Daemons: Über systemd und .service-Dateien steuerbar.

## macOS Daemons: 
macOS nutzt sogenannte launchd agents/daemons (launchctl), und du kannst deine App mithilfe einer .plist-Datei einbinden.

### 🧾 Beispiel: LaunchAgent .plist für macOS
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" 
  "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Label</key>
    <string>com.example.dotnetapp</string>
    <key>ProgramArguments</key>
    <array>
        <string>/usr/local/bin/dotnet</string>
        <string>/Pfad/zu/deinerApp.dll</string>
    </array>
    <key>RunAtLoad</key>
    <true/>
    <key>KeepAlive</key>
    <true/>
</dict>
</plist>
```
Diese Datei speicherst du z.B. unter `~/Library/LaunchAgents/com.example.dotnetapp.plist` und aktivierst sie via launchctl load.
Das Beste daran: Dein Code bleibt praktisch gleich — der Host und die Konfiguration passen sich der jeweiligen Plattform an. Falls du magst, kann ich dir gern ein Beispielprojekt mit Worker-Template zeigen, das unter allen drei Systemen sauber läuft. Lust? 😄1

## Generelles Beispielprojekt
Gerne! Hier ist ein kleines Beispiel für ein plattformübergreifendes .NET Worker Service-Projekt, das unter Windows als Service, unter Linux als systemd-Daemon und unter macOS als launchd-Daemon laufen kann.

### 🧰 Schritt 1: Neues Worker-Projekt erstellen
```bash
dotnet new worker -n MyWorkerService
cd MyWorkerService
```
### 📝 Program.cs anpassen
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder(args)
.UseWindowsService()         // Aktiviert Windows-Dienstmodus
.UseSystemd()                // Aktiviert systemd-Unterstützung für Linux
.ConfigureServices(services =>
{
services.AddHostedService<Worker>();
})
.Build();

await host.RunAsync();
```
Die Methoden UseWindowsService() und UseSystemd() sind plattformabhängig – sie tun nichts, wenn die App auf der jeweiligen Plattform nicht läuft. Somit kannst du denselben Code auf allen drei Systemen nutzen!

### 🧠 Worker.cs (Beispiel-Logik)
```csharp
public class Worker : BackgroundService
{
private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker läuft: {time}", DateTimeOffset.Now);
            await Task.Delay(5000, stoppingToken);
        }
    }
}
```
### 🧪 Test lokal:
```bash
dotnet run
```
### 📦 Veröffentlichen:
```bash
dotnet publish -c Release -r win-x64 --self-contained false    # Für Windows
dotnet publish -c Release -r linux-x64 --self-contained false  # Für Linux
dotnet publish -c Release -r osx-x64 --self-contained false    # Für macOS
```
## Schritt-für-Schritt-Anleitung für alle drei Plattformen
Beispiele für die .service-Datei für Linux, eine .plist für macOS für den Autostart und das Ganze als Windows Service zu registrieren.
Hier sind praktische Schritt-für-Schritt-Anleitung für alle drei Plattformen:

### 🐧 Linux systemd .service-Datei
Erstelle die Datei:
```bash
sudo nano /etc/systemd/system/myworker.service
```
Inhalt der Datei:
```ini
[Unit]
Description=Mein .NET Worker
After=network.target

[Service]
ExecStart=/usr/bin/dotnet /pfad/zu/MyWorkerService.dll
WorkingDirectory=/pfad/zu
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=myworker
User=deinbenutzername
Environment=DOTNET_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```
Dienst aktivieren und starten:
```bash
sudo systemctl daemon-reexec
sudo systemctl enable myworker
sudo systemctl start myworker
```

### 🍏 macOS LaunchAgent .plist-Datei
Datei erstellen unter:
```bash
nano ~/Library/LaunchAgents/com.example.myworker.plist
```
Inhalt:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" 
"http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Label</key>
    <string>com.example.myworker</string>
    <key>ProgramArguments</key>
    <array>
        <string>/usr/local/bin/dotnet</string>
        <string>/Pfad/zu/MyWorkerService.dll</string>
    </array>
    <key>RunAtLoad</key>
    <true/>
    <key>KeepAlive</key>
    <true/>
    <key>WorkingDirectory</key>
    <string>/Pfad/zu</string>
    <key>StandardOutPath</key>
    <string>/tmp/myworker.log</string>
    <key>StandardErrorPath</key>
    <string>/tmp/myworker.err</string>
</dict>
</plist>
```
Agent aktivieren:
```bash
launchctl load ~/Library/LaunchAgents/com.example.myworker.plist
```

### 🪟 Windows Service registrieren
Veröffentlichen:
```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

Installieren mit sc.exe:
```cmd
sc create "MyWorkerService" binPath= "C:\Pfad\zu\dotnet.exe C:\Pfad\zu\MyWorkerService.dll"
```
Alternativ via PowerShell mit New-Service:
```powershell
New-Service -Name "MyWorkerService" `
            -BinaryPathName "C:\Pfad\zu\dotnet.exe C:\Pfad\zu\MyWorkerService.dll" `
-DisplayName "My Worker Service" `
-StartupType Automatic
```
Starten:
```cmd
net start MyWorkerService
```

## Eine kompakte Beispielstruktur für dein plattformübergreifendes Worker-Service-Projekt inklusive Konfigs und Setup-Skripten für Windows, Linux und macOS.

### 📁 Projektstruktur (Vorschlag)
```plaintext
MyWorkerService/
├── Worker.cs
├── Program.cs
├── MyWorkerService.csproj
├── configs/
│   ├── linux/
│   │   └── myworker.service
│   ├── macos/
│   │   └── com.example.myworker.plist
│   └── windows/
│       └── install_service.ps1
├── setup/
│   ├── setup_linux.sh
│   └── setup_macos.sh
```
### 🐧 setup_linux.sh (für systemd)
```bash
#!/bin/bash
SERVICE_NAME=myworker
USER_NAME=$USER
APP_DIR=/opt/myworker
DLL_PATH=$APP_DIR/MyWorkerService.dll

sudo mkdir -p "$APP_DIR"
sudo cp -r ./bin/Release/netX.X/publish/* "$APP_DIR"

sudo cp ./configs/linux/myworker.service /etc/systemd/system/$SERVICE_NAME.service
sudo sed -i "s|/pfad/zu/MyWorkerService.dll|$DLL_PATH|" /etc/systemd/system/$SERVICE_NAME.service
sudo sed -i "s|deinbenutzername|$USER_NAME|" /etc/systemd/system/$SERVICE_NAME.service

sudo systemctl daemon-reexec
sudo systemctl enable $SERVICE_NAME
sudo systemctl start $SERVICE_NAME
```
### 🍏 setup_macos.sh (für launchd)
```bash
#!/bin/bash
PLIST_NAME=com.example.myworker.plist
PLIST_TARGET=~/Library/LaunchAgents/$PLIST_NAME
APP_PATH="$(pwd)/bin/Release/netX.X/publish/MyWorkerService.dll"

cp ./configs/macos/$PLIST_NAME "$PLIST_TARGET"
sed -i '' "s|/Pfad/zu/MyWorkerService.dll|$APP_PATH|" "$PLIST_TARGET"

launchctl load "$PLIST_TARGET"
```
### 🪟 install_service.ps1 (für Windows)
```powershell
$serviceName = "MyWorkerService"
$displayName = "My .NET Worker Service"
$exePath = "C:\Pfad\zu\dotnet.exe C:\Pfad\zu\MyWorkerService.dll"

New-Service -Name $serviceName `
    -BinaryPathName $exePath `
    -DisplayName $displayName `
    -StartupType Automatic

Start-Service $serviceName
```

## 🧱 Schritt-für-Schritt: Dein eigenes .zip bauen
Erstelle die Projektstruktur:
```bash
dotnet new worker -n MyWorkerService
cd MyWorkerService
```
Füge die zusätzlichen Ordner und Dateien ein:
```bash
mkdir -p configs/linux configs/macos configs/windows setup
```
Kopiere den Inhalt aus meiner vorherigen Antwort in folgende Dateien:
```plaintext
configs/linux/myworker.service
configs/macos/com.example.myworker.plist
configs/windows/install_service.ps1
setup/setup_linux.sh
setup/setup_macos.sh
```
Passe Program.cs und Worker.cs gemäß dem plattformübergreifenden Worker-Modell an (siehe oben).

Erstelle das .zip-Archiv (z.B. auf macOS/Linux):
```bash
cd ..
zip -r MyWorkerService.zip MyWorkerService/
```
Oder auf Windows (in PowerShell):
```powershell
Compress-Archive -Path .\MyWorkerService\* -DestinationPath .\MyWorkerService.zip
```

### 📦 Fertig!