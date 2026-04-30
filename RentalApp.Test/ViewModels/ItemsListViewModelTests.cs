using CommunityToolkit.Mvvm.Input;
// RentalApp.Test/ViewModels/ItemsListViewModelTests.cs
using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using RentalApp.ViewModels;
using Xunit;

namespace RentalApp.Test.ViewModels;

public class ItemsListViewModelTests
{
    private readonly Mock<IItemRepository> _itemRepo;
    private readonly Mock<INavigationService> _navService;
    private readonly ItemsListViewModel _viewModel;

    public ItemsListViewModelTests()
    {
        _itemRepo = new Mock<IItemRepository>();
        _navService = new Mock<INavigationService>();

        _itemRepo.Setup(r => r.GetAllAsync(
            It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Item>());

        _itemRepo.Setup(r => r.GetCategoriesAsync())
            .ReturnsAsync(new List<Category>());

        _viewModel = new ItemsListViewModel(_itemRepo.Object, _navService.Object);
    }

    public class LoadItemsCommand : ItemsListViewModelTests
    {
        public LoadItemsCommand() : base() { }

        [Fact]
        public async Task LoadItems_ReturnsAllItems()
        {
            // Arrange
            var items = new List<Item>
    {
        new Item { Id = 1, Title = "Power Drill" },
        new Item { Id = 2, Title = "Camping Tent" }
    };
            _itemRepo.Setup(r => r.GetAllAsync(
                It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(items);

            // Act
            await ((IAsyncRelayCommand)_viewModel.LoadItemsCommand).ExecuteAsync(null);

            // Assert
            Assert.Equal(2, _viewModel.Items.Count);
        }

        [Fact]
        public async Task LoadItems_WithSearch_CallsRepositoryWithSearchTerm()
        {
            // Arrange
            _viewModel.SearchText = "drill";

            // Act
            await ((IAsyncRelayCommand)_viewModel.LoadItemsCommand).ExecuteAsync(null);

            // Assert
            _itemRepo.Verify(r => r.GetAllAsync(null, "drill"), Times.AtLeastOnce);
        }

        [Fact]
        public async Task LoadItems_WithCategory_CallsRepositoryWithCategorySlug()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Tools", Slug = "tools" };
            _viewModel.SelectedCategory = category;

            // Act
            await ((IAsyncRelayCommand)_viewModel.LoadItemsCommand).ExecuteAsync(null);

            // Assert
            _itemRepo.Verify(r => r.GetAllAsync("tools", It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task LoadItems_EmptyResult_SetsEmptyCollection()
        {
            // Arrange
            _itemRepo.Setup(r => r.GetAllAsync(null, null))
                .ReturnsAsync(new List<Item>());

            // Act
            await ((IAsyncRelayCommand)_viewModel.LoadItemsCommand).ExecuteAsync(null);

            // Assert
            Assert.Empty(_viewModel.Items);
        }
    }

    public class CategoryFilter : ItemsListViewModelTests
    {
        public CategoryFilter() : base() { }

        [Fact]
        public void HasCategoryFilter_WhenCategorySelected_ReturnsTrue()
        {
            // Arrange
            _viewModel.SelectedCategory = new Category { Id = 1, Name = "Tools", Slug = "tools" };

            // Act & Assert
            Assert.True(_viewModel.HasCategoryFilter);
        }

        [Fact]
        public void HasCategoryFilter_WhenNoCategorySelected_ReturnsFalse()
        {
            // Arrange
            _viewModel.SelectedCategory = null;

            // Act & Assert
            Assert.False(_viewModel.HasCategoryFilter);
        }

        [Fact]
        public void ClearCategory_SetsSelectedCategoryToNull()
        {
            // Arrange
            _viewModel.SelectedCategory = new Category { Id = 1, Name = "Tools", Slug = "tools" };

            // Act
            _viewModel.ClearCategoryCommand.Execute(null);

            // Assert
            Assert.Null(_viewModel.SelectedCategory);
        }
    }
}