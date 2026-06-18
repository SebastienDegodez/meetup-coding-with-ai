# Implementation Plan — STORY-72 : Âge légal de conduite porté à 21 ans

**Story:** story-72
**Date:** 2026-06-03
**Strategy:** Outside-In TDD — Walking Skeleton A (Full InMemory / pure domain)

---

## Step 1 — Acceptance test (Application layer) — OUTER LOOP RED

- **Fichier :** `tests/MonAssurance.UnitTests/Eligibility/Application/LegalAgeChangeAcceptanceTests.cs`
- **Enters through :** `CheckEligibilityQueryHandler.Handle(CheckEligibilityQuery)`
- **Double :** aucun repository (domain pur) ; `FakeTimeProvider` pour contrôler `today`
- **Objectif :** les scénarios AC-01 et AC-04 doivent échouer sur une assertion métier (age 20 / age 18 accepté alors qu'il devrait être refusé).

## Step 2 — Correction du domaine (production code)

- **Fichier :** `src/MonAssurance.Domain/Eligibility/Vehicle.cs`
- **Modification :** `MinimumAge()` retourne `21` au lieu de `18` pour les véhicules non-trottinette.
- **Gated by :** RED de Step 1.

## Step 3 — Mise à jour des tests existants de régression

- **Fichiers :**
  - `tests/MonAssurance.UnitTests/Eligibility/Application/CheckEligibilityQueryHandlerTests.cs`
    - `Handle_WhenDriverIs18AndHasCar_ReturnsEligible` → renommer et inverser l'attendu : `IsEligible = false`, `RejectionReason = "Conducteur trop jeune pour ce véhicule"`
  - `tests/MonAssurance.UnitTests/Eligibility/Domain/EligibilityPolicyTests.cs`
    - `Evaluate_WhenDriverTurns18ExactlyToday_ReturnsAccepted` → changer le prédicat en `refused` avec la raison attendue

- **Objectif :** suite complète verte après Step 2.

## Step 4 — Intégration / API (non requis pour cette story)

Aucune modification d'endpoint ou de persistence. Le changement est purement domain.
Vérifier que `tests/MonAssurance.IntegrationTests/Eligibility/EligibilityEndpointsTests.cs` reste vert après Step 2 (pas de test d'âge 18 → éligible supposé dans les tests d'intégration).

---

## Ordre d'implémentation (outside-in)

```
RED   → Step 1 : tests d'acceptation échouent sur assertion métier
GREEN → Step 2 : Vehicle.MinimumAge() retourne 21
      + Step 3 : tests existants mis à jour
VERIFY → Step 4 : vérification des tests d'intégration
```
