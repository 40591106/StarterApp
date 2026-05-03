using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

namespace RentalApp.Services;

// Service that applies rental business rules and delegates persistence to the repository.
public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IItemRepository _itemRepository;

    // Creates the rental service with repository dependencies.
    public RentalService(IRentalRepository rentalRepository, IItemRepository itemRepository)
    {
        _rentalRepository = rentalRepository;
        _itemRepository = itemRepository;
    }

    // Determines whether an item can be rented for the requested date range.
    public async Task<bool> CanRentItemAsync(int itemId, DateTime startDate, DateTime endDate)
    {
        var startUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

        var existingRentals = await _rentalRepository.GetByItemIdAsync(itemId);
        return !existingRentals.Any(r =>
            (r.Status == "Approved" || r.Status == "Out for Rent")
            && r.StartDate < endUtc
            && r.EndDate > startUtc
        );
    }

    // Requests a rental if the item is available.
    public async Task<Rental> RequestRentalAsync(
        int itemId,
        int borrowerId,
        DateTime startDate,
        DateTime endDate
    )
    {
        var canRent = await CanRentItemAsync(itemId, startDate, endDate);
        if (!canRent)
            throw new Exception("Item is not available for the selected dates.");

        return await _rentalRepository.CreateAsync(itemId, startDate, endDate, borrowerId);
    }

    // Approves a rental request.
    public async Task ApproveRentalAsync(int rentalId)
    {
        await _rentalRepository.UpdateStatusAsync(rentalId, "Approved");
    }

    // Rejects a rental request.
    public async Task RejectRentalAsync(int rentalId)
    {
        await _rentalRepository.UpdateStatusAsync(rentalId, "Rejected");
    }

    // Marks a rental as out for rent when it starts.
    public async Task MarkOutForRentAsync(int rentalId)
    {
        await _rentalRepository.UpdateStatusAsync(rentalId, "Out for Rent");
    }

    // Marks a rental as returned.
    public async Task ReturnRentalAsync(int rentalId)
    {
        await _rentalRepository.UpdateStatusAsync(rentalId, "Returned");
    }

    // Advances approved rentals to out-for-rent when their start date arrives.
    public async Task UpdateOutForRentAsync()
    {
        var rentals = await _rentalRepository.GetAllActiveAsync();
        foreach (
            var rental in rentals.Where(r =>
                r.Status == "Approved" && r.StartDate <= DateTime.UtcNow
            )
        )
        {
            await _rentalRepository.UpdateStatusAsync(rental.Id, "Out for Rent");
        }
    }

    // Completes a rental and updates its status.
    public async Task CompleteRentalAsync(int rentalId)
    {
        await _rentalRepository.UpdateStatusAsync(rentalId, "Completed");
    }
}
