using System.Management.Automation;
using CtrlWorkerCommon.Controller;
using Terminal.Gui;

namespace CtrlWorkerServiceCmdlet
{
    /// <summary>
    /// Cmdlet to provide control actions for a Worker Service.
    /// Allows starting, stopping, or launching a terminal user interface (TUI) for the service.
    /// </summary>
    /// <remarks>
    /// The cmdlet supports multiple parameter sets for different operations.
    /// By default, it acts on a service with the name 'mein-service', but this can be overridden using the ServiceName parameter.
    /// Errors encountered during the process are captured and written using the WriteError method.
    /// </remarks>
    [Cmdlet(VerbsLifecycle.Invoke, "WorkerServiceControl")]
    [OutputType(typeof(string))]
    public class InvokeWorkerServiceControlCmdlet : PSCmdlet
    {
        /// <summary>
        /// Startet den konfigurierten Worker Service.
        /// </summary>
        /// <remarks>
        /// Verwenden Sie diesen Schalter, um den Dienst zu starten. Kombinieren Sie optional mit <c>-ServiceName</c>,
        /// um einen anderen Dienstnamen anzugeben, und mit <c>-Verbose</c> für detaillierte Ausgaben.
        /// </remarks>
        [Parameter(
            Mandatory = false,
            Position = 0,
            ParameterSetName = "Start",
            HelpMessage = "Startet den Worker Service")]
        public SwitchParameter Start { get; set; }

        /// <summary>
        /// Stoppt den konfigurierten Worker Service.
        /// </summary>
        /// <remarks>
        /// Verwenden Sie diesen Schalter, um den Dienst zu stoppen. Kombinieren Sie optional mit <c>-ServiceName</c>,
        /// um einen anderen Dienstnamen anzugeben, und mit <c>-Verbose</c> für detaillierte Ausgaben.
        /// </remarks>
        [Parameter(
            Mandatory = false,
            Position = 0,
            ParameterSetName = "Stop",
            HelpMessage = "Stoppt den Worker Service")]
        public SwitchParameter Stop { get; set; }

        /// <summary>
        /// Öffnet eine Terminal-Benutzeroberfläche (TUI) zur Steuerung des Dienstes.
        /// </summary>
        /// <remarks>
        /// Die TUI ermöglicht das Starten und Stoppen des Dienstes über eine einfache Benutzeroberfläche.
        /// </remarks>
        [Parameter(
            Mandatory = false,
            Position = 0,
            ParameterSetName = "TUI",
            HelpMessage = "Startet die Terminal-Benutzeroberfläche")]
        public SwitchParameter Tui { get; set; }

        /// <summary>
        /// Name des zu steuernden Dienstes.
        /// </summary>
        /// <value>Standardwert ist <c>"mein-service"</c>.</value>
        /// <remarks>
        /// Wenn kein Name angegeben wird, wird der Standardname verwendet. Der Wert darf nicht leer sein.
        /// </remarks>
        [Parameter(
            Mandatory = false,
            HelpMessage = "Name des Services (Standard: 'mein-service')")]
        [ValidateNotNullOrEmpty]
        public string ServiceName { get; set; } = "mein-service";


        /// <summary>
        /// Verarbeitet den Cmdlet-Aufruf und führt die angeforderte Aktion entsprechend dem Parametersatz aus.
        /// </summary>
        /// <remarks>
        /// Unterstützte Aktionen sind <c>-Start</c>, <c>-Stop</c> und <c>-Tui</c>. Wenn keiner der Schalter gesetzt ist,
        /// wird eine Hilfeausgabe angezeigt. Ausnahmen werden als Fehlerereignisse gemeldet.
        /// </remarks>
        protected override void ProcessRecord()
        {
            try
            {
                if (Start.IsPresent)
                {
                    StartService();
                }
                else if (Stop.IsPresent)
                {
                    StopService();
                }
                else if (Tui.IsPresent)
                {
                    StartTui();
                }
                else
                {
                    ShowHelp();
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "ServiceControlError",
                    ErrorCategory.OperationStopped,
                    null));
            }
        }

        private void StartService()
        {
            // Prüfen ob -Verbose Parameter explizit angegeben wurde
            bool Verbose = MyInvocation.BoundParameters.ContainsKey("Verbose");

            try
            {
                if (Verbose)
                {
                    WriteVerbose($"Starte Service '{ServiceName}'...");
                }

                var controller = new CrossPlatformServiceController(ServiceName);
                controller.StartService();

                WriteObject($"Service '{ServiceName}' wird gestartet...");
                
                if (Verbose)
                {
                    WriteVerbose($"Service '{ServiceName}' erfolgreich gestartet.");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "StartServiceError",
                    ErrorCategory.OperationStopped,
                    ServiceName));
            }
        }

        private void StopService()
        {
            // Prüfen ob -Verbose Parameter explizit angegeben wurde
            bool Verbose = MyInvocation.BoundParameters.ContainsKey("Verbose");

            try
            {
                if (Verbose)
                {
                    WriteVerbose($"Stoppe Service '{ServiceName}'...");
                }

                var controller = new CrossPlatformServiceController(ServiceName);
                controller.StopService();

                WriteObject($"Service '{ServiceName}' wird gestoppt...");
                
                if (Verbose)
                {
                    WriteVerbose($"Service '{ServiceName}' erfolgreich gestoppt.");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "StopServiceError",
                    ErrorCategory.OperationStopped,
                    ServiceName));
            }
        }

        private void StartTui()
        {
            // Prüfen ob -Verbose Parameter explizit angegeben wurde
            bool Verbose = MyInvocation.BoundParameters.ContainsKey("Verbose");

            try
            {
                if (Verbose)
                {
                    WriteVerbose("Starte Terminal-Benutzeroberfläche...");
                }

                // Terminal.Gui initialisieren
                Application.Init();

                var top = Application.Top;
                
                // Hauptfenster erstellen
                var win = new Window("Service Controller (PowerShell Cmdlet)")
                {
                    X = 0,
                    Y = 1,
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };

                // Menüleiste erstellen
                var menu = new MenuBar(new MenuBarItem[]
                {
                    new MenuBarItem("_Datei", new MenuItem[]
                    {
                        new MenuItem("_Beenden", "", () =>
                        {
                            Application.RequestStop();
                        })
                    }),
                    new MenuBarItem("_Hilfe", new MenuItem[]
                    {
                        new MenuItem("_Info", "", () =>
                        {
                            MessageBox.Query("Info", 
                                $"Service Controller PowerShell Cmdlet v1.0\n" +
                                $"Service: {ServiceName}\n" +
                                $"Zum Starten und Stoppen von Services", "OK");
                        })
                    })
                });

                // Service-Name Label
                var serviceLabel = new Label($"Service: {ServiceName}")
                {
                    X = Pos.Center() - 10,
                    Y = Pos.Center() - 6,
                    ColorScheme = Colors.Menu
                };

                // Buttons für Service-Steuerung
                var startButton = new Button("Service _Starten")
                {
                    X = Pos.Center() - 15,
                    Y = Pos.Center() - 2
                };

                var stopButton = new Button("Service St_oppen")
                {
                    X = Pos.Center() + 5,
                    Y = Pos.Center() - 2
                };

                var exitButton = new Button("_Beenden")
                {
                    X = Pos.Center() - 5,
                    Y = Pos.Center() + 2
                };

                // Status-Label
                var statusLabel = new Label("Bereit...")
                {
                    X = Pos.Center() - 10,
                    Y = Pos.Center() - 4,
                    ColorScheme = Colors.Base
                };

                // Event-Handler für Start-Button
                startButton.Clicked += () =>
                {
                    try
                    {
                        statusLabel.Text = "Service wird gestartet...";
                        Application.Refresh();
                        
                        var controller = new CrossPlatformServiceController(ServiceName);
                        controller.StartService();
                        
                        statusLabel.Text = "Service gestartet!";
                        MessageBox.Query("Erfolg", $"Service '{ServiceName}' wurde erfolgreich gestartet!", "OK");
                    }
                    catch (Exception ex)
                    {
                        statusLabel.Text = "Fehler beim Starten";
                        MessageBox.ErrorQuery("Fehler", $"Fehler beim Starten des Services '{ServiceName}':\n{ex.Message}", "OK");
                    }
                };

                // Event-Handler für Stop-Button
                stopButton.Clicked += () =>
                {
                    try
                    {
                        statusLabel.Text = "Service wird gestoppt...";
                        Application.Refresh();
                        
                        var controller = new CrossPlatformServiceController(ServiceName);
                        controller.StopService();
                        
                        statusLabel.Text = "Service gestoppt!";
                        MessageBox.Query("Erfolg", $"Service '{ServiceName}' wurde erfolgreich gestoppt!", "OK");
                    }
                    catch (Exception ex)
                    {
                        statusLabel.Text = "Fehler beim Stoppen";
                        MessageBox.ErrorQuery("Fehler", $"Fehler beim Stoppen des Services '{ServiceName}':\n{ex.Message}", "OK");
                    }
                };

                // Event-Handler für Exit-Button
                exitButton.Clicked += () =>
                {
                    Application.RequestStop();
                };

                // Komponenten zum Fenster hinzufügen
                win.Add(serviceLabel, statusLabel, startButton, stopButton, exitButton);
                
                // Komponenten zur Anwendung hinzufügen
                top.Add(menu, win);

                // Anwendung ausführen
                Application.Run();
                Application.Shutdown();

                WriteObject($"Terminal-UI für Service '{ServiceName}' beendet.");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "TuiError",
                    ErrorCategory.OperationStopped,
                    null));
            }
        }

        private void ShowHelp()
        {
            WriteObject("Service Controller PowerShell Cmdlet - Hilfe");
            WriteObject("===============================================");
            WriteObject("");
            WriteObject("Verwendung:");
            WriteObject("  Invoke-WorkerServiceControl -Start [-ServiceName <Name>] [-Verbose]");
            WriteObject("  Invoke-WorkerServiceControl -Stop [-ServiceName <Name>] [-Verbose]"); 
            WriteObject("  Invoke-WorkerServiceControl -Tui [-ServiceName <Name>] [-Verbose]");
            WriteObject("");
            WriteObject("Parameter:");
            WriteObject("  -Start         Startet den angegebenen Service");
            WriteObject("  -Stop          Stoppt den angegebenen Service");
            WriteObject("  -Tui           Startet die Terminal-Benutzeroberfläche");
            WriteObject("  -ServiceName   Name des Services (Standard: 'mein-service')");
            WriteObject("  -Verbose       Detaillierte Ausgabe aktivieren");
            WriteObject("");
            WriteObject("Beispiele:");
            WriteObject("  Invoke-WorkerServiceControl -Start");
            WriteObject("  Invoke-WorkerServiceControl -Stop -ServiceName 'custom-service'");
            WriteObject("  Invoke-WorkerServiceControl -Tui -Verbose");
        }
    }
}