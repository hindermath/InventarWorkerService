using System.Reflection;
using InventarWorkerCommon.Services.Common;
using InventarWorkerCommon.Services.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace InventarViewerApp;

partial class WebApi
{
    /// <summary>
    /// Starts the minimal Web API host that exposes the viewer's database via controllers and Swagger UI.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the host.</param>
    /// <returns>A task representing the lifetime of the web host.</returns>
    public static async Task WebApiAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services konfigurieren
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        // Erweiterte Swagger-Konfiguration
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Inventar API",
                Version = "v1",
                Description = "Eine API zur Verwaltung von Inventardaten",
                Contact = new OpenApiContact
                {
                    Name = "Support Team",
                    Email = "support@beispiel.com"
                }
            });

            // XML-Kommentare für detaillierte Dokumentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Beispiel für Authorization (falls benötigt)
            /*
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            */
        });
        
        // SqliteDbService registrieren
        var dbPath = Initialize.GetDbBasePath(); //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventar.db");
        builder.Services.AddSingleton<SqliteDbService>(provider => 
            new SqliteDbService($"Data Source={dbPath}"));

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

        // Datenbank initialisieren
        using (var scope = app.Services.CreateScope())
        {
            var dbService = scope.ServiceProvider.GetRequiredService<SqliteDbService>();
            dbService.InitializeDatabase();
        }

        // Pipeline konfigurieren
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventar API V1");
                options.RoutePrefix = "swagger"; // Swagger UI unter /swagger verfügbar
                options.DocExpansion(DocExpansion.None); // Alle Endpunkte initial eingeklappt
                options.DisplayRequestDuration(); // Zeigt Antwortzeiten an
                options.EnableDeepLinking(); // Ermöglicht Deep-Links zu spezifischen Endpunkten
                options.EnableFilter(); // Aktiviert Suchfilter
                options.ShowExtensions(); // Zeigt Vendor-Extensions
                options.EnableValidator(); // Aktiviert Validator
            });
        }
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventar API V1");
            options.RoutePrefix = "swagger"; // Swagger UI unter /swagger verfügbar
            options.DocExpansion(DocExpansion.None); // Alle Endpunkte initial eingeklappt
            options.DisplayRequestDuration(); // Zeigt Antwortzeiten an
            options.EnableDeepLinking(); // Ermöglicht Deep-Links zu spezifischen Endpunkten
            options.EnableFilter(); // Aktiviert Suchfilter
            options.ShowExtensions(); // Zeigt Vendor-Extensions
            options.EnableValidator(); // Aktiviert Validator
        });

        app.UseCors("AllowAll");
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        
        Console.WriteLine("Inventar API läuft auf http://localhost:5000");
        Console.WriteLine("Swagger UI verfügbar unter http://localhost:5000/swagger");
        
        await app.RunAsync();
    }
}