---
name: architecture-review-criteria
description: Use when reviewing DESIGN artefacts (event models, ADRs, component diagrams, context maps, interface contracts) for quality, DDD compliance, architectural correctness, and prohibition of negative or baseline-restating ADRs. Contains gate definitions and scoring rubric for the solution-architect-reviewer lenses.
---

# Architecture Review Criteria

## Overview

15 gates across 3 lenses, applied by the `solution-architect-reviewer` agent to DESIGN artefacts. Gates enforce DDD correctness, Clean Architecture compliance, persona-side consistency gate integrity, prohibition of negative or baseline-restating ADRs, and fitness for the stories in scope.

**Applied by:** `solution-architect-reviewer`
**Applied to:** ADRs, event models, component diagrams, context maps, interface contracts, consistency matrices, supersession plans, blocker files
**Prior phase required:** DESIGN artefacts from `solution-architect` (Phases 1–10 complete; no open blocker file)

---

## Gate Definitions

### Lens 1 — consistency-lens

Evaluates: ADRs + diagrams + contracts + consistency-matrix + supersession-plan + blockers

| Gate | Definition | Pass condition | Severity |
|---|---|---|---|
| G1 | Every structural element in a diagram (aggregate, bounded context, pattern) AND every complexity-adding pattern actually present in the source tree (CQRS+Bus, Event Sourcing, Saga) has a traceable ADR justification. No element exists without an architectural rationale. | All structural elements in all diagrams AND all detected source-tree patterns reference at least one ADR. | **BLOCKER** |
| G2 | No two ADRs contradict each other. Supersession links are bidirectional: superseded ADR carries `Superseded by ADR-{NNN}`; new ADR carries `Supersedes: ADR-{MMM}`. | Zero contradicting decisions across all ADRs. Zero broken supersession links. | BLOCKER |
| G10 | A `consistency-matrix-{story}.md` exists for every story under design and its `consistency-gate` cell is `PASS`. The back-propagation journal explains every rewrite. | Matrix file present and PASS for every story; journal complete. | BLOCKER |
| G12 | Every row in `supersession-plan-{story}.md` is fully realised: bidirectional ADR links present, AND no descriptive artefact still cites the superseded ADR. | Plan rows = realised supersessions; zero stale citations. | BLOCKER |
| G14 | No ADR encodes the verdict in the FILENAME: filenames must name the topic (`adr-NNN-event-sourcing.md`), never carry a verdict suffix (`*-rejected.md`, `*-accepted.md`, `*-deprecated.md`, `*-superseded.md`). The verdict belongs in the `Status:` frontmatter (`Proposed \| Accepted \| Rejected \| Deprecated \| Superseded`). A `Status: Rejected` ADR is admissible IFF a story or measurable force in this batch raised the question (per G9 traceability) AND the `Alternatives Rejected` section lists the option that was adopted instead. A `Rejected` ADR with no triggering story is a non-decision artefact. | Zero verdict-bearing filenames; every `Status: Rejected` ADR traces to a triggering story and names the adopted alternative. | BLOCKER |

### Lens 2 — architecture-compliance-lens

Evaluates: diagrams + contracts + event models

| Gate | Definition | Pass condition | Severity |
|---|---|---|---|
| G3 | Dependency rule: Domain and Application layers have no dependencies on Infrastructure or API layers. | Zero imports of Infrastructure or API types in Domain or Application contracts. | BLOCKER |
| G4 | All application interfaces (repositories, gateways, event publishers) are defined in the Application layer contracts. None are defined in Infrastructure. | Zero infrastructure-defined interfaces in contracts. All I* interfaces listed under Application layer. | BLOCKER |
| G5 | Each aggregate enforces its own invariants. No aggregate enforces invariants that belong to another aggregate. | Zero cross-aggregate invariant references in contracts or diagrams. | HIGH |
| G6 | Context map declares every inter-context relationship with an explicit pattern (ACL, Conformist, Shared Kernel, Partnership, Open Host Service, Published Language) AND every label is admissible: (a) no relationship labelled `Conformist` has a **Core** subdomain as its downstream; (b) no relationship labelled `Conformist` is in fact a published contract consumed with a local copy or translation (that is OHS/PL upstream + ACL downstream, see V-DDD-09 / V-DDD-10). | Zero unlabelled arrows between bounded contexts in context-map.md AND zero inadmissible labels. | HIGH |

### Lens 3 — fitness-lens

Evaluates: diagrams + contracts + stories + ADRs

| Gate | Definition | Pass condition | Severity |
|---|---|---|---|
| G7 | Every story from DISCUSS maps to at least one trigger (Command or Query) in the event model. | All story IDs from stories-{milestone}.md appear in at least one event model slice with either a Command or a Query trigger. | HIGH |
| G8 | Every Command in contracts has at least one corresponding domain event. Queries are exempt. No dangling commands. | Zero Commands without a corresponding domain event. | HIGH |
| G9 | No aggregate, bounded context, Event Sourcing adoption, or Saga is introduced without a traceable story justification. | Zero unjustified architectural elements. Every element traces back to a story ID. | MEDIUM |
| G11 | Every ADR adopting a complexity-adding pattern (`CQRS`, `Event Sourcing`, `Saga`, `eventual consistency`, `micro-service split`, `ACL`) cites at least one admissible force in Context AND includes `"do without the pattern"` in Alternatives Rejected. `"Consistency with existing code"` alone is **not** admissible. | All such ADRs pass both checks. | HIGH |
| G15 | No ADR documents a baseline already enforced by project skills or architecture tests as if it were a free decision. Topics already baseline-enforced (CQS at method level, Clean Architecture layer boundaries, convention-based DI handler registration, repository pattern) require an EXTENSION ADR (e.g., `Introduce a CQRS Dispatch Bus on top of CQS baseline`, `Add pipeline behaviors`), not a restatement. | Zero ADRs whose Decision merely restates a baseline already enforced by a project skill or architecture test. | HIGH |

### Cross-cutting — escalation gate

| Gate | Definition | Pass condition | Severity |
|---|---|---|---|
| G13 | No open blocker file exists under `.skraft/sdlc/design/blockers/` with frontmatter `status: awaiting_human`. | All blocker files resolved or absent. | BLOCKER |

---

## Severity Definitions

| Severity | Definition | Impact |
|---|---|---|
| **BLOCKER** | Fundamental violation that invalidates the architecture. Cannot proceed to DISTILL. | Forces `rejected` verdict |
| **HIGH** | Significant flaw that will cause problems in DISTILL or implementation. Requires correction. | Forces `changes_requested` verdict |
| **MEDIUM** | Design smell or sub-optimal choice. Correction recommended before DISTILL. | Forces `changes_requested` verdict |
| **LOW** | Minor inconsistency or style issue. Can be noted and tracked. | May still yield `approved` with notes |

---

## Verdict Derivation Table

| Condition | Verdict |
|---|---|
| G13 fires (any open blocker file) | `rejected` — escalation pending |
| ≥1 BLOCKER finding | `rejected` |
| ≥1 HIGH finding, 0 BLOCKERs | `changes_requested` |
| MEDIUM findings only, 0 HIGH, 0 BLOCKER | `changes_requested` |
| LOW findings only | `approved` (with notes) |
| Zero findings | `approved` |

---

## DDD Compliance Rules Summary

1. Aggregates are identified by the invariants they enforce, not by their data
2. Cross-aggregate references use IDs only — never object references
3. Domain events are raised by aggregate roots, not application services
4. Repository interfaces live in Application layer, not Domain or Infrastructure
5. Bounded context boundaries map to language boundaries — if the same word means different things, there is a boundary
6. Context map relationships are explicit and labelled — implicit dependencies are forbidden
7. Subdomains are classified (Core/Supporting/Generic) and the investment level is justified
8. A Core subdomain is never the downstream of a `Conformist` relationship — it protects its Ubiquitous Language via an ACL. Consuming a published contract (ViewModel/event/DTO) with a local copy or translation is OHS/PL + ACL, never Conformist, regardless of how trivial the translation is

---

## YAGNI Detection Heuristics

Apply during G9 evaluation:

| Element | Question to ask |
|---|---|
| New bounded context | Which story requires this context to be separate? |
| New aggregate | Which invariant does this aggregate enforce? Which story produces that invariant? |
| Event Sourcing | Which story requires audit trail or temporal queries? |
| Saga | Which cross-aggregate workflow spans multiple stories? |
| ACL | Which conflicting model in the upstream context makes translation necessary, OR is the downstream a Core subdomain protecting its Ubiquitous Language? |

If the answer is "none" or "future needs" — flag as G9 MEDIUM violation.

---

## References

- [gate-definitions.md](references/gate-definitions.md) — Detailed checklist per gate (G1–G9) with step-by-step checks
- [ddd-violations.md](references/ddd-violations.md) — 10 DDD violation patterns detectable in artefacts
- [clean-arch-violations.md](references/clean-arch-violations.md) — 8 Clean Architecture violations detectable in contracts and diagrams
- [verdict-rubric.md](references/verdict-rubric.md) — Verdict derivation, confidence levels, and 3 example review verdicts
