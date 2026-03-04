# Refactoring Plan: Testing Skills — V2 (Merge subskills)

**Date:** 2026-03-03  
**Goal:** Reduce 3 testing skills to 2 by merging `domain-layer-testing` + `application-layer-testing` into a single `ddd-unit-testing` skill. Zero duplication, clean ownership, writing-skills standards respected.

---

## V1 → V2 Decision

V1 kept 3 separate skills. V2 merges the two subskills because:

1. `domain-layer-testing` is too thin (354 words) to justify a standalone skill
2. Both share the same testing philosophy: real Domain objects, state-based assertions, fast, no Testcontainers
3. The "routing" between Application/Domain tests is a paragraph-level concern, not a skill-level concern
4. One subskill = one place to look = simpler agent decision (load `ddd-unit-testing` when writing unit tests for Domain or Application)
5. Eliminates the cross-reference dance between two complementary subskills

---

## Diagnostic Summary

### Word counts (current)

| File | Words |
|---|---|
| `outside-in-tdd/SKILL.md` | 1866 |
| `domain-layer-testing/SKILL.md` | 354 |
| `application-layer-testing/SKILL.md` | 891 |
| `application-layer-testing/references/` (4 files) | 3072 |
| `application-layer-testing/assets/` (2 files) | 360 |
| **Total** | **6543** |

### Problems Found

#### P1 — Massive responsibility overlap between parent and subskills

`outside-in-tdd` contains full inline C# examples for **both** Application-layer tests AND Domain-layer tests. It also inlines:
- Detailed mocking rules (owned by `application-layer-testing`)
- A "When Domain Logic Needs Its Own Tests" section (owned by `domain-layer-testing`)
- A full mutation policy with workflow steps (duplicated in *both* subskills AND in `references/mutation-testing.md`)

`application-layer-testing` contains a 6-step "Feature Implementation Workflow" (create test → define command → implement handler → complete test → run tests → mutation) which is the **TDD cycle** — that's the parent skill's job.

#### P2 — Token bloat

`outside-in-tdd` at **1866 words** far exceeds the ~500 word target for frequently-loaded skills. Every time someone implements a feature, this entire document is loaded. The inline code examples alone account for ~600 words.

#### P3 — Duplication inventory

| Content | Appears in |
|---|---|
| Mutation policy (0 survivors, propose-to-user workflow) | outside-in-tdd, domain-layer-testing, application-layer-testing, references/mutation-testing.md (**4 places**) |
| "Don't mock Domain objects" rule | outside-in-tdd, application-layer-testing SKILL.md, references/testing-strategy.md (**3 places**) |
| EligibilityPolicy code example | outside-in-tdd, domain-layer-testing (**2 places**) |
| CQRS handler patterns | application-layer-testing SKILL.md, references/cqrs-patterns.md (**2 places**) |
| Testing DO/DON'T rules | outside-in-tdd, application-layer-testing (**2 places**) |

#### P4 — Misleading multi-stack table

`outside-in-tdd` has a "Target Stacks" table listing .NET **and** Quarkus/Java, but every single code example is C# only. The table creates false expectations.

#### P5 — Cross-reference style doesn't follow standards

Skills reference each other with backtick names (`application-layer-testing`) instead of explicit requirement markers (`**REQUIRED SUB-SKILL:** Use application-layer-testing`).

#### P6 — Structural inconsistency

- `outside-in-tdd`: everything inline, no `references/` → bloated SKILL.md
- `domain-layer-testing`: self-contained, no extra files (appropriate for its size)
- `application-layer-testing`: proper `references/` and `assets/` but SKILL.md repeats content from references

#### P7 — domain-layer-testing is too thin for a standalone skill

At 354 words with no references or assets, it's more of a "section" than a "skill". Its content naturally belongs alongside Application testing as a granularity decision within the same skill.

---

## Target Architecture (V2 — 2 skills)

### Final skill inventory

```
outside-in-tdd (parent — orchestrator)
├── OWNS: 2-step cycle (RED → SYNTHESIZE GREEN)
├── OWNS: Gherkin → test level decision
├── OWNS: "Compilation errors ≠ RED" philosophy
├── OWNS: Architectural guidance (optional step between RED/GREEN)
├── OWNS: Mutation policy (single source of truth)
├── OWNS: Common rationalizations / red flags
├── OWNS: Quick reference table
├── DELEGATES TO ddd-unit-testing: all unit testing patterns
└── references/
    └── mutation-policy.md (authoritative mutation workflow)

ddd-unit-testing (subskill — all DDD unit testing)
├── OWNS: Sociable testing philosophy (real Domain + mocked Infrastructure)
├── OWNS: Application orchestration testing (handlers, CQRS)
├── OWNS: Domain logic testing (aggregates, VOs, domain services, invariants)
├── OWNS: When to write Application-level vs Domain-level tests (inline routing)
├── OWNS: Mocking rules (Infrastructure only, never Domain)
├── OWNS: Project structure for test files
├── OWNS: Test templates (assets/)
├── OWNS: Anti-patterns for both layers
├── DOES NOT OWN: TDD cycle, mutation policy, Gherkin
└── references/
    ├── cqrs-patterns.md (CQRS handler structure, commands vs queries)
    ├── test-examples.md (complete code examples)
    └── testing-strategy.md (sociable vs solitary philosophy)
```

### Deleted skills/files

| Path | Reason |
|---|---|
| `domain-layer-testing/` (entire folder) | Content merged into `ddd-unit-testing` |
| `application-layer-testing/` (entire folder) | Renamed/restructured to `ddd-unit-testing` |
| `application-layer-testing/references/mutation-testing.md` | Moved to `outside-in-tdd/references/mutation-policy.md` |

### Parent references to update

The following configs/files reference the old skill names and must be updated:

- `.github/copilot-instructions.md` or skill declarations (if any)
- `outside-in-tdd/SKILL.md` cross-references

### Target word counts

| File | Current | Target | Notes |
|---|---|---|---|
| `outside-in-tdd/SKILL.md` | 1866 | ~500 | -73%, remove inline examples + delegation |
| `outside-in-tdd/references/mutation-policy.md` | NEW | ~200 | Single source of truth |
| `ddd-unit-testing/SKILL.md` | NEW (~1245 combined) | ~500 | Merge + heavy trim |
| `ddd-unit-testing/references/testing-strategy.md` | 461 | ~350 | Trim duplication |
| `ddd-unit-testing/references/cqrs-patterns.md` | 569 | ~500 | Light trim |
| `ddd-unit-testing/references/test-examples.md` | 946 | ~900 | Keep mostly as-is |
| `ddd-unit-testing/assets/*` | 360 | 360 | No change |
| **SKILL.md total (loaded per conversation)** | **3111** | **~1000** | **-68%** |

---

## Step-by-Step Refactoring Plan

### Phase 1 — Create `ddd-unit-testing` (merge subskills)

**Actions:**

1. **Rename** `application-layer-testing/` → `ddd-unit-testing/` (keeps references/, assets/).

2. **Write new `ddd-unit-testing/SKILL.md`** by merging both skills. Structure:

   ```markdown
   ---
   name: ddd-unit-testing
   description: Use when writing unit tests for Domain or Application layers in DDD Clean Architecture with CQRS handlers, aggregates, value objects, and domain services
   ---

   # DDD Unit Testing

   ## Overview
   Unified testing guide for Application orchestration and Domain logic in Clean Architecture.
   Core rule: real Domain objects, mocked Infrastructure only, fast in-memory tests.

   ## Application Tests (Sociable — Handler Level)
   - Test command/query handlers with real Domain objects
   - Mock only Infrastructure (repositories, external services)
   - Verify orchestration flow + Domain state outcomes
   [Minimal inline example — one handler test]

   ## Domain Tests (Pure — Logic Level)
   - Test aggregates, VOs, domain services, invariants, state transitions
   - No mocks at all — pure in-memory state-based assertions
   - Business language naming
   [Minimal inline example — one domain policy test]

   ## When to Write Which
   - Handler orchestration (load/save/publish) → Application test
   - Complex business rules, edge-case matrices, invariants → Domain test
   - Simple rule adequately covered by handler test → don't duplicate

   ## Testing Rules (DO / DON'T)
   [Single consolidated list — no duplication]

   ## Project Structure
   [Single tree showing both Application/ and Domain/ test folders]

   ## Anti-Patterns
   [Merged list from both skills]

   ## References & Templates
   [Links to references/ and assets/]
   ```

3. **Delete the "Feature Implementation Workflow"** (steps 1-6) — TDD cycle belongs to parent.
4. **Delete mutation testing sections** — owned by parent.
5. **Absorb `domain-layer-testing` content** into the "Domain Tests" section and "Anti-Patterns".

**Verification:**
- `wc -w .github/skills/ddd-unit-testing/SKILL.md` should be ≤ 550
- All references/ and assets/ files are present and linked

### Phase 2 — Extract mutation policy to parent

**Actions:**

1. **Create `outside-in-tdd/references/mutation-policy.md`** with the authoritative mutation workflow:
   - 0 surviving mutants required (Domain + Application)
   - Inspect → propose functional tests → user validation → implement → rerun
   - Hard rule: proposed tests = business-oriented, not micro-tests
   - Stryker.NET configuration reference

2. **In `outside-in-tdd/SKILL.md`:** replace the full "Mutation Policy" section with a 2-line summary + `See references/mutation-policy.md for the complete workflow.`

3. **In `ddd-unit-testing/SKILL.md`:** no mutation content at all — just a note: `Mutation testing is governed by the parent skill outside-in-tdd.`

**Verification:** `grep -r "surviving mutant" .github/skills/` shows content only in `mutation-policy.md`.

### Phase 3 — Slim down `outside-in-tdd/SKILL.md`

**Actions:**

1. **Remove Target Stacks table** — misleading (Quarkus listed, no Java examples).
2. **Remove all inline C# code examples** from:
   - "Step 1: RED" → 3-line pseudocode + cross-ref to `ddd-unit-testing`
   - "Step 2: SYNTHESIZE GREEN" → same
   - "When Domain Logic Needs Its Own Tests" → **DELETE** entirely (now in `ddd-unit-testing`)
   - "Compilation Errors ≠ RED" → keep concept, compress example to 2 lines
3. **Compress Common Rationalizations** to top 5 entries (currently 10).
4. **Compress Red Flags** to flat bullet list.
5. **Fix cross-references:**
   - `**REQUIRED SUB-SKILL:** ddd-unit-testing` (single reference, replaces two)
   - `**REQUIRED BACKGROUND:** test-driven-development`
   - `**REQUIRED CONTEXT:** clean-architecture-dotnet`
6. **Update "Test Placement Rules"** section — simplify since routing is now inside `ddd-unit-testing`.

**Verification:** `wc -w .github/skills/outside-in-tdd/SKILL.md` ≤ 500.

### Phase 4 — Delete old skills

**Actions:**

1. **Delete `domain-layer-testing/`** folder entirely (content merged into `ddd-unit-testing`).
2. **Delete `application-layer-testing/`** folder entirely (restructured as `ddd-unit-testing`).
3. **Verify no orphan references:** `grep -rn "domain-layer-testing\|application-layer-testing" .github/` → should return nothing except this plan doc.

### Phase 5 — Clean up reference files

**Actions:**

1. **`ddd-unit-testing/references/testing-strategy.md`:** Remove mutation policy content, TDD cycle references. Add section on when to write Application vs Domain tests (absorbed from old routing logic).
2. **`ddd-unit-testing/references/cqrs-patterns.md`:** Light trim, remove content duplicated in SKILL.md.
3. **`ddd-unit-testing/references/test-examples.md`:** Add one Domain-level test example (from old `domain-layer-testing` EligibilityPolicy example). Keep existing Application examples.
4. **Delete `references/mutation-testing.md`** (already moved to parent in Phase 2).

**Verification:** `wc -w` on all reference files.

### Phase 6 — Final validation

**Actions:**

1. Word count check:
   ```bash
   wc -w .github/skills/outside-in-tdd/SKILL.md
   wc -w .github/skills/ddd-unit-testing/SKILL.md
   wc -w .github/skills/ddd-unit-testing/references/*.md
   ```
2. Duplication check:
   ```bash
   grep -rn "surviving mutant\|0 surviving\|mutant survives" .github/skills/
   grep -rn "A.Fake\|Mock only Infrastructure" .github/skills/
   ```
3. Orphan reference check:
   ```bash
   grep -rn "domain-layer-testing\|application-layer-testing" .github/
   ```
4. Frontmatter validation — both SKILL.md files:
   - `name`: letters, numbers, hyphens only ✓
   - `description`: starts with "Use when...", no workflow summary, < 500 chars ✓
5. Verify `ddd-unit-testing` directory contains:
   ```
   ddd-unit-testing/
   ├── SKILL.md
   ├── assets/
   │   ├── CommandHandlerTestTemplate.cs
   │   └── QueryHandlerTestTemplate.cs
   └── references/
       ├── cqrs-patterns.md
       ├── test-examples.md
       └── testing-strategy.md
   ```
6. Verify `outside-in-tdd` directory contains:
   ```
   outside-in-tdd/
   ├── SKILL.md
   └── references/
       └── mutation-policy.md
   ```

---

## Files Modified (Summary)

| Action | File |
|---|---|
| CREATE | `ddd-unit-testing/SKILL.md` (merged from both subskills) |
| CREATE | `outside-in-tdd/references/mutation-policy.md` |
| HEAVY EDIT | `outside-in-tdd/SKILL.md` (1866 → ~500 words) |
| MOVE | `application-layer-testing/references/` → `ddd-unit-testing/references/` |
| MOVE | `application-layer-testing/assets/` → `ddd-unit-testing/assets/` |
| EDIT | `ddd-unit-testing/references/testing-strategy.md` (add Domain routing) |
| EDIT | `ddd-unit-testing/references/test-examples.md` (add Domain example) |
| DELETE | `domain-layer-testing/` (entire folder) |
| DELETE | `application-layer-testing/` (entire folder) |
| DELETE | `ddd-unit-testing/references/mutation-testing.md` (moved to parent) |

---

## Risk Assessment

| Risk | Mitigation |
|---|---|
| Merged skill too big (> 500 words for SKILL.md) | Heavy reference material in `references/`, SKILL.md stays concise with cross-refs |
| Agent doesn't find `ddd-unit-testing` by old name | `grep` check in Phase 4 catches orphan references; `description` field covers both search terms (Domain, Application, CQRS, aggregates) |
| Removing too much from `outside-in-tdd` breaks behavior | Keep core rules (RED before GREEN, compilation ≠ RED). Pressure test. |
| Cross-references not followed by agent | Use `**REQUIRED SUB-SKILL:**` markers per writing-skills standard |
| Domain testing patterns get lost in merged skill | Dedicated "Domain Tests" section + example in `test-examples.md` |
| Mutation policy orphaned after consolidation | Single location in parent `references/mutation-policy.md`, both skills reference it |

---

## CSO (Claude Search Optimization) — New frontmatter

### `outside-in-tdd`
```yaml
name: outside-in-tdd
description: Use when implementing features or use cases starting from observable business behavior (Gherkin), where design should emerge from tests rather than be predetermined
```
*(Keep unchanged — triggers are correct, no workflow summary.)*

### `ddd-unit-testing`
```yaml
name: ddd-unit-testing
description: Use when writing unit tests for Domain or Application layers in DDD Clean Architecture with CQRS handlers, aggregates, value objects, and domain services
```
**Keyword coverage:** unit test, Domain, Application, DDD, Clean Architecture, CQRS, handler, aggregate, value object, domain service — covers both old skill trigger paths.
