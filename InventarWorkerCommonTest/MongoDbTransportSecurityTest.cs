using InventarWorkerCommon.Models.Settings;
using InventarWorkerCommon.Services.Database;
using MongoDB.Driver;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Prueft die MongoDB-Transportgrenze fuer lokale und entfernte Ziele.
/// EN: Verifies the MongoDB transport boundary for local and remote targets.
/// </summary>
[TestClass]
public sealed class MongoDbTransportSecurityTest
{
    /// <summary>
    /// DE: Erhaelt unverschluesselte Verbindungen zu eindeutigem Loopback.
    /// EN: Preserves plaintext connections to explicit loopback targets.
    /// </summary>
    /// <param name="connectionString">DE: Lokale Verbindung. EN: Local connection.</param>
    [TestMethod]
    [DataRow("mongodb://localhost:27017")]
    [DataRow("mongodb://LOCALHOST:27017/?tls=false")]
    [DataRow("mongodb://127.42.7.9:27017")]
    [DataRow("mongodb://[::1]:27017")]
    [DataRow("mongodb://[::ffff:127.0.0.1]:27017")]
    public void CreateClientSettings_LoopbackWithoutTls_IsAllowed(string connectionString)
    {
        var settings = MongoDbTransportSecurity.CreateClientSettings(connectionString);

        Assert.IsFalse(settings.UseTls);
        Assert.IsTrue(settings.DirectConnection);
    }

    /// <summary>
    /// DE: Erlaubt entfernte Verbindungen nur mit TLS und Zertifikatspruefung.
    /// EN: Allows remote connections only with TLS and certificate validation.
    /// </summary>
    /// <param name="connectionString">DE: Sichere entfernte Verbindung. EN: Secure remote connection.</param>
    [TestMethod]
    [DataRow("mongodb://inventory.example.test:27017/?tls=true")]
    [DataRow("mongodb://localhost:27017,inventory.example.test:27017/?tls=true")]
    [DataRow("mongodb+srv://inventory.example.test/")]
    public void CreateClientSettings_RemoteTls_IsAllowed(string connectionString)
    {
        var settings = MongoDbTransportSecurity.CreateClientSettings(connectionString);

        Assert.IsTrue(settings.UseTls);
        Assert.IsFalse(settings.AllowInsecureTls);
    }

    /// <summary>
    /// DE: Weist entfernte Klartextverbindungen und gemischte Seeds ab.
    /// EN: Rejects remote plaintext connections and mixed seeds.
    /// </summary>
    /// <param name="connectionString">DE: Unsichere entfernte Verbindung. EN: Insecure remote connection.</param>
    [TestMethod]
    [DataRow("mongodb://inventory.example.test:27017")]
    [DataRow("mongodb://inventory.example.test:27017/?tls=false")]
    [DataRow("mongodb://localhost:27017,inventory.example.test:27017")]
    [DataRow("mongodb://0.0.0.0:27017")]
    [DataRow("mongodb://[::]:27017")]
    [DataRow("mongodb+srv://inventory.example.test/?tls=false")]
    public void CreateClientSettings_RemoteWithoutTls_ThrowsInvalidOperationException(string connectionString)
    {
        Assert.ThrowsExactly<InvalidOperationException>(
            () => MongoDbTransportSecurity.CreateClientSettings(connectionString));
    }

    /// <summary>
    /// DE: Weist Remote-TLS ohne normale Zertifikatspruefung ab.
    /// EN: Rejects remote TLS without normal certificate validation.
    /// </summary>
    /// <param name="connectionString">DE: Unsichere TLS-Option. EN: Insecure TLS option.</param>
    [TestMethod]
    [DataRow("mongodb://inventory.example.test:27017/?tls=true&tlsInsecure=true")]
    [DataRow("mongodb://inventory.example.test:27017/?ssl=true&sslVerifyCertificate=false")]
    public void CreateClientSettings_RemoteInsecureTls_ThrowsInvalidOperationException(string connectionString)
    {
        Assert.ThrowsExactly<InvalidOperationException>(
            () => MongoDbTransportSecurity.CreateClientSettings(connectionString));
    }

    /// <summary>
    /// DE: Nicht unterstützte Invalid-Optionen dürfen die wirksame Zertifikatsprüfung nicht abschalten.
    /// EN: Unsupported invalid-certificate options must not disable effective certificate validation.
    /// </summary>
    /// <param name="connectionString">DE: Remote-TLS mit nicht unterstützter Option. EN: Remote TLS with unsupported option.</param>
    [TestMethod]
    [DataRow("mongodb://inventory.example.test:27017/?tls=true&tlsAllowInvalidCertificates=true")]
    [DataRow("mongodb://inventory.example.test:27017/?tls=true&tlsAllowInvalidHostnames=true")]
    public void CreateClientSettings_UnsupportedInvalidCertificateOption_RemainsSecure(string connectionString)
    {
        var settings = MongoDbTransportSecurity.CreateClientSettings(connectionString);

        Assert.IsTrue(settings.UseTls);
        Assert.IsFalse(settings.AllowInsecureTls);
        Assert.IsNull(settings.SslSettings.ServerCertificateValidationCallback);
    }

    /// <summary>
    /// DE: Klartext darf nicht durch Replica-Set-Erkennung auf entfernte Mitglieder expandieren.
    /// EN: Plaintext must not expand to remote members through replica-set discovery.
    /// </summary>
    /// <param name="connectionString">DE: Topologiefähige lokale Verbindung. EN: Topology-capable local connection.</param>
    [TestMethod]
    [DataRow("mongodb://localhost:27017/?replicaSet=rs0")]
    [DataRow("mongodb://localhost:27017,127.0.0.1:27018")]
    public void CreateClientSettings_LoopbackTopologyWithoutTls_ThrowsInvalidOperationException(string connectionString)
    {
        Assert.ThrowsExactly<InvalidOperationException>(
            () => MongoDbTransportSecurity.CreateClientSettings(connectionString));
    }

    /// <summary>
    /// DE: Das Settings-Modell erzeugt fuer entfernte Ziele TLS und escaped Credentials.
    /// EN: The settings model enables TLS for remote targets and escapes credentials.
    /// </summary>
    [TestMethod]
    public void MongoDbConnectionString_RemoteTarget_UsesTlsAndEscapesCredentials()
    {
        var model = new MongoDb
        {
            MongoDbFqdn = "inventory.example.test",
            MongoDbPort = "27017",
            MongoDbUser = "user@example",
            MongoDbPassword = "p@ss:/?#[]"
        };

        var url = new MongoUrl(model.MongoDbConnectionString);

        Assert.IsTrue(url.UseTls);
        Assert.AreEqual("user@example", url.Username);
        Assert.AreEqual("p@ss:/?#[]", url.Password);
        Assert.AreEqual("inventory.example.test", url.Server.Host);
    }

    /// <summary>
    /// DE: Das Settings-Modell bewahrt lokale MongoDB ohne TLS.
    /// EN: The settings model preserves local MongoDB without TLS.
    /// </summary>
    [TestMethod]
    public void MongoDbConnectionString_LoopbackTarget_RemainsPlaintext()
    {
        var model = new MongoDb
        {
            MongoDbFqdn = "::1",
            MongoDbPort = "27017"
        };

        var url = new MongoUrl(model.MongoDbConnectionString);

        Assert.IsFalse(url.UseTls);
        Assert.AreEqual("[::1]", url.Server.Host);
        Assert.IsTrue(url.DirectConnection);
    }

    /// <summary>
    /// DE: Port-Injektionen werden vor dem Erzeugen der URI abgewiesen.
    /// EN: Port injection is rejected before the URI is created.
    /// </summary>
    [TestMethod]
    public void CreateConnectionString_InjectedPort_ThrowsInvalidOperationException()
    {
        Assert.ThrowsExactly<InvalidOperationException>(
            () => MongoDbTransportSecurity.CreateConnectionString(
                "inventory.example.test",
                "27017/?tls=false"));
    }

    /// <summary>
    /// DE: Host-Injektionen werden vor dem Erzeugen der URI abgewiesen.
    /// EN: Host injection is rejected before the URI is created.
    /// </summary>
    [TestMethod]
    public void CreateConnectionString_InjectedHost_ThrowsInvalidOperationException()
    {
        Assert.ThrowsExactly<InvalidOperationException>(
            () => MongoDbTransportSecurity.CreateConnectionString(
                "inventory.example.test/?tls=false",
                "27017"));
    }

    /// <summary>
    /// DE: Parserfehler geben keine eingebetteten Credentials aus.
    /// EN: Parser failures do not disclose embedded credentials.
    /// </summary>
    [TestMethod]
    public void CreateClientSettings_InvalidConnectionString_DoesNotDiscloseCredentials()
    {
        const string password = "not-for-diagnostics";

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => MongoDbTransportSecurity.CreateClientSettings(
                $"mongodb://inventory-user:{password}@"));

        Assert.DoesNotContain(password, exception.ToString(), StringComparison.Ordinal);
    }

    /// <summary>
    /// DE: Der oeffentliche Dienstkonstruktor kann die Transportgrenze nicht umgehen.
    /// EN: The public service constructor cannot bypass the transport boundary.
    /// </summary>
    [TestMethod]
    public void MongoDbService_RemotePlaintext_ThrowsInvalidOperationException()
    {
        Assert.ThrowsExactly<InvalidOperationException>(
            () => new MongoDbService("mongodb://inventory.example.test:27017", writeEnabled: false));
    }
}
