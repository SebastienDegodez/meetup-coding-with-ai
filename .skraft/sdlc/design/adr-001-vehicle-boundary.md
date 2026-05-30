# ADR-001 : Frontière de Vehicle — Value Object (pas Aggregate Root)

**Status:** Proposed
**Date:** 2026-05-30

## Context

Dans le domaine de l'assurance auto, un véhicule a généralement une identité persistante (immatriculation, VIN). Cependant, dans le contexte de la vérification d'éligibilité (story-57 et tous les cas d'usage actuels), `Vehicle` est construit à la demande comme porteur de règles métier, sans cycle de vie, sans identité stable et sans événements propres.

La méthode `MinimumAge()` est une règle calculée à partir du type de véhicule. STORY-57 consiste à mettre à jour cette règle (18 → 21 ans pour les véhicules standards), ce qui soulève la question : doit-on traiter `Vehicle` comme un Aggregate Root (avec identité, lifecycle et événements) ou comme un Value Object (immuable, égalité par valeur, auto-validant) ?

Forces :
- **F1** : Aucun AC de story-57 ne requiert la persistance ou le suivi d'un véhicule individuel avec identité.
- **F2** : `Vehicle` n'a aucun invariant nécessitant une frontière de cohérence temporelle.
- **F3** : `Vehicle` est reconstruit à chaque appel `CheckEligibility` — il n'y a pas d'état à protéger entre deux appels.
- **F4** : Aucune story du milestone `v0.2-legal-age` n'introduit de mutation d'état sur un véhicule identifié.

## Decision

Nous classifions `Vehicle` comme **Value Object** dans le contexte borné `EligibilityContext`. Il est immuable, défini par ses attributs (`VehicleType`, `Power`) et sa méthode `MinimumAge()` est une règle métier pure sans effet de bord.

## Consequences

**Positif :**
- Modèle simple, sans overhead d'identité ni de repository dédié.
- `Vehicle` est sûr d'accès concurrent (immuabilité).
- Cohérence avec le pattern CQS en vigueur : pas de persistance, pas d'agrégat.

**Négatif / compromis :**
- Si une story future introduit un suivi de véhicule (historique, renouvellement), `Vehicle` devra être promu Aggregate Root et cet ADR sera supersedé.
- Les invariants liés à l'identité du véhicule (ex. : un même véhicule ne peut être assuré deux fois) ne peuvent pas être portés par ce Value Object.

**Neutre :**
- `VehicleType` reste une énumération (Car, Motorcycle, ElectricScooter).

## Alternatives Rejected

| Alternative | Raison rejetée |
|---|---|
| `Vehicle` comme Aggregate Root | Aucune story du batch actuel n'introduit d'identité, de lifecycle ou d'événements pour `Vehicle`. Ajouter un Aggregate Root sans invariant à protéger violerait YAGNI. |
| `Vehicle` comme Entity | Une Entity requiert une identité sans cycle de vie complet (ex. référence stable). Aucune story ne référence un véhicule par identifiant entre deux requêtes. |
