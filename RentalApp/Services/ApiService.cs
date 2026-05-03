using System.Net.Http.Headers;
using System.Net.Http.Json;
using RentalApp.Database.Models;

namespace RentalApp.Services;

// Service for communicating with the shared API, handling item retrieval, creation, updates, and fetching categories and nearby items.
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Adds the stored auth token to outgoing API request headers.
    private async Task SetAuthHeader()
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );
    }

    // Clears auth state and navigates the user back to login when the API returns unauthorized.
    private async Task HandleUnauthorizedAsync()
    {
        SecureStorage.Remove("auth_token");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        await Shell.Current.GoToAsync("//LoginPage");
    }

    // Get items with optional filtering by category and search term
    public async Task<List<Item>> GetItemsAsync(
        string? category = null,
        string? search = null,
        int page = 1
    )
    {
        var query = $"items?page={page}&pageSize=100";
        if (!string.IsNullOrEmpty(category))
            query += $"&category={category}";
        if (!string.IsNullOrEmpty(search))
            query += $"&search={search}";

        var response = await _httpClient.GetAsync(query);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ItemsListResponse>();
        return result?.Items ?? new List<Item>();
    }

    // Get a single item by ID
    public async Task<Item?> GetItemByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"items/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");

        }
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        var item = await response.Content.ReadFromJsonAsync<Item>();
        System.Diagnostics.Debug.WriteLine($"AVERAGE RATING: {item?.AverageRating}, TOTAL REVIEWS: {item?.TotalReviews}");
        return item;
    }

    // Create a new item
    public async Task<Item> CreateItemAsync(CreateItemRequest request)
    {
        await SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("items", request);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(error?.Message ?? "Failed to create item");
        }
        return await response.Content.ReadFromJsonAsync<Item>()
            ?? throw new Exception("Invalid response");
    }

    // Update an existing item
    public async Task<Item> UpdateItemAsync(int id, UpdateItemRequest request)
    {
        await SetAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"items/{id}", request);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(error?.Message ?? "Failed to update item");
        }
        return await response.Content.ReadFromJsonAsync<Item>()
            ?? throw new Exception("Invalid response");
    }

    // Get all categories
    public async Task<List<Category>> GetCategoriesAsync()
    {
        var response = await _httpClient.GetAsync("categories");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CategoriesResponse>();
        return result?.Categories ?? new List<Category>();
    }

    // Get nearby items based on location and radius
    public async Task<List<Item>> GetNearbyItemsAsync(double lat, double lon, double radiusKm)
    {
        System.Diagnostics.Debug.WriteLine($"NEARBY URL: items/nearby?lat={lat}&lon={lon}&radius={radiusKm}");
        var response = await _httpClient.GetAsync(
            $"items/nearby?lat={lat}&lon={lon}&radius={radiusKm}");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<NearbyItemsResponse>();
        return result?.Items ?? new List<Item>();
    }

    // Request a new rental
    public async Task<Rental> RequestRentalAsync(int itemId, DateTime startDate, DateTime endDate)
    {
        await SetAuthHeader();
        var request = new
        {
            itemId,
            startDate = startDate.ToString("yyyy-MM-dd"),
            endDate = endDate.ToString("yyyy-MM-dd")
        };
        var response = await _httpClient.PostAsJsonAsync("rentals", request);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(error?.Message ?? "Failed to create rental request");
        }
        return await response.Content.ReadFromJsonAsync<Rental>()
            ?? throw new Exception("Invalid response");
    }

    // Get incoming rentals
    public async Task<IEnumerable<Rental>> GetIncomingRentalsAsync()
    {
        await SetAuthHeader();
        var response = await _httpClient.GetAsync("rentals/incoming");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RentalsResponse>();
        return result?.Rentals ?? Enumerable.Empty<Rental>();
    }

    // Get outgoing rentals
    public async Task<IEnumerable<Rental>> GetOutgoingRentalsAsync()
    {
        await SetAuthHeader();
        var response = await _httpClient.GetAsync("rentals/outgoing");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RentalsResponse>();
        return result?.Rentals ?? Enumerable.Empty<Rental>();
    }

    // Update the status of an existing rental
    public async Task UpdateRentalStatusAsync(int rentalId, string status)
    {
        await SetAuthHeader();
        var request = new { status };
        var response = await _httpClient.PatchAsJsonAsync($"rentals/{rentalId}/status", request);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(error?.Message ?? "Failed to update rental status");
        }
    }

    // Placeholder for retrieving rentals filtered by item ID, currently not supported by the API.
    public Task<IEnumerable<Rental>> GetByItemIdAsync(int itemId)
    {
        return Task.FromResult(Enumerable.Empty<Rental>());
    }

    // Create a new review
    public async Task<Review> CreateReviewAsync(int rentalId, int itemId, int reviewerId, string comment, int rating)
    {
        await SetAuthHeader();
        var request = new { rentalId, rating, comment };
        var response = await _httpClient.PostAsJsonAsync("reviews", request);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(error?.Message ?? "Failed to create review");
        }
        return await response.Content.ReadFromJsonAsync<Review>()
            ?? throw new Exception("Invalid response");
    }

    // Get reviews for a specific item
    public async Task<IEnumerable<Review>> GetItemReviewsAsync(int itemId)
    {
        await SetAuthHeader();
        var response = await _httpClient.GetAsync($"items/{itemId}/reviews");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ReviewsResponse>();
        return result?.Reviews ?? Enumerable.Empty<Review>();
    }

    // Get reviews for a specific user
    public async Task<IEnumerable<Review>> GetUserReviewsAsync(int userId)
    {
        await SetAuthHeader();
        var response = await _httpClient.GetAsync($"users/{userId}/reviews");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new Exception("Session expired. Please log in again.");
        }
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ReviewsResponse>();
        return result?.Reviews ?? Enumerable.Empty<Review>();
    }
    private record NearbyItemsResponse(List<Item> Items, int TotalResults);
    private record ItemsListResponse(List<Item> Items, int TotalItems, int Page, int PageSize);
    private record RentalsResponse(List<Rental> Rentals, int TotalRentals);
    private record ReviewsResponse(List<Review> Reviews, int TotalReviews);

    private record CategoriesResponse(List<Category> Categories);

    private record ErrorResponse(string Error, string Message);
}
