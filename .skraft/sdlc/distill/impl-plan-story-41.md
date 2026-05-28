# Implementation Plan — STORY-41: Update minimum driving age to 21

**Story:** STORY-41 — Update minimum driving age to 21  
**Milestone:** v0.3-legal-minimum-age  
**Outside-in order:** Domain pure function → Application acceptance tests → Production domain change  

---

## Step 1 — Domain unit tests for `Vehicle.MinimumAge()` (RED)

**Test file:** `tests/MonAssurance.UnitTests/Domain/VehicleMinimumAgeTests.cs`  
**Enters through:** `Vehicle.MinimumAge()` — pure domain function  
**Double type:** None — call the function directly  

Write focused tests asserting:
- `Vehicle` of type `Car` → `MinimumAge()` returns `21`
- `Vehicle` of type `Motorcycle` → `MinimumAge()` returns `21`
- `Vehicle` of type `ElectricScooter` → `MinimumAge()` returns `16`

These tests will turn RED immediately because the current implementation returns `18` for Car and Motorcycle.

---

## Step 2 — Application acceptance tests for minimum age scenarios (RED)

**Test file:** `tests/MonAssurance.UnitTests/Application/CheckEligibilityMinimumAgeTests.cs`  
**Enters through:** `CheckEligibilityQueryHandler.Handle(CheckEligibilityQuery)`  
**Double type:** InMemory (no repository; query is stateless — handler instantiates domain objects directly)  

Write tests mapping to the Gherkin scenarios in `eligibility-minimum-age.feature`:

| Test method | AC | Expected outcome |
|---|---|---|
| `Driver_Aged20_WithCar_IsRefused` | AC-01 | `IsEligible = false`, `RejectionReason = "Conducteur trop jeune pour ce véhicule"` |
| `Driver_Aged21_WithCar_IsAccepted` | AC-02 | `IsEligible = true` |
| `Driver_Aged20_WithMotorcycle_IsRefused` | AC-03 | `IsEligible = false`, `RejectionReason = "Conducteur trop jeune pour ce véhicule"` |
| `Driver_Aged16_WithElectricScooter_IsAccepted` | AC-04 | `IsEligible = true` |
| `Driver_Aged18_WithCar_IsRefused` | AC-05 | `IsEligible = false`, `RejectionReason = "Conducteur trop jeune pour ce véhicule"` |

All tests will be RED because the current minimum age for Car/Motorcycle is 18.

> **Test factory helper:** Use or extend `CheckEligibilityQueryBuilder` (or equivalent object mother) in the test project to construct `CheckEligibilityQuery` instances from named domain parameters (age, vehicle type, licence years). Never build raw DTOs inline in test methods.

---

## Step 3 — Update existing tests that relied on age 18 as acceptance boundary (SYNTHESIZE)

**Files to update:** search `tests/MonAssurance.UnitTests/` for assertions that a driver aged 18 or 19 or 20 applying for a Car or Motorcycle is accepted — these are now incorrect.

For each such test:
- If the test's purpose was to verify the old threshold (18), update the expected outcome to `refused` with reason `"Conducteur trop jeune pour ce véhicule"`.
- If the test used age 18 as convenient input for a *different* rule (e.g., high-power motorcycle experience), update the age to ≥ 21 so the age rule does not mask the rule under test.

---

## Step 4 — Implement production change (GREEN)

**File:** `src/MonAssurance.Domain/Eligibility/Vehicle.cs`  
**Method:** `Vehicle.MinimumAge()`  

Replace:
```csharp
_type == VehicleType.ElectricScooter ? 16 : 18
```

With:
```csharp
public int MinimumAge() => _type switch
{
    VehicleType.ElectricScooter => 16,
    _                           => 21,
};
```

No other production file requires change (ADR-004 confirmed: API contract, application layer, and infrastructure layer are unaffected).

---

## Step 5 — Verify all tests GREEN

Run the full test suite:

```
dotnet test MonAssurance.sln
```

Expected: all tests pass. Failure pattern to watch for: any test asserting that a driver aged 18–20 with a Car or Motorcycle is **accepted** — these must be fixed in Step 3.

---

## Step 6 — Architecture test (no change expected)

The existing NetArchTest-based architecture tests in `tests/MonAssurance.IntegrationTests/` should continue to pass unchanged. Verify they still pass after the production change.

---

## Outside-in Order Summary

```
Step 1: Domain tests (pure function)          → RED
Step 2: Application acceptance tests          → RED
Step 4: Implement Vehicle.MinimumAge()        → GREEN (Steps 1 & 2)
Step 3: Fix broken pre-existing tests         → GREEN (full suite)
Step 5: Verify full suite GREEN               → DONE
Step 6: Architecture tests still pass         → DONE
```
