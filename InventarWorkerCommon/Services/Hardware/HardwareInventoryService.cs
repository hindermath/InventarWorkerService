using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Service;
using Microsoft.Extensions.Logging;
using Universe.CpuUsage;

namespace InventarWorkerCommon.Services.Hardware;

/// <summary>
/// Collects hardware-related information from the current system.
/// </summary>
public class HardwareInventoryService
{
    private readonly ILogger<HardwareInventoryService> _logger;
    private readonly PerformanceCounter? _cpuCounter;
    private readonly PerformanceCounter? _memoryAvailableCounter;

    /*
     * 2. Native macOS‑API via P/Invoke (host_statistics64)
     * Wenn du es selbst implementieren willst, nutzt macOS die Kernel‑API host_statistics64.
     * Damit bekommst du die CPU‑Ticks für:
     * - user
     * - system
     * - idle
     * - nice
     * Code (funktioniert auf macOS Intel & Apple Silicon)
     */
    private const int HOST_CPU_LOAD_INFO = 3;
    private const int HOST_CPU_LOAD_INFO_COUNT = 4;

    [DllImport("libSystem.dylib")]
    private static extern int host_statistics64(
        IntPtr host,
        int flavor,
        IntPtr data,
        ref int count);

    [DllImport("libSystem.dylib")]
    private static extern IntPtr mach_host_self();


    /// <summary>
    /// Initializes a new instance of HardwareInventoryService with the provided logger.
    /// </summary>
    /// <param name="logger">The logger used for diagnostic output.</param>
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

    /// <summary>
    /// Collects and aggregates hardware information asynchronously.
    /// </summary>
    /// <returns>A populated HardwareInfo instance.</returns>
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
                Uptime = TimeSpan.FromSeconds(Environment.TickCount / 1000.0),
                Platform = GetPlatformName(),
                Architecture = RuntimeInformation.OSArchitecture.ToString()
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
            return new CpuInfo
            {
                ProcessorCount = Environment.ProcessorCount,
                Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
                ProcessorName = GetProcessorName(),
                CurrentUsage = GetCpuUsage()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der CPU-Informationen");
            return new CpuInfo();
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

    private string GetProcessorName()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetWindowsProcessorName();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return GetOsxProcessorName();
            }
            else
            {
                return GetUnixProcessorName();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Prozessor-Name konnte nicht ermittelt werden");
            return "Unknown processor";
        }
    }

    private string GetOsxProcessorName()
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sysctl",
                    Arguments = "-n machdep.cpu.brand_string",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return string.IsNullOrWhiteSpace(output) ? "Unknown" : output.Trim();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "OSX-Prozessor-Name konnte nicht ermittelt werden");
            return "Unknown";

        }
        return "Unknown";
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
            #region GetCpuUsagePerSystem
            /*
            // Windows Performance Counter verwenden
            if (_cpuCounter != null)
            {
                return _cpuCounter.NextValue();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return GetOsxCpuUsage();
            }
            else
            {
                // Alternative Methode für Unix-Systeme
                return GetUnixCpuUsage();
            }
            */
            #endregion

            #region GetCpuUsagePerUniverseCpuUsage

            var cpuUsage = CpuUsage.GetByProcess();
            if (cpuUsage != null)
            {
                return cpuUsage.Value.UserUsage.TotalSeconds + cpuUsage.Value.KernelUsage.TotalSeconds;
            }

            #endregion
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CPU-Auslastung konnte nicht ermittelt werden");
            return 0;
        }
        return 0;
    }

    private double GetOsxCpuUsage()
    {
        IntPtr host = mach_host_self();
        int count = HOST_CPU_LOAD_INFO_COUNT;

        IntPtr ptr = Marshal.AllocHGlobal(sizeof(ulong) * count);
        try
        {
            int result = host_statistics64(host, HOST_CPU_LOAD_INFO, ptr, ref count);
            if (result != 0) return -1;

            ulong user = (ulong) Marshal.ReadInt64(ptr, 0);
            ulong system = (ulong) Marshal.ReadInt64(ptr, 8);
            ulong idle = (ulong) Marshal.ReadInt64(ptr, 16);
            ulong nice = (ulong) Marshal.ReadInt64(ptr, 24);

            ulong total = user + system + idle + nice;

            return Math.Round((double) (total - idle) / total * 100, 2);
        }
        catch (Exception e)
        {
           _logger.LogWarning(e, "OSX-CPU-Auslastung konnte nicht ermittelt werden");
            return 0;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    private double GetUnixCpuUsage()
    {
        try
        {
            if (File.Exists("/proc/stat"))
            {
                var lines = File.ReadAllLines("/proc/stat");
                var parts = lines[0].Split(' ');
                if (parts.Length > 1)
                {
                    return Double.Parse(parts[2].Trim());
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error collecting Unix CPU utilization");
            return 0.0;
        }
        return 0.0;
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

    /// <summary>
    /// Releases unmanaged resources held by performance counters.
    /// </summary>
    public void Dispose()
    {
        _cpuCounter?.Dispose();
        _memoryAvailableCounter?.Dispose();
    }

    /// <summary>
    /// High-level method to get hardware information with logging and error handling.
    /// </summary>
    /// <returns>The collected hardware info object or null if an error occurred.</returns>
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