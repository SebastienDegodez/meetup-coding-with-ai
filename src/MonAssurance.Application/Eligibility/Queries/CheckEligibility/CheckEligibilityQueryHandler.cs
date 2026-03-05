using MonAssurance.Application.Shared;
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.Application.Eligibility.Queries.CheckEligibility;

public sealed class CheckEligibilityQueryHandler : IQueryHandler<CheckEligibilityQuery, CheckEligibilityQueryResult>
{
    private readonly EligibilityPolicy _policy = new();

    public Task<CheckEligibilityQueryResult> HandleAsync(
        CheckEligibilityQuery query,
        CancellationToken cancellationToken = default)
    {
        var driver = new DriverInfo(new DriverAge(query.DriverAge), new LicenseExperience(query.DriverLicenseYears));
        var horsepower = query.VehicleHorsepower.HasValue ? new Horsepower(query.VehicleHorsepower.Value) : null;
        var vehicle = new VehicleInfo(query.VehicleType, horsepower);

        var result = _policy.Evaluate(driver, vehicle);

        return Task.FromResult(new CheckEligibilityQueryResult
        {
            IsEligible = result.IsAccepted(),
            RejectionReason = result.RejectionReason()
        });
    }
}
