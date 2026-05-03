using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

namespace RentalApp.Services;

/// <summary>
/// Service that applies rental business rules and delegates persistence to the repository.
/// </summary>
public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IItemRepository _itemRepository;

    /// <summary>
    /// Creates the rental service with repository dependencies.
    /// </summary>
    public RentalService(IRentalRepository rentalRepository, IItemRepository itemRepository)
    {
        _rentalRepository = rentalRepository;
        _itemRepository = itemRepository;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task ApproveRentalAsync(int rentalId)
    {
        await _rentalRepository.UpdateStatusAsync(rentalId, "Approved");
    }

    /// <inheritdoc/>
    public async Task RejectRentalAsync(int rentalId)
    {
        await _rentalRepository.UpdateStatusAsync(rentalId, "Rejected");
    }

    /// <inheritdoc/>
    public async Task MarkOutForRentAsync(int rentalId)
    {
        await _rentalRepository.UpdateStatusAsync(rentalId, "Out for Rent");
    }

    /// <inheritdoc/>
    public async Task ReturnRentalAsync(int rentalId)
    {
        await _rentalRepository.UpdateStatusAsync(rentalId, "Returned");
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task CompleteRentalAsync(int rentalId)
    {
        await _rentalRepository.UpdateStatusAsync(rentalId, "Completed");
    }

    /// <inheritdoc/>
    public async Task UpdateOverdueAsync()
    {
        var rentals = await _rentalRepository.GetAllActiveAsync();
        foreach (var rental in rentals.Where(r =>
            r.Status == "Out for Rent" && r.EndDate < DateTime.UtcNow))
        {
            await _rentalRepository.UpdateStatusAsync(rental.Id, "Overdue");
        }
    }
}