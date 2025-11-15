using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using IniParser;
using IniParser.Model;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Service;
using InventarWorkerCommon.Services.Paths;
using YamlDotNet.Serialization;

namespace InventarWorkerCommon.Services.Status;

/// <summary>
/// The ServiceStatusWriter class is responsible for managing and writing the status, logs, statistics,
/// performance metrics, and hardware inventory data of a service to files in a specified directory.
/// </summary>
public class ServiceStatusWriter
{
    private readonly string _statusDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// The <c>ServiceStatusWriter</c> class provides mechanisms for managing, serializing, and writing
    /// service-related data such as status, logs, statistics, performance metrics, and hardware inventory
    /// to a specified directory. This ensures that the service's state and activities are persistently
    /// stored for tracking, diagnostics, and operational insights.
    /// </summary>
    public ServiceStatusWriter(string statusDirectory = "inventar-service")
    {
        if (ServicePath.ExistsServiceStatusPath(Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory)) is
            false)
        {
            var directory =
                ServicePath.CreateServiceStatusPath(Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory));
            _statusDirectory = directory.FullName;
        }
        else
        {
            _statusDirectory = Path.Combine(ServicePath.GetServiceStatusPath(), statusDirectory);
        }

        _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };
    }

    /// <summary>
    /// Writes the current status of the service to the specified output format, allowing for the status
    /// to be serialized and stored or transmitted in a structured format like JSON, INI, XML, YAML, or
    /// all supported formats.
    /// </summary>
    /// <param name="status">An instance of <c>ServiceStatus</c> containing the current state and details
    /// of the service.</param>
    /// <param name="serviceStatusOutputFormat">The format in which the service status should be output.
    /// Defaults to JSON if not specified.</param>
    public void WriteStatus(ServiceStatus status,
        ServiceStatusOutputFormat serviceStatusOutputFormat = ServiceStatusOutputFormat.Json)
    {
        switch (serviceStatusOutputFormat)
        {
            case ServiceStatusOutputFormat.Json:
                WriteStatusJson(status);
                break;
            case ServiceStatusOutputFormat.Ini:
                WriteStatusIni(status);
                break;
            case ServiceStatusOutputFormat.Xml:
                WriteStatusXml(status);
                break;
            case ServiceStatusOutputFormat.Yaml:
                WriteStatusYaml(status);
                break;
            case ServiceStatusOutputFormat.All:
                WriteStatusJson(status);
                WriteStatusIni(status);
                WriteStatusXml(status);
                WriteStatusYaml(status);
                break;
            default:
                WriteStatusJson(status);
                break;
        }
    }

    private void WriteStatusYaml(ServiceStatus status)
    {
        var statusFile = Path.Combine(_statusDirectory, "status.yaml");
        
        var serializer = new SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();
        
        var yaml = serializer.Serialize(status);
        File.WriteAllText(statusFile, yaml);
    }

    private void WriteStatusXml(ServiceStatus status)
    {
        var statusFile = Path.Combine(_statusDirectory, "status.xml");
        
        var xmlSerializer = new XmlSerializer(typeof(ServiceStatus));
        var xmlSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };

        using var writer = XmlWriter.Create(statusFile, xmlSettings);
        xmlSerializer.Serialize(writer, status);
    }

    private void WriteStatusIni(ServiceStatus status)
    {
        var statusFile = Path.Combine(_statusDirectory, "status.ini");

        var parser = new FileIniDataParser();
        var iniData = new IniData();

        // Hauptsektion für Service Status
        iniData.Sections.AddSection("ServiceStatus");
        var statusSection = iniData["ServiceStatus"];

        // Grundlegende Eigenschaften mit Reflection
        var properties = typeof(ServiceStatus).GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(status);
            if (value == null) continue;

            var key = property.Name.ToLowerInvariant();

            switch (value)
            {
                case DateTime dt:
                    statusSection[key] = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case bool b:
                    statusSection[key] = b ? "true" : "false";
                    break;
                case string s:
                    statusSection[key] = s;
                    break;
                case var simple when IsSimpleType(property.PropertyType):
                    statusSection[key] = value.ToString();
                    break;
                default:
                    // Komplexe Objekte als separate Sektion behandeln
                    if (value != null)
                    {
                        AddComplexObjectSection(iniData, property.Name, value);
                    }
                    break;
            }
        }

        parser.WriteFile(statusFile, iniData);
    }

    private static void AddComplexObjectSection(IniData iniData, string sectionName, object obj)
    {
        iniData.Sections.AddSection(sectionName);
        var section = iniData[sectionName];

        var properties = obj.GetType().GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            if (value == null) continue;

            var key = property.Name.ToLowerInvariant();

            switch (value)
            {
                case DateTime dt:
                    section[key] = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case bool b:
                    section[key] = b ? "true" : "false";
                    break;
                case double d:
                    section[key] = d.ToString("F2");
                    break;
                case float f:
                    section[key] = f.ToString("F2");
                    break;
                case decimal dec:
                    section[key] = dec.ToString("F2");
                    break;
                default:
                    section[key] = value.ToString();
                    break;
            }
        }
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive ||
               type == typeof(string) ||
               type == typeof(DateTime) ||
               type == typeof(decimal) ||
               type.IsEnum ||
               (Nullable.GetUnderlyingType(type) != null && IsSimpleType(Nullable.GetUnderlyingType(type)));
    }
    /// <summary>
    /// Serializes the provided service status to JSON format and writes it to a predefined "status.json" file
    /// in the specified status directory. This method ensures the service status is stored persistently
    /// for further analysis or diagnostics.
    /// </summary>
    /// <param name="status">The <c>ServiceStatus</c> object representing the current state of the service,
    /// including details such as state, start time, last activity, processed items, and last error.</param>
    private void WriteStatusJson(ServiceStatus status)
    {
        var statusFile = Path.Combine(_statusDirectory, "status.json");
        var json = JsonSerializer.Serialize(status, _jsonOptions);
        File.WriteAllText(statusFile, json);
    }

    /// <summary>
    /// Appends a log entry to the service log file, containing a timestamp and the specified message.
    /// </summary>
    /// <param name="message">The message to be recorded in the service log.</param>
    public void WriteLog(string message)
    {
        var logFile = Path.Combine(_statusDirectory, "service.log");
        var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n";
        File.AppendAllText(logFile, logEntry);
    }

    /// <summary>
    /// The <c>WriteStatistics</c> method serializes and writes service statistics data,
    /// such as processing metrics and operational insights, to a predefined JSON file
    /// in the service's designated directory. This ensures that statistical information
    /// is properly stored and accessible for analysis.
    /// </summary>
    /// <param name="stats">An instance of the <c>ServiceStatistics</c> class containing
    /// statistical data related to the service's operations.</param>
    public void WriteStatistics(ServiceStatistics stats)
    {
        var statsFile = Path.Combine(_statusDirectory, "statistics.json");
        var json = JsonSerializer.Serialize(stats, _jsonOptions);
        File.WriteAllText(statsFile, json);
    }

    /// <summary>
    /// Writes performance metrics data to a JSON file within the service's status directory.
    /// This method serializes the provided <c>PerformanceMetrics</c> object to ensure
    /// that detailed metrics such as CPU usage, memory usage, thread count, and a timestamp are
    /// persistently stored for monitoring and diagnostics purposes.
    /// </summary>
    /// <param name="metrics">An instance of <c>PerformanceMetrics</c> containing information
    /// about CPU usage, memory usage, thread count, and the timestamp at which the metrics were captured.</param>
    public void WritePerformanceMetrics(PerformanceMetrics metrics)
    {
        var metricsFile = Path.Combine(_statusDirectory, "metrics.json");
        var json = JsonSerializer.Serialize(metrics, _jsonOptions);
        File.WriteAllText(metricsFile, json);
    }

    /// <summary>
    /// Saves the hardware inventory information to a JSON file in the specified service status directory.
    /// The file name is timestamped to ensure uniqueness and retain a historical record of hardware data.
    /// </summary>
    /// <param name="hardwareInfo">
    /// An instance of <c>HardwareInfo</c> containing details about the system's hardware,
    /// including CPU, memory, disks, network interfaces, operating system, and installed software.
    /// </param>
    /// <returns>
    /// A <c>Task</c> representing the asynchronous save operation.
    /// </returns>
    public async Task WriteHardwareInventory(HardwareInfo hardwareInfo)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var inventoryFile = Path.Combine(_statusDirectory, $"hardware_inventory_{timestamp}.json");
        var json = JsonSerializer.Serialize(hardwareInfo, _jsonOptions);
        await File.WriteAllTextAsync(inventoryFile, json);
    }
}