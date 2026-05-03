namespace RentalApp.Services;

// Contract for services that provide the current device location.
public interface ILocationService
{
    Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync();
}