using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Windows.Input;

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
            System.Diagnostics.Debug.WriteLine($"ITEMID SET: {value}");
            _itemId = value;
            OnPropertyChanged();
            _ = Task.Run(LoadItemAsync);
        }
    }

    public bool CanEdit => Item?.OwnerId == _authService.CurrentUser?.Id;
public ICommand NavigateToEditAsyncCommand { get; }
public ICommand NavigateBackAsyncCommand { get; }
    public ItemDetailViewModel(IItemRepository itemRepository, IAuthenticationService authService, INavigationService navigationService)
{
    _itemRepository = itemRepository;
    _authService = authService;
    _navigationService = navigationService;
    
    NavigateToEditAsyncCommand = new AsyncRelayCommand(NavigateToEditAsync);
    NavigateBackAsyncCommand = new AsyncRelayCommand(NavigateBackAsync);
}

    private async Task LoadItemAsync()
    {
        System.Diagnostics.Debug.WriteLine($"LOADING ITEM: {_itemId}");
        IsLoading = true;
        try
        {
            Item = await _itemRepository.GetByIdAsync(_itemId);
            System.Diagnostics.Debug.WriteLine($"ITEM LOADED: {Item?.Title ?? "null"}");
            OnPropertyChanged(nameof(CanEdit));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading item: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"ITEM ERROR: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"STACK: {ex.StackTrace}");
        }
        finally
        {
            IsLoading = false;
        }
    }

private async Task NavigateToEditAsync()
{
    System.Diagnostics.Debug.WriteLine("EDIT COMMAND FIRED");
    await _navigationService.NavigateToAsync($"CreateItemPage?itemId={_itemId}");
}

private async Task NavigateBackAsync()
{
    await Shell.Current.GoToAsync("..");
}
}
