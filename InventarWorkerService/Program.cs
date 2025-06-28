using InventarWorkerService;
using InventarWorkerService.Services.Hardware;
using InventarWorkerService.Services.Software;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Services registrieren
builder.Services.AddSingleton<HardwareInventoryService>();
builder.Services.AddSingleton<SoftwareInventoryService>();
builder.Services.AddHostedService<Worker>();

// REST API Services hinzufügen
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS konfigurieren (optional)
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

// Development Environment konfigurieren
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();