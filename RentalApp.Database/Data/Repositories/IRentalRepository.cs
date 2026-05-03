// IRentalRepository.cs
using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

// Repository interface for rental persistence operations.
public interface IRentalRepository : IRepository<Rental>
{
    // Creates a new rental request record.
    Task<Rental> CreateAsync(int itemId, DateTime startDate, DateTime endDate, int borrowerId);

    // Gets rentals where the current user is the owner.
    Task<IEnumerable<Rental>> GetIncomingAsync(int userId);

    // Gets rentals where the current user is the borrower.
    Task<IEnumerable<Rental>> GetOutgoingAsync(int userId);

    // Gets rentals filtered by item ID.
    Task<IEnumerable<Rental>> GetByItemIdAsync(int itemId);

    // Gets all active rental records.
    Task<IEnumerable<Rental>> GetAllActiveAsync();

    // Updates the rental status.
    Task UpdateStatusAsync(int rentalId, string status);
}