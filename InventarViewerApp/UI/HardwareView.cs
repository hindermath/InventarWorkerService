using InventarWorkerCommon.Services.Api;
using InventarWorkerCommon.Services.Database;
using Terminal.Gui;

namespace InventarViewerApp.UI
{
    /// <summary>
    /// A Terminal.Gui view that displays and persists hardware inventory data.
    /// </summary>
    public class HardwareView : FrameView
    {
        private readonly ApiService _apiService;
        private readonly SqliteDbService _dbService;
        private readonly PgSqlDbService _pgSqlDbService;
        private ListView _listView;
        private Label _statusLabel;
        private Button _refreshButton;
        private Button _saveButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareView"/> class.
        /// </summary>
        /// <param name="apiService">Service used to retrieve hardware inventory from the backend.</param>
        /// <param name="dbService">Service used to save data to the local SQLite database.</param>
        /// <param name="pgSqlDbService"></param>
        public HardwareView(ApiService apiService, SqliteDbService dbService, PgSqlDbService pgSqlDbService) : base("Hardware Inventar")
        {
            _apiService = apiService;
            _dbService = dbService;
            _pgSqlDbService = pgSqlDbService;
            
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
            
            // ListView für Hardware-Daten
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

        /// <summary>
        /// Refreshes the hardware inventory by calling the API and updating the view content.
        /// </summary>
        /// <returns>A task that represents the asynchronous refresh operation.</returns>
        public async Task RefreshData()
        {
            try
            {
                Application.MainLoop.Invoke(() => _statusLabel.Text = "Daten werden geladen...");
                
                var hardwareData = await _apiService.GetHardwareInventoryAsync();
                
                Application.MainLoop.Invoke(() => {
                    List<string> hwItems = new List<string>();
                    hwItems.Add($"System:");
                    hwItems.Add($"Name: {hardwareData.System.MachineName}, Arch: {hardwareData.System.Architecture}, Domain: {hardwareData.System.Domain}");
                    hwItems.Add("Operating System:");
                    hwItems.Add($"Arch: {hardwareData.OperatingSystem.Architecture}, Version: {hardwareData.OperatingSystem.Version}, Servicepack: ({hardwareData.OperatingSystem.ServicePack})");
                    hwItems.Add($"Desc: {hardwareData.OperatingSystem.Description}, Platform: {hardwareData.OperatingSystem.Platform}, 64 Bit: {hardwareData.OperatingSystem.Is64Bit}");
                    hwItems.Add("CPU:");
                    hwItems.Add($"Arch: {hardwareData.Cpu.Architecture}, Procesors: {hardwareData.Cpu.ProcessorCount}");
                    hwItems.Add("Memory:");
                    hwItems.Add($"Memory: {hardwareData.Memory.TotalPhysicalMemory / (1024 * 1024)} MB");
                    hwItems.Add($"Disk:");
                    foreach (var disk in hardwareData.Disks)
                    {
                        hwItems.Add($"  {disk.DriveName} - {disk.TotalSize / (1024 * 1024 * 1024)} GB");
                    }
                    hwItems.Add($"Network:");
                    foreach (var nic in hardwareData.NetworkInterfaces)
                    {
                        hwItems.Add($"  {nic.Name} - {nic.IpAddresses} ({nic.MacAddress})");
                    }

                    _listView.SetSource(hwItems);
                    _statusLabel.Text = $"Hardware Daten geladen: {DateTime.Now}";
                });
            }
            catch (Exception ex)
            {
                Application.MainLoop.Invoke(() => {
                    _statusLabel.Text = $"Fehler: {ex.Message}";
                    MessageBox.ErrorQuery("Fehler", $"Fehler beim Laden der Hardware-Daten: {ex.Message}", "OK");
                });
            }
        }

        private async Task SaveToDatabase()
        {
            try
            {
                Application.MainLoop.Invoke(() => _statusLabel.Text = "Speichere in Datenbank...");
                
                var hardwareData = await _apiService.GetHardwareInventoryAsync();
                
                var machine = await _dbService.GetMachineByNameAsync(hardwareData.System.MachineName);
                await _dbService.SaveHardwareInventoryAsync(machine.Id, hardwareData);

                Application.MainLoop.Invoke(() => {
                    _statusLabel.Text = $"In Datenbank gespeichert: {DateTime.Now}";
                    MessageBox.Query("Erfolg", $"Ermittelte Hardware in Datenbank gespeichert", "OK");
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