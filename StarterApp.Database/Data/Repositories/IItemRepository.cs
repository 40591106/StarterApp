using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public interface IItemRepository
{
    Task<List<Item>> GetAllAsync(string? category = null, string? search = null);
    Task<List<Item>> GetNearbyAsync(double lat, double lon, double radiusKm);
    Task<List<Category>> GetCategoriesAsync();
    Task<Item?> GetByIdAsync(int id);
    Task<Item> CreateAsync(Item item);
    Task UpdateAsync(Item item);
}
