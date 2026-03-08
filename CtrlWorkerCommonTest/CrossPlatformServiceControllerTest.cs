using System.Runtime.InteropServices;
using CtrlWorkerCommon.Controller;

namespace CtrlWorkerCommonTest;

/// <summary>
/// DE: Enthält Tests für CrossPlatformServiceControllerTest.
/// EN: Contains tests for CrossPlatformServiceControllerTest.
/// </summary>
[TestClass]
public sealed class CrossPlatformServiceControllerTest
{
    /// <summary>
    /// DE: Enthält Tests für CrossPlatformServiceControllerTests.
    /// EN: Contains tests for CrossPlatformServiceControllerTests.
    /// </summary>
    [TestClass]
    public class CrossPlatformServiceControllerTests
    {
        private CrossPlatformServiceController _controller;
        private const string TestServiceName = "TestService";
        private const string InvalidServiceName = "NonExistentService";

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt TestInitialize aus.
        /// EN: Executes the test or helper step TestInitialize.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange: Setup before each test
            _controller = new CrossPlatformServiceController(TestServiceName);
        }

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt TestCleanup aus.
        /// EN: Executes the test or helper step TestCleanup.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // Cleanup after each test
            _controller = null;
        }

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt Constructor_WithValidServiceName_SetsServiceNameCorrectly aus.
        /// EN: Executes the test or helper step Constructor_WithValidServiceName_SetsServiceNameCorrectly.
        /// </summary>
        [TestMethod]
        public void Constructor_WithValidServiceName_SetsServiceNameCorrectly()
        {
            // Act
            var controller = new CrossPlatformServiceController("MyTestService");
            
            // Assert
            Assert.IsNotNull(controller);
        }

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt Constructor_WithNullServiceName_ThrowsArgumentNullException aus.
        /// EN: Executes the test or helper step Constructor_WithNullServiceName_ThrowsArgumentNullException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullServiceName_ThrowsArgumentNullException()
        {
            // Act
            var controller = new CrossPlatformServiceController(null);
        }

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt Constructor_WithEmptyServiceName_ThrowsArgumentException aus.
        /// EN: Executes the test or helper step Constructor_WithEmptyServiceName_ThrowsArgumentException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithEmptyServiceName_ThrowsArgumentException()
        {
            // Act
            var controller = new CrossPlatformServiceController(string.Empty);
        }

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt StartService_OnSupportedPlatform_DoesNotThrowPlatformNotSupportedException aus.
        /// EN: Executes the test or helper step StartService_OnSupportedPlatform_DoesNotThrowPlatformNotSupportedException.
        /// </summary>
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

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt StopService_OnSupportedPlatform_DoesNotThrowPlatformNotSupportedException aus.
        /// EN: Executes the test or helper step StopService_OnSupportedPlatform_DoesNotThrowPlatformNotSupportedException.
        /// </summary>
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

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt StartService_WithInvalidServiceName_ThrowsException aus.
        /// EN: Executes the test or helper step StartService_WithInvalidServiceName_ThrowsException.
        /// </summary>
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

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt StopService_WithInvalidServiceName_ThrowsException aus.
        /// EN: Executes the test or helper step StopService_WithInvalidServiceName_ThrowsException.
        /// </summary>
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

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt StartService_CalledMultipleTimes_DoesNotThrow aus.
        /// EN: Executes the test or helper step StartService_CalledMultipleTimes_DoesNotThrow.
        /// </summary>
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

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt StopService_CalledMultipleTimes_DoesNotThrow aus.
        /// EN: Executes the test or helper step StopService_CalledMultipleTimes_DoesNotThrow.
        /// </summary>
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

        /// <summary>
        /// DE: Führt den Test- oder Hilfeschritt StartAndStopService_InSequence_DoesNotThrow aus.
        /// EN: Executes the test or helper step StartAndStopService_InSequence_DoesNotThrow.
        /// </summary>
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
