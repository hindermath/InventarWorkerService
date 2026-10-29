using System.Net;

namespace InventarWorkerCommon.Services.Api;

/// <summary>
/// DE: Erzwingt sichere Listener-Grenzen fuer die Inventar-APIs.
/// EN: Enforces secure listener boundaries for the inventory APIs.
/// </summary>
public static class ApiTransportSecurity
{
    /// <summary>
    /// DE: Prueft die wirksamen Serverbindungen, bevor Kestrel einen Port oeffnet.
    /// HTTP ist nur fuer Loopback erlaubt; entfernte oder Wildcard-Bindungen muessen HTTPS verwenden.
    /// EN: Validates effective server bindings before Kestrel opens a port.
    /// HTTP is allowed only for loopback; remote or wildcard bindings must use HTTPS.
    /// </summary>
    /// <param name="kestrelEndpointUrls">
    /// DE: URLs aus <c>Kestrel:Endpoints:*:Url</c>. EN: URLs from <c>Kestrel:Endpoints:*:Url</c>.
    /// </param>
    /// <param name="serverUrls">
    /// DE: Alternative URL-Liste aus <c>urls</c> oder dem Viewer-Legacyschluessel <c>Url</c>.
    /// EN: Alternative URL list from <c>urls</c> or the Viewer legacy key <c>Url</c>.
    /// </param>
    /// <param name="httpPorts">
    /// DE: Optionale HTTP-Portliste. Sie bindet ohne explizite URL an alle Schnittstellen und wird deshalb abgewiesen.
    /// EN: Optional HTTP port list. Without an explicit URL it binds to all interfaces and is therefore rejected.
    /// </param>
    /// <returns>DE: Die geprueften wirksamen URLs. EN: The validated effective URLs.</returns>
    /// <exception cref="InvalidOperationException">DE: Eine Bindung verletzt die Transportgrenze. EN: A binding violates the transport boundary.</exception>
    public static IReadOnlyList<string> EnsureSecureBindings(
        IEnumerable<string?> kestrelEndpointUrls,
        string? serverUrls = null,
        string? httpPorts = null)
    {
        ArgumentNullException.ThrowIfNull(kestrelEndpointUrls);

        var endpointUrls = kestrelEndpointUrls
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .SelectMany(SplitBindings)
            .ToArray();

        // DE: Benannte Kestrel-Endpunkte haben Vorrang vor generischen URL- und Portwerten.
        // EN: Named Kestrel endpoints take precedence over generic URL and port values.
        if (endpointUrls.Length > 0)
        {
            ValidateBindings(endpointUrls);
            return endpointUrls;
        }

        var fallbackUrls = SplitBindings(serverUrls).ToArray();
        if (fallbackUrls.Length > 0)
        {
            ValidateBindings(fallbackUrls);
            return fallbackUrls;
        }

        if (!string.IsNullOrWhiteSpace(httpPorts))
        {
            throw CreateInsecureBindingException($"HTTP_PORTS={httpPorts.Trim()}");
        }

        // DE: Kestrels Standard ohne Konfiguration ist eine lokale localhost-Bindung.
        // EN: Kestrel's default without configuration is a local localhost binding.
        return Array.Empty<string>();
    }

    private static IEnumerable<string> SplitBindings(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Array.Empty<string>()
            : value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static void ValidateBindings(IEnumerable<string> bindings)
    {
        foreach (var binding in bindings)
        {
            if (IsHttpsBinding(binding))
            {
                continue;
            }

            if (IsLoopbackHttpBinding(binding))
            {
                continue;
            }

            throw CreateInsecureBindingException(binding);
        }
    }

    private static bool IsHttpsBinding(string binding)
    {
        if (!binding.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Kestrel erlaubt '*' und '+' als Wildcard-Hosts, System.Uri jedoch nicht.
        if (binding.StartsWith("https://*:", StringComparison.OrdinalIgnoreCase) ||
            binding.StartsWith("https://+:", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return Uri.TryCreate(binding, UriKind.Absolute, out var uri) &&
               uri.Scheme == Uri.UriSchemeHttps &&
               !string.IsNullOrWhiteSpace(uri.Host);
    }

    private static bool IsLoopbackHttpBinding(string binding)
    {
        if (!Uri.TryCreate(binding, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttp)
        {
            return false;
        }

        if (string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var host = uri.Host.Trim('[', ']');
        return IPAddress.TryParse(host, out var address) && IPAddress.IsLoopback(address);
    }

    private static InvalidOperationException CreateInsecureBindingException(string binding) =>
        new(
            $"DE: Unsichere API-Bindung '{binding}'. HTTP ist nur auf Loopback erlaubt; " +
            "entfernte oder Wildcard-Bindungen muessen HTTPS verwenden. " +
            "EN: Insecure API binding. HTTP is allowed only on loopback; " +
            "remote or wildcard bindings must use HTTPS.");
}
