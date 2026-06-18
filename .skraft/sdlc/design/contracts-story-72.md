# Interface Contracts — story-72 : Âge légal de conduite porté à 21 ans

**Story:** story-72
**Date:** 2026-06-02

## EligibilityContext

### Query: `CheckEligibilityForVehicle`

| Field | Type | Required | Notes |
|---|---|---|---|
| `dateOfBirth` | `DateOnly` | yes | Date de naissance du conducteur |
| `licenseYears` | `int` | yes | Nombre d'années de permis |
| `vehicleType` | `VehicleType` | yes | `Car`, `Motorcycle`, `ElectricScooter` |
| `power` | `int?` | no | Puissance en cv ; null traité comme < 100 cv (pas de règle expérience) |

### Read Model: `EligibilityViewModel`

| Field | Type | Required | Notes |
|---|---|---|---|
| `eligible` | `bool` | yes | `true` si le conducteur est éligible |
| `rejectionReason` | `string?` | no | Motif de refus ; null si éligible |

### Domain Service: `EligibilityPolicy`

```
EligibilityResult Evaluate(Driver driver, Vehicle vehicle, DateOnly today)
```

| Parameter | Type | Notes |
|---|---|---|
| `driver` | `Driver` (Value Object) | Construit depuis dateOfBirth + licenseYears |
| `vehicle` | `Vehicle` (Value Object) | Construit depuis vehicleType + power |
| `today` | `DateOnly` | Date de référence fournie par TimeProvider |

**Invariants appliqués (post-story-72) :**
- `driver.Age(today) < vehicle.MinimumAge()` → `EligibilityResult.Refused("Conducteur trop jeune pour ce véhicule")`
  - `Vehicle.MinimumAge()` retourne **16** pour `ElectricScooter`, **21** pour tout autre type (changement story-72 : 18 → 21)
- `vehicle.IsHighPowerMotorcycle() && !driver.HasEnoughExperience(5)` → `EligibilityResult.Refused("Expérience insuffisante pour la puissance")`

### Value Object: `Vehicle`

| Behaviour | Signature | Post-story-72 |
|---|---|---|
| `MinimumAge()` | `int MinimumAge()` | Retourne 16 pour `ElectricScooter`, **21** pour tout autre `VehicleType` |
| `IsHighPowerMotorcycle()` | `bool IsHighPowerMotorcycle()` | Inchangé |

## Vocabulary cross-check (Phase 9 input)

Chaque `{contract-category}` dans ce fichier (`Query`, `Read Model`, `Domain Service`, `Value Object`) DOIT correspondre à la classification ratifiée pour le concept dans l'ADR correspondant. Une divergence déclenche `CLASSIFICATION_DRIFT` à Phase 9.
