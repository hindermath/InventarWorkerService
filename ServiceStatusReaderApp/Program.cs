using InventarWorkerCommon.Services.Status;

namespace ServiceStatusReaderApp;

/// <summary>
/// Entry point for the ServiceStatusReaderApp console application.
/// Reads and displays the status, statistics, and recent logs of the InventarWorkerService.
/// </summary>
static class Program
{
    /// <summary>
    /// The entry point for the ServiceStatusReaderApp console application. This method initializes the program,
    /// reads the status, statistics, and logs of the InventarWorkerService, and provides an optional monitoring feature.
    /// </summary>
    /// <param name="args">Array of command-line arguments passed to the application.
    /// Includes flags like --help, --harvester, and --monitor which control the program's behavior.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    static async Task Main(string[] args)
    {
        if (args.Contains("--help"))
        {
            Usage();
        }

        ServiceStatusReader reader;

        if (args.Contains("--harvester"))
        {
            reader = new ServiceStatusReader("harvester-service");
            Console.WriteLine("=== HarvesterWorkerService Status ===\n");
        }
        else
        {
            reader = new ServiceStatusReader();
            Console.WriteLine("=== InventarWorkerService Status ===\n");
        }

        // Check Service Status
        var isRunning = reader.IsServiceRunning();
        Console.WriteLine($"Service läuft: {(isRunning ? "JA" : "NEIN")}");
        
        // Detailed Status
        var status = reader.ReadStatus();
        if (status != null)
        {
            Console.WriteLine($"Zustand: {status.State}");
            Console.WriteLine($"Gestartet: {status.StartTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Letzte Aktivität: {status.LastActivity:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Verarbeitete Items: {status.ProcessedItems}");
            if (!string.IsNullOrEmpty(status.LastError))
            {
                Console.WriteLine($"Letzter Fehler: {status.LastError}");
            }
        }
        
        Console.WriteLine();
        
        // Statistics
        var stats = reader.ReadStatistics();
        if (stats != null)
        {
            Console.WriteLine("=== Statistiken ===");
            Console.WriteLine($"Gesamt verarbeitete Items: {stats.TotalProcessedItems}");
            Console.WriteLine($"Durchschnittliche Verarbeitungszeit: {stats.AverageProcessingTime:F2} ms");
            Console.WriteLine($"Laufzeit: {stats.Uptime:dd\\.hh\\:mm\\:ss}");
            Console.WriteLine($"Speicherverbrauch: {stats.MemoryUsage / 1024 / 1024:F2} MB");
        }
        
        Console.WriteLine();
        
        // Last Logs
        var logs = reader.ReadRecentLogs(10);
        if (logs.Any())
        {
            Console.WriteLine("=== Letzte 10 Log-Einträge ===");
            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }
        
        // Continuous monitoring
        if (args.Contains("--monitor"))
        {
            Console.WriteLine("\n=== Kontinuierliche Überwachung (Strg+C zum Beenden) ===");
            while (true)
            {
                await Task.Delay(5000);
                
                var currentStatus = reader.ReadStatus();
                if (currentStatus != null)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {currentStatus.State} - Items: {currentStatus.ProcessedItems}");
                }
            }
        }
    }

    /// <summary>
    /// Displays usage instructions for the ServiceStatusReaderApp executable. This method outlines supported
    /// command-line arguments and their intended functionality, such as toggling worker modes or enabling
    /// monitoring features.
    /// </summary>
    /// <exception cref="NotImplementedException">Thrown to indicate that this functionality has not been implemented yet.</exception>
    private static void Usage()
    {
        throw new NotImplementedException();
    }
}