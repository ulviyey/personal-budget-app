namespace Cardify.MAUI.Models;

public class TransactionUpdateDto
{
    public string? Name { get; set; }
    public DateTime? Date { get; set; }
    public decimal? Amount { get; set; }
    public string? Type { get; set; } // income, expense, debt, etc.
    public int? CardId { get; set; }
} 