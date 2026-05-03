using System.ComponentModel;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.Repositories;

// Repository for managing items by communicating with the shared API, handling CRUD operations and fetching item data.
/// <summary>
/// API implementation of IItemRepository using HttpClient to communicate with the shared API.
/// </summary>
public class ApiItemRepository : IItemRepository
{
    private readonly IApiService _apiService;

    // Initializes the repository with the API service
    public ApiItemRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    // Gets all items, optionally filtered by category or search term.
    /// <inheritdoc/>
    public async Task<List<Item>> GetAllAsync(string? category = null, string? search = null) =>
        await _apiService.GetItemsAsync(category: category, search: search);

    // Gets an item by its ID.
    /// <inheritdoc/>
    public async Task<Item?> GetByIdAsync(int id) => await _apiService.GetItemByIdAsync(id);

    // Gets nearby items based on location and radius.
    /// <inheritdoc/>
    public async Task<List<Item>> GetNearbyAsync(double lat, double lon, double radiusKm) =>
    await _apiService.GetNearbyItemsAsync(lat, lon, radiusKm);

    // Gets all categories available for items.
    /// <inheritdoc/>
    public async Task<List<Category>> GetCategoriesAsync() =>
        await _apiService.GetCategoriesAsync();

    // Creates a new item by sending the item details to the API.
    /// <inheritdoc/>
    public async Task<Item> CreateAsync(Item item) =>
        await _apiService.CreateItemAsync(
    new CreateItemRequest(
        item.Title,
        item.Description,
        item.DailyRate,
        item.CategoryId,
        item.Latitude ?? 55.9533,
        item.Longitude ?? -3.1883
    )
    );

    // Updates an existing item by sending the updated details to the API.
    /// <inheritdoc/>
    public async Task UpdateAsync(Item item) =>
        await _apiService.UpdateItemAsync(
            item.Id,
            new UpdateItemRequest(item.Title, item.Description, item.DailyRate, item.IsAvailable)
        );
}
