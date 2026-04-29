using System.Net.Http.Headers;
using System.Net.Http.Json;
using StarterApp.Database.Models;

namespace StarterApp.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
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

    private async Task HandleUnauthorizedAsync()
    {
        SecureStorage.Remove("auth_token");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        await Shell.Current.GoToAsync("//LoginPage");
    }

    public async Task<List<Item>> GetItemsAsync(
        string? category = null,
        string? search = null,
        int page = 1
    )
    {
        var query = $"items?page={page}";
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

    private record ItemsListResponse(List<Item> Items, int TotalItems, int Page, int PageSize);

    private record CategoriesResponse(List<Category> Categories);

    private record ErrorResponse(string Error, string Message);
}
