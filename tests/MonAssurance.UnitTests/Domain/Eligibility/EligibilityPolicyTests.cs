using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Domain.Eligibility;

public sealed class EligibilityPolicyTests
{
    private readonly EligibilityPolicy _policy = new();

    [Fact]
    public void WhenAge17WithCar_ShouldRejectWithDriverTooYoung()
    {
        var result = _policy.Evaluate(
            new DriverInfo(new DriverAge(17), new LicenseExperience(0)),
            new VehicleInfo(VehicleType.Car));

        Assert.False(result.IsAccepted());
        Assert.Equal(EligibilityRejectionReasons.DRIVER_TOO_YOUNG_FOR_VEHICLE, result.RejectionReason());
    }

    [Fact]
    public void WhenAge18WithCar_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo(new DriverAge(18), new LicenseExperience(0)),
            new VehicleInfo(VehicleType.Car));

        Assert.True(result.IsAccepted());
    }

    [Fact]
    public void WhenAge17WithMotorcycle_ShouldRejectWithDriverTooYoung()
    {
        var result = _policy.Evaluate(
            new DriverInfo(new DriverAge(17), new LicenseExperience(0)),
            new VehicleInfo(VehicleType.Motorcycle, new Horsepower(50)));

        Assert.False(result.IsAccepted());
        Assert.Equal(EligibilityRejectionReasons.DRIVER_TOO_YOUNG_FOR_VEHICLE, result.RejectionReason());
    }

    [Fact]
    public void WhenAge16WithElectricScooter_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo(new DriverAge(16), new LicenseExperience(0)),
            new VehicleInfo(VehicleType.ElectricScooter));

        Assert.True(result.IsAccepted());
    }

    [Fact]
    public void WhenAge15WithElectricScooter_ShouldRejectWithDriverTooYoung()
    {
        var result = _policy.Evaluate(
            new DriverInfo(new DriverAge(15), new LicenseExperience(0)),
            new VehicleInfo(VehicleType.ElectricScooter));

        Assert.False(result.IsAccepted());
        Assert.Equal(EligibilityRejectionReasons.DRIVER_TOO_YOUNG_FOR_VEHICLE, result.RejectionReason());
    }

    [Fact]
    public void WhenAge25With2YearLicenseAndMotorcycleAt100Hp_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo(new DriverAge(25), new LicenseExperience(2)),
            new VehicleInfo(VehicleType.Motorcycle, new Horsepower(100)));

        Assert.True(result.IsAccepted());
    }

    [Fact]
    public void WhenAge25With4YearLicenseAndMotorcycleAt101Hp_ShouldRejectWithInsufficientExperience()
    {
        var result = _policy.Evaluate(
            new DriverInfo(new DriverAge(25), new LicenseExperience(4)),
            new VehicleInfo(VehicleType.Motorcycle, new Horsepower(101)));

        Assert.False(result.IsAccepted());
        Assert.Equal(EligibilityRejectionReasons.INSUFFICIENT_EXPERIENCE_FOR_POWER, result.RejectionReason());
    }

    [Fact]
    public void WhenAge25With5YearLicenseAndMotorcycleAt101Hp_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo(new DriverAge(25), new LicenseExperience(5)),
            new VehicleInfo(VehicleType.Motorcycle, new Horsepower(101)));

        Assert.True(result.IsAccepted());
    }
}
