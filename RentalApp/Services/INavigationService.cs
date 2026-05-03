namespace RentalApp.Services;

// Contract for app navigation services using Shell routing.
public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task NavigateToAsync(string route, Dictionary<string, object> parameters);
    Task NavigateBackAsync();
    Task NavigateToRootAsync();
    Task PopToRootAsync();
}
