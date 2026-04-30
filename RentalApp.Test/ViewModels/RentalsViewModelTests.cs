// RentalApp.Test/ViewModels/RentalsViewModelTests.cs
using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using RentalApp.ViewModels;
using Xunit;

namespace RentalApp.Test.ViewModels;

public class RentalsViewModelTests
{
    private readonly Mock<IRentalService> _rentalService;
    private readonly Mock<IRentalRepository> _rentalRepo;
    private readonly Mock<IAuthenticationService> _authService;
    private readonly Mock<INavigationService> _navService;
    private readonly RentalsViewModel _viewModel;

    public RentalsViewModelTests()
    {
        _rentalService = new Mock<IRentalService>();
        _rentalRepo = new Mock<IRentalRepository>();
        _authService = new Mock<IAuthenticationService>();
        _navService = new Mock<INavigationService>();

        _authService.Setup(a => a.CurrentUser)
            .Returns(new User { Id = 1, FirstName = "Test", LastName = "User", Email = "test@test.com", PasswordHash = "hash", PasswordSalt = "salt" });

        _rentalService.Setup(s => s.UpdateOutForRentAsync())
            .Returns(Task.CompletedTask);

        _rentalRepo.Setup(r => r.GetIncomingAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Rental>());

        _rentalRepo.Setup(r => r.GetOutgoingAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Rental>());

        _viewModel = new RentalsViewModel(
            _rentalService.Object,
            _rentalRepo.Object,
            _authService.Object,
            _navService.Object);
    }

    public class TabSwitching : RentalsViewModelTests
    {
        public TabSwitching() : base() { }

        [Fact]
        public void ShowIncomingTab_SetsShowIncomingTrue()
        {
            // Arrange
            _viewModel.ShowIncoming = false;

            // Act
            _viewModel.ShowIncomingTabCommand.Execute(null);

            // Assert
            Assert.True(_viewModel.ShowIncoming);
        }

        [Fact]
        public void ShowOutgoingTab_SetsShowIncomingFalse()
        {
            // Act
            _viewModel.ShowOutgoingTabCommand.Execute(null);

            // Assert
            Assert.False(_viewModel.ShowIncoming);
        }

        [Fact]
        public void ShowOutgoing_IsOppositeOfShowIncoming()
        {
            // Arrange & Act
            _viewModel.ShowIncoming = true;
            Assert.False(_viewModel.ShowOutgoing);

            _viewModel.ShowIncoming = false;
            Assert.True(_viewModel.ShowOutgoing);
        }
    }

    public class LoadRentalsCommand : RentalsViewModelTests
    {
        public LoadRentalsCommand() : base() { }

        [Fact]
        public async Task LoadRentals_PopulatesIncomingRentals()
        {
            // Arrange
            var rentals = new List<Rental>
            {
                new Rental { Id = 1, Status = "Requested", OwnerId = 1 }
            };
            _rentalRepo.Setup(r => r.GetIncomingAsync(1)).ReturnsAsync(rentals);

            // Act
            await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

            // Assert
            Assert.Single(_viewModel.IncomingRentals);
        }

        [Fact]
        public async Task LoadRentals_PopulatesOutgoingRentals()
        {
            // Arrange
            var rentals = new List<Rental>
            {
                new Rental { Id = 2, Status = "Approved", BorrowerId = 1 }
            };
            _rentalRepo.Setup(r => r.GetOutgoingAsync(1)).ReturnsAsync(rentals);

            // Act
            await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

            // Assert
            Assert.Single(_viewModel.OutgoingRentals);
        }

        [Fact]
        public async Task LoadRentals_FilterOutRejectedRentals()
        {
            // Arrange
            var rentals = new List<Rental>
            {
                new Rental { Id = 1, Status = "Rejected", OwnerId = 1 }
            };
            _rentalRepo.Setup(r => r.GetIncomingAsync(1)).ReturnsAsync(rentals);

            // Act
            await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

            // Assert
            Assert.Empty(_viewModel.IncomingRentals);
        }
    }

    public class RentalActions : RentalsViewModelTests
    {
        public RentalActions() : base() { }

        [Fact]
        public async Task ApproveRental_CallsApproveOnService()
        {
            // Arrange
            var rental = new Rental { Id = 1, Status = "Requested" };
            _rentalService.Setup(s => s.ApproveRentalAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _viewModel.ApproveRentalCommand.ExecuteAsync(rental);

            // Assert
            _rentalService.Verify(s => s.ApproveRentalAsync(1), Times.Once);
        }

        [Fact]
        public async Task RejectRental_CallsRejectOnService()
        {
            // Arrange
            var rental = new Rental { Id = 1, Status = "Requested" };
            _rentalService.Setup(s => s.RejectRentalAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _viewModel.RejectRentalCommand.ExecuteAsync(rental);

            // Assert
            _rentalService.Verify(s => s.RejectRentalAsync(1), Times.Once);
        }

        [Fact]
        public async Task ReturnRental_CallsReturnOnService()
        {
            // Arrange
            var rental = new Rental { Id = 1, Status = "Out for Rent" };
            _rentalService.Setup(s => s.ReturnRentalAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _viewModel.ReturnRentalCommand.ExecuteAsync(rental);

            // Assert
            _rentalService.Verify(s => s.ReturnRentalAsync(1), Times.Once);
        }

        [Fact]
        public async Task CompleteRental_CallsCompleteOnService()
        {
            // Arrange
            var rental = new Rental { Id = 1, Status = "Returned" };
            _rentalService.Setup(s => s.CompleteRentalAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _viewModel.CompleteRentalCommand.ExecuteAsync(rental);

            // Assert
            _rentalService.Verify(s => s.CompleteRentalAsync(1), Times.Once);
        }
    }
}