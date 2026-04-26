using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class CreateRentalViewModel : ObservableObject
{
    private readonly IRentalRepository _rentalRepository;
private readonly IAuthenticationService _authService;
    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today;

    [ObservableProperty]
    private DateTime _endDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public CreateRentalViewModel(IRentalRepository rentalRepository, IAuthenticationService authService)
    {
        _rentalRepository = rentalRepository;
        _authService = authService;
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (EndDate <= StartDate)
        {
            ErrorMessage = "End date must be after start date.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var borrowerId = _authService.CurrentUser?.Id ?? 0;
            await _rentalRepository.CreateAsync(ItemId, StartDate, EndDate, borrowerId);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error submitting request: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}