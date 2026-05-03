// RentalApp.Test/Services/RentalServiceTests.cs
using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using Xunit;

namespace RentalApp.Test.Services;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _rentalRepo;
    private readonly Mock<IItemRepository> _itemRepo;
    private readonly RentalService _service;

    public RentalServiceTests()
    {
        _rentalRepo = new Mock<IRentalRepository>();
        _itemRepo = new Mock<IItemRepository>();
        _service = new RentalService(_rentalRepo.Object, _itemRepo.Object);
    }

    public class CanRentItemAsync : RentalServiceTests
    {
        public CanRentItemAsync() : base() { }

        [Fact]
        public async Task NoExistingRentals_ReturnsTrue()
        {
            // Arrange
            _rentalRepo.Setup(r => r.GetByItemIdAsync(1))
                .ReturnsAsync(new List<Rental>());

            // Act
            var result = await _service.CanRentItemAsync(1,
                DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task OverlappingApprovedRental_ReturnsFalse()
        {
            // Arrange
            _rentalRepo.Setup(r => r.GetByItemIdAsync(1))
                .ReturnsAsync(new List<Rental>
                {
                    new Rental
                    {
                        Status = "Approved",
                        StartDate = DateTime.UtcNow.AddDays(1),
                        EndDate = DateTime.UtcNow.AddDays(5)
                    }
                });

            // Act
            var result = await _service.CanRentItemAsync(1,
                DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(4));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task OverlappingOutForRentRental_ReturnsFalse()
        {
            // Arrange
            _rentalRepo.Setup(r => r.GetByItemIdAsync(1))
                .ReturnsAsync(new List<Rental>
                {
                    new Rental
                    {
                        Status = "Out for Rent",
                        StartDate = DateTime.UtcNow.AddDays(1),
                        EndDate = DateTime.UtcNow.AddDays(5)
                    }
                });

            // Act
            var result = await _service.CanRentItemAsync(1,
                DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(4));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task OverlappingRequestedRental_ReturnsTrue()
        {
            // Arrange - "Requested" status does NOT block availability
            _rentalRepo.Setup(r => r.GetByItemIdAsync(1))
                .ReturnsAsync(new List<Rental>
                {
                    new Rental
                    {
                        Status = "Requested",
                        StartDate = DateTime.UtcNow.AddDays(1),
                        EndDate = DateTime.UtcNow.AddDays(5)
                    }
                });

            // Act
            var result = await _service.CanRentItemAsync(1,
                DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(4));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AdjacentDates_ReturnsTrue()
        {
            // Arrange - ends exactly when new one starts (no overlap)
            _rentalRepo.Setup(r => r.GetByItemIdAsync(1))
                .ReturnsAsync(new List<Rental>
                {
                    new Rental
                    {
                        Status = "Approved",
                        StartDate = DateTime.UtcNow.AddDays(1),
                        EndDate = DateTime.UtcNow.AddDays(3)
                    }
                });

            // Act
            var result = await _service.CanRentItemAsync(1,
                DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(5));

            // Assert
            Assert.True(result);
        }
    }

    public class RequestRentalAsync : RentalServiceTests
    {
        public RequestRentalAsync() : base() { }

        [Fact]
        public async Task ItemAvailable_CreatesRental()
        {
            // Arrange
            var expected = new Rental { Id = 1, Status = "Requested" };
            _rentalRepo.Setup(r => r.GetByItemIdAsync(1))
                .ReturnsAsync(new List<Rental>());
            _rentalRepo.Setup(r => r.CreateAsync(1,
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), 2))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.RequestRentalAsync(1, 2,
                DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));

            // Assert
            Assert.Equal(expected.Id, result.Id);
            _rentalRepo.Verify(r => r.CreateAsync(1,
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), 2), Times.Once);
        }

        [Fact]
        public async Task ItemUnavailable_ThrowsException()
        {
            // Arrange
            _rentalRepo.Setup(r => r.GetByItemIdAsync(1))
                .ReturnsAsync(new List<Rental>
                {
                    new Rental
                    {
                        Status = "Approved",
                        StartDate = DateTime.UtcNow.AddDays(1),
                        EndDate = DateTime.UtcNow.AddDays(5)
                    }
                });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.RequestRentalAsync(1, 2,
                    DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(4)));
        }
    }

    public class StatusTransitions : RentalServiceTests
    {
        public StatusTransitions() : base() { }

        [Fact]
        public async Task ApproveRentalAsync_CallsUpdateWithApproved()
        {
            // Arrange
            _rentalRepo.Setup(r => r.UpdateStatusAsync(1, "Approved"))
                .Returns(Task.CompletedTask);

            // Act
            await _service.ApproveRentalAsync(1);

            // Assert
            _rentalRepo.Verify(r => r.UpdateStatusAsync(1, "Approved"), Times.Once);
        }

        [Fact]
        public async Task RejectRentalAsync_CallsUpdateWithRejected()
        {
            _rentalRepo.Setup(r => r.UpdateStatusAsync(1, "Rejected"))
                .Returns(Task.CompletedTask);

            await _service.RejectRentalAsync(1);

            _rentalRepo.Verify(r => r.UpdateStatusAsync(1, "Rejected"), Times.Once);
        }

        [Fact]
        public async Task MarkOutForRentAsync_CallsUpdateWithOutForRent()
        {
            _rentalRepo.Setup(r => r.UpdateStatusAsync(1, "Out for Rent"))
                .Returns(Task.CompletedTask);

            await _service.MarkOutForRentAsync(1);

            _rentalRepo.Verify(r => r.UpdateStatusAsync(1, "Out for Rent"), Times.Once);
        }

        [Fact]
        public async Task ReturnRentalAsync_CallsUpdateWithReturned()
        {
            _rentalRepo.Setup(r => r.UpdateStatusAsync(1, "Returned"))
                .Returns(Task.CompletedTask);

            await _service.ReturnRentalAsync(1);

            _rentalRepo.Verify(r => r.UpdateStatusAsync(1, "Returned"), Times.Once);
        }

        [Fact]
        public async Task CompleteRentalAsync_CallsUpdateWithCompleted()
        {
            _rentalRepo.Setup(r => r.UpdateStatusAsync(1, "Completed"))
                .Returns(Task.CompletedTask);

            await _service.CompleteRentalAsync(1);

            _rentalRepo.Verify(r => r.UpdateStatusAsync(1, "Completed"), Times.Once);
        }
    }

    public class UpdateOutForRentAsync : RentalServiceTests
    {
        public UpdateOutForRentAsync() : base() { }

        [Fact]
        public async Task ApprovedRentalPastStartDate_UpdatedToOutForRent()
        {
            // Arrange
            _rentalRepo.Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(new List<Rental>
                {
                    new Rental
                    {
                        Id = 1,
                        Status = "Approved",
                        StartDate = DateTime.UtcNow.AddDays(-1) // started yesterday
                    }
                });

            // Act
            await _service.UpdateOutForRentAsync();

            // Assert
            _rentalRepo.Verify(r => r.UpdateStatusAsync(1, "Out for Rent"), Times.Once);
        }

        [Fact]
        public async Task ApprovedRentalFutureStartDate_NotUpdated()
        {
            // Arrange
            _rentalRepo.Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(new List<Rental>
                {
                    new Rental
                    {
                        Id = 1,
                        Status = "Approved",
                        StartDate = DateTime.UtcNow.AddDays(2) // starts in future
                    }
                });

            // Act
            await _service.UpdateOutForRentAsync();

            // Assert
            _rentalRepo.Verify(r => r.UpdateStatusAsync(It.IsAny<int>(),
                It.IsAny<string>()), Times.Never);
        }
    }

    public class UpdateOverdueAsync : RentalServiceTests
    {
        public UpdateOverdueAsync() : base() { }

        [Fact]
        public async Task OutForRentPastEndDate_UpdatedToOverdue()
        {
            // Arrange
            _rentalRepo.Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(new List<Rental>
                {
                new Rental
                {
                    Id = 1,
                    Status = "Out for Rent",
                    EndDate = DateTime.UtcNow.AddDays(-1)
                }
                });

            // Act
            await _service.UpdateOverdueAsync();

            // Assert
            _rentalRepo.Verify(r => r.UpdateStatusAsync(1, "Overdue"), Times.Once);
        }

        [Fact]
        public async Task OutForRentFutureEndDate_NotUpdated()
        {
            // Arrange
            _rentalRepo.Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(new List<Rental>
                {
                new Rental
                {
                    Id = 1,
                    Status = "Out for Rent",
                    EndDate = DateTime.UtcNow.AddDays(2)
                }
                });

            // Act
            await _service.UpdateOverdueAsync();

            // Assert
            _rentalRepo.Verify(r => r.UpdateStatusAsync(
                It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task NonOutForRentStatus_NotUpdated()
        {
            // Arrange
            _rentalRepo.Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(new List<Rental>
                {
                new Rental
                {
                    Id = 1,
                    Status = "Approved",
                    EndDate = DateTime.UtcNow.AddDays(-1)
                }
                });

            // Act
            await _service.UpdateOverdueAsync();

            // Assert
            _rentalRepo.Verify(r => r.UpdateStatusAsync(
                It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }
    }
}