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
    private bool _isEditMode = false;
    private int _editingTransactionId = 0;

    public event EventHandler? TransactionAdded;
    public event EventHandler? TransactionUpdated;
    public event EventHandler? Cancelled;

    public AddTransactionView()
    {
        InitializeComponent();
        _transactionService = new ApiTransactionService();
        _cardService = new ApiCardService();
        
        // Set default date
        TransactionDatePicker.Date = DateTime.Now;
        
        LoadCards();
        UpdatePreview();
    }

    public async Task SetEditMode(int transactionId)
    {
        _isEditMode = true;
        _editingTransactionId = transactionId;
        
        // Update UI for edit mode
        HeaderLabel.Text = "Edit Transaction";
        SubHeaderLabel.Text = "Update transaction details";
        SaveTransactionButton.Text = "Update Transaction";
        
        try
        {
            // Load transaction data
            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);
            
            // Update internal state first
            _transactionName = transaction.Name;
            _amount = Math.Abs(transaction.Amount);
            _selectedType = transaction.Type.ToLower();
            _selectedDate = transaction.Date;
            _selectedCardId = transaction.CardId;
            
            // Populate form fields
            TransactionNameEntry.Text = _transactionName;
            AmountEntry.Text = _amount.ToString("F2");
            TransactionDatePicker.Date = _selectedDate;
            
            // Set transaction type
            switch (_selectedType)
            {
                case "income":
                    IncomeRadioButton.IsChecked = true;
                    break;
                case "expense":
                    ExpenseRadioButton.IsChecked = true;
                    break;
                case "debt":
                    DebtRadioButton.IsChecked = true;
                    break;
            }
            
            UpdatePreview();
            ValidateForm();
        }
        catch (Exception ex)
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Error", "Failed to load transaction: " + ex.Message, "OK");
        }
    }

    public void ResetToCreateMode()
    {
        _isEditMode = false;
        _editingTransactionId = 0;
        
        // Reset UI for create mode
        HeaderLabel.Text = "Add New Transaction";
        SubHeaderLabel.Text = "Enter transaction details to track your finances";
        SaveTransactionButton.Text = "Add Transaction";
        
        // Clear form fields
        TransactionNameEntry.Text = string.Empty;
        AmountEntry.Text = string.Empty;
        TransactionDatePicker.Date = DateTime.Now;
        IncomeRadioButton.IsChecked = false;
        ExpenseRadioButton.IsChecked = false;
        DebtRadioButton.IsChecked = false;
        CardPicker.SelectedIndex = -1;
        
        // Reset internal state
        _transactionName = string.Empty;
        _amount = 0;
        _selectedType = string.Empty;
        _selectedDate = DateTime.Now;
        _selectedCardId = 0;
        
        UpdatePreview();
        ValidateForm();
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
            
            // If in edit mode, set the selected card
            if (_isEditMode && _selectedCardId > 0)
            {
                var cardIndex = _availableCards.FindIndex(c => c.Id == _selectedCardId);
                if (cardIndex >= 0)
                {
                    CardPicker.SelectedIndex = cardIndex;
                }
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
        if (sender is RadioButton radioButton && radioButton.IsChecked)
        {
            _selectedType = radioButton.Content.ToString()?.ToLower() ?? string.Empty;
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
        var hasName = !string.IsNullOrWhiteSpace(_transactionName);
        var hasValidAmount = _amount > 0;
        var hasType = !string.IsNullOrWhiteSpace(_selectedType);
        var hasCard = _selectedCardId > 0;

        var isValid = hasName && hasValidAmount && hasType && hasCard;

        System.Diagnostics.Debug.WriteLine($"ValidateForm - Name: {hasName}, Amount: {hasValidAmount} ({_amount}), Type: {hasType}, Card: {hasCard}, Valid: {isValid}");

        SaveTransactionButton.IsEnabled = isValid;
    }

    private async void OnSaveTransactionClicked(object sender, EventArgs e)
    {
        try
        {
            SaveTransactionButton.IsEnabled = false;
            SaveTransactionButton.Text = _isEditMode ? "Updating..." : "Adding...";

            System.Diagnostics.Debug.WriteLine($"SaveTransactionClicked - Edit mode: {_isEditMode}");
            System.Diagnostics.Debug.WriteLine($"Transaction data - Name: {_transactionName}, Amount: {_amount}, Type: {_selectedType}, CardId: {_selectedCardId}");

            if (_isEditMode)
            {
                // Update existing transaction
                var updateDto = new TransactionUpdateDto
                {
                    Name = _transactionName,
                    Amount = _amount,
                    Type = _selectedType,
                    Date = _selectedDate,
                    CardId = _selectedCardId
                };

                System.Diagnostics.Debug.WriteLine($"Updating transaction {_editingTransactionId}");
                await _transactionService.UpdateTransactionAsync(_editingTransactionId, updateDto);
                System.Diagnostics.Debug.WriteLine("Transaction updated successfully");

                await Application.Current.Windows[0].Page.DisplayAlert(
                    "Success", 
                    "Transaction updated successfully!", 
                    "OK");

                TransactionUpdated?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Create new transaction
                var createDto = new TransactionCreateDto
                {
                    Name = _transactionName,
                    Amount = _amount,
                    Type = _selectedType,
                    Date = _selectedDate,
                    CardId = _selectedCardId
                };

                await _transactionService.CreateTransactionAsync(createDto);

                await Application.Current.Windows[0].Page.DisplayAlert(
                    "Success", 
                    "Transaction added successfully!", 
                    "OK");

                TransactionAdded?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnSaveTransactionClicked: {ex.Message}");
            await Application.Current.Windows[0].Page.DisplayAlert(
                "Error", 
                $"Failed to {( _isEditMode ? "update" : "add" )} transaction: {ex.Message}", 
                "OK");
        }
        finally
        {
            SaveTransactionButton.IsEnabled = true;
            SaveTransactionButton.Text = _isEditMode ? "Update Transaction" : "Add Transaction";
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }
} 