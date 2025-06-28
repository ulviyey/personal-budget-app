using Cardify.Core.Services;
using System.Text;
using System.Text.Json;

namespace Cardify.MAUI.Services
{
    public class ApiLoginService : ILoginService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:5144/api";

        // Static properties to store user data (simple approach for one-off project)
        public static int? CurrentUserId { get; set; }
        public static string? CurrentUsername { get; set; }

        public ApiLoginService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new
                {
                    usernameOrEmail = email,
                    password = password
                };

                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/users/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LoginResponse>(responseContent);
                    
                    // Store user info for later use
                    if (result?.user != null)
                    {
                        CurrentUserId = result.user.id;
                        CurrentUsername = result.user.username;
                    }
                    
                    return true;
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }

        private class LoginResponse
        {
            public string message { get; set; } = string.Empty;
            public UserInfo? user { get; set; }
        }

        private class UserInfo
        {
            public int id { get; set; }
            public string username { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
        }
    }
} 