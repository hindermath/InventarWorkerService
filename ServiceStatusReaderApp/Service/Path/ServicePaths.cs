using System.Runtime.InteropServices;

namespace ServiceStatusReaderApp.Service.Path;

public static class ServicePaths
{
    public static string GetStatusDirectory()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
            //     "InventarWorkerService");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "/var/lib/inventar-worker-service";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "/usr/local/var/inventar-worker-service";
        }
        
        return "/tmp/inventar-worker-service";
    }
}