using Cardify.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Cardify.Core.Services
{
    public class CardService : ICardService
    {
        private readonly CardifyDbContext _context;

        public CardService(CardifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetUserCardsAsync(int userId)
        {
            var cards = await _context.Cards
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.CardType,
                    c.LastFourDigits,
                    c.CardHolderName,
                    c.CardColorStart,
                    c.CardColorEnd,
                    c.CreatedAt
                })
                .ToListAsync();

            return cards;
        }

        public async Task<object?> GetCardByIdAsync(int id, int userId)
        {
            var card = await _context.Cards
                .Where(c => c.Id == id && c.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.CardType,
                    c.LastFourDigits,
                    c.CardHolderName,
                    c.CardColorStart,
                    c.CardColorEnd,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return card;
        }

        public async Task<int> CreateCardAsync(CardCreateDto dto, int userId)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.CardType) || string.IsNullOrWhiteSpace(dto.LastFourDigits) || string.IsNullOrWhiteSpace(dto.CardHolderName))
                return 0;

            // Check if user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return 0;

            var card = new Card
            {
                CardType = dto.CardType,
                LastFourDigits = dto.LastFourDigits,
                CardHolderName = dto.CardHolderName,
                CardColorStart = dto.CardColorStart,
                CardColorEnd = dto.CardColorEnd,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return card.Id;
        }

        public async Task<bool> UpdateCardAsync(int id, CardUpdateDto dto, int userId)
        {
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (card == null)
                return false;

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(dto.CardType))
                card.CardType = dto.CardType;
            if (!string.IsNullOrWhiteSpace(dto.LastFourDigits))
                card.LastFourDigits = dto.LastFourDigits;
            if (!string.IsNullOrWhiteSpace(dto.CardHolderName))
                card.CardHolderName = dto.CardHolderName;
            if (!string.IsNullOrWhiteSpace(dto.CardColorStart))
                card.CardColorStart = dto.CardColorStart;
            if (!string.IsNullOrWhiteSpace(dto.CardColorEnd))
                card.CardColorEnd = dto.CardColorEnd;

            card.UpdatedAt = DateTime.UtcNow;
            card.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCardAsync(int id, int userId)
        {
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (card == null)
                return false;

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 