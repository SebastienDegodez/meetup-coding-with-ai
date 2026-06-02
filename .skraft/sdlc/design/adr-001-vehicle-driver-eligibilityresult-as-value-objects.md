# ADR-001 : Classification de Vehicle, Driver et EligibilityResult comme Value Objects

**Status:** Proposed
**Date:** 2026-06-02

## Context

Story-72 modifie la règle `Vehicle.MinimumAge()` (18 → 21 ans pour les véhicules non-trottinette). Avant d'écrire le contrat d'interface, il faut ratifier la classification DDD de `Vehicle`, `Driver` et `EligibilityResult` dans le bounded context `EligibilityContext`.

Trois questions se posent :

1. `Vehicle` : a-t-il une **identité persistée** qui justifie un `Entity` ou un `Aggregate Root` ? Ou est-il entièrement défini par son type (`VehicleType`) et sa puissance (`power`) ?
2. `Driver` : a-t-il une identité métier qui le rend unique au-delà de ses attributs (date de naissance, années de permis) dans ce contexte ?
3. `EligibilityResult` : est-il un état mutable à persister, ou un résultat de calcul immuable ?

Forces observées dans le code existant :
- `Vehicle` est construit à la volée depuis la requête ; aucun identifiant persisté ; `MinimumAge()` est un calcul pur sur le type.
- `Driver` est construit à la volée depuis la requête ; `Age()` et `HasEnoughExperience()` sont des calculs purs sur les attributs.
- `EligibilityResult` encapsule `Accepted | Refused(reason)` ; pas de cycle de vie, pas d'identifiant.
- Aucun repository n'est déclaré pour ces trois concepts.
- Story-72 ne demande pas de persister l'historique des vérifications (pas d'audit trail, pas de temporal query).

## Decision

Nous classifions `Vehicle`, `Driver` et `EligibilityResult` comme **Value Objects** dans `EligibilityContext`. Ces trois concepts sont immuables, définis intégralement par leurs attributs, sans identité persistée. `Vehicle.MinimumAge()` et `Driver.Age()` sont des comportements purs du Value Object, non des invariants d'un Aggregate Root.

## Consequences

**Positive :**
- Modèle de domaine simple : pas de gestion de cycle de vie, pas de repository pour ces trois concepts.
- `Vehicle.MinimumAge()` peut être modifié sans impacter de contrat d'Aggregate.
- Testabilité maximale : instanciation directe dans les tests unitaires.

**Negative / trade-offs :**
- Si une future story introduit un identifiant persisté pour `Vehicle` (ex : numéro de plaque, historique assurance), cette décision devra être révisée et un ADR de supersession créé.
- Si une story demande un audit trail des vérifications d'éligibilité par conducteur, `Driver` devra être promu en Entity ou Aggregate Root.

**Neutral :**
- Les trois Value Objects restent dans la couche Domain (`MonAssurance.Domain.Eligibility`).

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| `Vehicle` en tant qu'`Entity` | Aucun identifiant persisté requis par story-72 ; le type et la puissance suffisent à définir le concept dans ce contexte. Un Entity sans identité persistée est un anti-pattern. |
| `Driver` en tant qu'`Aggregate Root` | Story-72 n'introduit aucun invariant à protéger au niveau `Driver` ; la règle `MinimumAge` appartient à `Vehicle`, pas à `Driver`. Aucune commande ne mute l'état de `Driver`. |
| `EligibilityResult` en tant que `Domain Event` | `EligibilityResult` est le retour d'un calcul synchrone, non un fait enregistré dans un journal d'événements. Aucun story ne demande de rejouer ou de persister les résultats. |
