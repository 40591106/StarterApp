using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.Repositories;

// Rental repository that forwards rental operations to the shared API.
public class ApiRentalRepository : IRentalRepository
{
    private readonly IApiService _apiService;

    // Creates the rental repository with the API service dependency.
    public ApiRentalRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    // Creates a new rental request using the API.
    public async Task<Rental> CreateAsync(int itemId, DateTime startDate, DateTime endDate, int borrowerId)
        => await _apiService.RequestRentalAsync(itemId, startDate, endDate);

    // Gets incoming rentals for the current user.
    public async Task<IEnumerable<Rental>> GetIncomingAsync(int userId)
        => await _apiService.GetIncomingRentalsAsync();

    // Gets outgoing rentals for the current user.
    public async Task<IEnumerable<Rental>> GetOutgoingAsync(int userId)
        => await _apiService.GetOutgoingRentalsAsync();

    // Updates the status of an existing rental.
    public async Task UpdateStatusAsync(int rentalId, string status)
        => await _apiService.UpdateRentalStatusAsync(rentalId, status);

    // Placeholder for fetching rentals by item ID when not available from the API.
    public Task<IEnumerable<Rental>> GetByItemIdAsync(int itemId)
        => Task.FromResult(Enumerable.Empty<Rental>());

    // Placeholder for fetching all active rentals when not available from the API.
    public Task<IEnumerable<Rental>> GetAllActiveAsync()
        => Task.FromResult(Enumerable.Empty<Rental>());

    // Placeholder for fetching a rental by ID when not available from the API.
    public Task<Rental?> GetByIdAsync(int id)
        => Task.FromResult<Rental?>(null);
}