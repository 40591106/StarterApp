using RentalApp.Database.Models;

namespace RentalApp.Services;

/// <summary>
/// Contract for the shared API service handling all HTTP communication with the SET09102 API.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Gets all items, optionally filtered by category slug or search term.
    /// </summary>
    Task<List<Item>> GetItemsAsync(string? category = null, string? search = null, int page = 1);

    /// <summary>
    /// Gets a single item by its ID.
    /// </summary>
    Task<Item?> GetItemByIdAsync(int id);

    /// <summary>
    /// Creates a new item listing via the API.
    /// </summary>
    Task<Item> CreateItemAsync(CreateItemRequest request);

    /// <summary>
    /// Updates an existing item listing via the API.
    /// </summary>
    Task<Item> UpdateItemAsync(int id, UpdateItemRequest request);

    /// <summary>
    /// Gets items within the specified radius using PostGIS spatial queries via the API.
    /// </summary>
    Task<List<Item>> GetNearbyItemsAsync(double lat, double lon, double radiusKm);

    /// <summary>
    /// Gets all available item categories.
    /// </summary>
    Task<List<Category>> GetCategoriesAsync();

    /// <summary>
    /// Creates a new rental request via the API.
    /// </summary>
    Task<Rental> RequestRentalAsync(int itemId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets all incoming rentals for the current authenticated user.
    /// </summary>
    Task<IEnumerable<Rental>> GetIncomingRentalsAsync();

    /// <summary>
    /// Gets all outgoing rentals for the current authenticated user.
    /// </summary>
    Task<IEnumerable<Rental>> GetOutgoingRentalsAsync();

    /// <summary>
    /// Updates the status of a rental via the API.
    /// </summary>
    Task UpdateRentalStatusAsync(int rentalId, string status);

    /// <summary>
    /// Creates a review for a completed rental via the API.
    /// </summary>
    Task<Review> CreateReviewAsync(int rentalId, int itemId, int reviewerId, string comment, int rating);

    /// <summary>
    /// Gets all reviews for a specific item.
    /// </summary>
    Task<IEnumerable<Review>> GetItemReviewsAsync(int itemId);

    /// <summary>
    /// Gets all reviews written by a specific user.
    /// </summary>
    Task<IEnumerable<Review>> GetUserReviewsAsync(int userId);
}

/// <summary>
/// Request model for creating a new item listing.
/// </summary>
public record CreateItemRequest(
    string Title,
    string Description,
    decimal DailyRate,
    int CategoryId,
    double Latitude,
    double Longitude
);

/// <summary>
/// Request model for updating an existing item listing.
/// </summary>
public record UpdateItemRequest(
    string Title,
    string Description,
    decimal DailyRate,
    bool IsAvailable
);