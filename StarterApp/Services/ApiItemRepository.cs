using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.Services;

public class ApiItemRepository : IItemRepository
{
    private readonly IApiService _apiService;

    public ApiItemRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<Item>> GetAllAsync()
        => await _apiService.GetItemsAsync();

    public async Task<Item?> GetByIdAsync(int id)
        => await _apiService.GetItemByIdAsync(id);

    public async Task<List<Item>> GetNearbyAsync(double lat, double lon, double radiusKm)
        => await _apiService.GetItemsAsync(); // TODO: replace with GetNearbyItemsAsync in Week 2

    public async Task<Item> CreateAsync(Item item)
        => await _apiService.CreateItemAsync(new CreateItemRequest(
            item.Title,
            item.Description,
            item.DailyRate,
            item.CategoryId,
            55.9533,   // TODO: replace with GPS latitude in Week 2
            -3.1883)); // TODO: replace with GPS longitude in Week 2

    public async Task UpdateAsync(Item item)
        => await _apiService.UpdateItemAsync(item.Id, new UpdateItemRequest(
            item.Title,
            item.Description,
            item.DailyRate,
            item.IsAvailable));
}