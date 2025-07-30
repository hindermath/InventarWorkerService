using Dapper;
using InventarViewerApp.Models.Database;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Software;
using Microsoft.Data.Sqlite;

namespace InventarViewerApp.Services;

/// <summary>
/// Provides services to interact with the application's SQLite database.
/// </summary>
public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Initializes the application's database by setting up required structures
    /// and ensuring that all necessary configurations are applied.
    /// This method should be called during the application's startup process
    /// to prepare the database for use.
    /// </summary>
    public void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var createTablesAndViewsQuery = 
            """
            CREATE TABLE IF NOT EXISTS Machines (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                OperatingSystem TEXT,
                LastSeen DATETIME,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
            );
            -- Ensure the Machines table is created with appropriate fields
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
            -- Ensure the SoftwareInventories table is created with appropriate fields
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
            -- Create indexes to improve query performance
            CREATE INDEX IF NOT EXISTS idx_machines_name ON Machines(Name);
            CREATE INDEX IF NOT EXISTS idx_hardware_machine_created ON HardwareInventories(MachineId, CreatedAt);
            CREATE INDEX IF NOT EXISTS idx_software_machine_created ON SoftwareInventories(MachineId, CreatedAt);

            -- This SQL script creates a view named `hardwareinventory` that aggregates hardware information 
            -- from the `Machines` and `HardwareInventories` tables.
            -- It includes machine ID, name, architecture, processor cores, total memory, 
            -- available memory, and memory usage percentage.
            CREATE VIEW IF NOT EXISTS HardwareInventoryView AS
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

            -- This SQL script retrieves the latest software and hardware inventory records for each machine.
            -- It uses a common table expression (CTE) to find the latest records based on the CreatedAt timestamp.
            -- The script also creates a view for the latest software inventories.
            CREATE VIEW IF NOT EXISTS main.LatestSoftwareInventoriesView AS
            SELECT si.*
            FROM main.SoftwareInventories si
                     INNER JOIN (
                SELECT MachineId, MAX(CreatedAt) as MaxCreatedAt
                FROM main.SoftwareInventories
                GROUP BY MachineId
            ) latest ON si.MachineId = latest.MachineId AND si.CreatedAt = latest.MaxCreatedAt
            ORDER BY si.CreatedAt DESC;

            -- This SQL script retrieves the latest hardware inventory records for each machine.
            -- It uses a common table expression (CTE) to find the latest records based on the CreatedAt timestamp.
            -- It also creates a view for the latest hardware inventories.
            CREATE VIEW IF NOT EXISTS LatestHardwareInventoriesView AS
            SELECT * FROM (
                              SELECT *,
                                     ROW_NUMBER() OVER (PARTITION BY MachineId ORDER BY CreatedAt DESC) as rn
                              FROM main.HardwareInventories
                          ) ranked
            WHERE rn = 1
            ORDER BY CreatedAt DESC;

            -- Statistics view for ComputerModel distribution
            -- This SQL script creates a view that aggregates statistics for each ComputerModel
            -- including the count of machines, unique machines, percentage of total machines,
            -- and the first and last recorded timestamps.
            CREATE VIEW IF NOT EXISTS ComputerModelStatisticsView AS
            SELECT
                ComputerModel,
                COUNT(*) as AnzahlMaschinen,
                COUNT(DISTINCT MachineId) as EinzigartigeMaschinen,
                ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM LatestHardwareInventoriesView), 2) as Prozentsatz,
                MIN(CreatedAt) as ErsteErfassung,
                MAX(CreatedAt) as LetzteErfassung
            FROM LatestHardwareInventoriesView
            WHERE ComputerModel IS NOT NULL
              AND ComputerModel != ''
            GROUP BY ComputerModel
            ORDER BY AnzahlMaschinen DESC;

            -- Statistics view for architecture distribution
            -- This SQL script creates a view that aggregates statistics for each architecture
            -- including the count of machines, unique machines, percentage of total machines,
            
            CREATE VIEW IF NOT EXISTS ArchitectureStatisticsView AS
            SELECT
                Architecture,
                COUNT(*) as AnzahlMaschinen,
                COUNT(DISTINCT MachineId) as EinzigartigeMaschinen,
                ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM LatestHardwareInventoriesView), 2) as Prozentsatz,
                AVG(ProcessorCores) as DurchschnittlicheKerne,
                ROUND(AVG(TotalMemoryGB / 1024 /1024 /1024),2) as DurchschnittlicherSpeicherGB,
                MIN(CreatedAt) as ErsteErfassung,
                MAX(CreatedAt) as LetzteErfassung
            FROM LatestHardwareInventoriesView
            WHERE Architecture IS NOT NULL
              AND Architecture != ''
            GROUP BY Architecture
            ORDER BY AnzahlMaschinen DESC;

            -- Combined statistics view for ComputerModel and Architecture
            -- This SQL script creates a view that aggregates statistics for each combination 
            -- of ComputerModel and Architecture,
            -- including the count of machines, unique machines, percentage of total machines,
            -- average processor cores, average total memory in GB, and the first and last recorded timestamps.
            CREATE VIEW IF NOT EXISTS ModelArchitectureStatisticsView AS
            SELECT
                ComputerModel,
                Architecture,
                COUNT(*) as AnzahlMaschinen,
                COUNT(DISTINCT MachineId) as EinzigartigeMaschinen,
                ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM LatestHardwareInventoriesView), 2) as Prozentsatz,
                ROUND(AVG(ProcessorCores), 0) as DurchschnittlicheKerne,
                ROUND(AVG(TotalMemoryGB / 1024 / 1024 / 1024),2) as DurchschnittlicherSpeicherGB,
                MIN(CreatedAt) as ErsteErfassung,
                MAX(CreatedAt) as LetzteErfassung
            FROM LatestHardwareInventoriesView
            WHERE ComputerModel IS NOT NULL
              AND ComputerModel != ''
              AND Architecture IS NOT NULL
              AND Architecture != ''
            GROUP BY ComputerModel, Architecture
            ORDER BY AnzahlMaschinen DESC;

            -- Advanced Hardware Statistics View
            -- This SQL script creates a view that provides an overview of hardware statistics
            -- including the count of machines, unique machines, percentage of total machines,
            -- and the first and last recorded timestamps for ComputerModel and Architecture.
            CREATE VIEW IF NOT EXISTS HardwareStatisticsOverview AS
            SELECT
                'ComputerModel' as Kategorie,
                ComputerModel as Wert,
                COUNT(*) as Anzahl,
                ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM LatestHardwareInventoriesView), 2) as Prozentsatz
            FROM LatestHardwareInventoriesView
            WHERE ComputerModel IS NOT NULL AND ComputerModel != ''
            GROUP BY ComputerModel

            UNION ALL

            SELECT
                'Architecture' as Kategorie,
                Architecture as Wert,
                COUNT(*) as Anzahl,
                ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM LatestHardwareInventoriesView), 2) as Prozentsatz
            FROM LatestHardwareInventoriesView
            WHERE Architecture IS NOT NULL AND Architecture != ''
            GROUP BY Architecture

            ORDER BY Kategorie, Anzahl DESC;
            """;

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
    
    public async Task<Machine?> GetMachineByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM Machines WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Machine>(query, new { Id = id });
    }

    public async Task<Machine?> GetMachineByNameAsync(string machineName)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM Machines WHERE Name = @MachineName";
        return await connection.QuerySingleOrDefaultAsync<Machine>(query, new { MachineName = machineName });
    }

    public async Task<HardwareInventories?> GetLatestHardwareInventoryAsync(int machineId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT * FROM HardwareInventories 
            WHERE MachineId = @MachineId 
            ORDER BY CreatedAt DESC 
            LIMIT 1";

        return await connection.QuerySingleOrDefaultAsync<HardwareInventories>(query, new { MachineId = machineId });
    }
    
    public async Task<SoftwareInventories?> GetLatestSoftwareInventoryAsync(int machineId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT * FROM SoftwareInventories 
            WHERE MachineId = @MachineId 
            ORDER BY CreatedAt DESC 
            LIMIT 1";

        //return await connection.QuerySingleOrDefaultAsync<SoftwareInventories>(query, new { MachineId = machineId });
        
        var result = await connection.QuerySingleOrDefaultAsync<SoftwareInventories>(query, new { MachineId = machineId });

        if (result == null)
            return null;

        return new SoftwareInventories
        {
            Id = result.Id,
            MachineId = result.MachineId,
            ProcessesJson = result.ProcessesJson, //System.Text.Json.JsonSerializer.Deserialize<List<ProcessInfo>>(result.ProcessesJson),
            InstalledSoftwareJson = result.InstalledSoftwareJson, //System.Text.Json.JsonSerializer.Deserialize<List<SoftwareInfo>>(result.InstalledSoftwareJson),
            ServicesJson = result.ServicesJson, //System.Text.Json.JsonSerializer.Deserialize<List<ServiceInfo>>(result.ServicesJson)
            EnvironmentJson = result.EnvironmentJson, //System.Text.Json.JsonSerializer.Deserialize<List<EnvironmentVariable>>(result.EnvironmentJson),
            StartupProgramsJson = result.StartupProgramsJson, //System.Text.Json.JsonSerializer.Deserialize<List<StartupProgram>>(result.StartupProgramsJson),
            RuntimeJson = result.RuntimeJson, //System.Text.Json.JsonSerializer.Deserialize<RuntimeInfo>(result.RuntimeJson),
            CreatedAt = result.CreatedAt
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
