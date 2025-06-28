using System.ComponentModel.DataAnnotations;

namespace Cardify.Core.Models
{
    public class CardCreateDto
    {
        [Required]
        [StringLength(30)]
        public string CardType { get; set; } = string.Empty;

        [Required]
        [StringLength(19)] // For formatted card number with spaces
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(4, MinimumLength = 4)]
        public string LastFourDigits { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CardHolderName { get; set; } = string.Empty;

        [StringLength(7)]
        public string CardColorStart { get; set; } = "#FFFFFF";

        [StringLength(7)]
        public string CardColorEnd { get; set; } = "#CCCCCC";
    }
} 