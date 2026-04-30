// RentalApp.Test/ViewModels/NearbyItemsViewModelTests.cs
using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using RentalApp.Test.Fixtures;
using RentalApp.ViewModels;
using Xunit;

namespace RentalApp.Test.ViewModels;

public class NearbyItemsViewModelTests
{
    private readonly Mock<IItemRepository> _itemRepo;
    private readonly Mock<INavigationService> _navService;
    private readonly MockLocationService _locationService;
    private readonly NearbyItemsViewModel _viewModel;

    public NearbyItemsViewModelTests()
    {
        _itemRepo = new Mock<IItemRepository>();
        _navService = new Mock<INavigationService>();
        _locationService = new MockLocationService(55.9533, -3.1883);

        _itemRepo.Setup(r => r.GetNearbyAsync(
            It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(new List<Item>());

        _viewModel = new NearbyItemsViewModel(
            _itemRepo.Object, _locationService, _navService.Object);
    }

    public class FindNearbyCommand : NearbyItemsViewModelTests
    {
        public FindNearbyCommand() : base() { }

        [Fact]
        public async Task WithValidLocation_CallsRepositoryWithCoordinates()
        {
            // Arrange
            _itemRepo.Setup(r => r.GetNearbyAsync(55.9533, -3.1883, 5.0))
                .ReturnsAsync(new List<Item>());

            // Act
            await _viewModel.FindNearbyCommand.ExecuteAsync(null);

            // Assert
            _itemRepo.Verify(r => r.GetNearbyAsync(55.9533, -3.1883, 5.0), Times.Once);
        }

        [Fact]
        public async Task WithValidLocation_PopulatesItems()
        {
            // Arrange
            var items = new List<Item>
            {
                new Item { Id = 1, Title = "Power Drill" },
                new Item { Id = 2, Title = "Camping Tent" }
            };
            _itemRepo.Setup(r => r.GetNearbyAsync(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(items);

            // Act
            await _viewModel.FindNearbyCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(2, _viewModel.Items.Count);
        }

        [Fact]
        public async Task WithValidLocation_SetsLocationStatus()
        {
            // Arrange
            var items = new List<Item> { new Item { Id = 1 } };
            _itemRepo.Setup(r => r.GetNearbyAsync(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(items);

            // Act
            await _viewModel.FindNearbyCommand.ExecuteAsync(null);

            // Assert
            Assert.Contains("1", _viewModel.LocationStatus);
        }

        [Fact]
        public async Task WithNullLocation_SetsErrorMessage()
        {
            // Arrange
            var nullLocationService = new MockLocationService(null);
            var viewModel = new NearbyItemsViewModel(
                _itemRepo.Object, nullLocationService, _navService.Object);

            // Act
            await viewModel.FindNearbyCommand.ExecuteAsync(null);

            // Assert
            Assert.NotEmpty(viewModel.ErrorMessage);
            Assert.Contains("location", viewModel.ErrorMessage.ToLower());
        }

        [Fact]
        public async Task AfterLoad_IsLoadingIsFalse()
        {
            // Act
            await _viewModel.FindNearbyCommand.ExecuteAsync(null);

            // Assert
            Assert.False(_viewModel.IsLoading);
        }

        [Fact]
        public async Task WithCustomRadius_PassesRadiusToRepository()
        {
            // Arrange
            _viewModel.RadiusKm = 10.0;

            // Act
            await _viewModel.FindNearbyCommand.ExecuteAsync(null);

            // Assert
            _itemRepo.Verify(r => r.GetNearbyAsync(
                It.IsAny<double>(), It.IsAny<double>(), 10.0), Times.Once);
        }
    }
}