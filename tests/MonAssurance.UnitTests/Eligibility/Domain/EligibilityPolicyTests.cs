// tests/MonAssurance.UnitTests/Eligibility/Domain/EligibilityPolicyTests.cs
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Eligibility.Domain;

public class EligibilityPolicyTests
{
    private static readonly DateOnly Today = new(2026, 1, 1);
    private readonly EligibilityPolicy _policy = new();

    // ── Age boundaries ──────────────────────────────────────────────────────

    [Fact]
    public void Evaluate_WhenDriverTurns21ExactlyToday_ReturnsAccepted()
    {
        var driver = new Driver(Today.AddYears(-21), licenseYears: 2);
        var vehicle = new Vehicle(VehicleType.Car, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        var wasAccepted = result.Match(onAccepted: () => true, onRefused: _ => false);
        Assert.True(wasAccepted);
    }

    [Fact]
    public void Evaluate_WhenDriverTurns21Tomorrow_ReturnsRefused()
    {
        var driver = new Driver(Today.AddYears(-21).AddDays(1), licenseYears: 2);
        var vehicle = new Vehicle(VehicleType.Car, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        var (wasAccepted, capturedReason) = result.Match(
            onAccepted: () => (true, (string?)null),
            onRefused: r => (false, (string?)r));
        Assert.False(wasAccepted);
        Assert.Equal("Conducteur trop jeune pour ce véhicule", capturedReason);
    }

    // AC-01, AC-03, AC-04 — drivers below the new legal minimum age (21) must be refused
    [Theory]
    [InlineData(20, VehicleType.Car)]
    [InlineData(20, VehicleType.Motorcycle)]
    [InlineData(18, VehicleType.Car)]
    public void Evaluate_WhenDriverBelowNewLegalAge_ReturnsRefused(int age, VehicleType vehicleType)
    {
        var driver = new Driver(Today.AddYears(-age), licenseYears: 10);
        var vehicle = new Vehicle(vehicleType, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        var (wasAccepted, capturedReason) = result.Match(
            onAccepted: () => (true, (string?)null),
            onRefused: r => (false, (string?)r));
        Assert.False(wasAccepted);
        Assert.Equal("Conducteur trop jeune pour ce véhicule", capturedReason);
    }

    [Fact]
    public void Evaluate_WhenElectricScooterDriverTurns16ExactlyToday_ReturnsAccepted()
    {
        var driver = new Driver(Today.AddYears(-16), licenseYears: 0);
        var vehicle = new Vehicle(VehicleType.ElectricScooter, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        var wasAccepted = result.Match(onAccepted: () => true, onRefused: _ => false);
        Assert.True(wasAccepted);
    }

    [Fact]
    public void Evaluate_WhenElectricScooterDriverTurns16Tomorrow_ReturnsRefused()
    {
        var driver = new Driver(Today.AddYears(-16).AddDays(1), licenseYears: 0);
        var vehicle = new Vehicle(VehicleType.ElectricScooter, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        var (wasAccepted, capturedReason) = result.Match(
            onAccepted: () => (true, (string?)null),
            onRefused: r => (false, (string?)r));
        Assert.False(wasAccepted);
        Assert.Equal("Conducteur trop jeune pour ce véhicule", capturedReason);
    }

    // ── Power boundaries ─────────────────────────────────────────────────────

    [Fact]
    public void Evaluate_WhenMotorcycleIsExactly100HpAnd4YearsLicense_ReturnsAccepted()
    {
        var driver = new Driver(Today.AddYears(-25), licenseYears: 4);
        var vehicle = new Vehicle(VehicleType.Motorcycle, power: 100);

        var result = _policy.Evaluate(driver, vehicle, Today);

        var wasAccepted = result.Match(onAccepted: () => true, onRefused: _ => false);
        Assert.True(wasAccepted);
    }

    [Fact]
    public void Evaluate_WhenMotorcycleIs101HpAnd4YearsLicense_ReturnsRefused()
    {
        var driver = new Driver(Today.AddYears(-25), licenseYears: 4);
        var vehicle = new Vehicle(VehicleType.Motorcycle, power: 101);

        var result = _policy.Evaluate(driver, vehicle, Today);

        var (wasAccepted, capturedReason) = result.Match(
            onAccepted: () => (true, (string?)null),
            onRefused: r => (false, (string?)r));
        Assert.False(wasAccepted);
        Assert.Equal("Expérience insuffisante pour la puissance", capturedReason);
    }

    // ── Experience boundaries ─────────────────────────────────────────────────

    [Fact]
    public void Evaluate_WhenMotorcycleIs101HpAndExactly5YearsLicense_ReturnsAccepted()
    {
        var driver = new Driver(Today.AddYears(-25), licenseYears: 5);
        var vehicle = new Vehicle(VehicleType.Motorcycle, power: 101);

        var result = _policy.Evaluate(driver, vehicle, Today);

        var wasAccepted = result.Match(onAccepted: () => true, onRefused: _ => false);
        Assert.True(wasAccepted);
    }

    [Fact]
    public void Evaluate_WhenMotorcycleIs101HpAndExactly4YearsLicense_ReturnsRefused()
    {
        var driver = new Driver(Today.AddYears(-25), licenseYears: 4);
        var vehicle = new Vehicle(VehicleType.Motorcycle, power: 101);

        var result = _policy.Evaluate(driver, vehicle, Today);

        var (wasAccepted, capturedReason) = result.Match(
            onAccepted: () => (true, (string?)null),
            onRefused: r => (false, (string?)r));
        Assert.False(wasAccepted);
        Assert.Equal("Expérience insuffisante pour la puissance", capturedReason);
    }

    // ── Null power ─────────────────────────────────────────────────────────────

    [Fact]
    public void Evaluate_WhenMotorcycleHasNullPowerAnd4YearsLicense_ReturnsAccepted()
    {
        var driver = new Driver(Today.AddYears(-25), licenseYears: 4);
        var vehicle = new Vehicle(VehicleType.Motorcycle, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        var wasAccepted = result.Match(onAccepted: () => true, onRefused: _ => false);
        Assert.True(wasAccepted);
    }
}
