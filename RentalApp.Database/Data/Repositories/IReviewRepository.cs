using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

/// <summary>
/// Repository interface for review persistence operations.
/// </summary>
public interface IReviewRepository : IRepository<Review>
{
    /// <summary>
    /// Creates a review associated with a completed rental. Throws if a review already exists for the rental.
    /// </summary>
    Task<Review> CreateAsync(int rentalId, int itemId, int userId, string comment, int rating);

    /// <summary>
    /// Gets all reviews for a specific item.
    /// </summary>
    Task<IEnumerable<Review>> GetByItemIdAsync(int itemId);

    /// <summary>
    /// Gets all reviews written by a specific user.
    /// </summary>
    Task<IEnumerable<Review>> GetByUserIdAsync(int userId);
}