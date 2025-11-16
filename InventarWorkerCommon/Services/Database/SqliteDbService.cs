using System.Globalization;
using System.Transactions;
using CsvHelper;
using Dapper;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Software;
using InventarWorkerCommon.Models.SqlDatabase;
using Microsoft.Data.Sqlite;

namespace InventarWorkerCommon.Services.Database;

/// <summary>
/// Provides services to interact with the application's SQLite database.
/// </summary>
public class SqliteDbService
{
    private readonly string _connectionString;

    /// <summary>
    /// Handles interactions with the SQLite database, providing methods for initialization,
    /// data storage, retrieval, and maintenance operations.
    /// </summary>
    public SqliteDbService(string connectionString)
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
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                IPv4 TEXT,
                IPv6 TEXT,
                FQDN TEXT,
                Disabled INTEGER NOT NULL DEFAULT 0,
                Deprovisioned INTEGER NOT NULL DEFAULT 0,
                LastHarvested DATETIME
            );
            -- Ensure the Machines table is created with appropriate fields
            -- Ensure the HardwareInventories table is created with appropriate fields
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

            -- View to retrieve all active machines (not disabled or deprovisioned)
            -- This view selects all machines that are currently active, meaning they are not disabled or deprovisioned.
            -- It includes the Id, Name, IPv4, IPv6, FQDN, Disabled, Deprovisioned, LastSeen, and LastHarvested columns.
            -- The view can be used to quickly access the list of active machines without having to filter the Machines table each time.
            -- The view is created only if it does not already exist.
            -- This allows for efficient querying of active machines in the inventory system.
            -- The view can be used to monitor the status of machines and ensure that only active machines are considered in reports and analyses.
            -- The view can be queried like a regular table, making it easy to integrate into existing queries and reports.
            -- The view is particularly useful for inventory management, monitoring.
            CREATE VIEW IF NOT EXISTS AllActiveMachinesView AS
            SELECT Id, Name, IPv4, IPv6, FQDN, Disabled, Deprovisioned, LastSeen, LastHarvested 
            FROM Machines
            WHERE DISABLED = 0 AND DEPROVISIONED = 0;

            -- View to retrieve all active machines with network information (not disabled or deprovisioned)
            -- This view selects all machines that are currently active, meaning they are not disabled or deprovisioned.
            -- It includes the Id, Name, IPv4, IPv6, FQDN, Disabled, Deprovisioned, LastSeen, and LastHarvested columns.
            -- The view can be used to quickly access the list of active machines without having to filter the Machines table each time.
            -- The view is created only if it does not already exist.
            -- This allows for efficient querying of active machines in the inventory system.
            -- The view can be used to monitor the status of machines and ensure that only active machines are considered in reports and analyses.
            -- The view can be queried like a regular table, making it easy to integrate into existing queries and reports.
            -- The view is particularly useful for inventory management, monitoring.
            CREATE VIEW IF NOT EXISTS AllActiveMachinesWithNetworkInfoView AS
            SELECT Id, Name, IPv4, IPv6, FQDN, Disabled, Deprovisioned, LastSeen, LastHarvested
            FROM Machines
            WHERE DISABLED = 0 AND DEPROVISIONED = 0 AND (
                (IPv4 IS NOT NULL AND IPv4 != '') OR
                (IPv6 IS NOT NULL AND IPv6 != '') OR
                (FQDN IS NOT NULL AND FQDN != '')
                );
            
            -- View to retrieve all disabled machines (not deprovisioned)
            -- This view selects all machines that are currently disabled, meaning they are not deprovisioned.
            -- It includes the Id, Name, IPv4, IPv6, FQDN, Disabled, Deprovisioned, LastSeen, and LastHarvested columns.
            -- The view can be used to quickly access the list of disabled machines without having to filter the Machines table each time.
            -- The view is created only if it does not already exist.
            -- This allows for efficient querying of disabled machines in the inventory system.
            -- The view can be used to monitor the status of machines and ensure that only disabled machines are considered in reports and analyses.
            -- The view can be queried like a regular table, making it easy to integrate into existing queries and reports.
            -- The view is particularly useful for inventory management, monitoring.
            CREATE VIEW IF NOT EXISTS AllDisabledMachinesView AS
            SELECT Id, Name, IPv4, IPv6, FQDN, Disabled, Deprovisioned, LastSeen, LastHarvested
            FROM Machines
            WHERE DISABLED = 1 AND DEPROVISIONED = 0;
            
            -- View to retrieve all deprovisioned machines (disabled)
            -- This view selects all machines that are currently deprovisioned, meaning they are disabled and marked as deprovisioned.
            -- It includes the Id, Name, IPv4, IPv6, FQDN, Disabled, Deprovisioned, LastSeen, and LastHarvested columns.
            -- The view can be used to quickly access the list of deprovisioned machines without having to filter the Machines table each time.
            -- The view is created only if it does not already exist.
            -- This allows for efficient querying of deprovisioned machines in the inventory system.
            -- The view can be used to monitor the status of machines and ensure that only deprovisioned machines are considered in reports and analyses.
            -- The view can be queried like a regular table, making it easy to integrate into existing queries and reports.
            -- The view is particularly useful for inventory management, monitoring, and auditing purposes.
            -- It helps in identifying machines that have been deprovisioned.
            CREATE VIEW IF NOT EXISTS AllDeprovisionedMachinesView AS
            SELECT Id, Name, IPv4, IPv6, FQDN, Disabled, Deprovisioned, LastSeen, LastHarvested
            FROM Machines
            WHERE DISABLED = 1 AND DEPROVISIONED = 1;
            """;

        connection.Execute(createTablesAndViewsQuery);
    }

    /// <summary>
    /// Asynchronously saves a new machine entry to the SQLite database or updates the
    /// existing entry based on the machine's name. Additional fields are conditionally
    /// updated based on the context of the request.
    /// </summary>
    /// <param name="machine">The machine object containing the details to save or update.</param>
    /// <param name="isHarvester">Indicates whether additional fields should be updated. Default is false.</param>
    /// <returns>The unique identifier of the machine entry in the database.</returns>
    public async Task<int> SaveOrUpdateMachineAsync(Machine machine, bool isHarvester = false)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string selectQuery = "SELECT Id FROM Machines WHERE Name = @Name";
        var existingMachineId = await connection.QuerySingleOrDefaultAsync<int?>(selectQuery, new { machine.Name });

        if (existingMachineId.HasValue)
        {
            // When called by HarvesterWorkerService, more fields will be updated
            if (isHarvester)
            {
                const string updateQuery = @"
                    UPDATE Machines 
                    SET OperatingSystem = @OperatingSystem, 
                        LastSeen = @LastSeen,
                        IPv4 = @IPv4,
                        IPv6 = @IPv6,
                        FQDN = @FQDN,
                        LastHarvested = @LastHarvested
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateQuery, new
                {
                    machine.OperatingSystem,
                    machine.LastSeen,
                    machine.IPv4,
                    machine.IPv6,
                    machine.FQDN,
                    machine.LastHarvested,
                    Id = existingMachineId.Value
                });

                return existingMachineId.Value;

            }
            else
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

    /// <summary>
    /// Saves the hardware inventory information for a specified machine to the database.
    /// </summary>
    /// <param name="machineId">The unique identifier of the machine this hardware inventory belongs to.</param>
    /// <param name="hardware">The hardware inventory data containing system, CPU, and memory information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Saves the software inventory data for a specific machine to the SQLite database.
    /// </summary>
    /// <param name="machineId">The unique identifier of the machine for which the software inventory is being saved.</param>
    /// <param name="software">The software inventory object containing details about installed software, running processes, services, startup programs, environment variables, and runtime information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>
    /// Retrieves a list of all machines from the SQLite database, ordered by their name.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains
    /// a list of <see cref="Machine"/> objects representing the machines retrieved from the database.</returns>
    public async Task<List<Machine>> GetMachinesAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM Machines ORDER BY Name";
        var machines = await connection.QueryAsync<Machine>(query);
        return machines.ToList();
    }

    /// <summary>
    /// Retrieves a list of all active machines from the database, ordered by name,
    /// using the "AllActiveMachinesView" database view.
    /// </summary>
    /// <returns>A list of <see cref="MachineState"/> objects representing all active machines.</returns>
    public async Task<List<MachineState>> GetAllActiveMachinesAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM AllActiveMachinesView ORDER BY Name";
        var machines = await connection.QueryAsync<MachineState>(query);
        return machines.ToList();
    }

    /// <summary>
    /// Retrieves a list of all active machines along with their associated network information
    /// from the database. Active machines are those that are neither disabled nor deprovisioned.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of
    /// <see cref="MachineState"/> instances with network information.
    /// </returns>
    public async Task<List<MachineState>> GetAllActiveMachinesWithNetworkInfoAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM AllActiveMachinesWithNetworkInfoView ORDER BY Name";
        var machines = await connection.QueryAsync<MachineState>(query);
        return machines.ToList();
    }

    /// <summary>
    /// Retrieves a list of all deprovisioned machines from the SQLite database.
    /// </summary>
    /// <returns>A list of <see cref="MachineState"/> representing deprovisioned machines.</returns>
    public async Task<List<MachineState>> GetAllDeprovisionedMachinesAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM Machines AllDeprovisionedMachinesView ORDER BY Name";
        var machines = await connection.QueryAsync<MachineState>(query);
        return machines.ToList();
    }

    /// <summary>
    /// Retrieves a list of all machines marked as disabled within the system,
    /// including their identities, network details, and relevant status information.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of disabled machines represented by <see cref="MachineState"/> objects.</returns>
    public async Task<List<MachineState>> GetAllDisabledMachinesAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM Machines AllDisabledMachinesView ORDER BY Name";
        var machines = await connection.QueryAsync<MachineState>(query);
        return machines.ToList();
    }

    /// <summary>
    /// Retrieves a machine from the database using its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the machine to be retrieved.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the machine
    /// if found, or null if no machine matches the provided identifier.
    /// </returns>
    public async Task<Machine?> GetMachineByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM Machines WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Machine>(query, new { Id = id });
    }

    /// <summary>
    /// Retrieves a machine record from the database based on the provided machine name.
    /// </summary>
    /// <param name="machineName">The name of the machine to search for in the database.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the <see cref="Machine"/>
    /// object if a match is found, or <c>null</c> if no matching machine is found.
    /// </returns>
    public async Task<Machine?> GetMachineByNameAsync(string machineName)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT * FROM Machines WHERE Name = @MachineName";
        return await connection.QuerySingleOrDefaultAsync<Machine>(query, new { MachineName = machineName });
    }

    /// <summary>
    /// Retrieves the latest hardware inventory record for a specific machine from the database.
    /// </summary>
    /// <param name="machineId">The unique identifier of the machine whose hardware inventory is being retrieved.</param>
    /// <returns>A <see cref="HardwareInventories"/> object containing the latest hardware inventory information, or null if no record is found.</returns>
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

    /// <summary>
    /// Retrieves the latest software inventory record for a given machine ID from the database.
    /// </summary>
    /// <param name="machineId">The unique identifier of the machine for which the latest software inventory is to be retrieved.</param>
    /// <returns>A <see cref="SoftwareInventories"/> object representing the latest software inventory of the specified machine, or null if no record exists.</returns>
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

    /// <summary>
    /// Removes old records from the hardware and software inventories based on the specified retention period.
    /// </summary>
    /// <param name="daysToKeep">The number of days for which to retain records. Records older than this period will be deleted. Defaults to 30.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Checks if there are any machine records in the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if machine records exist, otherwise false.</returns>
    public async Task<bool> HasMachineRecordsAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT EXISTS(SELECT 1 FROM Machines)";
        return await connection.QuerySingleAsync<bool>(query);
    }

    /// <summary>
    /// Checks if there are existing hardware inventory records in the database.
    /// </summary>
    /// <returns>True if there is at least one hardware inventory record in the database; otherwise, false.</returns>
    public async Task<bool> HasHardwareInventoryRecordsAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        const string query = "SELECT EXISTS(SELECT 1 FROM HardwareInventories)";
        return await connection.QuerySingleAsync<bool>(query);
    }

    /// <summary>
    /// Checks whether there are any records in the SoftwareInventories table of the database.
    /// </summary>
    /// <returns>Returns a boolean indicating if software inventory records exist.</returns>
    public async Task<bool> HasSoftwareInventoryRecordsAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        const string query = "SELECT EXISTS(SELECT 1 FROM SoftwareInventories)";
        return await connection.QuerySingleAsync<bool>(query);
    }

    /// <summary>
    /// Retrieves the total count of machines stored in the database.
    /// </summary>
    /// <returns>The number of machines as an integer.</returns>
    public async Task<int> GetMachineCountAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT COUNT(*) FROM Machines";
        return await connection.QuerySingleAsync<int>(query);
    }

    /// <summary>
    /// Retrieves the total count of hardware inventory records stored in the database.
    /// </summary>
    /// <returns>An integer representing the total count of hardware inventory records.</returns>
    public async Task<int> GetHardwareInventoryCountAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        const string query = "SELECT COUNT(*) FROM HardwareInventories";
        return await connection.QuerySingleAsync<int>(query);
    }

    /// <summary>
    /// Retrieves the total count of software inventory records stored in the database.
    /// </summary>
    /// <returns>The total number of software inventory records as an integer.</returns>
    public async Task<int> GetSoftwareInventoryCountAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        const string query = "SELECT COUNT(*) FROM SoftwareInventories";
        return await connection.QuerySingleAsync<int>(query);
    }

    /// <summary>
    /// Imports machine data from a specified CSV file into the database. The method reads the CSV file,
    /// parses the data into machine-specific records, and stores them in the database within a transaction.
    /// </summary>
    /// <param name="csvFilePath">The file path to the CSV file containing machine data.</param>
    /// <returns>The total number of machines successfully imported into the database.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified CSV file cannot be found.</exception>
    /// <exception cref="Exception">Thrown when an error occurs during the import process, causing the transaction to roll back.</exception>
    public async Task<int> InitializeMachinesFromCsvAsync(string csvFilePath)
    {
        if (File.Exists(csvFilePath) is false)
        {
            throw new FileNotFoundException($"The specified CSV file does not exist {csvFilePath}.");
        }

        var importedCount = 0;
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = connection.BeginTransaction();
        try
        {
            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Register CSV header (optional, if the columns are named differently)
            csv.Context.RegisterClassMap<MachineMap>();

            var machines = csv.GetRecords<MachineFromCsv>().ToList();

            foreach (var machine in machines)
            {
                // Check if the machine already exists
                const string selectQuery = "SELECT Id FROM Machines WHERE Name = @Name";
                var existingMachineId = await connection.QuerySingleOrDefaultAsync<int?>(
                    selectQuery,
                    new {machine.Name},
                    transaction);

                if (existingMachineId.HasValue is false)
                {
                    const string insertQuery = @"
                    INSERT INTO Machines (Name, OperatingSystem, LastSeen, CreatedAt)
                    VALUES (@Name, @OperatingSystem, @LastSeen, @CreatedAt)";

                    await connection.ExecuteAsync(insertQuery, new
                    {
                        machine.Name,
                        machine.OperatingSystem,
                        LastSeen = machine.LastSeen ?? DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    }, transaction);

                    importedCount++;
                }
            }

            transaction.Commit();
            return importedCount;
        }
        catch
        {
            transaction.Rollback();
            throw new Exception("Error occurred while importing machines from CSV file. Transaction rolled back.");
        }
    }
}
