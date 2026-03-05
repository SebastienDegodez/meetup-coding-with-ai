namespace MonAssurance.Domain.Eligibility;

public sealed class Horsepower
{
    private readonly int _value;

    public Horsepower(int value) => _value = value;

    public bool Exceeds(int threshold) => _value > threshold;
}
