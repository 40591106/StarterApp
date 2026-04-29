using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ReviewRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Review> CreateAsync(
        int rentalId,
        int itemId,
        int reviewerId,
        string comment,
        int rating
    )

    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var item =
                await context.Items.FindAsync(itemId) ?? throw new Exception("Item not found");

            var reviewerName =
                await context
                    .Users.Where(u => u.Id == reviewerId)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefaultAsync()
                ?? string.Empty;

            var review = new Review
            {
                ItemId = itemId,
                ItemTitle = item.Title,
                ReviewerId = reviewerId,
                ReviewerName = reviewerName,
                RentalId = rentalId,
                Comment = comment,
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();
            return review;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<IEnumerable<Review>> GetByItemIdAsync(int itemId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Reviews.Where(r => r.ItemId == itemId).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Reviews.Where(r => r.ReviewerId == userId).ToListAsync();
    }
}