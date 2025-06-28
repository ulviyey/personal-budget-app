using Cardify.Core.Models;

namespace Cardify.Core.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<object>> GetUserTransactionsAsync(int userId, int? cardId, string? type, DateTime? fromDate, DateTime? toDate);
        Task<object?> GetTransactionByIdAsync(int id, int userId);
        Task<int> CreateTransactionAsync(TransactionCreateDto dto, int userId);
        Task<bool> UpdateTransactionAsync(int id, TransactionUpdateDto dto, int userId);
        Task<bool> DeleteTransactionAsync(int id, int userId);
    }
} 