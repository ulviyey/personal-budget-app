using System.Text.Json;

namespace Cardify.MAUI.Services
{
    public class ApiDashboardService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:5144/api";

        public ApiDashboardService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<DashboardData?> GetDashboardDataAsync()
        {
            try
            {
                var userId = ApiLoginService.CurrentUserId;
                if (userId == null)
                {
                    System.Diagnostics.Debug.WriteLine("Dashboard service: User not logged in");
                    return null;
                }

                System.Diagnostics.Debug.WriteLine($"Dashboard service: Calling API for user {userId}");
                var response = await _httpClient.GetAsync($"{_baseUrl}/dashboard?userId={userId}");
                
                System.Diagnostics.Debug.WriteLine($"Dashboard service: Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Dashboard service: Response content: {responseContent}");
                    
                    var result = JsonSerializer.Deserialize<DashboardData>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Dashboard service: Error response: {errorContent}");
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard service exception: {ex.Message}");
                return null;
            }
        }

        public class DashboardData
        {
            public double TotalBalance { get; set; }
            public double MonthlyIncome { get; set; }
            public double MonthlyExpenses { get; set; }
            public int ActiveCards { get; set; }
            public List<TransactionData> RecentTransactions { get; set; } = new();
        }

        public class TransactionData
        {
            public int Id { get; set; }
            public string Description { get; set; } = string.Empty;
            public double Amount { get; set; }
            public string TransactionDate { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
        }
    }
} 