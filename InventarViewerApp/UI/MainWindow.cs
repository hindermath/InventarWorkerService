using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InventarViewerApp.API;
using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
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
        private MenuItem _webApiMenuItem;

        // Für die Historie
        private readonly List<string> _actionHistory = new();
        private StatusItem _historyStatusItem;
        private StatusBar _statusBar;


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

            _webApiMenuItem = new MenuItem("_WebApi", "Gestoppt", async () => await ToggleWebApiAction(), shortcut:Key.CtrlMask| Key.ShiftMask | Key.W);
            
            // Create Menu Bar
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem("_Datei", new MenuItem[] {
                    new MenuItem("_Aktualisieren", "", async () => await RefreshData(), shortcut:Key.F5),
                    null,
                    new MenuItem("Impor_t Maschinen aus CSV-Datei...", "", async () => await ImportCsvAction(), shortcut:Key.F3),
                    null,
                    new MenuItem("_Beenden", "", () => Application.RequestStop(), shortcut:Key.CtrlMask| Key.ShiftMask | Key.Q)
                }),
                new MenuBarItem("_Optionen", new MenuItem[] {
                    _webApiMenuItem,
                    null,
                    new MenuItem("Einstellun_gen...", "", () => ShowSettingsDialog(), shortcut:Key.F8)
                }),
                new MenuBarItem("_Hilfe", new MenuItem[] {
                    new MenuItem("Ü_ber", "", () => ShowAboutDialog(), shortcut:Key.F1)
                })
            });
            
            Application.Top.Add(menu);

            // --- STATUS BAR INITIALISIERUNG ---
            // Plattformspezifische Shortcut-Anzeige
            var quitShortcutText = GetPlatformSpecificShortcutText("B", "~B~eenden");
            var importShortcutText = GetPlatformSpecificShortcutText("T", "CSV Impor~t~");
            var webApiShortcutText = GetPlatformSpecificShortcutText("W", "~W~ebApi");


            // Item für die Historie (zeigt die letzte Nachricht)
            _historyStatusItem = new StatusItem(Key.F12, "~F12~ Historie (Verlauf) : Bereit", () => ShowHistoryDialog());

            _statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.CtrlMask| Key.B, quitShortcutText, () => Application.RequestStop()),
                new StatusItem(Key.CtrlMask| Key.T, importShortcutText, async () => await ImportCsvAction()),
                new StatusItem(Key.CtrlMask| Key.W, webApiShortcutText, async () => await ToggleWebApiAction()),
                // Trenner und Historie
                new StatusItem(Key.Null, "|", null),
                _historyStatusItem
            });

            Application.Top.Add(_statusBar);

        }

        // --- NEUE HILFSMETHODEN ---
        private string GetPlatformSpecificShortcutText(string key, string description)
        {
            var isMacOs = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.OSX);

            var modifier = isMacOs ? "^" : "Ctrl"; // ⌘
            return $"~{modifier}+{key}~ {description}";
        }

        private void AddToHistory(string message)
        {
            var timestampedMessage = $"{DateTime.Now:HH:mm:ss}: {message}";
            _actionHistory.Add(timestampedMessage);

            // Aktualisiere den Text in der StatusBar
            if (_historyStatusItem != null)
            {
                _historyStatusItem.Title = $"Log: {message}";
                _statusBar.SetNeedsDisplay();
            }
        }

        private void ShowHistoryDialog()
        {
            var historyText = _actionHistory.Count > 0
                ? string.Join("\n", _actionHistory)
                : "Keine Einträge vorhanden.";

            MessageBox.Query("Historie (Verlauf)", historyText, "OK");
        }

        private async Task ToggleWebApiAction()
        {
            try
            {
                if (!WebApi.IsRunning)
                {
                    // Start as Singleton
                    await WebApi.WebApiAsync(new[] {"--start"});
                    AddToHistory("WebApi wurde gestartet.");

                    // Optional: Trotzdem MessageBox wenn gewünscht, oder nur Statusleiste nutzen
                    // Application.MainLoop.Invoke(() => MessageBox.Query("WebApi", "WebApi wurde gestartet.", "OK"));
                }
                else
                {
                    // Stopping the current Singleton
                    await WebApi.WebApiAsync(new[] {"--stop"});
                    AddToHistory("WebApi wurde gestoppt.");
                }
                UpdateWebApiMenuItemText();
            }
            catch (Exception e)
            {
                var err = $"Fehler WebApi: {e.Message}";
                AddToHistory(err);
                Application.MainLoop.Invoke(() =>
                    MessageBox.ErrorQuery("Fehler", err, "OK"));
            }
        }

        private async Task ImportCsvAction()
        {
            try
            {
                // File Open Dialog for CSV files
                var openDialog = new OpenDialog("CSV-Datei auswählen", "Bitte eine CSV-Datei mit Maschinen auswählen")
                {
                    AllowsMultipleSelection = false,
                    AllowedFileTypes = new[] { ".csv" },
                };

                Application.Run(openDialog);

                if (openDialog.Canceled)
                {
                    AddToHistory("CSV-Import abgebrochen.");
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

                AddToHistory($"Starte Import aus {Path.GetFileName(filePath)}...");
                var machinesCountFromCsv = await _dbService.InitializeMachinesFromCsvAsync(filePath);

                var successMsg = $"Import erfolgreich: {machinesCountFromCsv} Maschinen importiert.";
                AddToHistory(successMsg);

                Application.MainLoop.Invoke(() => {
                    MessageBox.Query("Import", successMsg, "OK");
                });
            }
            catch (Exception exception)
            {
                var errorMsg = $"Fehler Import: {exception.Message}";
                AddToHistory(errorMsg);
                Application.MainLoop.Invoke(() => {
                    MessageBox.ErrorQuery("Fehler", errorMsg, "OK");
                });
            }
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

        private void ShowSettingsDialog()
        {
            // Note: Ideally, the current values should be loaded from a configuration here.
            // For simplicity, we use defaults here.
            var settingsDialog = new SettingsDialog();
            Application.Run(settingsDialog);

            if (!settingsDialog.Canceled)
            {
                var msg = $"Einstellungen gespeichert:\n" +
                          $"API: {settingsDialog.ClientApiFqdn}:{settingsDialog.ClientApiPort}\n" +
                          $"Mongo: {settingsDialog.MongoDbFqdn}:{settingsDialog.MongoDbPort}\n" +
                          $"PgSQL: {settingsDialog.PgSqlDbFqdn}:{settingsDialog.PgSqlDbPort}/{settingsDialog.PgSqlDbName}\n\n" +
                          "Hinweis: Ein Neustart ist erforderlich, um Änderungen anzuwenden.";

                MessageBox.Query("Einstellungen", msg, "OK");

                // TODO: Store values in a configuration file so they can be used on the next startup (in Program.cs).
            }
        }
        // Neue Methode zum Aktualisieren des WebApi MenuItem Textes
        private void UpdateWebApiMenuItemText()
        {
            if (_webApiMenuItem != null)
            {
                _webApiMenuItem.Help = WebApi.IsRunning ? "Gestartet" : "Gestoppt";
            }
        }

    }
}