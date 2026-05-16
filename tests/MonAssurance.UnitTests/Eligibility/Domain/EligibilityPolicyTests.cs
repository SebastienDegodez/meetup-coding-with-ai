// tests/MonAssurance.UnitTests/Eligibility/Domain/EligibilityPolicyTests.cs
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Eligibility.Domain;

public class EligibilityPolicyTests
{
    private static readonly DateOnly Today = new(2026, 1, 1);
    private readonly EligibilityPolicy _policy = new();

    // ── Age boundaries ──────────────────────────────────────────────────────

    [Fact]
    public void Evaluate_WhenDriverTurns18ExactlyToday_ReturnsAccepted()
    {
        var driver = new Driver(Today.AddYears(-18), licenseYears: 2);
        var vehicle = new Vehicle(VehicleType.Car, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        result.Match(
            onAccepted: () => true,
            onRefused: _ => throw new Exception("Expected Accepted"));
    }

    [Fact]
    public void Evaluate_WhenDriverTurns18Tomorrow_ReturnsRefused()
    {
        var driver = new Driver(Today.AddYears(-18).AddDays(1), licenseYears: 2);
        var vehicle = new Vehicle(VehicleType.Car, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        result.Match(
            onAccepted: () => throw new Exception("Expected Refused"),
            onRefused: reason =>
            {
                Assert.Equal("Conducteur trop jeune pour ce véhicule", reason);
                return true;
            });
    }

    [Fact]
    public void Evaluate_WhenElectricScooterDriverTurns16ExactlyToday_ReturnsAccepted()
    {
        var driver = new Driver(Today.AddYears(-16), licenseYears: 0);
        var vehicle = new Vehicle(VehicleType.ElectricScooter, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        result.Match(
            onAccepted: () => true,
            onRefused: _ => throw new Exception("Expected Accepted"));
    }

    [Fact]
    public void Evaluate_WhenElectricScooterDriverTurns16Tomorrow_ReturnsRefused()
    {
        var driver = new Driver(Today.AddYears(-16).AddDays(1), licenseYears: 0);
        var vehicle = new Vehicle(VehicleType.ElectricScooter, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        result.Match(
            onAccepted: () => throw new Exception("Expected Refused"),
            onRefused: reason =>
            {
                Assert.Equal("Conducteur trop jeune pour ce véhicule", reason);
                return true;
            });
    }

    // ── Power boundaries ─────────────────────────────────────────────────────

    [Fact]
    public void Evaluate_WhenMotorcycleIsExactly100HpAnd4YearsLicense_ReturnsAccepted()
    {
        var driver = new Driver(Today.AddYears(-25), licenseYears: 4);
        var vehicle = new Vehicle(VehicleType.Motorcycle, power: 100);

        var result = _policy.Evaluate(driver, vehicle, Today);

        result.Match(
            onAccepted: () => true,
            onRefused: _ => throw new Exception("Expected Accepted"));
    }

    [Fact]
    public void Evaluate_WhenMotorcycleIs101HpAnd4YearsLicense_ReturnsRefused()
    {
        var driver = new Driver(Today.AddYears(-25), licenseYears: 4);
        var vehicle = new Vehicle(VehicleType.Motorcycle, power: 101);

        var result = _policy.Evaluate(driver, vehicle, Today);

        result.Match(
            onAccepted: () => throw new Exception("Expected Refused"),
            onRefused: reason =>
            {
                Assert.Equal("Expérience insuffisante pour la puissance", reason);
                return true;
            });
    }

    // ── Experience boundaries ─────────────────────────────────────────────────

    [Fact]
    public void Evaluate_WhenMotorcycleIs101HpAndExactly5YearsLicense_ReturnsAccepted()
    {
        var driver = new Driver(Today.AddYears(-25), licenseYears: 5);
        var vehicle = new Vehicle(VehicleType.Motorcycle, power: 101);

        var result = _policy.Evaluate(driver, vehicle, Today);

        result.Match(
            onAccepted: () => true,
            onRefused: _ => throw new Exception("Expected Accepted"));
    }

    [Fact]
    public void Evaluate_WhenMotorcycleIs101HpAndExactly4YearsLicense_ReturnsRefused()
    {
        var driver = new Driver(Today.AddYears(-25), licenseYears: 4);
        var vehicle = new Vehicle(VehicleType.Motorcycle, power: 101);

        var result = _policy.Evaluate(driver, vehicle, Today);

        result.Match(
            onAccepted: () => throw new Exception("Expected Refused"),
            onRefused: reason =>
            {
                Assert.Equal("Expérience insuffisante pour la puissance", reason);
                return true;
            });
    }

    // ── Null power ─────────────────────────────────────────────────────────────

    [Fact]
    public void Evaluate_WhenMotorcycleHasNullPowerAnd4YearsLicense_ReturnsAccepted()
    {
        var driver = new Driver(Today.AddYears(-25), licenseYears: 4);
        var vehicle = new Vehicle(VehicleType.Motorcycle, power: null);

        var result = _policy.Evaluate(driver, vehicle, Today);

        result.Match(
            onAccepted: () => true,
            onRefused: _ => throw new Exception("Expected Accepted"));
    }
}
