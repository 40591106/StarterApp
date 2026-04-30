using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

public interface IRentalRepository
{
    Task<Rental> CreateAsync(int itemId, DateTime startDate, DateTime endDate, int borrowerId);
    Task<IEnumerable<Rental>> GetIncomingAsync(int userId);
    Task<IEnumerable<Rental>> GetOutgoingAsync(int userId);
    Task<IEnumerable<Rental>> GetByItemIdAsync(int itemId);
    Task<IEnumerable<Rental>> GetAllActiveAsync();
    Task UpdateStatusAsync(int rentalId, string status);
}
