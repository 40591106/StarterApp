using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

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

    partial void OnUserIdChanged(int value) => _ = Task.Run(LoadProfileAsync);

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

    private async Task LoadProfileAsync()
    {
        IsBusy = true;
        try
        {
            var reviews = await _reviewService.GetUserReviewsAsync(UserId);
            Reviews = new ObservableCollection<Review>(reviews);
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

    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }
}