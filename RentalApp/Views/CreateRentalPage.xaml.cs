using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class CreateRentalPage : ContentPage
{
    public CreateRentalPage(CreateRentalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
