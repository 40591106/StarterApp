using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public RentalRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

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

    public async Task<IEnumerable<Rental>> GetIncomingAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context
            .Rentals.Where(r => r.OwnerId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetOutgoingAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context
            .Rentals.Where(r => r.BorrowerId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByItemIdAsync(int itemId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Rentals.Where(r => r.ItemId == itemId).ToListAsync();
    }

    public async Task UpdateStatusAsync(int rentalId, string status)
    {
        using var context = _contextFactory.CreateDbContext();
        var rental =
            await context.Rentals.FindAsync(rentalId) ?? throw new Exception("Rental not found");
        rental.Status = status;
        context.Rentals.Update(rental);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Rental>> GetAllActiveAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context
            .Rentals.Where(r =>
                r.Status == "Approved" || r.Status == "Out for Rent" || r.Status == "Requested"
            )
            .ToListAsync();
    }
}
