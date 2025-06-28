using Cardify.MAUI.Models;

namespace Cardify.MAUI.Services;

public interface ITransactionService
{
    Task<List<Transaction>> GetTransactionsAsync();
    Task<Transaction> GetTransactionByIdAsync(int id);
    Task<Transaction> CreateTransactionAsync(TransactionCreateDto transactionDto);
    Task<Transaction> UpdateTransactionAsync(int id, TransactionUpdateDto transactionDto);
    Task DeleteTransactionAsync(int id);
} 