using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Service;
using Microsoft.Extensions.Logging;

namespace InventarWorkerCommon.Services.Hardware;

public class HardwareInventoryService
{
    private readonly ILogger<HardwareInventoryService> _logger;
    private readonly PerformanceCounter? _cpuCounter;
    private readonly PerformanceCounter? _memoryAvailableCounter;

    public HardwareInventoryService(ILogger<HardwareInventoryService> logger)
    {
        _logger = logger;
        
        try
        {
            // Performance Counter nur auf unterstützten Plattformen initialisieren
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _memoryAvailableCounter = new PerformanceCounter("Memory", "Available MBytes");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Performance Counter konnten nicht initialisiert werden");
        }
    }

    public async Task<HardwareInfo> CollectHardwareInfoAsync()
    {
        _logger.LogInformation("Sammle Hardware-Informationen...");

        var hardwareInfo = new HardwareInfo();

        try
        {
            // Parallel sammeln für bessere Performance
            var tasks = new Task[]
            {
                Task.Run(() => hardwareInfo.System = CollectSystemInfo()),
                Task.Run(() => hardwareInfo.Cpu = CollectCpuInfo()),
                Task.Run(() => hardwareInfo.Memory = CollectMemoryInfo()),
                Task.Run(() => hardwareInfo.Disks = CollectDiskInfo()),
                Task.Run(() => hardwareInfo.NetworkInterfaces = CollectNetworkInfo()),
                Task.Run(() => hardwareInfo.OperatingSystem = CollectOsInfo())
            };

            await Task.WhenAll(tasks);
            
            _logger.LogInformation("Hardware-Informationen erfolgreich gesammelt");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Hardware-Informationen");
        }

        return hardwareInfo;
    }

    private SystemInfo CollectSystemInfo()
    {
        try
        {
            return new SystemInfo
            {
                MachineName = Environment.MachineName,
                UserName = Environment.UserName,
                Domain = Environment.UserDomainName,
                Uptime = GetSystemUptime(),
                Platform = GetPlatformName(),
                Architecture = RuntimeInformation.ProcessArchitecture.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der System-Informationen");
            return new SystemInfo();
        }
    }

    private CpuInfo CollectCpuInfo()
    {
        try
        {
            var cpuInfo = new CpuInfo
            {
                ProcessorCount = Environment.ProcessorCount,
                Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
                ProcessorName = GetProcessorName()
            };

            // CPU-Auslastung ermitteln
            cpuInfo.CurrentUsage = GetCpuUsage();

            return cpuInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der CPU-Informationen");
            return new CpuInfo { ProcessorCount = Environment.ProcessorCount };
        }
    }

    private MemoryInfo CollectMemoryInfo()
    {
        try
        {
            var memoryInfo = new MemoryInfo
            {
                ManagedMemoryUsage = GC.GetTotalMemory(false)
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                CollectWindowsMemoryInfo(memoryInfo);
            }
            else
            {
                CollectUnixMemoryInfo(memoryInfo);
            }

            if (memoryInfo.TotalPhysicalMemory > 0)
            {
                memoryInfo.UsedPhysicalMemory = memoryInfo.TotalPhysicalMemory - memoryInfo.AvailablePhysicalMemory;
                memoryInfo.MemoryUsagePercentage = (double)memoryInfo.UsedPhysicalMemory / memoryInfo.TotalPhysicalMemory * 100;
            }

            return memoryInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Memory-Informationen");
            return new MemoryInfo();
        }
    }

    private List<DiskInfo> CollectDiskInfo()
    {
        var disks = new List<DiskInfo>();

        try
        {
            var drives = DriveInfo.GetDrives();
            
            foreach (var drive in drives)
            {
                try
                {
                    var diskInfo = new DiskInfo
                    {
                        DriveName = drive.Name,
                        DriveType = drive.DriveType.ToString(),
                        IsReady = drive.IsReady
                    };

                    if (drive.IsReady)
                    {
                        diskInfo.FileSystem = drive.DriveFormat;
                        diskInfo.TotalSize = drive.TotalSize;
                        diskInfo.AvailableSpace = drive.AvailableFreeSpace;
                        diskInfo.UsedSpace = drive.TotalSize - drive.AvailableFreeSpace;
                        diskInfo.UsagePercentage = (double)diskInfo.UsedSpace / diskInfo.TotalSize * 100;
                    }

                    disks.Add(diskInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Fehler beim Lesen der Laufwerk-Informationen für {DriveName}", drive.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Laufwerk-Informationen");
        }

        return disks;
    }

    private List<NetworkInfo> CollectNetworkInfo()
    {
        var networkInterfaces = new List<NetworkInfo>();

        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var networkInterface in interfaces)
            {
                try
                {
                    var networkInfo = new NetworkInfo
                    {
                        Name = networkInterface.Name,
                        Description = networkInterface.Description,
                        NetworkInterfaceType = networkInterface.NetworkInterfaceType.ToString(),
                        OperationalStatus = networkInterface.OperationalStatus.ToString(),
                        Speed = networkInterface.Speed
                    };

                    // MAC-Adresse
                    var physicalAddress = networkInterface.GetPhysicalAddress();
                    if (physicalAddress != null)
                    {
                        networkInfo.MacAddress = physicalAddress.ToString();
                    }

                    // IP-Adressen
                    var ipProperties = networkInterface.GetIPProperties();
                    foreach (var unicastAddress in ipProperties.UnicastAddresses)
                    {
                        networkInfo.IpAddresses.Add(unicastAddress.Address.ToString());
                    }

                    // Statistiken
                    var statistics = networkInterface.GetIPStatistics();
                    networkInfo.BytesSent = statistics.BytesSent;
                    networkInfo.BytesReceived = statistics.BytesReceived;

                    networkInterfaces.Add(networkInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Fehler beim Lesen der Netzwerk-Informationen für {InterfaceName}", networkInterface.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Netzwerk-Informationen");
        }

        return networkInterfaces;
    }

    private OsInfo CollectOsInfo()
    {
        try
        {
            return new OsInfo
            {
                Platform = GetPlatformName(),
                Version = Environment.OSVersion.Version.ToString(),
                ServicePack = Environment.OSVersion.ServicePack,
                Description = RuntimeInformation.OSDescription,
                Architecture = RuntimeInformation.OSArchitecture.ToString(),
                Is64Bit = Environment.Is64BitOperatingSystem,
                ProcessorCount = Environment.ProcessorCount,
                UserDomainName = Environment.UserDomainName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der OS-Informationen");
            return new OsInfo();
        }
    }

    private string GetPlatformName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "Windows";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "Linux";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "macOS";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            return "FreeBSD";
        
        return "Unknown";
    }

    private TimeSpan GetSystemUptime()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return TimeSpan.FromMilliseconds(Environment.TickCount);
            }
            else
            {
                // Für Unix-Systeme: /proc/uptime lesen
                return GetUnixUptime();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Uptime konnte nicht ermittelt werden");
            return TimeSpan.Zero;
        }
    }

    private TimeSpan GetUnixUptime()
    {
        try
        {
            if (File.Exists("/proc/uptime"))
            {
                var uptimeString = File.ReadAllText("/proc/uptime").Split(' ')[0];
                if (double.TryParse(uptimeString, out var uptimeSeconds))
                {
                    return TimeSpan.FromSeconds(uptimeSeconds);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unix-Uptime konnte nicht gelesen werden");
        }

        return TimeSpan.Zero;
    }

    private string GetProcessorName()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetWindowsProcessorName();
            }
            else
            {
                return GetUnixProcessorName();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Prozessor-Name konnte nicht ermittelt werden");
            return "Unknown";
        }
    }

    private string GetWindowsProcessorName()
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wmic",
                    Arguments = "cpu get name /format:list",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var lines = output.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("Name="))
                {
                    return line.Substring(5).Trim();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Windows-Prozessor-Name konnte nicht ermittelt werden");
        }

        return "Unknown";
    }

    private string GetUnixProcessorName()
    {
        try
        {
            if (File.Exists("/proc/cpuinfo"))
            {
                var lines = File.ReadAllLines("/proc/cpuinfo");
                foreach (var line in lines)
                {
                    if (line.StartsWith("model name"))
                    {
                        var parts = line.Split(':');
                        if (parts.Length > 1)
                        {
                            return parts[1].Trim();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unix-Prozessor-Name konnte nicht gelesen werden");
        }

        return "Unknown";
    }

    private double GetCpuUsage()
    {
        try
        {
            if (_cpuCounter != null)
            {
                return _cpuCounter.NextValue();
            }
            else
            {
                // Alternative Methode für Unix-Systeme
                return GetUnixCpuUsage();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CPU-Auslastung konnte nicht ermittelt werden");
            return 0;
        }
    }

    private double GetUnixCpuUsage()
    {
        // Vereinfachte CPU-Auslastung für Unix-Systeme
        // Hier könnte eine detailliertere Implementierung über /proc/stat erfolgen
        return 0;
    }

    private void CollectWindowsMemoryInfo(MemoryInfo memoryInfo)
    {
        try
        {
            if (_memoryAvailableCounter != null)
            {
                memoryInfo.AvailablePhysicalMemory = (long)_memoryAvailableCounter.NextValue() * 1024 * 1024; // MB zu Bytes
            }

            // Gesamten Speicher über Performance Counter oder WMI ermitteln
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wmic",
                    Arguments = "computersystem get TotalPhysicalMemory /format:list",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var lines = output.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("TotalPhysicalMemory="))
                {
                    var memoryString = line.Substring(20).Trim();
                    if (long.TryParse(memoryString, out var totalMemory))
                    {
                        memoryInfo.TotalPhysicalMemory = totalMemory;
                    }
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Windows-Memory-Informationen konnten nicht ermittelt werden");
        }
    }

    private void CollectUnixMemoryInfo(MemoryInfo memoryInfo)
    {
        try
        {
            if (File.Exists("/proc/meminfo"))
            {
                var lines = File.ReadAllLines("/proc/meminfo");
                
                foreach (var line in lines)
                {
                    if (line.StartsWith("MemTotal:"))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2 && long.TryParse(parts[1], out var totalKb))
                        {
                            memoryInfo.TotalPhysicalMemory = totalKb * 1024; // KB zu Bytes
                        }
                    }
                    else if (line.StartsWith("MemAvailable:"))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2 && long.TryParse(parts[1], out var availableKb))
                        {
                            memoryInfo.AvailablePhysicalMemory = availableKb * 1024; // KB zu Bytes
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unix-Memory-Informationen konnten nicht gelesen werden");
        }
    }

    public void Dispose()
    {
        _cpuCounter?.Dispose();
        _memoryAvailableCounter?.Dispose();
    }

    public async Task<object?> GetHardwareInfoAsync()
    {
        try
        {
            _logger.LogInformation("Hardware-Informationen werden abgerufen...");
        
            var hardwareInfo = await CollectHardwareInfoAsync();
        
            _logger.LogInformation("Hardware-Informationen erfolgreich abgerufen");
        
            return hardwareInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen der Hardware-Informationen");
            return null;
        }
    }
}