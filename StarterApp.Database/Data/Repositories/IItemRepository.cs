using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public interface IItemRepository
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetByIdAsync(int id);
    Task<Item> CreateAsync(Item item);
    Task UpdateAsync(Item item);
}