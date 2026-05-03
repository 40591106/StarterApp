namespace RentalApp.Services;

/// <summary>
/// Contract for app navigation services using Shell routing.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to the specified Shell route.
    /// </summary>
    Task NavigateToAsync(string route);

    /// <summary>
    /// Navigates to the specified Shell route with parameters.
    /// </summary>
    Task NavigateToAsync(string route, Dictionary<string, object> parameters);

    /// <summary>
    /// Navigates back to the previous page.
    /// </summary>
    Task NavigateBackAsync();

    /// <summary>
    /// Navigates to the root page of the application.
    /// </summary>
    Task NavigateToRootAsync();

    /// <summary>
    /// Pops the navigation stack to the root page.
    /// </summary>
    Task PopToRootAsync();
}