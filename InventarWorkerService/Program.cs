using InventarWorkerService;
using InventarWorkerService.Services.Hardware;
using InventarWorkerService.Services.Software;

var builder = Host.CreateApplicationBuilder(args);

// Services registrieren
builder.Services.AddSingleton<HardwareInventoryService>();
builder.Services.AddSingleton<SoftwareInventoryService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();