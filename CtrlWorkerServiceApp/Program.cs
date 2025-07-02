using CtrlWorkerServiceApp.Controller;
using Terminal.Gui;

namespace CtrlWorkerServiceApp;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Verwendung:");
            Console.WriteLine("  CtrlWorkerServiceApp.exe start   - Service starten");
            Console.WriteLine("  CtrlWorkerServiceApp.exe stop    - Service stoppen");
            Console.WriteLine("  CtrlWorkerServiceApp.exe --tui   - Terminal UI starten");
            Console.WriteLine("  CtrlWorkerServiceApp.exe --help  - Diese Hilfe anzeigen");
            return;
        }

        switch (args[0].ToLower())
        {
            case "--tui":
                StartTui();
                break;
                
            default:
                ExecuteCommand(args);
                break;
        }
    }

    static void StartTui()
    {
        Application.Init();

        var top = Application.Top;
        
        // Hauptfenster
        var win = new Window("Service Controller")
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        // Menüleiste
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
                    MessageBox.Query("Info", "Service Controller v1.0\nZum Starten und Stoppen von Services", "OK");
                })
            })
        });

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
                
                var controller = new CrossPlatformServiceController("InventarWorkerService");
                controller.StartService();
                
                statusLabel.Text = "Service gestartet!";
                MessageBox.Query("Erfolg", "Service wurde erfolgreich gestartet!", "OK");
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Fehler beim Starten";
                MessageBox.ErrorQuery("Fehler", $"Fehler beim Starten des Services:\n{ex.Message}", "OK");
            }
        };

        // Event-Handler für Stop-Button
        stopButton.Clicked += () =>
        {
            try
            {
                statusLabel.Text = "Service wird gestoppt...";
                Application.Refresh();
                
                var controller = new CrossPlatformServiceController("InventarWorkerService");
                controller.StopService();
                
                statusLabel.Text = "Service gestoppt!";
                MessageBox.Query("Erfolg", "Service wurde erfolgreich gestoppt!", "OK");
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Fehler beim Stoppen";
                MessageBox.ErrorQuery("Fehler", $"Fehler beim Stoppen des Services:\n{ex.Message}", "OK");
            }
        };

        // Event-Handler für Exit-Button
        exitButton.Clicked += () =>
        {
            Application.RequestStop();
        };

        // Komponenten zum Fenster hinzufügen
        win.Add(statusLabel, startButton, stopButton, exitButton);
        
        // Komponenten zur Anwendung hinzufügen
        top.Add(menu, win);

        Application.Run();
        Application.Shutdown();
    }

    static void ExecuteCommand(string[] args)
    {
        var controller = new CrossPlatformServiceController("InventarWorkerService");
        
        try
        {
            switch (args[0].ToLower())
            {
                case "start":
                    controller.StartService();
                    Console.WriteLine("Service wird gestartet...");
                    break;
                
                case "stop":
                    controller.StopService();
                    Console.WriteLine("Service wird gestoppt...");
                    break;
                
                case "--help":
                case "-h":
                case "help":
                    Console.WriteLine("Service Controller - Hilfe");
                    Console.WriteLine("==========================");
                    Console.WriteLine("Verfügbare Befehle:");
                    Console.WriteLine("  start     - Startet den Service 'mein-service'");
                    Console.WriteLine("  stop      - Stoppt den Service 'mein-service'");
                    Console.WriteLine("  --tui     - Startet die Terminal-Benutzeroberfläche");
                    Console.WriteLine("  --help    - Zeigt diese Hilfe an");
                    break;
                
                default:
                    Console.WriteLine($"Unbekannter Befehl: {args[0]}");
                    Console.WriteLine("Verwenden Sie '--help' für eine Liste der verfügbaren Befehle.");
                    Environment.Exit(1);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Ausführen des Befehls '{args[0]}': {ex.Message}");
            Environment.Exit(1);
        }
    }
}