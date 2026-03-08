using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Models.SqlDatabase;
using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using Terminal.Gui;

namespace InventarViewerApp.UI
{
    /// <summary>
    /// DE: Terminal.Gui-Ansicht zur Anzeige des Service-Status und zur Ablage des aktuellen Maschinen-Eintrags.
    /// EN: Terminal.Gui view that displays service status and persists the current machine entry.
    /// </summary>
    public class StatusView : FrameView
    {
        private readonly ApiService _apiService;
        private readonly SqliteDbService _sqliteDbService;
        private readonly PgSqlDbService _pgSqlDbService;
        private readonly JsonSerializerOptions _jsonOptions;
        private Label _statusLabel;
        private Label _contentLabel;
        private Button _refreshButton;

        /// <summary>
        /// DE: Initialisiert eine neue Instanz der <see cref="StatusView"/>-Klasse.
        /// EN: Initializes a new instance of the <see cref="StatusView"/> class.
        /// </summary>
        /// <param name="apiService">
        /// DE: Dienst zum Abrufen des Service-Status aus dem Backend.
        /// EN: Service used to retrieve the service status from the backend.
        /// </param>
        /// <param name="sqliteDbService">
        /// DE: Dienst zur lokalen Persistierung der Maschineninformationen in SQLite.
        /// EN: Service used to persist machine information in local SQLite storage.
        /// </param>
        /// <param name="pgSqlDbService">
        /// DE: Dienst für PostgreSQL-Zugriffe in der Viewer-Anwendung.
        /// EN: Service for PostgreSQL access in the viewer application.
        /// </param>
        public StatusView(ApiService apiService, SqliteDbService sqliteDbService, PgSqlDbService pgSqlDbService) : base("Service Status")
        {
            _apiService = apiService;
            _sqliteDbService = sqliteDbService;
            _pgSqlDbService = pgSqlDbService;

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

        /// <summary>
        /// DE: Lädt den Status neu, aktualisiert die Ansicht und stellt sicher, dass die Maschine in der Datenbank vorhanden ist.
        /// EN: Refreshes status data, updates the view, and ensures the machine exists in the database.
        /// </summary>
        /// <returns>
        /// DE: Asynchroner Vorgang für das Aktualisieren von Status- und Maschinendaten.
        /// EN: Asynchronous operation for refreshing status and machine data.
        /// </returns>
        public async Task RefreshData()
        {
            try
            {
                Application.MainLoop.Invoke(() => _statusLabel.Text = "Status wird abgefragt...");
                
                var status = await _apiService.GetServiceStatusAsync();
                
                // DE: JSON-Antwort auswerten, um den Maschinennamen robust zu extrahieren.
                // EN: Parse JSON response to extract the machine name in a robust way.
                string machineName;
                try
                {
                    // DE: Status in einen String umwandeln und als JsonDocument lesen.
                    // EN: Convert status to string and parse it as JsonDocument.
                    var statusString = status.ToString();
                    var statusDocument = JsonDocument.Parse(statusString);
                    
                    // DE: Für den Zugriff nach Schlüsseln in ein Dictionary deserialisieren.
                    // EN: Deserialize into a dictionary for key-based access.
                    var statusData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(statusDocument.RootElement.GetRawText(), _jsonOptions);

                    machineName = statusData.ContainsKey("machineName") && statusData["machineName"].ValueKind == JsonValueKind.String
                        ? statusData["machineName"].GetString()
                        : Environment.MachineName;
                }
                catch
                {
                    // DE: Fallback auf lokalen Namen, damit die UI trotz Formatfehler weiterarbeitet.
                    // EN: Fall back to local machine name so the UI keeps working even on format errors.
                    machineName = Environment.MachineName;
                }
                
                // DE: Maschinen-Informationen persistieren, um spätere Inventar-Speicherungen zuzuordnen.
                // EN: Persist machine information so later inventory writes can be linked correctly.
                var machine = new Machine
                {
                    Name = machineName,
                    OperatingSystem = Environment.OSVersion.ToString(),
                    LastSeen = DateTime.UtcNow
                };
                
                var machineId = await _sqliteDbService.SaveOrUpdateMachineAsync(machine);
                
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
