// tests/MonAssurance.UnitTests/Eligibility/Application/LegalAgeChangeAcceptanceTests.cs
// OUTER LOOP — Acceptance tests for STORY-72 (âge légal 18 → 21)
// Each test MUST fail when Vehicle.MinimumAge() is reverted to 18.
using Microsoft.Extensions.Time.Testing;
using MonAssurance.Application.Eligibility.Queries.CheckEligibility;
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Eligibility.Application;

public class LegalAgeChangeAcceptanceTests
{
    private static readonly DateOnly Today = new(2026, 1, 1);

    private static CheckEligibilityQueryHandler BuildHandler(DateOnly today)
    {
        var fakeTime = new FakeTimeProvider();
        fakeTime.SetUtcNow(today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        return new CheckEligibilityQueryHandler(new EligibilityPolicy(), fakeTime);
    }

    // AC-01 — Conducteur de 20 ans avec voiture est refusé (nouvelle loi; serait accepté sous ancienne loi min=18)
    [Fact]
    public void Handle_WhenDriverIs20AndHasCar_ReturnsRefused()
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-20),
            VehicleType: VehicleType.Car,
            Power: null,
            LicenseYears: 2);

        var result = handler.Handle(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Conducteur trop jeune pour ce véhicule", result.RejectionReason);
    }

    // AC-04 — Conducteur de 19 ans avec voiture est refusé (serait accepté sous ancienne loi min=18)
    [Fact]
    public void Handle_WhenDriverIs19AndHasCar_ReturnsRefused()
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-19),
            VehicleType: VehicleType.Car,
            Power: null,
            LicenseYears: 1);

        var result = handler.Handle(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Conducteur trop jeune pour ce véhicule", result.RejectionReason);
    }

    // AC-03 — Conducteur de 16 ans avec trottinette électrique est déclaré éligible (règle inchangée)
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
}
