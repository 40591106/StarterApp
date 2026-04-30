using RentalApp.ViewModels;

namespace RentalApp;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.MainPage), typeof(Views.MainPage));
        Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
        Routing.RegisterRoute(nameof(Views.RegisterPage), typeof(Views.RegisterPage));
        Routing.RegisterRoute(nameof(Views.ProfilePage), typeof(Views.ProfilePage));
        Routing.RegisterRoute(nameof(Views.ItemsListPage), typeof(Views.ItemsListPage));
        Routing.RegisterRoute(nameof(Views.ItemDetailPage), typeof(Views.ItemDetailPage));
        Routing.RegisterRoute(nameof(Views.CreateItemPage), typeof(Views.CreateItemPage));
        Routing.RegisterRoute(nameof(Views.RentalsPage), typeof(Views.RentalsPage));
        Routing.RegisterRoute(nameof(Views.CreateRentalPage), typeof(Views.CreateRentalPage));
        Routing.RegisterRoute(nameof(Views.ReviewsPage), typeof(Views.ReviewsPage));
        Routing.RegisterRoute(nameof(Views.CreateReviewPage), typeof(Views.CreateReviewPage));
        Routing.RegisterRoute(nameof(Views.NearbyItemsPage), typeof(Views.NearbyItemsPage));
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // var window = base.CreateWindow(activationState);
        // window.Page = new AppShell();

        var shell = _serviceProvider.GetService<AppShell>();
        if (shell == null)
        {
            // Handle the error if AppShell could not be resolved
            throw new InvalidOperationException(
                "AppShell could not be resolved from the service provider."
            );
        }
        var window = new Window(shell);
        return window;
    }
}
