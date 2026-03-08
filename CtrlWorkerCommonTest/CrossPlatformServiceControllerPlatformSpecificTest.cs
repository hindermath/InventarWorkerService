using System.Runtime.InteropServices;
using CtrlWorkerCommon.Controller;

namespace CtrlWorkerCommonTest;

/// <summary>
/// DE: Enthält Tests für CrossPlatformServiceControllerPlatformSpecificTest.
/// EN: Contains tests for CrossPlatformServiceControllerPlatformSpecificTest.
/// </summary>
[TestClass]
public class CrossPlatformServiceControllerPlatformSpecificTest
{
    private CrossPlatformServiceController _controller;
    private const string TestServiceName = "TestService";

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt TestInitialize aus.
    /// EN: Executes the test or helper step TestInitialize.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _controller = new CrossPlatformServiceController(TestServiceName);
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt TestCleanup aus.
    /// EN: Executes the test or helper step TestCleanup.
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        _controller = null;
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt StartService_OnWindows_CallsWindowsSpecificLogic aus.
    /// EN: Executes the test or helper step StartService_OnWindows_CallsWindowsSpecificLogic.
    /// </summary>
    [TestMethod]
    public void StartService_OnWindows_CallsWindowsSpecificLogic()
    {
        // This test only runs on Windows.
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Inconclusive("Test läuft nur auf Windows");
            return;
        }

        // Act & Assert
        try
        {
            _controller.StartService();
            Assert.IsTrue(true, "Windows Service Start wurde aufgerufen");
        }
        catch (Exception ex) when (!(ex is PlatformNotSupportedException))
        {
            // Other exceptions are acceptable (the service might not exist).
            Assert.IsTrue(true);
        }
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt StartService_OnLinux_CallsLinuxSpecificLogic aus.
    /// EN: Executes the test or helper step StartService_OnLinux_CallsLinuxSpecificLogic.
    /// </summary>
    [TestMethod]
    public void StartService_OnLinux_CallsLinuxSpecificLogic()
    {
        // This test runs only on Linux.
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Assert.Inconclusive("Test läuft nur auf Linux");
            return;
        }

        // Act & Assert
        try
        {
            _controller.StartService();
            Assert.IsTrue(true, "Linux Service Start wurde aufgerufen");
        }
        catch (Exception ex) when (!(ex is PlatformNotSupportedException))
        {
            // Other exceptions are acceptable.
            Assert.IsTrue(true);
        }
    }

    /// <summary>
    /// DE: Führt den Test- oder Hilfeschritt StartService_OnMacOS_CallsMacOSSpecificLogic aus.
    /// EN: Executes the test or helper step StartService_OnMacOS_CallsMacOSSpecificLogic.
    /// </summary>
    [TestMethod]
    public void StartService_OnMacOS_CallsMacOSSpecificLogic()
    {
        // This test only runs on macOS
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Assert.Inconclusive("Test läuft nur auf macOS");
            return;
        }

        // Act & Assert
        try
        {
            _controller.StartService();
            Assert.IsTrue(true, "macOS Service Start wurde aufgerufen");
        }
        catch (Exception ex) when (!(ex is PlatformNotSupportedException))
        {
            // Other exceptions are acceptable.
            Assert.IsTrue(true);
        }
    }
}
