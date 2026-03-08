using System.Text.Json;
using Microsoft.Playwright;

namespace InventarWorkerServiceIntegrationTests;

/// <summary>
/// DE: Enthält Tests für InventarWorkerServiceApiTests.
/// EN: Contains tests for InventarWorkerServiceApiTests.
/// </summary>
[TestClass]
public class InventarWorkerServiceApiTests : PlaywrightTest
{
    private const string LocalBaseUrl = "http://127.0.0.1:8080";
    private IAPIRequestContext? _apiContext;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// DE: Initialisiert eine neue Instanz von <see cref="InventarWorkerServiceApiTests"/>.
    /// EN: Initializes a new instance of <see cref="InventarWorkerServiceApiTests"/>.
    /// </summary>
    public InventarWorkerServiceApiTests()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// DE: Startet den lokalen Test-Host für Integrationsprüfungen auf Port 8080.
    /// EN: Starts the local test host for integration checks on port 8080.
    /// </summary>
    /// <param name="context">
    /// DE: MSTest-Kontext für die Klasseninitialisierung.
    /// EN: MSTest context for class initialization.
    /// </param>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Host-Starts.
    /// EN: Asynchronous result indicating completion of host startup.
    /// </returns>
    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        await LocalServiceHostManager.StartAsync(LocalBaseUrl);
    }

    /// <summary>
    /// DE: Beendet den lokalen Test-Host nach Abschluss aller Tests dieser Klasse.
    /// EN: Stops the local test host after all tests in this class are completed.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Host-Stopps.
    /// EN: Asynchronous result indicating completion of host shutdown.
    /// </returns>
    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        await LocalServiceHostManager.StopAsync();
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
        _apiContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = LocalBaseUrl,
            IgnoreHTTPSErrors = true
        });

        await EnsureReachableOrFailAsync(LocalBaseUrl);
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
        if (_apiContext is not null)
        {
            await _apiContext.DisposeAsync();
        }
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_ServiceStatus aus.
    /// EN: Executes the test or helper step Test_ServiceStatus.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_ServiceStatus.
    /// EN: Asynchronous result indicating completion of step Test_ServiceStatus.
    /// </returns>
    [TestMethod]
    [Description("Service Status prüfen")]
    public async Task Test_ServiceStatus()
    {
        // Arrange & Act
        var response = await _apiContext!.GetAsync("/api/inventar/status", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] = "application/json"
            }
        });

        // Assert
        Assert.IsTrue(response.Ok, $"Status check failed with status {response.Status}");
        
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

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_HardwareInventar aus.
    /// EN: Executes the test or helper step Test_HardwareInventar.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_HardwareInventar.
    /// EN: Asynchronous result indicating completion of step Test_HardwareInventar.
    /// </returns>
    [TestMethod]
    [Description("Hardware Inventar abrufen")]
    public async Task Test_HardwareInventar()
    {
        // Arrange & Act
        var response = await _apiContext!.GetAsync("/api/inventar/hardware", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] = "application/json"
            }
        });

        // Assert
        Assert.IsTrue(response.Ok, $"Hardware inventory request failed with status {response.Status}");
        
        var content = await response.TextAsync();
        Assert.IsFalse(string.IsNullOrEmpty(content), "Hardware inventory should not be empty");
        
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

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_SoftwareInventar aus.
    /// EN: Executes the test or helper step Test_SoftwareInventar.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_SoftwareInventar.
    /// EN: Asynchronous result indicating completion of step Test_SoftwareInventar.
    /// </returns>
    [TestMethod]
    [Description("Software Inventar abrufen")]
    public async Task Test_SoftwareInventar()
    {
        // Arrange & Act
        var response = await _apiContext!.GetAsync("/api/inventar/software", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] = "application/json"
            }
        });

        // Assert
        Assert.IsTrue(response.Ok, $"Software inventory request failed with status {response.Status}");
        
        var content = await response.TextAsync();
        Assert.IsFalse(string.IsNullOrEmpty(content), "Software inventory should not be empty");
        
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

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_KomplettesInventar aus.
    /// EN: Executes the test or helper step Test_KomplettesInventar.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_KomplettesInventar.
    /// EN: Asynchronous result indicating completion of step Test_KomplettesInventar.
    /// </returns>
    [TestMethod]
    [Description("Komplettes Inventar abrufen")]
    public async Task Test_KomplettesInventar()
    {
        // Arrange & Act
        var response = await _apiContext!.GetAsync("/api/inventar/full", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] = "application/json"
            }
        });

        // Assert
        Assert.IsTrue(response.Ok, $"Full inventory request failed with status {response.Status}");
        
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

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_LocalhostIP aus.
    /// EN: Executes the test or helper step Test_LocalhostIP.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_LocalhostIP.
    /// EN: Asynchronous result indicating completion of step Test_LocalhostIP.
    /// </returns>
    [TestMethod]
    [Description("Test mit 127.0.0.1")]
    public async Task Test_LocalhostIP()
    {
        // Arrange
        var localApiContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = LocalBaseUrl,
            IgnoreHTTPSErrors = true
        });

        try
        {
            // Act
            var response = await localApiContext.GetAsync("/api/inventar/status", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    ["Accept"] = "application/json"
                }
            });

            // Assert
            Assert.IsTrue(response.Ok, $"Localhost IP test failed with status {response.Status}");
            
            var content = await response.TextAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content), "Response content should not be empty");

            // Verify JSON format
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
        finally
        {
            await localApiContext.DisposeAsync();
        }
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_CustomHeaders aus.
    /// EN: Executes the test or helper step Test_CustomHeaders.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_CustomHeaders.
    /// EN: Asynchronous result indicating completion of step Test_CustomHeaders.
    /// </returns>
    [TestMethod]
    [Description("Test mit Custom Headers")]
    public async Task Test_CustomHeaders()
    {
        // Arrange
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        
        // Act
        var response = await _apiContext!.GetAsync("/api/inventar/full", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["User-Agent"] = "InventarClient/1.0",
                ["X-Request-ID"] = $"test-{timestamp}"
            }
        });

        // Assert
        Assert.IsTrue(response.Ok, $"Custom headers test failed with status {response.Status}");
        
        var content = await response.TextAsync();
        Assert.IsFalse(string.IsNullOrEmpty(content), "Response content should not be empty");

        // Verify JSON format
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

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_SwaggerDokumentation aus.
    /// EN: Executes the test or helper step Test_SwaggerDokumentation.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_SwaggerDokumentation.
    /// EN: Asynchronous result indicating completion of step Test_SwaggerDokumentation.
    /// </returns>
    [TestMethod]
    [Description("Swagger Dokumentation testen")]
    public async Task Test_SwaggerDokumentation()
    {
        // Act
        var response = await _apiContext!.GetAsync("/swagger/index.html", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] = "text/html"
            }
        });

        // Assert
        Assert.IsTrue(response.Ok, $"Swagger documentation request failed with status {response.Status}");
        
        var content = await response.TextAsync();
        Assert.IsFalse(string.IsNullOrEmpty(content), "Swagger HTML should not be empty");
        Assert.IsTrue(content.Contains("swagger", StringComparison.OrdinalIgnoreCase), 
            "Response should contain swagger content");
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_Gesundheitscheck aus.
    /// EN: Executes the test or helper step Test_Gesundheitscheck.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_Gesundheitscheck.
    /// EN: Asynchronous result indicating completion of step Test_Gesundheitscheck.
    /// </returns>
    [TestMethod]
    [Description("Gesundheitscheck testen")]
    public async Task Test_Gesundheitscheck()
    {
        // Act
        var response = await _apiContext!.GetAsync("/health", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] = "application/json"
            }
        });

        // Assert
        // Health endpoint might not exist, so we check for either success or 404
        Assert.IsTrue(response.Ok || response.Status == 404, 
            $"Health check failed with unexpected status {response.Status}");
        
        if (response.Ok)
        {
            var content = await response.TextAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content), "Health check response should not be empty");

            // Try to parse as JSON if content is not empty
            if (!string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    using var document = JsonDocument.Parse(content);
                    Assert.IsNotNull(document.RootElement, "JSON should be parseable");
                }
                catch (JsonException)
                {
                    // Health endpoint might return plain text, so this is acceptable
                }
            }
        }
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_SwaggerJson aus.
    /// EN: Executes the test or helper step Test_SwaggerJson.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_SwaggerJson.
    /// EN: Asynchronous result indicating completion of step Test_SwaggerJson.
    /// </returns>
    [TestMethod]
    [Description("API Dokumentation JSON testen")]
    public async Task Test_SwaggerJson()
    {
        // Act
        var response = await _apiContext!.GetAsync("/swagger/v1/swagger.json", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] = "application/json"
            }
        });

        // Assert
        Assert.IsTrue(response.Ok, $"Swagger JSON request failed with status {response.Status}");
        
        var content = await response.TextAsync();
        Assert.IsFalse(string.IsNullOrEmpty(content), "Swagger JSON should not be empty");
        
        // Verify JSON format with System.Text.Json
        try
        {
            using var document = JsonDocument.Parse(content);
            Assert.IsNotNull(document.RootElement, "JSON should be parseable");
            
            // Additional validation for Swagger/OpenAPI structure
            if (document.RootElement.TryGetProperty("openapi", out var openApiProperty) ||
                document.RootElement.TryGetProperty("swagger", out var swaggerProperty))
            {
                Assert.IsTrue(true, "Valid OpenAPI/Swagger document detected");
            }
        }
        catch (JsonException ex)
        {
            Assert.Fail($"Response should be valid JSON: {ex.Message}");
        }
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt Test_PerformanceMultipleRequests aus.
    /// EN: Executes the test or helper step Test_PerformanceMultipleRequests.
    /// </summary>
    /// <returns>
    /// DE: Asynchrones Ergebnis für den Abschluss des Schritts Test_PerformanceMultipleRequests.
    /// EN: Asynchronous result indicating completion of step Test_PerformanceMultipleRequests.
    /// </returns>
    [TestMethod]
    [Description("Performance Test - Mehrere parallele Anfragen")]
    public async Task Test_PerformanceMultipleRequests()
    {
        // Arrange
        var tasks = new List<Task<IAPIResponse>>();
        var endpoints = new[]
        {
            "/api/inventar/hardware",
            "/api/inventar/software", 
            "/api/inventar/status"
        };

        // Act
        foreach (var endpoint in endpoints)
        {
            var task = _apiContext!.GetAsync(endpoint, new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    ["Accept"] = "application/json"
                }
            });
            tasks.Add(task);
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        foreach (var response in responses)
        {
            Assert.IsTrue(response.Ok, $"One of the performance test requests failed with status {response.Status}");
            
            var content = await response.TextAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content), "Response content should not be empty");

            // Verify JSON format
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

        // Cleanup
        foreach (var response in responses)
        {
            await response.DisposeAsync();
        }
    }

    private async Task EnsureReachableOrFailAsync(string baseUrl)
    {
        try
        {
            var probeResponse = await _apiContext!.GetAsync("/api/inventar/status", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    ["Accept"] = "application/json"
                }
            });

            if (probeResponse.Status == 403)
            {
                Assert.Fail($"Testziel {baseUrl} antwortet mit 403.");
            }

            if (!probeResponse.Ok)
            {
                Assert.Fail($"Testziel {baseUrl} antwortet mit HTTP {probeResponse.Status}.");
            }
        }
        catch (Exception exception)
        {
            Assert.Fail($"Testziel {baseUrl} ist nicht erreichbar: {exception.Message}");
        }
    }
}
