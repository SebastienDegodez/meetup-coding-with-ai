# AC Draft — STORY-69-B : Consultation du tarif d'assurance

## Story

**ID:** STORY-69-B  
**GitHub Issue:** #69  
**Milestone:** v0.3-souscription

---

### User Story

> En tant que conducteur éligible avec une demande de souscription active,  
> je veux recevoir un tarif d'assurance,  
> afin d'évaluer le coût et décider si je souhaite finaliser ma police d'assurance.

---

## Domain Examples

1. **Marie Durand**, 32 ans, demande SUB-2026-001, Renault Clio (véhicule standard) → reçoit un tarif de **45 €/mois** avec une couverture de base (tiers)
2. **Karim Benali**, 28 ans, demande SUB-2026-002, Peugeot 308 GTI (puissance > 100 cv, catégorie moto haute puissance equivalent) → reçoit un tarif de **85 €/mois** avec surprime véhicule haute performance
3. **Sophie Martin**, 45 ans, demande SUB-2026-003, Ford Focus (profil senior, 0 accident) → reçoit un tarif de **32 €/mois** tarif fidélité conducteur expérimenté
4. **Lucas Petit**, 23 ans, **sans demande de souscription active**, tente d'obtenir un tarif → refus avec le motif "aucune demande de souscription active"

---

## Acceptance Criteria

### AC-01 — Obtention du tarif pour un véhicule standard (happy path)

*Tracé depuis : exemple 1*

```
Given a driver with an active subscription request for a standard vehicle
When the driver requests an insurance price quote
Then the driver receives a price quote with the monthly amount
And the coverage type is indicated
```

---

### AC-02 — Tarif avec surprime pour véhicule haute puissance

*Tracé depuis : exemple 2*

```
Given a driver with an active subscription request for a high-power vehicle
When the driver requests an insurance price quote
Then the driver receives a price quote reflecting the high-power vehicle risk premium
And the monthly amount is higher than the standard base rate
```

---

### AC-03 — Refus si aucune demande de souscription active

*Tracé depuis : exemple 4*

```
Given a driver without an active subscription request
When the driver attempts to request an insurance price quote
Then the request is rejected
And the driver receives a reason: "no active subscription request found"
```

---

## Technical Notes

- Cette story dépend de STORY-69-A : la demande de souscription (`SubscriptionRequest`) doit être créée avant que le tarif puisse être calculé.
- Le calcul du tarif est une nouvelle fonctionnalité dans le contexte `Subscription` (à concevoir en DESIGN). Il n'existe pas encore de service de tarification dans la codebase.
- Les règles de tarification doivent prendre en compte : type de véhicule (puissance), profil conducteur (âge, historique accidents). Les valeurs exactes sont à confirmer en DESIGN.
- La véhicule de type "haute puissance" (`power > 100 hp`) existe déjà dans `VehicleType` / domaine Eligibility. La même notion doit s'appliquer à la tarification.
- Aucune intégration externe requise pour cette story (calcul in-process).

---

## DoR Checklist

| Item | Statut | Note |
|---|---|---|
| 1. Problem statement | ✅ PASS | Conducteur éligible avec demande de souscription ne peut pas évaluer le coût sans cette story |
| 2. Specific persona | ✅ PASS | "Conducteur éligible avec une demande de souscription active" — rôle nommé |
| 3. 3+ domain examples | ✅ PASS | 4 exemples avec valeurs réelles (noms, âges, véhicules, tarifs chiffrés) |
| 4. UAT scenarios | ✅ PASS | 3 scénarios Given/When/Then validables par un non-technicien |
| 5. AC derived from UAT | ✅ PASS | Chaque AC trace explicitement un exemple domain |
| 6. Right-sized | ✅ PASS | M = 1 jour — livrable en solo en 1 journée |
| 7. Technical notes | ✅ PASS | Dépendance STORY-69-A, nouveau service de tarification, règles véhicule haute puissance |
| 8. Dependencies | ✅ PASS | STORY-69-A listée et dans le même sprint |

**DoR Verdict: ✅ READY FOR DESIGN**

---

## Antipattern Scan

| Antipattern | Détecté | Note |
|---|---|---|
| AP-DISCUSS-01 — Implement-X | ❌ Non | Story décrit un besoin utilisateur |
| AP-DISCUSS-02 — Generic Data | ❌ Non | Exemples avec tarifs chiffrés (45€, 85€, 32€), noms, âges |
| AP-DISCUSS-03 — Technical AC | ❌ Non | ACs en termes métier exclusivement |
| AP-DISCUSS-04 — Giant Stories | ❌ Non | 3 ACs, M = 1 jour |
| AP-DISCUSS-05 — No Examples | ❌ Non | 4 exemples domain présents |
| AP-DISCUSS-06 — Tests After Code | ❌ Non | ACs fondés sur état métier |
| AP-DISCUSS-07 — Vague Persona | ❌ Non | Persona spécifique nommée |
| AP-DISCUSS-08 — Missing Dependencies | ❌ Non | STORY-69-A explicitement listée comme dépendance |

**Aucun antipattern détecté.**
