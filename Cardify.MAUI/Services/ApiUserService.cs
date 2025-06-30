using Cardify.MAUI.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cardify.MAUI.Services
{
    public class ApiUserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "http://localhost:5144/api"; 

        public ApiUserService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> CreateUserAsync(UserCreateDto userCreateDto)
        {
            var json = JsonSerializer.Serialize(userCreateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseApiUrl}/users/register", content);

            return response.IsSuccessStatusCode;
        }
    }
} 