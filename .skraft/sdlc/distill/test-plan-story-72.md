# Test Plan — STORY-72 : Âge légal de conduite porté à 21 ans

**Story:** story-72
**Date:** 2026-06-03
**Use case boundary:** `CheckEligibilityQueryHandler.Handle(CheckEligibilityQuery)`

---

## Coverage Matrix

| Scenario | Use Case Boundary | Layer | Extraction Reason | Double Type | Walking Skeleton | Priority |
|---|---|---|---|---|---|---|
| AC-02 — Conducteur 21 ans, voiture → éligible | `CheckEligibilityQueryHandler` | Application | — | Aucun (domain pur, pas de repo) | A | P1 |
| AC-02 — Conducteur 21 ans exactement aujourd'hui, voiture → éligible (borne) | `CheckEligibilityQueryHandler` | Application | — | Aucun (domain pur, pas de repo) | A | P1 |
| AC-01 — Conducteur 20 ans, voiture → refusé | `CheckEligibilityQueryHandler` | Application | — | Aucun (domain pur, pas de repo) | A | P1 |
| AC-04 — Conducteur 18 ans, voiture → refusé (ancienne limite) | `CheckEligibilityQueryHandler` | Application | — | Aucun (domain pur, pas de repo) | A | P2 |
| AC-03 — Conducteur 16 ans, trottinette électrique → éligible | `CheckEligibilityQueryHandler` | Application | — | Aucun (domain pur, pas de repo) | A | P2 |

---

## Mandate 4 Analysis — Vehicle.MinimumAge()

La fonction `Vehicle.MinimumAge()` post-story-72 a `B(P) = {16, 21}`.

- Branch 16 : couvert par AC-03 (trottinette électrique, 16 ans → éligible).
- Branch 21 : couvert par AC-01 (voiture, 20 ans → refusé) et AC-02 (voiture, 21 ans → éligible).

`A(P) == B(P)`. Taille combinatoire = 3 cas, bien en dessous du seuil de 10–15.

**M4 negative — saturated by AC** : aucun test de domaine dédié autorisé pour `Vehicle.MinimumAge()`.

---

## Walking Skeleton Strategy

**Strategy A — Full InMemory** : la feature ne persiste aucune donnée. `CheckEligibilityQueryHandler` utilise `EligibilityPolicy` (domain service pur) et `TimeProvider`. Pas de repository.

---

## Mise à jour des tests existants (non-régression)

Les tests suivants encodent l'ancienne valeur 18 et devront être mis à jour par le software-engineer pour refléter la nouvelle règle 21 :

| Fichier | Test | Action |
|---|---|---|
| `tests/.../Application/CheckEligibilityQueryHandlerTests.cs` | `Handle_WhenDriverIs18AndHasCar_ReturnsEligible` | Changer l'attendu à `IsEligible = false` et ajouter `RejectionReason` |
| `tests/.../Domain/EligibilityPolicyTests.cs` | `Evaluate_WhenDriverTurns18ExactlyToday_ReturnsAccepted` | Changer le prédicat attendu à `refused` |
| `tests/.../Domain/EligibilityPolicyTests.cs` | `Evaluate_WhenDriverTurns18Tomorrow_ReturnsRefused` | Vérifier si ce test reste valide (18 ans demain → refusé — reste vrai) |
