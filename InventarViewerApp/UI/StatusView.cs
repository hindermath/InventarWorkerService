using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Models.Database;
using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using Terminal.Gui;

namespace InventarViewerApp.UI
{
    public class StatusView : FrameView
    {
        private readonly ApiService _apiService;
        private readonly DatabaseService _databaseService;
        private readonly JsonSerializerOptions _jsonOptions;
        private Label _statusLabel;
        private Label _contentLabel;
        private Button _refreshButton;

        public StatusView(ApiService apiService, DatabaseService databaseService) : base("Service Status")
        {
            _apiService = apiService;
            _databaseService = databaseService;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Status-Label
            _statusLabel = new Label("Bereit")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill()
            };
            
            // Content-Label für Statusinformationen
            _contentLabel = new Label("")
            {
                X = 0,
                Y = 2,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 4
            };
            
            // Refresh-Button
            _refreshButton = new Button("Aktualisieren")
            {
                X = 0,
                Y = Pos.Bottom(_contentLabel) + 1,
                Width = 15
            };
            
            _refreshButton.Clicked += async () => await RefreshData();
            
            Add(_statusLabel, _contentLabel, _refreshButton);
            
            // Initial Daten laden
            Task.Run(async () => await RefreshData());
        }

        public async Task RefreshData()
        {
            try
            {
                Application.MainLoop.Invoke(() => _statusLabel.Text = "Status wird abgefragt...");
                
                var status = await _apiService.GetServiceStatusAsync();
                
                // JSON-String deserialisieren, um Maschinennamen zu extrahieren
                string machineName;
                try
                {
                    // status zu string konvertieren und als JsonDocument deserialisieren
                    var statusString = status.ToString();
                    var statusDocument = JsonDocument.Parse(statusString);
                    
                    // JsonDocument in Dictionary deserialisieren für einfacheren Zugriff
                    var statusData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(statusDocument.RootElement.GetRawText(), _jsonOptions);

                    machineName = statusData.ContainsKey("machineName") && statusData["machineName"].ValueKind == JsonValueKind.String
                        ? statusData["machineName"].GetString()
                        : Environment.MachineName;
                }
                catch
                {
                    // Fallback auf lokalen Maschinennamen falls JSON-Deserialisierung fehlschlägt
                    machineName = Environment.MachineName;
                }
                
                // Maschinen-Information in die Datenbank speichern
                var machine = new Machine
                {
                    Name = machineName,
                    OperatingSystem = Environment.OSVersion.ToString(),
                    LastSeen = DateTime.UtcNow
                };
                
                var machineId = await _databaseService.SaveOrUpdateMachineAsync(machine);
                
                Application.MainLoop.Invoke(() => {
                    _contentLabel.Text = $"Service: {status}\nMaschine: {machineName} (ID: {machineId})\n\nAbgefragt: {DateTime.Now}";
                    _statusLabel.Text = $"Status abgefragt: {DateTime.Now}";
                });
            }
            catch (Exception ex)
            {
                Application.MainLoop.Invoke(() => {
                    _statusLabel.Text = $"Fehler: {ex.Message}";
                    _contentLabel.Text = $"Fehler beim Abrufen des Status:\n\n{ex.Message}";
                    MessageBox.ErrorQuery("Fehler", $"Fehler beim Abrufen des Service-Status: {ex.Message}", "OK");
                });
            }
        }
    }
}