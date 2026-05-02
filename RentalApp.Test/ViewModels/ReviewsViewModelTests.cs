// RentalApp.Test/ViewModels/ReviewsViewModelTests.cs
using Moq;
using RentalApp.Database.Models;
using RentalApp.Services;
using RentalApp.ViewModels;
using Xunit;

namespace RentalApp.Test.ViewModels;

public class ReviewsViewModelTests
{
    private readonly Mock<IReviewService> _reviewService;
    private readonly ReviewsViewModel _viewModel;

    public ReviewsViewModelTests()
    {
        _reviewService = new Mock<IReviewService>();
        _reviewService.Setup(s => s.GetItemReviewsAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Review>());
        _viewModel = new ReviewsViewModel(_reviewService.Object);
    }

    public class LoadReviewsCommand : ReviewsViewModelTests
    {
        public LoadReviewsCommand() : base() { }

        [Fact]
        public async Task ValidItemId_LoadsReviews()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { Id = 1, Rating = 5, Comment = "Great!" },
                new Review { Id = 2, Rating = 3, Comment = "OK" }
            };
            _reviewService.Setup(s => s.GetItemReviewsAsync(1)).ReturnsAsync(reviews);
            _viewModel.ItemId = 1;

            // Act
            await _viewModel.LoadReviewsCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(2, _viewModel.Reviews.Count);
        }

        [Fact]
        public async Task NoReviews_SetsEmptyCollection()
        {
            // Arrange
            _reviewService.Setup(s => s.GetItemReviewsAsync(1))
                .ReturnsAsync(new List<Review>());
            _viewModel.ItemId = 1;

            // Act
            await _viewModel.LoadReviewsCommand.ExecuteAsync(null);

            // Assert
            Assert.Empty(_viewModel.Reviews);
        }

        [Fact]
        public async Task AfterLoad_IsLoadingIsFalse()
        {
            // Arrange
            _viewModel.ItemId = 1;

            // Act
            await _viewModel.LoadReviewsCommand.ExecuteAsync(null);
            await Task.Delay(100); // wait for async operations to complete

            // Assert
            Assert.False(_viewModel.IsLoading);
        }

        [Fact]
        public async Task ServiceThrows_SetsErrorMessage()
        {
            // Arrange
            _reviewService.Setup(s => s.GetItemReviewsAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Service error"));
            _viewModel.ItemId = 1;

            // Act
            await _viewModel.LoadReviewsCommand.ExecuteAsync(null);

            // Assert
            Assert.NotEmpty(_viewModel.ErrorMessage);
            Assert.Contains("Service error", _viewModel.ErrorMessage);
        }
    }
}