using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.Services;

public class ApiReviewRepository : IReviewRepository
{
    private readonly HttpClient _httpClient;

    public ApiReviewRepository(HttpClient httpClient)
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

    public async Task<Review> CreateAsync(
        int rentalId,
int itemId,
int reviewerId,
        string comment,
        int rating
    )
    {
        await SetAuthHeader();
        var request = new
        {
            rentalId,
            comment,
            rating
        };
        var response = await _httpClient.PostAsJsonAsync("reviews", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(error?.Message ?? "Failed to create review");
        }
        return await response.Content.ReadFromJsonAsync<Review>()
            ?? throw new Exception("Invalid response");
    }

    public async Task<IEnumerable<Review>> GetByItemIdAsync(int itemId)
    {
        await SetAuthHeader();
        var response = await _httpClient.GetAsync($"items/{itemId}/reviews");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ReviewsResponse>();
        return result?.Reviews ?? Enumerable.Empty<Review>();
    }

    public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
    {
        await SetAuthHeader();
        var response = await _httpClient.GetAsync($"users/{userId}/reviews");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ReviewsResponse>();
        return result?.Reviews ?? Enumerable.Empty<Review>();
    }

    private record ReviewsResponse(List<Review> Reviews, int TotalReviews);

    private record ErrorResponse(string Message);
}
