using Microsoft.Maui.Controls.Shapes;
using System.Globalization;

namespace Cardify.MAUI.Views
{
    public class StatData
    {
        public required string Label { get; set; }
        public double Value { get; set; }
        public required string Change { get; set; }
        public required string Trend { get; set; } // "up" or "down"
        public required string IconChar { get; set; } // Using string for Unicode char
        public required Color Color { get; set; } // For base color like "blue", "green"
    }

    public class TransactionData
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public double Amount { get; set; }
        public required string Type { get; set; } // "income" or "expense"
        public required string Date { get; set; }
        public required string Category { get; set; }
    }

    public class AccountData
    {
        public required string Name { get; set; }
        public double Balance { get; set; }
        public required string Type { get; set; }
        public required string Bank { get; set; }
    }


    public partial class OverviewView : ContentView
    {
        // List to hold references to all balance-displaying labels for easy toggling
        private List<Label> _balanceLabels = [];

        // Hardcoded Data (as per React example)
        private List<StatData> _stats =
        [
            new StatData { Label = "Total Balance", Value = 24567.89, Change = "+12.5%", Trend = "up", IconChar = "$", Color = Colors.Blue },
            new StatData { Label = "Monthly Income", Value = 8450.00, Change = "+5.2%", Trend = "up", IconChar = "⬆️", Color = Colors.Green },
            new StatData { Label = "Monthly Expenses", Value = 3892.45, Change = "-8.1%", Trend = "down", IconChar = "⬇️", Color = Colors.Red },
            new StatData { Label = "Active Cards", Value = 6, Change = "+1", Trend = "up", IconChar = "💳", Color = Colors.Purple }
        ];

        private List<TransactionData> _recentTransactions =
        [
            new TransactionData { Id = 1, Description = "Salary Payment", Amount = 5000, Type = "income", Date = "2024-01-15", Category = "Salary" },
            new TransactionData { Id = 2, Description = "Grocery Store", Amount = -156.78, Type = "expense", Date = "2024-01-14", Category = "Food" },
            new TransactionData { Id = 3, Description = "Netflix Subscription", Amount = -15.99, Type = "expense", Date = "2024-01-14", Category = "Entertainment" },
            new TransactionData { Id = 4, Description = "Freelance Project", Amount = 1200, Type = "income", Date = "2024-01-13", Category = "Freelance" },
            new TransactionData { Id = 5, Description = "Gas Station", Amount = -45.20, Type = "expense", Date = "2024-01-13", Category = "Transportation" }
        ];

        private List<AccountData> _accounts =
        [
            new AccountData { Name = "Chase Checking", Balance = 12456.78, Type = "checking", Bank = "Chase" },
            new AccountData { Name = "Savings Account", Balance = 8901.23, Type = "savings", Bank = "Bank of America" },
            new AccountData { Name = "Investment Account", Balance = 3209.88, Type = "investment", Bank = "Fidelity" }
        ];

        public OverviewView()
        {
            InitializeComponent();
            PopulateOverviewContent();
        }

        /// <summary>
        /// Populates the various sections of the overview page with hardcoded data.
        /// </summary>
        private void PopulateOverviewContent()
        {
            PopulateStats();
            PopulateRecentTransactions();
            PopulateAccountsSummary();
        }

        /// <summary>
        /// Populates the StatsGrid with data from the _stats list.
        /// </summary>
        private void PopulateStats()
        {
            StatsGrid.Children.Clear(); // Clear existing children if any
            // Remove old stat labels from the _balanceLabels list
            _balanceLabels.RemoveAll(l => l.Parent is VerticalStackLayout vs && vs.Parent is Border b && b.Parent == StatsGrid);

            int col = 0;
            int row = 0;
            foreach (var stat in _stats)
            {
                // Determine colors based on the React example's Tailwind classes
                Color iconBgColor;
                Color iconTextColor;
                Color trendTextColor;

                // Simple switch for color mapping for icon background and text
                switch (stat.Color.ToArgbHex())
                {
                    case "#FF0000FF": // Blue
                        iconBgColor = Color.FromArgb("#DBEAFE"); // bg-blue-100
                        iconTextColor = Color.FromArgb("#2563EB"); // text-blue-600
                        break;
                    case "#FF008000": // Green (approx)
                        iconBgColor = Color.FromArgb("#D1FAE5"); // bg-green-100
                        iconTextColor = Color.FromArgb("#059669"); // text-green-600
                        break;
                    case "#FFFF0000": // Red (approx)
                        iconBgColor = Color.FromArgb("#FEE2F2"); // bg-red-100
                        iconTextColor = Color.FromArgb("#DC2626"); // text-red-600
                        break;
                    case "#FF800080": // Purple (approx)
                        iconBgColor = Color.FromArgb("#EDE9FE"); // bg-purple-100
                        iconTextColor = Color.FromArgb("#7C3AED"); // text-purple-600
                        break;
                    default:
                        iconBgColor = Colors.LightGray;
                        iconTextColor = Colors.DarkGray;
                        break;
                }

                trendTextColor = stat.Trend == "up" ? Color.FromArgb("#059669") : Color.FromArgb("#DC2626"); // Green-600 or Red-600

                var statCard = new Border
                {
                    Background = Colors.White,
                    Stroke = Color.FromArgb("#E5E7EB"), // border-gray-200
                    StrokeThickness = 1,
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
                    Padding = 20,
                };

                var statLayout = new VerticalStackLayout();

                var headerLayout = new HorizontalStackLayout
                {
                    VerticalOptions = LayoutOptions.Center,

                    Spacing = 10
                };

                var iconBorder = new Border
                {
                    WidthRequest = 48,
                    HeightRequest = 48,
                    Background = iconBgColor,
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
                    StrokeThickness = 0,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = new Label
                    {
                        Text = stat.IconChar, // Using simple char for icon
                        TextColor = iconTextColor,
                        FontSize = 24,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };

                headerLayout.Add(iconBorder);
                headerLayout.Add(new Label
                {
                    Text = stat.Change,
                    FontSize = 14,

                    TextColor = trendTextColor,

                    VerticalOptions = LayoutOptions.Center
                });

                statLayout.Add(headerLayout);
                var valueLabel = new Label
                {
                    // Fixed: Explicitly add '$' and format as number with 2 decimal places
                    Text = $"${stat.Value:N2}", // "N2" for number with 2 decimal places and thousands separators
                    FontSize = 22,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#1F2937")
                };
                _balanceLabels.Add(valueLabel); // Add to list for toggling
                statLayout.Add(valueLabel);
                statLayout.Add(new Label
                {
                    Text = stat.Label,
                    FontSize = 14,
                    TextColor = Color.FromArgb("#4B5563")
                });

                statCard.Content = statLayout;
                StatsGrid.Add(statCard, col, row);

                col++;
                if (col >= StatsGrid.ColumnDefinitions.Count)
                {
                    col = 0;
                    row++;
                }
            }
        }

        /// <summary>
        /// Populates the RecentTransactionsLayout with data from the _recentTransactions list.
        /// </summary>
        private void PopulateRecentTransactions()
        {
            RecentTransactionsLayout.Children.Clear(); // Clear existing children
            // Remove old transaction labels from the _balanceLabels list
            _balanceLabels.RemoveAll(l => l.Parent is VerticalStackLayout vs && vs.Parent is HorizontalStackLayout hs && hs.Parent is Border b && b.Parent == RecentTransactionsLayout);

            foreach (var transaction in _recentTransactions)
            {
                Color typeBgColor = transaction.Type == "income" ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#FEE2F2"); // bg-green-100 / bg-red-100
                Color typeTextColor = transaction.Type == "income" ? Color.FromArgb("#059669") : Color.FromArgb("#DC2626"); // text-green-600 / text-red-600

                var transactionCard = new Border
                {
                    Background = Color.FromArgb("#F9FAFB"), // bg-gray-50
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
                    Padding = 15, // p-4
                    StrokeThickness = 0,
                };

                var transactionLayout = new HorizontalStackLayout
                {

                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 15 // space-x-4
                };

                var iconContainer = new Border
                {
                    WidthRequest = 40,
                    HeightRequest = 40,
                    Background = typeBgColor,
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(20) },
                    StrokeThickness = 0,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = new Label
                    {
                        Text = transaction.Type == "income" ? "+" : "-",
                        TextColor = typeTextColor,
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };

                transactionLayout.Add(iconContainer);

                var descriptionLayout = new VerticalStackLayout
                {
                    new Label
                    {
                        Text = transaction.Description,
                        FontSize = 16,

                        TextColor = Color.FromArgb("#1F2937")
                    },
                    new Label
                    {
                        Text = $"{transaction.Category} • {transaction.Date}",
                        FontSize = 12,
                        TextColor = Color.FromArgb("#6B7280")
                    }
                };
                transactionLayout.Add(descriptionLayout);

                var amountLabel = new Label
                {
                    // Fixed: Explicitly add '$' and format as number with 2 decimal places
                    Text = $"${Math.Abs(transaction.Amount):N2}",
                    FontSize = 16,

                    TextColor = transaction.Type == "income" ? Color.FromArgb("#059669") : Color.FromArgb("#DC2626"), // text-green-600 / text-red-600
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                };
                _balanceLabels.Add(amountLabel); // Add to list for toggling
                transactionLayout.Add(amountLabel);

                transactionCard.Content = transactionLayout;
                RecentTransactionsLayout.Add(transactionCard);
            }
        }

        /// <summary>
        /// Populates the AccountsLayout with data from the _accounts list.
        /// </summary>
        private void PopulateAccountsSummary()
        {
            AccountsLayout.Children.Clear(); // Clear existing children
            // Remove old account labels from the _balanceLabels list
            _balanceLabels.RemoveAll(l => l.Parent is VerticalStackLayout vs && vs.Parent is Border b && b.Parent == AccountsLayout);

            foreach (var account in _accounts)
            {
                var accountCard = new Border
                {
                    Background = Color.FromArgb("#F9FAFB"), // bg-gray-50
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
                    Padding = 15, // p-4
                    StrokeThickness = 0,
                };

                var accountLayout = new VerticalStackLayout();

                var headerLayout = new HorizontalStackLayout
                {

                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 5 // mb-2
                };
                headerLayout.Add(new Label
                {
                    Text = account.Name,
                    FontSize = 16,

                    TextColor = Color.FromArgb("#1F2937"), // font-medium text-gray-900

                });
                headerLayout.Add(new Border
                {
                    Background = Color.FromArgb("#E5E7EB"), // bg-gray-200
                    Padding = 5,
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(4) }, // Apply CornerRadius here
                    Content = new Label
                    {
                        Text = account.Bank,
                        FontSize = 10,
                        TextColor = Color.FromArgb("#6B7280"), // text-xs text-gray-500
                        HorizontalOptions = LayoutOptions.End
                    }
                });
                accountLayout.Add(headerLayout);

                var balanceLabel = new Label
                {
                    // Fixed: Explicitly add '$' and format as number with 2 decimal places
                    Text = $"${account.Balance:N2}",
                    FontSize = 18,

                    TextColor = Color.FromArgb("#1F2937") // text-lg font-semibold text-gray-900
                };
                _balanceLabels.Add(balanceLabel); // Add to list for toggling
                accountLayout.Add(balanceLabel);

                accountLayout.Add(new Label
                {
                    Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(account.Type), // Capitalize first letter
                    FontSize = 12,
                    TextColor = Color.FromArgb("#6B7280") // text-sm text-gray-500 capitalize
                });

                accountCard.Content = accountLayout;
                AccountsLayout.Add(accountCard);
            }
        }

    }
}
