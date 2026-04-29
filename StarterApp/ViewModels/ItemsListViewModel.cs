using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

public partial class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private Category? _selectedCategory;

    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();

    partial void OnSearchTextChanged(string value) => _ = Task.Run(LoadItemsAsync);

    partial void OnSelectedCategoryChanged(Category? value)
    {
        _ = Task.Run(LoadItemsAsync);
        OnPropertyChanged(nameof(HasCategoryFilter));
    }

    [RelayCommand]
    private void ClearCategory() => SelectedCategory = null;

    public bool HasCategoryFilter => SelectedCategory != null;
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
        _ = Task.Run(async () =>
        {
            await LoadCategoriesAsync();
        });
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _itemRepository.GetCategoriesAsync();
        Categories = new ObservableCollection<Category>(categories);
    }

    private async Task LoadItemsAsync()
    {
        var items = await _itemRepository.GetAllAsync(
            category: SelectedCategory?.Slug,
            search: SearchText
        );
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
