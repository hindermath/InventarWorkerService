using InventarWorkerCommon.Services.Software;
using Microsoft.Extensions.Logging.Abstractions;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Prüft, dass Softwareinventare keine Geheimnisse aus der Prozessumgebung ausgeben.
/// EN: Verifies that software inventories do not expose secrets from the process environment.
/// </summary>
[TestClass]
[DoNotParallelize]
public sealed class SoftwareInventorySecurityTest
{
    /// <summary>
    /// DE: Ersetzt den Wert einer als geheim erkannten Umgebungsvariablen durch eine feste Markierung.
    /// EN: Replaces the value of a secret-like environment variable with a fixed marker.
    /// </summary>
    [TestMethod]
    public async Task CollectSoftwareInventory_SecretEnvironmentVariable_IsRedacted()
    {
        const string variableName = "FEATURE002_TEST_API_TOKEN";
        const string secretValue = "feature002-sensitive-value";
        var previous = Environment.GetEnvironmentVariable(variableName);
        Environment.SetEnvironmentVariable(variableName, secretValue);

        try
        {
            var service = new SoftwareInventoryService(NullLogger<SoftwareInventoryService>.Instance);
            var inventory = await service.CollectSoftwareInventoryAsync();
            var entry = inventory.EnvironmentVariables.Single(value => value.StartsWith($"{variableName}=", StringComparison.Ordinal));

            Assert.AreEqual($"{variableName}=[REDACTED]", entry);
            Assert.IsFalse(inventory.EnvironmentVariables.Any(value => value.Contains(secretValue, StringComparison.Ordinal)));
        }
        finally
        {
            Environment.SetEnvironmentVariable(variableName, previous);
        }
    }
}
