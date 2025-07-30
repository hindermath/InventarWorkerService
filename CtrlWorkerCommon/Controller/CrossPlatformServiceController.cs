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

    /// <summary>
    /// Starts the service for the specified operating system.
    /// </summary>
    /// <remarks>
    /// This method checks the operating system at runtime and attempts to
    /// start the service using platform-specific logic. Supported platforms include
    /// Windows, Linux, macOS, and FreeBSD. If the current operating system is not
    /// supported, a <see cref="PlatformNotSupportedException"/> will be thrown.
    /// </remarks>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown when the method is executed on an unsupported platform.
    /// </exception>
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

    /// <summary>
    /// Starts the service for the Linux operating system.
    /// </summary>
    /// <remarks>
    /// This method executes the Linux system command to start the service
    /// associated with the specified service name. It utilizes the `systemctl`
    /// command to initiate the service. The service name is converted to lowercase
    /// to ensure compatibility with Linux's case-sensitive file and service naming system.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the execution of the `systemctl` command fails, or if an error occurs
    /// during the command execution process.
    /// </exception>
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

    /// <summary>
    /// Executes a system command with the specified filename and arguments.
    /// </summary>
    /// <param name="fileName">The name of the executable file or command to run.</param>
    /// <param name="arguments">The arguments to pass to the command.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the command execution fails or if the process exits with a non-zero code.
    /// </exception>
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