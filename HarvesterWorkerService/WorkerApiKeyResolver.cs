using System.Text.Json;
using InventarWorkerCommon.Models.SqlDatabase;

namespace HarvesterWorkerService;

/// <summary>
/// DE: Löst pro Worker einen geheimen API-Schlüssel aus einem extern bereitgestellten JSON-Secret auf.
/// EN: Resolves a per-worker API key from an externally provided JSON secret.
/// </summary>
internal static class WorkerApiKeyResolver
{
    private const string EnvironmentVariable = "HARVESTER_WORKER_API_KEYS";

    internal static bool TryResolve(MachineState machine, out string apiKey)
    {
        apiKey = string.Empty;
        var serializedMappings = Environment.GetEnvironmentVariable(EnvironmentVariable);
        if (string.IsNullOrWhiteSpace(serializedMappings))
        {
            return false;
        }

        try
        {
            var mappings = JsonSerializer.Deserialize<Dictionary<string, string>>(serializedMappings);
            if (mappings is null)
            {
                return false;
            }

            foreach (var identity in new[] { machine.FQDN, machine.Name, machine.IPv4, machine.IPv6 })
            {
                if (!string.IsNullOrWhiteSpace(identity) && mappings.TryGetValue(identity, out var candidate) && !string.IsNullOrWhiteSpace(candidate))
                {
                    apiKey = candidate;
                    return true;
                }
            }
        }
        catch (JsonException)
        {
            // DE: Ungültige Secrets werden wie fehlende Zuordnungen fail-closed behandelt.
            // EN: Invalid secrets are treated fail-closed like missing mappings.
        }

        return false;
    }
}
