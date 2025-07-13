using Dapper;
using InventarViewerApp.Models.Database;
using InventarViewerApp.Models.Hardware;
using InventarViewerApp.Models.Software;
using Microsoft.Data.Sqlite;

namespace InventarViewerApp.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var createTablesAndViewsQuery = @"
            CREATE TABLE IF NOT EXISTS Machines (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                OperatingSystem TEXT,
                LastSeen DATETIME,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
            );

            CREATE TABLE IF NOT EXISTS HardwareInventories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                MachineId INTEGER NOT NULL,
                ComputerName TEXT,
                ComputerModel TEXT,
                ComputerManufacturer TEXT,
                Architecture TEXT,
                ProcessorName TEXT,
                ProcessorCores INTEGER,
                TotalMemoryGB REAL,
                AvailableMemoryGB REAL,
                MemoryUsagePercent REAL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (MachineId) REFERENCES Machines(Id)
            );

            CREATE TABLE IF NOT EXISTS SoftwareInventories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                MachineId INTEGER NOT NULL,
                ProcessesJson TEXT,
                InstalledSoftwareJson TEXT,
                ServicesJson TEXT,
                EnvironmentJson TEXT,
                StartupProgramsJson TEXT,
                RuntimeJson TEXT,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (MachineId) REFERENCES Machines(Id)
            );

            CREATE INDEX IF NOT EXISTS idx_machines_name ON Machines(Name);
            CREATE INDEX IF NOT EXISTS idx_hardware_machine_created ON HardwareInventories(MachineId, CreatedAt);
            CREATE INDEX IF NOT EXISTS idx_software_machine_created ON SoftwareInventories(MachineId, CreatedAt);

            -- This SQL script creates a view named `hardwareinventory` that aggregates hardware information 
            -- from the `Machines` and `HardwareInventories` tables.
            -- It includes machine ID, name, architecture, processor cores, total memory, 
            -- available memory, and memory usage percentage.
            CREATE VIEW IF NOT EXISTS hardwareinventory AS
            SELECT
                m.ID AS MachineID,
                m.Name AS MachineName,
                h.Architecture,
                h.ProcessorCores,
                ROUND(h.TotalMemoryGB / 1024 /1024 /1024,2) AS TotalMemoryGB,
                Round(h.AvailableMemoryGB / 1024 / 1024 / 1024,2) AS AvailableMemoryGB,
                ROUND(h.MemoryUsagePercent,2) AS MemoryUsagePercent
            FROM 
                Machines m
            INNER JOIN 
                    HardwareInventories h ON m.ID = h.MachineID
            GROUP BY 
                m.Name
            ORDER BY 
                m.Name ASC;
        ";

        connection.Execute(createTablesAndViewsQuery);
    }

    public async Task<int> SaveOrUpdateMachineAsync(Machine machine)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string selectQuery = "SELECT Id FROM Machines WHERE Name = @Name";
        var existingMachineId = await connection.QuerySingleOrDefaultAsync<int?>(selectQuery, new { machine.Name });

        if (existingMachineId.HasValue)
        {
            const string updateQuery = @"
                UPDATE Machines 
                SET OperatingSystem = @OperatingSystem, 
                    LastSeen = @LastSeen 
                WHERE Id = @Id";

            await connection.ExecuteAsync(updateQuery, new
            {
                machine.OperatingSystem,
                machine.LastSeen,
                Id = existingMachineId.Value
            });

            return existingMachineId.Value;
        }
        else
        {
            const string insertQuery = @"
                INSERT INTO Machines (Name, OperatingSystem, LastSeen, CreatedAt)
                VALUES (@Name, @OperatingSystem, @LastSeen, @CreatedAt);
                SELECT last_insert_rowid();";

            var machineId = await connection.QuerySingleAsync<int>(insertQuery, new
            {
                machine.Name,
                machine.OperatingSystem,
                machine.LastSeen,
                CreatedAt = DateTime.UtcNow
            });

            return machineId;
        }
    }

    public async Task SaveHardwareInventoryAsync(int machineId, HardwareInventory hardware)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string insertQuery = @"
            INSERT INTO HardwareInventories 
            (MachineId, ComputerName, ComputerModel, ComputerManufacturer, Architecture,
             ProcessorName, ProcessorCores,
             TotalMemoryGB, AvailableMemoryGB, MemoryUsagePercent, CreatedAt)
            VALUES 
            (@MachineId, @ComputerName, @ComputerModel, @ComputerManufacturer, @Architecture,
             @ProcessorName, @ProcessorCores,
             @TotalMemoryGB, @AvailableMemoryGB, @MemoryUsagePercent, @CreatedAt)";

        await connection.ExecuteAsync(insertQuery, new
        {
            MachineId = machineId,
            ComputerName = hardware.System.MachineName,
            ComputerModel = hardware.System.Platform,
            ComputerManufacturer = hardware.System.UserName,
            hardware.System.Architecture,
            hardware.Cpu.ProcessorName,
            ProcessorCores = hardware.Cpu.ProcessorCount,
            TotalMemoryGB = hardware.Memory.TotalPhysicalMemory,
            AvailableMemoryGB = hardware.Memory.AvailablePhysicalMemory,
            MemoryUsagePercent = hardware.Memory.MemoryUsagePercentage,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task SaveSoftwareInventoryAsync(int machineId, SoftwareInventory software)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string insertQuery = @"
            INSERT INTO SoftwareInventories 
            (MachineId, ProcessesJson, InstalledSoftwareJson, ServicesJson, EnvironmentJson, StartupProgramsJson, RuntimeJson, CreatedAt)
            VALUES 
            (@MachineId, @ProcessesJson, @InstalledSoftwareJson, @ServicesJson, @EnvironmentJson, @StartupProgramsJson, @RuntimeJson, @CreatedAt)";

        await connection.ExecuteAsync(insertQuery, new
        {
            MachineId = machineId,
            ProcessesJson = System.Text.Json.JsonSerializer.Serialize(software.RunningProcesses),
            InstalledSoftwareJson = System.Text.Json.JsonSerializer.Serialize(software.InstalledSoftware),
            ServicesJson = System.Text.Json.JsonSerializer.Serialize(software.WindowsServices),
            EnvironmentJson = System.Text.Json.JsonSerializer.Serialize(software.EnvironmentVariables),
            StartupProgramsJson = System.Text.Json.JsonSerializer.Serialize(software.StartupPrograms),
            RuntimeJson = System.Text.Json.JsonSerializer.Serialize(software.Runtime),
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task<List<Machine>> GetMachinesAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM Machines ORDER BY Name";
        var machines = await connection.QueryAsync<Machine>(query);
        return machines.ToList();
    }

    public async Task<Machine?> GetMachineByNameAsync(string machineName)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM Machines WHERE Name = @MachineName";
        return await connection.QuerySingleOrDefaultAsync<Machine>(query, new { MachineName = machineName });
    }

    public async Task<HardwareInventory?> GetLatestHardwareInventoryAsync(int machineId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT * FROM HardwareInventories 
            WHERE MachineId = @MachineId 
            ORDER BY CreatedAt DESC 
            LIMIT 1";

        return await connection.QuerySingleOrDefaultAsync<HardwareInventory>(query, new { MachineId = machineId });
    }

    public async Task<SoftwareInventory?> GetLatestSoftwareInventoryAsync(int machineId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT * FROM SoftwareInventories 
            WHERE MachineId = @MachineId 
            ORDER BY CreatedAt DESC 
            LIMIT 1";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(query, new { MachineId = machineId });

        if (result == null)
            return null;

        return new SoftwareInventory
        {
            RunningProcesses = System.Text.Json.JsonSerializer.Deserialize<List<ProcessInfo>>(result.ProcessesJson), // ?? new(),
            InstalledSoftware = System.Text.Json.JsonSerializer.Deserialize<List<SoftwareInfo>>(result.InstalledSoftwareJson), // ?? new(),
            WindowsServices = System.Text.Json.JsonSerializer.Deserialize<List<ServiceInfo>>(result.ServicesJson) //?? new()
        };
    }

    public async Task CleanupOldRecordsAsync(int daysToKeep = 30)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);

        const string deleteHardwareQuery = "DELETE FROM HardwareInventories WHERE CreatedAt < @CutoffDate";
        const string deleteSoftwareQuery = "DELETE FROM SoftwareInventories WHERE CreatedAt < @CutoffDate";

        await connection.ExecuteAsync(deleteHardwareQuery, new { CutoffDate = cutoffDate });
        await connection.ExecuteAsync(deleteSoftwareQuery, new { CutoffDate = cutoffDate });
    }
}
