using MonAssurance.Domain.Eligibility;

namespace MonAssurance.Application.Eligibility.Queries.CheckEligibility;

public sealed class CheckEligibilityQuery
{
    public int DriverAge { get; init; }
    public int DriverLicenseYears { get; init; }
    public VehicleType VehicleType { get; init; }
    public int? VehicleHorsepower { get; init; }
}
