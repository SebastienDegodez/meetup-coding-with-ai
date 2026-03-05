namespace MonAssurance.Application.Eligibility.Queries.CheckEligibility;

public sealed class CheckEligibilityQueryResult
{
    public bool IsEligible { get; init; }
    public string? RejectionReason { get; init; }
}
