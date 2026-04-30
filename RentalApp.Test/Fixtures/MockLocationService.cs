// RentalApp.Test/Fixtures/MockLocationService.cs
using RentalApp.Services;

namespace RentalApp.Test.Fixtures;

// Update MockLocationService.cs
public class MockLocationService : ILocationService
{
    private readonly double? _lat;
    private readonly double? _lon;

    public MockLocationService(double? lat = 55.9533, double? lon = -3.1883)
    {
        _lat = lat;
        _lon = lon;
    }

    public Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync()
    {
        if (_lat == null || _lon == null)
            return Task.FromResult<(double, double)?>(null);
        return Task.FromResult<(double, double)?>((_lat.Value, _lon.Value));
    }
}