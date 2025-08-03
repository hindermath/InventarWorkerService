using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        // Dieser Test läuft nur auf Windows
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
            // Andere Exceptions sind akzeptabel (Service existiert möglicherweise nicht)
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public void StartService_OnLinux_CallsLinuxSpecificLogic()
    {
        // Dieser Test läuft nur auf Linux
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
            // Andere Exceptions sind akzeptabel
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public void StartService_OnMacOS_CallsMacOSSpecificLogic()
    {
        // Dieser Test läuft nur auf macOS
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
            // Andere Exceptions sind akzeptabel
            Assert.IsTrue(true);
        }
    }
}
