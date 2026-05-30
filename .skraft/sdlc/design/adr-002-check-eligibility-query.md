# ADR-002 : CheckEligibility — Query (pas Command)

**Status:** Proposed
**Date:** 2026-05-30

## Context

La vérification d'éligibilité (story-57) consiste à calculer si un conducteur est éligible à l'assurance d'un véhicule. La question se pose : cette opération est-elle une **Query** (retourne un résultat sans persister d'état) ou un **Command** (mute un état et/ou publie un événement de domaine) ?

Forces :
- **F1** : Aucun AC de story-57 ne mentionne la persistance du résultat d'éligibilité.
- **F2** : La réponse du moteur (`EligibilityPolicy.Evaluate`) est une fonction pure — mêmes entrées, mêmes sorties, sans effet de bord.
- **F3** : L'implémentation actuelle (`CheckEligibilityQuery` / `CheckEligibilityQueryHandler`) suit déjà le pattern CQS : pas de write path.
- **F4** : Aucune story du milestone `v0.2-legal-age` n'introduit d'exigence d'audit trail ou de traçabilité du résultat.

La classification Query respecte le principe CQS (Command-Query Separation) en vigueur dans le projet.

## Decision

Nous classifions `CheckEligibility` comme **Query**. L'opération retourne un `EligibilityViewModel` (Read Model) sans persister d'état ni émettre d'événement de domaine. Le handler (`CheckEligibilityQueryHandler`) délègue à `EligibilityPolicy` (Domain Service) pour le calcul.

## Consequences

**Positif :**
- Opération idempotente et sûre à répéter (pas d'effet de bord).
- Testabilité maximale : entrées → sortie déterministe, sans mock de repository.
- Aligné avec le pattern CQS du projet.

**Négatif / compromis :**
- Si une story future demande la traçabilité des vérifications (audit, conformité réglementaire), l'opération devra être reprofilée en Command (ou un Event sera publié en parallèle), et cet ADR sera supersedé.
- Aucun événement `EligibilityChecked` n'est produit — intégration event-driven impossible tant que cette classification tient.

**Neutre :**
- `EligibilityViewModel` reste un Read Model propre à la couche Application, sans lien direct avec le Domain Model.

## Alternatives Rejected

| Alternative | Raison rejetée |
|---|---|
| Command avec persistance du résultat | Aucune story du batch ne demande de mémoriser les résultats de vérification. Ajouter un write path sans exigence identifiée violerait YAGNI et alourdit le modèle sans valeur ajoutée. |
| Command + Domain Event `EligibilityChecked` | L'émission d'un événement de domaine nécessiterait un Aggregate Root pour l'encapsuler. Aucun invariant à protéger ne justifie cet Aggregate Root (cf. ADR-001). |
