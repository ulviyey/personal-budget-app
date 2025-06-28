using Cardify.MAUI.Services;
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
                    UpdateStats(dashboardData);
                    PopulateTransactions(dashboardData.RecentTransactions);
                }
                else
                {
                    ShowEmptyState();
                }
            }
            catch (Exception ex)
            {
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

            if (!transactions.Any())
            {
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

            foreach (var transaction in transactions.Take(5)) // Show only 5 most recent
            {
                var transactionCard = CreateTransactionCard(transaction);
                TransactionsContainer.Children.Add(transactionCard);
            }
        }

        private Border CreateTransactionCard(ApiDashboardService.TransactionData transaction)
        {
            bool isIncome = transaction.Amount > 0;
            var amountColor = isIncome ? Color.FromArgb("#059669") : Color.FromArgb("#DC2626");

            var card = new Border
            {
                Background = Color.FromArgb("#F9FAFB"),
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(6) },
                Padding = 12,
                StrokeThickness = 0
            };

            var layout = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var descriptionLayout = new VerticalStackLayout
            {
                Spacing = 4
            };
            descriptionLayout.Add(new Label
            {
                Text = transaction.Description,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#1F2937")
            });
            descriptionLayout.Add(new Label
            {
                Text = $"{transaction.Category} • {transaction.TransactionDate}",
                FontSize = 12,
                TextColor = Color.FromArgb("#6B7280")
            });

            var amountLabel = new Label
            {
                Text = $"${Math.Abs(transaction.Amount):N2}",
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = amountColor,
                VerticalOptions = LayoutOptions.Center
            };

            Grid.SetColumn(descriptionLayout, 0);
            Grid.SetColumn(amountLabel, 1);

            layout.Add(descriptionLayout);
            layout.Add(amountLabel);

            card.Content = layout;
            return card;
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