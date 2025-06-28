using Cardify.MAUI.Services;
using Cardify.MAUI.Views;
using Cardify.MAUI.Models;
using Microsoft.Maui.Controls.Shapes;

namespace Cardify.MAUI.Views
{
    public partial class DashboardView : ContentView
    {
        private readonly ApiDashboardService _dashboardService;

        public DashboardView()
        {
            InitializeComponent();
            _dashboardService = new ApiDashboardService();
        }

        public async Task LoadData()
        {
            try
            {
                var dashboardData = await _dashboardService.GetDashboardDataAsync();
                if (dashboardData != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Dashboard loaded: {dashboardData.ActiveCards} cards, {dashboardData.RecentTransactions.Count} transactions");
                    UpdateStats(dashboardData);
                    PopulateTransactions(dashboardData.RecentTransactions);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Dashboard data is null");
                    ShowEmptyState();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard load error: {ex.Message}");
                ShowEmptyState();
            }
        }

        private void UpdateStats(ApiDashboardService.DashboardData data)
        {
            TotalCardsLabel.Text = data.ActiveCards.ToString();
            TotalBalanceLabel.Text = $"${data.TotalBalance:N2}";
            MonthlyIncomeLabel.Text = $"${data.MonthlyIncome:N2}";
            MonthlyExpensesLabel.Text = $"${data.MonthlyExpenses:N2}";
        }

        private void PopulateTransactions(List<ApiDashboardService.TransactionData> transactions)
        {
            TransactionsContainer.Children.Clear();
            
            System.Diagnostics.Debug.WriteLine($"PopulateTransactions called with {transactions?.Count ?? 0} transactions");

            if (!transactions.Any())
            {
                System.Diagnostics.Debug.WriteLine("No transactions to display");
                var emptyLabel = new Label
                {
                    Text = "No transactions yet. Add some transactions to see them here.",
                    TextColor = Color.FromArgb("#6B7280"),
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                TransactionsContainer.Children.Add(emptyLabel);
                return;
            }

            foreach (var transactionData in transactions.Take(5)) // Show only 5 most recent
            {
                System.Diagnostics.Debug.WriteLine($"Processing transaction: {transactionData.Description} - {transactionData.Amount}");
                
                var transaction = new Transaction
                {
                    Id = transactionData.Id,
                    Name = transactionData.Description,
                    Amount = (decimal)transactionData.Amount,
                    Type = transactionData.Category,
                    Date = transactionData.TransactionDate,
                    ShowActions = false // Hide actions in dashboard
                };
                
                var transactionCard = new TransactionCardTemplate
                {
                    BindingContext = transaction
                };
                TransactionsContainer.Children.Add(transactionCard);
            }
        }

        private void ShowEmptyState()
        {
            TotalCardsLabel.Text = "0";
            TotalBalanceLabel.Text = "$0.00";
            MonthlyIncomeLabel.Text = "$0.00";
            MonthlyExpensesLabel.Text = "$0.00";
            
            TransactionsContainer.Children.Clear();
            
            var emptyLabel = new Label
            {
                Text = "No data available. Add some cards and transactions to see your dashboard.",
                TextColor = Color.FromArgb("#6B7280"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            TransactionsContainer.Children.Add(emptyLabel);
        }
    }
} 