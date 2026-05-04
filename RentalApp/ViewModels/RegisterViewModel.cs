/// @file RegisterViewModel.cs
/// @brief User registration view model
/// @author RentalApp Development Team
/// @date 2025
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Services;

namespace RentalApp.ViewModels;

// View model for the user registration page.
public partial class RegisterViewModel : BaseViewModel
{
    /// @brief Authentication service for managing user registration
    private readonly IAuthenticationService _authService;

    /// @brief Navigation service for managing page navigation
    private readonly INavigationService _navigationService;

    /// @brief The user's first name
    /// @details Observable property bound to the first name input field
    [ObservableProperty]
    private string firstName = string.Empty;

    /// @brief The user's last name
    /// @details Observable property bound to the last name input field
    [ObservableProperty]
    private string lastName = string.Empty;

    /// @brief The user's email address
    /// @details Observable property bound to the email input field
    [ObservableProperty]
    private string email = string.Empty;

    /// @brief The user's password
    /// @details Observable property bound to the password input field
    [ObservableProperty]
    private string password = string.Empty;

    /// @brief Confirmation of the user's password
    /// @details Observable property bound to the confirm password input field
    [ObservableProperty]
    private string confirmPassword = string.Empty;

    /// @brief Whether the user accepts the terms and conditions
    /// @details Observable property bound to the terms acceptance checkbox
    [ObservableProperty]
    private bool acceptTerms;

    // Default constructor for design-time support.
    public RegisterViewModel()
    {
        // Default constructor for design time support
        Title = "Register";
    }

    /// <summary>Initializes a new instance of the RegisterViewModel class.</summary>
    public RegisterViewModel(
        IAuthenticationService authService,
        INavigationService navigationService
    )
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Register";
    }

    // Registers a new user account.
    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy)
            return;

        if (!ValidateForm())
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var result = await _authService.RegisterAsync(FirstName, LastName, Email, Password);

            if (result.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Registration successful! Please login.",
                    "OK"
                );
                await _navigationService.NavigateBackAsync();
            }
            else
            {
                SetError(result.Message);
            }
        }
        catch (Exception ex)
        {
            SetError($"Registration failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Navigates back to the login page.
    [RelayCommand]
    private async Task NavigateBackToLoginAsync()
    {
        await _navigationService.NavigateBackAsync();
    }

    // Validates the registration form data.
    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            SetError("First name is required");
            return false;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            SetError("Last name is required");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            SetError("Email is required");
            return false;
        }

        if (!IsValidEmail(Email))
        {
            SetError("Please enter a valid email address");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            SetError("Password is required");
            return false;
        }

        if (Password.Length < 6)
        {
            SetError("Password must be at least 6 characters long");
            return false;
        }

        if (Password != ConfirmPassword)
        {
            SetError("Passwords do not match");
            return false;
        }

        if (!AcceptTerms)
        {
            SetError("Please accept the terms and conditions");
            return false;
        }

        return true;
    }

    // Validates an email address format.
    private static bool IsValidEmail(string email)
    {
        const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
    }
}
