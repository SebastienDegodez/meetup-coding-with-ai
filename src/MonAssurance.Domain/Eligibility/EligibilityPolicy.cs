namespace MonAssurance.Domain.Eligibility;

public sealed class EligibilityPolicy
{
    public EligibilityResult Evaluate(DriverInfo driver, VehicleInfo vehicle)
    {
        if (driver.IsTooYoungFor(vehicle))
            return EligibilityResult.Rejected(EligibilityRejectionReasons.DRIVER_TOO_YOUNG_FOR_VEHICLE);

        if (driver.HasInsufficientExperienceFor(vehicle))
            return EligibilityResult.Rejected(EligibilityRejectionReasons.INSUFFICIENT_EXPERIENCE_FOR_POWER);

        return EligibilityResult.Accepted();
    }
}
