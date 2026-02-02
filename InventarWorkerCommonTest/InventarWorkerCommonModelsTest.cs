using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Models.Software;

namespace InventarWorkerCommonTest;

[TestClass]
public class CpuInfoTests
{
    private CpuInfo? _testCpuInfo;

    [TestInitialize]
    public void TestInitialize()
    {
        // Wird vor jedem Test ausgeführt
        _testCpuInfo = new CpuInfo();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Wird nach jedem Test ausgeführt
        _testCpuInfo = null;
    }

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        // Wird einmal vor allen Tests in dieser Klasse ausgeführt
        System.Diagnostics.Debug.WriteLine("CpuInfoTests - Klassen-Initialisierung");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Wird einmal nach allen Tests in dieser Klasse ausgeführt
        System.Diagnostics.Debug.WriteLine("CpuInfoTests - Klassen-Bereinigung");
    }

    [TestMethod]
    public void CpuInfo_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var cpuInfo = new CpuInfo();
        
        // Assert
        Assert.AreEqual(0, cpuInfo.ProcessorCount);
        Assert.AreEqual(string.Empty, cpuInfo.ProcessorName);
        Assert.AreEqual(string.Empty, cpuInfo.Architecture);
        Assert.AreEqual(0.0, cpuInfo.CurrentUsage);
    }

    [TestMethod]
    public void CpuInfo_SetAllProperties_ShouldRetainValues()
    {
        // Arrange
        var cpuInfo = new CpuInfo
        {
            ProcessorCount = 8,
            ProcessorName = "Intel Core i7-12700K",
            Architecture = "x64",
            CurrentUsage = 67.5
        };
        
        // Assert
        Assert.AreEqual(8, cpuInfo.ProcessorCount);
        Assert.AreEqual("Intel Core i7-12700K", cpuInfo.ProcessorName);
        Assert.AreEqual("x64", cpuInfo.Architecture);
        Assert.AreEqual(67.5, cpuInfo.CurrentUsage);
    }

    [TestMethod]
    public void CpuInfo_RecordEquality_ShouldWork()
    {
        // Arrange
        var cpuInfo1 = new CpuInfo { ProcessorCount = 4, ProcessorName = "AMD Ryzen 5" };
        var cpuInfo2 = new CpuInfo { ProcessorCount = 4, ProcessorName = "AMD Ryzen 5" };
        
        // Assert
        Assert.AreEqual(cpuInfo1, cpuInfo2);
    }
}

[TestClass]
public class OsInfoTests
{
    private OsInfo? _testOsInfo;

    [TestInitialize]
    public void TestInitialize()
    {
        // Wird vor jedem Test ausgeführt
        _testOsInfo = new OsInfo();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Wird nach jedem Test ausgeführt
        _testOsInfo = null;
    }

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        // Wird einmal vor allen Tests in dieser Klasse ausgeführt
        System.Diagnostics.Debug.WriteLine("OsInfoTests - Klassen-Initialisierung");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Wird einmal nach allen Tests in dieser Klasse ausgeführt
        System.Diagnostics.Debug.WriteLine("OsInfoTests - Klassen-Bereinigung");
    }

    [TestMethod]
    public void OsInfo_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var osInfo = new OsInfo();
        
        // Assert
        Assert.AreEqual(string.Empty, osInfo.Platform);
        Assert.AreEqual(string.Empty, osInfo.Version);
        Assert.AreEqual(string.Empty, osInfo.ServicePack);
        Assert.AreEqual(string.Empty, osInfo.Description);
        Assert.AreEqual(string.Empty, osInfo.Architecture);
        Assert.IsFalse(osInfo.Is64Bit);
        Assert.AreEqual(0, osInfo.ProcessorCount);
        Assert.AreEqual(string.Empty, osInfo.UserDomainName);
    }

    [TestMethod]
    public void OsInfo_SetAllProperties_ShouldRetainValues()
    {
        // Arrange
        var osInfo = new OsInfo
        {
            Platform = "Win32NT",
            Version = "10.0.22000",
            ServicePack = "Service Pack 1",
            Description = "Microsoft Windows 11 Professional",
            Architecture = "AMD64",
            Is64Bit = true,
            ProcessorCount = 16,
            UserDomainName = "WORKGROUP"
        };
        
        // Assert
        Assert.AreEqual("Win32NT", osInfo.Platform);
        Assert.AreEqual("10.0.22000", osInfo.Version);
        Assert.AreEqual("Service Pack 1", osInfo.ServicePack);
        Assert.AreEqual("Microsoft Windows 11 Professional", osInfo.Description);
        Assert.AreEqual("AMD64", osInfo.Architecture);
        Assert.IsTrue(osInfo.Is64Bit);
        Assert.AreEqual(16, osInfo.ProcessorCount);
        Assert.AreEqual("WORKGROUP", osInfo.UserDomainName);
    }

    [TestMethod]
    public void OsInfo_BooleanProperty_ShouldToggle()
    {
        // Arrange
        var osInfo = new OsInfo();
        
        // Act & Assert
        Assert.IsFalse(osInfo.Is64Bit);
        osInfo.Is64Bit = true;
        Assert.IsTrue(osInfo.Is64Bit);
    }
}

[TestClass]
public class ServiceStatisticsTests
{
    private ServiceStatistics? _testServiceStats;

    [TestInitialize]
    public void TestInitialize()
    {
        // Wird vor jedem Test ausgeführt
        _testServiceStats = new ServiceStatistics();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Wird nach jedem Test ausgeführt
        _testServiceStats = null;
    }

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        // Wird einmal vor allen Tests in dieser Klasse ausgeführt
        System.Diagnostics.Debug.WriteLine("ServiceStatisticsTests - Klassen-Initialisierung");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Wird einmal nach allen Tests in dieser Klasse ausgeführt
        System.Diagnostics.Debug.WriteLine("ServiceStatisticsTests - Klassen-Bereinigung");
    }

    [TestMethod]
    public void ServiceStatistics_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var stats = new ServiceStatistics();
        
        // Assert
        Assert.AreEqual(0, stats.TotalProcessedItems);
        Assert.AreEqual(0.0, stats.AverageProcessingTime);
        Assert.AreEqual(TimeSpan.Zero, stats.Uptime);
        Assert.AreEqual(0L, stats.MemoryUsage);
    }

    [TestMethod]
    public void ServiceStatistics_InitProperties_ShouldRetainValues()
    {
        // Arrange
        var uptime = TimeSpan.FromHours(24);
        var stats = new ServiceStatistics
        {
            TotalProcessedItems = 1500,
            AverageProcessingTime = 125.75,
            Uptime = uptime,
            MemoryUsage = 2048576L
        };
        
        // Assert
        Assert.AreEqual(1500, stats.TotalProcessedItems);
        Assert.AreEqual(125.75, stats.AverageProcessingTime);
        Assert.AreEqual(uptime, stats.Uptime);
        Assert.AreEqual(2048576L, stats.MemoryUsage);
    }

    [TestMethod]
    public void ServiceStatistics_RecordEquality_ShouldWork()
    {
        // Arrange
        var uptime = TimeSpan.FromMinutes(30);
        var stats1 = new ServiceStatistics
        {
            TotalProcessedItems = 100,
            AverageProcessingTime = 50.0,
            Uptime = uptime,
            MemoryUsage = 1024L
        };
        var stats2 = new ServiceStatistics
        {
            TotalProcessedItems = 100,
            AverageProcessingTime = 50.0,
            Uptime = uptime,
            MemoryUsage = 1024L
        };
        
        // Assert
        Assert.AreEqual(stats1, stats2);
    }
}

[TestClass]
public class SystemInfoTests
{
    private SystemInfo? _testSystemInfo;

    [TestInitialize]
    public void TestInitialize()
    {
        // Wird vor jedem Test ausgeführt
        _testSystemInfo = new SystemInfo();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Wird nach jedem Test ausgeführt
        _testSystemInfo = null;
    }

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        // Wird einmal vor allen Tests in dieser Klasse ausgeführt
        System.Diagnostics.Debug.WriteLine("SystemInfoTests - Klassen-Initialisierung");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Wird einmal nach allen Tests in dieser Klasse ausgeführt
        System.Diagnostics.Debug.WriteLine("SystemInfoTests - Klassen-Bereinigung");
    }

    [TestMethod]
    public void SystemInfo_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var systemInfo = new SystemInfo();
        
        // Assert
        Assert.AreEqual(string.Empty, systemInfo.MachineName);
        Assert.AreEqual(string.Empty, systemInfo.UserName);
        Assert.AreEqual(string.Empty, systemInfo.Domain);
        Assert.AreEqual(TimeSpan.Zero, systemInfo.Uptime);
        Assert.AreEqual(string.Empty, systemInfo.Platform);
        Assert.AreEqual(string.Empty, systemInfo.Architecture);
    }

    [TestMethod]
    public void SystemInfo_SetAllProperties_ShouldRetainValues()
    {
        // Arrange
        var uptime = TimeSpan.FromDays(7);
        var systemInfo = new SystemInfo
        {
            MachineName = "DESKTOP-ABC123",
            UserName = "testuser",
            Domain = "TESTDOMAIN",
            Uptime = uptime,
            Platform = "Win32NT",
            Architecture = "AMD64"
        };
        
        // Assert
        Assert.AreEqual("DESKTOP-ABC123", systemInfo.MachineName);
        Assert.AreEqual("testuser", systemInfo.UserName);
        Assert.AreEqual("TESTDOMAIN", systemInfo.Domain);
        Assert.AreEqual(uptime, systemInfo.Uptime);
        Assert.AreEqual("Win32NT", systemInfo.Platform);
        Assert.AreEqual("AMD64", systemInfo.Architecture);
    }

    [TestMethod]
    public void SystemInfo_UptimeProperty_ShouldHandleDifferentTimeSpans()
    {
        // Arrange
        var systemInfo = new SystemInfo();
        var shortUptime = TimeSpan.FromMinutes(30);
        var longUptime = TimeSpan.FromDays(365);
        
        // Act & Assert
        systemInfo.Uptime = shortUptime;
        Assert.AreEqual(shortUptime, systemInfo.Uptime);
        
        systemInfo.Uptime = longUptime;
        Assert.AreEqual(longUptime, systemInfo.Uptime);
    }
}

[TestClass]
public class SoftwareInfoTests
{
    private SoftwareInfo? _testSoftwareInfo;

    [TestInitialize]
    public void TestInitialize()
    {
        // Wird vor jedem Test ausgeführt
        _testSoftwareInfo = new SoftwareInfo();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Wird nach jedem Test ausgeführt
        _testSoftwareInfo = null;
    }

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        // Wird einmal vor allen Tests in dieser Klasse ausgeführt
        System.Diagnostics.Debug.WriteLine("SoftwareInfoTests - Klassen-Initialisierung");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Wird einmal nach allen Tests in dieser Klasse ausgeführt
        System.Diagnostics.Debug.WriteLine("SoftwareInfoTests - Klassen-Bereinigung");
    }

    [TestMethod]
    public void SoftwareInfo_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var softwareInfo = new SoftwareInfo();
        
        // Assert
        Assert.AreEqual(string.Empty, softwareInfo.Name);
        Assert.AreEqual(string.Empty, softwareInfo.Version);
        Assert.AreEqual(string.Empty, softwareInfo.Publisher);
        Assert.IsNull(softwareInfo.InstallDate);
        Assert.AreEqual(string.Empty, softwareInfo.InstallLocation);
        Assert.IsNull(softwareInfo.Size);
        Assert.AreEqual(string.Empty, softwareInfo.UninstallString);
        Assert.AreEqual(string.Empty, softwareInfo.DisplayIcon);
        Assert.IsFalse(softwareInfo.IsSystemComponent);
        Assert.AreEqual(string.Empty, softwareInfo.Architecture);
    }

    [TestMethod]
    public void SoftwareInfo_SetAllProperties_ShouldRetainValues()
    {
        // Arrange
        var installDate = new DateTime(2023, 6, 15);
        var softwareInfo = new SoftwareInfo
        {
            Name = "Visual Studio Code",
            Version = "1.80.0",
            Publisher = "Microsoft Corporation",
            InstallDate = installDate,
            InstallLocation = @"C:\Users\TestUser\AppData\Local\Programs\Microsoft VS Code",
            Size = 234567890L,
            UninstallString = @"C:\Users\TestUser\AppData\Local\Programs\Microsoft VS Code\unins000.exe",
            DisplayIcon = @"C:\Users\TestUser\AppData\Local\Programs\Microsoft VS Code\Code.exe",
            IsSystemComponent = false,
            Architecture = "x64"
        };
        
        // Assert
        Assert.AreEqual("Visual Studio Code", softwareInfo.Name);
        Assert.AreEqual("1.80.0", softwareInfo.Version);
        Assert.AreEqual("Microsoft Corporation", softwareInfo.Publisher);
        Assert.AreEqual(installDate, softwareInfo.InstallDate);
        Assert.AreEqual(@"C:\Users\TestUser\AppData\Local\Programs\Microsoft VS Code", softwareInfo.InstallLocation);
        Assert.AreEqual(234567890L, softwareInfo.Size);
        Assert.AreEqual(@"C:\Users\TestUser\AppData\Local\Programs\Microsoft VS Code\unins000.exe", softwareInfo.UninstallString);
        Assert.AreEqual(@"C:\Users\TestUser\AppData\Local\Programs\Microsoft VS Code\Code.exe", softwareInfo.DisplayIcon);
        Assert.IsFalse(softwareInfo.IsSystemComponent);
        Assert.AreEqual("x64", softwareInfo.Architecture);
    }

    [TestMethod]
    public void SoftwareInfo_NullableProperties_ShouldHandleNullValues()
    {
        // Arrange
        var softwareInfo = new SoftwareInfo
        {
            Name = "Test Software",
            InstallDate = null,
            Size = null
        };
        
        // Assert
        Assert.AreEqual("Test Software", softwareInfo.Name);
        Assert.IsNull(softwareInfo.InstallDate);
        Assert.IsNull(softwareInfo.Size);
    }

    [TestMethod]
    public void SoftwareInfo_SystemComponentFlag_ShouldToggle()
    {
        // Arrange
        var softwareInfo = new SoftwareInfo();
        
        // Act & Assert
        Assert.IsFalse(softwareInfo.IsSystemComponent);
        softwareInfo.IsSystemComponent = true;
        Assert.IsTrue(softwareInfo.IsSystemComponent);
    }

    [TestMethod]
    public void SoftwareInfo_RecordEquality_ShouldWork()
    {
        // Arrange
        var installDate = DateTime.Now;
        var software1 = new SoftwareInfo
        {
            Name = "Test App",
            Version = "1.0.0",
            InstallDate = installDate
        };
        var software2 = new SoftwareInfo
        {
            Name = "Test App",
            Version = "1.0.0",
            InstallDate = installDate
        };
        
        // Assert
        Assert.AreEqual(software1, software2);
    }
}

[TestClass]
public class DiskInfoTests
{
    [TestMethod]
    public void DiskInfo_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var diskInfo = new DiskInfo();

        // Assert
        Assert.AreEqual(string.Empty, diskInfo.DriveName);
        Assert.AreEqual(string.Empty, diskInfo.DriveType);
        Assert.AreEqual(string.Empty, diskInfo.FileSystem);
        Assert.AreEqual(0, diskInfo.TotalSize);
        Assert.AreEqual(0, diskInfo.AvailableSpace);
        Assert.AreEqual(0, diskInfo.UsedSpace);
        Assert.AreEqual(0.0, diskInfo.UsagePercentage);
        Assert.IsFalse(diskInfo.IsReady);
    }

    [TestMethod]
    public void DiskInfo_SetAllProperties_ShouldRetainValues()
    {
        // Arrange
        var diskInfo = new DiskInfo
        {
            DriveName = "C:",
            DriveType = "Fixed",
            FileSystem = "NTFS",
            TotalSize = 1000,
            AvailableSpace = 400,
            UsedSpace = 600,
            UsagePercentage = 60.0,
            IsReady = true
        };

        // Assert
        Assert.AreEqual("C:", diskInfo.DriveName);
        Assert.AreEqual("Fixed", diskInfo.DriveType);
        Assert.AreEqual("NTFS", diskInfo.FileSystem);
        Assert.AreEqual(1000, diskInfo.TotalSize);
        Assert.AreEqual(400, diskInfo.AvailableSpace);
        Assert.AreEqual(600, diskInfo.UsedSpace);
        Assert.AreEqual(60.0, diskInfo.UsagePercentage);
        Assert.IsTrue(diskInfo.IsReady);
    }

    [TestMethod]
    public void DiskInfo_RecordEquality_ShouldWork()
    {
        // Arrange
        var disk1 = new DiskInfo { DriveName = "C:", TotalSize = 100 };
        var disk2 = new DiskInfo { DriveName = "C:", TotalSize = 100 };

        // Assert
        Assert.AreEqual(disk1, disk2);
    }
}