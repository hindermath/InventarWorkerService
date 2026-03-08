namespace InventarWorkerCommon.Models.Settings;

/// <summary>
/// DE: Enthält die Verbindungsparameter für die Client-API.
/// EN: Contains connection parameters for the client API.
/// </summary>
public class ClientApi
{

    /// <summary>
    /// DE: Vollqualifizierter Domänenname (FQDN) der Client-API.
    /// EN: Fully qualified domain name (FQDN) of the client API.
    /// </summary>
    public string ClientApiFqdn { get; set; } = string.Empty;

    /// <summary>
    /// DE: Port der Client-API.
    /// EN: Port of the client API.
    /// </summary>
    public string ClientApiPort { get; set; } = string.Empty;

    /// <summary>
    /// DE: Vollständige URL der Client-API, zusammengesetzt aus FQDN und Port.
    /// EN: Full client API URL, composed from FQDN and port.
    /// </summary>
    public string ClientApiUrl
    {
        get
        {
            var host = string.IsNullOrWhiteSpace(ClientApiFqdn) ? "localhost" : ClientApiFqdn;
            var port = string.IsNullOrWhiteSpace(ClientApiPort) ? "80" : ClientApiPort;
            return $"http://{host}:{port}";
        }
    }
}
