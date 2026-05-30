namespace MonAssurance.Domain.Eligibility;

public sealed class Vehicle
{
    // Legal minimum driving age per French law (2026 regulatory update)
    private const int MinimumStandardAge = 21;

    // Lower age threshold for low-power vehicles (electric scooters) per French regulation
    private const int MinimumElectricScooterAge = 16;

    // High-power motorcycle threshold: above this requires extended driving experience per regulations
    private const int HighPowerMotorcycleThresholdHp = 100;

    private readonly VehicleType _type;
    private readonly int? _power;

    public Vehicle(VehicleType type, int? power)
    {
        _type = type;
        _power = power;
    }

    public int MinimumAge() => _type == VehicleType.ElectricScooter ? MinimumElectricScooterAge : MinimumStandardAge;

    // Convention: null power treated as < HighPowerMotorcycleThresholdHp — no experience rule triggered.
    public bool IsHighPowerMotorcycle() => _type == VehicleType.Motorcycle && _power > HighPowerMotorcycleThresholdHp;
}
