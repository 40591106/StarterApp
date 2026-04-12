using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Data;
using StarterApp.Database.Models;

Console.WriteLine("Running migrations...");
using var context = new AppDbContext();
await context.Database.MigrateAsync();
Console.WriteLine("Migrations complete.");

// Populate local database with user
if (!context.Users.Any())
{
    context.Users.Add(new StarterApp.Database.Models.User
    {
        Id = 8,
        FirstName = "Test",
        LastName = "User",
        Email = "test@test.com",
        PasswordHash = "placeholder",
        PasswordSalt = "placeholder",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        IsActive = true
    });
    await context.SaveChangesAsync();
}

// Populate local database with items
if (!context.Items.Any())
{
    context.Items.AddRange(
        new StarterApp.Database.Models.Item
        {
            Title = "Power Drill",
            Description = "A powerful drill for all your DIY needs.",
            DailyRate = 15.00m,
            CategoryId = 1,
            Location = "Edinburgh",
            OwnerId = 8,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new StarterApp.Database.Models.Item
        {
            Title = "Camping Tent",
            Description = "4 person tent, great for festivals.",
            DailyRate = 20.00m,
            CategoryId = 2,
            Location = "Glasgow",
            OwnerId = 8,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new StarterApp.Database.Models.Item
        {
            Title = "Board Game Collection",
            Description = "Selection of popular board games.",
            DailyRate = 5.00m,
            CategoryId = 3,
            Location = "Livingston",
            OwnerId = 8,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }
    );
    await context.SaveChangesAsync();
}