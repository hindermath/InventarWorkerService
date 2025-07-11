using Terminal.Gui;
using InventarViewerApp.Services;
using InventarViewerApp.UI;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace InventarViewerApp;

class Program
{
    static async Task Main(string[] args)
    {
        // Prüfen ob Web API Modus gewünscht ist
        if (args.Length > 0 && args[0] == "--api")
        {
            await StartWebApiAsync(args);
            return;
        }

        // Initialisiere Services
        var apiService = new ApiService("http://localhost:5000");
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventar.db");
        var dbService = new DatabaseService($"Data Source={dbPath}");
            
        // Erstelle Datenbankschema wenn nötig
        dbService.InitializeDatabase();
            
        // Starte Terminal.GUI Anwendung
        Application.Init();
            
        var mainWindow = new MainWindow(apiService, dbService);
            
        Application.Top.Add(mainWindow);
            
        Application.Run();
        Application.Shutdown();
    }
    
    static async Task StartWebApiAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services konfigurieren
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // DatabaseService registrieren
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventar.db");
        builder.Services.AddSingleton<DatabaseService>(provider => 
            new DatabaseService($"Data Source={dbPath}"));

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Pipeline konfigurieren
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventar API V1");
                options.RoutePrefix = string.Empty; // Swagger UI unter der Root-URL verfügbar machen
            });
        }

        app.UseCors();
        app.UseRouting();
        app.MapControllers();

        // Datenbank initialisieren
        using (var scope = app.Services.CreateScope())
        {
            var dbService = scope.ServiceProvider.GetRequiredService<DatabaseService>();
            dbService.InitializeDatabase();
        }

        Console.WriteLine("Inventar API läuft auf http://localhost:5000");
        Console.WriteLine("Swagger UI verfügbar unter http://localhost:5000/swagger");
        
        await app.RunAsync();
    }

}