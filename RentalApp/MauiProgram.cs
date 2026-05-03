using Microsoft.Extensions.Logging;
using RentalApp.Database.Data;
using RentalApp.Database.Data.Repositories;
using RentalApp.Services;
using RentalApp.ViewModels;
using RentalApp.Views;
using Plugin.LocalNotification;

namespace RentalApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        const bool useSharedApi = false; // Set to true to use the shared API, false to use local database

        if (useSharedApi)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://set09102-api.b-davison.workers.dev/"),
            };
            builder.Services.AddSingleton(httpClient);
            builder.Services.AddSingleton<IAuthenticationService, ApiAuthenticationService>();
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddScoped<IItemRepository, ApiItemRepository>();
            builder.Services.AddScoped<IRentalRepository, ApiRentalRepository>();
            builder.Services.AddScoped<IReviewRepository, ApiReviewRepository>();
        }
        else
        {
            builder.Services.AddDbContextFactory<AppDbContext>();
            builder.Services.AddSingleton<IAuthenticationService, LocalAuthenticationService>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<IRentalRepository, RentalRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
        }

        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // Shell
        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

        // Auth
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();

        // Main
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        // Items
        builder.Services.AddTransient<ItemsListViewModel>();
        builder.Services.AddTransient<ItemsListPage>();
        builder.Services.AddTransient<ItemDetailViewModel>();
        builder.Services.AddTransient<ItemDetailPage>();
        builder.Services.AddTransient<CreateItemViewModel>();
        builder.Services.AddTransient<CreateItemPage>();

        // Rentals
        builder.Services.AddTransient<RentalsViewModel>();
        builder.Services.AddTransient<RentalsPage>();
        builder.Services.AddTransient<CreateRentalViewModel>();
        builder.Services.AddTransient<CreateRentalPage>();
        builder.Services.AddTransient<IRentalService, RentalService>();

        // Reviews
        builder.Services.AddTransient<ReviewsViewModel>();
        builder.Services.AddTransient<ReviewsPage>();
        builder.Services.AddTransient<CreateReviewViewModel>();
        builder.Services.AddTransient<CreateReviewPage>();
        builder.Services.AddTransient<IReviewService, ReviewService>();

        // Profile
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ProfilePage>();

        // Nearby Items
        builder.Services.AddTransient<NearbyItemsViewModel>();
        builder.Services.AddTransient<NearbyItemsPage>();
        builder.Services.AddSingleton<ILocationService, LocationService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.UseLocalNotification();
        return builder.Build();
    }
}
