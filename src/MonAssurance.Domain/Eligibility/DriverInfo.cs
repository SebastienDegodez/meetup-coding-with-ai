namespace MonAssurance.Domain.Eligibility;

public sealed class DriverInfo
{
    private readonly DriverAge _age;
    private readonly LicenseExperience _licenseExperience;

    public DriverInfo(DriverAge age, LicenseExperience licenseExperience)
    {
        _age = age;
        _licenseExperience = licenseExperience;
    }

    public bool IsTooYoungFor(VehicleInfo vehicle) => !vehicle.AcceptsDriverAge(_age);

    public bool HasInsufficientExperienceFor(VehicleInfo vehicle) => !vehicle.AcceptsLicenseExperience(_licenseExperience);
}
