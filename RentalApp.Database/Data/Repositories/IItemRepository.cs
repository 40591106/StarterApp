using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

// Repository interface for item persistence operations.
public interface IItemRepository : IRepository<Item>
{
    // Gets all items, optionally filtering by category or search.
    Task<List<Item>> GetAllAsync(string? category = null, string? search = null);

    // Gets nearby items within the specified radius.
    Task<List<Item>> GetNearbyAsync(double lat, double lon, double radiusKm);
    // Gets all item categories.
    Task<List<Category>> GetCategoriesAsync();

    // Creates a new item record.
    Task<Item> CreateAsync(Item item);

    // Updates an existing item record.
    Task UpdateAsync(Item item);
}