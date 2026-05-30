# AC Draft — STORY-57 : Âge légal minimal porté à 21 ans pour les véhicules standards

## Story

```
En tant que conducteur demandant une assurance véhicule,
je veux que le système applique le nouvel âge légal minimal de 21 ans,
afin que mon dossier d'assurance soit conforme à la nouvelle législation française.
```

---

## Persona

**Conducteur demandant une assurance véhicule** — toute personne physique soumettant une demande d'éligibilité pour un véhicule standard (voiture, moto, etc.) via le service d'assurance.

---

## Problème Métier

Une nouvelle loi française relève l'âge légal minimal pour conduire un véhicule (hors trottinette électrique) de 18 à 21 ans. Le moteur d'éligibilité `Vehicle.MinimumAge()` retourne actuellement `18` pour les véhicules non-trottinette. Cette valeur doit être mise à jour à `21` pour rester conforme à la loi.

---

## Domain Examples

1. **Marie Dupont**, 19 ans, permis B valide (obtenu à 18 ans), Peugeot 308 (voiture standard)  
   → **non éligible** : âge < 21 ans (nouvel âge légal)

2. **Pierre Martin**, 21 ans, permis B valide (obtenu à 19 ans), Renault Clio  
   → **éligible** : âge = 21 ans (exactement le seuil légal)

3. **Lucas Bernard**, 20 ans, permis B obtenu il y a 2 ans, Citroën C3  
   → **non éligible** : âge < 21 ans

4. **Isabelle Mercier**, 25 ans, permis B valide depuis 7 ans, Volkswagen Golf  
   → **éligible** : âge > 21 ans, profil standard

5. **Tom Lefebvre**, 16 ans, permis AM, trottinette électrique  
   → **éligible** : trottinette électrique, âge minimal reste 16 ans (règle inchangée)

---

## Acceptance Criteria

### AC-01 : Conducteur de 19 ans avec véhicule standard — refus d'éligibilité

```gherkin
Given a driver aged 19 with a valid B licence
And a standard vehicle (e.g. Peugeot 308)
When the driver requests an eligibility check
Then the driver is declared ineligible
And the rejection reason is "minimum age not met"
```

*Source : Domain Example 1 — Marie Dupont*

---

### AC-02 : Conducteur de 21 ans avec véhicule standard — éligible

```gherkin
Given a driver aged 21 with a valid B licence
And a standard vehicle (e.g. Renault Clio)
When the driver requests an eligibility check
Then the driver is declared eligible
```

*Source : Domain Example 2 — Pierre Martin*

---

### AC-03 : Conducteur de 20 ans avec véhicule standard — refus d'éligibilité

```gherkin
Given a driver aged 20 with a valid B licence
And a standard vehicle (e.g. Citroën C3)
When the driver requests an eligibility check
Then the driver is declared ineligible
And the rejection reason is "minimum age not met"
```

*Source : Domain Example 3 — Lucas Bernard*

---

### AC-04 : Trottinette électrique — âge minimal 16 ans inchangé

```gherkin
Given a driver aged 16 with a valid AM licence
And an electric scooter
When the driver requests an eligibility check
Then the driver is declared eligible
```

*Source : Domain Example 5 — Tom Lefebvre (règle trottinette non impactée)*

---

## Technical Notes

- **Fichier cible** : `src/MonAssurance.Domain/Eligibility/Vehicle.cs`
- **Méthode impactée** : `Vehicle.MinimumAge()`
- **Changement** : Retourner `21` au lieu de `18` pour les véhicules non-trottinette électrique
- **Règle trottinette** : `VehicleType.ElectricScooter` → âge minimal reste `16` (inchangé)
- **Tests impactés** : Tous les tests d'éligibilité utilisant un véhicule standard avec un conducteur âgé de 18–20 ans doivent être mis à jour pour refléter le refus
- **Motif de refus existant** : Vérifier que le motif "minimum age not met" est déjà défini dans `EligibilityResult` ou équivalent

---

## DoR Checklist

| Item | Critère | Statut |
|---|---|---|
| 1 | Problem statement (pas "implement X") | ✅ PASS — Nouvelle loi française : âge légal porté à 21 ans pour les véhicules standards |
| 2 | Persona spécifique nommée | ✅ PASS — Conducteur demandant une assurance véhicule |
| 3 | 3+ exemples domaine avec valeurs réelles | ✅ PASS — 5 exemples avec noms, âges, types de véhicules |
| 4 | Scénarios UAT écrits | ✅ PASS — 4 ACs en Given/When/Then |
| 5 | ACs dérivés des scénarios UAT | ✅ PASS — Chaque AC référence son exemple source |
| 6 | Taille correcte (1–3 jours solo) | ✅ PASS — S = 0.5 jour |
| 7 | Notes techniques ajoutées | ✅ PASS — Fichier cible, méthode, impact tests listés |
| 8 | Dépendances listées | ✅ PASS — Aucune dépendance inter-story |

**DoR Result : ✅ PASS (8/8) — Story prête pour DESIGN**

---

## Sprint Planning

| Champ | Valeur |
|---|---|
| **Milestone** | `v0.2-legal-age` |
| **MoSCoW** | Must Have |
| **Effort** | S (0.5 jour) |
| **Priorité triage** | P0 |
| **Dépendances** | Aucune |
