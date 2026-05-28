# ADR-004: Minimum Age Rule Change — 18 → 21 for Car and Motorcycle

**Status:** Accepted  
**Date:** 2026-05-26

## Context

A legislative change raises the minimum legal driving age for car and motorcycle
insurance from 18 to 21. The electric scooter minimum age (16) is unchanged.

The current implementation of `Vehicle.MinimumAge()` returns:

```csharp
_type == VehicleType.ElectricScooter ? 16 : 18
```

This expression must be updated. The decision is where and how to encode the new
rule — specifically whether to:

1. Modify the existing `Vehicle.MinimumAge()` method, or
2. Introduce a new concept (e.g., a policy configuration object, a rules engine,
   or an external configuration value).

The minimum age rule is a **legislative invariant** — it does not vary by user,
by product, or by underwriting configuration. It is a hard legal floor that applies
uniformly across the product.

## Decision

We will update `Vehicle.MinimumAge()` in `MonAssurance.Domain/Eligibility/Vehicle.cs`
to return 21 for `Car` and `Motorcycle`, and retain 16 for `ElectricScooter`. The
rule remains a hard-coded domain invariant because it is a statutory minimum — not
configurable per product or per user.

```csharp
public int MinimumAge() => _type switch
{
    VehicleType.ElectricScooter => 16,
    _                           => 21,
};
```

All existing tests that assert acceptance for drivers aged 18–20 applying for a Car
or Motorcycle must be updated to assert refusal with rejection reason
`"Conducteur trop jeune pour ce véhicule"`.

## Consequences

**Positive:**
- Change is localised to a single method in the Domain layer — minimal risk surface
- `EligibilityPolicy.Evaluate()` requires no change — the rule delegation is already correct
- API contract, application layer, and infrastructure layer are unaffected
- The new rule is enforced consistently for both Car and Motorcycle in one expression

**Negative / trade-offs:**
- Hard-coded value means a future legislative change requires a code deployment
- Tests relying on age 18 as an acceptance boundary will fail and must be updated — this is intentional but requires developer awareness
- No runtime configurability — if the product team needs to run a staged rollout, a feature-flag mechanism would be needed (not in scope)

**Neutral:**
- The switch expression replaces the ternary — marginally more readable when future vehicle types are added

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| External configuration / appsettings | Legislative minimums are not operator-configurable; externalising them would imply they can be overridden per deployment, which is incorrect |
| Introducing a `MinimumAgePolicy` configuration object | Premature abstraction — no story requires per-product or per-vehicle-subtype configurability; YAGNI applies |
| Placing the rule in `EligibilityPolicy` instead of `Vehicle` | The minimum age is a property of the vehicle (its legal restriction), not a policy-level calculation; ownership belongs on `Vehicle` |
