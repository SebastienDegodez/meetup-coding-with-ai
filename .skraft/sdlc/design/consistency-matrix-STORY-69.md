# Consistency Matrix — STORY-69

**Story:** STORY-69-A, STORY-69-B
**Date:** 2026-06-02
**Source of truth:** ADR set under `.skraft/sdlc/design/adr-*.md`

## Matrix

| Concept | ADR (source of truth) | event-model-STORY-69.md | diagrams-STORY-69.md | contracts-STORY-69.md | Cause | Verdict |
|---|---|---|---|---|---|---|
| `SubscriptionRequest` | `Aggregate Root` (adr-001) | `Aggregate Root` (l.16) | `Aggregate Root` (l.13, l.52) | _(non structurel — agrégat interne)_ | n/a | PASS |
| `SubscriptionReference` | `Value Object` (adr-001) | `Value Object` (vocab cross-check) | `Value Object` (l.14, l.53) | _(non structurel — champ de payload)_ | n/a | PASS |
| `DriverId` | `Value Object` (adr-001) | `Value Object` (vocab cross-check) | `Value Object` (l.15, l.54) | _(champ de payload)_ | n/a | PASS |
| `VehicleDetails` | `Value Object` (adr-001) | `Value Object` (vocab cross-check) | `Value Object` (l.16, l.55) | _(champ de payload)_ | n/a | PASS |
| `PriceQuote` | `Value Object` (adr-003) | `Value Object` (vocab cross-check) | `Value Object` (l.18, l.57) | _(non structurel)_ | n/a | PASS |
| `PricingPolicy` | `Domain Service` (adr-003) | `Domain Service` (vocab cross-check l.129) | `Domain Service` (l.17, l.56) | _(non structurel — service interne)_ | n/a | PASS |
| `SubscriptionRequestInitiated` | `Domain Event` (adr-001) | `Domain Event` (vocab cross-check) | `Domain Event` (l.19, l.58) | `Domain Event` (l.55 heading) | n/a | PASS |
| `SubscriptionRequestRejected` | `Domain Event` (adr-001) | `Domain Event` (vocab cross-check) | `Domain Event` (l.20, l.59) | `Domain Event` (l.70 heading) | n/a | PASS |
| `ISubscriptionRequestRepository` | `Repository` (adr-001) | `Repository` (vocab cross-check) | `Repository` (l.21, l.60) | `Interface` / Repository (l.80, l.108) | n/a | PASS |
| `InitiateSubscriptionRequest` | `Command` (adr-001) | `Command` (l.13, l.26, l.126) | `Command` (l.65) | `Command` (l.31 heading, l.105) | n/a | PASS |
| `GetInsurancePriceQuote` | `Query` (adr-003) | `Query` (l.85, l.96, l.127) | `Query` (l.66) | `Query` (l.10 heading, l.104) | n/a | PASS |
| `SubscriptionRequestConfirmation` | `Read Model` (adr-001) | `Read Model` (l.28, l.52, l.130) | `Read Model` (l.34, l.61) | _(return shape — `SubscriptionRequestConfirmationViewModel`)_ | n/a | PASS |
| `InsurancePriceQuote` | `Read Model` (adr-003) | `Read Model` (l.98, l.130) | `Read Model` (l.35, l.62) | _(return shape — `InsurancePriceQuoteViewModel`)_ | n/a | PASS |
| `SubscriptionRequestRejectionDetail` | `Read Model` (adr-001) | `Read Model` (l.76, l.130) | `Read Model` (l.36, l.63) | _(return shape)_ | n/a | PASS |
| `PriceQuoteRejectionDetail` | `Read Model` (adr-003) | `Read Model` (l.120, l.130) | `Read Model` (l.37, l.64) | _(return shape)_ | n/a | PASS |

## Back-propagation journal

| Round | Concept | Artefact rewritten | Before → After | Trigger |
|---|---|---|---|---|
| 1 | `SubscriptionRequestInitiated`, `SubscriptionRequestRejected`, `SubscriptionReference`, `DriverId`, `VehicleDetails`, `PriceQuote`, `ISubscriptionRequestRepository` | `event-model-STORY-69.md` (vocabulary cross-check section) | Missing entries → Full listing with explicit `Domain Event` / `Value Object` / `Repository` labels | LABEL_DRIFT (incomplete cross-check) |

## Final verdict

- consistency-gate: **PASS**
- back-propagation rounds used: 1 (max 1 per artefact) — rewrite on event-model vocabulary cross-check section
- blockers raised: none
