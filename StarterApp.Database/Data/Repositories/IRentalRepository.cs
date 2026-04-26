using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public interface IRentalRepository
{
    Task<Rental> CreateAsync(int itemId, DateTime startDate, DateTime endDate, int borrowerId);
    Task<IEnumerable<Rental>> GetIncomingAsync(int userId);
    Task<IEnumerable<Rental>> GetOutgoingAsync(int userId);
}