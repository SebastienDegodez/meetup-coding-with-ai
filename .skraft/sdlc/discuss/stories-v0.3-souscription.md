# Stories — Milestone v0.3-souscription

## Milestone Summary

**Theme:** Souscription post-éligibilité  
**Scope:** Nouveau flux permettant à un conducteur éligible de soumettre une demande de souscription et d'obtenir un tarif d'assurance.  
**Non-goals:** Paiement, émission de police, annulation de contrat.  
**Sprint:** 2026-06-02

---

## Sprint Plan

### MoSCoW Table

| ID | Title | Persona | Effort | Priority | DoR |
|---|---|---|---|---|---|
| STORY-69-A | Initiation de la demande de souscription | Conducteur éligible | M | Must Have | ✅ READY |
| STORY-69-B | Consultation du tarif d'assurance | Conducteur éligible avec demande de souscription | M | Must Have | ✅ READY |

### Capacity Check

- Team: 1 engineer, sprint 5 jours → sustainable capacity: 5 × 1 × 0.7 = **3.5 story-days**
- STORY-69-A: M = 1.0 jour
- STORY-69-B: M = 1.0 jour
- **Total: 2.0 story-days ✅ (sous la capacité de 3.5)**

### Dependency Graph

```
STORY-69-A (Initiation de la demande de souscription)
  ↓
STORY-69-B (Consultation du tarif d'assurance) — dépend de STORY-69-A
```

DAG valide — aucun cycle détecté.

Séquence de livraison recommandée :
1. STORY-69-A en premier (flux de base, zéro dépendance externe non résolue)
2. STORY-69-B après démo/intégration de STORY-69-A

---

## Story List

### STORY-69-A — Initiation de la demande de souscription

| Champ | Valeur |
|---|---|
| ID | STORY-69-A |
| GitHub Issue | #69 |
| Titre | Initiation de la demande de souscription |
| Persona | Conducteur éligible ayant obtenu une confirmation d'éligibilité |
| Effort | M (1 jour) |
| Priorité | Must Have |
| DoR | ✅ READY |
| Dépendances | Aucune externe (fonctionnalité d'éligibilité déjà implémentée) |

**User Story:**
> En tant que conducteur éligible ayant reçu une confirmation d'éligibilité,  
> je veux soumettre une demande de souscription,  
> afin de formaliser ma démarche d'obtention d'une couverture d'assurance.

**Détail du fichier AC:** `ac-draft-STORY-69-A.md`

---

### STORY-69-B — Consultation du tarif d'assurance

| Champ | Valeur |
|---|---|
| ID | STORY-69-B |
| GitHub Issue | #69 |
| Titre | Consultation du tarif d'assurance |
| Persona | Conducteur éligible avec une demande de souscription active |
| Effort | M (1 jour) |
| Priorité | Must Have |
| DoR | ✅ READY |
| Dépendances | STORY-69-A (la demande de souscription doit exister avant de demander un tarif) |

**User Story:**
> En tant que conducteur éligible avec une demande de souscription active,  
> je veux recevoir un tarif d'assurance,  
> afin d'évaluer le coût et décider si je souhaite finaliser ma police d'assurance.

**Détail du fichier AC:** `ac-draft-STORY-69-B.md`
