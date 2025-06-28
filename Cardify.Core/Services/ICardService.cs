using Cardify.Core.Models;

namespace Cardify.Core.Services
{
    public interface ICardService
    {
        Task<IEnumerable<object>> GetUserCardsAsync(int userId);
        Task<object?> GetCardByIdAsync(int id, int userId);
        Task<int> CreateCardAsync(CardCreateDto dto, int userId);
        Task<bool> UpdateCardAsync(int id, CardUpdateDto dto, int userId);
        Task<bool> DeleteCardAsync(int id, int userId);
    }
} 