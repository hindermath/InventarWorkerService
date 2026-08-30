using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Services.Paths;
using YamlDotNet.Serialization;

namespace InventarWorkerCommon.Services.Settings;

/// <summary>
/// DE: Schreibt Anwendungseinstellungen atomar in ein begrenztes Statusverzeichnis.
/// EN: Writes application settings atomically to a bounded status directory.
/// </summary>
public class SettingsWriter
{
    private readonly string _statusDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// DE: Initialisiert den Writer für genau ein sicheres Unterverzeichnis.
    /// EN: Initializes the writer for exactly one safe subdirectory.
    /// </summary>
    /// <param name="statusDirectory">DE: Einzelner Name des Status-Unterverzeichnisses. EN: Single name of the status subdirectory.</param>
    /// <exception cref="ArgumentException">DE: Der Verzeichnisname ist nicht sicher. EN: The directory name is unsafe.</exception>
    public SettingsWriter(string statusDirectory = "inventar-service")
    {
        statusDirectory = SecureFileWriter.ValidateDirectoryName(statusDirectory);
        if (ServicePath.ExistsServiceStatusPath(Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory)) is
            false)
        {
            var directory =
                ServicePath.CreateServiceStatusPath(Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory));
            _statusDirectory = directory.FullName;
        }
        else
        {
            _statusDirectory = Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory);
        }

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };
    }

    /// <summary>
    /// Writes the specified application settings to a file in JSON format at the specified file path.
    /// </summary>
    /// <param name="settings">The settings object containing the configuration data to be written to the file.</param>
    public void WriteSettings(Models.Settings.Settings settings)
    {
        var settingsFile = Path.Combine(_statusDirectory, "settings.json");
        string json = JsonSerializer.Serialize(settings, _jsonOptions);
        SecureFileWriter.WriteAllText(settingsFile, json);
    }

    /// <summary>
    /// Writes the specified application settings to a file in YAML format at the specified file path.
    /// </summary>
    /// <param name="settings">The settings object containing the configuration data to be written to the file.</param>
    public void WriteSettingsYaml(Models.Settings.Settings settings)
    {
        var settingsFile = Path.Combine(_statusDirectory, "settings.yaml");

        var serializer = new SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(settings);
        SecureFileWriter.WriteAllText(settingsFile, yaml);
    }
}
