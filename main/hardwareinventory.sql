CREATE VIEW hardwareinventory AS
SELECT
    m.ID AS MachineID,
    m.Name AS MachineName,
    h.Architecture,
    h.ProcessorCores,
    ROUND(h.TotalMemoryGB / 1024 /1024 /1024,2) AS TotalMemoryGB,
    Round(h.AvailableMemoryGB / 1024 / 1024 / 1024,2) AS AvailableMemoryGB,
    ROUND(h.MemoryUsagePercent,2) AS MemoryUsagePercent
FROM 
    main.Machines m
INNER JOIN 
        main.HardwareInventories h ON m.ID = h.MachineID
GROUP BY 
    m.Name
ORDER BY 
    m.Name ASC;

