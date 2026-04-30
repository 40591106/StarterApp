using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Data.Repositories;
using RentalApp.Test.Fixtures;
using Xunit;

namespace RentalApp.Test.Repositories;

public class ItemRepositoryTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly ItemRepository _repository;

    public ItemRepositoryTests()
    {
        _fixture = new DatabaseFixture();
        _repository = new ItemRepository(
            new MockDbContextFactory(_fixture.Context));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        // Arrange — done in fixture

        // Act
        var items = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public async Task GetAllAsync_WithSearch_ReturnsFilteredItems()
    {
        // Arrange — done in fixture

        // Act
        var items = await _repository.GetAllAsync(search: "Drill");

        // Assert
        Assert.Single(items);
        Assert.Equal("Power Drill", items[0].Title);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsItem()
    {
        // Act
        var item = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(item);
        Assert.Equal("Power Drill", item.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var item = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(item);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}