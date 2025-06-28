using Terminal.Gui;
using InventarViewerApp.Services;

namespace InventarViewerApp.UI
{
    public class SoftwareView : FrameView
    {
        private readonly ApiService _apiService;
        private readonly DatabaseService _dbService;
        private ListView _listView;
        private Label _statusLabel;
        private Button _refreshButton;
        private Button _saveButton;

        public SoftwareView(ApiService apiService, DatabaseService dbService) : base("Software Inventar")
        {
            _apiService = apiService;
            _dbService = dbService;
            
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
                
                Application.MainLoop.Invoke(() => {
                    var items = new string[softwareData.Count];
                    
                    for (int i = 0; i < softwareData.Count; i++)
                    {
                        var sw = softwareData[i];
                        items[i] = $"{sw.OperatingSystem} - {sw.Updates}";
                    }
                    
                    _listView.SetSource(items);
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
                
                foreach (var sw in softwareData)
                {
                    await _dbService.SaveSoftwareInventoryAsync(sw);
                }
                
                Application.MainLoop.Invoke(() => {
                    _statusLabel.Text = $"In Datenbank gespeichert: {DateTime.Now}";
                    MessageBox.Query("Erfolg", $"{softwareData.Count} Software-Einträge in Datenbank gespeichert", "OK");
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