using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Windows.Input;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class CreateItemViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

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
    private string _location = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isNewItem = true;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

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

public CreateItemViewModel(IItemRepository itemRepository, IAuthenticationService authService, INavigationService navigationService)
{
    _itemRepository = itemRepository;
    _authService = authService;
    _navigationService = navigationService;
    
    SaveItemCommand = new AsyncRelayCommand(SaveItemAsync);
    NavigateBackCommand = new AsyncRelayCommand(NavigateBackAsync);
}

    private async Task LoadItemAsync()
    {
        if (_itemId == 0)
        {
            IsNewItem = true;
            return;
        }

        IsLoading = true;
        try
        {
            IsNewItem = false;
            _currentItem = await _itemRepository.GetByIdAsync(_itemId);

            if (_currentItem == null)
            {
                ErrorMessage = "Item not found";
                return;
            }

            Title = _currentItem.Title;
            Description = _currentItem.Description;
            DailyRate = _currentItem.DailyRate;
            CategoryId = _currentItem.CategoryId;
            Location = _currentItem.Location;

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
                    CategoryId = CategoryId,
                    Location = Location.Trim(),
                    OwnerId = _authService.CurrentUser!.Id
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
                _currentItem.Location = Location.Trim();
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

    private async Task NavigateBackAsync()
    {
        await _navigationService.NavigateToAsync("ItemsListPage");
    }
}