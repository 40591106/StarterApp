using RentalApp.Database.Models;

namespace RentalApp.Services;

/// <summary>
/// Contract for rental services managing booking status and availability checks.
/// </summary>
public interface IRentalService
{
    /// <summary>
    /// Checks if an item is available for the specified dates.
    /// </summary>
    Task<bool> CanRentItemAsync(int itemId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Creates a new rental request for an item.
    /// </summary>
    Task<Rental> RequestRentalAsync(
        int itemId,
        int borrowerId,
        DateTime startDate,
        DateTime endDate
    );

    /// <summary>
    /// Approves a pending rental request.
    /// </summary>
    Task ApproveRentalAsync(int rentalId);

    /// <summary>
    /// Rejects a pending rental request.
    /// </summary>
    Task RejectRentalAsync(int rentalId);

    /// <summary>
    /// Marks a rental as returned by the borrower.
    /// </summary>
    Task ReturnRentalAsync(int rentalId);

    /// <summary>
    /// Automatically transitions approved rentals to Out for Rent when start date is reached.
    /// </summary>
    Task UpdateOutForRentAsync();

    /// <summary>
    /// Marks a rental as completed after it has been returned.
    /// </summary>
    Task CompleteRentalAsync(int rentalId);

    /// <summary>
    /// Manually marks a rental as Out for Rent.
    /// </summary>
    Task MarkOutForRentAsync(int rentalId);

    /// <summary>
    /// Automatically transitions Out for Rent rentals to Overdue when end date has passed.
    /// </summary>
    Task UpdateOverdueAsync();
}