// tests/MonAssurance.UnitTests/Eligibility/Domain/VehicleMinimumAgeTests.cs
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Eligibility.Domain;

public class VehicleMinimumAgeTests
{
    [Theory]
    [InlineData(VehicleType.Car)]
    [InlineData(VehicleType.Motorcycle)]
    public void MinimumAge_WhenVehicleRequiresFullLicence_Returns21(VehicleType type)
    {
        var vehicle = new Vehicle(type, power: null);

        Assert.Equal(21, vehicle.MinimumAge());
    }

    [Fact]
    public void MinimumAge_WhenVehicleIsElectricScooter_Returns16()
    {
        var vehicle = new Vehicle(VehicleType.ElectricScooter, power: null);

        Assert.Equal(16, vehicle.MinimumAge());
    }
}
