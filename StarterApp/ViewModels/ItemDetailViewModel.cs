using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), ("itemId"))]
public partial class ItemDetailViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Item? _item;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

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

    public bool CanEdit => Item?.OwnerId == _authService.CurrentUser?.Id;

    public ItemDetailViewModel(IItemRepository itemRepository, IAuthenticationService authService, INavigationService navigationService)
    {
        _itemRepository = itemRepository;
        _authService = authService;
        _navigationService = navigationService;
    }

    private async Task LoadItemAsync()
    {
        IsLoading = true;
        try
        {
            Item = await _itemRepository.GetByIdAsync(_itemId);
            OnPropertyChanged(nameof(CanEdit));
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

    [RelayCommand]
    private async Task NavigateToEditAsync()
    {
        await _navigationService.NavigateToAsync($"CreateItemPage?itemId={_itemId}");
    }

    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await _navigationService.NavigateToAsync("ItemsListPage");
    }
}
