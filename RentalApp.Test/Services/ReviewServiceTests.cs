// RentalApp.Test/Services/ReviewServiceTests.cs
using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using Xunit;

namespace RentalApp.Test.Services;

public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _reviewRepo;
    private readonly Mock<IItemRepository> _itemRepo;
    private readonly ReviewService _service;

    public ReviewServiceTests()
    {
        _reviewRepo = new Mock<IReviewRepository>();
        _itemRepo = new Mock<IItemRepository>();
        _service = new ReviewService(_reviewRepo.Object, _itemRepo.Object);
    }

    public class SubmitReviewAsync : ReviewServiceTests
    {
        public SubmitReviewAsync() : base() { }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async Task ValidRating_CreatesReview(int rating)
        {
            // Arrange
            var expected = new Review { Id = 1, Rating = rating };
            _reviewRepo.Setup(r => r.CreateAsync(1, 1, 2, "Great!", rating))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.SubmitReviewAsync(1, 1, 2, "Great!", rating);

            // Assert
            Assert.Equal(expected.Id, result.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-1)]
        public async Task InvalidRating_ThrowsException(int rating)
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.SubmitReviewAsync(1, 1, 2, "Bad rating", rating));

            _reviewRepo.Verify(r => r.CreateAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ValidRating_CallsRepositoryOnce()
        {
            // Arrange
            _reviewRepo.Setup(r => r.CreateAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new Review());

            // Act
            await _service.SubmitReviewAsync(1, 1, 2, "Good stuff", 4);

            // Assert
            _reviewRepo.Verify(r => r.CreateAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }
    }

    public class GetItemReviewsAsync : ReviewServiceTests
    {
        public GetItemReviewsAsync() : base() { }

        [Fact]
        public async Task ValidItemId_ReturnsReviews()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { Id = 1, ItemId = 1, Rating = 5 },
                new Review { Id = 2, ItemId = 1, Rating = 3 }
            };
            _reviewRepo.Setup(r => r.GetByItemIdAsync(1)).ReturnsAsync(reviews);

            // Act
            var result = await _service.GetItemReviewsAsync(1);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task NoReviews_ReturnsEmpty()
        {
            // Arrange
            _reviewRepo.Setup(r => r.GetByItemIdAsync(99))
                .ReturnsAsync(new List<Review>());

            // Act
            var result = await _service.GetItemReviewsAsync(99);

            // Assert
            Assert.Empty(result);
        }
    }

    public class GetUserReviewsAsync : ReviewServiceTests
    {
        public GetUserReviewsAsync() : base() { }

        [Fact]
        public async Task ValidUserId_ReturnsReviews()
        {
            // Arrange
            var reviews = new List<Review> { new Review { Id = 1, ReviewerId = 2 } };
            _reviewRepo.Setup(r => r.GetByUserIdAsync(2)).ReturnsAsync(reviews);

            // Act
            var result = await _service.GetUserReviewsAsync(2);

            // Assert
            Assert.Single(result);
        }
    }
}