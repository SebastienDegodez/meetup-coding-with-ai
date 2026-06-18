# AC Draft — STORY-72 : Âge légal de conduite porté à 21 ans

## Story

```
En tant que conducteur déposant une demande d'assurance automobile,
je veux que le système d'éligibilité applique le nouvel âge légal minimum de 21 ans,
afin de recevoir une décision conforme à la loi française en vigueur.
```

**Persona :** Conducteur déposant une demande d'assurance automobile `[PERSONA INFERRED]`
**Effort :** S (0.5 jour)
**Priorité MoSCoW :** Must Have
**Issue GitHub :** #72

---

## Domain Examples

1. **Lucas Martin**, 20 ans, permis B valide depuis 2 ans, véhicule de type voiture (non trottinette) → **refusé** — trop jeune selon la nouvelle loi (âge < 21)
2. **Emma Dupont**, 21 ans, permis B valide depuis 3 ans, véhicule de type voiture → **accepté** — âge légal atteint
3. **Théo Bernard**, 16 ans, trottinette électrique → **accepté** — l'âge légal spécifique à la trottinette électrique (16 ans) reste inchangé
4. **Sophie Lefebvre**, 19 ans, permis B valide depuis 1 an, moto < 100 cv → **refusé** — trop jeune (âge < 21)
5. **Karim Diallo**, 21 ans exactement aujourd'hui, permis B valide, véhicule de type voiture → **accepté** — cas limite : exactement 21 ans est valide (≥ 21)

---

## Acceptance Criteria

### AC-01 — Refus d'un conducteur de moins de 21 ans pour un véhicule standard
*(Source : exemples domaine 1 & 4)*

```gherkin
Given un conducteur âgé de 20 ans avec un permis B valide
And un véhicule de type voiture (non trottinette électrique)
When le conducteur soumet une demande de vérification d'éligibilité
Then le conducteur est déclaré non éligible
And le motif de refus est "Conducteur trop jeune pour ce véhicule"
```

### AC-02 — Acceptation d'un conducteur ayant exactement 21 ans pour un véhicule standard
*(Source : exemples domaine 2 & 5)*

```gherkin
Given un conducteur âgé de 21 ans avec un permis B valide
And un véhicule de type voiture (non trottinette électrique)
When le conducteur soumet une demande de vérification d'éligibilité
Then le conducteur est déclaré éligible
```

### AC-03 — La règle d'âge légal pour la trottinette électrique reste inchangée à 16 ans
*(Source : exemple domaine 3)*

```gherkin
Given un conducteur âgé de 16 ans
And un véhicule de type trottinette électrique
When le conducteur soumet une demande de vérification d'éligibilité
Then le conducteur est déclaré éligible
```

### AC-04 — Refus d'un conducteur entre 16 et 20 ans inclus pour un véhicule non-trottinette
*(Source : exemples domaine 1 & 4 — couverture de la borne inférieure)*

```gherkin
Given un conducteur âgé de 18 ans avec un permis B valide
And un véhicule de type voiture (non trottinette électrique)
When le conducteur soumet une demande de vérification d'éligibilité
Then le conducteur est déclaré non éligible
And le motif de refus est "Conducteur trop jeune pour ce véhicule"
```

---

## Technical Notes

- La méthode concernée est `Vehicle.MinimumAge()` dans `MonAssurance.Domain.Eligibility.Vehicle`
- Actuellement : retourne `18` pour tous les véhicules non-trottinette ; doit retourner `21`
- La règle trottinette électrique (`VehicleType.ElectricScooter` → 16 ans) est **inchangée**
- Les tests existants dans `EligibilityPolicyTests` et `CheckEligibilityQueryHandlerTests` qui testent l'âge 18 pour un véhicule standard devront être mis à jour
- Aucune dépendance vers un service externe ; changement purement domain

---

## DoR Checklist

| Item | Statut | Notes |
|---|---|---|
| 1. Problem statement | ✅ PASS | Conformité légale obligatoire — âge minimal passe à 21 ans |
| 2. Specific persona | ✅ PASS | Conducteur déposant une demande d'assurance automobile |
| 3. 3+ domain examples | ✅ PASS | 5 exemples avec noms, âges et types de véhicule réels |
| 4. UAT scenarios | ✅ PASS | 4 scénarios Given/When/Then validables par un stakeholder non-technique |
| 5. AC derived from UAT | ✅ PASS | Chaque AC tracé vers un ou plusieurs exemples domaine |
| 6. Right-sized | ✅ PASS | Effort S — changement d'une règle domain + mise à jour des tests (0.5 jour) |
| 7. Technical notes | ✅ PASS | `Vehicle.MinimumAge()` identifié, impact tests précisé |
| 8. Dependencies | ✅ PASS | Aucune dépendance inter-story |

**DoR status : ✅ PASSED — story prête pour DESIGN**

---

## Antipattern Scan

| Code | Antipattern | Statut |
|---|---|---|
| AP-DISCUSS-01 | Implement-X | ✅ Absent — story axée sur le besoin métier |
| AP-DISCUSS-02 | Generic Data | ✅ Absent — exemples avec noms, âges et types précis |
| AP-DISCUSS-03 | Technical AC | ✅ Absent — ACs observables sans référence d'implémentation |
| AP-DISCUSS-04 | Giant Stories | ✅ Absent — 4 ACs, effort S |
| AP-DISCUSS-05 | No Examples | ✅ Absent — 5 exemples domaine |
| AP-DISCUSS-06 | Tests After Code | ✅ Absent — ACs sous forme Given/When/Then indépendants |
| AP-DISCUSS-07 | Vague Persona | ✅ Absent — persona identifié (inferred) |
| AP-DISCUSS-08 | Missing Dependencies | ✅ Absent — dépendances explicitement listées (aucune) |

---
Timestamp: 2026-06-02T21:31:48Z
