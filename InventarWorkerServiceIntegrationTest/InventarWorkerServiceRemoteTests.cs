using System.Text.Json;
using Microsoft.Playwright;

namespace InventarWorkerServiceIntegrationTests;

/// <summary>
/// DE: Enthält Tests für InventarWorkerServiceRemoteTests.
/// EN: Contains tests for InventarWorkerServiceRemoteTests.
/// </summary>
[TestClass]
public class InventarWorkerServiceRemoteTests : PlaywrightTest
{
    private const string RunRemoteTestsEnvVar = "INVENTAR_RUN_REMOTE_TESTS";
    private const string RemoteUrl = "http://192.168.2.169:5000";
    private IAPIRequestContext? _remoteApiContext;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// DE: Initialisiert eine neue Instanz von <see cref="InventarWorkerServiceRemoteTests"/>.
    /// EN: Initializes a new instance of <see cref="InventarWorkerServiceRemoteTests"/>.
    /// </summary>
    public InventarWorkerServiceRemoteTests()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Setup aus.
    /// EN: Executes the test or helper step Setup.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Setup.
    /// EN: Asynchronous result indicating completion of step Setup.
    /// </returns>
    [TestInitialize]
    public async Task Setup()
    {
        if (!IsRemoteTestsEnabled())
        {
            Assert.Inconclusive(
                $"Remote-Tests sind deaktiviert. Setze {RunRemoteTestsEnvVar}=true, um sie auszuführen.");
        }

        _remoteApiContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = RemoteUrl,
            IgnoreHTTPSErrors = true,
            Timeout = 10000 // 10 Sekunden Timeout für Remote-Tests
        });
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Cleanup aus.
    /// EN: Executes the test or helper step Cleanup.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Cleanup.
    /// EN: Asynchronous result indicating completion of step Cleanup.
    /// </returns>
    [TestCleanup]
    public async Task Cleanup()
    {
        if (_remoteApiContext is not null)
        {
            await _remoteApiContext.DisposeAsync();
        }
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_RemoteServiceStatus aus.
    /// EN: Executes the test or helper step Test_RemoteServiceStatus.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_RemoteServiceStatus.
    /// EN: Asynchronous result indicating completion of step Test_RemoteServiceStatus.
    /// </returns>
    [TestMethod]
    [Description("Remote Service Status prüfen")]
    public async Task Test_RemoteServiceStatus()
    {
        try
        {
            // Act
            var response = await _remoteApiContext!.GetAsync("/api/inventar/status", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    ["Accept"] = "application/json"
                }
            });

            // Assert
            Assert.IsTrue(response.Ok, $"Remote status check failed with status {response.Status}");
            
            var content = await response.TextAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content), "Response content should not be empty");
            
            // Verify JSON format with System.Text.Json
            try
            {
                using var document = JsonDocument.Parse(content);
                Assert.IsNotNull(document.RootElement, "JSON should be parseable");
            }
            catch (JsonException ex)
            {
                Assert.Fail($"Response should be valid JSON: {ex.Message}");
            }
        }
        catch (PlaywrightException ex)
        {
            Assert.Inconclusive($"Remote server not reachable at {RemoteUrl}: {ex.Message}");
        }
        catch (TimeoutException ex)
        {
            Assert.Inconclusive($"Remote server timeout at {RemoteUrl}: {ex.Message}");
        }
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_RemoteKomplettesInventar aus.
    /// EN: Executes the test or helper step Test_RemoteKomplettesInventar.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_RemoteKomplettesInventar.
    /// EN: Asynchronous result indicating completion of step Test_RemoteKomplettesInventar.
    /// </returns>
    [TestMethod]
    [Description("Remote komplettes Inventar abrufen")]
    public async Task Test_RemoteKomplettesInventar()
    {
        try
        {
            // Act
            var response = await _remoteApiContext!.GetAsync("/api/inventar/full", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    ["Accept"] = "application/json"
                }
            });

            // Assert
            Assert.IsTrue(response.Ok, $"Remote full inventory request failed with status {response.Status}");
            
            var content = await response.TextAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content), "Full inventory should not be empty");
            
            // Verify JSON format with System.Text.Json
            try
            {
                using var document = JsonDocument.Parse(content);
                Assert.IsNotNull(document.RootElement, "JSON should be parseable");
            }
            catch (JsonException ex)
            {
                Assert.Fail($"Response should be valid JSON: {ex.Message}");
            }
        }
        catch (PlaywrightException ex)
        {
            Assert.Inconclusive($"Remote server not reachable at {RemoteUrl}: {ex.Message}");
        }
        catch (TimeoutException ex)
        {
            Assert.Inconclusive($"Remote server timeout at {RemoteUrl}: {ex.Message}");
        }
    }

    private static bool IsRemoteTestsEnabled()
    {
        var value = Environment.GetEnvironmentVariable(RunRemoteTestsEnvVar);
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }
}
