using System.Reflection;
using InventarWorkerCommon.Services.Common;
using InventarWorkerCommon.Services.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace InventarViewerApp.API;

class WebApi
{
    private static readonly object _syncRoot = new();
    private static Task? _runningTask;
    private static CancellationTokenSource? _cts;

    /// <summary>
    /// Gets a value indicating whether the Web API is currently running.
    /// This property checks the status of the underlying task managing the API lifetime
    /// to determine if it is still active and has not been completed.
    /// </summary>
    public static bool IsRunning
    {
        get
        {
            lock (_syncRoot)
            {
                return _runningTask is {IsCompleted: false};
            }
        }
    }

    /// <summary>
    /// Starts, stops, or monitors the Web API host based on the specified command-line arguments.
    /// Handles commands such as starting the Web API in a singleton pattern or stopping it gracefully.
    /// </summary>
    /// <param name="args">
    /// An array of command-line arguments to configure the Web API host.
    /// Supported commands include:
    /// - "--start": Starts the Web API if it is not already running.
    /// - "--stop": Stops the running Web API instance.
    /// - "--api": Starts the Web API and waits until it is explicitly stopped.
    /// Other arguments are passed as configuration overrides or runtime parameters.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of managing the Web API. The task completes
    /// when the Web API is either started or stopped based on the provided command.
    /// </returns>
    public static async Task WebApiAsync(string[] args)
    {
        var command = args.FirstOrDefault()?.ToLowerInvariant();

        // STOP-Befehl
        if (command == "--stop")
        {
            Task? taskToWait = null;
            lock (_syncRoot)
            {
                if (_runningTask == null || _runningTask.IsCompleted)
                {
                    return; // nichts zu stoppen
                }

                _cts?.Cancel();
                taskToWait = _runningTask;
            }

            try
            {
                if (taskToWait != null)
                {
                    await taskToWait;
                }
            }
            catch (OperationCanceledException)
            {
                // Erwartet beim Cancel
            }

            return;
        }

        // Standard: START / --api / --start
        lock (_syncRoot)
        {
            // Läuft schon -> nichts Neues starten, bestehendes Task zurückgeben
            if (_runningTask is {IsCompleted: false})
            {
                return; // Caller muss hier nicht zwingend auf das Task warten
            }

            _cts = new CancellationTokenSource();
            var effectiveArgs = args
                .Where(a => a is not "--start" and not "--stop")
                .ToArray();

            _runningTask = RunWebApiInternalAsync(effectiveArgs, _cts.Token);
        }

        // Im CLI-Modus (--api) sinnvoll: auf das Task warten
        if (command == "--api")
        {
            try
            {
                await _runningTask!;
            }
            catch (OperationCanceledException)
            {
                // Normal beim Stop
            }
        }
    }

    /// <summary>
    /// Executes the internal logic for starting and running the Web API host with the specified arguments and cancellation token.
    /// </summary>
    /// <param name="args">
    /// Command-line arguments used to configure the Web API host. These arguments can include custom options
    /// such as configuration overrides or other runtime parameters. Arguments like "--start" and "--stop" are filtered out before use.
    /// </param>
    /// <param name="ctsToken">
    /// A CancellationToken to signal the termination of the Web API host. This token allows the operation to
    /// be cancelled externally, ensuring graceful shutdown of the application.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of running the Web API host. The task completes
    /// when the host is shut down explicitly or via the provided cancellation token.
    /// </returns>
    private static async Task? RunWebApiInternalAsync(string[] args, CancellationToken ctsToken)
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
                // Markdown is supported here. <br/> ensures line break.
                Description = "An API to view inventory data.<br />👉 **[Open complete documentation](/apidoc/index.html)**",
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

            xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (System.IO.File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            // Filter registrieren, um ExternalDocs hinzuzufügen
            options.DocumentFilter<ExternalDocsFilter>();
        });

        // Register SqliteDbService
        var dbPath =
            Path.Combine(Initialize.GetDbBasePath(),
                "inventar.db"); //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventar.db");
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

        // Make documentation from _site folder available under /apidoc
        var docPath = Path.Combine(builder.Environment.ContentRootPath, "_site");
        if (Directory.Exists(docPath))
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(docPath),
                RequestPath = "/apidoc"
            });
        }

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

        await app.RunAsync(ctsToken);
    }

    // Add this class at the end of the file (outside of top-level statements)
    internal class ExternalDocsFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, Swashbuckle.AspNetCore.SwaggerGen.DocumentFilterContext context)
        {
            swaggerDoc.ExternalDocs = new OpenApiExternalDocs
            {
                Description = "Complete Project Documentation",
                Url = new Uri("/apidoc/index.html", UriKind.Relative)
            };
        }
    }
}