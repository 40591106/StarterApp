using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

public partial class RentalsViewModel : ObservableObject
{
    private readonly IRentalService _rentalService;
    private readonly IRentalRepository _rentalRepository;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Rental> _incomingRentals = new();

    [ObservableProperty]
    private ObservableCollection<Rental> _outgoingRentals = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _showIncoming = true;

    public bool ShowOutgoing => !ShowIncoming;

    public Color IncomingTabColor => ShowIncoming ? Colors.DarkBlue : Colors.Gray;
    public Color OutgoingTabColor => !ShowIncoming ? Colors.DarkBlue : Colors.Gray;

    // Initializes a new instance of the RentalsViewModel class.
    public RentalsViewModel(
        IRentalService rentalService,
        IRentalRepository rentalRepository,
        IAuthenticationService authService,
        INavigationService navigationService
    )
    {
        _rentalService = rentalService;
        _rentalRepository = rentalRepository;
        _authService = authService;
        _navigationService = navigationService;
        _ = Task.Run(LoadRentalsAsync);
    }

    // Handles the change of ShowIncoming property.
    partial void OnShowIncomingChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowOutgoing));
        OnPropertyChanged(nameof(IncomingTabColor));
        OnPropertyChanged(nameof(OutgoingTabColor));
    }

    // Shows the incoming rentals tab.
    [RelayCommand]
    private void ShowIncomingTab() => ShowIncoming = true;

    // Shows the outgoing rentals tab.
    [RelayCommand]
    private void ShowOutgoingTab() => ShowIncoming = false;

    // Loads rentals asynchronously.
    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            await _rentalService.UpdateOutForRentAsync();
            var currentUserId = _authService.CurrentUser?.Id ?? 0;
            var incoming = await _rentalRepository.GetIncomingAsync(currentUserId);
            var outgoing = await _rentalRepository.GetOutgoingAsync(currentUserId);

            var activeStatuses = new[] { "Requested", "Approved", "Out for Rent", "Returned", "Completed" };

            IncomingRentals = new ObservableCollection<Rental>(
                incoming.Where(r => activeStatuses.Contains(r.Status))
            );
            OutgoingRentals = new ObservableCollection<Rental>(
                outgoing.Where(r => activeStatuses.Contains(r.Status))
            );
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading rentals: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Approves a rental request.
    [RelayCommand]
    private async Task ApproveRentalAsync(Rental rental)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            await _rentalService.ApproveRentalAsync(rental.Id);
            await LoadRentalsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error approving rental: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Rejects a rental request.
    [RelayCommand]
    private async Task RejectRentalAsync(Rental rental)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            await _rentalService.RejectRentalAsync(rental.Id);
            await LoadRentalsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error rejecting rental: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Marks a rental as out for rent.
    [RelayCommand]
    private async Task MarkOutForRentAsync(Rental rental)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            await _rentalService.MarkOutForRentAsync(rental.Id);
            await LoadRentalsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error updating rental: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Returns a rental.
    [RelayCommand]
    private async Task ReturnRentalAsync(Rental rental)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            await _rentalService.ReturnRentalAsync(rental.Id);
            await LoadRentalsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error returning rental: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Completes a rental.
    [RelayCommand]
    private async Task CompleteRentalAsync(Rental rental)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            await _rentalService.CompleteRentalAsync(rental.Id);
            await LoadRentalsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error completing rental: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Navigates to leave a review for the rental.
    [RelayCommand]
    private async Task LeaveReviewAsync(Rental rental)
    {
        await _navigationService.NavigateToAsync(
            $"CreateReviewPage?itemId={rental.ItemId}&rentalId={rental.Id}");
    }
}
