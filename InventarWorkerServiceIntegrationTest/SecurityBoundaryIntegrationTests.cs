namespace InventarWorkerServiceIntegrationTests;

/// <summary>
/// DE: Prüft statische Sicherheitsverträge des Worker-HTTP-Hosts.
/// EN: Verifies static security contracts of the Worker HTTP host.
/// </summary>
[TestClass]
public sealed class SecurityBoundaryIntegrationTests
{
    /// <summary>
    /// DE: Fordert Authentifizierung und die Transportpruefung vor dem Listener-Aufbau.
    /// EN: Requires authentication and transport validation before listener construction.
    /// </summary>
    [TestMethod]
    public void WorkerHost_SecurityPipeline_RequiresAuthenticationAndPreBindTransportGate()
    {
        var program = File.ReadAllText(PathInRepository("InventarWorkerService/Program.cs"));
        var configuration = File.ReadAllText(PathInRepository("InventarWorkerService/appsettings.json"));
        var transportGate = program.IndexOf("ApiTransportSecurity.EnsureSecureBindings(", StringComparison.Ordinal);
        var hostBuild = program.IndexOf("var app = builder.Build();", StringComparison.Ordinal);

        StringAssert.Contains(program, "app.UseAuthentication();");
        Assert.IsTrue(transportGate >= 0, "Das Transport-Gate fehlt im Worker-Host.");
        Assert.IsTrue(hostBuild > transportGate, "Das Transport-Gate muss vor builder.Build() laufen.");
        StringAssert.Contains(program, "builder.Configuration[\"HTTP_PORTS\"]");
        StringAssert.Contains(configuration, "http://127.0.0.1:80");
        Assert.IsFalse(configuration.Contains("http://0.0.0.0", StringComparison.Ordinal));
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
