---
name: architecture-decisions
description: Use when documenting architecture decisions as ADRs, evaluating trade-offs between alternatives, or managing the lifecycle of existing decisions. Covers ADR template, status transitions, consequence analysis, and quality criteria.
---

# Architecture Decisions

## Overview

Architecture Decision Records (ADRs) are lightweight documents that capture architectural decisions together with their context, alternatives considered, and consequences. They are the institutional memory of the architecture.

**Purpose:** An ADR answers the question: "Why does the architecture look the way it does?" Future readers — including future you — should be able to understand any structural decision without needing to ask the original author.

**Core rule:** One ADR per decision. A decision is a single, clear choice with trade-offs.

---

## ADR Template

### Blank Template

```markdown
# ADR-{NNN}: {Short Title}

**Status:** Proposed | Accepted | Rejected | Deprecated | Superseded by ADR-{NNN}
**Date:** {YYYY-MM-DD}

## Context

{Describe the situation and the forces that led to this decision. What problem were you solving? What constraints were in play? Why was a decision needed at all?}

## Decision

{State the decision clearly and directly. Start with "We will..." or "We have decided to...". One paragraph, maximum. Avoid explaining why here — that belongs in Context and Alternatives.}

## Consequences

**Positive:**
- {benefit 1}
- {benefit 2}

**Negative / trade-offs:**
- {trade-off 1}
- {trade-off 2}

**Neutral:**
- {change that is neither good nor bad but worth noting}

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| {option A} | {why not chosen — specific, not a strawman} |
| {option B} | {why not chosen — specific, not a strawman} |
```

### Filled Example — Auto Insurance Domain

```markdown
# ADR-001: Apply CQRS for Eligibility Check

**Status:** Accepted
**Date:** 2026-05-01

## Context

The eligibility check feature has distinct read and write concerns. The command side modifies eligibility state and raises the `EligibilityChecked` domain event. The query side needs to return a lightweight `EligibilityResult` read model optimised for display in the driver portal. Using a single model for both would force a compromise: either the domain model is polluted with display concerns, or the read model is burdened with invariant enforcement.

## Decision

We will apply simple CQRS: separate command handlers (mutate state, raise events) from query handlers (return read models). Both sides share the same relational database but use separate models. Command handlers write to the `eligibility` table via the domain aggregate. Query handlers read from an `eligibility_read` view or projection table.

## Consequences

**Positive:**
- Command and query models evolve independently — adding a display field does not touch the domain model
- Read models can be optimised (indexed, denormalised) without affecting domain logic
- Aligns with the event model produced in the Event Modeling session

**Negative / trade-offs:**
- Two code paths to maintain instead of one
- Slight increase in complexity for features that are simple reads/writes
- Developers must understand which side to modify when changing behaviour

**Neutral:**
- No separate database or event store required at this stage — keeping full CQRS as a future option if read volume grows

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| Single CRUD service (no CQRS) | Would mix mutation and query concerns in one model, forcing compromises in both directions as the feature grows |
| Full CQRS with separate stores | Over-engineering for current scale; no read replica needed yet |
```

---

## File Naming

`adr-{NNN}-{slug}.md`

Examples:
- `adr-001-cqrs-eligibility.md`
- `adr-002-aggregate-boundary-eligibility.md`
- `adr-003-event-sourcing.md` *(slug names the topic; verdict lives in the `Status:` field — `Accepted` or `Rejected`)*

Rules:
- Numbers are three digits, zero-padded: `001`, not `1`
- Slugs are lowercase kebab-case, derived from the short title
- **Slugs name the SUBJECT, never the verdict.** Forbidden suffixes: `-rejected`, `-accepted`, `-deprecated`, `-superseded`. The verdict lives in `Status:`.
- Gaps in numbering are forbidden — never skip a number
- Numbers are never reused — a deprecated ADR keeps its number

---

## Status and Lifecycle

```
Proposed → Accepted   → Deprecated
        ↘ Rejected    → Superseded by ADR-{NNN}
```

- **Proposed:** Decision is drafted, committed, and awaiting human ratification
- **Accepted:** Option was evaluated and adopted — the architecture reflects it
- **Rejected:** Option was evaluated and explicitly declined — the ADR records the analysis so the debate is not re-run later without new evidence
- **Deprecated:** Decision is no longer relevant (e.g., the feature was removed), but was never superseded
- **Superseded:** Decision has been replaced by a newer one — always reference the successor

**Linking rule:** When ADR-002 supersedes ADR-001:
- ADR-001 status: `Superseded by ADR-002`
- ADR-002 context: reference ADR-001 — "This supersedes ADR-001, which prescribed X."

**No ADR is ever deleted.** The historical record of why a decision was made is as valuable as the decision itself.

### Human-in-the-loop ratification

The `Proposed → Accepted | Rejected` transition is owned by a human, not the agent. Two channels:

- **Agentic workflow (GitHub):** the Proposed ADR is committed and the agent posts a ratification request as a comment on the originating issue. Human replies with `accept` or `reject` (with rationale). Agent then commits the status flip.
- **Local developer machine:** the Proposed ADR is committed and the agent prompts the developer in-terminal. Developer answers; agent commits the status flip.

**Both transitions are committed:** the `Proposed` revision AND the final `Accepted` / `Rejected` revision land in git history. The trail of "we paused here for a human" is part of the architectural record.

### Rejected ADRs

A `Rejected` ADR documents that an option was seriously evaluated and the team decided NOT to adopt it. The ADR exists so the same debate is not re-opened in six months without new evidence.

- **Filename:** named after the SUBJECT — `adr-NNN-event-sourcing.md`, NEVER `adr-NNN-event-sourcing-rejected.md`. The verdict lives in `Status:`, not in the filename.
- **Decision phrasing:** MAY start with "We will not adopt X because…" — this is precisely what a `Rejected` status means.
- **Trigger requirement:** a `Rejected` ADR is written ONLY when a story or measurable force in the current batch raised the question (see G9 traceability). A `Rejected` ADR with no triggering story is a non-decision artefact and is forbidden.
- **Alternatives Rejected section** is flipped: list what the team adopted INSTEAD (e.g., "state-based persistence with audit-log table").

---

## When to Write an ADR

Write an ADR for any decision that:
- Establishes or changes a layer boundary
- Chooses an aggregate boundary
- Adopts or rejects a pattern (CQRS, Event Sourcing, Saga, Specification)
- Establishes a bounded context boundary
- Chooses a context mapping relationship (ACL, Conformist, etc.)
- Affects a cross-cutting concern (error handling strategy, validation approach)

### When NOT to Write an ADR

Do not write an ADR for:
- Implementation details (naming conventions, code formatting)
- Library version choices — unless the version has architectural impact (e.g., switching from EF Core to Dapper changes the data access strategy)
- Configuration choices (connection string format, environment variable names)

---

## Trade-off Framework

Every ADR must demonstrate genuine trade-off analysis — not a decision with only upsides.

### The 5 Universal Forces

| Force | Question |
|---|---|
| **Simplicity** | Does this make the system easier to understand and change? |
| **Consistency** | Does this fit the patterns already established in this codebase? |
| **Performance** | Does this meet the performance requirements without over-engineering? |
| **Evolvability** | Does this make future changes easier or harder? |
| **Team capability** | Can the current team maintain this without a steep learning curve? |

### Trade-off Analysis Template

```
| Force | Option A | Option B | Weight |
|---|---|---|---|
| Simplicity | High | Low | High |
| Consistency | High | Medium | Medium |
| Performance | Medium | High | Low |
| Evolvability | High | High | High |
| Team capability | High | Low | High |
| → Score | 4.2 | 2.8 | |
```

Use this table in the Context section when the decision is genuinely hard. Skip it for obvious decisions.

---

## ADR Quality Checklist

Before writing the final ADR, verify:

- [ ] **Single decision** — the ADR captures exactly one choice, not a bundle of decisions
- [ ] **Clear decision statement** — starts with "We will…" or "We have decided to…"; no ambiguity
- [ ] **Context explains the why** — the forces that made this decision necessary are described
- [ ] **Consequences include negatives** — no decision is trade-off-free; if you can't name a downside, keep thinking
- [ ] **Alternatives are genuine** — rejected alternatives are real options that were seriously considered, not strawmen
- [ ] **Alternatives explain the rejection** — not "too complex" but "too complex because X and Y are not needed given the current story set"
- [ ] **Status is set** — `Proposed` when drafting, `Accepted` when ratified
- [ ] **Supersession is linked** — if this supersedes another ADR, both are updated

---

## References

- [adr-template.md](references/adr-template.md) — Blank template + filled auto-insurance example
- [decision-drivers.md](references/decision-drivers.md) — Trade-off analysis framework and decision scenarios
