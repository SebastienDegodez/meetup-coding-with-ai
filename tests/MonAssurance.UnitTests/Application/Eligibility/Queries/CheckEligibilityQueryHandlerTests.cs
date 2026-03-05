using MonAssurance.Application.Eligibility.Queries.CheckEligibility;
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Application.Eligibility.Queries;

public sealed class CheckEligibilityQueryHandlerTests
{
    [Fact]
    public async Task WhenDriverIsMinorAndVehicleIsCar_ShouldRefuseWithDriverTooYoungReason()
    {
        var handler = new CheckEligibilityQueryHandler();
        var query = new CheckEligibilityQuery
        {
            DriverAge = 15,
            DriverLicenseYears = 0,
            VehicleType = VehicleType.Car
        };

        var result = await handler.HandleAsync(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Driver too young for this vehicle", result.RejectionReason);
    }

    [Fact]
    public async Task WhenDriverIs16AndVehicleIsElectricScooter_ShouldAccept()
    {
        var handler = new CheckEligibilityQueryHandler();
        var query = new CheckEligibilityQuery
        {
            DriverAge = 16,
            DriverLicenseYears = 0,
            VehicleType = VehicleType.ElectricScooter
        };

        var result = await handler.HandleAsync(query);

        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }

    [Fact]
    public async Task WhenDriverHas2YearsLicenseAndMotorcycleIsOver100Hp_ShouldRefuseWithInsufficientExperienceReason()
    {
        var handler = new CheckEligibilityQueryHandler();
        var query = new CheckEligibilityQuery
        {
            DriverAge = 25,
            DriverLicenseYears = 2,
            VehicleType = VehicleType.Motorcycle,
            VehicleHorsepower = 120
        };

        var result = await handler.HandleAsync(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Insufficient experience for the power", result.RejectionReason);
    }
}
