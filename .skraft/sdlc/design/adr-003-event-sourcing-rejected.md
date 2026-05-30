# ADR-003: Event Sourcing Rejected for Eligibility

**Status:** Accepted
**Date:** 2026-05-30

## Context

Event Sourcing stores state as a sequence of events rather than as a current snapshot. It is justified when:
- An audit trail of all state changes is a regulatory or business requirement, OR
- Temporal queries ("what was the eligibility rule at time T?") are needed.

Story #52 requires updating the minimum driving age constant from 18 to 21. The eligibility check is a stateless evaluation (Query, per ADR-001). There is no entity being persisted, so there is no "state change history" to record. The story's acceptance criteria do not reference any audit, replay, or temporal query capability.

## Decision

We will not apply Event Sourcing to the Eligibility bounded context at this milestone.

## Consequences

**Positive:**
- No event store infrastructure required — simpler deployment, simpler testing.
- No projection rebuilding complexity.

**Negative / trade-offs:**
- If a future regulatory requirement mandates "prove that on date D, the minimum age rule was X", the system cannot answer this without external logging. At that point, an ADR superseding this one must be written.

**Neutral:**
- Domain events (e.g. `EligibilityEvaluated`) could be introduced later as a write-side addition without requiring full Event Sourcing. The two concepts are independent.

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| Apply Event Sourcing | No audit trail or temporal query requirement exists in the current story set. Introducing an event store for a 1-line domain rule change would be significant over-engineering. |
| Do without the pattern (current choice) | This IS the accepted decision. No penalty. |
