using StarterApp.Database.Models;

namespace StarterApp.Services;

public interface IApiService
{
    // Authentication (handled by ApiAuthenticationService)
    //Task<User> RegisterAsync(string firstName, string lastName, string email, string password);
    //Task<AuthToken> LoginAsync(string email, string password);
    //Task<User> GetCurrentUserAsync();
    //Task<User> GetUserProfileAsync(int userId);

    // Items (Week 1)
    Task<List<Item>> GetItemsAsync(string? category = null, string? search = null, int page = 1);
    Task<Item?> GetItemByIdAsync(int id);
    Task<Item> CreateItemAsync(CreateItemRequest request);
    Task<Item> UpdateItemAsync(int id, UpdateItemRequest request);

    // Nearby Items (Week 2)
    //Task<List<Item>> GetNearbyItemsAsync(double lat, double lon, double radius = 5.0, string? category = null);

    // Categories (Week 1)
    //Task<List<Category>> GetCategoriesAsync();

    // Rentals (Week 3)
    //Task<Rental> RequestRentalAsync(int itemId, DateTime startDate, DateTime endDate);
    //Task<List<Rental>> GetIncomingRentalsAsync(string? status = null);
    //Task<List<Rental>> GetOutgoingRentalsAsync(string? status = null);
    //Task<Rental> GetRentalAsync(int id);
    //Task UpdateRentalStatusAsync(int rentalId, string status);

    // Reviews (Week 4)
    //Task<Review> CreateReviewAsync(int rentalId, int rating, string comment);
    //Task<List<Review>> GetItemReviewsAsync(int itemId, int page = 1);
    //Task<List<Review>> GetUserReviewsAsync(int userId, int page = 1);
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