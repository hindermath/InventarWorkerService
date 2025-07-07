create table Machines
(
    Id              INTEGER
        primary key autoincrement,
    Name            TEXT not null
        unique,
    OperatingSystem TEXT,
    LastSeen        DATETIME,
    CreatedAt       DATETIME default CURRENT_TIMESTAMP
);

create index idx_machines_name
    on Machines (Name);

