namespace RentalApp.Services;

// Location service that retrieves the current device coordinates when permission is granted.
public class LocationService : ILocationService
{
    // Gets the current GPS location or returns null if unavailable.
    public async Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                return null;

            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            if (location == null)
                return null;

            return (location.Latitude, location.Longitude);
        }
        catch
        {
            return null;
        }
    }
}