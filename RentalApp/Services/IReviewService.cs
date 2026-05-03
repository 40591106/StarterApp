using RentalApp.Database.Models;

namespace RentalApp.Services;

/// <summary>
/// Contract for review services that submit and retrieve reviews.
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Submits a review for a completed rental. Validates that rating is between 1 and 5.
    /// </summary>
    Task<Review> SubmitReviewAsync(int rentalId, int itemId, int reviewerId, string comment, int rating);

    /// <summary>
    /// Gets all reviews for a specific item.
    /// </summary>
    Task<IEnumerable<Review>> GetItemReviewsAsync(int itemId);

    /// <summary>
    /// Gets all reviews written by a specific user.
    /// </summary>
    Task<IEnumerable<Review>> GetUserReviewsAsync(int userId);
}