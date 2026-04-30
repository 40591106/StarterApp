using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public interface IReviewRepository
{
    Task<Review> CreateAsync(int rentalId, int itemId, int userId, string comment, int rating);
    Task<IEnumerable<Review>> GetByItemIdAsync(int itemId);
    Task<IEnumerable<Review>> GetByUserIdAsync(int userId);

}
