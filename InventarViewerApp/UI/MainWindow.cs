using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using Terminal.Gui;

namespace InventarViewerApp.UI
{
    public class MainWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly DatabaseService _dbService;
        private readonly MongoDbService _mongoDbService;
        private TabView _tabView;

        public MainWindow(ApiService apiService, DatabaseService dbService, MongoDbService mongoDbService) : base("Main Window")
        {
            _apiService = apiService;
            _dbService = dbService;
            _mongoDbService = mongoDbService;
            
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Erstelle Tab-View mit unterschiedlichen Ansichten
            _tabView = new TabView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            
            // Füge Tabs hinzu
            _tabView.AddTab(new TabView.Tab("Status", new StatusView(_apiService, _dbService)), true);
            _tabView.AddTab(new TabView.Tab("Hardware", new HardwareView(_apiService, _dbService)), false);
            _tabView.AddTab(new TabView.Tab("Software", new SoftwareView(_apiService, _dbService, _mongoDbService)),false);
            
            Add(_tabView);
            
            // Menüleiste erstellen
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem("_Datei", new MenuItem[] {
                    new MenuItem("_Aktualisieren", "", async () => await RefreshData()),
                    new MenuItem("_Beenden", "", () => Application.RequestStop())
                }),
                new MenuBarItem("_Hilfe", new MenuItem[] {
                    new MenuItem("_Über", "", () => ShowAboutDialog())
                })
            });
            
            Application.Top.Add(menu);
        }

        private async Task RefreshData()
        {
            try
            {
                // Daten aktualisieren
                MessageBox.Query("Datenaktualisierung", "Daten werden aktualisiert...", "OK");
                
                // Daten für jeden Tab aktualisieren
                foreach (var tab in _tabView.Tabs)
                {
                    if (tab.View is HardwareView hardwareView)
                    {
                        await hardwareView.RefreshData();
                    }
                    else if (tab.View is SoftwareView softwareView)
                    {
                        await softwareView.RefreshData();
                    }
                    else if (tab.View is StatusView statusView)
                    {
                        await statusView.RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Fehler", $"Fehler beim Aktualisieren der Daten: {ex.Message}", "OK");
            }
        }

        private void ShowAboutDialog()
        {
            MessageBox.Query("Über", "Inventar Viewer\nVersion 1.0\n© 2025", "OK");
        }
    }
}