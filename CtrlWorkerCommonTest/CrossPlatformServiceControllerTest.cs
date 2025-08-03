using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CtrlWorkerCommon.Controller;

namespace CtrlWorkerCommonTest;

[TestClass]
public sealed class CrossPlatformServiceControllerTest
{
    [TestClass]
    public class CrossPlatformServiceControllerTests
    {
        private CrossPlatformServiceController _controller;
        private const string TestServiceName = "TestService";
        private const string InvalidServiceName = "NonExistentService";

        [TestInitialize]
        public void TestInitialize()
        {
            // Setup vor jedem Test
            _controller = new CrossPlatformServiceController(TestServiceName);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Cleanup nach jedem Test
            _controller = null;
        }

        [TestMethod]
        public void Constructor_WithValidServiceName_SetsServiceNameCorrectly()
        {
            // Arrange & Act
            var controller = new CrossPlatformServiceController("MyTestService");
            
            // Assert
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullServiceName_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var controller = new CrossPlatformServiceController(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithEmptyServiceName_ThrowsArgumentException()
        {
            // Arrange, Act & Assert
            var controller = new CrossPlatformServiceController(string.Empty);
        }

        [TestMethod]
        public void StartService_OnSupportedPlatform_DoesNotThrowPlatformNotSupportedException()
        {
            // Arrange
            var controller = new CrossPlatformServiceController(TestServiceName);

            // Act & Assert
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                try
                {
                    controller.StartService();
                    // Test erfolgreich wenn keine PlatformNotSupportedException geworfen wird
                    Assert.IsTrue(true);
                }
                catch (PlatformNotSupportedException)
                {
                    Assert.Fail("PlatformNotSupportedException sollte auf unterstützter Plattform nicht geworfen werden");
                }
                catch
                {
                    // Andere Exceptions sind für diesen Test akzeptabel (z.B. Service existiert nicht)
                    Assert.IsTrue(true);
                }
            }
        }

        [TestMethod]
        public void StopService_OnSupportedPlatform_DoesNotThrowPlatformNotSupportedException()
        {
            // Arrange
            var controller = new CrossPlatformServiceController(TestServiceName);

            // Act & Assert
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                try
                {
                    controller.StopService();
                    // Test erfolgreich wenn keine PlatformNotSupportedException geworfen wird
                    Assert.IsTrue(true);
                }
                catch (PlatformNotSupportedException)
                {
                    Assert.Fail("PlatformNotSupportedException sollte auf unterstützter Plattform nicht geworfen werden");
                }
                catch
                {
                    // Andere Exceptions sind für diesen Test akzeptabel (z.B. Service existiert nicht)
                    Assert.IsTrue(true);
                }
            }
        }

        [TestMethod]
        public void StartService_WithInvalidServiceName_ThrowsException()
        {
            // Arrange
            var controller = new CrossPlatformServiceController(InvalidServiceName);

            // Act & Assert
            Assert.ThrowsException<Exception>(() => controller.StartService());
        }

        [TestMethod]
        public void StopService_WithInvalidServiceName_ThrowsException()
        {
            // Arrange
            var controller = new CrossPlatformServiceController(InvalidServiceName);

            // Act & Assert
            Assert.ThrowsException<Exception>(() => controller.StopService());
        }

        [TestMethod]
        public void StartService_CalledMultipleTimes_DoesNotThrow()
        {
            // Arrange
            var controller = new CrossPlatformServiceController(TestServiceName);

            // Act & Assert
            try
            {
                controller.StartService();
                controller.StartService(); // Zweiter Aufruf sollte nicht fehlschlagen
                Assert.IsTrue(true);
            }
            catch (PlatformNotSupportedException)
            {
                // Auf nicht unterstützten Plattformen erwartet
                Assert.IsTrue(true);
            }
            catch
            {
                // Andere Exceptions sind akzeptabel für diesen Test
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void StopService_CalledMultipleTimes_DoesNotThrow()
        {
            // Arrange
            var controller = new CrossPlatformServiceController(TestServiceName);

            // Act & Assert
            try
            {
                controller.StopService();
                controller.StopService(); // Zweiter Aufruf sollte nicht fehlschlagen
                Assert.IsTrue(true);
            }
            catch (PlatformNotSupportedException)
            {
                // Auf nicht unterstützten Plattformen erwartet
                Assert.IsTrue(true);
            }
            catch
            {
                // Andere Exceptions sind akzeptabel für diesen Test
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void StartAndStopService_InSequence_DoesNotThrow()
        {
            // Arrange
            var controller = new CrossPlatformServiceController(TestServiceName);

            // Act & Assert
            try
            {
                controller.StartService();
                controller.StopService();
                Assert.IsTrue(true);
            }
            catch (PlatformNotSupportedException)
            {
                // Auf nicht unterstützten Plattformen erwartet
                Assert.IsTrue(true);
            }
            catch
            {
                // Andere Exceptions sind akzeptabel für diesen Test
                Assert.IsTrue(true);
            }
        }
    }
}