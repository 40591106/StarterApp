using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(UserName), "userName")]
[QueryProperty(nameof(UserId), "userId")]
public partial class ProfileViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;
    private readonly IReviewService _reviewService;

    [ObservableProperty]
    private User? currentUser;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private int _userId;

    [ObservableProperty]
    private ObservableCollection<Review> _reviews = new();

    public string AverageRatingText => Reviews.Count > 0
    ? $"{Reviews.Average(r => r.Rating):F1} ★"
    : "No ratings yet";

    partial void OnUserIdChanged(int value) => _ = Task.Run(LoadProfileAsync);

    // Initializes a new instance of the ProfileViewModel class.
    public ProfileViewModel(
        IAuthenticationService authService,
        INavigationService navigationService,
        IReviewService reviewService
    )
    {
        _authService = authService;
        _navigationService = navigationService;
        _reviewService = reviewService;
        Title = "Profile";
        CurrentUser = _authService.CurrentUser;
    }

    // Loads the user's profile data asynchronously.
    private async Task LoadProfileAsync()
    {
        IsBusy = true;
        try
        {
            var reviews = await _reviewService.GetUserReviewsAsync(UserId);
            Reviews = new ObservableCollection<Review>(reviews);
            OnPropertyChanged(nameof(AverageRatingText));
        }
        catch (Exception ex)
        {
            SetError($"Error loading profile: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Navigates back to the previous page.
    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }
}