using StarterApp.ViewModels;
using System.Diagnostics;

namespace StarterApp.Views;

public partial class ItemDetailPage : ContentPage
{
    public ItemDetailPage(ItemDetailViewModel viewModel)
{
    InitializeComponent();
    BindingContext = viewModel;
}
}