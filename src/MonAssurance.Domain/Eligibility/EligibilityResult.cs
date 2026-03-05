namespace MonAssurance.Domain.Eligibility;

public sealed class EligibilityResult
{
    private readonly bool _accepted;
    private readonly string? _rejectionReason;

    private EligibilityResult(bool accepted, string? rejectionReason)
    {
        _accepted = accepted;
        _rejectionReason = rejectionReason;
    }

    public static EligibilityResult Accepted() => new(true, null);
    public static EligibilityResult Rejected(string reason) => new(false, reason);

    public bool IsAccepted() => _accepted;

    public string? RejectionReason() => _rejectionReason;
}
