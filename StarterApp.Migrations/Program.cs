using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Data;
using StarterApp.Database.Models;

Console.WriteLine("Running migrations...");
using var context = new AppDbContext();
await context.Database.MigrateAsync();
Console.WriteLine("Migrations complete.");

// Seed categories first
if (!context.Categories.Any())
{
    context.Categories.AddRange(
        new Category
        {
            Id = 1,
            Name = "Tools",
            Slug = "tools",
        },
        new Category
        {
            Id = 2,
            Name = "Camping",
            Slug = "camping",
        },
        new Category
        {
            Id = 3,
            Name = "Sports",
            Slug = "sports",
        },
        new Category
        {
            Id = 4,
            Name = "Electronics",
            Slug = "electronics",
        },
        new Category
        {
            Id = 5,
            Name = "Games",
            Slug = "games",
        }
    );
    await context.SaveChangesAsync();
}

// Seed users
if (!context.Users.Any())
{
    var salt = BCrypt.Net.BCrypt.GenerateSalt();
    var hash = BCrypt.Net.BCrypt.HashPassword("Password1!", salt);
    context.Users.Add(
        new User
        {
            Id = 8,
            FirstName = "Test",
            LastName = "User",
            Email = "testadmin@gmail.com",
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
        }
    );
    await context.SaveChangesAsync();
}

// Seed items last (depends on categories and users)
if (!context.Items.Any())
{
    context.Items.AddRange(
        new Item
        {
            Title = "Power Drill",
            Description = "A powerful drill for all your DIY needs.",
            DailyRate = 15.00m,
            CategoryId = 1,
            Latitude = 55.9533,
            Longitude = -3.1883,
            OwnerId = 8,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        },
        new Item
        {
            Title = "Camping Tent",
            Description = "4 person tent, great for festivals.",
            DailyRate = 20.00m,
            CategoryId = 2,
            Latitude = 55.8642,
            Longitude = -4.2518,
            OwnerId = 8,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        },
        new Item
        {
            Title = "Board Game Collection",
            Description = "Selection of popular board games.",
            DailyRate = 5.00m,
            CategoryId = 3,
            Latitude = 55.8864,
            Longitude = -3.5217,
            OwnerId = 8,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        }
    );
    await context.SaveChangesAsync();
}
