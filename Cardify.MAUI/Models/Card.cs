using System;
using System.Collections.Generic;

namespace Cardify.MAUI.Models;

public class Card
{
    public int Id { get; set; }
    public string CardType { get; set; } = string.Empty; // e.g., "Credit Card", "Debit Card"
    public string CardNumber { get; set; } = string.Empty; // Full card number
    public string LastFourDigits { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string CardColorStart { get; set; } = "#FFFFFF"; // For visual distinction
    public string CardColorEnd { get; set; } = "#CCCCCC";   // For visual distinction
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }

    public string MaskedNumber => "**** **** **** " + LastFourDigits;
}