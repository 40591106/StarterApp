using System.ComponentModel;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

namespace RentalApp.Services;

public class ApiItemRepository : IItemRepository
{
    private readonly IApiService _apiService;

    public ApiItemRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<Item>> GetAllAsync(string? category = null, string? search = null) =>
        await _apiService.GetItemsAsync(category: category, search: search);

    public async Task<Item?> GetByIdAsync(int id) => await _apiService.GetItemByIdAsync(id);

    public async Task<List<Item>> GetNearbyAsync(double lat, double lon, double radiusKm) =>
    await _apiService.GetNearbyItemsAsync(lat, lon, radiusKm);
    public async Task<List<Category>> GetCategoriesAsync() =>
        await _apiService.GetCategoriesAsync();

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

    public async Task UpdateAsync(Item item) =>
        await _apiService.UpdateItemAsync(
            item.Id,
            new UpdateItemRequest(item.Title, item.Description, item.DailyRate, item.IsAvailable)
        );
}
