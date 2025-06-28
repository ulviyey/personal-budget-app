using Microsoft.EntityFrameworkCore;

namespace Cardify.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly CardifyDbContext _context;

        public DashboardService(CardifyDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetDashboardDataAsync(int userId)
        {
            // Check if user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            // Get current month and year
            var currentDate = DateTime.UtcNow;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            // Get all transactions for the user
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Card)
                .ToListAsync();

            // Get all cards for the user (just for count)
            var cards = await _context.Cards
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // Calculate totals
            var totalExpenses = transactions.Where(t => t.Type == "expense").Sum(t => t.Amount);
            var totalIncome = transactions.Where(t => t.Type == "income").Sum(t => t.Amount);
            var totalDebts = transactions.Where(t => t.Type == "debt").Sum(t => t.Amount);
            var totalBalance = totalIncome - totalExpenses - totalDebts;

            // Calculate monthly totals
            var monthlyExpenses = transactions
                .Where(t => t.Type == "expense" && t.Date.Month == currentMonth && t.Date.Year == currentYear)
                .Sum(t => t.Amount);
            var monthlyIncome = transactions
                .Where(t => t.Type == "income" && t.Date.Month == currentMonth && t.Date.Year == currentYear)
                .Sum(t => t.Amount);

            // Get recent transactions (last 5) in the format expected by MAUI
            var recentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(5)
                .Select(t => new
                {
                    Id = t.Id,
                    Description = t.Name,
                    Amount = t.Type == "expense" ? -t.Amount : t.Amount, // Negative for expenses, positive for income
                    TransactionDate = t.Date.ToString("MMM dd, yyyy"),
                    Category = t.Type
                })
                .ToList();

            return new
            {
                TotalBalance = totalBalance,
                MonthlyIncome = monthlyIncome,
                MonthlyExpenses = monthlyExpenses,
                ActiveCards = cards.Count,
                RecentTransactions = recentTransactions
            };
        }
    }
} 