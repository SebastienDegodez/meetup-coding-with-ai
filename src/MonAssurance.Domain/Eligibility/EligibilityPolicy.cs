namespace MonAssurance.Domain.Eligibility;

public sealed class EligibilityPolicy
{
    // French insurance regulation: high-power motorcycles require minimum 5 years of license
    private const int MinimumExperienceYearsForHighPower = 5;

    public EligibilityResult Evaluate(Driver driver, Vehicle vehicle, DateOnly today)
    {
        if (driver.Age(today) < vehicle.MinimumAge())
            return EligibilityResult.Refused("Conducteur trop jeune pour ce véhicule");

        if (vehicle.IsHighPowerMotorcycle() && !driver.HasEnoughExperience(MinimumExperienceYearsForHighPower))
            return EligibilityResult.Refused("Expérience insuffisante pour la puissance");

        return EligibilityResult.Accepted();
    }
}
