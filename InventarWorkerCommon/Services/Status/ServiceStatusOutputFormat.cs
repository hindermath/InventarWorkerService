namespace InventarWorkerCommon.Services.Status;

/// <summary>
/// Specifies the output format for service status data.
/// </summary>
public enum ServiceStatusOutputFormat
{
    /// <summary>
    /// Specifies that the service status data should be formatted as JSON.
    /// </summary>
    Json,

    /// <summary>
    /// Indicates that the service status data should be formatted as INI.
    /// </summary>
    Ini,

    /// <summary>
    /// Specifies that the service status data should be formatted as XML.
    /// </summary>
    Xml,

    /// <summary>
    /// Specifies that the service status data should be formatted as YAML.
    /// </summary>
    Yaml,

    /// <summary>
    /// Specifies that the service status data should include all available formats.
    /// </summary>
    All
}