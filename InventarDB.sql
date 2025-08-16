-- This SQL script creates the Machines table.
-- The table contains information about the machines that are managed by the inventory system.
-- The table has the following columns:
-- Id: The unique identifier for the machine.
-- Name: The name of the machine.
-- OperatingSystem: The operating system of the machine.
-- LastSeen: The last time the machine was seen by the inventory system.
-- CreatedAt: The timestamp when the machine was first seen by the inventory system.
CREATE TABLE IF NOT EXISTS main.Machines
(
    Id              INTEGER
        primary key autoincrement,
    Name            TEXT not null
        unique,
    OperatingSystem TEXT,
    LastSeen        DATETIME,
    CreatedAt       DATETIME default CURRENT_TIMESTAMP
);
-- This SQL script creates an index on the Name column of the Machines table.
CREATE INDEX main.idx_machines_name
    ON main.Machines (Name);

-- This SQL script creates the HardwareInventories table.
-- The table contains information about the hardware of the machines that are managed by the inventory system.
-- The table has the following columns:
-- Id: The unique identifier for the hardware inventory record.
-- MachineId: The unique identifier for the machine.
-- ComputerModel: The model of the computer.
-- Architecture: The architecture of the computer.
-- ProcessorCores: The number of processor cores.
-- TotalMemoryGB: The total amount of memory in GB.
-- CreatedAt: The timestamp when the hardware inventory record was created.
CREATE TABLE IF NOT EXISTS main.SoftwareInventories
(
    Id                    INTEGER
        primary key autoincrement,
    MachineId             INTEGER not null
        references main.Machines,
    ProcessesJson         TEXT,
    InstalledSoftwareJson TEXT,
    ServicesJson          TEXT,
    EnvironmentJson       TEXT,
    StartupProgramsJson   TEXT,
    RuntimeJson           TEXT,
    CreatedAt             DATETIME default CURRENT_TIMESTAMP
);
-- This SQL script creates an index on the MachineId and CreatedAt columns of the SoftwareInventories table.
CREATE INDEX main.idx_software_machine_created
    ON main.SoftwareInventories (MachineId, CreatedAt);

-- This SQL script creates the HardwareInventories table.
-- The table contains information about the hardware of the machines that are managed by the inventory system.
-- The table has the following columns:
-- Id: The unique identifier for the hardware inventory record.
-- MachineId: The unique identifier for the machine.
-- ComputerModel: The model of the computer.
-- Architecture: The architecture of the computer.
-- ProcessorCores: The number of processor cores.
-- TotalMemoryGB: The total amount of memory in GB.
-- CreatedAt: The timestamp when the hardware inventory record was created.
CREATE TABLE IF NOT EXISTS main.HardwareInventories
(
    Id                   INTEGER
        primary key autoincrement,
    MachineId            INTEGER not null
        references main.Machines,
    ComputerName         TEXT,
    ComputerModel        TEXT,
    ComputerManufacturer TEXT,
    Architecture         TEXT,
    ProcessorName        TEXT,
    ProcessorCores       INTEGER,
    TotalMemoryGB        REAL,
    AvailableMemoryGB    REAL,
    MemoryUsagePercent   REAL,
    CreatedAt            DATETIME default CURRENT_TIMESTAMP
);
-- This SQL script creates an index on the MachineId and CreatedAt columns of the HardwareInventories table.
CREATE INDEX main.idx_hardware_machine_created
    ON main.HardwareInventories (MachineId, CreatedAt);

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

-- Query to retrieve the latest software inventory for a specific machine
SELECT * FROM LatestSoftwareInventoriesView
WHERE MachineId = @MachineId
LIMIT 1;

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

-- Query to retrieve the latest hardware inventory for a specific machine
SELECT * FROM LatestHardwareInventoriesView
WHERE MachineId = @MachineId
LIMIT 1;

-- Statistics view for ComputerModel distribution
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
CREATE VIEW IF NOT EXISTS ModelArchitectureStatisticsView AS
SELECT
    ComputerModel,
    Architecture,
    COUNT(*) as AnzahlMaschinen,
    COUNT(DISTINCT MachineId) as EinzigartigeMaschinen,
    ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM LatestHardwareInventoriesView), 2) as Prozentsatz,
    ROUND(AVG(ProcessorCores),0) as DurchschnittlicheKerne,
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

-- Top 5 ComputerModels
SELECT * FROM ComputerModelStatisticsView LIMIT 5;

-- Architectural distribution
SELECT * FROM ArchitectureStatisticsView;

-- Combined Statistics
SELECT * FROM ModelArchitectureStatisticsView
WHERE AnzahlMaschinen > @AnzahlSchwelle
ORDER BY AnzahlMaschinen DESC;

-- Overview of all statistics
SELECT * FROM HardwareStatisticsOverview;

SELECT * FROM HardwareStatisticsOverview
WHERE Anzahl > @AnzahlSchwelle;

-- Check if the Machines table exists and has data
-- This query checks if the Machines table exists and contains any records.
-- If the table does not exist or is empty, it will return false.
-- If the table exists and has records, it will return true.
SELECT EXISTS(SELECT 1 FROM machines);

-- 1) Check if the table 'Machines' exists
SELECT EXISTS (
    SELECT 1
    FROM sqlite_master
    WHERE type = 'table' AND name = 'Machines'
) AS MachinesExists;

-- 2) Check if the columns already exist
SELECT name AS ColumnName
FROM pragma_table_info('Machines')
WHERE name IN ('IPv4', 'FQDN', 'Disabled', 'Deprovisioned')
ORDER BY name;

-- 3) Add columns (execute individually, only if they do not yet exist)
-- Note: SQLite does not support "IF NOT EXISTS" for columns.
-- Execute the ALTER TABLE commands only if the respective column is missing.

-- IPv4 (Textual IPv4 address)
ALTER TABLE Machines ADD COLUMN IPv4 TEXT;

-- For IPv4 column
SELECT CASE
           WHEN NOT EXISTS (
               SELECT 1
               FROM pragma_table_info('Machines')
               WHERE name = 'IPv4'
           )
               THEN 'ALTER TABLE Machines ADD COLUMN IPv4 TEXT;'
           ELSE ''
           END AS SqlToExecute;


-- FQDN (Fully Qualified Domain Name)
ALTER TABLE Machines ADD COLUMN FQDN TEXT;

-- For FQDN column
SELECT CASE
           WHEN NOT EXISTS (
               SELECT 1
               FROM pragma_table_info('Machines')
               WHERE name = 'FQDN'
           )
               THEN 'ALTER TABLE Machines ADD COLUMN FQDN TEXT;'
           ELSE ''
           END AS SqlToExecute;

-- Disabled (Boolean flag as INTEGER 0/1)
ALTER TABLE Machines ADD COLUMN Disabled INTEGER NOT NULL DEFAULT 0;

-- For Disabled column
SELECT CASE
           WHEN NOT EXISTS (
               SELECT 1
               FROM pragma_table_info('Machines')
               WHERE name = 'Disabled'
           )
               THEN 'ALTER TABLE Machines ADD COLUMN Disabled INTEGER NOT NULL DEFAULT 0;'
           ELSE NULL
           END AS SqlToExecute;

-- Deprovisioned (Boolean flag as INTEGER 0/1; use the specified format)
ALTER TABLE Machines ADD COLUMN Deprovisioned INTEGER NOT NULL DEFAULT 0;

-- For Deprovisioned column
SELECT CASE
           WHEN NOT EXISTS (
               SELECT 1
               FROM pragma_table_info('Machines')
               WHERE name = 'Deprovisioned'
           )
               THEN 'ALTER TABLE Machines ADD COLUMN Deprovisioned INTEGER NOT NULL DEFAULT 0;'
           ELSE NULL
           END AS SqlToExecute;
