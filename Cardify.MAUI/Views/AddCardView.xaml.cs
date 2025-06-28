using Cardify.MAUI.Models;
using Cardify.MAUI.Services;

namespace Cardify.MAUI.Views;

public partial class AddCardView : ContentView
{
    private readonly ICardService _cardService;
    private string _selectedCardType = string.Empty;
    private string _cardNumber = string.Empty;
    private string _cardHolderName = string.Empty;
    private string _startColor = "#FFFFFF";
    private string _endColor = "#CCCCCC";

    public event EventHandler? CardAdded;
    public event EventHandler? Cancelled;

    public AddCardView()
    {
        InitializeComponent();
        _cardService = new ApiCardService();
        
        // Populate card type picker
        CardTypePicker.Items.Add("Credit Card");
        CardTypePicker.Items.Add("Debit Card");
        CardTypePicker.Items.Add("Prepaid Card");
        CardTypePicker.Items.Add("Gift Card");
        
        UpdatePreview();
    }

    private void OnCardTypeChanged(object sender, EventArgs e)
    {
        if (CardTypePicker.SelectedItem != null)
        {
            _selectedCardType = CardTypePicker.SelectedItem.ToString() ?? string.Empty;
            UpdatePreview();
            ValidateForm();
        }
    }

    private void OnCardNumberChanged(object sender, TextChangedEventArgs e)
    {
        var input = e.NewTextValue ?? string.Empty;
        
        // Remove all non-digit characters
        var digitsOnly = new string(input.Where(char.IsDigit).ToArray());
        
        // Limit to 16 digits
        if (digitsOnly.Length > 16)
        {
            digitsOnly = digitsOnly.Substring(0, 16);
        }
        
        // Format with spaces every 4 digits
        var formatted = FormatCardNumber(digitsOnly);
        
        // Update the entry if the formatted text is different
        if (formatted != input)
        {
            CardNumberEntry.Text = formatted;
            CardNumberEntry.CursorPosition = formatted.Length;
        }
        
        _cardNumber = digitsOnly; // Store only digits for validation
        UpdatePreview();
        ValidateForm();
    }

    private string FormatCardNumber(string digits)
    {
        if (string.IsNullOrEmpty(digits))
            return string.Empty;

        var result = string.Empty;
        for (int i = 0; i < digits.Length; i++)
        {
            if (i > 0 && i % 4 == 0)
                result += " ";
            result += digits[i];
        }
        return result;
    }

    private void OnCardHolderNameChanged(object sender, TextChangedEventArgs e)
    {
        _cardHolderName = e.NewTextValue ?? string.Empty;
        UpdatePreview();
        ValidateForm();
    }

    private async void OnStartColorClicked(object sender, EventArgs e)
    {
        var color = await Application.Current.Windows[0].Page.DisplayActionSheet(
            "Select Start Color",
            "Cancel",
            null,
            "White (#FFFFFF)",
            "Blue (#3B82F6)",
            "Green (#10B981)",
            "Purple (#8B5CF6)",
            "Orange (#F59E0B)",
            "Red (#EF4444)",
            "Gray (#6B7280)");

        if (color != null && color != "Cancel")
        {
            _startColor = GetColorHex(color);
            StartColorFrame.BackgroundColor = Color.FromArgb(_startColor);
            StartColorLabel.Text = _startColor;
            UpdatePreview();
        }
    }

    private async void OnEndColorClicked(object sender, EventArgs e)
    {
        var color = await Application.Current.Windows[0].Page.DisplayActionSheet(
            "Select End Color",
            "Cancel",
            null,
            "White (#FFFFFF)",
            "Blue (#3B82F6)",
            "Green (#10B981)",
            "Purple (#8B5CF6)",
            "Orange (#F59E0B)",
            "Red (#EF4444)",
            "Gray (#6B7280)",
            "Black (#000000)",
            "Dark Blue (#1E40AF)",
            "Dark Green (#059669)",
            "Dark Purple (#7C3AED)",
            "Dark Orange (#D97706)",
            "Dark Red (#DC2626)");

        if (color != null && color != "Cancel")
        {
            _endColor = GetColorHex(color);
            EndColorFrame.BackgroundColor = Color.FromArgb(_endColor);
            EndColorLabel.Text = _endColor;
            UpdatePreview();
        }
    }

    private string GetColorHex(string colorName)
    {
        return colorName switch
        {
            "White (#FFFFFF)" => "#FFFFFF",
            "Blue (#3B82F6)" => "#3B82F6",
            "Green (#10B981)" => "#10B981",
            "Purple (#8B5CF6)" => "#8B5CF6",
            "Orange (#F59E0B)" => "#F59E0B",
            "Red (#EF4444)" => "#EF4444",
            "Gray (#6B7280)" => "#6B7280",
            "Black (#000000)" => "#000000",
            "Dark Blue (#1E40AF)" => "#1E40AF",
            "Dark Green (#059669)" => "#059669",
            "Dark Purple (#7C3AED)" => "#7C3AED",
            "Dark Orange (#D97706)" => "#D97706",
            "Dark Red (#DC2626)" => "#DC2626",
            _ => "#FFFFFF"
        };
    }

    private void UpdatePreview()
    {
        PreviewCardType.Text = string.IsNullOrEmpty(_selectedCardType) ? "Card Type" : _selectedCardType;
        
        if (string.IsNullOrEmpty(_cardNumber))
        {
            PreviewCardNumber.Text = "**** **** **** ****";
        }
        else if (_cardNumber.Length < 4)
        {
            PreviewCardNumber.Text = $"**** **** **** {_cardNumber}";
        }
        else
        {
            var lastFour = _cardNumber.Substring(Math.Max(0, _cardNumber.Length - 4));
            PreviewCardNumber.Text = $"**** **** **** {lastFour}";
        }
        
        PreviewCardHolder.Text = string.IsNullOrEmpty(_cardHolderName) ? "Card Holder Name" : _cardHolderName;

        // Create gradient background for preview using both selected colors
        var gradient = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1),
            GradientStops = new GradientStopCollection
            {
                new GradientStop(Color.FromArgb(_startColor), 0.0f),
                new GradientStop(Color.FromArgb(_endColor), 1.0f)
            }
        };

        PreviewFrame.Background = gradient;
    }

    private void ValidateForm()
    {
        var isValid = !string.IsNullOrWhiteSpace(_selectedCardType) &&
                     !string.IsNullOrWhiteSpace(_cardNumber) &&
                     _cardNumber.Length >= 13 && // Minimum card number length
                     _cardNumber.Length <= 16 && // Maximum card number length
                     !string.IsNullOrWhiteSpace(_cardHolderName);

        AddCardButton.IsEnabled = isValid;
    }

    private async void OnAddCardClicked(object sender, EventArgs e)
    {
        try
        {
            AddCardButton.IsEnabled = false;
            AddCardButton.Text = "Adding...";

            // Extract last 4 digits for storage
            var lastFourDigits = _cardNumber.Length >= 4 ? _cardNumber.Substring(_cardNumber.Length - 4) : _cardNumber;
            
            // Format the full card number with spaces
            var formattedCardNumber = FormatCardNumber(_cardNumber);

            var cardDto = new CardCreateDto
            {
                CardType = _selectedCardType,
                CardNumber = formattedCardNumber,
                LastFourDigits = lastFourDigits,
                CardHolderName = _cardHolderName,
                CardColorStart = _startColor,
                CardColorEnd = _endColor
            };

            await _cardService.CreateCardAsync(cardDto);

            await Application.Current.Windows[0].Page.DisplayAlert(
                "Success", 
                "Card added successfully!", 
                "OK");

            CardAdded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            await Application.Current.Windows[0].Page.DisplayAlert(
                "Error", 
                $"Failed to add card: {ex.Message}", 
                "OK");
        }
        finally
        {
            AddCardButton.IsEnabled = true;
            AddCardButton.Text = "Add Card";
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }
} 