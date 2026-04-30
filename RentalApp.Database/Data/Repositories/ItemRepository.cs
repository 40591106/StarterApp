using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ItemRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Item>> GetAllAsync(string? category = null, string? search = null)
    {
        using var context = _contextFactory.CreateDbContext();
        var query = context.Items.Include(i => i.CategoryNavigation).AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(i =>
                i.CategoryNavigation != null && i.CategoryNavigation.Slug == category
            );

        if (!string.IsNullOrEmpty(search))
            query = query.Where(i => i.Title.Contains(search) || i.Description.Contains(search));

        var items = await query.ToListAsync();
        foreach (var item in items)
            item.Category = item.CategoryNavigation?.Name;
        return items;
    }

    public async Task<List<Item>> GetNearbyAsync(double lat, double lon, double radiusKm)
    {
        using var context = _contextFactory.CreateDbContext();
        var radiusMetres = radiusKm * 1000;

        var items = await context.Items
            .FromSqlRaw(@"
            SELECT * FROM items
            WHERE ""Latitude"" IS NOT NULL 
            AND ""Longitude"" IS NOT NULL
            AND ST_DWithin(
                ST_MakePoint(""Longitude"", ""Latitude"")::geography,
                ST_MakePoint({0}, {1})::geography,
                {2}
            )", lon, lat, radiusMetres)
            .Include(i => i.CategoryNavigation)
            .ToListAsync();

        foreach (var item in items)
        {
            item.Category = item.CategoryNavigation?.Name;
            if (item.Latitude.HasValue && item.Longitude.HasValue)
            {
                item.Distance = CalculateDistance(lat, lon,
                    item.Latitude.Value, item.Longitude.Value);
            }
        }

        return items;
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    public async Task<Item?> GetByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var item = await context.Items
            .Include(i => i.CategoryNavigation)
            .Include(i => i.Owner)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item != null)
        {
            item.Category = item.CategoryNavigation?.Name;
            item.OwnerName = item.Owner != null ? $"{item.Owner.FirstName} {item.Owner.LastName}" : string.Empty;
            var reviews = await context.Reviews
                .Where(r => r.ItemId == id)
                .ToListAsync();

            item.TotalReviews = reviews.Count;
            item.AverageRating = reviews.Count > 0
                ? reviews.Average(r => r.Rating)
                : null;
        }

        return item;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Categories.ToListAsync();
    }

    public async Task<Item> CreateAsync(Item item)
    {
        using var context = _contextFactory.CreateDbContext();
        item.CreatedAt = DateTime.UtcNow;
        context.Items.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateAsync(Item item)
    {
        using var context = _contextFactory.CreateDbContext();
        item.UpdatedAt = DateTime.UtcNow;
        context.Items.Update(item);
        await context.SaveChangesAsync();
    }
}
