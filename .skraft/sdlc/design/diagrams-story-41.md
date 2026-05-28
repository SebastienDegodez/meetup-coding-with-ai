# Diagrams — STORY-41: Update minimum driving age to 21

**Story:** STORY-41  
**Milestone:** v0.3-legal-minimum-age  
**Date:** 2026-05-26

---

## EligibilityContext — Component Diagram

```mermaid
graph TD
    subgraph EligibilityContext_Domain["Domain Layer"]
        EligibilityPolicy["EligibilityPolicy\n(Domain Service)"]
        Driver["Driver\n(Value Object)"]
        Vehicle["Vehicle\n(Value Object)"]
        VehicleType["VehicleType\n(Enum: Car | Motorcycle | ElectricScooter)"]
        EligibilityResult["EligibilityResult\n(Value Object)"]
        RejectionReason["RejectionReason\n(string constant)"]
    end

    subgraph EligibilityContext_Application["Application Layer"]
        CheckEligibilityQuery["CheckEligibilityQuery\n(Query)"]
        CheckEligibilityQueryHandler["CheckEligibilityQueryHandler\n(Query Handler)"]
        EligibilityViewModel["EligibilityViewModel\n(Read Model)"]
        IQueryHandler["IQueryHandler&lt;TQuery, TResult&gt;\n(Interface)"]
        IQueryBus["IQueryBus\n(Interface)"]
    end

    subgraph EligibilityContext_Infrastructure["Infrastructure Layer"]
        QueryBus["QueryBus\n(Implementation of IQueryBus)"]
        DI["DependencyInjection\n(Service Registration)"]
    end

    subgraph EligibilityContext_Api["API Layer"]
        EligibilityEndpoints["EligibilityEndpoints\n(Minimal API)"]
    end

    EligibilityPolicy --> Driver
    EligibilityPolicy --> Vehicle
    EligibilityPolicy --> EligibilityResult
    Vehicle --> VehicleType
    EligibilityResult --> RejectionReason

    CheckEligibilityQueryHandler --> EligibilityPolicy
    CheckEligibilityQueryHandler --> Driver
    CheckEligibilityQueryHandler --> Vehicle
    CheckEligibilityQueryHandler --> EligibilityViewModel
    CheckEligibilityQueryHandler --> CheckEligibilityQuery
    CheckEligibilityQueryHandler -.->|implements| IQueryHandler

    QueryBus -.->|implements| IQueryBus
    QueryBus --> CheckEligibilityQueryHandler

    EligibilityEndpoints --> IQueryBus
    EligibilityEndpoints --> CheckEligibilityQuery
```

---

## STORY-41 Change Localisation

The rule change is **entirely contained** within `Vehicle.MinimumAge()` in the
Domain layer. No application, infrastructure, or API layer components change.

```mermaid
graph TD
    Vehicle["Vehicle.MinimumAge()"]:::changed
    EligibilityPolicy["EligibilityPolicy.Evaluate()"]:::unchanged
    Driver["Driver.Age()"]:::unchanged
    classDef changed fill:#ff9,stroke:#f90,stroke-width:2px
    classDef unchanged fill:#f0f0f0,stroke:#aaa

    EligibilityPolicy --> Vehicle
    EligibilityPolicy --> Driver
```

**Legend:**
- 🟡 Yellow — modified by STORY-41
- ⬜ Grey — unchanged

---

## Aggregate / Value Object / Domain Service Classification

| Type | Name | Role | STORY-41 Impact |
|---|---|---|---|
| Domain Service | `EligibilityPolicy` | Orchestrates eligibility rules; has no mutable state | None — logic delegates to `Vehicle.MinimumAge()` |
| Value Object | `Driver` | Holds date of birth and license years; equality by value composition, no lifecycle identity in EligibilityContext | None |
| Value Object | `Vehicle` | Holds vehicle type and engine power; **owns `MinimumAge()` rule** | **Modified** — returns 21 for Car/Motorcycle |
| Value Object | `EligibilityResult` | Wraps `Eligible`/`Ineligible` + optional rejection reason | None |
| Enum | `VehicleType` | `Car`, `Motorcycle`, `ElectricScooter` | None |
