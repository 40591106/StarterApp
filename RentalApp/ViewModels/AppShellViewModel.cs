/// @file AppShellViewModel.cs
/// @brief Application shell view model for managing navigation and authentication state
/// @author RentalApp Development Team
/// @date 2025
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Services;

namespace RentalApp.ViewModels
{
    /// <summary>View model for the application shell that manages navigation and authentication.
    public partial class AppShellViewModel : BaseViewModel
    {
        /// @brief Authentication service for managing user authentication
        private readonly IAuthenticationService _authService;

        /// @brief Navigation service for managing page navigation
        private readonly INavigationService _navigationService;

        /// @brief Collection of dynamic menu bar items
        /// @details Observable collection that can be modified at runtime based on user permissions
        public ObservableCollection<MenuBarItem> DynamicMenuBarItems { get; } = new();

        // Default constructor for design-time support.
        public AppShellViewModel()
        {
            Title = "RentalApp";
        }

        // Initializes a new instance of the AppShellViewModel class.
        public AppShellViewModel(
            IAuthenticationService authService,
            INavigationService navigationService
        )
        {
            _authService = authService;
            _navigationService = navigationService;
            _authService.AuthenticationStateChanged += OnAuthenticationStateChanged;
            Title = "RentalApp";
        }

        // Determines if guest actions can be executed.
        private bool CanExecuteGuestAction() => _authService.HasRole("Guest");

        // Determines if user actions can be executed.
        private bool CanExecuteUserAction() => _authService.HasRole("OrdinaryUser");
        /// Determines if admin actions can be executed.
        private bool CanExecuteAdminAction()
        {
            return _authService.HasRole("Admin");
        }

        // Determines if authenticated actions can be executed.
        private bool CanExecuteAuthenticatedAction()
        {
            return _authService.IsAuthenticated;
        }

        // Handles authentication state changes.
        private void OnAuthenticationStateChanged(object? sender, bool isAuthenticated)
        {
            LogoutCommand.NotifyCanExecuteChanged();
            NavigateToProfileCommand.NotifyCanExecuteChanged();
            NavigateToSettingsCommand.NotifyCanExecuteChanged();
        }

        // Navigates to the current user's profile page.
        [RelayCommand]
        private async Task NavigateToProfileAsync()
        {
            await _navigationService.NavigateToAsync("TempPage");
        }

        // Navigates to the current user's settings page.
        [RelayCommand]
        private async Task NavigateToSettingsAsync()
        {
            await _navigationService.NavigateToAsync("TempPage");
        }

        // Logs out the current user and navigates to login page.
        [RelayCommand(CanExecute = nameof(CanExecuteAuthenticatedAction))]
        private async Task LogoutAsync()
        {
            await _authService.LogoutAsync();
            await _navigationService.NavigateToAsync("LoginPage");

            LogoutCommand.NotifyCanExecuteChanged();
            NavigateToProfileCommand.NotifyCanExecuteChanged();
            NavigateToSettingsCommand.NotifyCanExecuteChanged();
        }
    }
}
