using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Software;
using RestSharp;

namespace InventarWorkerCommon.Services.Api
{
    /// <summary>
    /// DE: Kapselt den Zugriff auf die Inventar-API und stellt deserialisierte Ergebnisobjekte bereit.
    /// EN: Encapsulates access to the inventory API and returns deserialized result objects.
    /// </summary>
    public class ApiService
    {
        private readonly RestClient _client;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string? _apiKey;

        /// <summary>
        /// DE: Initialisiert den Dienst mit der Basisadresse der Ziel-API.
        /// EN: Initializes the service with the base address of the target API.
        /// </summary>
        /// <param name="baseUrl">
        /// DE: Basis-URL der Backend-API.
        /// EN: Base URL of the backend API.
        /// </param>
        /// <param name="apiKey">DE: Optionaler API-Schlüssel; er wird nie in Fehlermeldungen ausgegeben. EN: Optional API key; it is never included in error messages.</param>
        public ApiService(string baseUrl, string? apiKey = null)
        {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
            {
                throw new ArgumentException("DE: Die API-Basisadresse ist ungültig. EN: The API base address is invalid.", nameof(baseUrl));
            }

            // DE: Unverschlüsseltes HTTP bleibt auf Loopback-Entwicklung begrenzt, damit
            // entfernte Inventardaten und Credentials nicht versehentlich offen übertragen werden.
            // EN: Plain HTTP remains limited to loopback development so remote inventory
            // data and credentials cannot accidentally travel without transport protection.
            if (baseUri.Scheme != Uri.UriSchemeHttps &&
                !(baseUri.Scheme == Uri.UriSchemeHttp && baseUri.IsLoopback))
            {
                throw new ArgumentException(
                    "DE: Entfernte API-Ziele müssen HTTPS verwenden. EN: Remote API targets must use HTTPS.",
                    nameof(baseUrl));
            }

            _client = new RestClient(new RestClientOptions(baseUri)
            {
                Timeout = TimeSpan.FromSeconds(30)
            });
            _apiKey = string.IsNullOrWhiteSpace(apiKey) ? null : apiKey;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
        }

        /// <summary>
        /// DE: Lädt den aktuellen Servicestatus von der Backend-API.
        /// EN: Retrieves the current service status from the backend API.
        /// </summary>
        /// <returns>
        /// DE: Deserialisierte JSON-Antwort des Status-Endpunkts.
        /// EN: Deserialized JSON response from the status endpoint.
        /// </returns>
        /// <param name="cancellationToken">DE: Bricht den ausgehenden Aufruf ab. EN: Cancels the outbound request.</param>
        /// <exception cref="Exception">
        /// DE: Wird ausgelöst, wenn der API-Aufruf fehlschlägt oder kein verwertbarer Inhalt zurückkommt.
        /// EN: Thrown when the API call fails or no usable response content is returned.
        /// </exception>
        /// <exception cref="OperationCanceledException">DE: Der Aufruf wurde abgebrochen. EN: The request was cancelled.</exception>
        public async Task<object> GetServiceStatusAsync(CancellationToken cancellationToken = default)
        {
            var request = CreateRequest("api/inventar/status");
            var response = await _client.ExecuteAsync(request, cancellationToken);

            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                return JsonSerializer.Deserialize<object>(response.Content, _jsonOptions)
                    ?? throw new Exception("API-Fehler: Status-Antwort konnte nicht deserialisiert werden.");
            }

            throw CreateExternalFailure();
        }

        /// <summary>
        /// DE: Lädt Hardware-Inventardaten von der Backend-API.
        /// EN: Retrieves hardware inventory data from the backend API.
        /// </summary>
        /// <returns>
        /// DE: Deserialisiertes Hardware-Inventarobjekt.
        /// EN: Deserialized hardware inventory object.
        /// </returns>
        /// <param name="cancellationToken">DE: Bricht den ausgehenden Aufruf ab. EN: Cancels the outbound request.</param>
        /// <exception cref="Exception">
        /// DE: Wird ausgelöst, wenn der API-Aufruf fehlschlägt oder kein verwertbarer Inhalt zurückkommt.
        /// EN: Thrown when the API call fails or no usable response content is returned.
        /// </exception>
        /// <exception cref="OperationCanceledException">DE: Der Aufruf wurde abgebrochen. EN: The request was cancelled.</exception>
        public async Task<HardwareInventory> GetHardwareInventoryAsync(CancellationToken cancellationToken = default)
        {
            var request = CreateRequest("api/inventar/hardware");
            var response = await _client.ExecuteAsync(request, cancellationToken);

            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                return JsonSerializer.Deserialize<HardwareInventory>(response.Content, _jsonOptions)
                    ?? throw new Exception("API-Fehler: Hardware-Antwort konnte nicht deserialisiert werden.");
            }

            throw CreateExternalFailure();
        }

        /// <summary>
        /// DE: Lädt Software-Inventardaten von der Backend-API.
        /// EN: Retrieves software inventory data from the backend API.
        /// </summary>
        /// <returns>
        /// DE: Deserialisiertes Software-Inventarobjekt.
        /// EN: Deserialized software inventory object.
        /// </returns>
        /// <param name="cancellationToken">DE: Bricht den ausgehenden Aufruf ab. EN: Cancels the outbound request.</param>
        /// <exception cref="Exception">
        /// DE: Wird ausgelöst, wenn der API-Aufruf fehlschlägt oder kein verwertbarer Inhalt zurückkommt.
        /// EN: Thrown when the API call fails or no usable response content is returned.
        /// </exception>
        /// <exception cref="OperationCanceledException">DE: Der Aufruf wurde abgebrochen. EN: The request was cancelled.</exception>
        public async Task<SoftwareInventory> GetSoftwareInventoryAsync(CancellationToken cancellationToken = default)
        {
            var request = CreateRequest("api/inventar/software");
            var response = await _client.ExecuteAsync(request, cancellationToken);

            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                return JsonSerializer.Deserialize<SoftwareInventory>(response.Content, _jsonOptions)
                    ?? throw new Exception("API-Fehler: Software-Antwort konnte nicht deserialisiert werden.");
            }

            throw CreateExternalFailure();
        }

        /// <summary>
        /// DE: Lädt das vollständige Inventar von der Backend-API.
        /// EN: Retrieves the full inventory from the backend API.
        /// </summary>
        /// <returns>
        /// DE: Deserialisierte JSON-Antwort mit kombiniertem Inventar.
        /// EN: Deserialized JSON response with combined inventory data.
        /// </returns>
        /// <param name="cancellationToken">DE: Bricht den ausgehenden Aufruf ab. EN: Cancels the outbound request.</param>
        /// <exception cref="Exception">
        /// DE: Wird ausgelöst, wenn der API-Aufruf fehlschlägt oder kein verwertbarer Inhalt zurückkommt.
        /// EN: Thrown when the API call fails or no usable response content is returned.
        /// </exception>
        /// <exception cref="OperationCanceledException">DE: Der Aufruf wurde abgebrochen. EN: The request was cancelled.</exception>
        public async Task<object> GetFullInventoryAsync(CancellationToken cancellationToken = default)
        {
            var request = CreateRequest("api/inventar/full");
            var response = await _client.ExecuteAsync(request, cancellationToken);

            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                return JsonSerializer.Deserialize<object>(response.Content, _jsonOptions)
                    ?? throw new Exception("API-Fehler: Vollständige Antwort konnte nicht deserialisiert werden.");
            }

            throw CreateExternalFailure();
        }

        private RestRequest CreateRequest(string resource)
        {
            var request = new RestRequest(resource, Method.Get);
            if (_apiKey is not null)
            {
                request.AddHeader("X-Inventory-Api-Key", _apiKey);
            }

            return request;
        }

        private static InvalidOperationException CreateExternalFailure() =>
            new("DE: Die Inventar-API ist derzeit nicht erreichbar. EN: The inventory API is currently unavailable.");
    }
}
