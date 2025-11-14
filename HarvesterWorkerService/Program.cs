using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace HarvesterWorkerService;

/// <summary>
/// Entry point for the HarvesterWorkerService application.
/// Configures hosting, registers inventory services, and starts the background worker.
/// </summary>
public static class Program
{
    /// <summary>
    /// Application entry method. Builds and runs the host for the worker service.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        // Add Windows Service Support
        builder.Services.AddWindowsService(options =>
        {
            options.ServiceName = "HarvesterWorkerService";
        });
        // Add Systemd support for Linux/Unix
        builder.Services.AddSystemd();
        
        // Register services
        builder.Services.AddHostedService<Worker>();
        
        // Configure JSON options if needed
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        });

        var host = builder.Build();
        host.Run();
    }
}