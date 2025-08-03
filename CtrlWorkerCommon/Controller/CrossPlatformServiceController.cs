using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CtrlWorkerCommon.Controller;

/// <summary>
/// Provides cross-platform service control functionality, allowing
/// services to be started and stopped on supported operating systems.
/// </summary>
/// <remarks>
/// This class includes platform-specific implementations for managing
/// services on Windows, Linux, macOS, and FreeBSD. It evaluates the
/// runtime environment to execute the appropriate logic. An exception
/// will be thrown if executed on an unsupported platform.
/// </remarks>
public class CrossPlatformServiceController
{
    /// <summary>
    /// Represents the name of the service to be controlled by the
    /// CrossPlatformServiceController. This variable holds the identifier
    /// used to start, stop, or manage the service across different operating systems.
    /// </summary>
    /// <remarks>
    /// The value of this variable is assigned during the initialization of the
    /// CrossPlatformServiceController and is used in service control operations
    /// such as starting or stopping the service on Windows, Linux, macOS, or FreeBSD.
    /// </remarks>
    private readonly string _serviceName;

    /// <summary>
    /// Provides functionality to manage and control services across multiple operating systems.
    /// </summary>
    /// <remarks>
    /// This class is designed to handle service lifecycle operations such as starting and stopping services
    /// on various platforms. It abstracts platform-specific service management logic, enabling the caller
    /// to interact with services in a cross-platform manner.
    /// </remarks>
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

    /// <summary>
    /// Stops the currently managed service on the platform it is running on.
    /// </summary>
    /// <remarks>
    /// This method determines the operating system and executes the appropriate logic to stop the service.
    /// It supports Windows, Linux, macOS, and FreeBSD platforms. Internally, platform-specific stop operations
    /// are delegated to corresponding methods.
    /// </remarks>
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