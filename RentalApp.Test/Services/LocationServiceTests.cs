// RentalApp.Test/Services/LocationServiceTests.cs
using RentalApp.Services;
using RentalApp.Test.Fixtures;
using Xunit;

namespace RentalApp.Test.Services;

public class LocationServiceTests
{
    [Fact]
    public async Task GetCurrentLocationAsync_ReturnsConfiguredLocation()
    {
        // Arrange
        var mockLocation = new MockLocationService(55.9533, -3.1883);

        // Act
        var result = await mockLocation.GetCurrentLocationAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(55.9533, result.Value.Latitude);
        Assert.Equal(-3.1883, result.Value.Longitude);
    }

    [Fact]
    public async Task GetCurrentLocationAsync_DefaultLocation_ReturnsEdinburgh()
    {
        // Arrange
        var mockLocation = new MockLocationService();

        // Act
        var result = await mockLocation.GetCurrentLocationAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(55.9533, result.Value.Latitude);
        Assert.Equal(-3.1883, result.Value.Longitude);
    }

    [Theory]
    [InlineData(55.9533, -3.1883)]  // Edinburgh
    [InlineData(55.8642, -4.2518)]  // Glasgow
    [InlineData(51.5074, -0.1278)]  // London
    public async Task GetCurrentLocationAsync_VariousLocations_ReturnsCorrectCoordinates(
        double lat, double lon)
    {
        // Arrange
        var mockLocation = new MockLocationService(lat, lon);

        // Act
        var result = await mockLocation.GetCurrentLocationAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(lat, result.Value.Latitude);
        Assert.Equal(lon, result.Value.Longitude);
    }
}