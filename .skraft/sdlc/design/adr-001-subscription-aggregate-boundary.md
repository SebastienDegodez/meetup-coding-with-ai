# ADR-001: SubscriptionRequest comme Aggregate Root

**Status:** Proposed
**Date:** 2026-06-02

## Context

STORY-69-A introduit la notion de "demande de souscription". Cette entité possède :

1. **Une identité** : la référence unique `SUB-YYYY-NNN` (AC-01 — "the driver receives a unique subscription reference number").
2. **Un invariant d'éligibilité** : la demande ne peut être créée que si le conducteur a une confirmation d'éligibilité (AC-03 — "subscription requires prior eligibility confirmation"). Cet invariant est une règle de gard qui doit être appliquée à chaque tentative de création.
3. **Un invariant d'idempotence** : si une demande existe déjà pour ce conducteur, la re-soumission retourne la demande existante sans créer de doublon (AC-02). La frontière de cohérence s'étend donc sur l'existence d'une demande par conducteur.
4. **Un cycle de vie** : la demande est créée et peut évoluer (consultation de tarif en STORY-69-B).

Ces trois invariants (identité, garde d'éligibilité, unicité par conducteur) constituent une frontière de cohérence non triviale. Un simple Value Object (sans identité ni cycle de vie) ne peut pas gérer ces invariants. Une entité sans racine d'agrégat ne peut pas garantir la cohérence sur plusieurs opérations.

La codebase existante dans `EligibilityContext` utilise `Driver` et `Vehicle` comme Value Objects (pas d'identité propre). `SubscriptionRequest` ne peut pas être modélisé de la même façon : il faut une identité durable et un cycle de vie géré.

## Decision

Nous modéliserons `SubscriptionRequest` comme un **Aggregate Root** dans le `SubscriptionContext`. La racine détient :
- `SubscriptionReference` (Value Object) — référence unique de type `SUB-YYYY-NNN`
- `DriverId` (Value Object) — référence opaque au conducteur, issue de l'EligibilityContext
- `VehicleDetails` (Value Object) — type de véhicule et puissance (copie locale dans le contexte Subscription)
- `status` — statut interne de la demande

L'invariant d'éligibilité est contrôlé par la couche Application avant la création de l'agrégat (le handler vérifie `EligibilityResult.IsEligible` via le contexte Eligibility, puis délègue à l'agrégat uniquement si éligible). L'agrégat émet `SubscriptionRequestInitiated` à la création.

La classification provisoire du trigger `InitiateSubscriptionRequest` est **Command** — elle mute l'état et lève un événement de domaine. Cette classification est ratifiée par ce ADR.

## Consequences

**Positive:**
- Les invariants (unicité, éligibilité, cycle de vie) sont encapsulés dans l'agrégat et ne peuvent pas être violés depuis l'extérieur.
- Le domaine peut évoluer (ajout de statuts, de règles) sans impacter les autres contextes.
- `SubscriptionRequestInitiated` comme Domain Event permet une intégration future event-driven.

**Negative / trade-offs:**
- L'agrégat introduit une complexité supplémentaire par rapport à un simple enregistrement CRUD. Justifiée par les trois invariants.
- La vérification d'éligibilité dans la couche Application crée un couplage Application → EligibilityContext. Un ACL sera nécessaire pour isoler ce couplage (voir ADR-002).

**Neutral:**
- `ISubscriptionRequestRepository` doit être défini dans Application et implémenté dans Infrastructure (convention de la codebase existante).

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| `SubscriptionRequest` comme Entity sans Aggregate Root | Pas de frontière de cohérence définie — l'invariant d'unicité par conducteur ne peut pas être garanti sans une racine qui contrôle l'accès. |
| `SubscriptionRequest` comme Value Object | Un Value Object ne possède pas d'identité (`SubscriptionReference`) ni de cycle de vie. Incompatible avec AC-01 (référence unique) et AC-02 (idempotence). |
| Vérification d'éligibilité dans l'agrégat lui-même | L'agrégat ne doit pas dépendre d'un autre contexte borné (EligibilityContext). La vérification doit rester dans la couche Application pour respecter les frontières de contexte. |
