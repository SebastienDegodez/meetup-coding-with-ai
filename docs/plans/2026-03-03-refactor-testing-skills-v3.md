# Refactoring Plan V3: 2 Skills — Process + Knowledge

**Date:** 2026-03-03  
**Supersedes:** V2 (kept as `2026-03-03-refactor-testing-skills.md` for history)  
**Goal:** Split the 3 existing testing skills into 2 with a clean **process vs knowledge** separation.

---

## V2 → V3 Decision

V2 proposed `outside-in-tdd` (parent) + `ddd-unit-testing` (merged subskill). V3 goes further:

- **Fuse** the testing knowledge (Application + Domain patterns) **into** `outside-in-tdd`
- **Extract** the 2-step AI TDD cycle (RED → SYNTHESIZE GREEN) into a **new standalone process skill**

### Why this split is cleaner

| Concern | V2 | V3 |
|---|---|---|
| TDD cycle discipline | Mixed into `outside-in-tdd` alongside testing patterns | **Own skill** — reusable across any stack/domain |
| DDD testing patterns | Separate skill (`ddd-unit-testing`) with cross-refs | **Fused into `outside-in-tdd`** — one place for DDD testing |
| Agent decision | "Load parent, then maybe load subskill" | "Load process skill + load DDD testing skill" — no nesting |
| Reusability | Process tied to DDD testing context | Process skill works for **any** project using TDD |

The 2-step AI cycle (RED → SYNTHESIZE GREEN, compilation ≠ RED, rationalizations) is a **universal TDD discipline**. It shouldn't be locked inside a DDD-specific skill.

---

## Current State (3 skills)

```
outside-in-tdd/SKILL.md              (1866 words) — mixed process + DDD testing
domain-layer-testing/SKILL.md        (354 words)  — Domain testing patterns
application-layer-testing/            (891 words)  — Application testing patterns
├── references/ (4 files, 3072 words)
└── assets/ (2 files, 360 words)
```

**Total SKILL.md loaded per feature conversation: ~3111 words**

## Target State (2 skills)

```
red-synthesize-green/                 NEW — universal AI TDD process
└── SKILL.md (~400 words)

outside-in-tdd/                       REWORKED — complete DDD testing knowledge
├── SKILL.md (~550 words)
├── assets/
│   ├── CommandHandlerTestTemplate.cs  (from application-layer-testing)
│   └── QueryHandlerTestTemplate.cs   (from application-layer-testing)
└── references/
    ├── mutation-policy.md             NEW (single source of truth)
    ├── testing-strategy.md            (from application-layer-testing)
    ├── cqrs-patterns.md              (from application-layer-testing)
    └── test-examples.md              (from application-layer-testing, enriched with Domain examples)
```

**Total SKILL.md loaded per feature conversation: ~950 words (-69%)**

### Deleted

```
domain-layer-testing/                  DELETED (absorbed into outside-in-tdd)
application-layer-testing/             DELETED (absorbed into outside-in-tdd)
```

---

## Ownership Boundaries

### `red-synthesize-green` — AI TDD Process (stack-agnostic)

```
OWNS:
├── 2-step AI cycle definition (RED → SYNTHESIZE GREEN)
├── "Compilation errors ≠ RED" (wishful thinking phase)
├── Architectural guidance (optional step between RED/GREEN)
├── Common rationalizations table
├── Red flags (STOP and start over)
├── Quick reference table (phases / criteria)
└── When to use / when not to use

DOES NOT OWN:
├── Mocking strategies
├── Testing patterns (sociable, solitary)
├── Code examples in any specific language
├── Mutation testing policy
└── Project structure or tooling
```

### `outside-in-tdd` — DDD Testing Knowledge (C# / .NET)

```
OWNS:
├── Outside-in approach (Gherkin → test → design emergence)
├── Test placement routing (Application test vs Domain test)
├── Application handler testing (sociable, mocked infra, real Domain)
├── Domain logic testing (aggregates, VOs, domain services, invariants)
├── Mocking rules (Infrastructure only, never Domain)
├── Mutation policy (0 survivors, propose functional tests, validate)
├── Anti-patterns (merged from both old subskills)
├── Project structure for test files
├── Templates (CommandHandlerTestTemplate, QueryHandlerTestTemplate)
└── References (testing-strategy, cqrs-patterns, test-examples)

DOES NOT OWN:
├── TDD cycle definition (→ red-synthesize-green)
├── Rationalizations / red flags (→ red-synthesize-green)
└── Stack-agnostic TDD philosophy
```

---

## New Frontmatter (CSO)

### `red-synthesize-green`

```yaml
name: red-synthesize-green
description: Use when following TDD to implement any feature or fix — defines the AI-optimized 2-step cycle where RED means behavior failure only and SYNTHESIZE GREEN produces clean code without a refactor phase
```

**Keywords hit:** TDD, RED, GREEN, refactor, behavior failure, clean code, AI, synthesis, cycle, implementation

### `outside-in-tdd`

```yaml
name: outside-in-tdd
description: Use when writing unit tests for DDD Clean Architecture layers — covers outside-in testing with Gherkin scenarios, Application handler orchestration tests, Domain logic tests, mocking rules, and mutation policy
```

**Keywords hit:** unit test, DDD, Clean Architecture, outside-in, Gherkin, Application, Domain, handler, mock, mutation, aggregate, value object

---

## Destination SKILL.md Outlines

### `red-synthesize-green/SKILL.md` (~400 words)

```markdown
---
name: red-synthesize-green
description: Use when following TDD to implement any feature or fix — defines the AI-optimized 2-step cycle where RED means behavior failure only and SYNTHESIZE GREEN produces clean code without a refactor phase
---

# RED → SYNTHESIZE GREEN (AI TDD Cycle)

## Overview
2-step cycle replacing traditional 3-step TDD. Optimized for AI synthesis.
- Traditional: RED → green (dirty) → Refactor
- AI: RED (behavior failure) → SYNTHESIZE GREEN (clean synthesis)
Hard rule: no implementation before RED is a clean behavior failure.

## Step 1: RED (Behavior Failure Only)
Write failing test. Run it.
- Compilation errors = wishful thinking → stub to compile, rerun
- Assertion/behavior failure = RED ✓ → proceed
- Never treat compilation errors as RED

## Between Steps: Architectural Guidance (Optional)
Orient design approach: patterns, ownership, immutability decisions.
Not micro-management — direction-setting.

## Step 2: SYNTHESIZE GREEN (Clean Synthesis)
Implement complete, clean, production-ready solution in one shot.
- Follows all architectural rules and coding standards
- No dirty-then-refactor — synthesize properly from the start
- If test misunderstood → revise test, restart RED

## Rationalizations (Top 5)
[Table: compilation=RED, dirty-then-refactor, skip RED, 3-step safer, domain first]

## Red Flags — STOP and Restart
[Flat bullet list of the 5 critical violations]

## Quick Reference
[Phase / What / Success Criteria table]

## Integration
**REQUIRED BACKGROUND:** test-driven-development
Pair with domain-specific testing skills for patterns and examples.
```

### `outside-in-tdd/SKILL.md` (~550 words)

```markdown
---
name: outside-in-tdd
description: Use when writing unit tests for DDD Clean Architecture layers — covers outside-in testing with Gherkin scenarios, Application handler orchestration tests, Domain logic tests, mocking rules, and mutation policy
---

# Outside-In DDD Testing

## Overview
Complete testing guide for Application and Domain layers in Clean Architecture.
Start with observable business behavior (Gherkin), let design emerge from tests.
Core rule: real Domain objects, mocked Infrastructure only, fast in-memory tests.

## Outside-In Approach
1. Gherkin scenario (Given/When/Then) — WHAT, not HOW
2. Map to Application test (handler entry point)
3. Domain design emerges from test failures

## Application Tests (Sociable — Handler Level)
- Test command/query handlers with real Domain objects
- Mock only Infrastructure ports (repositories, external services)
- Verify orchestration + Domain state outcomes
[One minimal inline example — ~8 lines]

## Domain Tests (Pure — Logic Level)
- Test aggregates, VOs, domain services, invariants
- No mocks — pure state-based assertions
- Business language naming
[One minimal inline example — ~8 lines]

## When to Write Which
- Orchestration (load/save/publish) → Application test
- Complex rules, edge matrices, invariants → Domain test
- Simple rule covered by handler test → don't duplicate

## Testing Rules
### DO ✅
[Consolidated 5-item list]
### DON'T ❌
[Consolidated 5-item list]

## Mutation Policy
0 surviving mutants (Domain + Application). See references/mutation-policy.md for complete workflow.

## Anti-Patterns
[Merged list from both old subskills — 6 items max]

## Project Structure
[Single tree showing Application/ and Domain/ test folders]

## Common Mistakes
[Top 5 from current table, compressed]

## References & Templates
- references/testing-strategy.md — sociable vs solitary philosophy
- references/cqrs-patterns.md — handler structure, commands vs queries
- references/test-examples.md — complete code examples
- assets/CommandHandlerTestTemplate.cs
- assets/QueryHandlerTestTemplate.cs

## Integration
**REQUIRED PROCESS:** red-synthesize-green (follow the 2-step cycle)
**REQUIRED CONTEXT:** clean-architecture-dotnet (layer boundaries)
```

---

## Step-by-Step Execution Plan

### Phase 1 — Create `red-synthesize-green` skill

**Actions:**
1. Create `red-synthesize-green/SKILL.md` following the outline above.
2. Extract content from `outside-in-tdd/SKILL.md`:
   - Overview (2-step cycle definition)
   - "Compilation Errors ≠ RED" section (compress, remove C# example)
   - Common Rationalizations (keep top 5)
   - Red Flags (compress to bullet list)
   - Quick Reference table
3. Make it **stack-agnostic**: no C# code, no .NET references, no DDD specifics.

**Verification:** `wc -w` ≤ 450 and zero C# code blocks.

### Phase 2 — Restructure `outside-in-tdd` to absorb DDD testing

**Actions:**
1. Move `application-layer-testing/references/` → `outside-in-tdd/references/`
2. Move `application-layer-testing/assets/` → `outside-in-tdd/assets/`
3. Create `outside-in-tdd/references/mutation-policy.md` (single source of truth for mutation workflow).
4. Delete `application-layer-testing/references/mutation-testing.md` (content moved to mutation-policy.md).

**Verification:** Directory structure matches target.

### Phase 3 — Rewrite `outside-in-tdd/SKILL.md`

**Actions:**
1. Rewrite SKILL.md following the outline above.
2. Absorb content from:
   - `domain-layer-testing/SKILL.md` → "Domain Tests" section + "Anti-Patterns"
   - `application-layer-testing/SKILL.md` → "Application Tests" section + "Testing Rules" + "When to Route"
   - Current `outside-in-tdd/SKILL.md` → "Outside-In Approach", "When to Write Which", "Common Mistakes"
3. **Delete from current SKILL.md:**
   - Target Stacks table (misleading, Quarkus with no Java examples)
   - Full inline code examples (replaced with minimal 8-line snippets)
   - 2-step cycle sections (moved to `red-synthesize-green`)
   - Rationalizations / red flags (moved to `red-synthesize-green`)
   - "When Domain Logic Needs Its Own Tests" (absorbed into "When to Write Which")
4. **Update frontmatter** with new description.
5. **Fix cross-references:**
   - `**REQUIRED PROCESS:** red-synthesize-green`
   - `**REQUIRED CONTEXT:** clean-architecture-dotnet`

**Verification:** `wc -w` ≤ 600 and no duplication with `red-synthesize-green`.

### Phase 4 — Enrich reference files

**Actions:**
1. **`references/testing-strategy.md`:** Remove mutation policy content and TDD cycle steps. Add "When Application vs Domain" routing section.
2. **`references/test-examples.md`:** Add Domain-level examples (EligibilityPolicy from old `domain-layer-testing`).
3. **`references/mutation-policy.md` (new):** Authoritative workflow:
   - 0 surviving mutants (Domain + Application)
   - Inspect → propose functional tests → user validation → implement → rerun
   - Hard rule: functional tests only, no micro-tests
   - Stryker.NET configuration guidance
4. **`references/cqrs-patterns.md`:** Light trim of content now in SKILL.md.

### Phase 5 — Delete old skills

**Actions:**
1. Delete `domain-layer-testing/` folder.
2. Delete `application-layer-testing/` folder.
3. Verify no orphan references:
   ```bash
   grep -rn "domain-layer-testing\|application-layer-testing" .github/
   ```

### Phase 6 — Final validation

**Actions:**
1. Word counts:
   ```bash
   wc -w .github/skills/red-synthesize-green/SKILL.md     # ≤ 450
   wc -w .github/skills/outside-in-tdd/SKILL.md           # ≤ 600
   wc -w .github/skills/outside-in-tdd/references/*.md    # reference budget
   ```
2. Duplication check:
   ```bash
   grep -rn "surviving mutant\|0 surviving" .github/skills/
   grep -rn "Compilation.*RED\|compilation.*red" .github/skills/
   grep -rn "A.Fake\|Mock only Infrastructure" .github/skills/
   ```
3. Orphan check:
   ```bash
   grep -rn "domain-layer-testing\|application-layer-testing" .github/
   ```
4. Frontmatter: both skills have `name` (hyphens only) + `description` (starts "Use when...", no workflow summary, < 500 chars).
5. Final directory structure:
   ```
   .github/skills/
   ├── red-synthesize-green/
   │   └── SKILL.md
   ├── outside-in-tdd/
   │   ├── SKILL.md
   │   ├── assets/
   │   │   ├── CommandHandlerTestTemplate.cs
   │   │   └── QueryHandlerTestTemplate.cs
   │   └── references/
   │       ├── mutation-policy.md
   │       ├── testing-strategy.md
   │       ├── cqrs-patterns.md
   │       └── test-examples.md
   ├── clean-architecture-dotnet/
   ├── reviewing-business-terminology/
   └── updating-business-lexicon/
   ```

---

## Files Summary

| Action | File |
|---|---|
| **CREATE** | `red-synthesize-green/SKILL.md` |
| **CREATE** | `outside-in-tdd/references/mutation-policy.md` |
| **HEAVY REWRITE** | `outside-in-tdd/SKILL.md` (1866 → ~550) |
| **MOVE** | `application-layer-testing/references/*.md` → `outside-in-tdd/references/` |
| **MOVE** | `application-layer-testing/assets/*.cs` → `outside-in-tdd/assets/` |
| **EDIT** | `outside-in-tdd/references/testing-strategy.md` |
| **EDIT** | `outside-in-tdd/references/test-examples.md` (add Domain examples) |
| **DELETE** | `domain-layer-testing/` (entire folder) |
| **DELETE** | `application-layer-testing/` (entire folder) |
| **DELETE** | `outside-in-tdd/references/mutation-testing.md` (after move) |

---

## Risk Assessment

| Risk | Mitigation |
|---|---|
| `red-synthesize-green` too abstract without examples | Include one minimal pseudocode example (stack-agnostic) to ground each step |
| `outside-in-tdd` becomes bloated absorbing 2 skills | Heavy reference offloading — SKILL.md stays ~550 words, details in references/ |
| Agent doesn't load `red-synthesize-green` when doing TDD | Descriptive frontmatter with TDD/RED/GREEN keywords + `**REQUIRED PROCESS:**` in outside-in-tdd |
| Breaking old skill references in code comments or docs | Phase 5 grep catches all orphans |
| Naming confusion: `red-synthesize-green` vs `test-driven-development` (existing superpowers skill) | Description clearly states "AI-optimized 2-step cycle" — distinct from classical TDD skill |

---

## Naming Options for the Process Skill

| Name | Pros | Cons |
|---|---|---|
| `red-synthesize-green` | Descriptive, maps to the 2 steps, memorable | Long |
| `ai-tdd-cycle` | Short, clear audience | Sounds like buzzword |
| `synthesize-green-tdd` | Highlights differentiator | Less intuitive step order |
| `two-step-tdd` | Simple | Doesn't convey the AI synthesis aspect |

**Recommended: `red-synthesize-green`** — it names the actual skill being taught and is immediately recognizable to anyone who knows TDD.
