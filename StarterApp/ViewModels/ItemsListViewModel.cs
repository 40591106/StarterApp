public class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private ObservableCollection<Item> _items;

    public ObservableCollection<Item> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    public ICommand LoadItemsCommand { get; }

    public ItemsListViewModel(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
        LoadItemsCommand = new AsyncRelayCommand(LoadItemsAsync);
    }

    private async Task LoadItemsAsync()
    {
        var items = await _itemRepository.GetAllAsync();
        Items = new ObservableCollection<Item>(items);
    }
}