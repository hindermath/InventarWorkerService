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

    /// <summary>
    /// Gets the full URL of the client API, constructed using the FQDN and port information.
    /// </summary>
    public string ClientApiUrl
    {
        get
        {
            var host = string.IsNullOrWhiteSpace(ClientApiFqdn) ? "localhost" : ClientApiFqdn;
            var port = string.IsNullOrWhiteSpace(ClientApiPort) ? "80" : ClientApiPort;
            return $"http://{host}:{port}/api";
        }
    }
}