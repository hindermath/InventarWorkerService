using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Services.Paths;

namespace InventarWorkerCommon.Services.Settings;

/// <summary>
/// Provides functionality to handle the writing of application settings.
/// </summary>
public class SettingsWriter
{
    private readonly string _statusDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Handles the writing of application settings to a specified directory in a JSON format.
    /// </summary>
    public SettingsWriter(string statusDirectory = "inventar-service")
    {
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
    public void WriteSettingsToFile(Models.Settings.Settings settings)
    {
        var settingsFile = Path.Combine(_statusDirectory, "settings.json");
        string json = JsonSerializer.Serialize(settings, _jsonOptions);
        File.WriteAllText(settingsFile, json);
    }
}