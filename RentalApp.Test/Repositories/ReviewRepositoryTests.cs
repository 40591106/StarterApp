using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Test.Fixtures;
using Xunit;

namespace RentalApp.Test.Repositories;

public class ReviewRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly ReviewRepository _repository;

    public ReviewRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new ReviewRepository(new MockDbContextFactory(_fixture.Options));
    }
    public class CreateAsync : ReviewRepositoryTests
    {
        public CreateAsync(DatabaseFixture fixture) : base(fixture) { }
        [Fact]
        public async Task ValidReview_CreatesAndReturnsReview()
        {
            // Arrange
            _fixture.Context.Rentals.Add(new Rental
            {
                Id = 1,
                ItemId = 1,
                BorrowerId = 2,
                OwnerId = 1,
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(-3),
                Status = "Completed",
                TotalPrice = 30.00m
            });
            await _fixture.Context.SaveChangesAsync();

            // Act
            var review = await _repository.CreateAsync(1, 1, 2, "Great item!", 5);

            // Assert
            Assert.NotNull(review);
            Assert.Equal("Great item!", review.Comment);
            Assert.Equal(5, review.Rating);
        }

        [Fact]
        public async Task DuplicateReview_ThrowsException()
        {
            // Arrange
            _fixture.Context.Rentals.Add(new Rental
            {
                Id = 2,
                ItemId = 1,
                BorrowerId = 2,
                OwnerId = 1,
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(-3),
                Status = "Completed",
                TotalPrice = 30.00m
            });
            _fixture.Context.Reviews.Add(new Review
            {
                RentalId = 2,
                ItemId = 1,
                ReviewerId = 2,
                Comment = "Already reviewed",
                Rating = 4
            });
            await _fixture.Context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _repository.CreateAsync(2, 1, 2, "Trying again", 5));
        }
    }

    public class GetByItemIdAsync : ReviewRepositoryTests
    {
        public GetByItemIdAsync(DatabaseFixture fixture) : base(fixture) { }
        [Fact]
        public async Task ValidItemId_ReturnsReviews()
        {
            // Arrange
            _fixture.Context.Reviews.AddRange(
                new Review { ItemId = 1, ReviewerId = 2, RentalId = 1, Comment = "Good", Rating = 4 },
                new Review { ItemId = 1, ReviewerId = 2, RentalId = 2, Comment = "Great", Rating = 5 }
            );
            await _fixture.Context.SaveChangesAsync();

            // Act
            var reviews = await _repository.GetByItemIdAsync(1);

            // Assert
            Assert.Equal(2, reviews.Count());
        }

        [Fact]
        public async Task InvalidItemId_ReturnsEmpty()
        {
            // Act
            var reviews = await _repository.GetByItemIdAsync(999);

            // Assert
            Assert.Empty(reviews);
        }
    }

    public class GetByUserIdAsync : ReviewRepositoryTests
    {
        public GetByUserIdAsync(DatabaseFixture fixture) : base(fixture) { }
        [Fact]
        public async Task ValidUserId_ReturnsReviews()
        {
            // Arrange
            _fixture.Context.Reviews.Add(
                new Review { ItemId = 1, ReviewerId = 2, RentalId = 1, Comment = "Good", Rating = 4 }
            );
            await _fixture.Context.SaveChangesAsync();

            // Act
            var reviews = await _repository.GetByUserIdAsync(1);

            // Assert
            Assert.NotEmpty(reviews);
        }

        [Fact]
        public async Task InvalidUserId_ReturnsEmpty()
        {
            // Act
            var reviews = await _repository.GetByUserIdAsync(999);

            // Assert
            Assert.Empty(reviews);
        }
    }

}