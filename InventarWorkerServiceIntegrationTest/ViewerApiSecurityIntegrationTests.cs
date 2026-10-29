namespace InventarWorkerServiceIntegrationTests;

/// <summary>
/// DE: Prüft Sicherheitsverträge des Viewer-HTTP-Hosts.
/// EN: Verifies security contracts of the Viewer HTTP host.
/// </summary>
[TestClass]
public sealed class ViewerApiSecurityIntegrationTests
{
    /// <summary>
    /// DE: Verlangt Authentifizierung, ein Pre-Bind-Transport-Gate und generische Fehler.
    /// EN: Requires authentication, a pre-bind transport gate, and generic errors.
    /// </summary>
    [TestMethod]
    public void ViewerHost_SecurityBoundary_IsFailSafe()
    {
        var webApi = File.ReadAllText(PathInRepository("InventarViewerApp/API/WebApi.cs"));
        var controller = File.ReadAllText(PathInRepository("InventarViewerApp/Controllers/InventarController.cs"));
        var configuration = File.ReadAllText(PathInRepository("InventarViewerApp/appsettings.json"));
        var transportGate = webApi.IndexOf("ApiTransportSecurity.EnsureSecureBindings(", StringComparison.Ordinal);
        var hostBuild = webApi.IndexOf("var app = builder.Build();", StringComparison.Ordinal);

        StringAssert.Contains(webApi, "app.UseAuthentication();");
        StringAssert.Contains(webApi, "a is not \"--api\" and not \"--start\" and not \"--stop\"");
        Assert.IsTrue(transportGate >= 0, "Das Transport-Gate fehlt im Viewer-Host.");
        Assert.IsTrue(hostBuild > transportGate, "Das Transport-Gate muss vor builder.Build() laufen.");
        StringAssert.Contains(webApi, "builder.Configuration[\"HTTP_PORTS\"]");
        StringAssert.Contains(configuration, "http://127.0.0.1:80");
        Assert.IsFalse(configuration.Contains("http://0.0.0.0", StringComparison.Ordinal));
        Assert.IsFalse(controller.Contains("ex.Message", StringComparison.Ordinal));
    }

    private static string PathInRepository(string relativePath)
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null && !File.Exists(Path.Combine(current.FullName, "InventarWorkerService.sln")))
        {
            current = current.Parent;
        }

        return Path.Combine(current?.FullName ?? throw new DirectoryNotFoundException(), relativePath);
    }
}
