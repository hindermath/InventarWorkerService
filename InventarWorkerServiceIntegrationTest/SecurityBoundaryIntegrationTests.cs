namespace InventarWorkerServiceIntegrationTests;

/// <summary>
/// DE: Prüft statische Sicherheitsverträge des Worker-HTTP-Hosts.
/// EN: Verifies static security contracts of the Worker HTTP host.
/// </summary>
[TestClass]
public sealed class SecurityBoundaryIntegrationTests
{
    /// <summary>
    /// DE: Fordert Authentifizierung und HTTPS im Pipelinevertrag.
    /// EN: Requires authentication and HTTPS in the pipeline contract.
    /// </summary>
    [TestMethod]
    public void WorkerHost_SecurityPipeline_RequiresAuthenticationAndHttps()
    {
        var program = File.ReadAllText(PathInRepository("InventarWorkerService/Program.cs"));
        StringAssert.Contains(program, "app.UseAuthentication();");
        StringAssert.Contains(program, "app.UseHttpsRedirection();");
    }

    /// <summary>
    /// DE: Verhindert interne Exception-Meldungen in Außenantworten.
    /// EN: Prevents internal exception messages in external responses.
    /// </summary>
    [TestMethod]
    public void WorkerController_ErrorResponses_DoNotExposeExceptionMessage()
    {
        var controller = File.ReadAllText(PathInRepository("InventarWorkerService/Controllers/InventarController.cs"));
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
