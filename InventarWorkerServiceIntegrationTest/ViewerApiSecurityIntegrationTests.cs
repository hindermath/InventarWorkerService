namespace InventarWorkerServiceIntegrationTests;

/// <summary>
/// DE: Prüft Sicherheitsverträge des Viewer-HTTP-Hosts.
/// EN: Verifies security contracts of the Viewer HTTP host.
/// </summary>
[TestClass]
public sealed class ViewerApiSecurityIntegrationTests
{
    /// <summary>
    /// DE: Verlangt Authentifizierung, HTTPS und generische Fehler.
    /// EN: Requires authentication, HTTPS, and generic errors.
    /// </summary>
    [TestMethod]
    public void ViewerHost_SecurityBoundary_IsFailSafe()
    {
        var webApi = File.ReadAllText(PathInRepository("InventarViewerApp/API/WebApi.cs"));
        var controller = File.ReadAllText(PathInRepository("InventarViewerApp/Controllers/InventarController.cs"));
        StringAssert.Contains(webApi, "app.UseAuthentication();");
        StringAssert.Contains(webApi, "app.UseHttpsRedirection();");
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
