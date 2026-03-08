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

        /// <summary>
        /// DE: Initialisiert den Dienst mit der Basisadresse der Ziel-API.
        /// EN: Initializes the service with the base address of the target API.
        /// </summary>
        /// <param name="baseUrl">
        /// DE: Basis-URL der Backend-API.
        /// EN: Base URL of the backend API.
        /// </param>
        public ApiService(string baseUrl)
        {
            _client = new RestClient(baseUrl);
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
        /// <exception cref="Exception">
        /// DE: Wird ausgelöst, wenn der API-Aufruf fehlschlägt oder kein verwertbarer Inhalt zurückkommt.
        /// EN: Thrown when the API call fails or no usable response content is returned.
        /// </exception>
        public async Task<object> GetServiceStatusAsync()
        {
            var request = new RestRequest("api/inventar/status", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                return JsonSerializer.Deserialize<object>(response.Content, _jsonOptions)
                    ?? throw new Exception("API-Fehler: Status-Antwort konnte nicht deserialisiert werden.");
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }

        /// <summary>
        /// DE: Lädt Hardware-Inventardaten von der Backend-API.
        /// EN: Retrieves hardware inventory data from the backend API.
        /// </summary>
        /// <returns>
        /// DE: Deserialisiertes Hardware-Inventarobjekt.
        /// EN: Deserialized hardware inventory object.
        /// </returns>
        /// <exception cref="Exception">
        /// DE: Wird ausgelöst, wenn der API-Aufruf fehlschlägt oder kein verwertbarer Inhalt zurückkommt.
        /// EN: Thrown when the API call fails or no usable response content is returned.
        /// </exception>
        public async Task<HardwareInventory> GetHardwareInventoryAsync()
        {
            var request = new RestRequest("api/inventar/hardware", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                return JsonSerializer.Deserialize<HardwareInventory>(response.Content, _jsonOptions)
                    ?? throw new Exception("API-Fehler: Hardware-Antwort konnte nicht deserialisiert werden.");
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }

        /// <summary>
        /// DE: Lädt Software-Inventardaten von der Backend-API.
        /// EN: Retrieves software inventory data from the backend API.
        /// </summary>
        /// <returns>
        /// DE: Deserialisiertes Software-Inventarobjekt.
        /// EN: Deserialized software inventory object.
        /// </returns>
        /// <exception cref="Exception">
        /// DE: Wird ausgelöst, wenn der API-Aufruf fehlschlägt oder kein verwertbarer Inhalt zurückkommt.
        /// EN: Thrown when the API call fails or no usable response content is returned.
        /// </exception>
        public async Task<SoftwareInventory> GetSoftwareInventoryAsync()
        {
            var request = new RestRequest("api/inventar/software", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                return JsonSerializer.Deserialize<SoftwareInventory>(response.Content, _jsonOptions)
                    ?? throw new Exception("API-Fehler: Software-Antwort konnte nicht deserialisiert werden.");
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }

        /// <summary>
        /// DE: Lädt das vollständige Inventar von der Backend-API.
        /// EN: Retrieves the full inventory from the backend API.
        /// </summary>
        /// <returns>
        /// DE: Deserialisierte JSON-Antwort mit kombiniertem Inventar.
        /// EN: Deserialized JSON response with combined inventory data.
        /// </returns>
        /// <exception cref="Exception">
        /// DE: Wird ausgelöst, wenn der API-Aufruf fehlschlägt oder kein verwertbarer Inhalt zurückkommt.
        /// EN: Thrown when the API call fails or no usable response content is returned.
        /// </exception>
        public async Task<object> GetFullInventoryAsync()
        {
            var request = new RestRequest("api/inventar/full", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                return JsonSerializer.Deserialize<object>(response.Content, _jsonOptions)
                    ?? throw new Exception("API-Fehler: Vollständige Antwort konnte nicht deserialisiert werden.");
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }
    }
}
