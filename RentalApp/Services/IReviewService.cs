using RentalApp.Database.Models;

namespace RentalApp.Services;

public interface IReviewService
{
    Task<Review> SubmitReviewAsync(int rentalId, int itemId, int reviewerId, string comment, int rating);
    Task<IEnumerable<Review>> GetItemReviewsAsync(int itemId);
    Task<IEnumerable<Review>> GetUserReviewsAsync(int userId);
}