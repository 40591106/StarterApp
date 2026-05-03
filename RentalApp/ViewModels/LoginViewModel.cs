/// @file LoginViewModel.cs
/// @brief Login page view model for user authentication
/// @author RentalApp Development Team
/// @date 2025
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Services;

namespace RentalApp.ViewModels;

// View model for the login page that handles user authentication.
public partial class LoginViewModel : BaseViewModel
{
    /// @brief Authentication service for managing user login
    private readonly IAuthenticationService _authService;

    /// @brief Navigation service for managing page navigation
    private readonly INavigationService _navigationService;

    /// @brief The user's email address
    /// @details Observable property bound to the email input field
    [ObservableProperty]
    private string email = string.Empty;

    /// @brief The user's password
    /// @details Observable property bound to the password input field
    [ObservableProperty]
    private string password = string.Empty;

    /// @brief Whether to remember the user's login credentials
    /// @details Observable property bound to the remember me checkbox
    [ObservableProperty]
    private bool rememberMe;

    /// @brief Indicates whether a login operation is in progress
    /// @details Observable property that notifies the LoginCommand when changed
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool _isBusy;

    // Default constructor for design-time support.
    public LoginViewModel()
    {
        // Default constructor for design time support
        Title = "Login";
    }

    // Initializes a new instance of the LoginViewModel class.
    public LoginViewModel(IAuthenticationService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Login";
    }

    // Performs user login authentication.
    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            SetError("Please enter both email and password");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var result = await _authService.LoginAsync(Email, Password);

            if (result.IsSuccess)
            {
                await _navigationService.NavigateToAsync("MainPage");
            }
            else
            {
                SetError(result.Message);
            }
        }
        catch (Exception ex)
        {
            SetError($"Login failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Navigates to the registration page.
    [RelayCommand]
    private async Task NavigateToRegisterAsync()
    {
        await _navigationService.NavigateToAsync("RegisterPage");
    }

    // Handles forgot password functionality.
    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        // TODO: Implement forgot password functionality
        await Application.Current.MainPage.DisplayAlert(
            "Info",
            "Forgot password functionality not implemented yet",
            "OK"
        );
    }
}
