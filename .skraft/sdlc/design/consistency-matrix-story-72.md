# Consistency Matrix — story-72

**Story:** story-72
**Date:** 2026-06-02
**Source of truth:** ADR set under `.skraft/sdlc/design/adr-*.md`

## Matrix

| Concept | ADR (source of truth) | event-model-story-72.md | diagrams-story-72.md | contracts-story-72.md | Cause | Verdict |
|---|---|---|---|---|---|---|
| `Vehicle` | `Value Object` (adr-001) | `Value Object` | `Value Object` | `Value Object` | n/a | PASS |
| `Driver` | `Value Object` (adr-001) | `Value Object` | `Value Object` | `Value Object` | n/a | PASS |
| `EligibilityResult` | `Value Object` (adr-001) | `Value Object` | `Value Object` | — (référencé comme type de retour) | n/a | PASS |
| `EligibilityPolicy` | `Domain Service` (adr-002) | `Domain Service` | `Domain Service` | `Domain Service` | n/a | PASS |
| `CheckEligibilityForVehicle` | `Query` (adr-002) | `Query` | `Query` | `Query` | n/a | PASS |
| `EligibilityViewModel` | `Read Model` (adr-002) | `Read Model` | `Read Model` | `Read Model` | n/a | PASS |
| `CheckEligibilityQueryHandler` | `Application Query` (adr-002) | — (non cité, correct : handler n'est pas un concept du modèle événementiel) | `Application Query` | — (non cité comme contrat, correct) | n/a | PASS |

## Back-propagation journal

| Round | Concept | Artefact rewritten | Before → After | Trigger |
|---|---|---|---|---|
| — | — | — | — | Aucun drift détecté ; aucune back-propagation effectuée |

## Final verdict

- consistency-gate: **PASS**
- back-propagation rounds used: 0 (max 1 par artefact)
- blockers raised: none
