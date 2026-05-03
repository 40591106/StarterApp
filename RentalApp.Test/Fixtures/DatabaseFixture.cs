using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Data;

namespace RentalApp.Test.Fixtures;

public class DatabaseFixture : IDisposable
{
    public AppDbContext Context { get; private set; }
    public DbContextOptions<AppDbContext> Options { get; private set; }
    // In DatabaseFixture.cs
    public AppDbContext CreateFreshContext() => new AppDbContext(Options);

    public DatabaseFixture()
    {
        Options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new AppDbContext(Options);
        Context.Database.EnsureCreated();
        SeedData();
    }
    // rest stays the same


    private void SeedData()
    {
        Context.Categories.AddRange(
            new Database.Models.Category { Id = 1, Name = "Tools", Slug = "tools" },
            new Database.Models.Category { Id = 2, Name = "Camping", Slug = "camping" },
            new Database.Models.Category { Id = 3, Name = "Sports", Slug = "sports" }
        );

        Context.Users.AddRange(
            new Database.Models.User { Id = 1, FirstName = "Test", LastName = "Owner", Email = "owner@test.com", PasswordHash = "hash", PasswordSalt = "salt" },
            new Database.Models.User { Id = 2, FirstName = "Test", LastName = "Borrower", Email = "borrower@test.com", PasswordHash = "hash", PasswordSalt = "salt" }
        );

        Context.Items.AddRange(
            new Database.Models.Item { Id = 1, Title = "Power Drill", Description = "A drill", DailyRate = 15.00m, CategoryId = 1, OwnerId = 1, Latitude = 55.9533, Longitude = -3.1883 },
            new Database.Models.Item { Id = 2, Title = "Camping Tent", Description = "A tent", DailyRate = 20.00m, CategoryId = 2, OwnerId = 1, Latitude = 55.8642, Longitude = -4.2518 },
            new Database.Models.Item { Id = 3, Title = "Mountain Bike", Description = "A bike", DailyRate = 25.00m, CategoryId = 3, OwnerId = 2, Latitude = 55.9533, Longitude = -3.1883 },
            new Database.Models.Item { Id = 4, Title = "Camping Stove", Description = "A stove", DailyRate = 10.00m, CategoryId = 2, OwnerId = 2, Latitude = 55.8642, Longitude = -4.2518 }
        );

        Context.Rentals.AddRange(
            new Database.Models.Rental { Id = 1, ItemId = 1, ItemTitle = "Power Drill", BorrowerId = 2, BorrowerName = "Test Borrower", OwnerId = 1, OwnerName = "Test Owner", StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(-2), Status = "Overdue", TotalPrice = 45.00m },
            new Database.Models.Rental { Id = 2, ItemId = 2, ItemTitle = "Camping Tent", BorrowerId = 2, BorrowerName = "Test Borrower", OwnerId = 1, OwnerName = "Test Owner", StartDate = DateTime.UtcNow.AddDays(1), EndDate = DateTime.UtcNow.AddDays(3), Status = "Approved", TotalPrice = 40.00m },
            new Database.Models.Rental { Id = 3, ItemId = 3, ItemTitle = "Mountain Bike", BorrowerId = 1, BorrowerName = "Test Owner", OwnerId = 2, OwnerName = "Test Borrower", StartDate = DateTime.UtcNow.AddDays(-3), EndDate = DateTime.UtcNow.AddDays(-1), Status = "Returned", TotalPrice = 50.00m },
            new Database.Models.Rental { Id = 4, ItemId = 4, ItemTitle = "Camping Stove", BorrowerId = 1, BorrowerName = "Test Owner", OwnerId = 2, OwnerName = "Test Borrower", StartDate = DateTime.UtcNow.AddDays(2), EndDate = DateTime.UtcNow.AddDays(4), Status = "Requested", TotalPrice = 20.00m }
        );

        Context.Reviews.AddRange(
    new Database.Models.Review { Id = 1, RentalId = 3, ItemId = 3, ReviewerId = 1, ReviewerName = "Test Owner", Rating = 5, Comment = "Great bike!", ItemTitle = "Mountain Bike" },
    new Database.Models.Review { Id = 2, RentalId = 1, ItemId = 1, ReviewerId = 2, ReviewerName = "Test Borrower", Rating = 4, Comment = "Good drill!", ItemTitle = "Power Drill" }
);

        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}