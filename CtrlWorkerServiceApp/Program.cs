using CtrlWorkerServiceApp.Controller;

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
            Console.WriteLine("  CtrlWorkerServiceApp.exe --help  - Diese Hilfe anzeigen");
            return;
        }

        var controller = new CrossPlatformServiceController("mein-service");
        
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