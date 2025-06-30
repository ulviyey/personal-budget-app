namespace Cardify.MAUI.Models;

public class Transaction
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty; // income, expense, debt, etc.
    public int CardId { get; set; }
    public Card? Card { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public string Category { get; set; } = string.Empty;
    
    // Computed properties for display
    public string FormattedAmount => $"${Amount:N2}";
    public string FormattedDate => Date.ToString("MMM dd, yyyy");
    public string TypeDisplay => Type switch
    {
        "income" => "Income",
        "expense" => "Expense", 
        "debt" => "Debt",
        _ => Type
    };
    public bool IsIncome => Type == "income";
    public bool IsExpense => Type == "expense";
    public bool IsDebt => Type == "debt";
    
    // Template control property
    public bool ShowActions { get; set; } = true;
} 