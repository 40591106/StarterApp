using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

public partial class NearbyItemsViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly ILocationService _locationService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private double _radiusKm = 5.0;

    [ObservableProperty]
    private string _locationStatus = string.Empty;

    public NearbyItemsViewModel(
        IItemRepository itemRepository,
        ILocationService locationService,
        INavigationService navigationService)
    {
        _itemRepository = itemRepository;
        _locationService = locationService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task FindNearbyAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        LocationStatus = "Getting your location...";
        try
        {

            var location = await _locationService.GetCurrentLocationAsync();
            if (location == null)
            {
                ErrorMessage = "Could not get your location. Please check permissions.";
                LocationStatus = string.Empty;
                return;
            }
            System.Diagnostics.Debug.WriteLine($"SEARCHING FROM: {location.Value.Latitude}, {location.Value.Longitude} RADIUS: {RadiusKm}");
            LocationStatus = $"Searching within {RadiusKm}km...";
            var items = await _itemRepository.GetNearbyAsync(
                location.Value.Latitude,
                location.Value.Longitude,
                RadiusKm);
            Items = new ObservableCollection<Item>(items);
            LocationStatus = $"Found {items.Count} items within {RadiusKm}km";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error finding nearby items: {ex.Message}";
            LocationStatus = string.Empty;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(int id)
    {
        await _navigationService.NavigateToAsync($"ItemDetailPage?itemId={id}");
    }
}