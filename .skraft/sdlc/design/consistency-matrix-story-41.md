# Consistency Matrix — STORY-41: Update minimum driving age to 21

**Story:** STORY-41  
**Milestone:** v0.3-legal-minimum-age  
**Date:** 2026-05-28  
**Overall Verdict:** ✅ PASS (after back-propagation)

---

## Matrix

| Concept | ADR (truth) | event-model-story-41.md | diagrams-story-41.md | contracts-story-41.md | Cause | Verdict |
|---|---|---|---|---|---|---|
| `CheckEligibility` | Query (ADR-001) | ~~Command~~ → **Query** (fixed) | Query (CheckEligibilityQuery label) | Query (## Query: CheckEligibility) | CLASSIFICATION_DRIFT in event-model | ✅ PASS (after back-prop) |
| `EligibilityPolicy` | Domain Service (ADR-002) | Domain Service (implicit) | Domain Service (explicit label) | Domain Service (explicit) | — | ✅ PASS |
| `Driver` | Value Object (ADR-002) | not classified (input only) | ~~Entity~~ → **Value Object** (fixed) | not classified (input only) | CLASSIFICATION_DRIFT in diagrams | ✅ PASS (after back-prop) |
| `Vehicle` | Value Object (ADR-002) | not classified (input only) | Value Object (explicit label) | Value Object (explicit) | — | ✅ PASS |
| `EligibilityResult` | Value Object (ADR-002) | Read Model (event model role) | Value Object (explicit label) | Read Model (EligibilityViewModel) | LABEL_DRIFT (role vs DDD type — compatible) | ✅ PASS |
| `EligibilityChecked` | Domain Event conceptual (ADR-003) | Event (timeline) | not listed | Domain Event (conceptual) | — | ✅ PASS |
| `EligibilityViewModel` | Read Model (ADR-001) | Read Model (timeline) | Read Model (Application layer) | Read Model (explicit) | — | ✅ PASS |

---

## Back-Propagation Journal

| Round | Artefact modified | Concept | Before | After | Result |
|---|---|---|---|---|---|
| 1 | `event-model-story-41.md` | `CheckEligibility` classification | `section Command` / `Command` row | `section Query` / `Query` row | PASS — verified by grep |
| 1 | `diagrams-story-41.md` | `Driver` classification | `(Entity)` node label / `Entity` table row | `(Value Object)` node label / `Value Object` table row | PASS — verified by grep |

---

## Notes

- `EligibilityResult` is classified as `Value Object` in the diagrams (DDD structural type) and as `Read Model` in the event-model and contracts (event-model role). These are compatible — `EligibilityResult` is the domain value object; `EligibilityViewModel` is the application-layer read model derived from it. No drift.
- All structural concepts from ADRs 001–004 verified against all three artefacts. No open blockers.
