using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

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
    public bool CanRent => !CanEdit;
    public ICommand NavigateToEditAsyncCommand { get; }
    public ICommand NavigateToRentAsyncCommand { get; }
    public ICommand NavigateBackAsyncCommand { get; }
    public ICommand NavigateToReviewsAsyncCommand { get; }
    public ICommand NavigateToOwnerProfileAsyncCommand { get; }


    // Initializes a new instance of the ItemDetailViewModel class.
    public ItemDetailViewModel(
        IItemRepository itemRepository,
        IAuthenticationService authService,
        INavigationService navigationService
    )
    {
        _itemRepository = itemRepository;
        _authService = authService;
        _navigationService = navigationService;

        NavigateToEditAsyncCommand = new AsyncRelayCommand(NavigateToEditAsync);
        NavigateToRentAsyncCommand = new AsyncRelayCommand(NavigateToRentAsync);
        NavigateBackAsyncCommand = new AsyncRelayCommand(NavigateBackAsync);
        NavigateToReviewsAsyncCommand = new AsyncRelayCommand(NavigateToReviewsAsync);
        NavigateToOwnerProfileAsyncCommand = new AsyncRelayCommand(NavigateToOwnerProfileAsync);
    }

    // Loads the item details asynchronously.
    private async Task LoadItemAsync()
    {
        System.Diagnostics.Debug.WriteLine($"LOADING ITEM: {_itemId}");
        IsLoading = true;
        try
        {
            Item = await _itemRepository.GetByIdAsync(_itemId);
            System.Diagnostics.Debug.WriteLine($"ITEM LOADED: {Item?.Title ?? "null"}");
            OnPropertyChanged(nameof(CanEdit));
            OnPropertyChanged(nameof(CanRent));
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

    // Navigates to the edit item page.
    private async Task NavigateToEditAsync()
    {
        System.Diagnostics.Debug.WriteLine("EDIT COMMAND FIRED");
        await _navigationService.NavigateToAsync($"CreateItemPage?itemId={_itemId}");
    }

    // Navigates to the create rental page.
    private async Task NavigateToRentAsync()
    {
        System.Diagnostics.Debug.WriteLine("RENT COMMAND FIRED");
        await _navigationService.NavigateToAsync(
            $"CreateRentalPage?itemId={_itemId}&dailyRate={Item!.DailyRate}"
        );
    }

    // Navigates back to the previous page.
    private async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    // Navigates to the reviews page for the item.
    private async Task NavigateToReviewsAsync()
    {
        await _navigationService.NavigateToAsync($"ReviewsPage?itemId={_itemId}");
    }

    // Navigates to the owner's profile page.
    private async Task NavigateToOwnerProfileAsync()
    {
        await _navigationService.NavigateToAsync(
            $"ProfilePage?userId={Item!.OwnerId}&userName={Item.OwnerName}");
    }


}
