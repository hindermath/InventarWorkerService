using System.Net.NetworkInformation;
using InventarWorkerCommon.Helpers.Calculate;
using InventarWorkerCommon.Helpers.Exceptions;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Network;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.Settings;
using InventarWorkerCommon.Services.Paths;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Schließt kleine Coverage-Lücken bei Hilfsklassen, Pfaden und Restmodellen.
/// EN: Closes smaller coverage gaps in helper classes, path utilities and remaining models.
/// </summary>
[TestClass]
public class CoverageSupportTest
{
    /// <summary>
    /// DE: Prüft die Durchschnittsberechnung als Klassen- und Extension-Pfad.
    /// EN: Verifies average calculation through the class and extension paths.
    /// </summary>
    [TestMethod]
    public void AverageProcessingTime_CalculatesExpectedBounds()
    {
        var helper = new AverageProcessingTime();
        var startTime = DateTime.Now.AddSeconds(-2);

        var helperAverage = helper.CalculateAverageProcessingTime(2, startTime);
        var extensionAverage = 2.CalculateAverageProcessingTime(startTime);

        Assert.IsTrue(helperAverage > 0);
        Assert.IsTrue(extensionAverage > 0);
        Assert.AreEqual(0, helper.CalculateAverageProcessingTime(0, startTime));
        Assert.AreEqual(0, 0.CalculateAverageProcessingTime(startTime));
    }

    /// <summary>
    /// DE: Prüft die Netzwerk-Exceptions inklusive Fallback-Name und InnerException.
    /// EN: Verifies network exceptions including fallback name and inner exception.
    /// </summary>
    [TestMethod]
    public void NetworkInformationExceptions_SetMachineNameAndInnerException()
    {
        var inner = new InvalidOperationException("Inner");

        var missing = new NetworkInformation.NetworkInformationMissingException(null, inner);
        var unresolved = new NetworkInformation.HostNetworkInformationCannotResolveException("machine-01", inner);

        Assert.AreEqual("\"Unknown Machine Name\"", missing.MachineName);
        Assert.AreSame(inner, missing.InnerException);
        Assert.AreEqual("machine-01", unresolved.MachineName);
        Assert.AreSame(inner, unresolved.InnerException);
    }

    /// <summary>
    /// DE: Prüft Host- und Ping-Modelle auf Default- und Erfolgslogik.
    /// EN: Verifies host and ping models for default and success logic.
    /// </summary>
    [TestMethod]
    public void NetworkModels_DefaultsAndComputedState_WorkAsExpected()
    {
        var host = new HostInformationResult
        {
            HostName = "localhost",
            Aliases = ["loopback"],
            AddressList = ["127.0.0.1"],
            IPv4Addresses = ["127.0.0.1"],
            IPv6Addresses = ["::1"],
            ErrorMessage = string.Empty
        };
        var ping = new PingResult
        {
            IsSuccess = true,
            Status = IPStatus.Success,
            RoundTripTime = 1,
            Address = "127.0.0.1"
        };

        Assert.IsTrue(host.IsSuccess);
        Assert.AreEqual("loopback", host.Aliases[0]);
        Assert.IsTrue(ping.IsSuccess);
        Assert.AreEqual(IPStatus.Success, ping.Status);
    }

    /// <summary>
    /// DE: Prüft Restmodelle und Verbindungszeichenfolgen für Settings.
    /// EN: Verifies remaining models and connection strings for settings.
    /// </summary>
    [TestMethod]
    public void SettingsAndModelDefaults_RetainExpectedValues()
    {
        var pgWithAuth = new PgSqlDb
        {
            PgSqlDbFqdn = "db.local",
            PgSqlDbPort = "5432",
            PgSqlDbName = "inventar",
            PgSqlUser = "inventar",
            PgSqlPassword = "secret",
            WriteEnabled = true
        };
        var pgWithoutAuth = new PgSqlDb();
        var hardwareInfo = new HardwareInfo
        {
            NetworkInterfaces = [new NetworkInfo { Name = "en0", Speed = 1000 }],
            Software = new InventarWorkerCommon.Models.Software.SoftwareInventory()
        };
        var metrics = new PerformanceMetrics
        {
            CpuUsage = 9.5,
            MemoryUsage = 2048,
            ThreadCount = 7,
            Timestamp = DateTime.UtcNow
        };
        var serviceStatus = new ServiceStatus
        {
            State = "Running",
            StartTime = DateTime.UtcNow.AddMinutes(-1),
            LastActivity = DateTime.UtcNow,
            ProcessedItems = 3,
            LastError = string.Empty
        };

        Assert.AreEqual("Host=db.local;Port=5432;Database=inventar;Username=inventar;Password=secret;", pgWithAuth.PgSqlConnectionString);
        Assert.AreEqual("Host=localhost;Port=5432;Database=postgres;", pgWithoutAuth.PgSqlConnectionString);
        Assert.AreEqual("en0", hardwareInfo.NetworkInterfaces[0].Name);
        Assert.AreEqual(9.5, metrics.CpuUsage);
        Assert.AreEqual("Running", serviceStatus.State);
    }

    /// <summary>
    /// DE: Prüft Basis- und Servicepfade inklusive Erzeugung und Existenz.
    /// EN: Verifies base and service paths including creation and existence.
    /// </summary>
    [TestMethod]
    public void PathServices_CreateAndDetectDirectories()
    {
        var basePath = BasePath.GetBasePath();
        var serviceStatusPath = ServicePath.GetServiceStatusPath();
        var testPath = Path.Combine(serviceStatusPath, $"coverage-path-{Guid.NewGuid():N}");

        var directory = ServicePath.CreateServiceStatusPath(testPath);

        Assert.IsFalse(string.IsNullOrWhiteSpace(basePath));
        Assert.IsFalse(string.IsNullOrWhiteSpace(serviceStatusPath));
        Assert.IsTrue(ServicePath.ExistsServiceStatusPath(directory.FullName));
    }
}
