namespace MonAssurance.Domain.Eligibility;

public sealed class Vehicle
{
    // Legal minimum ages (French road code — art. R221-1 as amended 2026)
    private const int MinimumDrivingAge = 21;
    private const int MinimumElectricScooterAge = 16;

    // High-power threshold: French insurance regulation (> 100 ch = permis A obligatoire + expérience)
    private const int HighPowerThresholdHp = 100;

    private readonly VehicleType _type;
    private readonly int? _power;

    public Vehicle(VehicleType type, int? power)
    {
        _type = type;
        _power = power;
    }

    public int MinimumAge() => _type == VehicleType.ElectricScooter ? MinimumElectricScooterAge : MinimumDrivingAge;

    // Convention: null power treated as ≤ 100 hp — no experience rule triggered.
    public bool IsHighPowerMotorcycle() => _type == VehicleType.Motorcycle && _power > HighPowerThresholdHp;
}
