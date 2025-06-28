using Cardify.MAUI.Models;
using Cardify.MAUI.Services;

namespace Cardify.MAUI.Views;

public partial class CardsView : ContentView
{
    private readonly ICardService _cardService;

    public CardsView()
    {
        InitializeComponent();
        _cardService = new ApiCardService();
        LoadCards();
    }

    private async void LoadCards()
    {
        try
        {
            var cards = await _cardService.GetCardsAsync();
            DisplayCards(cards);
        }
        catch (Exception ex)
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Error", "Failed to load cards: " + ex.Message, "OK");
        }
    }

    private void DisplayCards(List<Card> cards)
    {
        CardsContainer.Children.Clear();

        if (!cards.Any())
        {
            // Show empty state
            var emptyLabel = new Label
            {
                Text = "No cards found.\nTap 'Add Card' to get started!",
                FontSize = 16,
                TextColor = Color.FromArgb("#6B7280"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            CardsContainer.Children.Add(emptyLabel);
            return;
        }

        foreach (var card in cards)
        {
            var cardView = CreateCardView(card);
            CardsContainer.Children.Add(cardView);
        }
    }

    private View CreateCardView(Card card)
    {
        var cardBorder = new Border
        {
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#E5E7EB"),
            StrokeThickness = 1,
            Padding = new Thickness(20),
            Margin = new Thickness(0, 0, 0, 10)
        };

        var cardContent = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        };

        // Card holder name and type
        var nameLabel = new Label
        {
            Text = card.CardHolderName,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#1F2937")
        };

        var typeLabel = new Label
        {
            Text = card.CardType,
            FontSize = 14,
            TextColor = Color.FromArgb("#6B7280")
        };

        // Card number (masked)
        var maskedNumber = "**** **** **** " + card.LastFourDigits;
        var numberLabel = new Label
        {
            Text = maskedNumber,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#374151")
        };

        // Created date
        var createdLabel = new Label
        {
            Text = $"Added: {card.CreatedAt:MMM dd, yyyy}",
            FontSize = 14,
            TextColor = Color.FromArgb("#6B7280")
        };

        // Action buttons
        var buttonStack = new HorizontalStackLayout
        {
            Spacing = 10,
            HorizontalOptions = LayoutOptions.End
        };

        var editButton = new Button
        {
            Text = "Edit",
            BackgroundColor = Color.FromArgb("#F59E0B"),
            TextColor = Colors.White,
            FontSize = 12,
            Padding = new Thickness(12, 6),
            CornerRadius = 6
        };
        editButton.Clicked += (s, e) => OnEditCardClicked(card);

        var deleteButton = new Button
        {
            Text = "Delete",
            BackgroundColor = Color.FromArgb("#EF4444"),
            TextColor = Colors.White,
            FontSize = 12,
            Padding = new Thickness(12, 6),
            CornerRadius = 6
        };
        deleteButton.Clicked += (s, e) => OnDeleteCardClicked(card);

        buttonStack.Children.Add(editButton);
        buttonStack.Children.Add(deleteButton);

        // Add elements to grid
        cardContent.Add(nameLabel, 0, 0);
        cardContent.Add(typeLabel, 0, 1);
        cardContent.Add(numberLabel, 0, 2);
        cardContent.Add(createdLabel, 1, 0);
        cardContent.Add(buttonStack, 1, 2);

        cardBorder.Content = cardContent;
        return cardBorder;
    }

    private async void OnAddCardClicked(object sender, EventArgs e)
    {
        // TODO: Implement add card functionality
        await Application.Current.Windows[0].Page.DisplayAlert("Coming Soon", "Add card functionality will be implemented next!", "OK");
    }

    private async void OnEditCardClicked(Card card)
    {
        // TODO: Implement edit card functionality
        await Application.Current.Windows[0].Page.DisplayAlert("Coming Soon", "Edit card functionality will be implemented next!", "OK");
    }

    private async void OnDeleteCardClicked(Card card)
    {
        var result = await Application.Current.Windows[0].Page.DisplayAlert(
            "Delete Card", 
            $"Are you sure you want to delete '{card.CardHolderName}'?", 
            "Delete", "Cancel");

        if (result)
        {
            try
            {
                await _cardService.DeleteCardAsync(card.Id);
                LoadCards(); // Refresh the list
                await Application.Current.Windows[0].Page.DisplayAlert("Success", "Card deleted successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Error", "Failed to delete card: " + ex.Message, "OK");
            }
        }
    }
} 