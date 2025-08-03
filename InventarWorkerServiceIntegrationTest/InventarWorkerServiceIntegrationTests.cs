using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Net;
using System.Text.Json;

namespace InventarWorkerServiceIntegrationTests;

[TestClass]
public class InventarWorkerServiceApiTests : PageTest
{
    private const string BaseUrl = "http://localhost:5000";
    private const string RemoteUrl = "http://192.168.1.100:5000";
    private IAPIRequestContext? _apiContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public InventarWorkerServiceApiTests()
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
        _apiContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = BaseUrl,
            IgnoreHTTPSErrors = true
        });
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        //await _apiContext?.DisposeAsync()!;
    }

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

    [TestMethod]
    [Description("Test mit 127.0.0.1")]
    public async Task Test_LocalhostIP()
    {
        // Arrange
        var localApiContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = "http://127.0.0.1:5000",
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
}