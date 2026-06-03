// tests/MonAssurance.UnitTests/Eligibility/Application/LegalAgeChangeAcceptanceTests.cs
// OUTER LOOP — RED acceptance tests for STORY-72 (âge légal 18 → 21)
// Values copied VERBATIM from ac-draft-story-72.md and eligibility-legal-age.feature
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

    // AC-02 — Conducteur de 21 ans avec voiture est déclaré éligible
    [Fact]
    public void Handle_WhenDriverIs21AndHasCar_ReturnsEligible()
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-21),
            VehicleType: VehicleType.Car,
            Power: null,
            LicenseYears: 3);

        var result = handler.Handle(query);

        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }

    // AC-02 — Conducteur de 21 ans exactement aujourd'hui, voiture → éligible (borne inférieure inclusive)
    [Fact]
    public void Handle_WhenDriverTurns21ExactlyToday_AndHasCar_ReturnsEligible()
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-21),
            VehicleType: VehicleType.Car,
            Power: null,
            LicenseYears: 3);

        var result = handler.Handle(query);

        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }

    // AC-01 — Conducteur de 20 ans avec voiture est refusé
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

    // AC-04 — Conducteur de 18 ans avec voiture est refusé (ancienne limite devenue invalide)
    [Fact]
    public void Handle_WhenDriverIs18AndHasCar_ReturnsRefused()
    {
        var handler = BuildHandler(Today);
        var query = new CheckEligibilityQuery(
            DateOfBirth: Today.AddYears(-18),
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
