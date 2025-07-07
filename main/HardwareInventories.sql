create table HardwareInventories
(
    Id                   INTEGER
        primary key autoincrement,
    MachineId            INTEGER not null
        references Machines,
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

create index idx_hardware_machine_created
    on HardwareInventories (MachineId, CreatedAt);

