using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

namespace RentalApp.Services;

public class ApiRentalRepository : IRentalRepository
{
    private readonly IApiService _apiService;

    public ApiRentalRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<Rental> CreateAsync(int itemId, DateTime startDate, DateTime endDate, int borrowerId)
        => await _apiService.RequestRentalAsync(itemId, startDate, endDate);

    public async Task<IEnumerable<Rental>> GetIncomingAsync(int userId)
        => await _apiService.GetIncomingRentalsAsync();

    public async Task<IEnumerable<Rental>> GetOutgoingAsync(int userId)
        => await _apiService.GetOutgoingRentalsAsync();

    public async Task UpdateStatusAsync(int rentalId, string status)
        => await _apiService.UpdateRentalStatusAsync(rentalId, status);

    public async Task<IEnumerable<Rental>> GetByItemIdAsync(int itemId)
        => await _apiService.GetByItemIdAsync(itemId);

    public Task<IEnumerable<Rental>> GetAllActiveAsync()
        => Task.FromResult(Enumerable.Empty<Rental>());
}