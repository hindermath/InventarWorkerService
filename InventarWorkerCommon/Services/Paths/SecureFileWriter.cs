using System.Text;

namespace InventarWorkerCommon.Services.Paths;

/// <summary>
/// DE: Stellt begrenzte Pfadprüfung und atomare Dateiersetzung für Statusartefakte bereit.
/// EN: Provides bounded path validation and atomic file replacement for status artifacts.
/// </summary>
public static class SecureFileWriter
{
    private const UnixFileMode OwnerReadWrite = UnixFileMode.UserRead | UnixFileMode.UserWrite;

    /// <summary>
    /// DE: Prüft, dass ein konfiguriertes Unterverzeichnis genau ein sicherer Pfadabschnitt ist.
    /// EN: Verifies that a configured subdirectory is exactly one safe path segment.
    /// </summary>
    /// <param name="directoryName">DE: Zu prüfender Verzeichnisname. EN: Directory name to validate.</param>
    /// <returns>DE: Der geprüfte Name. EN: The validated name.</returns>
    /// <exception cref="ArgumentException">DE: Der Name ist leer, zu lang oder enthält Pfadsyntax. EN: The name is empty, too long, or contains path syntax.</exception>
    public static string ValidateDirectoryName(string directoryName)
    {
        if (string.IsNullOrWhiteSpace(directoryName) ||
            directoryName.Length > 128 ||
            directoryName is "." or ".." ||
            directoryName.IndexOfAny([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar]) >= 0 ||
            directoryName != Path.GetFileName(directoryName))
        {
            throw new ArgumentException(
                "DE: Das Statusverzeichnis muss ein einzelner sicherer Name sein. EN: The status directory must be one safe name.",
                nameof(directoryName));
        }

        return directoryName;
    }

    /// <summary>
    /// DE: Schreibt Text über eine temporäre Datei und ersetzt das Ziel atomar im selben Verzeichnis.
    /// EN: Writes text through a temporary file and atomically replaces the target in the same directory.
    /// </summary>
    /// <param name="destinationPath">DE: Zielpfad im bereits geprüften Verzeichnis. EN: Destination in an already validated directory.</param>
    /// <param name="contents">DE: Zu schreibender Text. EN: Text to write.</param>
    /// <exception cref="IOException">DE: Schreiben oder Ersetzen ist fehlgeschlagen. EN: Writing or replacement failed.</exception>
    public static void WriteAllText(string destinationPath, string contents)
    {
        ServicePath.EnsurePrivateFilePath(destinationPath);
        WriteAllTextCore(destinationPath, contents);
    }

    /// <summary>
    /// DE: Schreibt vertraulichen Text atomar und beschränkt Unix-Dateirechte auf den Eigentümer.
    /// EN: Writes confidential text atomically and restricts Unix file permissions to the owner.
    /// </summary>
    /// <param name="destinationPath">DE: Zielpfad im bereits geprüften Verzeichnis. EN: Destination in an already validated directory.</param>
    /// <param name="contents">DE: Zu schreibender vertraulicher Text. EN: Confidential text to write.</param>
    internal static void WriteAllTextOwnerOnly(string destinationPath, string contents)
    {
        ServicePath.EnsurePrivateFilePath(destinationPath);
        WriteAllTextCore(destinationPath, contents);
    }

    /// <summary>
    /// DE: Liest vertraulichen Text, nachdem vorhandene Unix-Dateirechte am geöffneten Handle auf den Eigentümer beschränkt wurden.
    /// EN: Reads confidential text after restricting existing Unix file permissions on the open handle to the owner.
    /// </summary>
    internal static string ReadAllTextOwnerOnly(string path)
    {
        if (OperatingSystem.IsWindows())
        {
            return File.ReadAllText(path);
        }

        ServicePath.EnsurePrivateFilePath(path);

        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        File.SetUnixFileMode(stream.SafeFileHandle, OwnerReadWrite);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// DE: Liest Text aus einem vorab geprüften Statusartefakt ohne dessen Berechtigungen zu verändern.
    /// EN: Reads text from a prevalidated status artifact without changing its permissions.
    /// </summary>
    /// <param name="path">DE: Pfad zum Statusartefakt. EN: Path to the status artifact.</param>
    /// <returns>DE: Inhalt der Datei. EN: File contents.</returns>
    internal static string ReadAllText(string path)
    {
        ServicePath.EnsurePrivateFilePath(path);
        return File.ReadAllText(path);
    }

    /// <summary>
    /// DE: Migriert vorhandene Settings-Dateien auf Eigentümerrechte und entfernt ausschließlich alte atomare Settings-Temporärdateien.
    /// EN: Migrates existing settings files to owner-only permissions and removes only stale atomic settings temporary files.
    /// </summary>
    internal static void ProtectExistingSettingsArtifacts(string directoryPath)
    {
        ServicePath.EnsurePrivateFilePath(Path.Combine(directoryPath, "settings.json"));

        foreach (var fileName in new[] { "settings.json", "settings.yaml" })
        {
            var settingsPath = Path.Combine(directoryPath, fileName);
            ServicePath.EnsurePrivateFilePath(settingsPath);
            ProtectExistingOwnerOnlyFile(settingsPath);

            foreach (var candidatePath in Directory.EnumerateFiles(directoryPath, $".{fileName}.*.tmp"))
            {
                if (IsAtomicTemporaryFile(candidatePath, fileName))
                {
                    File.Delete(candidatePath);
                }
            }
        }
    }

    /// <summary>
    /// DE: Beschränkt ein vorhandenes Dienstartefakt auf Eigentümerrechte, wenn die Plattform Unix-Rechte unterstützt.
    /// EN: Restricts an existing service artifact to owner permissions when the platform supports Unix permissions.
    /// </summary>
    /// <param name="path">DE: Pfad des vorhandenen Artefakts. EN: Path of the existing artifact.</param>
    public static void ProtectExistingArtifact(string path)
    {
        ServicePath.EnsurePrivateFilePath(path);
        ProtectExistingOwnerOnlyFile(path);
    }

    /// <summary>
    /// DE: Beschränkt eine SQLite-Datenbank und ihre bekannten Nebenartefakte auf Eigentümerrechte.
    /// EN: Restricts a SQLite database and its known sidecar artifacts to owner permissions.
    /// </summary>
    /// <param name="databasePath">DE: Pfad zur SQLite-Hauptdatei. EN: Path to the SQLite main file.</param>
    public static void ProtectSqliteArtifacts(string databasePath)
    {
        foreach (var path in new[] { databasePath, $"{databasePath}-wal", $"{databasePath}-shm", $"{databasePath}-journal" })
        {
            ProtectExistingArtifact(path);
        }
    }

    private static void WriteAllTextCore(string destinationPath, string contents)
    {
        var temporaryPath = CreateTemporaryPath(destinationPath);
        try
        {
            if (!OperatingSystem.IsWindows())
            {
                WriteAllTextOwnerOnlyOnUnix(temporaryPath, contents);
            }
            else
            {
                File.WriteAllText(temporaryPath, contents);
            }

            File.Move(temporaryPath, destinationPath, overwrite: true);
            ProtectExistingOwnerOnlyFile(destinationPath);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }

    private static void WriteAllTextOwnerOnlyOnUnix(string path, string contents)
    {
        if (OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Owner-only Unix file creation is unavailable on Windows.");
        }

        var options = new FileStreamOptions
        {
            Access = FileAccess.Write,
            Mode = FileMode.CreateNew,
            Share = FileShare.None,
            UnixCreateMode = OwnerReadWrite
        };

        using var stream = new FileStream(path, options);

        // DE: Die Erstellungsmaske darf Rechte nur weiter einschränken. Vor dem
        // Schreiben stellen wir dennoch exakt 0600 her, ohne ein Lesefenster zu öffnen.
        // EN: The creation mask may only reduce permissions. Before writing, we still
        // establish exact 0600 without opening a readability window.
        File.SetUnixFileMode(stream.SafeFileHandle, OwnerReadWrite);

        using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        writer.Write(contents);
    }

    private static void ProtectExistingOwnerOnlyFile(string path)
    {
        if (OperatingSystem.IsWindows())
        {
            WindowsStateSecurity.ProtectFile(path);
            return;
        }

        if (!File.Exists(path))
        {
            return;
        }

        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        File.SetUnixFileMode(stream.SafeFileHandle, OwnerReadWrite);
    }

    /// <summary>
    /// DE: Fügt Text nur an eine vorab geprüfte Statusdatei an.
    /// EN: Appends text only to a prevalidated status file.
    /// </summary>
    /// <param name="destinationPath">DE: Zielpfad im geschützten Dienstzustand. EN: Destination in protected service state.</param>
    /// <param name="contents">DE: Anzuhängender Text. EN: Text to append.</param>
    internal static void AppendAllText(string destinationPath, string contents)
    {
        ServicePath.EnsurePrivateFilePath(destinationPath);
        if (OperatingSystem.IsWindows())
        {
            ProtectExistingOwnerOnlyFile(destinationPath);
            File.AppendAllText(destinationPath, contents);
            ProtectExistingOwnerOnlyFile(destinationPath);
            return;
        }

        var options = new FileStreamOptions
        {
            Access = FileAccess.Write,
            Mode = FileMode.Append,
            Share = FileShare.Read,
            UnixCreateMode = OwnerReadWrite
        };
        using var stream = new FileStream(destinationPath, options);
        File.SetUnixFileMode(stream.SafeFileHandle, OwnerReadWrite);
        using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        writer.Write(contents);
    }

    private static bool IsAtomicTemporaryFile(string candidatePath, string fileName)
    {
        var candidateName = Path.GetFileName(candidatePath);
        var prefix = $".{fileName}.";
        const string suffix = ".tmp";
        if (!candidateName.StartsWith(prefix, StringComparison.Ordinal) ||
            !candidateName.EndsWith(suffix, StringComparison.Ordinal))
        {
            return false;
        }

        var identifier = candidateName[prefix.Length..^suffix.Length];
        return identifier.Length == 32 && identifier.All(Uri.IsHexDigit);
    }

    /// <summary>
    /// DE: Schreibt Text asynchron über eine temporäre Datei und ersetzt das Ziel atomar.
    /// EN: Writes text asynchronously through a temporary file and atomically replaces the target.
    /// </summary>
    /// <param name="destinationPath">DE: Zielpfad im bereits geprüften Verzeichnis. EN: Destination in an already validated directory.</param>
    /// <param name="contents">DE: Zu schreibender Text. EN: Text to write.</param>
    /// <param name="cancellationToken">DE: Bricht den asynchronen Schreibvorgang ab. EN: Cancels the asynchronous write.</param>
    /// <returns>DE: Aufgabe für den atomaren Schreibvorgang. EN: Task for the atomic write.</returns>
    /// <exception cref="IOException">DE: Schreiben oder Ersetzen ist fehlgeschlagen. EN: Writing or replacement failed.</exception>
    /// <exception cref="OperationCanceledException">DE: Der Schreibvorgang wurde abgebrochen. EN: The write operation was cancelled.</exception>
    public static async Task WriteAllTextAsync(
        string destinationPath,
        string contents,
        CancellationToken cancellationToken = default)
    {
        ServicePath.EnsurePrivateFilePath(destinationPath);
        var temporaryPath = CreateTemporaryPath(destinationPath);
        try
        {
            if (OperatingSystem.IsWindows())
            {
                await File.WriteAllTextAsync(temporaryPath, contents, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var options = new FileStreamOptions
                {
                    Access = FileAccess.Write,
                    Mode = FileMode.CreateNew,
                    Share = FileShare.None,
                    Options = FileOptions.Asynchronous,
                    UnixCreateMode = OwnerReadWrite
                };
                await using var stream = new FileStream(temporaryPath, options);
                File.SetUnixFileMode(stream.SafeFileHandle, OwnerReadWrite);
                await using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                await writer.WriteAsync(contents.AsMemory(), cancellationToken).ConfigureAwait(false);
            }

            File.Move(temporaryPath, destinationPath, overwrite: true);
            ProtectExistingOwnerOnlyFile(destinationPath);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }

    private static string CreateTemporaryPath(string destinationPath)
    {
        var directory = Path.GetDirectoryName(destinationPath)
            ?? throw new ArgumentException("DE: Zielpfad ohne Verzeichnis. EN: Destination path has no directory.", nameof(destinationPath));
        return Path.Combine(directory, $".{Path.GetFileName(destinationPath)}.{Guid.NewGuid():N}.tmp");
    }
}
