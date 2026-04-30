using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class ReviewsViewModel : ObservableObject
{
    private readonly IReviewService _reviewService;
    private readonly IReviewRepository _reviewRepository;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Review> _reviews = new();

    public ReviewsViewModel(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    partial void OnItemIdChanged(int value) => _ = Task.Run(LoadReviewsAsync);

    [RelayCommand]
    private async Task LoadReviewsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var reviews = await _reviewService.GetItemReviewsAsync(ItemId);
            Reviews = new ObservableCollection<Review>(reviews);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading reviews: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

}
