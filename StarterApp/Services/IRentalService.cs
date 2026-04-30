using StarterApp.Database.Models;

namespace StarterApp.Services;

public interface IRentalService
{
    Task<bool> CanRentItemAsync(int itemId, DateTime startDate, DateTime endDate);
    Task<Rental> RequestRentalAsync(
        int itemId,
        int borrowerId,
        DateTime startDate,
        DateTime endDate
    );
    Task ApproveRentalAsync(int rentalId);
    Task RejectRentalAsync(int rentalId);
    Task ReturnRentalAsync(int rentalId);
    Task UpdateOutForRentAsync();
    Task CompleteRentalAsync(int rentalId);
    Task MarkOutForRentAsync(int rentalId);
}
