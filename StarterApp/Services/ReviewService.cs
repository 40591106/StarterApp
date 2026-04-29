using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IItemRepository _itemRepository;

    public ReviewService(IReviewRepository reviewRepository, IItemRepository itemRepository)
    {
        _reviewRepository = reviewRepository;
        _itemRepository = itemRepository;
    }


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

    public async Task<IEnumerable<Review>> GetItemReviewsAsync(int itemId)
    {
        return await _reviewRepository.GetByItemIdAsync(itemId);
    }

    public async Task<IEnumerable<Review>> GetUserReviewsAsync(int userId)
    {
        return await _reviewRepository.GetByUserIdAsync(userId);
    }

    
}
