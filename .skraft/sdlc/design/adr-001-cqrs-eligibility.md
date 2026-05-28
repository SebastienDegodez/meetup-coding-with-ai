# ADR-001: Apply Query-Side CQRS for Eligibility Check

**Status:** Accepted  
**Date:** 2026-05-26

## Context

The eligibility check feature has a clear asymmetry between read and write concerns.
The business operation is fundamentally a **query** — a driver submits their details
and receives an eligibility decision. There is no persistent state mutation at this
stage (no policy issuance, no premium calculation). However, the domain logic
(`EligibilityPolicy.Evaluate`) is rich enough to warrant a dedicated domain layer
separate from the API response shape.

The codebase already contains `IQueryBus`, `IQueryHandler<TQuery, TResult>`,
`QueryBus`, and `CheckEligibilityQueryHandler` — meaning CQRS on the query side is
already in use. This ADR ratifies that decision and establishes it as the canonical
pattern for future features.

No command side (write path) is active in the current milestone. The `ICommandBus`
and `ICommandHandler` interfaces exist but are not yet used by any handler.

## Decision

We will apply **query-side CQRS** for the eligibility feature: the `CheckEligibility`
operation is modelled as a query (not a command), dispatched through `IQueryBus`,
handled by `CheckEligibilityQueryHandler`, and returns `EligibilityViewModel` as the
read model. The domain `EligibilityPolicy` remains free of any API or view concerns.

Command-side CQRS will be introduced only when a story requires persistent state
mutation (e.g., policy issuance, application submission).

## Consequences

**Positive:**
- Domain logic (`EligibilityPolicy`) is fully decoupled from the HTTP response shape
- `EligibilityViewModel` can evolve independently of the domain model
- The query handler is independently testable without an HTTP context
- Consistent with the existing codebase pattern — no architectural divergence

**Negative / trade-offs:**
- Two classes instead of one for what is conceptually a single operation (`Query` + `Handler`)
- Developers must know to add a handler and register it in DI when adding a new query
- `ICommandBus` and `ICommandHandler` exist but are unused — creates dead interface clutter until command-side stories arrive

**Neutral:**
- No separate read database or event store required — query handler computes the result in-process

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| Direct service call from API endpoint | Couples the API layer to domain logic; hinders independent testing and future command-side extension |
| Full CQRS with separate read store | Over-engineering: no read volume problem exists; eligibility is computed synchronously with no persistence |
| MediatR / dispatcher library | Not needed: the hand-rolled `IQueryBus` already provides the dispatch pattern with zero extra dependencies |
