using System.Text;
using System.Text.Json;
using Cardify.MAUI.Models;

namespace Cardify.MAUI.Services;

public class ApiTransactionService : ITransactionService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5144/api";

    public ApiTransactionService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<List<Transaction>> GetTransactionsAsync()
    {
        try
        {
            var userId = ApiLoginService.CurrentUserId;
            if (userId == null)
            {
                throw new Exception("User not logged in");
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/transactions?userId={userId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return transactions ?? new List<Transaction>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get transactions: {ex.Message}");
        }
    }

    public async Task<Transaction> GetTransactionByIdAsync(int id)
    {
        try
        {
            var userId = ApiLoginService.CurrentUserId;
            if (userId == null)
            {
                throw new Exception("User not logged in");
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/transactions/{id}?userId={userId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transaction = JsonSerializer.Deserialize<Transaction>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return transaction ?? throw new Exception("Transaction not found");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get transaction: {ex.Message}");
        }
    }

    public async Task<Transaction> CreateTransactionAsync(TransactionCreateDto transactionDto)
    {
        try
        {
            var userId = ApiLoginService.CurrentUserId;
            if (userId == null)
            {
                throw new Exception("User not logged in");
            }

            var json = JsonSerializer.Serialize(transactionDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/transactions?userId={userId}", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var transaction = JsonSerializer.Deserialize<Transaction>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return transaction ?? throw new Exception("Failed to create transaction");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create transaction: {ex.Message}");
        }
    }

    public async Task<Transaction> UpdateTransactionAsync(int id, TransactionUpdateDto transactionDto)
    {
        try
        {
            var userId = ApiLoginService.CurrentUserId;
            if (userId == null)
            {
                throw new Exception("User not logged in");
            }

            var json = JsonSerializer.Serialize(transactionDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/transactions/{id}?userId={userId}", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var transaction = JsonSerializer.Deserialize<Transaction>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return transaction ?? throw new Exception("Failed to update transaction");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update transaction: {ex.Message}");
        }
    }

    public async Task DeleteTransactionAsync(int id)
    {
        try
        {
            var userId = ApiLoginService.CurrentUserId;
            if (userId == null)
            {
                throw new Exception("User not logged in");
            }

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/transactions/{id}?userId={userId}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete transaction: {ex.Message}");
        }
    }
} 