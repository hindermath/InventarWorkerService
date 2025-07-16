

function Invoke-WorkerServiceControl {
    [CmdletBinding(DefaultParameterSetName = "Help")]
    [OutputType([string])]
    param(
        [Parameter(
            Mandatory = $false,
            Position = 0,
            ParameterSetName = "Start",
            HelpMessage = "Startet den Worker Service")]
        [switch]$Start,

        [Parameter(
            Mandatory = $false,
            Position = 0,
            ParameterSetName = "Stop",
            HelpMessage = "Stoppt den Worker Service")]
        [switch]$Stop,

        [Parameter(
            Mandatory = $false,
            Position = 0,
            ParameterSetName = "TUI",
            HelpMessage = "Startet die Terminal-Benutzeroberfläche")]
        [switch]$Tui,

        [Parameter(
            Mandatory = $false,
            HelpMessage = "Name des Services (Standard: 'mein-service')")]
        [ValidateNotNullOrEmpty()]
        [string]$ServiceName = "mein-service"
    )

    try {
        switch ($PSCmdlet.ParameterSetName) {
            "Start" { Start-WorkerService -ServiceName $ServiceName }
            "Stop" { Stop-WorkerService -ServiceName $ServiceName }
            "TUI" { Start-WorkerServiceTui -ServiceName $ServiceName }
            default { Show-WorkerServiceHelp }
        }
    }
    catch {
        Write-Error -Message $_.Exception.Message -Category OperationStopped
    }
}

function Start-WorkerService {
    param([string]$ServiceName)
    
    if ($VerbosePreference -eq "Continue") {
        Write-Verbose "Starte Service '$ServiceName'..."
    }
    
    try {
        Start-Service -Name $ServiceName -ErrorAction Stop
        Write-Output "Service '$ServiceName' wird gestartet..."
        
        if ($VerbosePreference -eq "Continue") {
            Write-Verbose "Service '$ServiceName' erfolgreich gestartet."
        }
    }
    catch {
        Write-Error -Message "Fehler beim Starten des Services '$ServiceName': $($_.Exception.Message)" -Category OperationStopped
    }
}

function Stop-WorkerService {
    param([string]$ServiceName)
    
    if ($VerbosePreference -eq "Continue") {
        Write-Verbose "Stoppe Service '$ServiceName'..."
    }
    
    try {
        Stop-Service -Name $ServiceName -ErrorAction Stop
        Write-Output "Service '$ServiceName' wird gestoppt..."
        
        if ($VerbosePreference -eq "Continue") {
            Write-Verbose "Service '$ServiceName' erfolgreich gestoppt."
        }
    }
    catch {
        Write-Error -Message "Fehler beim Stoppen des Services '$ServiceName': $($_.Exception.Message)" -Category OperationStopped
    }
}

# Terminal.Gui PowerShell Script für Service Controller


# Funktion zur Service-Steuerung (vereinfacht)
function Start-ServiceControl {
    param(
        [string]$ServiceName = "mein-service"
    )

    Write-Host "Service '$ServiceName' wird gestartet..." -ForegroundColor Green
    # Hier würde die tatsächliche Service-Logik stehen
    Start-Sleep -Seconds 2
    Write-Host "Service '$ServiceName' gestartet!" -ForegroundColor Green
}

function Stop-ServiceControl {
    param(
        [string]$ServiceName = "mein-service"
    )

    Write-Host "Service '$ServiceName' wird gestoppt..." -ForegroundColor Red
    # Hier würde die tatsächliche Service-Logik stehen
    Start-Sleep -Seconds 2
    Write-Host "Service '$ServiceName' gestoppt!" -ForegroundColor Red
}

function Start-TerminalGui {
    param(
        [string]$ServiceName = "mein-service"
    )
    # Lade das Terminal.Gui Assembly
    $terminalGuiPath = "$env:HOME/.nuget/packages/terminal.gui/1.19.0/lib/net8.0/Terminal.Gui.dll"
    $nStackPath = "$env:HOME/.nuget/packages/nstack.core/1.1.1/lib/netstandard2.0/NStack.dll"
    Add-Type -Path $terminalGuiPath
    Add-Type -Path $nStackPath

    try {
        # Terminal.Gui initialisieren
        [Terminal.Gui.Application]::Init()

        $top = [Terminal.Gui.Application]::Top

        # Hauptfenster erstellen
        $win = [Terminal.Gui.Window]::new("Service Controller (PowerShell Script)")
        $win.X = 0
        $win.Y = 1
        $win.Width = [Terminal.Gui.Dim]::Fill()
        $win.Height = [Terminal.Gui.Dim]::Fill()

        # Menüleiste erstellen
        $dateiMenu = [Terminal.Gui.MenuItem[]]@(
            [Terminal.Gui.MenuItem]::new("_Beenden", "", {
                [Terminal.Gui.Application]::RequestStop()
            })
        )

        $hilfeMenu = [Terminal.Gui.MenuItem[]]@(
            [Terminal.Gui.MenuItem]::new("_Info", "", {
                [Terminal.Gui.MessageBox]::Query("Info",
                        "Service Controller PowerShell Script v1.0`nService: $ServiceName`nZum Starten und Stoppen von Services", "OK")
            })
        )

        $menuItems = [Terminal.Gui.MenuBarItem[]]@(
            [Terminal.Gui.MenuBarItem]::new("_Datei", $dateiMenu),
            [Terminal.Gui.MenuBarItem]::new("_Hilfe", $hilfeMenu)
        )

        $menu = [Terminal.Gui.MenuBar]::new($menuItems)

        # Service-Name Label
        $serviceLabel = [Terminal.Gui.Label]::new("Service: $ServiceName")
        $serviceLabel.X = [Terminal.Gui.Pos]::Center() - 10
        $serviceLabel.Y = [Terminal.Gui.Pos]::Center() - 6
        $serviceLabel.ColorScheme = [Terminal.Gui.Colors]::Menu

        # Status-Label
        $statusLabel = [Terminal.Gui.Label]::new("Bereit...")
        $statusLabel.X = [Terminal.Gui.Pos]::Center() - 10
        $statusLabel.Y = [Terminal.Gui.Pos]::Center() - 4
        $statusLabel.ColorScheme = [Terminal.Gui.Colors]::Base

        # Start-Button
        $startButton = [Terminal.Gui.Button]::new("Service _Starten")
        $startButton.X = [Terminal.Gui.Pos]::Center() - 15
        $startButton.Y = [Terminal.Gui.Pos]::Center() - 2

        # Stop-Button
        $stopButton = [Terminal.Gui.Button]::new("Service St_oppen")
        $stopButton.X = [Terminal.Gui.Pos]::Center() + 5
        $stopButton.Y = [Terminal.Gui.Pos]::Center() - 2

        # Exit-Button
        $exitButton = [Terminal.Gui.Button]::new("_Beenden")
        $exitButton.X = [Terminal.Gui.Pos]::Center() - 5
        $exitButton.Y = [Terminal.Gui.Pos]::Center() + 2

        # Event-Handler für Start-Button
        $startButton.add_Clicked({
            try {
                $statusLabel.Text = "Service wird gestartet..."
                [Terminal.Gui.Application]::Refresh()

                Start-ServiceControl -ServiceName $ServiceName

                $statusLabel.Text = "Service gestartet!"
                [Terminal.Gui.MessageBox]::Query("Erfolg",
                        "Service '$ServiceName' wurde erfolgreich gestartet!", "OK")
            }
            catch {
                $statusLabel.Text = "Fehler beim Starten"
                [Terminal.Gui.MessageBox]::ErrorQuery("Fehler",
                        "Fehler beim Starten des Services '$ServiceName':`n$($_.Exception.Message)", "OK")
            }
        })

        # Event-Handler für Stop-Button
        $stopButton.add_Clicked({
            try {
                $statusLabel.Text = "Service wird gestoppt..."
                [Terminal.Gui.Application]::Refresh()

                Stop-ServiceControl -ServiceName $ServiceName

                $statusLabel.Text = "Service gestoppt!"
                [Terminal.Gui.MessageBox]::Query("Erfolg",
                        "Service '$ServiceName' wurde erfolgreich gestoppt!", "OK")
            }
            catch {
                $statusLabel.Text = "Fehler beim Stoppen"
                [Terminal.Gui.MessageBox]::ErrorQuery("Fehler",
                        "Fehler beim Stoppen des Services '$ServiceName':`n$($_.Exception.Message)", "OK")
            }
        })

        # Event-Handler für Exit-Button
        $exitButton.add_Clicked({
            [Terminal.Gui.Application]::RequestStop()
        })

        # Komponenten zum Fenster hinzufügen
        $win.Add($serviceLabel)
        $win.Add($statusLabel)
        $win.Add($startButton)
        $win.Add($stopButton)
        $win.Add($exitButton)

        # Komponenten zur Anwendung hinzufügen
        $top.Add($menu)
        $top.Add($win)

        # Anwendung ausführen
        [Terminal.Gui.Application]::Run()
        [Terminal.Gui.Application]::Shutdown()

        Write-Host "Terminal-UI für Service '$ServiceName' beendet." -ForegroundColor Yellow
    }
    catch {
        Write-Error "Fehler in Terminal-UI: $($_.Exception.Message)"
    }
}

# Hauptfunktion
function Invoke-WorkerServiceControl {
    param(
        [switch]$Start,
        [switch]$Stop,
        [switch]$Tui,
        [string]$ServiceName = "mein-service"
    )

    if ($Start) {
        Start-ServiceControl -ServiceName $ServiceName
    }
    elseif ($Stop) {
        Stop-ServiceControl -ServiceName $ServiceName
    }
    elseif ($Tui) {
        Start-TerminalGui -ServiceName $ServiceName
    }
    else {
        Write-Host "Service Controller PowerShell Script - Hilfe" -ForegroundColor Cyan
        Write-Host "=============================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Verwendung:"
        Write-Host "  Invoke-WorkerServiceControl -Start [-ServiceName <Name>]"
        Write-Host "  Invoke-WorkerServiceControl -Stop [-ServiceName <Name>]"
        Write-Host "  Invoke-WorkerServiceControl -Tui [-ServiceName <Name>]"
        Write-Host ""
        Write-Host "Parameter:"
        Write-Host "  -Start         Startet den angegebenen Service"
        Write-Host "  -Stop          Stoppt den angegebenen Service"
        Write-Host "  -Tui           Startet die Terminal-Benutzeroberfläche"
        Write-Host "  -ServiceName   Name des Services (Standard: 'mein-service')"
        Write-Host ""
        Write-Host "Beispiele:"
        Write-Host "  Invoke-WorkerServiceControl -Start"
        Write-Host "  Invoke-WorkerServiceControl -Stop -ServiceName 'custom-service'"
        Write-Host "  Invoke-WorkerServiceControl -Tui"
    }
}

# Beispielaufruf
Invoke-WorkerServiceControl -Tui