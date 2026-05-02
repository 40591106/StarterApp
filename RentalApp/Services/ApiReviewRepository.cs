using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

namespace RentalApp.Services;

public class ApiReviewRepository : IReviewRepository
{
    private readonly IApiService _apiService;

    public ApiReviewRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<Review> CreateAsync(int rentalId, int itemId, int reviewerId, string comment, int rating)
        => await _apiService.CreateReviewAsync(rentalId, itemId, reviewerId, comment, rating);

    public async Task<IEnumerable<Review>> GetByItemIdAsync(int itemId)
        => await _apiService.GetItemReviewsAsync(itemId);

    public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
        => await _apiService.GetUserReviewsAsync(userId);
}