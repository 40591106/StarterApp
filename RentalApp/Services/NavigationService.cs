namespace RentalApp.Services;

// Navigation service for moving between app routes using Shell navigation.
/// <summary>
/// Provides Shell-based navigation for the MAUI application.
/// </summary>
public class NavigationService : INavigationService
{
    // Navigates to the specified route.
    /// <inheritdoc/>
    public async Task NavigateToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    // Navigates to a route with route parameters.
    /// <inheritdoc/>
    public async Task NavigateToAsync(string route, Dictionary<string, object> parameters)
    {
        await Shell.Current.GoToAsync(route, parameters);
    }

    // Navigates back one step in the navigation stack.
    /// <inheritdoc/>
    public async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    // Navigates to the app root route.
    /// <inheritdoc/>
    public async Task NavigateToRootAsync()
    {
        await Shell.Current.GoToAsync("//login");
    }

    // Pops the navigation stack back to the root page.
    /// <inheritdoc/>
    public async Task PopToRootAsync()
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}
