using Dapper;
using Microsoft.Data.Sqlite;
using InventarViewerApp.Models;
using InventarViewerApp.Models.Hardware;
using InventarViewerApp.Models.Software;

namespace InventarViewerApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS HardwareInventory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        MachineName TEXT,
                        Processor TEXT,
                        Memory TEXT,
                        DiskSpace TEXT,
                        NetworkInfo TEXT,
                        Timestamp TEXT
                    );
                    
                    CREATE TABLE IF NOT EXISTS SoftwareInventory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        OperatingSystem TEXT,
                        InstalledApplications TEXT,
                        Updates TEXT,
                        Timestamp TEXT
                    );
                ");
            }
        }

        public async Task SaveHardwareInventoryAsync(HardwareInventory hardware)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                await connection.ExecuteAsync(@"
                    INSERT INTO HardwareInventory (MachineName, Processor, Memory, DiskSpace, NetworkInfo, Timestamp)
                    VALUES (@MachineName, @Processor, @Memory, @DiskSpace, @NetworkInfo, @Timestamp)",
                    hardware);
            }
        }

        public async Task SaveSoftwareInventoryAsync(SoftwareInventory software)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                await connection.ExecuteAsync(@"
                    INSERT INTO SoftwareInventory (OperatingSystem, InstalledApplications, Updates, Timestamp)
                    VALUES (@OperatingSystem, @InstalledApplications, @Updates, @Timestamp)",
                    software);
            }
        }

        public async Task<List<HardwareInventory>> GetHardwareInventoryAsync()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                var result = await connection.QueryAsync<HardwareInventory>(
                    "SELECT * FROM HardwareInventory ORDER BY Timestamp DESC");
                
                return result.AsList();
            }
        }

        public async Task<List<SoftwareInventory>> GetSoftwareInventoryAsync()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                var result = await connection.QueryAsync<SoftwareInventory>(
                    "SELECT * FROM SoftwareInventory ORDER BY Timestamp DESC");
                
                return result.AsList();
            }
        }
    }
}