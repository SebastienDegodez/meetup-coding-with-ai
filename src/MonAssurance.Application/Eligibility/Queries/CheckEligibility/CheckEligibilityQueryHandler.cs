using MonAssurance.Application.Shared;

namespace MonAssurance.Application.Eligibility.Queries.CheckEligibility;

public sealed class CheckEligibilityQueryHandler : IQueryHandler<CheckEligibilityQuery, CheckEligibilityQueryResult>
{
    public Task<CheckEligibilityQueryResult> HandleAsync(
        CheckEligibilityQuery query,
        CancellationToken cancellationToken = default)
        => Task.FromResult(new CheckEligibilityQueryResult());
}
