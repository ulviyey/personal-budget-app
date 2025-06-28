using System.Text.Json;
using Cardify.MAUI.Models;

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

                System.Diagnostics.Debug.WriteLine($"Dashboard service: Fetching data for user {userId}");

                // Fetch cards and transactions in parallel
                var cardsTask = GetCardsAsync(userId.Value);
                var transactionsTask = GetTransactionsAsync(userId.Value);

                await Task.WhenAll(cardsTask, transactionsTask);

                var cards = await cardsTask;
                var transactions = await transactionsTask;

                if (cards == null || transactions == null)
                {
                    System.Diagnostics.Debug.WriteLine("Dashboard service: Failed to fetch data");
                    return null;
                }

                // Calculate dashboard stats
                var dashboardData = CalculateDashboardStats(cards, transactions);
                return dashboardData;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard service exception: {ex.Message}");
                return null;
            }
        }

        private async Task<List<Card>?> GetCardsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/cards?userId={userId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Card>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching cards: {ex.Message}");
            }
            return null;
        }

        private async Task<List<Transaction>?> GetTransactionsAsync(int userId)
        {
            try
            {
                // Get all transactions for total balance calculation
                var response = await _httpClient.GetAsync($"{_baseUrl}/transactions?userId={userId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Transaction>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching transactions: {ex.Message}");
            }
            return null;
        }

        private DashboardData CalculateDashboardStats(List<Card> cards, List<Transaction> transactions)
        {
            var activeCards = cards.Count;
            
            System.Diagnostics.Debug.WriteLine($"Dashboard calculation - Total transactions: {transactions.Count}, Cards: {activeCards}");
            
            // Calculate total balance from all transactions
            // For now, we'll calculate it as income - expenses
            var totalIncome = transactions.Where(t => t.Type == "income").Sum(t => t.Amount);
            var totalExpenses = transactions.Where(t => t.Type == "expense").Sum(t => t.Amount);
            var totalBalance = totalIncome - totalExpenses;

            System.Diagnostics.Debug.WriteLine($"Dashboard calculation - Total income: {totalIncome}, Total expenses: {totalExpenses}, Balance: {totalBalance}");

            // Calculate monthly income and expenses
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            
            var monthlyTransactions = transactions.Where(t => 
                t.Date.Month == currentMonth && t.Date.Year == currentYear).ToList();

            var monthlyIncome = monthlyTransactions.Where(t => t.Type == "income").Sum(t => t.Amount);
            var monthlyExpenses = monthlyTransactions.Where(t => t.Type == "expense").Sum(t => t.Amount);

            System.Diagnostics.Debug.WriteLine($"Dashboard calculation - Monthly transactions: {monthlyTransactions.Count}, Income: {monthlyIncome}, Expenses: {monthlyExpenses}");

            // Get recent transactions (last 5)
            var recentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(5)
                .Select(t => new TransactionData
                {
                    Id = t.Id,
                    Description = t.Name,
                    Amount = (double)t.Amount,
                    TransactionDate = t.Date,
                    Category = t.Type
                })
                .ToList();

            return new DashboardData
            {
                TotalBalance = (double)totalBalance,
                MonthlyIncome = (double)monthlyIncome,
                MonthlyExpenses = (double)monthlyExpenses,
                ActiveCards = activeCards,
                RecentTransactions = recentTransactions
            };
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
            public DateTime TransactionDate { get; set; }
            public string Category { get; set; } = string.Empty;
        }
    }
} 