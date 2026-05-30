# ADR-001: Eligibility Check as a Query (No Write Side)

**Status:** Accepted
**Date:** 2026-05-30

## Context

Story #52 requires that the eligibility system enforce a new minimum driving age of 21 for standard vehicles. The eligibility evaluation is a decision-support operation: given driver attributes and vehicle type, it returns a pass/fail result with an optional rejection reason. It does not create or modify any persisted record — there is no "eligibility entity" that is saved. The existing `CheckEligibilityQuery` / `CheckEligibilityQueryHandler` infrastructure reflects this: the handler constructs ephemeral domain objects, invokes `EligibilityPolicy.Evaluate`, and returns a `EligibilityViewModel`. No write path exists or is required.

Force: the operation has a pure read/evaluate signature (input → result, no side effects). Introducing a Command + write side would require persisting something that the domain does not yet need to persist, adding complexity with no current business benefit.

## Decision

We will model the eligibility check exclusively as a Query. `CheckEligibilityQuery` is the single entry point. No Command or write handler is introduced. The `EligibilityPolicy` Domain Service evaluates rules and returns an `EligibilityResult` in-memory. The Application layer maps this to `EligibilityViewModel` for presentation.

## Consequences

**Positive:**
- Zero persistence complexity — no database schema, migrations, or repository required for eligibility evaluation.
- Domain logic (age rules, experience rules) is fully encapsulated in Value Objects and the Domain Service, testable without infrastructure.
- Aligns with the existing infrastructure: `IQueryBus`, `IQueryHandler<,>`, `CheckEligibilityQueryHandler` are reused unchanged.

**Negative / trade-offs:**
- No audit trail of eligibility decisions — if a future regulatory requirement demands "log every eligibility evaluation", this decision must be revisited and a write side added.
- No ability to replay or debug historical evaluations without logging at the infrastructure layer.

**Neutral:**
- If eligibility decisions must eventually be persisted (e.g. for quote journaling), an ADR superseding this one will introduce a Command + `EligibilityEvaluated` domain event. The query path would remain unchanged.

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| Command + domain event (`EligibilityEvaluated`) | No current story requires persisting or reacting to eligibility decisions. Adding a write side now would violate YAGNI: extra infrastructure, extra handler, extra tests, for zero current business value. |
| Single CRUD service (no CQRS split) | The application layer already uses `IQueryBus` / `ICommandBus` separation. Deviating would introduce inconsistency. |
| Do without the CQRS pattern (single service method) | Rejected on the same consistency grounds; also, a plain service method would couple the API controller directly to the domain policy, bypassing the established handler + bus infrastructure. |
