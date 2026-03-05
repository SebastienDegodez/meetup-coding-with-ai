namespace MonAssurance.Domain.Eligibility;

public sealed class VehicleInfo
{
    private const int MINIMUM_AGE_FOR_CAR_OR_MOTORCYCLE = 18;
    private const int MINIMUM_AGE_FOR_ELECTRIC_SCOOTER = 16;
    private const int POWERFUL_MOTORCYCLE_HORSEPOWER_THRESHOLD = 100;
    private const int MINIMUM_LICENSE_YEARS_FOR_POWERFUL_MOTORCYCLE = 5;

    private readonly VehicleType _type;
    private readonly Horsepower? _horsepower;

    public VehicleInfo(VehicleType type, Horsepower? horsepower = null)
    {
        _type = type;
        _horsepower = horsepower;
    }

    public bool AcceptsDriverAge(DriverAge age)
    {
        if (_type is VehicleType.Car or VehicleType.Motorcycle)
            return age.IsAtLeast(MINIMUM_AGE_FOR_CAR_OR_MOTORCYCLE);

        if (_type is VehicleType.ElectricScooter)
            return age.IsAtLeast(MINIMUM_AGE_FOR_ELECTRIC_SCOOTER);

        return true;
    }

    public bool AcceptsLicenseExperience(LicenseExperience experience)
    {
        if (!IsPowerfulMotorcycle())
            return true;

        return experience.HasAtLeastYears(MINIMUM_LICENSE_YEARS_FOR_POWERFUL_MOTORCYCLE);
    }

    private bool IsPowerfulMotorcycle()
        => _type == VehicleType.Motorcycle
        && _horsepower is not null
        && _horsepower.Exceeds(POWERFUL_MOTORCYCLE_HORSEPOWER_THRESHOLD);
}
