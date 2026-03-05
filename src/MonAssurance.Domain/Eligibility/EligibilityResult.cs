namespace MonAssurance.Domain.Eligibility;

public sealed class EligibilityResult
{
    public bool IsEligible { get; init; }
    public string? RejectionReason { get; init; }

    public static EligibilityResult Accepted() => new() { IsEligible = true };
    public static EligibilityResult Rejected(string reason) => new() { IsEligible = false, RejectionReason = reason };
}
