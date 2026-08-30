using InventarWorkerCommon.Services.Api;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Prüft sichere Zielgrenzen des ausgehenden API-Dienstes.
/// EN: Verifies secure target boundaries of the outbound API service.
/// </summary>
[TestClass]
public sealed class ApiServiceSecurityTest
{
    /// <summary>
    /// DE: Weist unverschlüsselte entfernte Ziele vor dem Netzwerkzugriff ab.
    /// EN: Rejects unencrypted remote targets before network access.
    /// </summary>
    [TestMethod]
    public void Constructor_RemoteHttpTarget_ThrowsArgumentException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new ApiService("http://example.com"));
    }

    /// <summary>
    /// DE: Erlaubt HTTP nur für explizite Loopback-Entwicklung.
    /// EN: Allows HTTP only for explicit loopback development.
    /// </summary>
    [TestMethod]
    public void Constructor_LoopbackHttpTarget_IsAllowed()
    {
        _ = new ApiService("http://127.0.0.1:5000");
    }
}
