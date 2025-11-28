using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Services.Paths;

namespace InventarWorkerCommon.Services.Settings;

/// <summary>
/// Provides functionality to read configuration settings from various sources.
/// This class is designed to simplify the retrieval and management of settings
/// across the application.
/// </summary>
public class SettingsReader
{
    private readonly string _statusDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Provides functionality to read configuration settings for the application.
    /// This class enables reading configuration data from a predefined status directory
    /// and deserializes it into strongly-typed objects.
    /// </summary>
    public SettingsReader(string statusDirectory = "inventar-service")
    {
        if (ServicePath.ExistsServiceStatusPath(Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory)))
            _statusDirectory = Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

    }

    /// <summary>
    /// Reads the configuration settings from a predefined status directory.
    /// This method attempts to locate and deserialize the settings stored in a JSON file
    /// within the specified status directory. If the file doesn't exist or an error occurs
    /// during the read operation, it returns null.
    /// </summary>
    /// <returns>
    /// An instance of the <see cref="Models.Settings.Settings"/> class containing
    /// the deserialized configuration settings, or null if the file is not found or cannot be read.
    /// </returns>
    public Models.Settings.Settings? ReadSettings()
    {
        try
        {
            var settingsFile = System.IO.Path.Combine(_statusDirectory, "settings.json");
            if (!File.Exists(settingsFile)) return null;

            var json = File.ReadAllText(settingsFile);
            return JsonSerializer.Deserialize<Models.Settings.Settings>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Lesen des Status: {ex.Message}");
            return null;
        }

    }
}