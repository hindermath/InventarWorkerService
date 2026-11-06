using InventarWorkerCommon.Services.Network;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Prüft die Betreiber-Allowlist für Harvester-Workerziele.
/// EN: Tests the operator allowlist for harvester Worker targets.
/// </summary>
[TestClass]
[DoNotParallelize]
public class WorkerTargetAuthorizationTest
{
    private string? _previousTargets;

    /// <summary>DE: Sichert die vorhandene Allowlist. EN: Stores the existing allowlist.</summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _previousTargets = Environment.GetEnvironmentVariable(WorkerTargetAuthorization.AllowedTargetsEnvironmentVariable);
    }

    /// <summary>DE: Stellt die Allowlist wieder her. EN: Restores the allowlist.</summary>
    [TestCleanup]
    public void TestCleanup()
    {
        Environment.SetEnvironmentVariable(WorkerTargetAuthorization.AllowedTargetsEnvironmentVariable, _previousTargets);
    }

    /// <summary>DE: Ohne Allowlist wird abgewiesen. EN: Missing allowlists reject targets.</summary>
    [TestMethod]
    public void IsAuthorized_MissingAllowlist_RejectsTarget()
    {
        Environment.SetEnvironmentVariable(WorkerTargetAuthorization.AllowedTargetsEnvironmentVariable, null);

        Assert.IsFalse(WorkerTargetAuthorization.IsAuthorized("10.20.30.40", out _));
    }

    /// <summary>DE: Akzeptiert nur passende IP-/CIDR-Einträge. EN: Accepts only matching IP/CIDR entries.</summary>
    [TestMethod]
    public void IsAuthorized_ExactIpAndCidr_OnlyAcceptsConfiguredTargets()
    {
        Environment.SetEnvironmentVariable(WorkerTargetAuthorization.AllowedTargetsEnvironmentVariable, "10.20.0.0/16;2001:db8:42::/64");

        Assert.IsTrue(WorkerTargetAuthorization.IsAuthorized("10.20.30.40", out _));
        Assert.IsTrue(WorkerTargetAuthorization.IsAuthorized("2001:db8:42::7", out _));
        Assert.IsFalse(WorkerTargetAuthorization.IsAuthorized("10.21.30.40", out _));
        Assert.IsFalse(WorkerTargetAuthorization.IsAuthorized("203.0.113.7", out _));
    }

    /// <summary>DE: Lehnt Nicht-IP- und Multicastziele ab. EN: Rejects non-IP and multicast targets.</summary>
    [TestMethod]
    public void IsAuthorized_NonLiteralAndMulticastAddresses_RejectsTarget()
    {
        Environment.SetEnvironmentVariable(WorkerTargetAuthorization.AllowedTargetsEnvironmentVariable, "0.0.0.0/0;::/0");

        Assert.IsFalse(WorkerTargetAuthorization.IsAuthorized("worker.example", out _));
        Assert.IsFalse(WorkerTargetAuthorization.IsAuthorized("224.0.0.1", out _));
        Assert.IsFalse(WorkerTargetAuthorization.IsAuthorized("::", out _));
    }
}
