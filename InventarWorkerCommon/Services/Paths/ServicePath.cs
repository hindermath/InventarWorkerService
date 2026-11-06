namespace InventarWorkerCommon.Services.Paths;

/// <summary>
/// Provides methods for managing and retrieving platform-specific paths used by services.
/// </summary>
public static class ServicePath
{
    private const UnixFileMode OwnerOnlyDirectory = UnixFileMode.UserRead |
                                                     UnixFileMode.UserWrite |
                                                     UnixFileMode.UserExecute;

    /// <summary>
    /// Retrieves the platform-specific path used to store or access service status information.
    /// Combines the base directory with a service status directory environment variable or defaults to a predefined
    /// name if the variable is not set.
    /// </summary>
    /// <returns>
    /// A string representing the computed service status path.
    /// </returns>
    public static string GetServiceStatusPath()
    {
        var directoryName = SecureFileWriter.ValidateDirectoryName(
            Environment.GetEnvironmentVariable("SERVICESTATUSDIRECTORY") ?? "InventarWorkerService");
        return System.IO.Path.Combine(BasePath.GetBasePath(), directoryName);
    }

    /// <summary>
    /// Creates a directory at the specified service status path if it does not already exist.
    /// Validates the creation by checking if the directory exists after the attempt.
    /// </summary>
    /// <param name="serviceStatusPath">The path where the service status directory is to be created.</param>
    /// <returns>
    /// A boolean value indicating whether the directory exists after the creation attempt.
    /// Returns true if the directory exists, false otherwise.
    /// </returns>
    public static DirectoryInfo CreateServiceStatusPath(string serviceStatusPath)
    {
        var normalizedRoot = Path.GetFullPath(GetServiceStatusPath());
        var normalizedPath = Path.GetFullPath(serviceStatusPath);
        var isRoot = ValidateServiceDirectoryPath(normalizedRoot, normalizedPath, nameof(serviceStatusPath));

        EnsurePrivateDirectory(normalizedRoot);
        if (!isRoot)
        {
            EnsurePrivateDirectory(normalizedPath);
        }

        return new DirectoryInfo(normalizedPath);
    }

    /// <summary>
    /// Checks whether the specified service status path exists in the file system.
    /// </summary>
    /// <param name="serviceStatusPath">The path to the service status directory to check for existence.</param>
    /// <returns>
    /// A boolean value indicating whether the specified path exists in the file system.
    /// Returns true if the path exists, false otherwise.
    /// </returns>
    public static bool ExistsServiceStatusPath(string serviceStatusPath)
    {
        var normalizedRoot = Path.GetFullPath(GetServiceStatusPath());
        var normalizedPath = Path.GetFullPath(serviceStatusPath);
        var isRoot = ValidateServiceDirectoryPath(normalizedRoot, normalizedPath, nameof(serviceStatusPath));
        if (!Directory.Exists(normalizedPath))
        {
            return false;
        }

        EnsurePrivateDirectory(normalizedRoot);
        if (!isRoot)
        {
            EnsurePrivateDirectory(normalizedPath);
        }

        return true;
    }

    /// <summary>
    /// DE: Prüft, dass ein Dateipfad im geschützten Dienststamm liegt und weder sein Verzeichnis noch eine vorhandene Datei ein Link ist.
    /// EN: Verifies that a file path is inside the protected service root and neither its directory nor an existing file is a link.
    /// </summary>
    /// <param name="filePath">DE: Zu prüfender Artefaktpfad. EN: Artifact path to validate.</param>
    /// <exception cref="UnauthorizedAccessException">DE: Der Pfad verletzt die private Zustandsgrenze. EN: The path violates the private state boundary.</exception>
    internal static void EnsurePrivateFilePath(string filePath)
    {
        var directoryPath = Path.GetDirectoryName(filePath)
            ?? throw new ArgumentException("DE: Zielpfad ohne Verzeichnis. EN: Destination path has no directory.", nameof(filePath));
        var normalizedRoot = Path.GetFullPath(GetServiceStatusPath());
        var normalizedDirectory = Path.GetFullPath(directoryPath);
        var isRoot = string.Equals(normalizedDirectory, normalizedRoot, StringComparison.Ordinal);
        var isDirectChild = string.Equals(Path.GetDirectoryName(normalizedDirectory), normalizedRoot, StringComparison.Ordinal);
        if (!isRoot && !isDirectChild)
        {
            throw new UnauthorizedAccessException(
                "DE: Statusartefakte dürfen nur im geschützten Dienststamm oder dessen direktem Unterverzeichnis liegen. " +
                "EN: Status artifacts may only reside in the protected service root or its direct child directory.");
        }

        EnsurePrivateDirectory(normalizedRoot);
        if (!isRoot)
        {
            EnsurePrivateDirectory(normalizedDirectory);
        }

        EnsureNotLink(filePath, "Datei");
    }

    private static void EnsurePrivateDirectory(string directoryPath)
    {
        EnsureTrustedBaseDirectory();
        var existed = Directory.Exists(directoryPath);
        if (!existed)
        {
            Directory.CreateDirectory(directoryPath);
            if (!OperatingSystem.IsWindows())
            {
                File.SetUnixFileMode(directoryPath, OwnerOnlyDirectory);
            }
        }

        EnsureNotLink(directoryPath, "Verzeichnis");
        if (OperatingSystem.IsWindows())
        {
            WindowsStateSecurity.ProtectDirectory(directoryPath);
            return;
        }

        UnixIdentityGuard.EnsureOwnedByCurrentUser(directoryPath);
        if (File.GetUnixFileMode(directoryPath) != OwnerOnlyDirectory)
        {
            throw new UnauthorizedAccessException(
                "DE: Das Dienstverzeichnis muss dem Dienstkonto gehören und exakt den Modus 0700 verwenden. " +
                "EN: The service directory must belong to the service account and use exact mode 0700.");
        }
    }

    private static void EnsureNotLink(string path, string objectType)
    {
        if (!OperatingSystem.IsWindows() && UnixIdentityGuard.IsSymbolicLink(path))
        {
            throw new UnauthorizedAccessException(
                $"DE: Das {objectType} darf kein symbolischer Link sein. EN: The {objectType} must not be a symbolic link.");
        }

        FileSystemInfo info = Directory.Exists(path) ? new DirectoryInfo(path) : new FileInfo(path);
        if (info.Exists && (info.LinkTarget is not null || (info.Attributes & FileAttributes.ReparsePoint) != 0))
        {
            throw new UnauthorizedAccessException(
                $"DE: Das {objectType} darf kein symbolischer Link sein. EN: The {objectType} must not be a symbolic link.");
        }
    }

    private static bool ValidateServiceDirectoryPath(string normalizedRoot, string normalizedPath, string parameterName)
    {
        var isRoot = string.Equals(normalizedPath, normalizedRoot, StringComparison.Ordinal);
        var isDirectChild = string.Equals(Path.GetDirectoryName(normalizedPath), normalizedRoot, StringComparison.Ordinal);
        if (!isRoot && !isDirectChild)
        {
            throw new ArgumentException(
                "DE: Das Statusverzeichnis muss der geschützte Dienststamm oder dessen direktes Unterverzeichnis sein. " +
                "EN: The status directory must be the protected service root or its direct child directory.",
                parameterName);
        }

        if (!isRoot)
        {
            _ = SecureFileWriter.ValidateDirectoryName(Path.GetFileName(normalizedPath));
        }

        return isRoot;
    }

    private static void EnsureTrustedBaseDirectory()
    {
        if (OperatingSystem.IsWindows() || !string.IsNullOrWhiteSpace(BasePath.UnixBasePathOverride))
        {
            return;
        }

        var basePath = Path.GetFullPath(BasePath.GetBasePath());
        for (var current = new DirectoryInfo(basePath); current.Parent is not null; current = current.Parent)
        {
            EnsureNotLink(current.FullName, "Basisverzeichnis");
            var mode = File.GetUnixFileMode(current.FullName);
            const UnixFileMode unsafeWriteBits = UnixFileMode.GroupWrite | UnixFileMode.OtherWrite;
            if ((mode & unsafeWriteBits) != 0)
            {
                throw new UnauthorizedAccessException(
                    "DE: Der Dienstzustand benötigt eine nicht gruppen- oder weltbeschreibbare Systembasis. " +
                    "EN: Service state requires a system base that is not group- or world-writable.");
            }
        }
    }
}
