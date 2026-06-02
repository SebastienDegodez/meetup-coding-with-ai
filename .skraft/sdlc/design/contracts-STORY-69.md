# Interface Contracts — STORY-69: Offre de Souscription

**Story:** STORY-69-A, STORY-69-B
**Date:** 2026-06-02

---

## SubscriptionContext

### Query: `GetInsurancePriceQuoteQuery`

| Field | Type | Required | Notes |
|---|---|---|---|
| `subscriptionReference` | `string` | yes | Format: SUB-YYYY-NNN. Identifie la demande de souscription active. |

**Return shape: `InsurancePriceQuoteViewModel`**

| Field | Type | Nullable | Notes |
|---|---|---|---|
| `monthlyAmount` | `decimal` | no | Montant mensuel en euros |
| `coverageType` | `string` | no | Ex: "tiers", "tous risques" |
| `isPremiumApplied` | `bool` | no | true si surprime haute puissance appliquée |
| `rejectionReason` | `string` | yes | Présent uniquement si la demande est rejetée (aucune demande active) |

**Invariants:**
- Si `subscriptionReference` ne correspond à aucune demande active → retourner `rejectionReason: "no active subscription request found"`
- Si `isPremiumApplied = true` → `monthlyAmount > base rate`

---

### Command: `InitiateSubscriptionRequestCommand`

| Field | Type | Required | Notes |
|---|---|---|---|
| `driverId` | `string` | yes | Identifiant opaque du conducteur (référence cross-context depuis EligibilityContext) |
| `vehicleType` | `VehicleType` | yes | Car, Motorcycle, ElectricScooter |
| `power` | `int?` | no | Puissance du véhicule en cv. null = non spécifié (< 100 cv implicite) |
| `eligibilityConfirmed` | `bool` | yes | Doit être `true`. Sinon la commande est rejetée. |

**Return shape: `SubscriptionRequestConfirmationViewModel`**

| Field | Type | Nullable | Notes |
|---|---|---|---|
| `subscriptionReference` | `string` | no | Format: SUB-YYYY-NNN. Référence unique de la demande. |
| `status` | `string` | no | "initiated" |
| `rejectionReason` | `string` | yes | Présent uniquement si `eligibilityConfirmed = false` |

**Invariants:**
- Si `eligibilityConfirmed = false` → rejeter avec reason: "subscription requires prior eligibility confirmation"
- Si une demande active existe déjà pour ce `driverId` → retourner la référence existante (idempotence, AC-02)
- `subscriptionReference` est unique et non réutilisable

---

### Domain Event: `SubscriptionRequestInitiated`

| Field | Type | Required | Notes |
|---|---|---|---|
| `subscriptionReference` | `string` | yes | Référence unique générée à la création |
| `driverId` | `string` | yes | Identifiant opaque du conducteur |
| `vehicleType` | `VehicleType` | yes | Type de véhicule associé à la demande |
| `power` | `int?` | no | Puissance du véhicule en cv |
| `initiatedAt` | `DateTimeOffset` | yes | Timestamp UTC de création |

**Invariants:**
- Émis uniquement à la première création de la demande (pas émis lors de la re-soumission idempotente)

---

### Domain Event: `SubscriptionRequestRejected`

| Field | Type | Required | Notes |
|---|---|---|---|
| `driverId` | `string` | yes | Identifiant du conducteur ayant tenté la création |
| `reason` | `string` | yes | Motif de rejet. Ex: "subscription requires prior eligibility confirmation" |
| `rejectedAt` | `DateTimeOffset` | yes | Timestamp UTC du rejet |

---

### Interface: `ISubscriptionRequestRepository`

```csharp
public interface ISubscriptionRequestRepository
{
    Task<SubscriptionRequest?> FindByDriverIdAsync(string driverId, CancellationToken ct = default);
    Task<SubscriptionRequest?> FindByReferenceAsync(string subscriptionReference, CancellationToken ct = default);
    Task SaveAsync(SubscriptionRequest subscriptionRequest, CancellationToken ct = default);
}
```

**Invariants:**
- `FindByDriverIdAsync` retourne `null` si aucune demande n'existe pour ce conducteur (utilisé pour l'idempotence)
- `FindByReferenceAsync` retourne `null` si la référence n'existe pas (utilisé pour `GetInsurancePriceQuote`)
- `SaveAsync` est idempotent sur la même référence (création ou mise à jour)

---

## Vocabulary cross-check (Phase 9 input)

Every `{contract-category}` heading MUST match the classification ratified for `{contract-name}` in the corresponding ADR:

| Contract name | Category in this file | ADR ratification |
|---|---|---|
| `GetInsurancePriceQuoteQuery` | `Query` | ADR-003 |
| `InitiateSubscriptionRequestCommand` | `Command` | ADR-001 |
| `SubscriptionRequestInitiated` | `Domain Event` | ADR-001 |
| `SubscriptionRequestRejected` | `Domain Event` | ADR-001 |
| `ISubscriptionRequestRepository` | `Interface` (Repository) | ADR-001 |
