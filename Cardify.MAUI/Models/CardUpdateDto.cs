using System.ComponentModel.DataAnnotations;

namespace Cardify.MAUI.Models;

public class CardUpdateDto
{
    [StringLength(30)]
    public string? CardType { get; set; }

    [StringLength(19)] // For formatted card number with spaces
    public string? CardNumber { get; set; }

    [StringLength(4, MinimumLength = 4)]
    public string? LastFourDigits { get; set; }

    [StringLength(100)]
    public string? CardHolderName { get; set; }

    [StringLength(7)]
    public string? CardColorStart { get; set; }

    [StringLength(7)]
    public string? CardColorEnd { get; set; }
} 