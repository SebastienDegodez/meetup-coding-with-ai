// tests/MonAssurance.UnitTests/Eligibility/Application/CheckEligibilityMinimumAgeTests.cs
using Microsoft.Extensions.Time.Testing;
using MonAssurance.Application.Eligibility.Queries.CheckEligibility;
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Eligibility.Application;

public class CheckEligibilityMinimumAgeTests
{
    private static readonly DateOnly Today = new(2026, 1, 1);

    private static CheckEligibilityQueryHandler BuildHandler()
    {
        var fakeTime = new FakeTimeProvider();
        fakeTime.SetUtcNow(Today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        return new CheckEligibilityQueryHandler(new EligibilityPolicy(), fakeTime);
    }

    // AC-02: Driver aged 21 applying for a car is accepted
    [Fact]
    public void Handle_WhenDriverIs21AndHasCar_ReturnsEligible()
    {
        var handler = BuildHandler();
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-21),
            VehicleType: VehicleType.Car,
            Power: null,
            LicenseYears: 2);

        var result = handler.Handle(query);

        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }

    // AC-04: Driver aged 16 applying for an electric scooter is accepted
    [Fact]
    public void Handle_WhenDriverIs16AndHasElectricScooter_ReturnsEligible()
    {
        var handler = BuildHandler();
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-16),
            VehicleType: VehicleType.ElectricScooter,
            Power: null,
            LicenseYears: 0);

        var result = handler.Handle(query);

        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }

    // AC-01: Driver aged 20 applying for a car is refused
    [Fact]
    public void Handle_WhenDriverIs20AndHasCar_ReturnsRefused()
    {
        var handler = BuildHandler();
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-20),
            VehicleType: VehicleType.Car,
            Power: null,
            LicenseYears: 2);

        var result = handler.Handle(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Conducteur trop jeune pour ce véhicule", result.RejectionReason);
    }

    // AC-03: Driver aged 20 applying for a motorcycle is refused
    [Fact]
    public void Handle_WhenDriverIs20AndHasMotorcycle_ReturnsRefused()
    {
        var handler = BuildHandler();
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-20),
            VehicleType: VehicleType.Motorcycle,
            Power: 80,
            LicenseYears: 1);

        var result = handler.Handle(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Conducteur trop jeune pour ce véhicule", result.RejectionReason);
    }

    // AC-05: Driver aged 18 applying for a car is refused (boundary: previously eligible, now refused)
    [Fact]
    public void Handle_WhenDriverIs18AndHasCar_ReturnsRefused()
    {
        var handler = BuildHandler();
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-18),
            VehicleType: VehicleType.Car,
            Power: null,
            LicenseYears: 2);

        var result = handler.Handle(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Conducteur trop jeune pour ce véhicule", result.RejectionReason);
    }
}
