using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.Repositories;

// Review repository that routes review operations through the shared API.
public class ApiReviewRepository : IReviewRepository
{
    private readonly IApiService _apiService;

    // Initializes the review repository with the API service dependency.
    public ApiReviewRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    // Submits a review through the API.
    public async Task<Review> CreateAsync(int rentalId, int itemId, int reviewerId, string comment, int rating)
        => await _apiService.CreateReviewAsync(rentalId, itemId, reviewerId, comment, rating);

    // Retrieves reviews for a specific item.
    public async Task<IEnumerable<Review>> GetByItemIdAsync(int itemId)
        => await _apiService.GetItemReviewsAsync(itemId);

    // Retrieves reviews written for a specific user.
    public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
        => await _apiService.GetUserReviewsAsync(userId);

    // Placeholder for fetching a review by ID when not available from the API.
    public Task<Review?> GetByIdAsync(int id)
        => Task.FromResult<Review?>(null);
}