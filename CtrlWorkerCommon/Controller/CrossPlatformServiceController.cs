using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CtrlWorkerCommon.Controller;

public class CrossPlatformServiceController
{
    private readonly string _serviceName;
    
    public CrossPlatformServiceController(string serviceName)
    {
        _serviceName = serviceName;
    }
    
    public void StartService()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            StartWindowsService();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            StartLinuxService();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            StartMacOSService();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            StartFreeBSDService();
        }
        else
        {
            throw new PlatformNotSupportedException("Plattform wird nicht unterstützt");
        }
    }
    
    public void StopService()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            StopWindowsService();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            StopLinuxService();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            StopMacOSService();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            StopFreeBSDService();
        }
    }
    
    private void StartWindowsService()
    {
        using var service = new System.ServiceProcess.ServiceController(_serviceName);
        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
        {
            service.Start();
        }
    }
    
    private void StopWindowsService()
    {
        using var service = new System.ServiceProcess.ServiceController(_serviceName);
        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
        {
            service.Stop();
        }
    }
    
    private void StartLinuxService()
    {
        ExecuteCommand("systemctl", $"start {_serviceName.ToLower()}.service");
    }
    
    private void StopLinuxService()
    {
        ExecuteCommand("systemctl", $"stop {_serviceName.ToLower()}.service");
    }
    
    private void StartMacOSService()
    {
        ExecuteCommand("launchctl", $"load {Environment.GetEnvironmentVariable("HOME")}/Library/LaunchAgents/com.{_serviceName.ToLower()}.plist");
    }
    
    private void StopMacOSService()
    {
        ExecuteCommand("launchctl", $"unload {Environment.GetEnvironmentVariable("HOME")}/Library/LaunchAgents/com.{_serviceName.ToLower()}.plist");
    }
    
    private void StartFreeBSDService()
    {
        ExecuteCommand("service", $"{_serviceName.ToLower()} start");
    }
    
    private void StopFreeBSDService()
    {
        ExecuteCommand("service", $"{_serviceName.ToLower()} stop");
    }
    
    private void ExecuteCommand(string fileName, string arguments)
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            
            process?.WaitForExit();
            
            if (process?.ExitCode != 0)
            {
                var error = process.StandardError.ReadToEnd();
                throw new InvalidOperationException($"Befehl fehlgeschlagen: {error}");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Fehler beim Ausführen von '{fileName} {arguments}': {ex.Message}");
        }
    }
}