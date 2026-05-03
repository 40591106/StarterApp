using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

// Repository implementation for rental persistence using the database context.
public class RentalRepository : IRentalRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    // Initializes the rental repository with a database context factory.
    public RentalRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // Creates a rental record for the specified item and borrower.
    public async Task<Rental> CreateAsync(
        int itemId,
        DateTime startDate,
        DateTime endDate,
        int borrowerId
    )
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var item =
                await context.Items.FindAsync(itemId) ?? throw new Exception("Item not found");

            var borrowerName =
                await context
                    .Users.Where(u => u.Id == borrowerId)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefaultAsync()
                ?? string.Empty;

            var ownerName =
                await context
                    .Users.Where(u => u.Id == item.OwnerId)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefaultAsync()
                ?? string.Empty;

            var days = (endDate - startDate).Days;
            var rental = new Rental
            {
                ItemId = itemId,
                ItemTitle = item.Title,
                BorrowerId = borrowerId,
                OwnerId = item.OwnerId,
                BorrowerName = borrowerName,
                OwnerName = ownerName,
                StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc),
                Status = "Requested",
                CreatedAt = DateTime.UtcNow,
                TotalPrice = item.DailyRate * days,
            };

            context.Rentals.Add(rental);
            await context.SaveChangesAsync();
            return rental;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.InnerException?.Message ?? ex.Message);
        }
    }

    // Gets incoming rental requests for the specified owner.
    public async Task<IEnumerable<Rental>> GetIncomingAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context
            .Rentals.Where(r => r.OwnerId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    // Gets outgoing rental requests for the specified borrower.
    public async Task<IEnumerable<Rental>> GetOutgoingAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context
            .Rentals.Where(r => r.BorrowerId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    // Gets rentals associated with a specific item.
    public async Task<IEnumerable<Rental>> GetByItemIdAsync(int itemId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Rentals.Where(r => r.ItemId == itemId).ToListAsync();
    }

    // Updates the status of an existing rental record.
    public async Task UpdateStatusAsync(int rentalId, string status)
    {
        using var context = _contextFactory.CreateDbContext();
        var rental =
            await context.Rentals.FindAsync(rentalId) ?? throw new Exception("Rental not found");
        rental.Status = status;
        context.Rentals.Update(rental);
        await context.SaveChangesAsync();
    }

    // Gets all active rentals that are requested, approved, or out for rent.
    public async Task<IEnumerable<Rental>> GetAllActiveAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context
            .Rentals.Where(r =>
                r.Status == "Approved" || r.Status == "Out for Rent" || r.Status == "Requested" || r.Status == "Overdue"
            )
            .ToListAsync();
    }

    // Gets a rental by its ID.
    public async Task<Rental?> GetByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Rentals.FindAsync(id);
    }
}
