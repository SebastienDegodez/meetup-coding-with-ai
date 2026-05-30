namespace MonAssurance.Domain.Eligibility;

public sealed class EligibilityPolicy
{
    // Minimum years of driving experience required for high-power motorcycles per French regulation
    private const int HighPowerMotorcycleMinimumExperienceYears = 5;

    public EligibilityResult Evaluate(Driver driver, Vehicle vehicle, DateOnly today)
    {
        if (driver.Age(today) < vehicle.MinimumAge())
            return EligibilityResult.Refused("Conducteur trop jeune pour ce véhicule");

        if (vehicle.IsHighPowerMotorcycle() && !driver.HasEnoughExperience(HighPowerMotorcycleMinimumExperienceYears))
            return EligibilityResult.Refused("Expérience insuffisante pour la puissance");

        return EligibilityResult.Accepted();
    }
}
