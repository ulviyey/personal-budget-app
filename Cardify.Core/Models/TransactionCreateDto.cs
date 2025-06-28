using System.ComponentModel.DataAnnotations;

namespace Cardify.Core.Models
{
    public class TransactionCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = string.Empty; // income, expense, debt, etc.

        [Required]
        public int CardId { get; set; }
    }
} 