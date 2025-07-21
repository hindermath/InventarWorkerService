using System.Text.Json;
using System.Text.Json.Serialization;
using InventarWorkerCommon.Models.Hardware;
using InventarWorkerCommon.Models.Software;
using RestSharp;

namespace InventarViewerApp.Services
{
    public class ApiService
    {
        private readonly RestClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(string baseUrl)
        {
            _client = new RestClient(baseUrl);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
        }

        public async Task<object> GetServiceStatusAsync()
        {
            var request = new RestRequest("api/inventar/status", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful)
            {
                return JsonSerializer.Deserialize<object>(response.Content, _jsonOptions);
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }

        public async Task<HardwareInventory> GetHardwareInventoryAsync()
        {
            var request = new RestRequest("api/inventar/hardware", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful)
            {
                return JsonSerializer.Deserialize<HardwareInventory>(response.Content, _jsonOptions);
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }

        public async Task<SoftwareInventory> GetSoftwareInventoryAsync()
        {
            var request = new RestRequest("api/inventar/software", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful)
            {
                return JsonSerializer.Deserialize<SoftwareInventory>(response.Content, _jsonOptions);
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }

        public async Task<object> GetFullInventoryAsync()
        {
            var request = new RestRequest("api/inventar/full", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful)
            {
                return JsonSerializer.Deserialize<object>(response.Content, _jsonOptions);
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }
    }
}