create table SoftwareInventories
(
    Id                    INTEGER
        primary key autoincrement,
    MachineId             INTEGER not null
        references Machines,
    ProcessesJson         TEXT,
    InstalledSoftwareJson TEXT,
    ServicesJson          TEXT,
    EnvironmentJson       TEXT,
    StartupProgramsJson   TEXT,
    RuntimeJson           TEXT,
    CreatedAt             DATETIME default CURRENT_TIMESTAMP
);

create index idx_software_machine_created
    on SoftwareInventories (MachineId, CreatedAt);

