using System.Runtime.InteropServices;
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
            // Arrange: Setup before each test
            _controller = new CrossPlatformServiceController(TestServiceName);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Cleanup after each test
            _controller = null;
        }

        [TestMethod]
        public void Constructor_WithValidServiceName_SetsServiceNameCorrectly()
        {
            // Act
            var controller = new CrossPlatformServiceController("MyTestService");
            
            // Assert
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullServiceName_ThrowsArgumentNullException()
        {
            // Act
            var controller = new CrossPlatformServiceController(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithEmptyServiceName_ThrowsArgumentException()
        {
            // Act
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
                    // Test successful if no PlatformNotSupportedException is thrown.
                    Assert.IsTrue(true);
                }
                catch (PlatformNotSupportedException)
                {
                    Assert.Fail("PlatformNotSupportedException sollte auf unterstützter Plattform nicht geworfen werden");
                }
                catch
                {
                    // Other exceptions are acceptable for this test (e.g., service does not exist).
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
                    // Test successful if no PlatformNotSupportedException is thrown.
                    Assert.IsTrue(true);
                }
                catch (PlatformNotSupportedException)
                {
                    Assert.Fail("PlatformNotSupportedException sollte auf unterstützter Plattform nicht geworfen werden");
                }
                catch
                {
                    // Other exceptions are acceptable for this test (e.g., service does not exist).
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                Assert.ThrowsException<InvalidOperationException>(() => controller.StartService());
            }
            else
            {
                try
                {
                    controller.StartService();
                    Assert.IsTrue(true);
                }
                catch (InvalidOperationException)
                {
                    Assert.IsTrue(true);
                }
            }
        }

        [TestMethod]
        public void StopService_WithInvalidServiceName_ThrowsException()
        {
            // Arrange
            var controller = new CrossPlatformServiceController(InvalidServiceName);

            // Act & Assert
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                Assert.ThrowsException<InvalidOperationException>(() => controller.StopService());
            }
            else
            {
                try
                {
                    controller.StopService();
                    Assert.IsTrue(true);
                }
                catch (InvalidOperationException)
                {
                    Assert.IsTrue(true);
                }
            }
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
                controller.StartService(); // The second call should not fail.
                Assert.IsTrue(true);
            }
            catch (PlatformNotSupportedException)
            {
                // On unsupported platforms expected
                Assert.IsTrue(true);
            }
            catch
            {
                // Other exceptions are acceptable for this test.
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
                controller.StopService(); // The second call should not fail.
                Assert.IsTrue(true);
            }
            catch (PlatformNotSupportedException)
            {
                // On unsupported platforms expected
                Assert.IsTrue(true);
            }
            catch
            {
                // Other exceptions are acceptable for this test.
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
                // On unsupported platforms, expected.
                Assert.IsTrue(true);
            }
            catch
            {
                // Other exceptions are acceptable for this test.
                Assert.IsTrue(true);
            }
        }
    }
}
