namespace InventarWorkerCommon.Models.Settings;

/// <summary>
/// Represents the configuration settings required to connect to the Client API.
/// </summary>
public class ClientApi
{
    /// <summary>
    /// Gets the fully qualified domain name (FQDN) of the client API.
    /// </summary>
    public string ClientApiFqdn { get; set; }

    /// <summary>
    /// Gets the port of the client API.
    /// </summary>
    public string ClientApiPort { get; set; }
}