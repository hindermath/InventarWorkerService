using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Management.Automation;
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

    // Felder für die CPU-Berechnung (Zwei-Punkt-Messung)
    // OSX CPU Felder
    private ulong _lastOsxUser;
    private ulong _lastOsxSystem;
    private ulong _lastOsxIdle;
    private ulong _lastOsxNice;
    private DateTime? _lastCpuCheck;

    // Unix CPU Felder
    private ulong _lastUnixUser;
    private ulong _lastUnixNice;
    private ulong _lastUnixSystem;
    private ulong _lastUnixIdle;
    private ulong _lastUnixIowait;
    private ulong _lastUnixIrq;
    private ulong _lastUnixSoftirq;
    private bool _unixCpuInitialized;

    // Felder für die CPU-Berechnung (Zwei-Punkt-Messung)
    // mit der Universe.CpuUsage NuGet-Bibliothek
    private CpuUsage? _lastLibraryCpuUsage;
    private DateTime? _lastLibraryCpuCheck;

    // Felder für die CPU-Berechnung (Zwei-Punkt-Messung)
    // mit Environment.CpuUsage (Systemweit)
    private Environment.ProcessCpuUsage? _lastEnvCpuUsage;
    private DateTime? _lastEnvCpuCheck;


    /*
     * host_statistics64 (macOS Mach Kernel API)
     * ----------------------------------------
     * Dokumentation:
     * Die Funktion host_statistics64 ist Teil des Mach-Kernels in macOS (XNU) und wird über die libSystem.dylib exportiert.
     * Sie dient zum Abrufen von statistischen Informationen über den Host auf 64-Bit-Basis.
     * 
     * Header-Dateien:
     * Die Header-Dateien sind Teil des macOS SDKs und befinden sich typischerweise unter:
     * /Library/Developer/CommandLineTools/SDKs/MacOSX.sdk/usr/include/mach/host_info.h
     * /Library/Developer/CommandLineTools/SDKs/MacOSX.sdk/usr/include/mach/mach_host.h
     * (Der Pfad kann je nach installierter Xcode-Version oder CommandLineTools variieren).
     * 
     * Man-Pages:
     * Informationen zu Mach-Kernel-Aufrufen können oft über die Man-Pages abgerufen werden (Sektion 2):
     * - man host_statistics (allgemeine Informationen, falls installiert)
     * Hinweis: Auf vielen modernen macOS-Systemen sind die Mach-Kernel-Dokumentationen primär über die Apple Developer Documentation online verfügbar.
     * 
     * Definition in den Apple XNU-Sourcen (mach/host_info.h):
     * kern_return_t host_statistics64(
     *     host_t host_priv,
     *     host_flavor_t flavor,
     *     host_info64_t host_info64_out,
     *     mach_msg_type_number_t *host_info64_outCnt
     * );
     * 
     * Parameter:
     * - host_priv: Ein Port auf den Host, für den Statistiken abgefragt werden sollen (meist mach_host_self()).
     * - flavor: Der Typ der Statistiken, die abgefragt werden sollen. Mögliche Werte sind u.a.:
     *     - HOST_VM_INFO64 (4): Informationen über den virtuellen Speicher.
     *     - HOST_CPU_LOAD_INFO (3): Informationen über die CPU-Auslastung (Ticks).
     *     - HOST_EXPIRED_TASKS_INFO (5): Informationen über abgelaufene Tasks.
     * - host_info64_out: Ein Zeiger auf einen Puffer (Struktur), in dem die Daten gespeichert werden.
     * - host_info64_outCnt: Ein Zeiger auf eine Ganzzahl, die die Größe des Puffers in mach_msg_type_number_t Einheiten angibt.
     * 
     * Rückgabewert:
     * - KERN_SUCCESS (0) bei Erfolg.
     * - Andere Werte weisen auf Fehler hin (siehe mach/kern_return.h).
     * 
     * Die Struktur für HOST_CPU_LOAD_INFO ist host_cpu_load_info, die 4 Ticks enthält:
     * - CPU_STATE_USER (0)
     * - CPU_STATE_SYSTEM (1)
     * - CPU_STATE_IDLE (2)
     * - CPU_STATE_NICE (3)
     */
    private const int HOST_CPU_LOAD_INFO = 3;
    private const int HOST_CPU_LOAD_INFO_COUNT = 4;

    [DllImport("libSystem.dylib")]
    private static extern int host_statistics64(
        IntPtr host,
        int flavor,
        IntPtr data,
        ref int count);

    /*
     * mach_host_self (macOS Mach Kernel API)
     * -------------------------------------
     * Dokumentation:
     * Die Funktion mach_host_self() gibt den Sende-Port für den Host-Port des aktuellen Hosts zurück.
     * Dieser Port wird für verschiedene Host-Abfragen verwendet, wie z.B. host_statistics64.
     * 
     * Header-Dateien:
     * Die Definition befindet sich typischerweise in:
     * /Library/Developer/CommandLineTools/SDKs/MacOSX.sdk/usr/include/mach/mach_init.h
     * 
     * Definition in den Apple XNU-Sourcen (mach/mach_init.h):
     * mach_port_t mach_host_self(void);
     * 
     * Rückgabewert:
     * - Gibt einen Mach-Port (mach_port_t) zurück, der den Host repräsentiert.
     * - Dieser Port sollte normalerweise nach Gebrauch nicht freigegeben (deallocated) werden, 
     *   da es sich um einen speziellen Port handelt, den der Kernel verwaltet.
     * 
     * Man-Pages:
     * - man mach_host_self
     */
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
                    _logger.LogWarning(ex, $"Fehler beim Lesen der Laufwerk-Informationen für {drive.Name}");
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
                    _logger.LogWarning(ex, $"Fehler beim Lesen der Netzwerk-Informationen für {networkInterface.Name}");
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

            if (process.Start())
            {
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
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "wmic konnte nicht gestartet werden, versuche PowerShell");
        }

        // Fallback via PowerShell SDK
        try
        {
            using var ps = PowerShell.Create();
            ps.AddScript("(Get-CimInstance Win32_Processor).Name");
            var results = ps.Invoke();
            if (results.Count > 0 && results[0] != null)
            {
                return results[0].ToString() ?? "Unknown";
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Windows-Prozessor-Name konnte auch via PowerShell nicht ermittelt werden");
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
            // Windows Performance Counter verwenden
            if (_cpuCounter != null)
            {
                return Math.Round(Math.Clamp(_cpuCounter.NextValue(), 0.0, 100.0),2);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return GetOsxCpuUsage();
            }
            else
            {
                return GetUnixCpuUsage();
            }
            #endregion

            #region GetCpuUsagePerUniverseCpuUsage
            return GetUniverseCpuUsage();
            #endregion
            #region GetEnvirionmentCpuUsage
            return GetEnvironmentCpuUsage();
            #endregion
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CPU-Auslastung konnte nicht ermittelt werden");
            return 0;
        }
    }

    private double GetEnvironmentCpuUsage()
    {
        try
        {
            var current = Environment.CpuUsage; // kumulative Zeiten: User/Kernel/Idle
            var now = DateTime.UtcNow;

            if (_lastEnvCpuUsage == null || _lastEnvCpuCheck == null)
            {
                _lastEnvCpuUsage = current;
                _lastEnvCpuCheck = now;

                Thread.Sleep(250); // kurzer Abstand für ein sinnvolles Delta
                current = Environment.CpuUsage;
                now = DateTime.UtcNow;
            }

            var last = _lastEnvCpuUsage.Value;

            var deltaUserTime = (current.UserTime - last.UserTime).TotalSeconds;
            var deltaTotalTime = (current.TotalTime - last.TotalTime).TotalSeconds;
            _lastEnvCpuUsage = current;
            _lastEnvCpuCheck = now;

            var deltaTotal = deltaUserTime + deltaTotalTime;
            if (deltaTotal <= 0) return 0.0;

            var usage = deltaUserTime / deltaTotal * 100.0;

            // Schutz gegen Rundungs-/Messartefakte
            usage = Math.Clamp(usage, 0.0, 100.0);
            return Math.Round(usage, 2);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CPU-Auslastung konnte nicht mit Environment.CpuUsage ermittelt werden");
            return 0.0;
        }
    }

    private double GetUniverseCpuUsage()
    {
        var cpuUsage = CpuUsage.GetByProcess();
        if (cpuUsage != null)
        {
            if (_lastLibraryCpuUsage == null || _lastLibraryCpuCheck == null)
            {
                _lastLibraryCpuUsage = cpuUsage;
                _lastLibraryCpuCheck = DateTime.Now;

                Thread.Sleep(100);
                cpuUsage = CpuUsage.GetByProcess();
                if (cpuUsage == null) return 0;
            }

            var currentUsage = cpuUsage.Value;
            var lastUsage = _lastLibraryCpuUsage.Value;
            var currentTime = DateTime.Now;
            var elapsed = currentTime - _lastLibraryCpuCheck.Value;

            double totalSeconds = (currentUsage.UserUsage.TotalSeconds + currentUsage.KernelUsage.TotalSeconds) -
                                 (lastUsage.UserUsage.TotalSeconds + lastUsage.KernelUsage.TotalSeconds);

            _lastLibraryCpuUsage = currentUsage;
            _lastLibraryCpuCheck = currentTime;

            if (elapsed.TotalSeconds <= 0) return 0;

            // Berechnung: (Verbrauchte CPU-Zeit / (Vergangene Zeit * Anzahl Kerne)) * 100
            double usage = (totalSeconds / (elapsed.TotalSeconds * Environment.ProcessorCount)) * 100.0;
            usage = Math.Clamp(usage, 0.0, 100.0);
            return Math.Round(usage, 2);
        }

        return 0;
    }


    /*
     * Klassischer C-Code für den Aufruf von host_statistics64:
     * ------------------------------------------------------
     * #include <mach/mach_host.h>
     * #include <mach/host_info.h>
     * #include <stdio.h>
     *
     * double get_osx_cpu_usage() {
     *     host_t host = mach_host_self();
     *     host_cpu_load_info_data_t load_info;
     *     mach_msg_type_number_t count = HOST_CPU_LOAD_INFO_COUNT;
     *
     *     kern_return_t kr = host_statistics64(host, HOST_CPU_LOAD_INFO, (host_info64_t)&load_info, &count);
     *     if (kr != KERN_SUCCESS) return -1.0;
     *
     *     unsigned long long user = load_info.cpu_ticks[CPU_STATE_USER];
     *     unsigned long long system = load_info.cpu_ticks[CPU_STATE_SYSTEM];
     *     unsigned long long idle = load_info.cpu_ticks[CPU_STATE_IDLE];
     *     unsigned long long nice = load_info.cpu_ticks[CPU_STATE_NICE];
     *
     *     unsigned long long total = user + system + idle + nice;
     *     if (total == 0) return 0.0;
     *
     *     return (double)(total - idle) / total * 100.0;
     * }
     *
     * Die Marshal-Klasse wird hier verwendet, um mit nicht verwaltetem (unmanaged) Speicher zu interagieren.
     * In diesem Kontext:
     * - Marshal.AllocHGlobal: Reserviert einen Speicherblock im unmanaged Speicher (ähnlich wie malloc in C).
     * - Marshal.ReadInt64: Liest einen 64-Bit-Ganzzahlwert (ulong) aus dem unmanaged Speicher an einem bestimmten Offset.
     * - Marshal.FreeHGlobal: Gibt den zuvor reservierten Speicherblock wieder frei (ähnlich wie free in C).
     * 
     */
        private double GetOsxCpuUsage()
        {
            IntPtr host = mach_host_self();
            int count = HOST_CPU_LOAD_INFO_COUNT;

            // host_cpu_load_info liefert natural_t[4] => i.d.R. 4 * 4 Bytes
            IntPtr ptr = Marshal.AllocHGlobal(sizeof(uint) * count);
            try
            {
                int result = host_statistics64(host, HOST_CPU_LOAD_INFO, ptr, ref count);
                if (result != 0) return -1;

                ulong user = (uint)Marshal.ReadInt32(ptr, 0);
                ulong system = (uint)Marshal.ReadInt32(ptr, 4);
                ulong idle = (uint)Marshal.ReadInt32(ptr, 8);
                ulong nice = (uint)Marshal.ReadInt32(ptr, 12);

                if (_lastCpuCheck == null)
                {
                    _lastOsxUser = user;
                    _lastOsxSystem = system;
                    _lastOsxIdle = idle;
                    _lastOsxNice = nice;
                    _lastCpuCheck = DateTime.Now;

                    Thread.Sleep(250); // etwas länger -> stabilere Deltas

                    result = host_statistics64(host, HOST_CPU_LOAD_INFO, ptr, ref count);
                    if (result != 0) return -1;

                    user = (uint)Marshal.ReadInt32(ptr, 0);
                    system = (uint)Marshal.ReadInt32(ptr, 4);
                    idle = (uint)Marshal.ReadInt32(ptr, 8);
                    nice = (uint)Marshal.ReadInt32(ptr, 12);
                }

                ulong diffUser = user - _lastOsxUser;
                ulong diffSystem = system - _lastOsxSystem;
                ulong diffIdle = idle - _lastOsxIdle;
                ulong diffNice = nice - _lastOsxNice;

                _lastOsxUser = user;
                _lastOsxSystem = system;
                _lastOsxIdle = idle;
                _lastOsxNice = nice;
                _lastCpuCheck = DateTime.Now;

                ulong totalDiff = diffUser + diffSystem + diffIdle + diffNice;
                if (totalDiff == 0) return 0.0;

                ulong busyDiff = totalDiff - diffIdle;

                double usage = (double)busyDiff / totalDiff * 100.0;
                usage = Math.Clamp(usage, 0.0, 100.0);
                return Math.Round(usage, 2);
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
                var cpuLine = lines.FirstOrDefault(l => l.StartsWith("cpu "));
                if (string.IsNullOrEmpty(cpuLine)) return 0.0;

                var parts = cpuLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 8) return 0.0;

                // /proc/stat cpu line: user nice system idle iowait irq softirq steal guest guest_nice
                ulong user = ulong.Parse(parts[1]);
                ulong nice = ulong.Parse(parts[2]);
                ulong system = ulong.Parse(parts[3]);
                ulong idle = ulong.Parse(parts[4]);
                ulong iowait = ulong.Parse(parts[5]);
                ulong irq = ulong.Parse(parts[6]);
                ulong softirq = ulong.Parse(parts[7]);

                if (!_unixCpuInitialized)
                {
                    _lastUnixUser = user;
                    _lastUnixNice = nice;
                    _lastUnixSystem = system;
                    _lastUnixIdle = idle;
                    _lastUnixIowait = iowait;
                    _lastUnixIrq = irq;
                    _lastUnixSoftirq = softirq;
                    _unixCpuInitialized = true;

                    Thread.Sleep(100);
                    return GetUnixCpuUsage(); // Rekursiv aufrufen nach Initialisierung
                }

                ulong diffUser = user - _lastUnixUser;
                ulong diffNice = nice - _lastUnixNice;
                ulong diffSystem = system - _lastUnixSystem;
                ulong diffIdle = idle - _lastUnixIdle;
                ulong diffIowait = iowait - _lastUnixIowait;
                ulong diffIrq = irq - _lastUnixIrq;
                ulong diffSoftirq = softirq - _lastUnixSoftirq;

                _lastUnixUser = user;
                _lastUnixNice = nice;
                _lastUnixSystem = system;
                _lastUnixIdle = idle;
                _lastUnixIowait = iowait;
                _lastUnixIrq = irq;
                _lastUnixSoftirq = softirq;

                ulong totalDiff = diffUser + diffNice + diffSystem + diffIdle + diffIowait + diffIrq + diffSoftirq;
                ulong idleDiff = diffIdle + diffIowait;

                if (totalDiff == 0) return 0.0;

                // Berechnung: (Verbrauchte CPU-Zeit / (Vergangene Zeit * Anzahl Kerne)) * 100
                double usage = (double)(totalDiff - idleDiff) / totalDiff * 100.0;
                usage = Math.Clamp(usage, 0.0, 100.0);
                return Math.Round(usage, 2);
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