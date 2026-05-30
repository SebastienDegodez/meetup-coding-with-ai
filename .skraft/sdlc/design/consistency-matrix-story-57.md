# Consistency Matrix — story-57

**Story:** story-57
**Date:** 2026-05-30
**Source de vérité :** ADR set `.skraft/sdlc/design/adr-*.md`

## Matrix

| Concept | ADR (source de vérité) | event-model-story-57.md | diagrams-story-57.md | contracts-story-57.md | Cause | Verdict |
|---|---|---|---|---|---|---|
| `Vehicle` | `Value Object` (adr-001, l.10) | `Value Object` (l.32) | `Value Object` (l.14, l.34) | `Value Object` (l.45) | n/a | PASS |
| `EligibilityPolicy` | `Domain Service` (adr-001 — qualifié implicitement par méthode pure sans état) | `Domain Service` (l.33) | `Domain Service` (l.13, l.35) | `Domain Service` (l.56) | n/a | PASS |
| `CheckEligibility` | `Query` (adr-002, l.10) | `Query` (l.11–22) | `Query` (l.11) | `Query` (l.8, l.54) | n/a | PASS |
| `EligibilityViewModel` | `Read Model` (adr-002, l.10) | `Read Model` (l.14, l.34) | `Read Model` (l.17, l.40) | `Read Model` (l.24, l.55) | n/a | PASS |

> **Note sur `EligibilityPolicy`** : ADR-001 couvre la frontière de `Vehicle` comme Value Object. `EligibilityPolicy` est implicitement ratifié comme `Domain Service` par ADR-001 (méthode pure, pas d'état propre, porteur de règles métier cross-entités). Une ADR dédiée sera écrite si une story future change sa nature (ex. : injection d'un repository).

> **Note sur `Driver`** : `Driver` apparaît dans les diagrammes et contrats comme `Value Object` (même raisonnement que `Vehicle` : construit par requête, sans identité ni lifecycle). Pas de row dans cette matrice car `Driver` n'est pas nommé dans un ADR du batch actuel. Une ADR le couvrant sera produite lors d'une story qui en modifie la frontière.

## Back-propagation journal

| Round | Concept | Artefact réécrit | Avant → Après | Déclencheur |
|---|---|---|---|---|
| — | — | — | — | — |

> Aucune back-propagation effectuée : tous les artefacts sont en accord avec la source de vérité ADR dès la première passe.

## Final verdict

- **consistency-gate:** PASS
- **back-propagation rounds used:** 0 (max 1 par artefact)
- **blockers raised:** aucun
