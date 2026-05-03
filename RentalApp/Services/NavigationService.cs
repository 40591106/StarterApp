namespace RentalApp.Services;

// Navigation service for moving between app routes using Shell navigation.
public class NavigationService : INavigationService
{
    // Navigates to the specified route.
    public async Task NavigateToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    // Navigates to a route with route parameters.
    public async Task NavigateToAsync(string route, Dictionary<string, object> parameters)
    {
        await Shell.Current.GoToAsync(route, parameters);
    }

    // Navigates back one step in the navigation stack.
    public async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    // Navigates to the app root route.
    public async Task NavigateToRootAsync()
    {
        await Shell.Current.GoToAsync("//login");
    }

    // Pops the navigation stack back to the root page.
    public async Task PopToRootAsync()
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}
