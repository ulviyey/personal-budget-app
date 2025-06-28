using System.Text;
using System.Text.Json;
using Cardify.MAUI.Models;

namespace Cardify.MAUI.Services;

public class ApiCardService : ICardService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5144/api";

    public ApiCardService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<List<Card>> GetCardsAsync()
    {
        try
        {
            var userId = ApiLoginService.CurrentUserId;
            if (userId == null)
            {
                throw new Exception("User not logged in");
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/cards?userId={userId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var cards = JsonSerializer.Deserialize<List<Card>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return cards ?? new List<Card>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get cards: {ex.Message}");
        }
    }

    public async Task<Card> GetCardByIdAsync(int id)
    {
        try
        {
            var userId = ApiLoginService.CurrentUserId;
            if (userId == null)
            {
                throw new Exception("User not logged in");
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/cards/{id}?userId={userId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var card = JsonSerializer.Deserialize<Card>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return card ?? throw new Exception("Card not found");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get card: {ex.Message}");
        }
    }

    public async Task<Card> CreateCardAsync(CardCreateDto cardDto)
    {
        try
        {
            var userId = ApiLoginService.CurrentUserId;
            if (userId == null)
            {
                throw new Exception("User not logged in");
            }

            var json = JsonSerializer.Serialize(cardDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/cards?userId={userId}", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var card = JsonSerializer.Deserialize<Card>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return card ?? throw new Exception("Failed to create card");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create card: {ex.Message}");
        }
    }

    public async Task<Card> UpdateCardAsync(int id, CardUpdateDto cardDto)
    {
        try
        {
            var userId = ApiLoginService.CurrentUserId;
            if (userId == null)
            {
                throw new Exception("User not logged in");
            }

            var json = JsonSerializer.Serialize(cardDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/cards/{id}?userId={userId}", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var card = JsonSerializer.Deserialize<Card>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return card ?? throw new Exception("Failed to update card");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update card: {ex.Message}");
        }
    }

    public async Task DeleteCardAsync(int id)
    {
        try
        {
            var userId = ApiLoginService.CurrentUserId;
            if (userId == null)
            {
                throw new Exception("User not logged in");
            }

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/cards/{id}?userId={userId}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete card: {ex.Message}");
        }
    }
} 