using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class ReviewsViewModel : ObservableObject
{
    private readonly IReviewService _reviewService;

    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Review> _reviews = new();

    /// <summary>Initializes a new instance of the ReviewsViewModel class.</summary>
    public ReviewsViewModel(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }


    // Handles the change of ItemId property by loading reviews asynchronously.

    partial void OnItemIdChanged(int value) => _ = Task.Run(LoadReviewsAsync);


    // Loads reviews for the current item asynchronously.
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