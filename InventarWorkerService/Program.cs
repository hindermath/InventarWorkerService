using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace InventarWorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddLogging(
            loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    loggingBuilder.AddEventLog();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // Systemd Journal Logging für Linux
                    loggingBuilder.AddSystemdConsole();
                    
                    // Optional: Syslog-Provider hinzufügen (requires Microsoft.Extensions.Logging.Syslog NuGet package)
                    //loggingBuilder.AddSyslog();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // Apple System Log für macOS
                    loggingBuilder.AddSystemdConsole();
                    
                    // Optional: OSLog-Provider für macOS (native Apple logging)
                    // Requires additional configuration or third-party provider
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                {
                    // FreeBSD Syslog-Integration
                    loggingBuilder.AddSystemdConsole();
                    
                    // FreeBSD nutzt traditionelles Syslog-System
                    // Optional: Syslog-Provider für FreeBSD
                    //loggingBuilder.AddSyslog();
                }
                else
                {
                    // Fallback für andere Unix-ähnliche Systeme
                    loggingBuilder.AddSystemdConsole();
                    // Optional: Syslog-Provider für FreeBSD
                    //loggingBuilder.AddSyslog();
                }
                
                // Zusätzliche plattformübergreifende Optionen
                loggingBuilder.SetMinimumLevel(LogLevel.Information);
                
                // Optional: File-Logging für alle Plattformen
                //loggingBuilder.AddFile($"logs/app-{DateTime.Today}.txt"); // requires Serilog.Extensions.Logging.File
            }
        );
        
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}