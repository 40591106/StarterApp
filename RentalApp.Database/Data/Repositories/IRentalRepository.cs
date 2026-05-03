using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

/// <summary>
/// Repository interface for rental persistence operations.
/// </summary>
public interface IRentalRepository : IRepository<Rental>
{
    /// <summary>
    /// Creates a new rental request record, calculating total price from daily rate and duration.
    /// </summary>
    Task<Rental> CreateAsync(int itemId, DateTime startDate, DateTime endDate, int borrowerId);

    /// <summary>
    /// Gets all rentals where the specified user is the item owner.
    /// </summary>
    Task<IEnumerable<Rental>> GetIncomingAsync(int userId);

    /// <summary>
    /// Gets all rentals where the specified user is the borrower.
    /// </summary>
    Task<IEnumerable<Rental>> GetOutgoingAsync(int userId);

    /// <summary>
    /// Gets all rentals for a specific item, used for double-booking validation.
    /// </summary>
    Task<IEnumerable<Rental>> GetByItemIdAsync(int itemId);

    /// <summary>
    /// Gets all active rentals with status Requested, Approved, Out for Rent or Overdue.
    /// </summary>
    Task<IEnumerable<Rental>> GetAllActiveAsync();

    /// <summary>
    /// Updates the status of a rental record.
    /// </summary>
    Task UpdateStatusAsync(int rentalId, string status);
}