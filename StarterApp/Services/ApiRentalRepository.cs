using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.Services;

public class ApiRentalRepository : IRentalRepository
{
    private readonly HttpClient _httpClient;

    public ApiRentalRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private async Task SetAuthHeader()
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );
    }

    public async Task<Rental> CreateAsync(
        int itemId,
        DateTime startDate,
        DateTime endDate,
        int borrowerId
    )
    {
        await SetAuthHeader();
        var request = new
        {
            itemId,
            startDate = startDate.ToString("yyyy-MM-dd"),
            endDate = endDate.ToString("yyyy-MM-dd"),
        };
        var response = await _httpClient.PostAsJsonAsync("rentals", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(error?.Message ?? "Failed to create rental request");
        }
        return await response.Content.ReadFromJsonAsync<Rental>()
            ?? throw new Exception("Invalid response");
    }

    public async Task<IEnumerable<Rental>> GetIncomingAsync(int userId)
    {
        await SetAuthHeader();
        var response = await _httpClient.GetAsync("rentals/incoming");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RentalsResponse>();
        return result?.Rentals ?? Enumerable.Empty<Rental>();
    }

    public async Task<IEnumerable<Rental>> GetOutgoingAsync(int userId)
    {
        await SetAuthHeader();
        var response = await _httpClient.GetAsync("rentals/outgoing");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RentalsResponse>();
        return result?.Rentals ?? Enumerable.Empty<Rental>();
    }

    public async Task UpdateStatusAsync(int rentalId, string status)
    {
        await SetAuthHeader();
        var request = new { status };
        var response = await _httpClient.PatchAsJsonAsync($"rentals/{rentalId}/status", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(error?.Message ?? "Failed to update rental status");
        }
    }

    private record RentalsResponse(List<Rental> Rentals, int TotalRentals);

    private record ErrorResponse(string Error, string Message);
}
