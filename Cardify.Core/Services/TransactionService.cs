using Cardify.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Cardify.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly CardifyDbContext _context;

        public TransactionService(CardifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetUserTransactionsAsync(int userId, int? cardId, string? type, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Transactions.Where(t => t.UserId == userId);

            // Apply filters
            if (cardId.HasValue)
                query = query.Where(t => t.CardId == cardId.Value);
            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(t => t.Type == type);
            if (fromDate.HasValue)
                query = query.Where(t => t.Date >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(t => t.Date <= toDate.Value);

            var transactions = await query
                .Include(t => t.Card)
                .OrderByDescending(t => t.Date)
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
                        t.Card.LastFourDigits,
                        t.Card.CardHolderName
                    },
                    t.CreatedAt
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<object?> GetTransactionByIdAsync(int id, int userId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Card)
                .Where(t => t.Id == id && t.UserId == userId)
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
                        t.Card.LastFourDigits,
                        t.Card.CardHolderName
                    },
                    t.CreatedAt,
                    t.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return transaction;
        }

        public async Task<int> CreateTransactionAsync(TransactionCreateDto dto, int userId)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Amount <= 0 || string.IsNullOrWhiteSpace(dto.Type))
                return 0;

            // Check if user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return 0;

            // Check if card exists and belongs to user
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == dto.CardId && c.UserId == userId);
            if (card == null)
                return 0;

            var transaction = new Transaction
            {
                Name = dto.Name,
                Date = dto.Date,
                Amount = dto.Amount,
                Type = dto.Type,
                CardId = dto.CardId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction.Id;
        }

        public async Task<bool> UpdateTransactionAsync(int id, TransactionUpdateDto dto, int userId)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (transaction == null)
                return false;

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(dto.Name))
                transaction.Name = dto.Name;
            if (dto.Date.HasValue)
                transaction.Date = dto.Date.Value;
            if (dto.Amount.HasValue && dto.Amount.Value > 0)
                transaction.Amount = dto.Amount.Value;
            if (!string.IsNullOrWhiteSpace(dto.Type))
                transaction.Type = dto.Type;

            // If CardId is being updated, validate it belongs to user
            if (dto.CardId.HasValue && dto.CardId.Value != transaction.CardId)
            {
                var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == dto.CardId.Value && c.UserId == userId);
                if (card == null)
                    return false;

                transaction.CardId = dto.CardId.Value;
            }

            transaction.UpdatedAt = DateTime.UtcNow;
            transaction.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTransactionAsync(int id, int userId)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (transaction == null)
                return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 