using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Test.Fixtures;
using Xunit;

namespace RentalApp.Test.Repositories;

public class RentalRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly RentalRepository _repository;

    public RentalRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new RentalRepository(new MockDbContextFactory(_fixture.Options));
    }

    public class GetIncomingAsync : RentalRepositoryTests
    {
        public GetIncomingAsync(DatabaseFixture fixture) : base(fixture) { }
        [Fact]
        public async Task ValidOwnerId_ReturnsIncomingRentals()
        {
            // Arrange
            _fixture.Context.Rentals.Add(new Rental
            {
                ItemId = 1,
                ItemTitle = "Power Drill",
                BorrowerId = 2,
                BorrowerName = "Test Borrower",
                OwnerId = 1,
                OwnerName = "Test Owner",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                Status = "Requested",
                TotalPrice = 30.00m
            });
            await _fixture.Context.SaveChangesAsync();

            // Act
            var rentals = await _repository.GetIncomingAsync(1);

            // Assert
            Assert.Single(rentals);
            Assert.Equal("Power Drill", rentals.First().ItemTitle);
        }

        [Fact]
        public async Task InvalidOwnerId_ReturnsEmpty()
        {
            // Act
            var rentals = await _repository.GetIncomingAsync(999);

            // Assert
            Assert.Empty(rentals);
        }
    }

    public class GetOutgoingAsync : RentalRepositoryTests
    {
        public GetOutgoingAsync(DatabaseFixture fixture) : base(fixture) { }
        [Fact]
        public async Task ValidBorrowerId_ReturnsOutgoingRentals()
        {
            // Arrange
            _fixture.Context.Rentals.Add(new Rental
            {
                ItemId = 1,
                ItemTitle = "Power Drill",
                BorrowerId = 2,
                BorrowerName = "Test Borrower",
                OwnerId = 1,
                OwnerName = "Test Owner",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                Status = "Requested",
                TotalPrice = 30.00m
            });
            await _fixture.Context.SaveChangesAsync();

            // Act
            var rentals = await _repository.GetOutgoingAsync(2);

            // Assert
            Assert.Single(rentals);
        }

        [Fact]
        public async Task InvalidBorrowerId_ReturnsEmpty()
        {
            // Act
            var rentals = await _repository.GetOutgoingAsync(999);

            // Assert
            Assert.Empty(rentals);
        }
    }

    public class UpdateStatusAsync : RentalRepositoryTests
    {
        public UpdateStatusAsync(DatabaseFixture fixture) : base(fixture) { }
        [Theory]
        [InlineData("Approved")]
        [InlineData("Rejected")]
        [InlineData("Returned")]
        [InlineData("Completed")]
        public async Task ValidStatus_UpdatesRental(string status)
        {
            // Arrange
            var rental = new Rental
            {
                ItemId = 1,
                ItemTitle = "Power Drill",
                BorrowerId = 2,
                OwnerId = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                Status = "Requested",
                TotalPrice = 30.00m
            };
            _fixture.Context.Rentals.Add(rental);
            await _fixture.Context.SaveChangesAsync();

            // Act
            await _repository.UpdateStatusAsync(rental.Id, status);

            // Assert
            using var freshContext = _fixture.CreateFreshContext();
            var updated = await freshContext.Rentals.FindAsync(rental.Id);
            Assert.Equal(status, updated!.Status);
        }

        [Fact]
        public async Task InvalidId_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _repository.UpdateStatusAsync(999, "Approved"));
        }
    }

    public class GetByItemIdAsync : RentalRepositoryTests
    {
        public GetByItemIdAsync(DatabaseFixture fixture) : base(fixture) { }

        [Fact]
        public async Task ValidItemId_ReturnsRentals()
        {
            // Arrange
            _fixture.Context.Rentals.Add(new Rental
            {
                ItemId = 1,
                ItemTitle = "Power Drill",
                BorrowerId = 2,
                OwnerId = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                Status = "Requested",
                TotalPrice = 30.00m
            });
            await _fixture.Context.SaveChangesAsync();

            // Act
            var rentals = await _repository.GetByItemIdAsync(1);

            // Assert
            Assert.NotEmpty(rentals);
            Assert.All(rentals, r => Assert.Equal(1, r.ItemId));
        }

        [Fact]
        public async Task InvalidItemId_ReturnsEmpty()
        {
            // Act
            var rentals = await _repository.GetByItemIdAsync(999);

            // Assert
            Assert.Empty(rentals);
        }
    }

    public class GetAllActiveAsync : RentalRepositoryTests
    {
        public GetAllActiveAsync(DatabaseFixture fixture) : base(fixture) { }

        [Theory]
        [InlineData("Approved")]
        [InlineData("Out for Rent")]
        [InlineData("Requested")]
        public async Task ActiveStatuses_IncludedInResults(string status)
        {
            // Arrange
            _fixture.Context.Rentals.Add(new Rental
            {
                ItemId = 1,
                BorrowerId = 2,
                OwnerId = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                Status = status,
                TotalPrice = 30.00m
            });
            await _fixture.Context.SaveChangesAsync();

            // Act
            var rentals = await _repository.GetAllActiveAsync();

            // Assert
            Assert.Contains(rentals, r => r.Status == status);
        }

        [Theory]
        [InlineData("Completed")]
        [InlineData("Rejected")]
        public async Task InactiveStatuses_ExcludedFromResults(string status)
        {
            // Arrange
            _fixture.Context.Rentals.Add(new Rental
            {
                ItemId = 1,
                BorrowerId = 2,
                OwnerId = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                Status = status,
                TotalPrice = 30.00m
            });
            await _fixture.Context.SaveChangesAsync();

            // Act
            var rentals = await _repository.GetAllActiveAsync();

            // Assert
            Assert.DoesNotContain(rentals, r => r.Status == status);
        }
    }

    public class CreateAsync : RentalRepositoryTests
    {
        public CreateAsync(DatabaseFixture fixture) : base(fixture) { }

        [Fact]
        public async Task ValidRental_CreatesAndReturnsRental()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(3);

            // Act
            var rental = await _repository.CreateAsync(1, startDate, endDate, 2);

            // Assert
            Assert.NotEqual(0, rental.Id);
            Assert.Equal("Requested", rental.Status);
            Assert.Equal(1, rental.ItemId);
            Assert.Equal(2, rental.BorrowerId);
        }

        [Fact]
        public async Task InvalidItemId_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _repository.CreateAsync(999, DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(3), 2));
        }
    }
}