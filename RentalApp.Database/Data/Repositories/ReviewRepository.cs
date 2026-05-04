using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

// Repository implementation for review persistence using the database context.
/// <summary>
/// Local database implementation of IReviewRepository using Entity Framework Core.
/// </summary>
public class ReviewRepository : IReviewRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    // Initializes the review repository with a database context factory.
    public ReviewRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // Creates a review record for the specified rental and item.
    /// <inheritdoc/>
    public async Task<Review> CreateAsync(
        int rentalId,
        int itemId,
        int userId,
        string comment,
        int rating
    )

    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var item =
                await context.Items.FindAsync(itemId) ?? throw new Exception("Item not found");

            var existing = await context.Reviews.FirstOrDefaultAsync(r => r.RentalId == rentalId);
            if (existing != null)
                throw new Exception("You have already reviewed this rental.");

            var reviewerName =
                await context
                    .Users.Where(u => u.Id == userId)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefaultAsync()
                ?? string.Empty;

            var review = new Review
            {
                ItemId = itemId,
                ItemTitle = item.Title,
                ReviewerId = userId,
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

    // Gets reviews associated with a specific item.
    /// <inheritdoc/>
    public async Task<IEnumerable<Review>> GetByItemIdAsync(int itemId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Reviews.Where(r => r.ItemId == itemId).ToListAsync();
    }

    // Gets reviews for items owned by the specified user.
    /// <inheritdoc/>
    public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        var ownerItemIds = await context.Items
            .Where(i => i.OwnerId == userId)
            .Select(i => i.Id)
            .ToListAsync();

        return await context.Reviews
            .Where(r => ownerItemIds.Contains(r.ItemId))
            .ToListAsync();
    }

    // Gets a review by its ID.
    /// <inheritdoc/>
    public async Task<Review?> GetByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Reviews.FindAsync(id);
    }
}