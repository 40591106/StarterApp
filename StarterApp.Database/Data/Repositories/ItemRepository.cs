using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

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
        return await context.Items.ToListAsync();
    }

    public async Task<Item?> GetByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var item = await context
            .Items.Include(i => i.CategoryNavigation)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (item != null)
            item.Category = item.CategoryNavigation?.Name;
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
