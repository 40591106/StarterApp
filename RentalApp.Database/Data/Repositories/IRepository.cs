namespace RentalApp.Database.Data.Repositories;

// Generic repository interface for basic entity retrieval operations.
public interface IRepository<T> where T : class
{
    // Gets an entity by its ID.
    Task<T?> GetByIdAsync(int id);
}
