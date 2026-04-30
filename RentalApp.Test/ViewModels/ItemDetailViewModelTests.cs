// RentalApp.Test/ViewModels/ItemDetailViewModelTests.cs
using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using RentalApp.ViewModels;
using Xunit;

namespace RentalApp.Test.ViewModels;

public class ItemDetailViewModelTests
{
    private readonly Mock<IItemRepository> _itemRepo;
    private readonly Mock<IAuthenticationService> _authService;
    private readonly Mock<INavigationService> _navService;
    private readonly ItemDetailViewModel _viewModel;

    public ItemDetailViewModelTests()
    {
        _itemRepo = new Mock<IItemRepository>();
        _authService = new Mock<IAuthenticationService>();
        _navService = new Mock<INavigationService>();

        _authService.Setup(a => a.CurrentUser)
            .Returns(new User
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                PasswordHash = "hash",
                PasswordSalt = "salt"
            });

        _itemRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Item?)null);

        _viewModel = new ItemDetailViewModel(
            _itemRepo.Object, _authService.Object, _navService.Object);
    }

    public class LoadItem : ItemDetailViewModelTests
    {
        public LoadItem() : base() { }

        [Fact]
        public async Task ValidItemId_LoadsItem()
        {
            // Arrange
            var item = new Item { Id = 1, Title = "Power Drill", OwnerId = 2 };
            _itemRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

            // Act
            _viewModel.ItemId = 1;
            await Task.Delay(100); // allow Task.Run to complete

            // Assert
            Assert.NotNull(_viewModel.Item);
            Assert.Equal("Power Drill", _viewModel.Item.Title);
        }

        [Fact]
        public async Task InvalidItemId_ItemIsNull()
        {
            // Arrange
            _itemRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Item?)null);

            // Act
            _viewModel.ItemId = 999;
            await Task.Delay(100);

            // Assert
            Assert.Null(_viewModel.Item);
        }
    }

    public class CanEditCanRent : ItemDetailViewModelTests
    {
        public CanEditCanRent() : base() { }

        [Fact]
        public async Task WhenUserIsOwner_CanEditIsTrue()
        {
            // Arrange
            var item = new Item { Id = 1, Title = "Power Drill", OwnerId = 1 };
            _itemRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

            // Act
            _viewModel.ItemId = 1;
            await Task.Delay(100);

            // Assert
            Assert.True(_viewModel.CanEdit);
            Assert.False(_viewModel.CanRent);
        }

        [Fact]
        public async Task WhenUserIsNotOwner_CanRentIsTrue()
        {
            // Arrange
            var item = new Item { Id = 1, Title = "Power Drill", OwnerId = 2 };
            _itemRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

            // Act
            _viewModel.ItemId = 1;
            await Task.Delay(100);

            // Assert
            Assert.False(_viewModel.CanEdit);
            Assert.True(_viewModel.CanRent);
        }
    }
}