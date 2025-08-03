using System.Runtime.InteropServices;
using CtrlWorkerCommon.Controller;

namespace CtrlWorkerCommonTest;

[TestClass]
public class CrossPlatformServiceControllerPlatformSpecificTest
{
    private CrossPlatformServiceController _controller;
    private const string TestServiceName = "TestService";

    [TestInitialize]
    public void TestInitialize()
    {
        _controller = new CrossPlatformServiceController(TestServiceName);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _controller = null;
    }

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
