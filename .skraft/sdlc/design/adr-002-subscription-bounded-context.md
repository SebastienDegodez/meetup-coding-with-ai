# ADR-002: SubscriptionContext comme contexte borné séparé

**Status:** Proposed
**Date:** 2026-06-02

## Context

STORY-69-A et STORY-69-B introduisent un domaine de souscription avec son propre langage ubiquitaire (`SubscriptionRequest`, `SubscriptionReference`, `PriceQuote`) distinct du langage du domaine d'éligibilité (`EligibilityResult`, `Driver`, `Vehicle`, `EligibilityPolicy`).

La codebase existante contient un unique contexte `Eligibility` avec ses entités de domaine. Ajouter les concepts de souscription dans ce même contexte poserait plusieurs problèmes :
- Le modèle `Driver` actuel (Value Object sans identité) ne convient pas pour la souscription où un conducteur doit être référencé de façon stable.
- `EligibilityPolicy` est une règle de gard en entrée de souscription, non une règle interne à la souscription.
- Les invariants de souscription (unicité de demande par conducteur, calcul de tarif) ne partagent aucune frontière de cohérence avec les règles d'éligibilité.

La séparation en deux contextes bornés respecte le principe de responsabilité unique au niveau du domaine et prévient la contamination du modèle d'éligibilité par des préoccupations de souscription.

La relation entre les deux contextes est **Conformist** : le `SubscriptionContext` consomme le résultat de l'éligibilité tel qu'il est produit par l'`EligibilityContext` sans le transformer (pas d'Anti-Corruption Layer nécessaire à ce stade, le format `EligibilityViewModel` est déjà lisible en dehors du contexte). Un ACL peut être ajouté si le modèle `EligibilityContext` évolue de façon incompatible.

## Decision

Nous créerons un **nouveau contexte borné `SubscriptionContext`** avec sa propre organisation de namespaces (`MonAssurance.Domain.Subscription`, `MonAssurance.Application.Subscription.*`). Le `SubscriptionContext` est classifié comme **subdomain Core** (logique métier différenciante de MonAssurance).

La relation avec `EligibilityContext` est **Upstream/Downstream — Conformist** : le `SubscriptionContext` (Downstream) se conforme au modèle publié par `EligibilityContext` (Upstream) sans couche d'isolation.

## Consequences

**Positive:**
- Les deux contextes évoluent indépendamment. Une modification du modèle d'éligibilité n'affecte pas la logique de souscription tant que le contrat de sortie (`EligibilityViewModel.IsEligible`) est stable.
- Le langage ubiquitaire de chaque contexte reste cohérent et non pollué.
- Le `SubscriptionContext` peut être extrait en service indépendant dans le futur sans refactoring massif.

**Negative / trade-offs:**
- La vérification d'éligibilité dans le handler de souscription introduit un couplage entre les couches Application des deux contextes. Ce couplage est accepté car la condition (eligibility confirmée) est une règle de gard et non une règle métier interne.
- La duplication de certains concepts (ex. `VehicleType` référencé dans les deux contextes) est inévitable avec la séparation en contextes bornés. La copie locale dans `SubscriptionContext` est préférable à une dépendance directe sur le domaine Eligibility.

**Neutral:**
- Les namespaces de `SubscriptionContext` suivent la convention existante : `MonAssurance.Domain.Subscription`, `MonAssurance.Application.Subscription.Commands.*`, `MonAssurance.Application.Subscription.Queries.*`.

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| Ajouter les concepts de souscription dans `EligibilityContext` | Contaminerait le modèle d'éligibilité avec des responsabilités de souscription. L'agrégat `SubscriptionRequest` n'a aucun lien de cohérence avec `EligibilityPolicy`. |
| Anti-Corruption Layer (ACL) entre Subscription et Eligibility | L'interface `EligibilityViewModel` est simple et stable (`IsEligible: bool`). Un ACL est du sur-ingénierie à ce stade. Peut être ajouté si le contrat évolue. |
| Subdomain Supporting ou Generic | La souscription est une capacité centrale de MonAssurance (logique différenciante). Classification Core est correcte. |
