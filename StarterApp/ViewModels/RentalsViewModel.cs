using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using System.Collections.ObjectModel;
using StarterApp.Services;

namespace StarterApp.ViewModels;

public partial class RentalsViewModel : ObservableObject
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IAuthenticationService _authService;

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

    public Color IncomingTabColor => ShowIncoming
    ? Colors.DarkBlue : Colors.Gray;
    public Color OutgoingTabColor => !ShowIncoming
        ? Colors.DarkBlue : Colors.Gray;

    public RentalsViewModel(IRentalRepository rentalRepository, IAuthenticationService authService)
    {
        _rentalRepository = rentalRepository;
        _authService = authService;
        _ = Task.Run(LoadRentalsAsync);
    }

    partial void OnShowIncomingChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowOutgoing));
        OnPropertyChanged(nameof(IncomingTabColor));
        OnPropertyChanged(nameof(OutgoingTabColor));
    }

    [RelayCommand]
    private void ShowIncomingTab() => ShowIncoming = true;

    [RelayCommand]
    private void ShowOutgoingTab() => ShowIncoming = false;

    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var currentUserId = _authService.CurrentUser?.Id ?? 0;
            var incoming = await _rentalRepository.GetIncomingAsync(currentUserId);
            var outgoing = await _rentalRepository.GetOutgoingAsync(currentUserId);

            IncomingRentals = new ObservableCollection<Rental>(incoming);
            OutgoingRentals = new ObservableCollection<Rental>(outgoing);
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
}