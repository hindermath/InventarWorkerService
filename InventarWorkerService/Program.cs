using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using InventarWorkerCommon.Services.Hardware;
using InventarWorkerCommon.Services.Software;
using InventarWorkerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add Windows Service Support
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "InventarWorkerService";
});

// Add Systemd support for Linux/Unix
builder.Services.AddSystemd();

// Register services
builder.Services.AddSingleton<HardwareInventoryService>();
builder.Services.AddSingleton<SoftwareInventoryService>();
builder.Services.AddHostedService<Worker>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Add REST API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationHandler.SchemeName,
        _ => { });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InventarWorkerService API",
        Version = "v1",
        Description = "An API to retrieve and view inventory data",
        Contact = new OpenApiContact
        {
            Name = "InventarWorkerService Support Team",
            Email = "support@tmyttmaap.info",
            Url = new Uri("http://tmyttmaap.info")
        }
    });

    // Include XML comments in Swagger for public code elements
    var xmlFile = "InventarWorkerService.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

builder.Services.Configure<JsonOptions>(options =>
{
    // Configure JSON serialization
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
});

// Configure CORS (optional)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure Development Environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "InventarWorkerService API V1");
        options.RoutePrefix = "swagger"; // Swagger UI available at /swagger
        options.DocExpansion(DocExpansion.None); // All endpoints initially collapsed
        options.DisplayRequestDuration(); // Displays response times
        options.EnableDeepLinking(); // Enables deep links to specific endpoints
        options.EnableFilter(); // Activates search filters
        options.ShowExtensions(); // Shows Vendor Extensions
        options.EnableValidator(); // Activate validator
    });
    //app.MapOpenApi();
    //app.UseMigrationsEndPoint();
}
else
{
    app.UseHttpsRedirection();
    // app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
}

// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseCookiePolicy();

// app.UseRouting();
// app.UseRateLimiter();
// app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();
// app.UseSession();
// app.UseResponseCompression();
// app.UseResponseCaching();
// app.MapRazorPages();
// app.MapDefaultControllerRoute();

app.MapControllers();

app.Run();

/// <summary>
/// DE: Authentifiziert Inventar-API-Aufrufe mit einem konfigurierten API-Schlüssel.
/// EN: Authenticates inventory API requests with a configured API key.
/// </summary>
public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>
    /// DE: Name des Authentifizierungsschemas.
    /// EN: Authentication scheme name.
    /// </summary>
    public const string SchemeName = "InventoryApiKey";

    private const string HeaderName = "X-Inventory-Api-Key";
    private readonly IConfiguration _configuration;

    /// <summary>
    /// DE: Initialisiert den Handler mit sicheren ASP.NET-Core-Diensten.
    /// EN: Initializes the handler with secure ASP.NET Core services.
    /// </summary>
    /// <param name="options">DE: Laufende Schemaoptionen. EN: Monitored scheme options.</param>
    /// <param name="logger">DE: Logger-Factory für interne Diagnosen. EN: Logger factory for internal diagnostics.</param>
    /// <param name="encoder">DE: URL-Encoder des Frameworks. EN: Framework URL encoder.</param>
    /// <param name="configuration">DE: Konfiguration mit dem extern bereitgestellten API-Schlüssel. EN: Configuration containing the externally supplied API key.</param>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var configuredKey = _configuration["InventoryApi:ApiKey"];
        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API key is not configured."));
        }

        if (!Request.Headers.TryGetValue(HeaderName, out var suppliedHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var suppliedKey = suppliedHeader.ToString();
        var expectedBytes = Encoding.UTF8.GetBytes(configuredKey);
        var suppliedBytes = Encoding.UTF8.GetBytes(suppliedKey);
        if (!CryptographicOperations.FixedTimeEquals(expectedBytes, suppliedBytes))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
        }

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "inventory-api-client")], SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
