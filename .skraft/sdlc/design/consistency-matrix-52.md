# Consistency Matrix — story-52

**Story:** story-52
**Date:** 2026-05-30
**Source of truth:** ADR set under `.skraft/sdlc/design/adr-*.md`

## Matrix

| Concept | ADR (source of truth) | event-model-52.md | diagrams-52.md | contracts-52.md | Cause | Verdict |
|---|---|---|---|---|---|---|
| `CheckEligibility` | `Query` (adr-001) | `Query` (line 22) | `Query` (line 11, 34) | `Query` (line 8) | n/a | PASS |
| `EligibilityViewModel` | `Read Model` (adr-001) | `Read Model` (line 24 — after back-prop) | `Read Model` (line 17, 40) | `Read Model` (line 17) | n/a | PASS |
| `EligibilityPolicy` | `Domain Service` (adr-002) | `Domain Service` (line 29) | `Domain Service` (line 13, 36) | _(not named as contract; Application Handler orchestrates it)_ | n/a | PASS |
| `Driver` | `Value Object` (adr-002) | _(not named directly; constructed by handler)_ | `Value Object` (line 14, 37) | `Value Object` (field in Query) | n/a | PASS |
| `Vehicle` | `Value Object` (adr-002) | _(not named directly; constructed by handler)_ | `Value Object` (line 15, 38) | `Value Object` (field in Query) | n/a | PASS |
| `EligibilityResult` | `Value Object` (adr-002) | `Value Object` (line 24 inline — after back-prop) | `Value Object` (line 16, 39) | `Value Object` (line 45) | n/a | PASS |
| `IQueryHandler<CheckEligibilityQuery, EligibilityViewModel>` | Application Interface (adr-001) | _(not in event model — application layer concept)_ | _(not in diagram — application layer detail)_ | `Interface` (line 31) | n/a | PASS |

## Back-propagation journal

| Round | Concept | Artefact rewritten | Before → After | Trigger |
|---|---|---|---|---|
| 1 | `EligibilityResult` / `EligibilityViewModel` | `event-model-52.md` | Read Model slot: `EligibilityResult` → `EligibilityViewModel`; inline note added to clarify `EligibilityResult` is a `Value Object` | CLASSIFICATION_DRIFT (EligibilityResult listed as Read Model; ADR-002 ratifies it as Value Object; ADR-001 ratifies EligibilityViewModel as Read Model) |

## Final verdict

- consistency-gate: **PASS**
- back-propagation rounds used: **1** (max 1 per artefact — within budget)
- blockers raised: none
