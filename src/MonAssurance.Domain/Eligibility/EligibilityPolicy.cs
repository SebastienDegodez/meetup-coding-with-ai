namespace MonAssurance.Domain.Eligibility;

public sealed class EligibilityPolicy
{
    private const int MINIMUM_AGE_FOR_CAR_OR_MOTORCYCLE = 18;
    private const int MINIMUM_AGE_FOR_ELECTRIC_SCOOTER = 16;
    private const int MINIMUM_LICENSE_YEARS_FOR_POWERFUL_MOTORCYCLE = 5;
    private const int POWERFUL_MOTORCYCLE_HORSEPOWER_THRESHOLD = 100;

    public EligibilityResult Evaluate(DriverInfo driver, VehicleInfo vehicle)
    {
        if (vehicle.Type is VehicleType.Car or VehicleType.Motorcycle
            && driver.Age < MINIMUM_AGE_FOR_CAR_OR_MOTORCYCLE)
            return EligibilityResult.Rejected(EligibilityRejectionReasons.DRIVER_TOO_YOUNG_FOR_VEHICLE);

        if (vehicle.Type == VehicleType.ElectricScooter
            && driver.Age < MINIMUM_AGE_FOR_ELECTRIC_SCOOTER)
            return EligibilityResult.Rejected(EligibilityRejectionReasons.DRIVER_TOO_YOUNG_FOR_VEHICLE);

        if (vehicle.Type == VehicleType.Motorcycle
            && vehicle.Horsepower > POWERFUL_MOTORCYCLE_HORSEPOWER_THRESHOLD
            && driver.LicenseYears < MINIMUM_LICENSE_YEARS_FOR_POWERFUL_MOTORCYCLE)
            return EligibilityResult.Rejected(EligibilityRejectionReasons.INSUFFICIENT_EXPERIENCE_FOR_POWER);

        return EligibilityResult.Accepted();
    }
}
