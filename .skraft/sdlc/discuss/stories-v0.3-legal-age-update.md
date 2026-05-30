# Stories — Milestone v0.3-legal-age-update

## Milestone Summary

| Field | Value |
|---|---|
| Title | v0.3-legal-age-update |
| Theme | Regulatory compliance: French minimum driver age update |
| Goal | Update the eligibility domain to enforce the new French legal minimum driving age of 21 for standard vehicles (car and motorcycle). |
| Non-goals | Changes to electric scooter age (governed by issue #45); high-power motorcycle experience rule. |
| Capacity | 3.5 effective team-days (5 days × 0.7) |

---

## Sprint Plan

### MoSCoW Table

| Story ID | Title | MoSCoW | Effort | Story-days | Reasoning |
|---|---|---|---|---|---|
| #52 | Legal Minimum Driver Age (21 years) | Must Have | S | 0.5 | Legal compliance — required for next release to remain compliant with French insurance regulations |

**Total scheduled: 0.5 story-days ✅ (capacity: 3.5 team-days — well within limit)**

### Capacity Check

```
Sustainable capacity: 5 days × 1 engineer × 0.7 = 3.5 story-days
Scheduled:           0.5 story-days
Remaining:           3.0 story-days
Status:              ✅ Within capacity
```

### Dependency Graph

```
#52 (Legal Minimum Driver Age)  — no dependencies
```

DAG validation: no cycles. ✅

---

## Story List

### STORY-52 — Legal Minimum Driver Age (21 years)

| Field | Value |
|---|---|
| GitHub Issue | #52 |
| Persona | Driver applying for vehicle insurance in France |
| Effort | S (0.5 days) |
| MoSCoW | Must Have |
| DoR Status | ✅ READY |
| Dependencies | None |

**Story Statement:**
> As a driver applying for vehicle insurance in France,  
> I want the eligibility system to enforce the new legal minimum driving age of 21,  
> so that insurance quotes are only issued to drivers who meet the current French regulatory requirements.

**Summary of ACs (full file: `ac-draft-52-legal-minimum-age.md`):**
- AC-01: Driver under 21 refused for Car (age 20 → not eligible)
- AC-02: Driver aged exactly 21 eligible for Car
- AC-03: Driver under 21 refused for Motorcycle (age 20 → not eligible)
- AC-04: Driver at old minimum age (18) is now refused (regression guard)
- AC-05: Electric scooter rule unaffected (age 16 → eligible)

---

## Stories NOT Ready (DoR Failed)

None. All stories passed DoR.

---

_Generated: 2026-05-30_
_Triage source: .skraft/sdlc/discover/triage-2026-05-30.md_
