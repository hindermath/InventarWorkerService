using InventarWorkerCommon.Services.Api;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Prueft die eingehende Transportgrenze beider Inventar-API-Hosts.
/// EN: Verifies the inbound transport boundary of both inventory API hosts.
/// </summary>
[TestClass]
public sealed class ApiTransportSecurityTest
{
    /// <summary>
    /// DE: Erlaubt HTTP fuer alle unterstuetzten Loopback-Darstellungen.
    /// EN: Allows HTTP for all supported loopback representations.
    /// </summary>
    /// <param name="binding">DE: Zu pruefende Bindung. EN: Binding under test.</param>
    [TestMethod]
    [DataRow("http://localhost:80")]
    [DataRow("http://127.0.0.1:8080")]
    [DataRow("http://127.42.7.9:8080")]
    [DataRow("http://[::1]:8080")]
    public void EnsureSecureBindings_LoopbackHttp_IsAllowed(string binding)
    {
        var result = ApiTransportSecurity.EnsureSecureBindings([binding]);

        CollectionAssert.AreEqual(new[] { binding }, result.ToArray());
    }

    /// <summary>
    /// DE: Erlaubt verschluesselte entfernte und Wildcard-Bindungen.
    /// EN: Allows encrypted remote and wildcard bindings.
    /// </summary>
    /// <param name="binding">DE: Zu pruefende Bindung. EN: Binding under test.</param>
    [TestMethod]
    [DataRow("https://inventory.example.test:443")]
    [DataRow("https://0.0.0.0:443")]
    [DataRow("https://[::]:443")]
    [DataRow("https://*:443")]
    [DataRow("https://+:443")]
    public void EnsureSecureBindings_RemoteHttps_IsAllowed(string binding)
    {
        var result = ApiTransportSecurity.EnsureSecureBindings([binding]);

        CollectionAssert.AreEqual(new[] { binding }, result.ToArray());
    }

    /// <summary>
    /// DE: Weist entfernte und Wildcard-HTTP-Bindungen vor dem Listener-Start ab.
    /// EN: Rejects remote and wildcard HTTP bindings before listener startup.
    /// </summary>
    /// <param name="binding">DE: Unsichere Bindung. EN: Insecure binding.</param>
    [TestMethod]
    [DataRow("http://0.0.0.0:80")]
    [DataRow("http://[::]:80")]
    [DataRow("http://*:80")]
    [DataRow("http://+:80")]
    [DataRow("http://192.168.1.10:80")]
    [DataRow("http://inventory.example.test:80")]
    public void EnsureSecureBindings_RemoteHttp_ThrowsInvalidOperationException(string binding)
    {
        Assert.ThrowsExactly<InvalidOperationException>(
            () => ApiTransportSecurity.EnsureSecureBindings([binding]));
    }

    /// <summary>
    /// DE: Eine unsichere Adresse in einer kombinierten URL-Liste sperrt die gesamte Konfiguration.
    /// EN: One insecure address in a combined URL list rejects the complete configuration.
    /// </summary>
    [TestMethod]
    public void EnsureSecureBindings_MixedServerUrls_ThrowsInvalidOperationException()
    {
        Assert.ThrowsExactly<InvalidOperationException>(
            () => ApiTransportSecurity.EnsureSecureBindings(
                [],
                "https://0.0.0.0:443;http://0.0.0.0:80"));
    }

    /// <summary>
    /// DE: Benannte Kestrel-Endpunkte besitzen Vorrang vor ignorierten generischen URLs.
    /// EN: Named Kestrel endpoints take precedence over ignored generic URLs.
    /// </summary>
    [TestMethod]
    public void EnsureSecureBindings_KestrelEndpoint_PrecedesGenericUrls()
    {
        var result = ApiTransportSecurity.EnsureSecureBindings(
            ["https://0.0.0.0:443"],
            "http://0.0.0.0:80",
            "80");

        CollectionAssert.AreEqual(new[] { "https://0.0.0.0:443" }, result.ToArray());
    }

    /// <summary>
    /// DE: Eine reine HTTP-Portkonfiguration wuerde alle Schnittstellen oeffnen und wird abgewiesen.
    /// EN: A port-only HTTP configuration would open all interfaces and is rejected.
    /// </summary>
    [TestMethod]
    public void EnsureSecureBindings_HttpPortsWithoutExplicitUrl_ThrowsInvalidOperationException()
    {
        Assert.ThrowsExactly<InvalidOperationException>(
            () => ApiTransportSecurity.EnsureSecureBindings([], httpPorts: "80;8080"));
    }

    /// <summary>
    /// DE: Weist unvollstaendige oder nicht unterstuetzte Bindungsformate ab.
    /// EN: Rejects incomplete or unsupported binding formats.
    /// </summary>
    /// <param name="binding">DE: Ungueltige Bindung. EN: Invalid binding.</param>
    [TestMethod]
    [DataRow("localhost:80")]
    [DataRow("ftp://localhost:21")]
    [DataRow("https://")]
    public void EnsureSecureBindings_InvalidBinding_ThrowsInvalidOperationException(string binding)
    {
        Assert.ThrowsExactly<InvalidOperationException>(
            () => ApiTransportSecurity.EnsureSecureBindings([binding]));
    }
}
