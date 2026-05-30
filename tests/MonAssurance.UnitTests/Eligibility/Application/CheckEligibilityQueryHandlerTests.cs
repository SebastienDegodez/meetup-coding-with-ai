// tests/MonAssurance.UnitTests/Eligibility/Application/CheckEligibilityQueryHandlerTests.cs
using Microsoft.Extensions.Time.Testing;
using MonAssurance.Application.Eligibility.Queries.CheckEligibility;
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Eligibility.Application;

public class CheckEligibilityQueryHandlerTests
{
    private static readonly DateOnly Today = new(2026, 1, 1);

    private static CheckEligibilityQueryHandler BuildHandler(DateOnly today)
    {
        var fakeTime = new FakeTimeProvider();
        fakeTime.SetUtcNow(today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        return new CheckEligibilityQueryHandler(new EligibilityPolicy(), fakeTime);
    }

    // ── Age boundaries — standard vehicles (Car / Motorcycle) ───────────────

    // AC-01, AC-03, AC-04: drivers below the legal minimum (21) are refused
    [Theory]
    [InlineData(20, VehicleType.Car)]        // AC-01: one year below the new threshold
    [InlineData(20, VehicleType.Motorcycle)] // AC-03: motorcycle, one year below
    [InlineData(18, VehicleType.Car)]        // AC-04: regression guard — old threshold now refused
    [InlineData(17, VehicleType.Car)]
    [InlineData(17, VehicleType.Motorcycle)]
    public void Handle_WhenDriverIsBelowMinimumAge_ReturnsRefused(int age, VehicleType vehicleType)
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-age),
            VehicleType: vehicleType,
            Power: null,
            LicenseYears: 1);

        var result = handler.Handle(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Conducteur trop jeune pour ce véhicule", result.RejectionReason);
    }

    // AC-02, AC-03 boundary: drivers at exactly 21 are eligible for Car and Motorcycle
    [Theory]
    [InlineData(VehicleType.Car)]        // AC-02
    [InlineData(VehicleType.Motorcycle)] // AC-03 boundary
    public void Handle_WhenDriverIsExactly21_ReturnsEligible(VehicleType vehicleType)
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-21),
            VehicleType: vehicleType,
            Power: null,
            LicenseYears: 2);

        var result = handler.Handle(query);

        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }

    // ── Electric scooter age boundary ───────────────────────────────────────

    [Fact]
    public void Handle_WhenDriverIs15AndHasElectricScooter_ReturnsRefused()
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-15),
            VehicleType: VehicleType.ElectricScooter,
            Power: null,
            LicenseYears: 0);

        var result = handler.Handle(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Conducteur trop jeune pour ce véhicule", result.RejectionReason);
    }

    [Fact]
    public void Handle_WhenDriverIs16AndHasElectricScooter_ReturnsEligible()
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-16),
            VehicleType: VehicleType.ElectricScooter,
            Power: null,
            LicenseYears: 0);

        var result = handler.Handle(query);

        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }

    // ── High-power motorcycle experience rule ────────────────────────────────

    [Fact]
    public void Handle_WhenMotorcycleIsHighPowerAndDriverHas4YearsLicense_ReturnsRefused()
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-25),
            VehicleType: VehicleType.Motorcycle,
            Power: 101,
            LicenseYears: 4);

        var result = handler.Handle(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Expérience insuffisante pour la puissance", result.RejectionReason);
    }

    [Fact]
    public void Handle_WhenMotorcycleIsHighPowerAndDriverHas5YearsLicense_ReturnsEligible()
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-25),
            VehicleType: VehicleType.Motorcycle,
            Power: 101,
            LicenseYears: 5);

        var result = handler.Handle(query);

        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }

    [Fact]
    public void Handle_WhenMotorcycleIsExactly100HpAndDriverHas4YearsLicense_ReturnsEligible()
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-25),
            VehicleType: VehicleType.Motorcycle,
            Power: 100,
            LicenseYears: 4);

        var result = handler.Handle(query);

        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }
}
