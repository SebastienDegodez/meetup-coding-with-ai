namespace MonAssurance.Domain.Eligibility;

public sealed class DriverAge
{
    private readonly int _value;

    public DriverAge(int value) => _value = value;

    public bool IsAtLeast(int minimumAge) => _value >= minimumAge;
}
