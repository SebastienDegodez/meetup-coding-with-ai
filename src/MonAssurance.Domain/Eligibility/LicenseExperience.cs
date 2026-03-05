namespace MonAssurance.Domain.Eligibility;

public sealed class LicenseExperience
{
    private readonly int _years;

    public LicenseExperience(int years) => _years = years;

    public bool HasAtLeastYears(int minimumYears) => _years >= minimumYears;
}
