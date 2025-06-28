using System.ComponentModel.DataAnnotations;

namespace Cardify.Core.Models
{
    public class TransactionUpdateDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        public DateTime? Date { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Amount { get; set; }

        [StringLength(20)]
        public string? Type { get; set; } // income, expense, debt, etc.

        public int? CardId { get; set; }
    }
} 