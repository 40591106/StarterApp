namespace RentalApp.Database.Data.Repositories;

/// <summary>
/// Generic base repository interface providing common entity retrieval operations.
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    Task<T?> GetByIdAsync(int id);
}