using InventarWorkerCommon.Services.Status;

namespace ServiceStatusReaderApp;

/// <summary>
/// Entry point for the ServiceStatusReaderApp console application.
/// Reads and displays the status, statistics, and recent logs of the InventarWorkerService.
/// </summary>
static class Program
{
    /// <summary>
    /// Application entry point. Reads and prints current service status and statistics.
    /// Use the --monitor argument to continuously watch and print updates.
    /// </summary>
    /// <param name="args">Command line arguments. Specify --monitor to enable continuous monitoring.</param>
    static async Task Main(string[] args)
    {
        var reader = new ServiceStatusReader();
        
        Console.WriteLine("=== InventarWorkerService Status ===\n");
        
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
        
        // Letzte Logs
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
}