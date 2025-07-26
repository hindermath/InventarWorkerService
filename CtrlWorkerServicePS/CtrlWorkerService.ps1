<#
	.SYNOPSIS
		EntryPoint
	
	.DESCRIPTION
		Entry point function of the script
	
	.PARAMETER Start
		Switch for starting the WorkerService
	
	.PARAMETER Stop
		Stop the WorkerService
	
	.PARAMETER Tui
		Show Terminal GUI for controlling the WorkerService
	
	.PARAMETER Help
		Show usage of the controlling WorkerService script
	
	.PARAMETER ServiceName
		Override the default service name
	
	.EXAMPLE
		PS C:\> .\CtrlWorkerService.ps1
	
	.NOTES
		Additional information about the file.
#>
[CmdletBinding(DefaultParameterSetName = 'Tui')]
param
(
	[Parameter(ParameterSetName = 'Start',
			   Mandatory = $false,
			   Position = 0,
			   HelpMessage = 'Switch for starting the WorkerService')]
	[switch]$Start = $false,
	[Parameter(ParameterSetName = 'Stop',
			   Mandatory = $false,
			   Position = 1,
			   HelpMessage = 'Stop the WorkerService')]
	[switch]$Stop = $false,
	[Parameter(ParameterSetName = 'Tui',
			   Mandatory = $false,
			   Position = 2,
			   HelpMessage = 'Show Terminal GUI for controlling the WorkerService')]
	[switch]$Tui = $false,
	[Parameter(ParameterSetName = 'Help',
			   Mandatory = $false,
			   Position = 3,
			   HelpMessage = 'Show usage of the controlling WorkerService script')]
	[switch]$Help = $false,
	[Parameter(Mandatory = $false,
			   Position = 4,
			   HelpMessage = 'Override the default service name')]
	[ValidateNotNullOrEmpty()]
	[string]$ServiceName = 'default-service'
)
function Invoke-WorkerServiceControl
{
	try
	{
		if ($Start)
		{
			Start-WorkerService -ServiceName $ServiceName
		}
		elseif ($Stop)
		{
			Stop-WorkerService -ServiceName $ServiceName
		}
		elseif ($Tui)
		{
			Start-TerminalGui -ServiceName $ServiceName
		}
		else
		{
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
	catch
	{
		Write-Error -Message $_.Exception.Message -Category OperationStopped
	}
}

function Start-TerminalGui
{
	param (
		[string]$ServiceName = "mein-service"
	)
	# Lade das Terminal.Gui Assembly
	$terminalGuiPath = "$HOME/.nuget/packages/terminal.gui/1.19.0/lib/net8.0/Terminal.Gui.dll"
	$nStackPath = "$HOME/.nuget/packages/nstack.core/1.1.1/lib/netstandard2.0/NStack.dll"
	$ctrlWorkerCommon = "$HOME\RiderProjects\InventarWorkerService\CtrlWorkerCommon\bin\Debug\net9.0\CtrlWorkerCommon.dll"
	Add-Type -Path $terminalGuiPath
	Add-Type -Path $nStackPath
	Add-Type -Path $ctrlWorkerCommon
	
	try
	{
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
		$serviceLabel = [Terminal.Gui.Label]::new("Service: '$ServiceName'")
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
				try
				{
					$statusLabel.Text = "Service wird gestartet..."
					[Terminal.Gui.Application]::Refresh()
					
					#Start-WorkerService -ServiceName $ServiceName
					[CtrlWorkerCommon.Controller.CrossPlatformServiceController]::CrossPlatformServiceController($ServiceName)
					[CtrlWorkerCommon.Controller.CrossPlatformServiceController]::StartService()
					
					$statusLabel.Text = "Service gestartet!"
					[Terminal.Gui.MessageBox]::Query("Erfolg",
						"Service '$ServiceName' wurde erfolgreich gestartet!", "OK")
				}
				catch
				{
					$statusLabel.Text = "Fehler beim Starten"
					[Terminal.Gui.MessageBox]::ErrorQuery("Fehler",
						"Fehler beim Starten des Services '$ServiceName':`n$($_.Exception.Message)", "OK")
				}
			})
		
		# Event-Handler für Stop-Button
		$stopButton.add_Clicked({
				try
				{
					$statusLabel.Text = "Service wird gestoppt..."
					[Terminal.Gui.Application]::Refresh()
					
					Stop-WorkerService -ServiceName $ServiceName
					
					$statusLabel.Text = "Service gestoppt!"
					[Terminal.Gui.MessageBox]::Query("Erfolg",
						"Service '$ServiceName' wurde erfolgreich gestoppt!", "OK")
				}
				catch
				{
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
	catch
	{
		Write-Error "Fehler in Terminal-UI: $($_.Exception.Message)"
	}
}


function Start-WorkerService {
	param([string]$ServiceName)
	
	if ($VerbosePreference -eq "Continue") {
		Write-Verbose "Starte Service '$ServiceName'..."
	}
	
	try {
		Write-Host "Service '$ServiceName' wird gestartet..." -ForegroundColor Green
		# Hier würde die tatsächliche Service-Logik stehen
		Start-Sleep -Seconds 2
		Write-Host "Service '$ServiceName' gestartet!" -ForegroundColor Green
		
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
		Write-Host "Service '$ServiceName' wird gestoppt..." -ForegroundColor Red
		# Hier würde die tatsächliche Service-Logik stehen
		Start-Sleep -Seconds 2
		Write-Host "Service '$ServiceName' gestoppt!" -ForegroundColor Red
		
		Write-Output "Service '$ServiceName' wird gestoppt..."
		
		if ($VerbosePreference -eq "Continue") {
			Write-Verbose "Service '$ServiceName' erfolgreich gestoppt."
		}
	}
	catch {
		Write-Error -Message "Fehler beim Stoppen des Services '$ServiceName': $($_.Exception.Message)" -Category OperationStopped
	}
}

# Aufruf
Invoke-WorkerServiceControl