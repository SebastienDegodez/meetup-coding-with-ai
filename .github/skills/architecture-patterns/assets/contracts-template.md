# Interface Contracts Template

Used by the `solution-architect` persona at Phase 8 (INTERFACE CONTRACTS).

This template is intentionally generic. The contract category (`Command` \| `Query` \| `Domain Event` \| `Interface`) for each item MUST come from the ADR set ratified at Phase 7.

---

## Caller-supplied slots

| Slot | Source | Notes |
|---|---|---|
| `{story-id}` | story metadata | e.g. `story-41` |
| `{bounded-context-name}` | persona Phase 5 | section per context |
| `{contract-category}` | **ratified by ADR at Phase 7** | one of: `Command` \| `Query` \| `Domain Event` \| `Interface` |
| `{contract-name}` | persona Phase 8 | `PascalCase` |
| `{field-list}` | persona Phase 8 | name + type + nullability |

---

## Template body

```markdown
# Interface Contracts — {story-id}: {story-title}

**Story:** {story-id}
**Date:** {YYYY-MM-DD}

## {bounded-context-name}

### {contract-category}: `{contract-name}`

| Field | Type | Required | Notes |
|---|---|---|---|
| `{field-name}` | `{type}` | yes \| no | {validation / invariant note} |

Repeat one block per contract item, grouped by `{contract-category}` in this order:
1. Queries (read operations)
2. Commands (write operations)
3. Domain Events (state-change records)
4. Interfaces (repository / service signatures)

## Vocabulary cross-check (Phase 9 input)

Every `{contract-category}` heading MUST match the classification ratified for `{contract-name}` in the corresponding ADR. A `Command` in this file paired with a `Query` ADR triggers `CLASSIFICATION_DRIFT` at Phase 9.
```

---

## Forbidden in this template

- Hard-coded domain names (`CheckEligibility`, `EligibilityChecked`, `IEligibilityRepository`, …).
- Hard-coded category for any specific name (no `Command: CheckEligibility` baked in).
- Field examples that telegraph a particular bounded context.
