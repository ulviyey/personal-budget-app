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
                    return null;
                }

                var response = await _httpClient.GetAsync($"{_baseUrl}/dashboard/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<DashboardData>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result;
                }
                
                return null;
            }
            catch
            {
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
            public List<CardData> Cards { get; set; } = new();
        }

        public class TransactionData
        {
            public int Id { get; set; }
            public string Description { get; set; } = string.Empty;
            public double Amount { get; set; }
            public string TransactionDate { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
        }

        public class CardData
        {
            public int Id { get; set; }
            public string CardNumber { get; set; } = string.Empty;
            public string CardHolder { get; set; } = string.Empty;
            public string CardType { get; set; } = string.Empty;
        }
    }
} 