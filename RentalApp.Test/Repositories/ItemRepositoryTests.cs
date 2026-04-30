using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Test.Fixtures;
using Xunit;

namespace RentalApp.Test.Repositories;

public class ItemRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly ItemRepository _repository;

    public ItemRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new ItemRepository(new MockDbContextFactory(_fixture.Options));
    }

    public class GetAllAsync : ItemRepositoryTests
    {
        public GetAllAsync(DatabaseFixture fixture) : base(fixture) { }

        [Fact]
        public async Task NoFilter_ReturnsAllItems()
        {
            var items = await _repository.GetAllAsync();
            Assert.Equal(2, items.Count);
        }

        [Theory]
        [InlineData("Drill", 1)]
        [InlineData("Tent", 1)]
        [InlineData("xyz", 0)]
        public async Task WithSearch_ReturnsFilteredItems(string search, int expectedCount)
        {
            var items = await _repository.GetAllAsync(search: search);
            Assert.Equal(expectedCount, items.Count);
        }

        [Theory]
        [InlineData("tools", 1)]
        [InlineData("camping", 1)]
        public async Task WithCategory_ReturnsFilteredItems(string category, int expectedCount)
        {
            var items = await _repository.GetAllAsync(category: category);
            Assert.Equal(expectedCount, items.Count);
        }
    }

    public class GetByIdAsync : ItemRepositoryTests
    {
        public GetByIdAsync(DatabaseFixture fixture) : base(fixture) { }

        [Fact]
        public async Task ValidId_ReturnsItem()
        {
            var item = await _repository.GetByIdAsync(1);
            Assert.NotNull(item);
            Assert.Equal("Power Drill", item.Title);
        }

        [Fact]
        public async Task InvalidId_ReturnsNull()
        {
            var item = await _repository.GetByIdAsync(999);
            Assert.Null(item);
        }
    }

    public class CreateAsync : ItemRepositoryTests
    {
        public CreateAsync(DatabaseFixture fixture) : base(fixture) { }

        [Fact]
        public async Task ValidItem_CreatesAndReturnsItem()
        {
            // Arrange
            var item = new Item
            {
                Title = "Lawn Mower",
                Description = "A lawn mower",
                DailyRate = 25.00m,
                CategoryId = 1,
                OwnerId = 1
            };

            // Act
            var created = await _repository.CreateAsync(item);

            // Assert
            Assert.NotEqual(0, created.Id);
            using var freshContext = _fixture.CreateFreshContext();
            var found = await freshContext.Items.FindAsync(created.Id);
            Assert.NotNull(found);
            Assert.Equal("Lawn Mower", found.Title);
        }
    }

    public class UpdateAsync : ItemRepositoryTests
    {
        public UpdateAsync(DatabaseFixture fixture) : base(fixture) { }

        [Fact]
        public async Task ValidItem_UpdatesItem()
        {
            // Arrange
            var item = await _repository.GetByIdAsync(1);
            item!.Title = "Updated Drill";

            // Act
            await _repository.UpdateAsync(item);

            // Assert
            using var freshContext = _fixture.CreateFreshContext();
            var updated = await freshContext.Items.FindAsync(1);
            Assert.Equal("Updated Drill", updated!.Title);
        }
    }

    public class GetCategoriesAsync : ItemRepositoryTests
    {
        public GetCategoriesAsync(DatabaseFixture fixture) : base(fixture) { }

        [Fact]
        public async Task ReturnsAllCategories()
        {
            // Act
            var categories = await _repository.GetCategoriesAsync();

            // Assert
            Assert.Equal(2, categories.Count);
        }

        [Fact]
        public async Task ReturnsCorrectCategoryNames()
        {
            // Act
            var categories = await _repository.GetCategoriesAsync();

            // Assert
            Assert.Contains(categories, c => c.Slug == "tools");
            Assert.Contains(categories, c => c.Slug == "camping");
        }
    }
}