namespace RentalApp.Services;

/// <summary>
/// Contract for services that provide the current device GPS location.
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Gets the current device location as latitude and longitude coordinates.
    /// Returns null if location is unavailable or permission is denied.
    /// </summary>
    Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync();
}