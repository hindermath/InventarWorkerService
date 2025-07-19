using InventarViewerApp.Services;
using Terminal.Gui;

namespace InventarViewerApp.UI
{
    public class SoftwareView : FrameView
    {
        private readonly ApiService _apiService;
        private readonly DatabaseService _dbService;
        private readonly MongoDbService _mongoDbService;
        private ListView _listView;
        private Label _statusLabel;
        private Button _refreshButton;
        private Button _saveButton;

        public SoftwareView(ApiService apiService, DatabaseService dbService, MongoDbService mongoDbService) : base("Software Inventar")
        {
            _apiService = apiService;
            _dbService = dbService;
            _mongoDbService = mongoDbService;
            
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
            
            // ListView für Software-Daten
            _listView = new ListView()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 3
            };
            
            // Buttons
            _refreshButton = new Button("Aktualisieren")
            {
                X = 0,
                Y = Pos.Bottom(_listView),
                Width = 15
            };
            
            _refreshButton.Clicked += async () => await RefreshData();
            
            _saveButton = new Button("In DB speichern")
            {
                X = Pos.Right(_refreshButton) + 2,
                Y = Pos.Bottom(_listView),
                Width = 20
            };
            
            _saveButton.Clicked += async () => await SaveToDatabase();
            
            Add(_statusLabel, _listView, _refreshButton, _saveButton);
            
            // Initial Daten laden
            Task.Run(async () => await RefreshData());
        }

        public async Task RefreshData()
        {
            try
            {
                Application.MainLoop.Invoke(() => _statusLabel.Text = "Daten werden geladen...");
                
                var softwareData = await _apiService.GetSoftwareInventoryAsync();
                
                Application.MainLoop.Invoke(() =>
                {
                    List<string> swItems = new List<string>();
                    foreach (var item in softwareData.InstalledSoftware)
                    {
                        swItems.Add($"Name: {item.Name}, Version {item.Version}, Hersteller: {item.Publisher}");
                    }
                    _listView.SetSource(swItems);
                    _statusLabel.Text = $"Software Daten geladen: {DateTime.Now}";
                });
            }
            catch (Exception ex)
            {
                Application.MainLoop.Invoke(() => {
                    _statusLabel.Text = $"Fehler: {ex.Message}";
                    MessageBox.ErrorQuery("Fehler", $"Fehler beim Laden der Software-Daten: {ex.Message}", "OK");
                });
            }
        }

        private async Task SaveToDatabase()
        {
            try
            {
                Application.MainLoop.Invoke(() => _statusLabel.Text = "Speichere in Datenbank...");
                
                var softwareData = await _apiService.GetSoftwareInventoryAsync();
                var hardwareData = await _apiService.GetHardwareInventoryAsync();

                var machine = await _dbService.GetMachineByNameAsync(hardwareData.System.MachineName);
                await _dbService.SaveSoftwareInventoryAsync(machine.Id, softwareData);
                await _mongoDbService.SaveSoftwareInventoryAsync(machine.Id, softwareData);

                Application.MainLoop.Invoke(() => {
                    _statusLabel.Text = $"In Datenbank gespeichert: {DateTime.Now}";
                    MessageBox.Query("Erfolg", $"Software in Datenbank gespeichert", "OK");
                });
            }
            catch (Exception ex)
            {
                Application.MainLoop.Invoke(() => {
                    _statusLabel.Text = $"Fehler: {ex.Message}";
                    MessageBox.ErrorQuery("Fehler", $"Fehler beim Speichern in die Datenbank: {ex.Message}", "OK");
                });
            }
        }
    }
}