using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Domain.Eligibility;

public sealed class EligibilityPolicyTests
{
    private readonly EligibilityPolicy _policy = new();

    [Fact]
    public void WhenAge17WithCar_ShouldRejectWithDriverTooYoung()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 17, LicenseYears = 0 },
            new VehicleInfo { Type = VehicleType.Car });

        Assert.False(result.IsEligible);
        Assert.Equal(EligibilityRejectionReasons.DRIVER_TOO_YOUNG_FOR_VEHICLE, result.RejectionReason);
    }

    [Fact]
    public void WhenAge18WithCar_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 18, LicenseYears = 0 },
            new VehicleInfo { Type = VehicleType.Car });

        Assert.True(result.IsEligible);
    }

    [Fact]
    public void WhenAge17WithMotorcycle_ShouldRejectWithDriverTooYoung()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 17, LicenseYears = 0 },
            new VehicleInfo { Type = VehicleType.Motorcycle, Horsepower = 50 });

        Assert.False(result.IsEligible);
        Assert.Equal(EligibilityRejectionReasons.DRIVER_TOO_YOUNG_FOR_VEHICLE, result.RejectionReason);
    }

    [Fact]
    public void WhenAge16WithElectricScooter_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 16, LicenseYears = 0 },
            new VehicleInfo { Type = VehicleType.ElectricScooter });

        Assert.True(result.IsEligible);
    }

    [Fact]
    public void WhenAge15WithElectricScooter_ShouldRejectWithDriverTooYoung()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 15, LicenseYears = 0 },
            new VehicleInfo { Type = VehicleType.ElectricScooter });

        Assert.False(result.IsEligible);
        Assert.Equal(EligibilityRejectionReasons.DRIVER_TOO_YOUNG_FOR_VEHICLE, result.RejectionReason);
    }

    [Fact]
    public void WhenAge25With2YearLicenseAndMotorcycleAt100Hp_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 25, LicenseYears = 2 },
            new VehicleInfo { Type = VehicleType.Motorcycle, Horsepower = 100 });

        Assert.True(result.IsEligible);
    }

    [Fact]
    public void WhenAge25With4YearLicenseAndMotorcycleAt101Hp_ShouldRejectWithInsufficientExperience()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 25, LicenseYears = 4 },
            new VehicleInfo { Type = VehicleType.Motorcycle, Horsepower = 101 });

        Assert.False(result.IsEligible);
        Assert.Equal(EligibilityRejectionReasons.INSUFFICIENT_EXPERIENCE_FOR_POWER, result.RejectionReason);
    }

    [Fact]
    public void WhenAge25With5YearLicenseAndMotorcycleAt101Hp_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 25, LicenseYears = 5 },
            new VehicleInfo { Type = VehicleType.Motorcycle, Horsepower = 101 });

        Assert.True(result.IsEligible);
    }
}
