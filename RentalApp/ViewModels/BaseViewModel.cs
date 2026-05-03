/// @file BaseViewModel.cs
/// @brief Base view model class providing common functionality for all view models
/// @author RentalApp Development Team
/// @date 2025
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RentalApp.ViewModels;


// Base view model class that provides common properties and functionality.

public partial class BaseViewModel : ObservableObject
{
    /// @brief Indicates whether the view model is currently performing a busy operation
    /// @details Observable property that can be bound to UI elements to show loading states
    [ObservableProperty]
    private bool isBusy;

    /// @brief The title of the current page or view
    /// @details Observable property that can be bound to page titles or headers
    [ObservableProperty]
    private string title = string.Empty;

    /// @brief The current error message, if any
    /// @details Observable property that stores error messages for display to the user
    [ObservableProperty]
    private string errorMessage = string.Empty;

    /// @brief Indicates whether there is currently an error
    /// @details Observable property that indicates if an error state exists
    [ObservableProperty]
    private bool hasError;

    // Sets an error message and updates the error state.
    protected void SetError(string message)
    {
        ErrorMessage = message;
        HasError = !string.IsNullOrEmpty(message);
    }

    // Clears the current error state.
    protected void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    // Command to clear the current error state.
    [RelayCommand]
    private void ClearErrorCommand()
    {
        ClearError();
    }
}
