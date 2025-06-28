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

            // Get monthly data for the last 6 months
            var monthlyData = new List<object>();
            for (int i = 5; i >= 0; i--)
            {
                var month = currentDate.AddMonths(-i);
                var monthTransactions = transactions
                    .Where(t => t.Date.Month == month.Month && t.Date.Year == month.Year)
                    .ToList();

                var monthIncome = monthTransactions.Where(t => t.Type == "income").Sum(t => t.Amount);
                var monthExpenses = monthTransactions.Where(t => t.Type == "expense").Sum(t => t.Amount);

                monthlyData.Add(new
                {
                    Month = month.ToString("MMMM yyyy"),
                    Income = monthIncome,
                    Expenses = monthExpenses
                });
            }

            // Get card-wise summaries
            var cardSummaries = await _context.Cards
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.CardType,
                    c.LastFourDigits,
                    c.CardHolderName,
                    c.CardColorStart,
                    c.CardColorEnd,
                    TotalTransactions = c.Transactions!.Count,
                    TotalAmount = c.Transactions!.Sum(t => t.Amount)
                })
                .ToListAsync();

            // Get recent transactions (last 5)
            var recentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(5)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Date,
                    t.Amount,
                    t.Type,
                    Card = new
                    {
                        t.Card!.Id,
                        t.Card.CardType,
                        t.Card.LastFourDigits
                    }
                })
                .ToList();

            return new
            {
                User = new
                {
                    user.Id,
                    user.Username,
                    user.Name
                },
                Summary = new
                {
                    TotalBalance = totalBalance,
                    TotalIncome = totalIncome,
                    TotalExpenses = totalExpenses,
                    TotalDebts = totalDebts,
                    MonthlyIncome = monthlyIncome,
                    MonthlyExpenses = monthlyExpenses
                },
                MonthlyData = monthlyData,
                CardSummaries = cardSummaries,
                RecentTransactions = recentTransactions
            };
        }
    }
} 