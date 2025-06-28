using System.Collections.ObjectModel;
using Cardify.MAUI.Models;
using Cardify.MAUI.Services;

namespace Cardify.MAUI.Views;

public partial class CardsView : ContentView
{
    private readonly ICardService _cardService;
    
    public ObservableCollection<Card> Cards { get; set; } = new();

    // Events for card modifications
    public event EventHandler? CardModified;

    public CardsView()
    {
        InitializeComponent();
        _cardService = new ApiCardService(); 
        BindingContext = this;
        LoadCards();
    }

    public void OnTabChanged(object sender, string section)
    {
        // Refresh data when cards tab becomes active
        if (section == "cards")
        {
            LoadCards();
        }
    }

    private async void LoadCards()
    {
        try
        {
            var cards = await _cardService.GetCardsAsync();
            Cards.Clear();
            foreach (var card in cards)
            {
                Cards.Add(card);
            }
        }
        catch (Exception ex)
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Error", "Failed to load cards: " + ex.Message, "OK");
        }
    }

    private void OnAddCardClicked(object sender, EventArgs e)
    {
        // Show the Add Card view in add mode
        AddCardView.SetAddMode();
        CardsListGrid.IsVisible = false;
        AddCardView.IsVisible = true;
    }

    private void OnCardAdded(object sender, EventArgs e)
    {
        // Hide the Add Card view and show the cards list
        AddCardView.IsVisible = false;
        CardsListGrid.IsVisible = true;
        
        // Refresh the cards list
        LoadCards();
        
        // Notify that a card was modified
        CardModified?.Invoke(this, EventArgs.Empty);
    }

    private void OnCardUpdated(object sender, EventArgs e)
    {
        // Hide the Add Card view and show the cards list
        AddCardView.IsVisible = false;
        CardsListGrid.IsVisible = true;
        
        // Refresh the cards list
        LoadCards();
        
        // Notify that a card was modified
        CardModified?.Invoke(this, EventArgs.Empty);
    }

    private void OnCardCancelled(object sender, EventArgs e)
    {
        // Hide the Add Card view and show the cards list
        AddCardView.IsVisible = false;
        CardsListGrid.IsVisible = true;
    }

    private async void OnEditCardClicked(object sender, Card card)
    {
        // Show the Add Card view in edit mode
        AddCardView.SetEditMode(card);
        CardsListGrid.IsVisible = false;
        AddCardView.IsVisible = true;
    }

    private async void OnDeleteCardClicked(object sender, Card card)
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
                Cards.Remove(card); // Remove from the collection
                await Application.Current.Windows[0].Page.DisplayAlert("Success", "Card deleted successfully!", "OK");
                
                // Notify that a card was modified
                CardModified?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Error", "Failed to delete card: " + ex.Message, "OK");
            }
        }
    }
} 