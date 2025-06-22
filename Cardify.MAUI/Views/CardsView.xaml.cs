using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Globalization;

namespace Cardify.MAUI.Views
{
    // Data Models (simple classes, no INotifyPropertyChanged)
    public class Card
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; } // "Credit" or "Debit"
        public required string Number { get; set; } // Masked number
        public double Balance { get; set; }
        public double? Limit { get; set; } // Nullable for debit cards
        public required string Expiry { get; set; }
        public required string Bank { get; set; }
        public required string ColorFrom { get; set; } // Hex color string for gradient start
        public required string ColorTo { get; set; }   // Hex color string for gradient end
        public required string Network { get; set; } // Visa, Mastercard, Amex, Discover
        public double Cashback { get; set; } // Percentage
        public bool Featured { get; set; }

        // Helper properties for display logic in code-behind
        public double UsedPercentage => Limit.HasValue && Limit.Value > 0 ? (Balance / Limit.Value) * 100 : 0;
        public double AvailableCredit => Limit.HasValue ? (Limit.Value - Balance) : 0;
    }

    public class CardTransaction
    {
        public int Id { get; set; }
        public required string CardName { get; set; } // Name of the card involved in the transaction
        public required string Merchant { get; set; }
        public double Amount { get; set; } // Negative for expenses
        public required string Date { get; set; } // Keep as string for simplicity as in React code
        public required string Category { get; set; }
    }

    public partial class CardsView : ContentView
    {
        private readonly List<Card> _cardsData =
        [
            new Card
            {
                Id = 1, Name = "Chase Sapphire Preferred", Type = "Credit", Number = "**** **** **** 4829",
                Balance = 2456.78, Limit = 15000, Expiry = "12/26", Bank = "Chase",
                ColorFrom = "#2B6CB0", ColorTo = "#1A4E7A", Network = "Visa", Cashback = 2.5, Featured = true
            },
            new Card
            {
                Id = 2, Name = "American Express Gold", Type = "Credit", Number = "**** **** **** 8392",
                Balance = 892.45, Limit = 25000, Expiry = "08/27", Bank = "Amex",
                ColorFrom = "#D69E2E", ColorTo = "#975A16", Network = "Amex", Cashback = 4.0, Featured = false
            },
            new Card
            {
                Id = 3, Name = "Bank of America Checking", Type = "Debit", Number = "**** **** **** 1234",
                Balance = 12456.78, Limit = null, Expiry = "03/25", Bank = "Bank of America",
                ColorFrom = "#E53E3E", ColorTo = "#8B1E1E", Network = "Mastercard", Cashback = 0, Featured = false
            },
            new Card
            {
                Id = 4, Name = "Capital One Venture", Type = "Credit", Number = "**** **** **** 9876",
                Balance = 1234.56, Limit = 20000, Expiry = "09/25", Bank = "Capital One",
                ColorFrom = "#805AD5", ColorTo = "#553C9A", Network = "Visa", Cashback = 2.0, Featured = false
            },
            new Card
            {
                Id = 5, Name = "Wells Fargo Active Cash", Type = "Credit", Number = "**** **** **** 5678",
                Balance = 678.90, Limit = 12000, Expiry = "01/26", Bank = "Wells Fargo",
                ColorFrom = "#38A169", ColorTo = "#276749", Network = "Mastercard", Cashback = 2.0, Featured = false
            },
            new Card
            {
                Id = 6, Name = "Discover It Cash Back", Type = "Credit", Number = "**** **** **** 3456",
                Balance = 345.67, Limit = 8000, Expiry = "06/27", Bank = "Discover",
                ColorFrom = "#ED8936", ColorTo = "#C05621", Network = "Discover", Cashback = 5.0, Featured = false
            }
        ];

        private readonly List<CardTransaction> _recentCardTransactionsData =
        [
            new CardTransaction { Id = 1, CardName = "Chase Sapphire Preferred", Merchant = "Amazon", Amount = -89.99, Date = "2024-01-15", Category = "Shopping" },
            new CardTransaction { Id = 2, CardName = "American Express Gold", Merchant = "Starbucks", Amount = -12.45, Date = "2024-01-14", Category = "Food" },
            new CardTransaction { Id = 3, CardName = "Bank of America Checking", Merchant = "ATM Withdrawal", Amount = -100.00, Date = "2024-01-14", Category = "Cash" },
            new CardTransaction { Id = 4, CardName = "Capital One Venture", Merchant = "United Airlines", Amount = -456.78, Date = "2024-01-13", Category = "Travel" }
        ];

        public CardsView()
        {
            InitializeComponent();
            PopulateAllContent();
        }

        private void PopulateAllContent()
        {
            PopulateCardsGrid();
            PopulateRecentCardTransactions();
            PopulateCardStatistics();
        }

        private void PopulateCardsGrid()
        {
            CardsGridContainer.Children.Clear();
            int col = 0;
            int row = 0;

            foreach (var card in _cardsData)
            {
                var cardMainBorder = new Border
                {
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(20) },
                    StrokeThickness = 0,
                    Padding = 20,
                    HeightRequest = 224,
                    Margin = new Thickness(0, 100, 0, 100),
                    Background = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops =
                        [
                            new GradientStop(Color.FromArgb(card.ColorFrom), 0),
                            new GradientStop(Color.FromArgb(card.ColorTo), 1)
                        ]
                    },
                    Shadow = new Shadow { Offset = new Point(0, 10), Radius = 15, Opacity = 0.2f, Brush = new SolidColorBrush(Colors.Black) }
                };

                var cardContentStack = new VerticalStackLayout
                {
                    Spacing = 0,
                    Children =
                    {
                        // Card Header
                        new Grid
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = GridLength.Star },
                                new ColumnDefinition { Width = GridLength.Star }
                            },
                            VerticalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0,0,0,32),
                            Children =
                            {
                                new HorizontalStackLayout
                                {
                                    Spacing = 5,
                                    VerticalOptions = LayoutOptions.Center,
                                    HorizontalOptions = LayoutOptions.Start,
                                    Children =
                                    {
                                        new Label { Text = card.Featured ? "⭐" : "", FontSize = 16, TextColor = Color.FromArgb("#F6E05E") },
                                        new Label { Text = card.Bank, FontSize = 14, TextColor = Colors.White, Opacity = 0.9 }
                                    }
                                },
                                new HorizontalStackLayout
                                {
                                    Spacing = 5,
                                    VerticalOptions = LayoutOptions.Center,
                                    HorizontalOptions = LayoutOptions.End,
                                    Children =
                                    {
                                        new Label { Text = "📡", FontSize = 16, TextColor = Colors.White, Opacity = 0.75 }, // Wifi icon
                                        new Label { Text = "⋮", FontSize = 16, TextColor = Colors.White, Opacity = 0.75 } // MoreVertical icon
                                    }
                                }
                            }
                        },
                        // Card Number
                        new Label
                        {
                            Text = card.Number,
                            FontSize = 18,
                            FontFamily = "monospace",
                            TextColor = Colors.White,
                            CharacterSpacing = 1,
                            Margin = new Thickness(0,0,0,24), // mb-6
                            VerticalOptions = LayoutOptions.Center
                        },
                        // Card Footer
                        new HorizontalStackLayout
                        {
                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalOptions = LayoutOptions.End,
                            Children =
                            {
                                new VerticalStackLayout
                                {
                                    HorizontalOptions = LayoutOptions.Start,
                                    Children =
                                    {
                                        new Label { Text = "CARD HOLDER", FontSize = 10, Opacity = 0.75, TextColor = Colors.White, Margin = new Thickness(0,0,0,2) },
                                        new Label { Text = "John Doe", FontSize = 14, TextColor = Colors.White }
                                    }
                                },
                                new VerticalStackLayout
                                {
                                    HorizontalOptions = LayoutOptions.Center,
                                    Children =
                                    {
                                        new Label { Text = "EXPIRES", FontSize = 10, Opacity = 0.75, TextColor = Colors.White, Margin = new Thickness(0,0,0,2) },
                                        new Label { Text = card.Expiry, FontSize = 14, TextColor = Colors.White }
                                    }
                                },
                                new VerticalStackLayout
                                {
                                    HorizontalOptions = LayoutOptions.End,
                                    Children =
                                    {
                                        new Label { Text = card.Network, FontSize = 10, Opacity = 0.75, TextColor = Colors.White, Margin = new Thickness(0,0,0,2) },
                                        new Label { Text = "💳", FontSize = 24, TextColor = Colors.White } // CreditCard icon
                                    }
                                }
                            }
                        }
                    }
                };

                // Chip (Absolute positioning simulation)
                var chip = new Border
                {
                    WidthRequest = 48,
                    HeightRequest = 32,
                    Background = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops =
                        [
                            new GradientStop(Color.FromArgb("#F6E05E"), 0), // from-yellow-300
                            new GradientStop(Color.FromArgb("#D69E2E"), 1)  // to-yellow-500
                        ]
                    },
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(6) },

                    Opacity = 0.8
                };

                // Use AbsoluteLayout to position the chip over the card
                var cardLayoutWithChip = new AbsoluteLayout();
                AbsoluteLayout.SetLayoutFlags(cardMainBorder, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(cardMainBorder, new Rect(0, 0, 1, 1));
                cardLayoutWithChip.Add(cardMainBorder);

                AbsoluteLayout.SetLayoutFlags(chip, AbsoluteLayoutFlags.None);
                AbsoluteLayout.SetLayoutBounds(chip, new Rect(20, 80, 48, 32));
                cardLayoutWithChip.Add(chip);

                cardMainBorder.Content = cardContentStack;

                // Card Details
                var cardDetailsStack = new VerticalStackLayout
                {
                    Spacing = 0,
                    Margin = new Thickness(0, 15, 0, 0) // mt-4
                };

                var cardDetailsHeader = new HorizontalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 0, 10),
                    Children =
                    {
                        new Label { Text = card.Name, FontSize = 16, TextColor = Color.FromArgb("#1F2937"), HorizontalOptions = LayoutOptions.Start },
                        new Border
                        {
                            Background = card.Type == "Credit" ? Color.FromArgb("#DBEAFE") : Color.FromArgb("#D1FAE5"),
                            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(20) },
                            Padding = new Thickness(8, 4),
                            StrokeThickness = 0,
                            Content = new Label
                            {
                                Text = card.Type,
                                FontSize = 10,
                                TextColor = card.Type == "Credit" ? Color.FromArgb("#2563EB") : Color.FromArgb("#059669")
                            }
                        }
                    }
                };
                cardDetailsStack.Add(cardDetailsHeader);

                // Current Balance
                var currentBalanceLayout = new HorizontalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    Children =
                    {
                        new Label { Text = "Current Balance", FontSize = 14, TextColor = Color.FromArgb("#4B5563"), HorizontalOptions = LayoutOptions.Start },
                        new Label
                        {
                            Text = $"${card.Balance:N2}",
                            FontSize = 14, TextColor = Color.FromArgb("#1F2937"),
                            HorizontalOptions = LayoutOptions.End
                        }
                    }
                };
                cardDetailsStack.Add(currentBalanceLayout);


                if (card.Limit.HasValue)
                {
                    var creditLimitLayout = new HorizontalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.Fill,
                        Children =
                        {
                            new Label { Text = "Credit Limit", FontSize = 14, TextColor = Color.FromArgb("#4B5563"), HorizontalOptions = LayoutOptions.Start },
                            new Label
                            {
                                Text = $"${card.Limit.Value:N2}",
                                FontSize = 14, TextColor = Color.FromArgb("#1F2937"),
                                HorizontalOptions = LayoutOptions.End
                            }
                        }
                    };
                    cardDetailsStack.Add(creditLimitLayout);

                    var progressBar = new Border
                    {
                        HeightRequest = 8,
                        BackgroundColor = Color.FromArgb("#E5E7EB"),
                        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(4) },

                        StrokeThickness = 0,
                        Content = new BoxView
                        {
                            WidthRequest = (card.UsedPercentage / 100) * 200,
                            HeightRequest = 8,
                            BackgroundColor = Color.FromArgb("#2563EB"),
                            HorizontalOptions = LayoutOptions.Start,
                            CornerRadius = 4
                        }
                    };
                    cardDetailsStack.Add(progressBar);

                    var usedAvailableLayout = new HorizontalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.Fill,
                        Children =
                        {
                            new Label { Text = $"Used: {card.UsedPercentage:N1}%", FontSize = 12, TextColor = Color.FromArgb("#6B7280"), HorizontalOptions = LayoutOptions.Start },
                            new Label
                            {
                                Text = $"Available: ${card.AvailableCredit:N2}", // This will be masked/unmasked
                                FontSize = 12, TextColor = Color.FromArgb("#6B7280"),
                                HorizontalOptions = LayoutOptions.End
                            }
                        }
                    };
                    cardDetailsStack.Add(usedAvailableLayout);
                }

                // Cashback Rate (if applicable)
                if (card.Cashback > 0)
                {
                    var cashbackLayout = new HorizontalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.Fill,
                        Children =
                        {
                            new Label { Text = "Cash Back Rate", FontSize = 14, TextColor = Color.FromArgb("#4B5563"), HorizontalOptions = LayoutOptions.Start },
                            new Label { Text = $"{card.Cashback}%", FontSize = 14, TextColor = Color.FromArgb("#059669"), HorizontalOptions = LayoutOptions.End }
                        }
                    };
                    cardDetailsStack.Add(cashbackLayout);
                }

                var fullCardItem = new VerticalStackLayout
                {
                    Children =
                    {
                        cardLayoutWithChip,
                        new Border
                        {
                            Background = Colors.White,
                            Stroke = Color.FromArgb("#E5E7EB"),
                            StrokeThickness = 1,
                            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
                            Padding = 15,
                            Content = cardDetailsStack
                        }
                    }
                };

                CardsGridContainer.Add(fullCardItem, col, row);

                col++;
                if (col >= CardsGridContainer.ColumnDefinitions.Count)
                {
                    col = 0;
                    row++;
                }
            }
        }

        private void PopulateRecentCardTransactions()
        {
            RecentCardTransactionsLayout.Children.Clear();

            foreach (var transaction in _recentCardTransactionsData)
            {
                var transactionCard = new Border
                {
                    Background = Color.FromArgb("#F9FAFB"),
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
                    Padding = 15,
                    StrokeThickness = 0,
                    HorizontalOptions = LayoutOptions.Fill
                };

                var transactionLayout = new HorizontalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 15
                };

                var iconContainer = new Border
                {
                    WidthRequest = 40,
                    HeightRequest = 40,
                    Background = Color.FromArgb("#FEE2E2"),
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(20) },
                    StrokeThickness = 0,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = new Label
                    {
                        Text = "💳",
                        TextColor = Color.FromArgb("#DC2626"),
                        FontSize = 24,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };
                transactionLayout.Add(iconContainer);

                var descriptionLayout = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Start
                };
                descriptionLayout.Add(new Label { Text = transaction.Merchant, FontSize = 16, TextColor = Color.FromArgb("#1F2937") });
                descriptionLayout.Add(new Label { Text = $"{transaction.CardName} • {transaction.Category}", FontSize = 12, TextColor = Color.FromArgb("#6B7280") });
                transactionLayout.Add(descriptionLayout);

                var amountLayout = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.End
                };
                var amountLabel = new Label
                {
                    Text = $"${Math.Abs(transaction.Amount):N2}", // This will be masked/unmasked
                    FontSize = 16,
                    TextColor = Color.FromArgb("#DC2626")
                };

                amountLayout.Add(amountLabel);
                amountLayout.Add(new Label
                {
                    Text = transaction.Date,
                    FontSize = 12,
                    TextColor = Color.FromArgb("#6B7280")
                });
                transactionLayout.Add(amountLayout);

                transactionCard.Content = transactionLayout;
                RecentCardTransactionsLayout.Add(transactionCard);
            }
        }

        private void PopulateCardStatistics()
        {
            CardStatisticsGrid.Children.Clear();

            // Calculate totals for statistics
            double totalCreditLimit = _cardsData.Where(c => c.Limit.HasValue).Sum(c => c.Limit.Value);
            double totalBalance = _cardsData.Sum(c => c.Balance);
            double totalAvailableCredit = _cardsData.Where(c => c.Limit.HasValue).Sum(c => c.AvailableCredit);
            double creditUtilization = totalCreditLimit > 0 ? (totalBalance / totalCreditLimit) * 100 : 0;

            var stats = new List<(string Label, string RawValue, string FormattedValue, string Description, string Icon, Color ValueColor, bool IsCurrency)>
            {
                ("Total Credit Limit", totalCreditLimit.ToString("N2", CultureInfo.CurrentCulture), $"${totalCreditLimit:N2}", "Across all cards", "💳", Color.FromArgb("#1F2937"), true),
                ("Total Balance", totalBalance.ToString("N2", CultureInfo.CurrentCulture), $"${totalBalance:N2}", "Current debt", "💳", Color.FromArgb("#1F2937"), true),
                ("Available Credit", totalAvailableCredit.ToString("N2", CultureInfo.CurrentCulture), $"${totalAvailableCredit:N2}", "Remaining limit", "💳", Color.FromArgb("#059669"), true), // text-green-600
                ("Credit Utilization", creditUtilization.ToString("N1", CultureInfo.CurrentCulture), $"{creditUtilization:N1}%", "Excellent rating", "💳", Color.FromArgb("#2563EB"), false) // text-blue-600
            };

            int col = 0;
            int row = 0;
            foreach (var stat in stats)
            {
                var statCard = new Border
                {
                    Background = Colors.White,
                    Stroke = Color.FromArgb("#E5E7EB"),
                    StrokeThickness = 1,
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
                    Padding = 20,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                var statLayout = new VerticalStackLayout();

                var headerLayout = new HorizontalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 0, 15)
                };
                headerLayout.Add(new Label { Text = stat.Label, FontSize = 16, TextColor = Color.FromArgb("#1F2937"), HorizontalOptions = LayoutOptions.Start });
                headerLayout.Add(new Label { Text = stat.Icon, FontSize = 18, TextColor = Color.FromArgb("#9CA3AF") });
                statLayout.Add(headerLayout);

                var valueLabel = new Label
                {
                    Text = stat.FormattedValue,
                    FontSize = 22,
                    TextColor = stat.ValueColor
                };

                statLayout.Add(valueLabel);
                statLayout.Add(new Label { Text = stat.Description, FontSize = 14, TextColor = Color.FromArgb("#6B7280") });

                statCard.Content = statLayout;
                CardStatisticsGrid.Add(statCard, col, row);

                col++;
                if (col >= CardStatisticsGrid.ColumnDefinitions.Count)
                {
                    col = 0;
                    row++;
                }
            }
        }



        /// <summary>
        /// Handles the click event for the "Add Card" button.
        /// </summary>
        private void OnAddCardClicked(object sender, EventArgs e)
        {
            //DisplayAlert("Action", "Add Card button clicked!", "OK");
        }

        /// <summary>
        /// Handles the click event for the "View All" transactions button.
        /// </summary>
        private void OnViewAllCardTransactionsClicked(object sender, EventArgs e)
        {
            //DisplayAlert("Action", "View All Card Transactions clicked!", "OK");
        }
    }
}
