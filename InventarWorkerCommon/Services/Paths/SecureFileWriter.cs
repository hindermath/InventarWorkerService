namespace InventarWorkerCommon.Services.Paths;

/// <summary>
/// DE: Stellt begrenzte Pfadprüfung und atomare Dateiersetzung für Statusartefakte bereit.
/// EN: Provides bounded path validation and atomic file replacement for status artifacts.
/// </summary>
public static class SecureFileWriter
{
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
        var temporaryPath = CreateTemporaryPath(destinationPath);
        try
        {
            File.WriteAllText(temporaryPath, contents);
            File.Move(temporaryPath, destinationPath, overwrite: true);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
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
        var temporaryPath = CreateTemporaryPath(destinationPath);
        try
        {
            await File.WriteAllTextAsync(temporaryPath, contents, cancellationToken).ConfigureAwait(false);
            File.Move(temporaryPath, destinationPath, overwrite: true);
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
