# Component Diagram Template

Used by the `solution-architect` persona at Phase 6 (TACTICAL DESIGN / DIAGRAMS).

This template is intentionally generic. Do NOT inline DDD classifications for specific names here — every node's classification comes from an ADR (Phase 7) and may need back-propagation through Phase 9.

---

## Caller-supplied slots

| Slot | Source | Notes |
|---|---|---|
| `{story-id}` | story metadata | e.g. `story-41` |
| `{bounded-context-name}` | persona Phase 5 | one per bounded context |
| `{concept-list}` | persona Phase 6 | list of `{name, classification}` pairs |
| `{classification}` (per concept) | **provisional during Phase 6; ratified by ADR at Phase 7** | exactly one of: `Entity` \| `Value Object` \| `Aggregate Root` \| `Domain Service` \| `Domain Event` \| `Repository` \| `Read Model` |

---

## Template body

```markdown
# Diagrams — {story-id}: {story-title}

**Story:** {story-id}
**Date:** {YYYY-MM-DD}

## {bounded-context-name} — Component Diagram

```mermaid
graph TD
    subgraph {bounded-context-name}
        %% Repeat per concept in {concept-list}:
        {ConceptName}["{ConceptName}\n({classification})"]
        %% ...
    end

    %% Dependency edges between concepts go here.
```

## Classification table (Phase 9 input)

| Concept | Classification (source: ADR) | Story impact |
|---|---|---|
| `{ConceptName}` | `{classification}` (adr-{NNN}) | {none \| modified \| added} |

## Vocabulary cross-check

Every `{classification}` on a node label MUST equal the classification recorded in its source ADR row above. Phase 9 enforces this via grep; any divergence triggers back-propagation or HALT.
```

---

## Forbidden in this template

- Hard-coded names (`EligibilityAggregate`, `DriverId`, `RejectionReason`, …).
- Hard-coded classifications outside the slot (no `Driver(Entity)` baked in).
- Examples that pre-commit a concept to one DDD category before the ADR is written.
