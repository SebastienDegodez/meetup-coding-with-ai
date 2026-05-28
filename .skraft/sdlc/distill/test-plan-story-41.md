# Test Plan — STORY-41: Update minimum driving age to 21

**Story:** STORY-41 — Update minimum driving age to 21  
**Milestone:** v0.3-legal-minimum-age  
**Feature file:** `.skraft/sdlc/distill/eligibility-minimum-age.feature`  
**Use case boundary:** `CheckEligibilityQueryHandler` (Application layer entry point)  
**Walking Skeleton Strategy:** A — Full InMemory (feature is purely internal; no external services)

---

## Coverage Matrix

| Scenario | AC | Use Case Boundary | Layer | Double Type | Walking Skeleton | Priority |
|---|---|---|---|---|---|---|
| Driver aged 21 applying for a car is accepted | AC-02 | `CheckEligibilityQueryHandler` | Application | InMemory (stateless query) | A | P1 |
| Driver aged 16 applying for an electric scooter is accepted | AC-04 | `CheckEligibilityQueryHandler` | Application | InMemory | A | P1 |
| Driver aged 20 applying for a car is refused | AC-01 | `CheckEligibilityQueryHandler` | Application | InMemory | A | P1 |
| Driver aged 20 applying for a motorcycle is refused | AC-03 | `CheckEligibilityQueryHandler` | Application | InMemory | A | P2 |
| Driver aged 18 applying for a car is refused | AC-05 | `CheckEligibilityQueryHandler` | Application | InMemory | A | P2 |
| `Vehicle.MinimumAge()` returns 21 for Car | — | `Vehicle.MinimumAge()` (domain pure function) | Domain | None (pure function) | — | P1 |
| `Vehicle.MinimumAge()` returns 21 for Motorcycle | — | `Vehicle.MinimumAge()` (domain pure function) | Domain | None (pure function) | — | P1 |
| `Vehicle.MinimumAge()` returns 16 for ElectricScooter | — | `Vehicle.MinimumAge()` (domain pure function) | Domain | None (pure function) | — | P1 |

---

## AC → Scenario Traceability

| AC | Scenario | Status |
|---|---|---|
| AC-01 — Driver aged 20, Car → refused | Driver aged 20 applying for a car is refused | ✅ Covered |
| AC-02 — Driver aged 21, Car → accepted | Driver aged 21 applying for a car is accepted | ✅ Covered |
| AC-03 — Driver aged 20, Motorcycle → refused | Driver aged 20 applying for a motorcycle is refused | ✅ Covered |
| AC-04 — Driver aged 16, ElectricScooter → accepted | Driver aged 16 applying for an electric scooter is accepted | ✅ Covered |
| AC-05 — Driver aged 18, Car → refused | Driver aged 18 applying for a car is refused | ✅ Covered |

---

## Mandate Checklist

| Mandate | Status | Notes |
|---|---|---|
| M1 — Layer Boundary Enforcement | ✅ | All Application tests enter through `CheckEligibilityQueryHandler.Handle()`; domain pure-function tests enter through `Vehicle.MinimumAge()` |
| M2 — Business Language Abstraction | ✅ | Gherkin uses zero technical terms; step methods will delegate to builder helpers |
| M3 — User Journey Completeness | ✅ | Every scenario has Given (state), When (trigger), Then (observable business result) |
| M4 — Pure Function Extraction | ✅ | `Vehicle.MinimumAge()` is a pure function; it will be tested directly at the Domain layer before the Application acceptance test |

---

## Scenarios Not Covered (Out of Scope for STORY-41)

- High-power motorcycle + insufficient experience: independent rule, unaffected by this story
- Infrastructure / persistence: eligibility check is stateless — no repository involved; no infrastructure test needed
