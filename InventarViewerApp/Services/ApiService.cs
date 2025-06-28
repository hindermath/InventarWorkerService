using Newtonsoft.Json;
using RestSharp;
using InventarViewerApp.Models;

namespace InventarViewerApp.Services
{
    public class ApiService
    {
        private readonly RestClient _client;

        public ApiService(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        public async Task<object> GetServiceStatusAsync()
        {
            var request = new RestRequest("api/inventar/status", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<object>(response.Content);
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }

        public async Task<List<HardwareInventory>> GetHardwareInventoryAsync()
        {
            var request = new RestRequest("api/inventar/hardware", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<List<HardwareInventory>>(response.Content);
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }

        public async Task<List<SoftwareInventory>> GetSoftwareInventoryAsync()
        {
            var request = new RestRequest("api/inventar/software", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<List<SoftwareInventory>>(response.Content);
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }

        public async Task<object> GetFullInventoryAsync()
        {
            var request = new RestRequest("api/inventar/full", Method.Get);
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<object>(response.Content);
            }
            
            throw new Exception($"API-Fehler: {response.ErrorMessage}");
        }
    }
}