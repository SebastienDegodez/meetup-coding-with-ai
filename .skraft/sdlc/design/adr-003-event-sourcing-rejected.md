# ADR-003: Event Sourcing — Rejected for Eligibility Context

**Status:** Accepted  
**Date:** 2026-05-26

## Context

Event Sourcing reconstructs aggregate state by replaying a sequence of domain
events stored in an event log. It is appropriate when:

- An audit trail of all state changes is a business or regulatory requirement, or
- Temporal queries ("what was the eligibility decision on date X?") are needed, or
- Multiple projections of the same state must be maintained independently.

The current eligibility check (through v0.3-legal-minimum-age) has none of these
characteristics: there is no persisted aggregate state, decisions are computed
synchronously from inputs, and no story requires querying past decisions.

## Decision

We will **not apply Event Sourcing** to the EligibilityContext. The domain service
model (ADR-002) is sufficient. If a future story requires audit trail or temporal
query capability on eligibility decisions, this ADR should be superseded with one
that introduces `EligibilityAggregate` and an event store.

## Consequences

**Positive:**
- No event store infrastructure required (no Kafka, EventStoreDB, or outbox table)
- No projection maintenance — the read model is computed on demand
- Onboarding complexity is kept low — team does not need event sourcing expertise for this feature

**Negative / trade-offs:**
- No built-in audit trail — if regulators require a log of eligibility decisions, infrastructure must be added later
- Retrofitting event sourcing is a structural refactor, not an incremental addition
- Any temporal query requirement ("was this driver eligible on date X?") cannot be answered without replay

**Neutral:**
- The conceptual domain event `EligibilityChecked` is documented in the interface contracts for modelling purposes but is not persisted.

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| Event sourcing from the start | No story in the current or planned milestone set requires audit trail or temporal queries; YAGNI applies |
| Application-level audit log (separate table) | Could be added independently when a story requires it, without Event Sourcing overhead — preferred future path if audit becomes a requirement |
