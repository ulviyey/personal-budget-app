using Cardify.MAUI.Models;

namespace Cardify.MAUI.Services;

public interface ICardService
{
    Task<List<Card>> GetCardsAsync();
    Task<Card> GetCardByIdAsync(int id);
    Task<Card> CreateCardAsync(CardCreateDto cardDto);
    Task<Card> UpdateCardAsync(int id, CardUpdateDto cardDto);
    Task DeleteCardAsync(int id);
} 