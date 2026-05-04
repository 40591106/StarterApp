using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.Repositories;

// Review repository that routes review operations through the shared API.
/// <summary>
/// API implementation of IReviewRepository using HttpClient to communicate with the shared API.
/// </summary>
public class ApiReviewRepository : IReviewRepository
{
    private readonly IApiService _apiService;

    // Initializes the review repository with the API service dependency.
    public ApiReviewRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    // Submits a review through the API.
    /// <inheritdoc/>
    public async Task<Review> CreateAsync(int rentalId, int itemId, int userId, string comment, int rating)
        => await _apiService.CreateReviewAsync(rentalId, itemId, userId, comment, rating);

    // Retrieves reviews for a specific item.
    /// <inheritdoc/>
    public async Task<IEnumerable<Review>> GetByItemIdAsync(int itemId)
        => await _apiService.GetItemReviewsAsync(itemId);

    // Retrieves reviews written for a specific user.
    /// <inheritdoc/>
    public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
        => await _apiService.GetUserReviewsAsync(userId);

    // Placeholder for fetching a review by ID when not available from the API.
    /// <inheritdoc/>
    public Task<Review?> GetByIdAsync(int id)
        => Task.FromResult<Review?>(null);
}