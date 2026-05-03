using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

/// <summary>
/// Repository interface for item persistence operations.
/// </summary>
public interface IItemRepository : IRepository<Item>
{
    /// <summary>
    /// Gets all items, optionally filtering by category slug or search term.
    /// </summary>
    Task<List<Item>> GetAllAsync(string? category = null, string? search = null);

    /// <summary>
    /// Gets items within the specified radius of the given coordinates using PostGIS spatial queries.
    /// </summary>
    Task<List<Item>> GetNearbyAsync(double lat, double lon, double radiusKm);

    /// <summary>
    /// Gets all available item categories.
    /// </summary>
    Task<List<Category>> GetCategoriesAsync();

    /// <summary>
    /// Creates a new item record in the database.
    /// </summary>
    Task<Item> CreateAsync(Item item);

    /// <summary>
    /// Updates an existing item record in the database.
    /// </summary>
    Task UpdateAsync(Item item);
}