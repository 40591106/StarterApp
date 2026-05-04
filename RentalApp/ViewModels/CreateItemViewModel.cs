using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class CreateItemViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;
    private readonly ILocationService _locationService;

    private Item? _currentItem;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _dailyRate;

    [ObservableProperty]
    private int _categoryId;

    [ObservableProperty]
    private List<Category> _categories = new();

    [ObservableProperty]
    private Category? _selectedCategory;

    [ObservableProperty]
    private double _latitude;

    [ObservableProperty]
    private double _longitude;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isNewItem = true;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    [ObservableProperty]
    private bool _useMyLocation;
    /// <summary>Gets the page title based on whether creating or editing.</summary>
    public string PageTitle => IsNewItem ? "Create New Item" : "Edit Item";

    private int _itemId;
    public int ItemId
    {
        get => _itemId;
        set
        {
            _itemId = value;
            OnPropertyChanged();
            _ = Task.Run(LoadItemAsync);
        }
    }

    public ICommand SaveItemCommand { get; }
    public ICommand NavigateBackCommand { get; }

    /// <summary>Initializes a new instance of the CreateItemViewModel class.</summary>

    public CreateItemViewModel(
        IItemRepository itemRepository,
        IAuthenticationService authService,
        INavigationService navigationService,
        ILocationService locationService
    )
    {
        _itemRepository = itemRepository;
        _authService = authService;
        _navigationService = navigationService;
        _locationService = locationService;

        SaveItemCommand = new AsyncRelayCommand(SaveItemAsync);
        NavigateBackCommand = new AsyncRelayCommand(NavigateBackAsync);

        _ = Task.Run(LoadCategoriesAsync); // load categories on startup for create mode
    }

    // Loads categories asynchronously.
    private async Task LoadCategoriesAsync()
    {
        var categories = await _itemRepository.GetCategoriesAsync();
        Categories = categories;
    }

    // Loads the item for editing asynchronously.
    private async Task LoadItemAsync()
    {
        if (_itemId == 0)
        {
            IsNewItem = true;
            await LoadCategoriesAsync();
            return;
        }

        IsLoading = true;
        try
        {
            IsNewItem = false;
            await LoadCategoriesAsync(); // load categories first
            _currentItem = await _itemRepository.GetByIdAsync(_itemId);

            Title = _currentItem!.Title;
            Description = _currentItem.Description;
            DailyRate = _currentItem.DailyRate;
            Latitude = _currentItem.Latitude ?? 0.0;
            Longitude = _currentItem.Longitude ?? 0.0;
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == _currentItem.CategoryId);

            OnPropertyChanged(nameof(PageTitle));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading item: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Saves the item asynchronously.
    private async Task SaveItemAsync()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Title) || DailyRate <= 0)
        {
            ErrorMessage = "Title and a valid daily rate are required.";
            return;
        }

        IsLoading = true;
        try
        {
            if (IsNewItem)
            {
                var item = new Item
                {
                    Title = Title.Trim(),
                    Description = Description.Trim(),
                    DailyRate = DailyRate,
                    CategoryId = SelectedCategory?.Id ?? 1,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    OwnerId = _authService.CurrentUser!.Id,
                };
                await _itemRepository.CreateAsync(item);
                SuccessMessage = "Item created successfully!";
            }
            else
            {
                _currentItem!.Title = Title.Trim();
                _currentItem.Description = Description.Trim();
                _currentItem.DailyRate = DailyRate;
                _currentItem.CategoryId = CategoryId;
                _currentItem.Latitude = Latitude;
                _currentItem.Longitude = Longitude;
                await _itemRepository.UpdateAsync(_currentItem);
                SuccessMessage = "Item updated successfully!";
            }

            await Task.Delay(1500);
            await NavigateBackAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving item: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Handles the change of UseMyLocation property, asks for permission to use device location when checked
    partial void OnUseMyLocationChanged(bool value)
    {
        if (value) _ = Task.Run(async () =>
        {
            var status = await MainThread.InvokeOnMainThreadAsync(
                () => Permissions.RequestAsync<Permissions.LocationWhenInUse>()
            );

            if (status != PermissionStatus.Granted)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UseMyLocation = false;
                    ErrorMessage = "Location permission is required.";
                });
                return;
            }

            var location = await _locationService.GetCurrentLocationAsync();
            if (location != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Latitude = location.Value.Latitude;
                    Longitude = location.Value.Longitude;
                });
            }
        });
    }

    // Navigates back to the items list page asynchronously.
    private async Task NavigateBackAsync()
    {
        await _navigationService.NavigateToAsync("ItemsListPage");
    }
}
