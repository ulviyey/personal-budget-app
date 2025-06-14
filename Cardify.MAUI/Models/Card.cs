namespace Cardify.MAUI.Models;

public class Card
{
    public int Id { get; set; }
    public string CardType { get; set; } = string.Empty; // e.g., "Credit Card", "Debit Card"
    public string LastFourDigits { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string CardColorStart { get; set; } = "#FFFFFF"; // For visual distinction
    public string CardColorEnd { get; set; } = "#CCCCCC";   // For visual distinction
}