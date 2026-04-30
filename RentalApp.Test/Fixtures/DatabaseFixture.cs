using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Data;

namespace RentalApp.Test.Fixtures;

public class DatabaseFixture : IDisposable
{
    public AppDbContext Context { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new AppDbContext(options);
        Context.Database.EnsureCreated();
        SeedData();
    }

    private void SeedData()
    {
        Context.Categories.AddRange(
            new Database.Models.Category { Id = 1, Name = "Tools", Slug = "tools" },
            new Database.Models.Category { Id = 2, Name = "Camping", Slug = "camping" }
        );

        Context.Users.AddRange(
            new Database.Models.User { Id = 1, FirstName = "Test", LastName = "Owner", Email = "owner@test.com", PasswordHash = "hash", PasswordSalt = "salt" },
            new Database.Models.User { Id = 2, FirstName = "Test", LastName = "Borrower", Email = "borrower@test.com", PasswordHash = "hash", PasswordSalt = "salt" }
        );

        Context.Items.AddRange(
            new Database.Models.Item { Id = 1, Title = "Power Drill", Description = "A drill", DailyRate = 15.00m, CategoryId = 1, OwnerId = 1, Latitude = 55.9533, Longitude = -3.1883 },
            new Database.Models.Item { Id = 2, Title = "Camping Tent", Description = "A tent", DailyRate = 20.00m, CategoryId = 2, OwnerId = 1, Latitude = 55.8642, Longitude = -4.2518 }
        );

        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}