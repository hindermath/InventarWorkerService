using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Text.Json;

namespace InventarWorkerServiceIntegrationTests;

[TestClass]
public class InventarWorkerServiceRemoteTests : PageTest
{
    private const string RemoteUrl = "http://192.168.1.100:5000";
    private IAPIRequestContext? _remoteApiContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public InventarWorkerServiceRemoteTests()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    [TestInitialize]
    public async Task Setup()
    {
        _remoteApiContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = RemoteUrl,
            IgnoreHTTPSErrors = true,
            Timeout = 10000 // 10 Sekunden Timeout für Remote-Tests
        });
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        //await _remoteApiContext?.DisposeAsync()!;
    }

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
        catch (PlaywrightException ex) when (ex.Message.Contains("ECONNREFUSED") || ex.Message.Contains("timeout"))
        {
            Assert.Inconclusive($"Remote server not reachable at {RemoteUrl}: {ex.Message}");
        }
    }

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
        catch (PlaywrightException ex) when (ex.Message.Contains("ECONNREFUSED") || ex.Message.Contains("timeout"))
        {
            Assert.Inconclusive($"Remote server not reachable at {RemoteUrl}: {ex.Message}");
        }
    }
}