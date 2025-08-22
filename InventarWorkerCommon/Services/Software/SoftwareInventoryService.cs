using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using InventarWorkerCommon.Models.Software;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace InventarWorkerService.Services.Software;

public class SoftwareInventoryService
{
    private readonly ILogger<SoftwareInventoryService> _logger;

    public SoftwareInventoryService(ILogger<SoftwareInventoryService> logger)
    {
        _logger = logger;
    }

    public async Task<SoftwareInventory> CollectSoftwareInventoryAsync()
    {
        _logger.LogInformation("Sammle Software-Inventar...");

        var softwareInventory = new SoftwareInventory();

        try
        {
            // Parallel sammeln für bessere Performance
            var tasks = new Task[]
            {
                Task.Run(() => softwareInventory.InstalledSoftware = CollectInstalledSoftware()),
                Task.Run(() => softwareInventory.RunningProcesses = CollectRunningProcesses()),
                Task.Run(() => softwareInventory.WindowsServices = CollectWindowsServices()),
                Task.Run(() => softwareInventory.StartupPrograms = CollectStartupPrograms()),
                Task.Run(() => softwareInventory.EnvironmentVariables = CollectEnvironmentVariables()),
                Task.Run(() => softwareInventory.Runtime = CollectRuntimeInfo())
            };

            await Task.WhenAll(tasks);
            
            _logger.LogInformation("Software-Inventar erfolgreich gesammelt");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln des Software-Inventars");
        }

        return softwareInventory;
    }

    private List<SoftwareInfo> CollectInstalledSoftware()
    {
        var software = new List<SoftwareInfo>();

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                software.AddRange(CollectWindowsInstalledSoftware());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                software.AddRange(CollectLinuxInstalledSoftware());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                software.AddRange(CollectMacOSInstalledSoftware());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der installierten Software");
        }

        return software;
    }

    private List<SoftwareInfo> CollectWindowsInstalledSoftware()
    {
        var software = new List<SoftwareInfo>();

        try
        {
            // Registry-Pfade für installierte Software
            var registryPaths = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var registryPath in registryPaths)
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(registryPath);
                    if (key == null) continue;

                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        try
                        {
                            using var subKey = key.OpenSubKey(subKeyName);
                            if (subKey == null) continue;

                            var displayName = subKey.GetValue("DisplayName")?.ToString();
                            if (string.IsNullOrWhiteSpace(displayName)) continue;

                            var softwareInfo = new SoftwareInfo
                            {
                                Name = displayName,
                                Version = subKey.GetValue("DisplayVersion")?.ToString() ?? string.Empty,
                                Publisher = subKey.GetValue("Publisher")?.ToString() ?? string.Empty,
                                InstallLocation = subKey.GetValue("InstallLocation")?.ToString() ?? string.Empty,
                                UninstallString = subKey.GetValue("UninstallString")?.ToString() ?? string.Empty,
                                DisplayIcon = subKey.GetValue("DisplayIcon")?.ToString() ?? string.Empty,
                                IsSystemComponent = subKey.GetValue("SystemComponent")?.ToString() == "1",
                                Architecture = registryPath.Contains("WOW6432Node") ? "x86" : "x64"
                            };

                            // Installationsdatum parsen
                            var installDateString = subKey.GetValue("InstallDate")?.ToString();
                            if (!string.IsNullOrEmpty(installDateString) && installDateString.Length == 8)
                            {
                                if (DateTime.TryParseExact(installDateString, "yyyyMMdd", null, 
                                    System.Globalization.DateTimeStyles.None, out var installDate))
                                {
                                    softwareInfo.InstallDate = installDate;
                                }
                            }

                            // Größe parsen
                            var sizeString = subKey.GetValue("EstimatedSize")?.ToString();
                            if (!string.IsNullOrEmpty(sizeString) && long.TryParse(sizeString, out var size))
                            {
                                softwareInfo.Size = size * 1024; // KB zu Bytes
                            }

                            software.Add(softwareInfo);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Fehler beim Lesen der Software-Information für {SubKey}", subKeyName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Fehler beim Zugriff auf Registry-Pfad {RegistryPath}", registryPath);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Windows-Software");
        }

        return software;
    }

    private List<SoftwareInfo> CollectLinuxInstalledSoftware()
    {
        var software = new List<SoftwareInfo>();

        try
        {
            // Verschiedene Paketmanager ausprobieren
            var packageManagers = new[]
            {
                ("dpkg", "dpkg -l"),
                ("rpm", "rpm -qa"),
                ("pacman", "pacman -Q"),
                ("zypper", "zypper search --installed-only")
            };

            foreach (var (manager, command) in packageManagers)
            {
                try
                {
                    var packages = ExecuteCommand(command);
                    if (!string.IsNullOrEmpty(packages))
                    {
                        _logger.LogInformation("Gefundener Paketmanager: {Manager}", manager);
                        software.AddRange(ParseLinuxPackages(packages, manager));
                        break; // Ersten funktionierenden Paketmanager verwenden
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Paketmanager {Manager} nicht verfügbar", manager);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Linux-Software");
        }

        return software;
    }

    private List<SoftwareInfo> CollectMacOSInstalledSoftware()
    {
        var software = new List<SoftwareInfo>();

        try
        {
            // macOS-Anwendungen in /Applications sammeln
            var applicationsPath = "/Applications";
            if (Directory.Exists(applicationsPath))
            {
                var appDirectories = Directory.GetDirectories(applicationsPath, "*.app");
                
                foreach (var appDir in appDirectories)
                {
                    try
                    {
                        var appName = Path.GetFileNameWithoutExtension(appDir);
                        var infoPlistPath = Path.Combine(appDir, "Contents", "Info.plist");
                        
                        var softwareInfo = new SoftwareInfo
                        {
                            Name = appName,
                            InstallLocation = appDir
                        };

                        // Info.plist auslesen falls vorhanden
                        if (File.Exists(infoPlistPath))
                        {
                            var plistContent = File.ReadAllText(infoPlistPath);
                            // Vereinfachte plist-Parsing (für Produktiveinsatz sollte eine plist-Bibliothek verwendet werden)
                            ExtractMacOSAppInfo(plistContent, softwareInfo);
                        }

                        software.Add(softwareInfo);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Fehler beim Lesen der App-Information für {AppDir}", appDir);
                    }
                }
            }

            // Homebrew-Pakete
            try
            {
                var brewOutput = ExecuteCommand("brew list --formula");
                if (!string.IsNullOrEmpty(brewOutput))
                {
                    var brewPackages = brewOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var package in brewPackages)
                    {
                        software.Add(new SoftwareInfo
                        {
                            Name = package.Trim(),
                            Publisher = "Homebrew"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Homebrew nicht verfügbar oder Fehler beim Auslesen");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der macOS-Software");
        }

        return software;
    }

    private List<ProcessInfo> CollectRunningProcesses()
    {
        var processes = new List<ProcessInfo>();

        try
        {
            var runningProcesses = Process.GetProcesses();

            foreach (var process in runningProcesses)
            {
                try
                {
                    var processInfo = new ProcessInfo
                    {
                        ProcessName = process.ProcessName,
                        ProcessId = process.Id,
                        MainWindowTitle = process.MainWindowTitle,
                        IsResponding = process.Responding,
                        ThreadCount = process.Threads.Count
                    };

                    // Zusätzliche Informationen sammeln (können Exceptions werfen)
                    try
                    {
                        processInfo.StartTime = process.StartTime;
                        processInfo.WorkingSet = process.WorkingSet64;
                        processInfo.VirtualMemorySize = process.VirtualMemorySize64;
                        processInfo.TotalProcessorTime = process.TotalProcessorTime;

                        if (process.MainModule != null)
                        {
                            processInfo.FileName = process.MainModule.FileName;
                            var versionInfo = process.MainModule.FileVersionInfo;
                            processInfo.FileVersion = versionInfo.FileVersion ?? string.Empty;
                            processInfo.ProductVersion = versionInfo.ProductVersion ?? string.Empty;
                            processInfo.Company = versionInfo.CompanyName ?? string.Empty;
                            processInfo.Description = versionInfo.FileDescription ?? string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "Fehler beim Sammeln erweiterter Process-Informationen für {ProcessName}", process.ProcessName);
                    }

                    processes.Add(processInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Fehler beim Sammeln der Process-Information für PID {ProcessId}", process.Id);
                }
                finally
                {
                    process.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der laufenden Prozesse");
        }

        return processes;
    }

    private List<ServiceInfo> CollectWindowsServices()
    {
        var services = new List<ServiceInfo>();

        try
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return services;
            }

            var windowsServices = ServiceController.GetServices();

            foreach (var service in windowsServices)
            {
                try
                {
                    var serviceInfo = new ServiceInfo
                    {
                        ServiceName = service.ServiceName,
                        DisplayName = service.DisplayName,
                        Status = service.Status.ToString(),
                        CanStop = service.CanStop,
                        CanPauseAndContinue = service.CanPauseAndContinue
                    };

                    // Starttyp über Registry ermitteln
                    try
                    {
                        var servicePath = $@"SYSTEM\CurrentControlSet\Services\{service.ServiceName}";
                        using var key = Registry.LocalMachine.OpenSubKey(servicePath);
                        if (key != null)
                        {
                            var startValue = key.GetValue("Start");
                            if (startValue != null)
                            {
                                serviceInfo.StartType = ConvertStartType((int)startValue);
                            }

                            serviceInfo.BinaryPath = key.GetValue("ImagePath")?.ToString() ?? string.Empty;
                            serviceInfo.Description = key.GetValue("Description")?.ToString() ?? string.Empty;
                            serviceInfo.ServiceAccount = key.GetValue("ObjectName")?.ToString() ?? string.Empty;

                            // Abhängigkeiten
                            var dependOnService = key.GetValue("DependOnService") as string[];
                            if (dependOnService != null)
                            {
                                serviceInfo.Dependencies.AddRange(dependOnService);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "Fehler beim Lesen der Registry-Informationen für Service {ServiceName}", service.ServiceName);
                    }

                    services.Add(serviceInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Fehler beim Sammeln der Service-Information für {ServiceName}", service.ServiceName);
                }
                finally
                {
                    service.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Windows-Services");
        }

        return services;
    }

    private List<string> CollectStartupPrograms()
    {
        var startupPrograms = new List<string>();

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var registryPaths = new[]
                {
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                    @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run"
                };

                foreach (var registryPath in registryPaths)
                {
                    try
                    {
                        using var key = Registry.LocalMachine.OpenSubKey(registryPath);
                        if (key != null)
                        {
                            foreach (var valueName in key.GetValueNames())
                            {
                                var value = key.GetValue(valueName)?.ToString();
                                if (!string.IsNullOrWhiteSpace(value))
                                {
                                    startupPrograms.Add($"{valueName}: {value}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Fehler beim Lesen des Registry-Pfads {RegistryPath}", registryPath);
                    }
                }

                // Benutzer-spezifische Startup-Programme
                try
                {
                    using var userKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    if (userKey != null)
                    {
                        foreach (var valueName in userKey.GetValueNames())
                        {
                            var value = userKey.GetValue(valueName)?.ToString();
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                startupPrograms.Add($"[User] {valueName}: {value}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Fehler beim Lesen der Benutzer-Startup-Programme");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Startup-Programme");
        }

        return startupPrograms;
    }

    private List<string> CollectEnvironmentVariables()
    {
        var environmentVariables = new List<string>();

        try
        {
            var variables = Environment.GetEnvironmentVariables();
            
            foreach (var key in variables.Keys.Cast<string>().OrderBy(k => k))
            {
                var value = variables[key]?.ToString() ?? string.Empty;
                environmentVariables.Add($"{key}={value}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Umgebungsvariablen");
        }

        return environmentVariables;
    }

    private RuntimeInfo CollectRuntimeInfo()
    {
        var runtimeInfo = new RuntimeInfo();

        try
        {
            // .NET Version
            runtimeInfo.DotNetVersion = Environment.Version.ToString();

            // Installierte .NET Frameworks
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                runtimeInfo.InstalledFrameworks.AddRange(GetInstalledDotNetFrameworks());
            }

            // PowerShell Version
            try
            {
                var psVersion = ExecuteCommand("powershell -Command \"$PSVersionTable.PSVersion.ToString()\"");
                if (!string.IsNullOrWhiteSpace(psVersion))
                {
                    runtimeInfo.PowerShellVersion = psVersion.Trim();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "PowerShell-Version konnte nicht ermittelt werden");
            }

            // Verfügbare PowerShell-Module
            try
            {
                var modules = ExecuteCommand("powershell -Command \"Get-Module -ListAvailable | Select-Object -ExpandProperty Name\"");
                if (!string.IsNullOrWhiteSpace(modules))
                {
                    runtimeInfo.AvailableModules.AddRange(
                        modules.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                               .Select(m => m.Trim())
                               .Where(m => !string.IsNullOrWhiteSpace(m))
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "PowerShell-Module konnten nicht ermittelt werden");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Runtime-Informationen");
        }

        return runtimeInfo;
    }

    #region Helper Methods

    private string ExecuteCommand(string command)
    {
        try
        {
            var parts = command.Split(' ', 2);
            var fileName = parts[0];
            var arguments = parts.Length > 1 ? parts[1] : string.Empty;

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(5000); // 5 Sekunden Timeout

            return output;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Fehler beim Ausführen des Kommandos: {Command}", command);
            return string.Empty;
        }
    }

    private List<SoftwareInfo> ParseLinuxPackages(string packages, string packageManager)
    {
        var software = new List<SoftwareInfo>();

        try
        {
            var lines = packages.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                try
                {
                    var softwareInfo = packageManager switch
                    {
                        "dpkg" => ParseDpkgLine(line),
                        "rpm" => ParseRpmLine(line),
                        "pacman" => ParsePacmanLine(line),
                        _ => null
                    };

                    if (softwareInfo != null)
                    {
                        software.Add(softwareInfo);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Fehler beim Parsen der Paket-Zeile: {Line}", line);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Parsen der Linux-Pakete");
        }

        return software;
    }

    private SoftwareInfo? ParseDpkgLine(string line)
    {
        // dpkg -l Format: ii  package-name  version  architecture  description
        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 4 && parts[0] == "ii")
        {
            return new SoftwareInfo
            {
                Name = parts[1],
                Version = parts[2],
                Architecture = parts[3],
                Publisher = "Debian Package"
            };
        }
        return null;
    }

    private SoftwareInfo? ParseRpmLine(string line)
    {
        // rpm -qa Format: package-name-version-release.architecture
        var match = System.Text.RegularExpressions.Regex.Match(line, @"^(.+)-([^-]+)-([^-]+)\.(.+)$");
        if (match.Success)
        {
            return new SoftwareInfo
            {
                Name = match.Groups[1].Value,
                Version = $"{match.Groups[2].Value}-{match.Groups[3].Value}",
                Architecture = match.Groups[4].Value,
                Publisher = "RPM Package"
            };
        }
        return null;
    }

    private SoftwareInfo? ParsePacmanLine(string line)
    {
        // pacman -Q Format: package-name version
        var parts = line.Split(' ', 2);
        if (parts.Length == 2)
        {
            return new SoftwareInfo
            {
                Name = parts[0],
                Version = parts[1],
                Publisher = "Arch Package"
            };
        }
        return null;
    }

    private void ExtractMacOSAppInfo(string plistContent, SoftwareInfo softwareInfo)
    {
        try
        {
            // Vereinfachte plist-Parsing
            var versionMatch = System.Text.RegularExpressions.Regex.Match(plistContent, @"<key>CFBundleShortVersionString</key>\s*<string>([^<]+)</string>");
            if (versionMatch.Success)
            {
                softwareInfo.Version = versionMatch.Groups[1].Value;
            }

            var publisherMatch = System.Text.RegularExpressions.Regex.Match(plistContent, @"<key>CFBundleIdentifier</key>\s*<string>([^<]+)</string>");
            if (publisherMatch.Success)
            {
                var bundleId = publisherMatch.Groups[1].Value;
                var parts = bundleId.Split('.');
                if (parts.Length > 1)
                {
                    softwareInfo.Publisher = parts[1]; // z.B. "com.apple.Safari" -> "apple"
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Fehler beim Parsen der plist-Datei");
        }
    }

    private string ConvertStartType(int startType)
    {
        return startType switch
        {
            0 => "Boot",
            1 => "System",
            2 => "Automatic",
            3 => "Manual",
            4 => "Disabled",
            _ => "Unknown"
        };
    }

    private List<string> GetInstalledDotNetFrameworks()
    {
        var frameworks = new List<string>();

        try
        {
            // .NET Framework über Registry
            using var ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\");
            if (ndpKey != null)
            {
                foreach (var versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        using var versionKey = ndpKey.OpenSubKey(versionKeyName);
                        if (versionKey != null)
                        {
                            var version = versionKey.GetValue("Version")?.ToString();
                            if (!string.IsNullOrEmpty(version))
                            {
                                frameworks.Add($".NET Framework {version}");
                            }
                        }
                    }
                }
            }

            // .NET Core/.NET 5+ über dotnet CLI
            try
            {
                var dotnetInfo = ExecuteCommand("dotnet --list-runtimes");
                if (!string.IsNullOrWhiteSpace(dotnetInfo))
                {
                    var lines = dotnetInfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            frameworks.Add(line.Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "dotnet CLI nicht verfügbar");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Ermitteln der .NET Frameworks");
        }

        return frameworks;
    }

    #endregion

    public async Task<object?> GetSoftwareInfoAsync()
    {
        try
        {
            _logger.LogInformation("Sammle Software-Informationen...");
        
            var softwareInventory = await CollectSoftwareInventoryAsync();
        
            _logger.LogInformation("Software-Informationen erfolgreich gesammelt");
        
            return softwareInventory;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Sammeln der Software-Informationen");
            return null;
        }
    }
}