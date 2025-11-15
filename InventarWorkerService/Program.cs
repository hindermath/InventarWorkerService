using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Services.Hardware;
using InventarWorkerCommon.Services.Software;
using InventarWorkerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
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
builder.Services.AddSwaggerGen(options =>
{
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
    app.MapOpenApi();
    //app.UseMigrationsEndPoint();
}
else
{
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
app.UseCors("AllowAll");

// app.UseAuthentication();
app.UseAuthorization();
// app.UseSession();
// app.UseResponseCompression();
// app.UseResponseCaching();
// app.MapRazorPages();
// app.MapDefaultControllerRoute();

app.MapControllers();

app.Run();