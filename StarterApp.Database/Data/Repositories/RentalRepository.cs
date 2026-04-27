using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly AppDbContext _context;

    public RentalRepository(AppDbContext context)
    {
        _context = context;
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
            var item =
                await _context.Items.FindAsync(itemId) ?? throw new Exception("Item not found");
            var borrower = await _context.Users.FindAsync(borrowerId);
            if (borrower != null)
                _context.Entry(borrower).State = EntityState.Detached;

            var owner = await _context.Users.FindAsync(item.OwnerId);
            if (owner != null)
                _context.Entry(owner).State = EntityState.Detached;
            var days = (endDate - startDate).Days;
            var rental = new Rental
            {
                ItemId = itemId,
                ItemTitle = item.Title,
                BorrowerId = borrowerId,
                OwnerId = item.OwnerId,
                BorrowerName =
                    borrower != null ? $"{borrower.FirstName} {borrower.LastName}" : string.Empty,
                OwnerName = owner != null ? $"{owner.FirstName} {owner.LastName}" : string.Empty,
                StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc),
                Status = "Requested",
                CreatedAt = DateTime.UtcNow,
                TotalPrice = item.DailyRate * days,
            };

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();
            return rental;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<IEnumerable<Rental>> GetIncomingAsync(int userId)
    {
        return await _context
            .Rentals.Where(r => r.OwnerId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetOutgoingAsync(int userId)
    {
        return await _context
            .Rentals.Where(r => r.BorrowerId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(int rentalId, string status)
    {
        var rental =
            await _context.Rentals.FindAsync(rentalId) ?? throw new Exception("Rental not found");

        rental.Status = status;
        _context.Rentals.Update(rental);
        await _context.SaveChangesAsync();
    }
}
