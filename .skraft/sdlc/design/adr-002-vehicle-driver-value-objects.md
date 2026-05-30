# ADR-002: Vehicle and Driver as Value Objects; EligibilityPolicy as Domain Service

**Status:** Accepted
**Date:** 2026-05-30

## Context

Story #52 changes the minimum driving age enforced inside `Vehicle.MinimumAge()`. This raises the question of where in the DDD tactical pattern space `Vehicle`, `Driver`, and `EligibilityPolicy` belong.

**Vehicle:** A vehicle in this domain is described entirely by its type (`VehicleType`) and optional engine power (`int?`). Two vehicles of the same type and power are interchangeable — there is no "vehicle #1234 is different from vehicle #1235 of the same make and power". There is no lifecycle, no identity, and no mutable state. The `MinimumAge()` and `IsHighPowerMotorcycle()` methods are derived properties of its type+power tuple.

**Driver:** Similarly, a driver is described by date of birth and licence years. `Age(today)` and `HasEnoughExperience(n)` are pure derivations from those fields. No identity or lifecycle is tracked.

**EligibilityPolicy:** The policy coordinates `Driver` and `Vehicle` to produce an `EligibilityResult`. It owns no state and has no identity. It is not an Aggregate (it does not protect invariants over a cluster of entities). It is a stateless evaluator — the textbook Domain Service.

The minimum age rule (`MinimumAge()` returning 21 for Car/Motorcycle, 16 for ElectricScooter) is self-contained inside `Vehicle` — consistent with its role as a self-validating Value Object.

## Decision

We will classify `Vehicle` and `Driver` as **Value Objects**, and `EligibilityPolicy` as a **Domain Service**. No Aggregate Root is required for this story's scope.

## Consequences

**Positive:**
- Simple instantiation: `new Vehicle(type, power)` and `new Driver(dob, licYears)` — no repository, no identity management.
- Immutability enforced by design: Value Objects have no setters; rules are re-evaluated each call.
- Minimal age rule change (`18 → 21` in `Vehicle.MinimumAge()`) fits naturally within the Value Object boundary.

**Negative / trade-offs:**
- If a future story introduces the concept of "a named insured vehicle with a registration plate and history", `Vehicle` may need to be promoted to an Entity or Aggregate Root — requiring an ADR supersession.
- Value Objects hold no identity, so cross-request caching of `Vehicle` instances provides no correctness guarantee (though performance is unaffected since they are trivially cheap to construct).

**Neutral:**
- `EligibilityResult` is also a Value Object (immutable, equality-by-value), used as the output of `EligibilityPolicy.Evaluate`. This is consistent with the established pattern.

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| `Vehicle` as Entity | An entity requires identity. No story requires distinguishing one Car from another Car of the same type and power. Adding an ID would be meaningless for current scope. |
| `Vehicle` as Aggregate Root | An aggregate root is justified when invariants must be protected across a cluster of child objects. `Vehicle` has no children and no transactional invariant beyond what `MinimumAge()` already expresses. |
| `EligibilityPolicy` as Aggregate Root | The policy holds no state and emits no domain events. Classifying it as an Aggregate would be a category error — it coordinates but does not own any entity or value object lifecycle. |
