# Interface Contracts — STORY-41: Update minimum driving age to 21

**Story:** STORY-41  
**Milestone:** v0.3-legal-minimum-age  
**Date:** 2026-05-26

---

## Query: CheckEligibility

```
CheckEligibilityQuery
- DateOfBirth:  DateOnly   (required) — driver's date of birth; used to compute age at check time
- LicenseYears: int        (required, ≥ 0) — number of years the driver has held their licence
- VehicleType:  VehicleType (required) — Car | Motorcycle | ElectricScooter
- Power:        int?       (optional, nullable) — engine power in hp; null treated as < 100 hp
```

**Validation rules (unchanged by STORY-41):**
- `DateOfBirth` must be in the past
- `LicenseYears` must be ≥ 0 and ≤ (current year − birth year)
- `Power` must be > 0 when provided

---

## Read Model: EligibilityViewModel

```
EligibilityViewModel
- IsEligible:      bool    — true when driver is accepted; false when refused
- RejectionReason: string? — nullable; present only when IsEligible is false
```

---

## Domain Service: EligibilityPolicy

```csharp
// Signature — unchanged by STORY-41
EligibilityResult Evaluate(Driver driver, Vehicle vehicle, DateOnly today)
```

**Evaluation order (unchanged):**
1. `driver.Age(today) < vehicle.MinimumAge()` → Refused("Conducteur trop jeune pour ce véhicule")
2. `vehicle.IsHighPowerMotorcycle() && !driver.HasEnoughExperience(5)` → Refused("Expérience insuffisante pour la puissance")
3. → Accepted()

---

## Value Object: Vehicle — MinimumAge() Rule (MODIFIED)

```
Vehicle.MinimumAge() : int

Current rule:   ElectricScooter → 16, else → 18
Target rule:    ElectricScooter → 16, else → 21

Invariant table:
  VehicleType.ElectricScooter  → minimum age = 16  (unchanged)
  VehicleType.Car              → minimum age = 21  (changed: was 18)
  VehicleType.Motorcycle       → minimum age = 21  (changed: was 18)
```

---

## Domain Events (Conceptual — no event store)

```
EligibilityChecked  (conceptual — not persisted as a domain event in current architecture)
- driverId:       derived from query context
- result:         Eligible | Ineligible
- checkedAt:      DateTimeOffset (UTC)
- reason:         RejectionReason? — null when Eligible; "Conducteur trop jeune pour ce véhicule"
                  when refused for age; "Expérience insuffisante pour la puissance" when refused
                  for experience
```

> **Note:** The current architecture uses a synchronous query pattern. `EligibilityChecked`
> is described here as a conceptual domain event for modelling purposes. If an audit
> trail or temporal query requirement is introduced in a future story, this event
> should be persisted (see ADR-003).

---

## Application Interface: IQueryHandler (unchanged)

```csharp
public interface IQueryHandler<TQuery, TResult>
{
    TResult Handle(TQuery query);
}
// Implemented by: CheckEligibilityQueryHandler
```

---

## API Contract (unchanged)

```
GET /eligibility?dateOfBirth={date}&licenseYears={int}&vehicleType={type}&power={int?}

Response 200 OK:
{
  "isEligible": bool,
  "rejectionReason": string | null
}
```

No API contract change is introduced by STORY-41.
