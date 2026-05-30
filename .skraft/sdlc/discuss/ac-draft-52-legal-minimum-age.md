# AC Draft — Story #52: Legal Minimum Driver Age (21 years)

## Story Statement

As a driver applying for vehicle insurance in France,
I want the eligibility system to enforce the new legal minimum driving age of 21,
so that insurance quotes are only issued to drivers who meet the current French regulatory requirements.

`[PERSONA INFERRED]` — issue body does not name a persona; closest domain role is "driver applying for insurance".

---

## Context

A new French regulation sets the minimum driving age to **21 years** for standard vehicles (car and motorcycle). The previous minimum was 18. The electric scooter minimum age (16) is governed by a separate rule (see issue #45) and is **not changed** by this story.

Current implementation: `Vehicle.MinimumAge()` returns `18` for `Car` and `Motorcycle`, `16` for `ElectricScooter`.

---

## Domain Examples

1. **Lucas Bernard, age 20**, valid B licence (2 years), applies with a Renault Clio (Car) → **not eligible** — below the new legal minimum age of 21.
2. **Camille Lefebvre, age 21**, valid B licence (3 years), applies with a Peugeot 208 (Car) → **eligible** — meets the minimum age requirement.
3. **Antoine Moreau, age 20**, valid A licence (1 year), applies with a Yamaha MT-07 125hp (Motorcycle) → **not eligible** — below 21 AND insufficient experience.
4. **Élodie Rousseau, age 18**, valid B licence (0 years), applies with a Car → **not eligible** — previously eligible, now excluded by the new law.
5. **Théo Garnier, age 16**, applies with an Electric Scooter → **eligible** — scooter minimum age rule (16) is unaffected by this story.

---

## Acceptance Criteria

### AC-01 — Driver under 21 is refused for a standard vehicle (Car)

```
Given a driver aged 20 with a valid B licence
When the driver requests an eligibility check for a Car
Then the driver is declared not eligible
And the rejection reason is "Conducteur trop jeune pour ce véhicule"
```

_Derived from domain example 1 (Lucas Bernard, age 20, Car)._

---

### AC-02 — Driver aged exactly 21 is eligible for a standard vehicle (Car)

```
Given a driver aged 21 with a valid B licence
When the driver requests an eligibility check for a Car
Then the driver is declared eligible
```

_Derived from domain example 2 (Camille Lefebvre, age 21, Car)._

---

### AC-03 — Driver under 21 is refused for a standard vehicle (Motorcycle)

```
Given a driver aged 20 with a valid A licence
When the driver requests an eligibility check for a Motorcycle with 50hp
Then the driver is declared not eligible
And the rejection reason is "Conducteur trop jeune pour ce véhicule"
```

_Derived from domain example 3 (Antoine Moreau, age 20, Motorcycle)._

---

### AC-04 — Driver previously at minimum age (18) is now refused

```
Given a driver aged 18 with a valid B licence
When the driver requests an eligibility check for a Car
Then the driver is declared not eligible
And the rejection reason is "Conducteur trop jeune pour ce véhicule"
```

_Derived from domain example 4 (Élodie Rousseau, age 18) — regression guard for the old 18-year rule._

---

### AC-05 — Electric scooter minimum age rule is unaffected

```
Given a driver aged 16
When the driver requests an eligibility check for an Electric Scooter
Then the driver is declared eligible
```

_Derived from domain example 5 (Théo Garnier, age 16) — ensures the scooter rule is not accidentally changed._

---

## Technical Notes

- **Target file**: `src/MonAssurance.Domain/Eligibility/Vehicle.cs`
- **Change**: In `MinimumAge()`, the default return value for non-scooter vehicles must change from `18` to `21`.
- **Current code**: `public int MinimumAge() => _type == VehicleType.ElectricScooter ? 16 : 18;`
- **Expected change**: `public int MinimumAge() => _type == VehicleType.ElectricScooter ? 16 : 21;`
- No new domain concepts or new files are needed.
- Existing tests covering the 18-year boundary must be updated to reflect the new 21-year boundary.
- The high-power motorcycle experience rule (`HasEnoughExperience(5)`) is independent and unaffected.

---

## Effort Estimate

**Size: S** (0.5 story-days)

- 1 line change in production code
- 5 ACs, but low complexity — all ACs test the same boundary at different ages
- Pattern already exists in codebase (extend, don't invent) → -1 size adjustment applies
- No integration or external service dependency

---

## DoR Checklist

| # | Item | Status | Notes |
|---|---|---|---|
| 1 | Problem statement | ✅ | Concrete regulatory non-compliance: age threshold is wrong |
| 2 | Specific persona | ✅ | "Driver applying for vehicle insurance in France" (inferred) |
| 3 | 3+ domain examples | ✅ | 5 domain examples with real names, real ages, real vehicles |
| 4 | UAT scenarios | ✅ | 5 Given/When/Then ACs |
| 5 | AC derived from UAT | ✅ | Each AC cites its domain example |
| 6 | Right-sized | ✅ | S (0.5 days) |
| 7 | Technical notes | ✅ | Target file, line, before/after code snippet included |
| 8 | Dependencies | ✅ | None — standalone domain rule change |

**DoR Status: ✅ READY**

---

## Dependencies

None. This story is a standalone domain rule change in `Vehicle.cs`.

---

_Generated: 2026-05-30_
_Issue: #52_
_Milestone: v0.3-legal-age-update_
