using CtrlWorkerServiceApp.Controller;

namespace CtrlWorkerServiceApp;

class Program
{
    static void Main(string[] args)
    {
        var controller = new CrossPlatformServiceController("mein-service");
        
        try
        {
            controller.StartService();
            Console.WriteLine("Service gestartet");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler: {ex.Message}");
        }
    }
}