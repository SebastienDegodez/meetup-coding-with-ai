# AC Draft — STORY-69-A : Initiation de la demande de souscription

## Story

**ID:** STORY-69-A  
**GitHub Issue:** #69  
**Milestone:** v0.3-souscription

---

### User Story

> En tant que conducteur éligible ayant reçu une confirmation d'éligibilité,  
> je veux soumettre une demande de souscription,  
> afin de formaliser ma démarche d'obtention d'une couverture d'assurance.

---

## Domain Examples

1. **Marie Durand**, 32 ans, éligibilité confirmée (Renault Clio, B), soumet une demande de souscription → une demande est créée avec la référence `SUB-2026-001`
2. **Karim Benali**, 28 ans, éligibilité confirmée (Peugeot 308, B), soumet une demande de souscription → une demande est créée avec la référence `SUB-2026-002`
3. **Sophie Martin**, 45 ans, éligibilité confirmée (Ford Focus, B), soumet une demande de souscription → demande créée et confirmation immédiate que la demande est en cours de traitement
4. **Lucas Petit**, 23 ans, **sans confirmation d'éligibilité**, tente de soumettre une demande de souscription → la demande est refusée avec le motif "éligibilité non confirmée"

---

## Acceptance Criteria

### AC-01 — Création de la demande de souscription (happy path)

*Tracé depuis : exemple 1 & 2*

```
Given a driver whose eligibility has been confirmed
When the driver submits a subscription request
Then a subscription request is created
And the driver receives a unique subscription reference number
```

---

### AC-02 — Demande déjà existante (idempotence)

*Tracé depuis : exemple 2 (re-soumission)*

```
Given a driver whose eligibility has been confirmed
And the driver has already submitted a subscription request
When the driver submits a new subscription request
Then the existing subscription request is returned
And no duplicate subscription request is created
```

---

### AC-03 — Refus si éligibilité non confirmée

*Tracé depuis : exemple 4*

```
Given a driver who has not received eligibility confirmation
When the driver attempts to submit a subscription request
Then the subscription request is rejected
And the driver receives a reason: "subscription requires prior eligibility confirmation"
```

---

## Technical Notes

- La fonctionnalité d'éligibilité (`CheckEligibilityQuery`) est déjà implémentée dans `MonAssurance.Application.Eligibility.Queries.CheckEligibility`.
- Le résultat d'éligibilité est modélisé via `EligibilityResult` (domain) avec les états `Accepted` et `Refused`.
- La nouvelle fonctionnalité requiert un nouveau contexte `Subscription` dans le domaine (aggregate ou entité à confirmer en DESIGN).
- La référence de souscription doit être unique et reproductible (ex. : UUID ou séquence préfixée).
- Aucune exigence de performance spécifique pour cette story (charge < 10 req/s anticipée).

---

## DoR Checklist

| Item | Statut | Note |
|---|---|---|
| 1. Problem statement | ✅ PASS | Conducteur éligible ne peut pas formaliser sa démarche sans cette story |
| 2. Specific persona | ✅ PASS | "Conducteur éligible ayant reçu une confirmation d'éligibilité" — rôle nommé |
| 3. 3+ domain examples | ✅ PASS | 4 exemples avec valeurs réelles (noms, âges, véhicules, références) |
| 4. UAT scenarios | ✅ PASS | 3 scénarios Given/When/Then validables par un non-technicien |
| 5. AC derived from UAT | ✅ PASS | Chaque AC trace explicitement un exemple domain |
| 6. Right-sized | ✅ PASS | M = 1 jour — livrable en solo en 1 journée |
| 7. Technical notes | ✅ PASS | Notes sur l'intégration avec EligibilityResult, nouveau contexte Subscription |
| 8. Dependencies | ✅ PASS | Aucune dépendance externe non résolue (éligibilité déjà implémentée) |

**DoR Verdict: ✅ READY FOR DESIGN**

---

## Antipattern Scan

| Antipattern | Détecté | Note |
|---|---|---|
| AP-DISCUSS-01 — Implement-X | ❌ Non | Story décrit un besoin utilisateur, pas une implémentation |
| AP-DISCUSS-02 — Generic Data | ❌ Non | Exemples avec noms, âges, véhicules réels |
| AP-DISCUSS-03 — Technical AC | ❌ Non | ACs en termes métier, aucune référence HTTP/classe |
| AP-DISCUSS-04 — Giant Stories | ❌ Non | 3 ACs, M = 1 jour |
| AP-DISCUSS-05 — No Examples | ❌ Non | 4 exemples domain présents |
| AP-DISCUSS-06 — Tests After Code | ❌ Non | ACs fondés sur état métier, pas sur implémentation |
| AP-DISCUSS-07 — Vague Persona | ❌ Non | Persona spécifique nommée |
| AP-DISCUSS-08 — Missing Dependencies | ❌ Non | Dépendances listées et résolues |

**Aucun antipattern détecté.**
