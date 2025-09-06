using InventarWorkerCommon.Services.Paths;
using ServiceStatusReaderApp.Service.Status;

namespace ServiceStatusReaderApp.Service.Monitor;

public class AdvancedServiceMonitor
{
    private readonly ServiceStatusReader _reader = new();

    /// <summary>
    /// Starts monitoring a designated directory for changes to JSON files.
    /// When a change is detected, the method logs the file name of the modified file and displays the current service status.
    /// The directory being monitored is determined by the path provided by the <see cref="ServicePath.GetServiceStatusPath" /> method.
    /// File changes are tracked based on the last write time. When a change occurs, the service status is read using
    /// the <see cref="ServiceStatusReader" /> and logged to the console.
    /// This method blocks execution until a key is pressed, after which it disposes of the monitoring resources and exits.
    /// </summary>
    public void WatchForChanges()
    {
        var watcher = new FileSystemWatcher(ServicePath.GetServiceStatusPath())
        {
            Filter = "*.json",
            NotifyFilter = NotifyFilters.LastWrite
        };
        
        watcher.Changed += (_, e) =>
        {
            Console.WriteLine($"Status-Datei geändert: {e.Name}");
            DisplayCurrentStatus();
        };
        
        watcher.EnableRaisingEvents = true;
        Console.WriteLine("Überwachung gestartet. Drücke eine Taste zum Beenden...");
        Console.ReadKey();
        watcher.Dispose();
    }

    /// <summary>
    /// Displays the current status of the service by reading the status information
    /// using the <see cref="ServiceStatusReader.ReadStatus"/> method. If a valid
    /// status is retrieved, it logs the state and the number of processed items along
    /// with a timestamp to the console.
    /// </summary>
    private void DisplayCurrentStatus()
    {
        var status = _reader.ReadStatus();
        if (status != null)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Status: {status.State}, Items: {status.ProcessedItems}");
        }
    }
}