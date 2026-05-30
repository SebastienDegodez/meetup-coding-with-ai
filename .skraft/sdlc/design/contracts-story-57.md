# Interface Contracts — story-57 : Âge légal minimal porté à 21 ans pour les véhicules standards

**Story:** story-57
**Date:** 2026-05-30

## EligibilityContext

### Query : `CheckEligibility`

| Field | Type | Required | Notes |
|---|---|---|---|
| `DateOfBirth` | `DateOnly` | yes | Date de naissance du conducteur |
| `VehicleType` | `VehicleType` (enum) | yes | `Car` \| `Motorcycle` \| `ElectricScooter` |
| `Power` | `int?` | no | Puissance moteur en CV ; `null` → traité comme < 100 CV |
| `LicenseYears` | `int` | yes | Nombre d'années depuis l'obtention du permis ; ≥ 0 |

**Règle de validation :**
- `DateOfBirth` doit être une date passée valide.
- `LicenseYears` doit être ≥ 0.
- Si `VehicleType == Motorcycle`, `Power` peut être null (traité comme non-haute puissance).

---

### Read Model : `EligibilityViewModel`

| Field | Type | Required | Notes |
|---|---|---|---|
| `IsEligible` | `bool` | yes | `true` si éligible, `false` si refusé |
| `RejectionReason` | `string?` | no | Présent uniquement si `IsEligible == false` |

**Motifs de refus possibles (invariants) :**
- `"minimum age not met"` — conducteur trop jeune pour le type de véhicule (âge < `Vehicle.MinimumAge()`)
  - Pour véhicule standard : âge minimal = **21 ans** (story-57 : mise à jour de 18 → 21)
  - Pour trottinette électrique : âge minimal = **16 ans** (règle inchangée)
- `"insufficient experience"` — moto haute puissance (> 100 CV) avec < 5 ans de permis

---

### Interface : `EligibilityPolicy`

```
EligibilityResult Evaluate(Driver driver, Vehicle vehicle, DateOnly today)
```

- Entrée : `Driver` (Value Object), `Vehicle` (Value Object), date du jour
- Sortie : `EligibilityResult` (Value Object) — `Accepted()` ou `Refused(reason)`
- Aucun effet de bord — méthode pure.

---

## Vocabulary cross-check (entrée Phase 9)

Chaque heading `{contract-category}` DOIT correspondre à la classification ratifiée pour `{contract-name}` dans l'ADR correspondant :
- `CheckEligibility` → `Query` (adr-002) ✓
- `EligibilityViewModel` → `Read Model` (adr-002) ✓
- `EligibilityPolicy` → `Domain Service` (adr-001) ✓
