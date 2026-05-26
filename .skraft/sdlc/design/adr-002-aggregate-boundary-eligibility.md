# ADR-002: Aggregate Boundary — EligibilityPolicy as Domain Service, not Aggregate

**Status:** Accepted  
**Date:** 2026-05-26

## Context

In DDD, an **Aggregate** enforces invariants over a consistency boundary and is
identified by a root entity with a stable identity. When designing the eligibility
check, two candidate structures were considered:

1. An `EligibilityAggregate` rooted on a driver identity that owns the eligibility
   decision as persisted state.
2. A **domain service** (`EligibilityPolicy`) that accepts `Driver` and `Vehicle`
   value objects and computes a result without persisting state.

The current story set (through v0.3) does not require persisting an eligibility
decision. The check is always computed fresh from the input: there is no "last
eligibility result" that must be consistent with a prior check.

## Decision

We will model `EligibilityPolicy` as a **stateless domain service** (not an
aggregate root). `Driver` and `Vehicle` are **value objects** — they carry the
inputs needed to apply the rule and have no lifecycle or identity of their own in
the eligibility context. The consistency boundary is enforced within a single
synchronous `Evaluate()` call with no external side effects.

If a future story requires persisting eligibility decisions (e.g., for audit,
renewal detection, or temporal querying), a new `EligibilityAggregate` should be
introduced at that point and this ADR superseded.

## Consequences

**Positive:**
- No persistence infrastructure needed for the eligibility check (no repository, no database table)
- `EligibilityPolicy.Evaluate()` is a pure function — trivially unit-testable
- Model reflects the actual business behaviour: eligibility is re-evaluated on each request, not stored
- Reduces complexity: no aggregate ID, no event sourcing, no optimistic concurrency concern

**Negative / trade-offs:**
- No audit trail of past eligibility decisions — if a regulatory audit requires it, this design must change
- If eligibility becomes stateful (e.g., caching a decision for a fixed validity window), a structural refactor is needed
- The boundary decision must be revisited as soon as a story introduces "check eligibility once and remember the result"

**Neutral:**
- `Driver` and `Vehicle` are constructors-only value objects. They cannot be used as entities in other contexts without remodelling.

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| `EligibilityAggregate` with persisted state | No story requires storing the result; introducing a repository for a stateless computation adds accidental complexity |
| Embedding rules directly in the API handler | Violates Clean Architecture — domain logic must live in the Domain layer, not the API or Application layer |
