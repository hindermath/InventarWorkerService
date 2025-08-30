using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Services.Hardware;
using InventarWorkerCommon.Services.Software;
using Microsoft.AspNetCore.Mvc;

namespace HarvesterWorkerService;

public class Program
{
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
        builder.Services.AddSingleton<HardwareInventoryService>();
        builder.Services.AddSingleton<SoftwareInventoryService>();
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