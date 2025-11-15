using System.Reflection;
using InventarWorkerCommon.Services.Common;
using InventarWorkerCommon.Services.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace InventarViewerApp.API;

class WebApi
{
    /// <summary>
    /// Starts the minimal Web API host that exposes the viewer's database via controllers and Swagger UI.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the host.</param>
    /// <returns>A task representing the lifetime of the web host.</returns>
    public static async Task WebApiAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Read URLs from configuration
        var urls = builder.Configuration["Url"];
        if (!string.IsNullOrEmpty(urls))
        {
            builder.WebHost.UseUrls(urls);
        }

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Configure Services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        // Extended Swagger Configuration
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Inventory Viewing API",
                Version = "v1",
                Description = "An API to view inventory data",
                Contact = new OpenApiContact
                {
                    Name = "InventarWorkerService Support Team",
                    Email = "support@tmyttmaap.info",
                    Url = new Uri("http://tmyttmaap.info")
                }
            });

            // XML comments for detailed documentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Example of Authorization (if needed)
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
        
        // Register SqliteDbService
        var dbPath = Path.Combine(Initialize.GetDbBasePath(), "inventar.db"); //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventar.db");
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

        // Initialize database
        using (var scope = app.Services.CreateScope())
        {
            var dbService = scope.ServiceProvider.GetRequiredService<SqliteDbService>();
            dbService.InitializeDatabase();
        }

        // Configure Pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "InventarViewerApp API V1");
                options.RoutePrefix = "swagger"; // Swagger UI available at /swagger
                options.DocExpansion(DocExpansion.None); // All endpoints initially collapsed
                options.DisplayRequestDuration(); // Displays response times
                options.EnableDeepLinking(); // Enables deep links to specific endpoints
                options.EnableFilter(); // Activates search filters
                options.ShowExtensions(); // Shows Vendor Extensions
                options.EnableValidator(); // Activate validator
            });
            app.MapOpenApi();
        }
        else
        {
            // app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            // app.UseHsts();
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "InventarViewerApp API V1");
            options.RoutePrefix = "swagger"; // Swagger UI available at /swagger
            options.DocExpansion(DocExpansion.None); // All endpoints initially collapsed
            options.DisplayRequestDuration(); // Displays response times
            options.EnableDeepLinking(); // Enables deep links to specific endpoints
            options.EnableFilter(); // Activates search filters
            options.ShowExtensions(); // Shows Vendor Extensions
            options.EnableValidator(); // Activate validator
        });

        // app.UseHttpsRedirection();
        // app.UseStaticFiles();
        // app.UseCookiePolicy();

        // app.UseRouting();
        // app.UseRateLimiter();
        // app.UseRequestLocalization();
        app.UseCors("AllowAll");

        // app.UseAuthentication();
        app.UseAuthorization();
        // app.UseSession();
        // app.UseResponseCompression();
        // app.UseResponseCaching();
        // app.MapRazorPages();
        // app.MapDefaultControllerRoute();

        app.MapControllers();
        
        Console.WriteLine("Inventory API runs on http://localhost:80");
        Console.WriteLine("Swagger UI available at http://localhost:80/swagger");
        
        await app.RunAsync();
    }
}