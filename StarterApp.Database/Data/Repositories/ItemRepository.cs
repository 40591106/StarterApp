using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _context;

    public ItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Item>> GetNearbyAsync(double lat, double lon, double radiusKm)
    {
        // TODO: implement PostGIS spatial query in Week 2
        return await _context.Items.ToListAsync();
    }
    public async Task<List<Item>> GetAllAsync()
{
    var items = await _context.Items
        .Include(i => i.CategoryNavigation)
        .ToListAsync();
    
    foreach (var item in items)
        item.Category = item.CategoryNavigation?.Name;
    
    return items;
}

public async Task<Item?> GetByIdAsync(int id)
{
    var item = await _context.Items
        .Include(i => i.CategoryNavigation)
        .FirstOrDefaultAsync(i => i.Id == id);
    
    if (item != null)
        item.Category = item.CategoryNavigation?.Name;
    
    return item;
}

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Item> CreateAsync(Item item)
    {
        item.CreatedAt = DateTime.UtcNow;
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateAsync(Item item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        _context.Items.Update(item);
        await _context.SaveChangesAsync();
    }
}