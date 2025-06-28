using Terminal.Gui;
using InventarViewerApp.Services;

namespace InventarViewerApp.UI
{
    public class StatusView : FrameView
    {
        private readonly ApiService _apiService;
        private Label _statusLabel;
        private Label _contentLabel;
        private Button _refreshButton;

        public StatusView(ApiService apiService) : base("Service Status")
        {
            _apiService = apiService;
            
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
                
                Application.MainLoop.Invoke(() => {
                    _contentLabel.Text = $"Service: {status}\n\nAbgefragt: {DateTime.Now}";
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