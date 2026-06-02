# Diagrams — STORY-69: Offre de Souscription

**Story:** STORY-69-A, STORY-69-B
**Date:** 2026-06-02

---

## SubscriptionContext — Component Diagram

```mermaid
graph TD
    subgraph SubscriptionContext
        SR["SubscriptionRequest\n(Aggregate Root)"]
        SRef["SubscriptionReference\n(Value Object)"]
        DId["DriverId\n(Value Object)"]
        VD["VehicleDetails\n(Value Object)"]
        PP["PricingPolicy\n(Domain Service)"]
        PQ["PriceQuote\n(Value Object)"]
        SRI["SubscriptionRequestInitiated\n(Domain Event)"]
        SRR["SubscriptionRequestRejected\n(Domain Event)"]
        ISRRepo["ISubscriptionRequestRepository\n(Repository)"]

        SR --> SRef
        SR --> DId
        SR --> VD
        SR -->|emits| SRI
        SR -->|emits| SRR
        PP -->|computes| PQ
        PP --> VD
        ISRRepo -->|persists/retrieves| SR
    end

    subgraph ReadModels
        SRC["SubscriptionRequestConfirmation\n(Read Model)"]
        IPQ["InsurancePriceQuote\n(Read Model)"]
        SRRD["SubscriptionRequestRejectionDetail\n(Read Model)"]
        PQRD["PriceQuoteRejectionDetail\n(Read Model)"]
    end

    SR -->|projects| SRC
    PP -->|projects| IPQ
    SRR -->|projects| SRRD
    ISRRepo -->|not found →| PQRD
```

---

## Classification table (Phase 9 input)

| Concept | Classification (source: ADR) | Story impact |
|---|---|---|
| `SubscriptionRequest` | `Aggregate Root` (adr-001) | added |
| `SubscriptionReference` | `Value Object` (adr-001) | added |
| `DriverId` | `Value Object` (adr-001) | added |
| `VehicleDetails` | `Value Object` (adr-001) | added |
| `PricingPolicy` | `Domain Service` (adr-003) | added |
| `PriceQuote` | `Value Object` (adr-003) | added |
| `SubscriptionRequestInitiated` | `Domain Event` (adr-001) | added |
| `SubscriptionRequestRejected` | `Domain Event` (adr-001) | added |
| `ISubscriptionRequestRepository` | `Repository` (adr-001) | added |
| `SubscriptionRequestConfirmation` | `Read Model` (adr-001) | added |
| `InsurancePriceQuote` | `Read Model` (adr-003) | added |
| `SubscriptionRequestRejectionDetail` | `Read Model` (adr-001) | added |
| `PriceQuoteRejectionDetail` | `Read Model` (adr-003) | added |
| `InitiateSubscriptionRequest` | `Command` (adr-001) | added |
| `GetInsurancePriceQuote` | `Query` (adr-003) | added |

---

## Vocabulary cross-check

Every `{classification}` on a node label MUST equal the classification recorded in its source ADR row above. Phase 9 enforces this via grep; any divergence triggers back-propagation or HALT.
