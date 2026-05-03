using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

namespace RentalApp.Services;

// Service for validating and submitting reviews through the review repository.
public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IItemRepository _itemRepository;

    // Creates the review service with repository dependencies.
    public ReviewService(IReviewRepository reviewRepository, IItemRepository itemRepository)
    {
        _reviewRepository = reviewRepository;
        _itemRepository = itemRepository;
    }

    // Validates and submits a review for a rental item.
    public async Task<Review> SubmitReviewAsync(
        int rentalId,
        int itemId,
        int reviewerId,
        string comment,
        int rating
    )
    {
        if (rating < 1 || rating > 5)
            throw new Exception("Rating must be between 1 and 5.");

        return await _reviewRepository.CreateAsync(rentalId, itemId, reviewerId, comment, rating);
    }

    // Retrieves reviews for a specific item.
    public async Task<IEnumerable<Review>> GetItemReviewsAsync(int itemId)
    {
        return await _reviewRepository.GetByItemIdAsync(itemId);
    }

    // Retrieves reviews authored by a specific user.
    public async Task<IEnumerable<Review>> GetUserReviewsAsync(int userId)
    {
        return await _reviewRepository.GetByUserIdAsync(userId);
    }
}
