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
        var driver = new DriverInfo { Age = query.DriverAge, LicenseYears = query.DriverLicenseYears };
        var vehicle = new VehicleInfo { Type = query.VehicleType, Horsepower = query.VehicleHorsepower };

        var result = _policy.Evaluate(driver, vehicle);

        return Task.FromResult(new CheckEligibilityQueryResult
        {
            IsEligible = result.IsEligible,
            RejectionReason = result.RejectionReason
        });
    }
}
