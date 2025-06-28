using ServiceStatusReaderApp.Service.Path;
using ServiceStatusReaderApp.ServiceStatus;

namespace ServiceStatusReaderApp.Service.Monitor;

public class AdvancedServiceMonitor
{
    private readonly ServiceStatusReader _reader;
    
    public AdvancedServiceMonitor()
    {
        _reader = new ServiceStatusReader();
    }
    
    public void WatchForChanges()
    {
        var watcher = new FileSystemWatcher(ServicePaths.GetStatusDirectory())
        {
            Filter = "*.json",
            NotifyFilter = NotifyFilters.LastWrite
        };
        
        watcher.Changed += (sender, e) =>
        {
            Console.WriteLine($"Status-Datei geändert: {e.Name}");
            DisplayCurrentStatus();
        };
        
        watcher.EnableRaisingEvents = true;
        Console.WriteLine("Überwachung gestartet. Drücke eine Taste zum Beenden...");
        Console.ReadKey();
        watcher.Dispose();
    }
    
    private void DisplayCurrentStatus()
    {
        var status = _reader.ReadStatus();
        if (status != null)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Status: {status.State}, Items: {status.ProcessedItems}");
        }
    }
}