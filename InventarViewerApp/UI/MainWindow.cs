using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using InventarViewerApp.API;
using Terminal.Gui;

namespace InventarViewerApp.UI
{
    /// <summary>
    /// The main application window that hosts tabs for status, hardware and software views.
    /// </summary>
    public class MainWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly SqliteDbService _dbService;
        private readonly MongoDbService _mongoDbService;
        private TabView _tabView;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="apiService">API service used by child views.</param>
        /// <param name="dbService">SQLite DB service used by child views.</param>
        /// <param name="mongoDbService">MongoDB service used by child views.</param>
        public MainWindow(ApiService apiService, SqliteDbService dbService, MongoDbService mongoDbService) : base("Main Window")
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
                    null,
                    new MenuItem("_Import Maschinen aus CSV-Datei...", "", async () =>
                    {
                        try
                        {
                            // Datei-Öffnen-Dialog für CSV-Dateien
                            var openDialog = new OpenDialog("CSV-Datei auswählen", "Bitte eine CSV-Datei mit Maschinen auswählen")
                            {
                                AllowsMultipleSelection = false,
                                AllowedFileTypes = new[] { ".csv" },
                            };


                            Application.Run(openDialog);

                            if (openDialog.Canceled)
                            {
                                Application.MainLoop.Invoke(() =>
                                {
                                    MessageBox.ErrorQuery("Fehler", "CSV-Dateiauswahl wurde abgebrochen.", "OK");
                                });
                                return;
                            }

                            var filePath = openDialog.FilePath?.ToString();

                            if (string.IsNullOrWhiteSpace(filePath))
                            {
                                Application.MainLoop.Invoke(() =>
                                {
                                    MessageBox.ErrorQuery("Fehler", "Keine Datei ausgewählt.", "OK");
                                });
                                return;
                            }

                            var machinesCountFromCsv = await _dbService.InitializeMachinesFromCsvAsync(filePath);

                            Application.MainLoop.Invoke(() => {
                                MessageBox.Query("Import", $"Import erfolgreich: {machinesCountFromCsv} Maschinen wurden aus\n\"{filePath}\" importiert.", "OK");
                            });
                        }
                        catch (Exception exception)
                        {
                            Application.MainLoop.Invoke(() => {
                                MessageBox.ErrorQuery("Fehler", $"Fehler beim Importieren der Maschinen aus CSV-Datei: {exception.Message}", "OK");
                            });
                        }
                    }),
                    null,
                    new MenuItem("_Beenden", "", () => Application.RequestStop())
                }),
                new MenuBarItem("_Optionen", new MenuItem[] {
                    new MenuItem("_WebApi", "", async () =>
                    {
                        try
                        {
                            if (!WebApi.IsRunning)
                            {
                                // Start as Singleton
                                await WebApi.WebApiAsync(new[] { "--start" });
                                Application.MainLoop.Invoke(() =>
                                    MessageBox.Query("WebApi", "WebApi wurde gestartet.", "OK"));
                            }
                            else
                            {
                                // Stopping the current Singleton
                                await WebApi.WebApiAsync(new[] { "--stop" });
                                Application.MainLoop.Invoke(() =>
                                    MessageBox.Query("WebApi", "WebApi wurde gestoppt.", "OK"));
                            }
                        }
                        catch (Exception e)
                        {
                            Application.MainLoop.Invoke(() =>
                                MessageBox.ErrorQuery("Fehler", $"Fehler beim Umschalten der WebApi: {e.Message}", "OK"));
                        }
                    })
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