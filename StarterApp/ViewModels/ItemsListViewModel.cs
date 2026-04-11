using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StarterApp.ViewModels;

public partial class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly INavigationService _navigationService;

    private ObservableCollection<Item>? _items;
    public ObservableCollection<Item> Items
    {
        get => _items!;
        set => SetProperty(ref _items, value);
    }

    public ICommand LoadItemsCommand { get; }
    public ICommand NavigateToDetailCommand { get; }
    public ICommand NavigateToCreateCommand { get; }

    public ItemsListViewModel(IItemRepository itemRepository, INavigationService navigationService)
    {
        _itemRepository = itemRepository;
        _navigationService = navigationService;
        LoadItemsCommand = new AsyncRelayCommand(LoadItemsAsync);
        NavigateToDetailCommand = new AsyncRelayCommand<int>(NavigateToDetailAsync);
        NavigateToCreateCommand = new AsyncRelayCommand(NavigateToCreateAsync);
    }

    private async Task LoadItemsAsync()
    {
        var items = await _itemRepository.GetAllAsync();
        Items = new ObservableCollection<Item>(items);
    }

    private async Task NavigateToDetailAsync(int id)
    {
        await _navigationService.NavigateToAsync($"ItemDetailPage?itemId={id}");
    }

    private async Task NavigateToCreateAsync()
    {
        await _navigationService.NavigateToAsync("CreateItemPage");
    }
}