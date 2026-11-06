using System.Security.AccessControl;
using System.Security.Principal;

namespace InventarWorkerCommon.Services.Paths;

/// <summary>
/// DE: Beschränkt Windows-Dienstzustand auf das aktuelle Dienstkonto und Administratoren.
/// EN: Restricts Windows service state to the current service account and administrators.
/// </summary>
internal static class WindowsStateSecurity
{
    internal static void ProtectDirectory(string path)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var identity = WindowsIdentity.GetCurrent().User
            ?? throw new UnauthorizedAccessException("The Windows service identity has no SID.");
        var administrators = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
        var security = new DirectorySecurity();
        security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
        security.SetOwner(identity);
        security.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl,
            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
        security.AddAccessRule(new FileSystemAccessRule(administrators, FileSystemRights.FullControl,
            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
        new DirectoryInfo(path).SetAccessControl(security);
    }

    internal static void ProtectFile(string path)
    {
        if (!OperatingSystem.IsWindows() || !File.Exists(path))
        {
            return;
        }

        var identity = WindowsIdentity.GetCurrent().User
            ?? throw new UnauthorizedAccessException("The Windows service identity has no SID.");
        var administrators = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
        var security = new FileSecurity();
        security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
        security.SetOwner(identity);
        security.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl, AccessControlType.Allow));
        security.AddAccessRule(new FileSystemAccessRule(administrators, FileSystemRights.FullControl, AccessControlType.Allow));
        new FileInfo(path).SetAccessControl(security);
    }
}
