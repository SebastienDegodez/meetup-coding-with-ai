# ADR-004: Single Eligibility Bounded Context; No Context Split

**Status:** Accepted
**Date:** 2026-05-30

## Context

Story #52 operates entirely within driver age validation for vehicle insurance eligibility. The domain concepts involved — `Driver`, `Vehicle`, `EligibilityPolicy`, `EligibilityResult` — all belong to the same cohesive decision: "is this driver eligible to insure this vehicle?" There is no indication of a second ubiquitous language, a second team ownership boundary, or a second deployment unit in this story or any adjacent story in the current milestone (`v0.3-legal-age-update`).

A bounded context split is justified when concepts in two subdomains evolve independently, are owned by different teams, or carry incompatible models. None of these forces apply here.

## Decision

We will maintain a single **Eligibility** bounded context (Core subdomain) containing all eligibility evaluation concepts. No context split is introduced.

## Consequences

**Positive:**
- No anti-corruption layer required — all domain objects share one ubiquitous language.
- No inter-context communication overhead.

**Negative / trade-offs:**
- If a future milestone introduces a Policy issuance context (downstream of Eligibility), a boundary and context-mapping relationship (Upstream/Downstream with ACL or Conformist) will need to be added. This ADR will need updating.

**Neutral:**
- The Electric Scooter age rule (governed by issue #45) remains in the same `Eligibility` context — consistent with this decision.

## Alternatives Rejected

| Alternative | Reason rejected |
|---|---|
| Split into Eligibility + VehicleRules contexts | No separate team, no separate model evolution, no separate deployment. Splitting now would add inter-context plumbing for a 1-line change. |
| Split into Eligibility + DriverProfile contexts | Same reasoning. Driver is a Value Object computed at evaluation time; there is no profile service or driver entity lifecycle in the current story. |
