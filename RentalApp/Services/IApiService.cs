using RentalApp.Database.Models;

namespace RentalApp.Services;

public interface IApiService
{
    // Items
    Task<List<Item>> GetItemsAsync(string? category = null, string? search = null, int page = 1);
    Task<Item?> GetItemByIdAsync(int id);
    Task<Item> CreateItemAsync(CreateItemRequest request);
    Task<Item> UpdateItemAsync(int id, UpdateItemRequest request);
    Task<List<Item>> GetNearbyItemsAsync(double lat, double lon, double radiusKm);

    // Categories
    Task<List<Category>> GetCategoriesAsync();

    // Rentals
    Task<Rental> RequestRentalAsync(int itemId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Rental>> GetIncomingRentalsAsync();
    Task<IEnumerable<Rental>> GetOutgoingRentalsAsync();
    Task UpdateRentalStatusAsync(int rentalId, string status);


    // Reviews
    Task<Review> CreateReviewAsync(int rentalId, int itemId, int reviewerId, string comment, int rating);
    Task<IEnumerable<Review>> GetItemReviewsAsync(int itemId);
    Task<IEnumerable<Review>> GetUserReviewsAsync(int userId);
}

public record CreateItemRequest(
    string Title,
    string Description,
    decimal DailyRate,
    int CategoryId,
    double Latitude,
    double Longitude
);

public record UpdateItemRequest(
    string Title,
    string Description,
    decimal DailyRate,
    bool IsAvailable
);