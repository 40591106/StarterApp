using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Data;

namespace RentalApp.Test.Fixtures;

public class MockDbContextFactory : IDbContextFactory<AppDbContext>
{
    private readonly AppDbContext _context;

    public MockDbContextFactory(AppDbContext context)
    {
        _context = context;
    }

    public AppDbContext CreateDbContext() => _context;
}