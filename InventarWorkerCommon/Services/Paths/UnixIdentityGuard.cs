using System.Runtime.InteropServices;

namespace InventarWorkerCommon.Services.Paths;

/// <summary>
/// DE: Prüft auf Unix die Eigentümerschaft von privaten Dienstzustandsobjekten ohne Shell-Aufrufe.
/// EN: Verifies Unix ownership of private service-state objects without shell invocations.
/// </summary>
internal static class UnixIdentityGuard
{
    [DllImport("libc", SetLastError = true)]
    private static extern uint getuid();

    [DllImport("libc", EntryPoint = "lstat", SetLastError = true)]
    private static extern int LStat(string path, IntPtr buffer);

    /// <summary>
    /// DE: Stellt sicher, dass der Pfad dem aktuellen Unix-Dienstkonto gehört.
    /// EN: Ensures that the path belongs to the current Unix service account.
    /// </summary>
    /// <param name="path">DE: Vorher als nicht verlinkt geprüfter Pfad. EN: Previously checked non-link path.</param>
    /// <exception cref="UnauthorizedAccessException">DE: Der Eigentümer unterscheidet sich vom Dienstkonto. EN: The owner differs from the service account.</exception>
    internal static void EnsureOwnedByCurrentUser(string path)
    {
        var expectedUid = getuid();
        const int statBufferSize = 512;
        var buffer = Marshal.AllocHGlobal(statBufferSize);
        try
        {
            if (LStat(path, buffer) != 0)
            {
                throw new IOException($"DE: Der Eigentümer von '{path}' konnte nicht geprüft werden. EN: Could not verify owner of '{path}'.", new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()));
            }

            // DE: Die POSIX-struktur unterscheidet sich zwischen Darwin und Linux. Der
            // Puffer vermeidet eine ABI-abhängige verwaltete Gesamtstruktur; nur st_uid wird gelesen.
            // EN: The POSIX structure differs between Darwin and Linux. The buffer avoids an ABI-
            // dependent managed full struct; only st_uid is read.
            var userIdOffset = OperatingSystem.IsMacOS() ? 16 : 28;
            var actualUid = unchecked((uint)Marshal.ReadInt32(buffer, userIdOffset));

            if (actualUid != expectedUid)
            {
                throw new UnauthorizedAccessException(
                    "DE: Das Dienstzustandsobjekt gehört nicht dem aktuellen Dienstkonto. EN: The service-state object is not owned by the current service account.");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// DE: Prüft mit <c>lstat</c>, ob auch ein hängender Unix-Pfad ein symbolischer Link ist.
    /// EN: Uses <c>lstat</c> to determine whether even a dangling Unix path is a symbolic link.
    /// </summary>
    /// <param name="path">DE: Zu prüfender Pfad. EN: Path to inspect.</param>
    /// <returns>DE: <c>true</c> bei symbolischem Link. EN: <c>true</c> for a symbolic link.</returns>
    internal static bool IsSymbolicLink(string path)
    {
        const int statBufferSize = 512;
        var buffer = Marshal.AllocHGlobal(statBufferSize);
        try
        {
            if (LStat(path, buffer) != 0)
            {
                return false;
            }

            var modeOffset = OperatingSystem.IsMacOS() ? 4 : 24;
            var mode = unchecked((uint)Marshal.ReadInt32(buffer, modeOffset));
            return (mode & 0xF000) == 0xA000;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
