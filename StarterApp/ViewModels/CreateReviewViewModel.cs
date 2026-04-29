using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class CreateReviewViewModel : ObservableObject
{
    private readonly IReviewService _reviewService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private int rentalId;

    [ObservableProperty]
    private string comment;
    
    [ObservableProperty]
    private int rating;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;


    public CreateReviewViewModel(IReviewService reviewService, IAuthenticationService authService)
    {
        _reviewService = reviewService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task SubmitReviewAsync()
    {
        
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var reviewerId = _authService.CurrentUser?.Id ?? 0;
            await _reviewService.SubmitReviewAsync(rentalId, ItemId, reviewerId, comment, rating);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error submitting request: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

}
