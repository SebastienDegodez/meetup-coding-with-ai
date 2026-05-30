# Interface Contracts — story-52: Legal Minimum Driver Age (21 years)

**Story:** story-52
**Date:** 2026-05-30

## Eligibility

### Query: `CheckEligibility`

| Field | Type | Required | Notes |
|---|---|---|---|
| `dateOfBirth` | `DateOnly` | yes | Driver's date of birth; used to compute age at evaluation time |
| `vehicleType` | `VehicleType` | yes | Enum: `Car`, `Motorcycle`, `ElectricScooter` |
| `power` | `int?` | no | Engine power in hp; `null` treated as < 100 hp (no high-power rule triggered) |
| `licenseYears` | `int` | yes | Years since licence was issued; non-negative |

### Read Model: `EligibilityResult`

| Field | Type | Required | Notes |
|---|---|---|---|
| `isEligible` | `bool` | yes | `true` if the driver passes all eligibility rules |
| `rejectionReason` | `string?` | no | Populated only when `isEligible = false`; one of the domain rejection reasons |

**Domain rejection reasons (enforced by `EligibilityPolicy`):**

| Rejection reason | French (domain) | Trigger condition |
|---|---|---|
| Too young for vehicle | `"Conducteur trop jeune pour ce véhicule"` | `driver.Age(today) < vehicle.MinimumAge()` |
| Insufficient experience | `"Expérience insuffisante pour la puissance"` | `vehicle.IsHighPowerMotorcycle() && licenseYears < 5` |

### Interface: `IQueryHandler<CheckEligibilityQuery, EligibilityViewModel>`

```
Handle(query: CheckEligibilityQuery) → EligibilityViewModel
```

- Implemented by `CheckEligibilityQueryHandler` (Application layer)
- Registered in DI via `MonAssurance.Infrastructure.DependencyInjection`
- No repository dependency — all evaluation is in-memory

## Vocabulary cross-check (Phase 9 input)

Every `{contract-category}` heading matches the classification ratified in the corresponding ADR:
- `Query: CheckEligibility` → ratified as Query by ADR-001
- `Read Model: EligibilityResult` → ratified as Value Object / Read Model by ADR-001 and ADR-002
- `Interface: IQueryHandler<...>` → Application interface, consistent with ADR-001 (query-only pattern)
