// IReviewRepository.cs
using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

// Repository interface for review persistence operations.
public interface IReviewRepository : IRepository<Review>
{
    // Creates a review associated with a rental and item.
    Task<Review> CreateAsync(int rentalId, int itemId, int userId, string comment, int rating);

    // Gets reviews for a specific item.
    Task<IEnumerable<Review>> GetByItemIdAsync(int itemId);

    // Gets reviews for items owned by a specific user.
    Task<IEnumerable<Review>> GetByUserIdAsync(int userId);
}