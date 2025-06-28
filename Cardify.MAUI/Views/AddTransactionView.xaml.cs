using Cardify.MAUI.Models;
using Cardify.MAUI.Services;

namespace Cardify.MAUI.Views;

public partial class AddTransactionView : ContentView
{
    private readonly ITransactionService _transactionService;
    private readonly ICardService _cardService;
    private string _transactionName = string.Empty;
    private decimal _amount = 0;
    private string _selectedType = string.Empty;
    private DateTime _selectedDate = DateTime.Now;
    private int _selectedCardId = 0;
    private List<Card> _availableCards = new();

    public event EventHandler? TransactionAdded;
    public event EventHandler? Cancelled;

    public AddTransactionView()
    {
        InitializeComponent();
        _transactionService = new ApiTransactionService();
        _cardService = new ApiCardService();
        
        // Set default date
        TransactionDatePicker.Date = DateTime.Now;
        
        // Populate transaction type picker
        TransactionTypePicker.Items.Add("Income");
        TransactionTypePicker.Items.Add("Expense");
        TransactionTypePicker.Items.Add("Debt");
        
        LoadCards();
        UpdatePreview();
    }

    private async void LoadCards()
    {
        try
        {
            _availableCards = await _cardService.GetCardsAsync();
            
            // Populate card picker
            CardPicker.Items.Clear();
            foreach (var card in _availableCards)
            {
                CardPicker.Items.Add($"{card.CardHolderName} - {card.CardType}");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Error", "Failed to load cards: " + ex.Message, "OK");
        }
    }

    private void OnTransactionNameChanged(object sender, TextChangedEventArgs e)
    {
        _transactionName = e.NewTextValue ?? string.Empty;
        UpdatePreview();
        ValidateForm();
    }

    private void OnAmountChanged(object sender, TextChangedEventArgs e)
    {
        var input = e.NewTextValue ?? string.Empty;
        
        // Remove non-numeric characters except decimal point
        var cleanInput = new string(input.Where(c => char.IsDigit(c) || c == '.').ToArray());
        
        // Ensure only one decimal point
        var parts = cleanInput.Split('.');
        if (parts.Length > 2)
        {
            cleanInput = parts[0] + "." + string.Join("", parts.Skip(1));
        }
        
        // Limit to 2 decimal places
        if (parts.Length == 2 && parts[1].Length > 2)
        {
            cleanInput = parts[0] + "." + parts[1].Substring(0, 2);
        }
        
        // Update the entry if the cleaned text is different
        if (cleanInput != input)
        {
            AmountEntry.Text = cleanInput;
            AmountEntry.CursorPosition = cleanInput.Length;
        }
        
        if (decimal.TryParse(cleanInput, out var amount))
        {
            _amount = amount;
        }
        else
        {
            _amount = 0;
        }
        
        UpdatePreview();
        ValidateForm();
    }

    private void OnTransactionTypeChanged(object sender, EventArgs e)
    {
        if (TransactionTypePicker.SelectedItem != null)
        {
            _selectedType = TransactionTypePicker.SelectedItem.ToString()?.ToLower() ?? string.Empty;
            UpdatePreview();
            ValidateForm();
        }
    }

    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        _selectedDate = e.NewDate;
        UpdatePreview();
        ValidateForm();
    }

    private void OnCardChanged(object sender, EventArgs e)
    {
        if (CardPicker.SelectedIndex >= 0 && CardPicker.SelectedIndex < _availableCards.Count)
        {
            _selectedCardId = _availableCards[CardPicker.SelectedIndex].Id;
            UpdatePreview();
            ValidateForm();
        }
    }

    private void UpdatePreview()
    {
        PreviewName.Text = string.IsNullOrEmpty(_transactionName) ? "Transaction Name" : _transactionName;
        PreviewAmount.Text = $"${_amount:N2}";
        PreviewType.Text = string.IsNullOrEmpty(_selectedType) ? "Type" : _selectedType;
        PreviewDate.Text = _selectedDate.ToString("MMM dd, yyyy");
        
        var selectedCard = _availableCards.FirstOrDefault(c => c.Id == _selectedCardId);
        PreviewCard.Text = selectedCard != null ? $"{selectedCard.CardHolderName} - {selectedCard.CardType}" : "Card";
        
        // Update amount color based on type
        var amountColor = _selectedType switch
        {
            "income" => "#059669", // Green
            "expense" => "#DC2626", // Red
            "debt" => "#F59E0B",    // Orange
            _ => "#059669"          // Default green
        };
        
        PreviewAmount.TextColor = Color.FromArgb(amountColor);
    }

    private void ValidateForm()
    {
        var isValid = !string.IsNullOrWhiteSpace(_transactionName) &&
                     _amount > 0 &&
                     !string.IsNullOrWhiteSpace(_selectedType) &&
                     _selectedCardId > 0;

        AddTransactionButton.IsEnabled = isValid;
    }

    private async void OnAddTransactionClicked(object sender, EventArgs e)
    {
        try
        {
            AddTransactionButton.IsEnabled = false;
            AddTransactionButton.Text = "Adding...";

            var transactionDto = new TransactionCreateDto
            {
                Name = _transactionName,
                Amount = _amount,
                Type = _selectedType,
                Date = _selectedDate,
                CardId = _selectedCardId
            };

            await _transactionService.CreateTransactionAsync(transactionDto);

            await Application.Current.Windows[0].Page.DisplayAlert(
                "Success", 
                "Transaction added successfully!", 
                "OK");

            TransactionAdded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            await Application.Current.Windows[0].Page.DisplayAlert(
                "Error", 
                $"Failed to add transaction: {ex.Message}", 
                "OK");
        }
        finally
        {
            AddTransactionButton.IsEnabled = true;
            AddTransactionButton.Text = "Add Transaction";
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }
} 