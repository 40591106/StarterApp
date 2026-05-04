using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(DailyRate), "dailyRate")]
[QueryProperty(nameof(ItemId), "itemId")]
public partial class CreateRentalViewModel : ObservableObject
{
    private readonly IRentalService _RentalService;
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

    [ObservableProperty]
    private decimal _dailyRate;

    /// <summary>Gets the estimated total cost based on daily rate and rental duration.</summary>
    public decimal EstimatedTotal => (decimal)(EndDate - StartDate).Days * DailyRate;

    /// <summary>Initializes a new instance of the CreateRentalViewModel class.</summary>
    public CreateRentalViewModel(IRentalService rentalService, IAuthenticationService authService)
    {
        _RentalService = rentalService;
        _authService = authService;
    }

    // Submits the rental request asynchronously.
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
            await _RentalService.RequestRentalAsync(ItemId, borrowerId, StartDate, EndDate);
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

    // Navigates back to the previous page.
    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    // Handles the change of StartDate property.
    partial void OnStartDateChanged(DateTime value) => OnPropertyChanged(nameof(EstimatedTotal));

    // Handles the change of EndDate property.
    partial void OnEndDateChanged(DateTime value) => OnPropertyChanged(nameof(EstimatedTotal));

    // Handles the change of DailyRate property.
    partial void OnDailyRateChanged(decimal value) => OnPropertyChanged(nameof(EstimatedTotal));
}
