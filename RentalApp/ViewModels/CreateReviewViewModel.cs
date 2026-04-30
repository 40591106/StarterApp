using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
[QueryProperty(nameof(RentalId), "rentalId")]
public partial class CreateReviewViewModel : ObservableObject
{
    private readonly IReviewService _reviewService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private int rentalId;

    [ObservableProperty]
    private string comment = string.Empty;

    [ObservableProperty]
    private int rating = 5;

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
            await _reviewService.SubmitReviewAsync(RentalId, ItemId, reviewerId, Comment, Rating);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                ex.Message,
                "OK");
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
