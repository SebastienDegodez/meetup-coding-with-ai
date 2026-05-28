# AC Draft — STORY-41: Update minimum driving age to 21

## Story

**ID:** STORY-41  
**GitHub Issue:** #41  
**Milestone:** v0.3-legal-minimum-age  
**Effort:** M (1 day)  
**MoSCoW:** Must Have

### User Story

As a driver applying for auto insurance,  
I want to know whether my age meets the legal minimum for the vehicle I want to insure,  
so that I receive an accurate eligibility decision that reflects the current law.

[PERSONA INFERRED] — issue does not name a persona; closest domain role is "driver applying for auto insurance".

---

## Domain Examples

1. **Lucas Bernard, 20 years old**, B licence (2 years), car (Peugeot 208, 90 hp) → **refused** — below the new minimum age of 21 for cars.
2. **Camille Leroy, 21 years old**, B licence (3 years), car (Renault Clio, 75 hp) → **accepted** — exactly meets the minimum age of 21.
3. **Théo Dumont, 20 years old**, A licence (1 year), motorcycle (Kawasaki Z400, 45 hp) → **refused** — below 21, regardless of licence type.
4. **Manon Petit, 16 years old**, AM licence (1 year), electric scooter → **accepted** — electric scooter minimum age remains 16.
5. **Antoine Garnier, 18 years old**, B licence (0 years), car (Citroën C3, 82 hp) → **refused** — 18 is below the new minimum of 21 for cars.

---

## Acceptance Criteria

### AC-01 — Driver aged 20 applying for a car is refused

```gherkin
Given a driver aged 20 with a valid B licence
And a vehicle of type Car
When the driver requests an eligibility check
Then the eligibility is refused
And the rejection reason is "Conducteur trop jeune pour ce véhicule"
```

### AC-02 — Driver aged 21 applying for a car is accepted

```gherkin
Given a driver aged 21 with a valid B licence
And a vehicle of type Car
When the driver requests an eligibility check
Then the eligibility is accepted
```

### AC-03 — Driver aged 20 applying for a motorcycle is refused

```gherkin
Given a driver aged 20 with a valid A licence (1 year)
And a vehicle of type Motorcycle with power ≤ 100 hp
When the driver requests an eligibility check
Then the eligibility is refused
And the rejection reason is "Conducteur trop jeune pour ce véhicule"
```

### AC-04 — Driver aged 16 applying for an electric scooter is accepted (age rule unchanged)

```gherkin
Given a driver aged 16 with a valid AM licence
And a vehicle of type ElectricScooter
When the driver requests an eligibility check
Then the eligibility is accepted
```

### AC-05 — Driver aged 18 applying for a car is refused (boundary: previously eligible, now refused)

```gherkin
Given a driver aged 18 with a valid B licence
And a vehicle of type Car
When the driver requests an eligibility check
Then the eligibility is refused
And the rejection reason is "Conducteur trop jeune pour ce véhicule"
```

---

## Technical Notes

- The change is localized to `Vehicle.MinimumAge()` in `src/MonAssurance.Domain/Eligibility/Vehicle.cs`.
- Current implementation: `_type == VehicleType.ElectricScooter ? 16 : 18`
- Target implementation: minimum age for Car and Motorcycle becomes 21; ElectricScooter remains 16.
- All existing eligibility tests that rely on age 18 for Car/Motorcycle will break and must be updated.
- No API contract changes — the `EligibilityPolicy.Evaluate` signature is unchanged.
- The high-power motorcycle experience rule (5 years) is independent and unaffected.

---

## Definition of Ready Checklist

| Item | Status | Notes |
|---|---|---|
| 1. Problem statement | ✅ | Legislative change raising minimum driving age to 21 for cars and motorcycles |
| 2. Specific persona | ✅ | Driver applying for auto insurance [PERSONA INFERRED] |
| 3. 3+ domain examples with real values | ✅ | 5 examples with real ages, names, vehicle types, and outcomes |
| 4. UAT scenarios | ✅ | 5 Given/When/Then ACs covering key boundaries |
| 5. AC derived from UAT | ✅ | Each AC traces to a domain example |
| 6. Right-sized (1-3 days) | ✅ | M estimate — single method change with test updates |
| 7. Technical notes | ✅ | Affected file identified; contract unchanged |
| 8. Dependencies | ✅ | None — self-contained domain change |

---

## Antipattern Scan

- AP-DISCUSS-01 (Implement-X): ✅ No — story describes user outcome, not technical instruction
- AP-DISCUSS-02 (Generic Data): ✅ No — all examples use real ages and vehicle types
- AP-DISCUSS-03 (Technical AC): ✅ No — ACs are behaviour-based, not implementation-based
- AP-DISCUSS-04 (Giant Story): ✅ No — single-rule change, M-sized
- AP-DISCUSS-05 (No Examples): ✅ No — 5 concrete domain examples
- AP-DISCUSS-06 (Tests After Code): ✅ No — ACs are written before implementation
- AP-DISCUSS-07 (Vague Persona): ✅ No — "driver applying for auto insurance" is specific
- AP-DISCUSS-08 (Missing Dependencies): ✅ No — explicitly stated as None

---

## INVEST Check

| Criterion | Status | Notes |
|---|---|---|
| Independent | ✅ | No dependency on other stories |
| Negotiable | ✅ | ACs describe behaviour, not implementation |
| Valuable | ✅ | Legal compliance — eligibility must reflect current law |
| Estimable | ✅ | Change is localised; complexity is low |
| Small | ✅ | 1 day estimate |
| Testable | ✅ | 5 concrete ACs with real values |
