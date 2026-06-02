# ADR-003: PricingPolicy comme Domain Service

**Status:** Proposed
**Date:** 2026-06-02

## Context

STORY-69-B requiert le calcul d'un tarif d'assurance basé sur les caractéristiques du véhicule (type, puissance) et le profil du conducteur (âge, historique). Ce calcul :

1. **N'appartient pas à un agrégat** : le tarif est calculé à partir des données de `SubscriptionRequest` (qui contient `VehicleDetails`) mais le résultat (`PriceQuote`) n'est pas persisté comme état de l'agrégat — il est calculé à la demande (AC-01, AC-02 ne mentionnent pas de stockage du tarif).
2. **Est une logique métier significative** : les règles de tarification (surprime haute puissance, profil senior) constituent une logique de domaine qui ne peut pas résider dans la couche Application sans violer les principes de Clean Architecture.
3. **Est sans état** : le calcul du tarif ne nécessite pas de cycle de vie ni d'identité propre. Il prend des paramètres en entrée et retourne un résultat en sortie.
4. **N'est pas un Value Object** : un Value Object est une valeur immutable qui encapsule son propre état. `PricingPolicy` opère sur des objets externes (VehicleDetails, profil conducteur) et n'a pas d'état à encapsuler.

Le pattern **Domain Service** est le choix canonique pour la logique de domaine significative qui s'applique sur plusieurs entités ou agrégats et qui est sans état. La codebase existante utilise `EligibilityPolicy` avec ce même pattern pour la logique d'éligibilité.

La classification provisoire du trigger `GetInsurancePriceQuote` est **Query** — elle lit l'état de `SubscriptionRequest`, invoque `PricingPolicy.Calculate(...)`, et retourne un `InsurancePriceQuote` Read Model sans muter d'état. Cette classification est ratifiée par ce ADR.

## Decision

Nous classerons `PricingPolicy` comme **Domain Service** dans `MonAssurance.Domain.Subscription`. Il prend en paramètre `VehicleDetails` et `DriverProfile` (Value Objects) et retourne `PriceQuote` (Value Object immutable avec `MonthlyAmount` et `CoverageType`).

Le trigger `GetInsurancePriceQuote` est classifié **Query** : il lit `SubscriptionRequest` via `ISubscriptionRequestRepository`, appelle `PricingPolicy`, et retourne `InsurancePriceQuote` (Read Model). Aucune mutation d'état ne se produit.

## Consequences

**Positive:**
- La logique de tarification reste dans le domaine et peut évoluer sans toucher la couche Application.
- Le pattern est cohérent avec `EligibilityPolicy` déjà en place — les développeurs reconnaissent immédiatement le rôle de `PricingPolicy`.
- `PriceQuote` comme Value Object immutable est testable de façon isolée.

**Negative / trade-offs:**
- Les règles de tarification (montants, seuils) sont hardcodées dans `PricingPolicy` à ce stade. Une évolution vers une tarification configurable nécessitera une refactorisation.
- Le Domain Service ne peut pas émettre de Domain Events directement (il est stateless). Si une future story exige de tracer les calculs de tarif, il faudra revisiter cette décision.

**Neutral:**
- `PricingPolicy` sera enregistré comme `Singleton` dans le conteneur DI (stateless, même pattern que `EligibilityPolicy`).

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| Logique de tarification dans le handler Application | Violerait le principe de Clean Architecture : la logique métier doit résider dans le domaine, pas dans la couche Application. |
| `PricingPolicy` comme méthode sur `SubscriptionRequest` (Aggregate Root) | L'agrégat ne devrait pas contenir la logique de calcul du tarif, qui dépend de règles métier indépendantes du cycle de vie de la demande. Violerait SRP au niveau de l'agrégat. |
| `PriceQuote` comme Aggregate Root | Le tarif calculé n'a pas d'invariants à protéger ni de cycle de vie propre — c'est un résultat de calcul immutable. Un Aggregate Root serait du sur-ingénierie. |
| CQRS dispatch bus pour `GetInsurancePriceQuote` | Aucune asymétrie read/write ne justifie un bus de dispatch à ce stade. La charge anticipée est < 10 req/s (spécifiée dans les notes techniques STORY-69-A). Un handler direct suit la convention existante de la codebase. |
