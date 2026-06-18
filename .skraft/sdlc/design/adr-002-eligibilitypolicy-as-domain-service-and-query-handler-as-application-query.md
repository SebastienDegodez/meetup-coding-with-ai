# ADR-002 : EligibilityPolicy comme Domain Service et CheckEligibilityQueryHandler comme Application Query

**Status:** Proposed
**Date:** 2026-06-02

## Context

Story-72 modifie la règle `Vehicle.MinimumAge()` au sein d'une politique d'éligibilité qui coordonne plusieurs Value Objects (`Vehicle`, `Driver`). Deux questions de classification se posent :

1. `EligibilityPolicy` : est-ce un comportement appartenant à un Aggregate Root, ou une règle métier transverse qui coordonne plusieurs Value Objects sans posséder d'état persisté propre ?
2. `CheckEligibilityQueryHandler` : la vérification d'éligibilité mute-t-elle un état, ou est-ce une requête pure qui retourne un résultat calculé ?

Forces observées :
- `EligibilityPolicy.Evaluate(Driver, Vehicle, DateOnly)` combine les règles de deux Value Objects. Elle ne possède aucun état persisté et n'émet aucun Domain Event.
- `CheckEligibilityQueryHandler.Handle(query)` construit les Value Objects depuis la requête et délègue à `EligibilityPolicy` ; aucune persistance n'est déclenchée, aucun événement n'est levé.
- Story-72 ne demande ni persistance de résultats, ni audit trail, ni séparation store lecture/écriture.

## Decision

Nous classifions :
- `EligibilityPolicy` comme **Domain Service** dans la couche `Domain` : règle métier transverse coordonnant `Vehicle` et `Driver`, sans état propre.
- `CheckEligibilityQueryHandler` comme **Application Query** dans la couche `Application` : orchestration de la Query sans mutation d'état. L'interface handler (port) est déclarée dans `Application`, conformément à Clean Architecture (l'Application déclare les ports qu'elle orchestre).

## Consequences

**Positive :**
- `EligibilityPolicy` est testable unitairement sans base de données ni repository.
- La séparation Domain / Application reste nette : la règle (`EligibilityPolicy`) est domain-pure ; l'orchestration (`CheckEligibilityQueryHandler`) est application-layer.
- Story-72 se réduit à modifier une ligne dans `Vehicle.MinimumAge()` — aucun contrat d'Aggregate ne change.

**Negative / trade-offs :**
- Si une future story demande de tracer chaque vérification d'éligibilité (audit log), `CheckEligibilityQueryHandler` devra être étendu pour déclencher un Domain Event, et `EligibilityPolicy` pourrait évoluer vers un comportement d'Aggregate Root.

**Neutral :**
- L'injection de `EligibilityPolicy` dans le handler reste par DI conventionnelle (pas de bus de commande/query).

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| `EligibilityPolicy` en tant que comportement d'un `EligibilityAggregate` | Aucun invariant à protéger via un Aggregate Root : story-72 ne demande pas de persister d'état ni d'émettre de Domain Event. Créer un Aggregate sans état persisté violerait YAGNI et alourdirait inutilement le modèle. |
| CQRS avec bus de commande/query | Story-72 n'introduit ni read/write asymmetry nécessitant des stores séparés, ni audit trail, ni seuil de charge mesuré ou projeté ≥ 100 req/s. L'absence de force admissible (per ADR quality gate §PATTERN-NECESSITY) conduit au rejet du bus CQRS. CQS au niveau méthode reste la baseline non-ADR. |
| `CheckEligibilityQueryHandler` en Command (mutation) | La vérification d'éligibilité ne mute aucun état persisté ; la classer Command serait sémantiquement incorrect et enfreindrait CQS. |
