namespace Cardify.MAUI.Models;

public class TransactionCreateDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty; // income, expense, debt, etc.
    public int CardId { get; set; }
    public string Category { get; set; } = string.Empty;
} 