# Testing Skill Refactor Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Split 3 testing skills (outside-in-tdd, domain-layer-testing, application-layer-testing) into 2 skills with clean process-vs-knowledge separation: `red-synthesize-green` (universal AI TDD cycle) + `outside-in-tdd` (complete DDD testing knowledge).

**Architecture:** Extract the 2-step AI TDD cycle (RED → SYNTHESIZE GREEN) into a new standalone, stack-agnostic process skill. Fuse Application + Domain testing patterns into the reworked `outside-in-tdd` skill which owns all DDD testing knowledge. Reference files move from `application-layer-testing/` to `outside-in-tdd/`. A new `mutation-policy.md` becomes the single source of truth for mutation workflow.

**Tech Stack:** Copilot custom skills (Markdown SKILL.md files with YAML frontmatter), C# templates (.cs assets)

**Design doc:** `docs/plans/2026-03-03-refactor-testing-skills-v3.md`

---

### Task 1: Create `red-synthesize-green/SKILL.md`

**Files:**
- Create: `.github/skills/red-synthesize-green/SKILL.md`

**Step 1: Create the new process skill file**

```markdown
---
name: red-synthesize-green
description: Use when following TDD to implement any feature or fix — defines the AI-optimized 2-step cycle where RED means behavior failure only and SYNTHESIZE GREEN produces clean code without a refactor phase
---

# RED → SYNTHESIZE GREEN (AI TDD Cycle)

## Overview

2-step cycle replacing traditional 3-step TDD. Optimized for AI synthesis.

- **Traditional (3 steps):** RED → green (dirty) → Refactor
- **AI-Optimized (2 steps):** RED (behavior failure) → SYNTHESIZE GREEN (clean synthesis)

Architectural guidance is optional framing between those two execution steps.

**Hard rule:** No implementation code before RED is a clean behavior failure.

## Step 1: RED (Behavior Failure Only)

Write the failing test. Run it.

- Compilation errors = **wishful thinking phase** → implement stubs/empty returns to compile, rerun
- Assertion/behavior failure = **RED** ✓ → proceed to Step 2
- Never treat compilation errors as RED

**Programming by Wishful Thinking:** When your test won't compile, you're discovering the API you need. Stub just enough to compile, then confirm the test fails on behavior.

## Between Steps: Architectural Guidance (Optional)

Orient the design approach before synthesis:

- Which pattern? (specification, factory, builder)
- Which layer owns the logic?
- Immutability, return values vs mutations?

This is direction-setting, not micro-management.

## Step 2: SYNTHESIZE GREEN (Clean Synthesis)

Implement complete, clean, production-ready solution in one shot.

- Follows all architectural rules and coding standards
- No dirty-then-refactor — synthesize properly from the start
- Idiomatic code, domain semantics, SOLID principles
- If test was misunderstood → revise test, restart from RED

**No iteration after SYNTHESIZE GREEN** unless RED was wrong or architectural guidance changed.

## Common Rationalizations

| Excuse | Reality |
|---|---|
| "Compilation error IS red" | No. Compilation = wishful thinking. RED = behavior failure. |
| "I'll write dirty code then refactor" | That's 3-step TDD. SYNTHESIZE GREEN produces clean code. |
| "I can skip RED, I know it'll fail" | Run it. RED proves your test catches real failures. |
| "Three steps is safer" | Three steps was a human workaround. Use SYNTHESIZE GREEN. |
| "I'll write domain objects first" | Still code before RED. Design emerges from test needs. |

## Red Flags — STOP and Restart

- Writing implementation code before RED is a behavior failure
- Treating compilation errors as RED and jumping to SYNTHESIZE GREEN
- Skipping RED entirely or "manually testing" instead
- Changing the test after seeing the failure (instead of fixing code)
- Refining code after SYNTHESIZE GREEN instead of revising RED

**All of these mean:** Delete code, start over with proper RED.

## Quick Reference

| Phase | What | Success Criteria |
|---|---|---|
| **RED** | Write test, stub until compiles, run | Test fails on **behavior** (assertion), not compilation |
| **Guidance** (optional) | Orient architectural approach | Design direction clear |
| **SYNTHESIZE GREEN** | Synthesize complete clean solution | Tests green, architecture respected, production-ready |

## Integration

**REQUIRED BACKGROUND:** `test-driven-development` skill (TDD discipline foundation — this skill supersets it).
Pair with domain-specific testing skills for patterns and examples.
```

**Step 2: Validate word count**

Run:
```bash
wc -w .github/skills/red-synthesize-green/SKILL.md
```
Expected: ≤ 450 words

**Step 3: Verify no C# code blocks exist**

Run:
```bash
grep -c '```csharp' .github/skills/red-synthesize-green/SKILL.md
```
Expected: `0`

**Step 4: Commit**

```bash
git add .github/skills/red-synthesize-green/SKILL.md
git commit -m "feat: create red-synthesize-green process skill (stack-agnostic AI TDD cycle)"
```

---

### Task 2: Move reference files to `outside-in-tdd/references/`

**Files:**
- Move: `.github/skills/application-layer-testing/references/testing-strategy.md` → `.github/skills/outside-in-tdd/references/testing-strategy.md`
- Move: `.github/skills/application-layer-testing/references/cqrs-patterns.md` → `.github/skills/outside-in-tdd/references/cqrs-patterns.md`
- Move: `.github/skills/application-layer-testing/references/test-examples.md` → `.github/skills/outside-in-tdd/references/test-examples.md`

**Step 1: Create `outside-in-tdd/references/` directory and move files**

Run:
```bash
mkdir -p .github/skills/outside-in-tdd/references
mv .github/skills/application-layer-testing/references/testing-strategy.md .github/skills/outside-in-tdd/references/
mv .github/skills/application-layer-testing/references/cqrs-patterns.md .github/skills/outside-in-tdd/references/
mv .github/skills/application-layer-testing/references/test-examples.md .github/skills/outside-in-tdd/references/
```

**Step 2: Verify files landed correctly**

Run:
```bash
ls -la .github/skills/outside-in-tdd/references/
```
Expected: 3 files — `testing-strategy.md`, `cqrs-patterns.md`, `test-examples.md`

**Step 3: Commit**

```bash
git add .github/skills/outside-in-tdd/references/ .github/skills/application-layer-testing/references/
git commit -m "refactor: move reference files from application-layer-testing to outside-in-tdd"
```

---

### Task 3: Move asset files to `outside-in-tdd/assets/`

**Files:**
- Move: `.github/skills/application-layer-testing/assets/CommandHandlerTestTemplate.cs` → `.github/skills/outside-in-tdd/assets/CommandHandlerTestTemplate.cs`
- Move: `.github/skills/application-layer-testing/assets/QueryHandlerTestTemplate.cs` → `.github/skills/outside-in-tdd/assets/QueryHandlerTestTemplate.cs`

**Step 1: Create `outside-in-tdd/assets/` directory and move files**

Run:
```bash
mkdir -p .github/skills/outside-in-tdd/assets
mv .github/skills/application-layer-testing/assets/CommandHandlerTestTemplate.cs .github/skills/outside-in-tdd/assets/
mv .github/skills/application-layer-testing/assets/QueryHandlerTestTemplate.cs .github/skills/outside-in-tdd/assets/
```

**Step 2: Verify files landed correctly**

Run:
```bash
ls -la .github/skills/outside-in-tdd/assets/
```
Expected: 2 files — `CommandHandlerTestTemplate.cs`, `QueryHandlerTestTemplate.cs`

**Step 3: Commit**

```bash
git add .github/skills/outside-in-tdd/assets/ .github/skills/application-layer-testing/assets/
git commit -m "refactor: move test templates from application-layer-testing to outside-in-tdd"
```

---

### Task 4: Create `outside-in-tdd/references/mutation-policy.md`

**Files:**
- Create: `.github/skills/outside-in-tdd/references/mutation-policy.md`
- Source content from: `.github/skills/application-layer-testing/references/mutation-testing.md` (authoritative workflow parts only)

This file becomes the **single source of truth** for mutation policy across all skills. It replaces the duplicated mutation content that currently lives in 4 places.

**Step 1: Create the authoritative mutation policy file**

```markdown
# Mutation Policy

## Goal

0 surviving mutants for Domain and Application code. Mutation testing validates that tests verify behavior, not just coverage.

## Tool: Stryker.NET

```bash
# Install (one-time)
dotnet tool install -g dotnet-stryker

# Run from test project directory
dotnet stryker

# With config
dotnet stryker --config-file stryker-config.json

# CI: only mutate changed code since base branch
dotnet stryker --since:main
```

## Workflow When Mutants Survive

1. **Inspect** the Stryker HTML report (`StrykerOutput/reports/mutation-report.html`)
2. **Map** each surviving mutant to missing business behavior — not technical gaps
3. **Propose** new functional tests to the user (Given/When/Then style)
4. **Wait** for user validation before implementing
5. **Implement** only validated tests
6. **Rerun** mutation tests — repeat until 0 surviving mutants

**Hard rule:** Proposed tests must be functional and business-oriented, never technical micro-tests.

## Common Mutations to Watch

| Mutation Type | Example | Fix |
|---|---|---|
| Arithmetic boundary | `>` → `>=` | Test edge value (e.g., exactly 0) |
| Logical operator | `&&` → `\|\|` | Test with one condition false |
| Return value | `return true` → `return false` | Assert on return value |
| Statement removal | `order.Confirm()` removed | Verify state change after call |

## Configuration

```json
{
  "stryker-config": {
    "mutate": [
      "!**/*ViewModel.cs",
      "!**/*Dto.cs",
      "!**/*Command.cs",
      "!**/*Query.cs",
      "**/*.cs"
    ],
    "thresholds": {
      "high": 100,
      "low": 100,
      "break": 100
    },
    "reporters": ["html", "progress", "cleartext"]
  }
}
```

**Exclude from mutation:** DTOs, ViewModels, Commands, Queries (no logic).
**Focus on:** Handlers and Domain objects (business logic).

## CI Integration

Run mutation tests on changed code only (since base branch) in CI pipelines. Run full mutation tests locally before PR.
```

**Step 2: Validate word count**

Run:
```bash
wc -w .github/skills/outside-in-tdd/references/mutation-policy.md
```
Expected: ~200–250 words

**Step 3: Commit**

```bash
git add .github/skills/outside-in-tdd/references/mutation-policy.md
git commit -m "feat: create single-source-of-truth mutation-policy.md in outside-in-tdd"
```

---

### Task 5: Edit `testing-strategy.md` — remove duplication

**Files:**
- Modify: `.github/skills/outside-in-tdd/references/testing-strategy.md`

Remove content now owned by other files:
- Remove the "Mutation Testing" references (now in `mutation-policy.md`)
- Remove the "Feature Implementation Workflow" TDD cycle steps (now in `red-synthesize-green`)
- Add clearer "When Application vs Domain" routing section

**Step 1: Rewrite `testing-strategy.md`**

Replace the entire file content with:

```markdown
# Testing Strategy: Sociable Tests for DDD

## Philosophy

This testing strategy follows Martin Fowler's **sociable testing** approach:

- **Sociable tests** use real collaborators from within the same layer or below
- **Solitary tests** (with mocks) are used only for external dependencies

## What to Test Where

### Application Layer Tests (primary focus)
- Test use cases (Command/Query handlers) with real Domain objects
- Mock only Infrastructure dependencies (repositories, external services)
- Validate the entire business flow including Domain logic
- Located in: `tests/[Project].UnitTests/Application/`

### Domain Layer Tests (when needed)
- Test complex domain logic that needs isolated validation
- Test specifications, complex calculations, or critical invariants
- Most Domain logic is already tested through Application tests
- Located in: `tests/[Project].UnitTests/Domain/`

### Integration Tests (full stack)
- Test API endpoints with real infrastructure (Testcontainers)
- Located in: `tests/[Project].IntegrationTests/`

## When Application vs Domain Tests

| Signal | Route to |
|---|---|
| Orchestration (load/save/publish/map) | Application test |
| Complex business rules with edge-case matrices | Domain test |
| Aggregate invariants across state transitions | Domain test |
| Value object validation with boundary conditions | Domain test |
| Domain service with non-trivial policy logic | Domain test |
| Simple rule adequately covered by handler test | Don't duplicate — Application test is enough |

**Default:** Start with Application test. Add Domain test only when complexity warrants it.

## Testing Rules

### DO ✅
- Test handlers with real Domain objects (aggregates, VOs, services)
- Mock only Infrastructure layer (repositories, external services)
- Keep tests fast (no Testcontainers, no DB, no network, < 100ms)
- Name tests with business language (`WhenDoingSomething_ShouldExpectedBehavior`)
- Verify Domain state changes through observable outcomes

### DON'T ❌
- Don't mock Domain objects (`A.Fake<Order>()` — never)
- Don't centralize strategic rules in handlers — keep them in Domain
- Don't use Testcontainers in unit tests — save for Integration
- Don't test implementation details — test behavior

## Benefits

1. **Fast execution**: No external dependencies in unit tests
2. **Real behavior**: Tests verify actual Domain logic, not mocks
3. **Refactoring safety**: Tests break only when behavior changes
4. **Clear intent**: Tests show how Domain and Application work together
5. **Maintainability**: Fewer mocks = less maintenance overhead
```

**Step 2: Validate word count**

Run:
```bash
wc -w .github/skills/outside-in-tdd/references/testing-strategy.md
```
Expected: ~300–350 words (down from ~460)

**Step 3: Commit**

```bash
git add .github/skills/outside-in-tdd/references/testing-strategy.md
git commit -m "refactor: trim testing-strategy.md, add Application vs Domain routing table"
```

---

### Task 6: Enrich `test-examples.md` with Domain examples

**Files:**
- Modify: `.github/skills/outside-in-tdd/references/test-examples.md`

Add a new "Example 3: Domain Logic Test" section at the end, using the EligibilityPolicy example from the old `domain-layer-testing` skill. This ensures Domain testing examples are in one place.

**Step 1: Append Domain example to `test-examples.md`**

Add the following content at the very end of the file (after the last example):

````markdown

## Example 3: Domain Logic Test (Pure — No Mocks)

### Domain: Eligibility Policy

```csharp
namespace MonAssurance.Domain.Eligibility;

public sealed class EligibilityPolicy
{
    public EligibilityResult Evaluate(DriverInfo driver, VehicleInfo vehicle)
    {
        if (driver.Age < 18)
            return EligibilityResult.Rejected("driver_under_minimum_age");

        if (driver.LicenseYears < 2)
            return EligibilityResult.Rejected("insufficient_license_experience");

        if (vehicle.Age > 15)
            return EligibilityResult.Rejected("vehicle_too_old");

        return EligibilityResult.Eligible();
    }
}
```

### Test: Domain Policy Tests

```csharp
namespace MonAssurance.UnitTests.Domain.Eligibility;

public sealed class EligibilityPolicyTests
{
    private readonly EligibilityPolicy _policy = new();

    [Fact]
    public void WhenDriverIsUnder18_ShouldBeIneligible()
    {
        var driver = new DriverInfo(Age: 17, LicenseYears: 0);
        var vehicle = new VehicleInfo(Type: "sedan", Age: 1);

        var result = _policy.Evaluate(driver, vehicle);

        Assert.False(result.IsEligible);
        Assert.Equal("driver_under_minimum_age", result.RejectionReason);
    }

    [Fact]
    public void WhenDriverHasInsufficientExperience_ShouldBeIneligible()
    {
        var driver = new DriverInfo(Age: 25, LicenseYears: 1);
        var vehicle = new VehicleInfo(Type: "sedan", Age: 3);

        var result = _policy.Evaluate(driver, vehicle);

        Assert.False(result.IsEligible);
        Assert.Equal("insufficient_license_experience", result.RejectionReason);
    }

    [Fact]
    public void WhenVehicleIsTooOld_ShouldBeIneligible()
    {
        var driver = new DriverInfo(Age: 30, LicenseYears: 10);
        var vehicle = new VehicleInfo(Type: "sedan", Age: 16);

        var result = _policy.Evaluate(driver, vehicle);

        Assert.False(result.IsEligible);
        Assert.Equal("vehicle_too_old", result.RejectionReason);
    }

    [Fact]
    public void WhenAllCriteriaMet_ShouldBeEligible()
    {
        var driver = new DriverInfo(Age: 30, LicenseYears: 10);
        var vehicle = new VehicleInfo(Type: "sedan", Age: 3);

        var result = _policy.Evaluate(driver, vehicle);

        Assert.True(result.IsEligible);
    }
}
```

### Key Differences from Application Tests

| Aspect | Application Test | Domain Test |
|---|---|---|
| Dependencies | Mock Infrastructure | None (pure) |
| Subject | Handler (orchestrator) | Aggregate/VO/Service |
| Assertions | Infrastructure calls + Domain state | Domain state only |
| When to use | Orchestration flows | Complex business rules, edge matrices |
````

**Step 2: Validate word count**

Run:
```bash
wc -w .github/skills/outside-in-tdd/references/test-examples.md
```
Expected: ~1200–1300 words (was ~946, added ~300)

**Step 3: Commit**

```bash
git add .github/skills/outside-in-tdd/references/test-examples.md
git commit -m "feat: add Domain logic test examples (EligibilityPolicy) to test-examples.md"
```

---

### Task 7: Light trim of `cqrs-patterns.md`

**Files:**
- Modify: `.github/skills/outside-in-tdd/references/cqrs-patterns.md`

Remove the "Testing Commands vs Queries" section at the end of the file (if it exists — it's a partial duplicate of what's now in `test-examples.md` and the main SKILL.md).

**Step 1: Check for the testing section**

Run:
```bash
grep -n "Testing Commands vs Queries" .github/skills/outside-in-tdd/references/cqrs-patterns.md
```
Expected: Line number where "Testing Commands vs Queries" heading appears.

**Step 2: If found, remove everything from that heading to end of file**

Truncate the file at that heading. Keep everything above it intact.

**Step 3: Validate**

Run:
```bash
wc -w .github/skills/outside-in-tdd/references/cqrs-patterns.md
```
Expected: ~450–500 words (was ~569)

**Step 4: Commit**

```bash
git add .github/skills/outside-in-tdd/references/cqrs-patterns.md
git commit -m "refactor: trim duplicate testing section from cqrs-patterns.md"
```

---

### Task 8: Rewrite `outside-in-tdd/SKILL.md`

**Files:**
- Rewrite: `.github/skills/outside-in-tdd/SKILL.md`

This is the heaviest task. Replace the current 1866-word SKILL.md with a ~550-word version that:
- Absorbs `domain-layer-testing` content (Domain Tests section, anti-patterns)
- Absorbs `application-layer-testing` content (Application Tests section, testing rules)
- Removes the 2-step cycle (now in `red-synthesize-green`)
- Removes rationalizations/red flags (now in `red-synthesize-green`)
- Removes the misleading multi-stack table
- Removes full inline code examples (replaced by minimal ~8-line snippets)
- Links to the process skill and references

**Step 1: Replace `outside-in-tdd/SKILL.md` with new content**

Replace the entire file with:

```markdown
---
name: outside-in-tdd
description: Use when writing unit tests for DDD Clean Architecture layers — covers outside-in testing with Gherkin scenarios, Application handler orchestration tests, Domain logic tests, mocking rules, and mutation policy
---

# Outside-In DDD Testing

## Overview

Complete testing guide for Application and Domain layers in Clean Architecture.
Start with observable business behavior (Gherkin), let design emerge from tests.

**Core rule:** Real Domain objects, mocked Infrastructure only, fast in-memory tests.

## Outside-In Approach

1. **Gherkin scenario** (Given/When/Then) — describes WHAT the customer/system does, not HOW
2. **Map to Application test** — test the handler entry point, mock only Infrastructure ports
3. **Domain emerges** — test failures guide what Domain objects and rules you need

**Not outside-in:** Mocking database setup first, writing integration tests before unit tests, building the API before understanding the business rule.

## Application Tests (Sociable — Handler Level)

Test command/query handlers with real Domain objects. Mock only Infrastructure (repositories, external services). Verify orchestration flow + Domain state outcomes.

```csharp
[Fact]
public async Task WhenSubmittingValidApplication_ShouldPersistPendingApplication()
{
    var repository = A.Fake<IApplicationRepository>();
    var handler = new SubmitApplicationCommandHandler(repository);
    var command = new SubmitApplicationCommand(CustomerId.CreateNew(),
        new DriverInfo(Age: 25, LicenseYears: 3), new VehicleInfo(Type: "sedan", Age: 2));

    await handler.Handle(command);

    A.CallTo(() => repository.AddAsync(
        A<Application>.That.Matches(a => a.Status == ApplicationStatus.Pending),
        A<CancellationToken>._)).MustHaveHappenedOnceExactly();
}
```

## Domain Tests (Pure — Logic Level)

Test aggregates, value objects, domain services, invariants. No mocks at all — pure state-based assertions. Name tests with business language.

```csharp
[Fact]
public void WhenDriverIsUnder18_ShouldBeIneligible()
{
    var policy = new EligibilityPolicy();
    var driver = new DriverInfo(Age: 17, LicenseYears: 0);
    var vehicle = new VehicleInfo(Type: "sedan", Age: 1);

    var result = policy.Evaluate(driver, vehicle);

    Assert.False(result.IsEligible);
    Assert.Equal("driver_under_minimum_age", result.RejectionReason);
}
```

## When to Write Which

| Signal | Route to |
|---|---|
| Orchestration (load/save/publish) | Application test |
| Complex rules, edge matrices, invariants | Domain test |
| Simple rule covered by handler test | Don't duplicate |

**Default:** Application test first. Add Domain tests when complexity warrants isolation.

## Testing Rules

### DO ✅
- Mock only Infrastructure (repositories, external services)
- Use real Domain objects (aggregates, VOs, services)
- Keep tests fast (< 100ms, no DB, no network)
- Name tests with business language (`WhenCondition_ShouldOutcome`)
- Cover meaningful edge-case combinations for strategic rules

### DON'T ❌
- Don't mock Domain objects (`A.Fake<Order>()` — never)
- Don't centralize strategic rules in handlers — keep in Domain
- Don't use Testcontainers in unit tests — Integration only
- Don't test implementation details — test behavior
- Don't use FluentAssertions — use xUnit native `Assert.*` (licensing)

## Mutation Policy

0 surviving mutants required for Domain + Application. See [references/mutation-policy.md](references/mutation-policy.md) for complete workflow.

**Task is not done while mutants survive.** Propose functional business-oriented tests to the user and wait for validation before implementing.

## Anti-Patterns

- Strategic rules implemented in Application handlers instead of Domain
- Catch-all Domain service with mixed responsibilities
- Over-mocking that hides real business behavior
- Testing infrastructure mapping through handlers
- Treating coverage percentage as the quality target
- Duplicating Application test coverage with redundant Domain tests

## Project Structure

```
tests/[Project].UnitTests/
  Application/
    [Feature]/
      Commands/
        [Action]CommandHandlerTests.cs
      Queries/
        [Action]QueryHandlerTests.cs
  Domain/
    [Feature]/
      [Policy]Tests.cs
```

## Common Mistakes

| Mistake | Fix |
|---|---|
| Mocking Domain objects in Application tests | Use real Domain objects, mock only Infrastructure |
| Writing Domain objects before RED test | Let design emerge from test failures |
| Treating compilation errors as RED | Stub to compile, then confirm behavior failure |
| Accepting surviving mutants | Propose functional tests, validate, implement, rerun |
| Skipping Gherkin ("too small") | Even small features benefit from behavior-first thinking |

## References & Templates

- [references/testing-strategy.md](references/testing-strategy.md) — sociable vs solitary philosophy
- [references/cqrs-patterns.md](references/cqrs-patterns.md) — handler structure, commands vs queries
- [references/test-examples.md](references/test-examples.md) — complete code examples (Application + Domain)
- [references/mutation-policy.md](references/mutation-policy.md) — Stryker.NET workflow, 0-survivor policy
- [assets/CommandHandlerTestTemplate.cs](assets/CommandHandlerTestTemplate.cs) — Command handler test starter
- [assets/QueryHandlerTestTemplate.cs](assets/QueryHandlerTestTemplate.cs) — Query handler test starter

## Integration

**REQUIRED PROCESS:** `red-synthesize-green` (follow the 2-step AI TDD cycle)
**REQUIRED CONTEXT:** `clean-architecture-dotnet` (layer boundaries)
```

**Step 2: Validate word count**

Run:
```bash
wc -w .github/skills/outside-in-tdd/SKILL.md
```
Expected: ≤ 600 words (down from 1866)

**Step 3: Verify no duplication with process skill**

Run:
```bash
grep -c "Compilation.*RED\|compilation.*red\|Rationaliz" .github/skills/outside-in-tdd/SKILL.md
```
Expected: `0` (rationalizations and compilation-vs-RED details live in `red-synthesize-green` only)

**Step 4: Commit**

```bash
git add .github/skills/outside-in-tdd/SKILL.md
git commit -m "refactor: rewrite outside-in-tdd SKILL.md — absorb DDD testing, delegate process to red-synthesize-green"
```

---

### Task 9: Delete old `application-layer-testing/` folder

**Files:**
- Delete: `.github/skills/application-layer-testing/` (entire folder)

At this point all content has been moved or absorbed:
- `references/` → moved to `outside-in-tdd/references/` (Task 2)
- `assets/` → moved to `outside-in-tdd/assets/` (Task 3)
- `SKILL.md` → absorbed into `outside-in-tdd/SKILL.md` (Task 8)
- `references/mutation-testing.md` → replaced by `outside-in-tdd/references/mutation-policy.md` (Task 4)

**Step 1: Delete the folder**

Run:
```bash
rm -rf .github/skills/application-layer-testing/
```

**Step 2: Verify it's gone**

Run:
```bash
ls .github/skills/application-layer-testing/ 2>&1
```
Expected: `No such file or directory`

**Step 3: Commit**

```bash
git add .github/skills/application-layer-testing/
git commit -m "refactor: delete application-layer-testing skill (absorbed into outside-in-tdd)"
```

---

### Task 10: Delete old `domain-layer-testing/` folder

**Files:**
- Delete: `.github/skills/domain-layer-testing/` (entire folder)

Content absorbed into `outside-in-tdd/SKILL.md` Domain Tests section (Task 8) and `test-examples.md` (Task 6).

**Step 1: Delete the folder**

Run:
```bash
rm -rf .github/skills/domain-layer-testing/
```

**Step 2: Verify it's gone**

Run:
```bash
ls .github/skills/domain-layer-testing/ 2>&1
```
Expected: `No such file or directory`

**Step 3: Commit**

```bash
git add .github/skills/domain-layer-testing/
git commit -m "refactor: delete domain-layer-testing skill (absorbed into outside-in-tdd)"
```

---

### Task 11: Final validation

Run all validation checks to confirm the refactor is complete and correct.

**Step 1: Verify final directory structure**

Run:
```bash
find .github/skills/ -type f | sort
```

Expected output:
```
.github/skills/clean-architecture-dotnet/SKILL.md
.github/skills/outside-in-tdd/SKILL.md
.github/skills/outside-in-tdd/assets/CommandHandlerTestTemplate.cs
.github/skills/outside-in-tdd/assets/QueryHandlerTestTemplate.cs
.github/skills/outside-in-tdd/references/cqrs-patterns.md
.github/skills/outside-in-tdd/references/mutation-policy.md
.github/skills/outside-in-tdd/references/test-examples.md
.github/skills/outside-in-tdd/references/testing-strategy.md
.github/skills/red-synthesize-green/SKILL.md
.github/skills/reviewing-business-terminology/SKILL.md
.github/skills/updating-business-lexicon/SKILL.md
```

**Step 2: Validate SKILL.md word counts**

Run:
```bash
echo "--- SKILL.md word counts ---"
echo -n "red-synthesize-green: "; wc -w < .github/skills/red-synthesize-green/SKILL.md
echo -n "outside-in-tdd: "; wc -w < .github/skills/outside-in-tdd/SKILL.md
echo "--- Reference word counts ---"
for f in .github/skills/outside-in-tdd/references/*.md; do echo -n "$(basename $f): "; wc -w < "$f"; done
```

Expected:
- `red-synthesize-green/SKILL.md`: ≤ 450
- `outside-in-tdd/SKILL.md`: ≤ 600
- Total SKILL.md loaded per conversation: ≤ 1050 (down from 3111)

**Step 3: Check for orphan references to deleted skills**

Run:
```bash
grep -rn "domain-layer-testing\|application-layer-testing" .github/
```

Expected: **0 matches.** If any found, fix those references.

**Step 4: Check for content duplication across skills**

Run:
```bash
echo "--- Mutation refs (should only be in outside-in-tdd) ---"
grep -rln "surviving mutant\|0 surviving" .github/skills/
echo "--- Compilation-RED refs (should only be in red-synthesize-green) ---"
grep -rln "Compilation.*RED\|compilation.*red\|wishful thinking" .github/skills/
echo "--- Mock rule refs (should only be in outside-in-tdd) ---"
grep -rln "A.Fake\|Mock only Infrastructure\|Mock Infrastructure" .github/skills/
```

Expected:
- Mutation refs: only `outside-in-tdd/SKILL.md` and `outside-in-tdd/references/mutation-policy.md`
- Compilation-RED refs: only `red-synthesize-green/SKILL.md`
- Mock rule refs: only `outside-in-tdd/` files

**Step 5: Validate frontmatter format**

Run:
```bash
head -5 .github/skills/red-synthesize-green/SKILL.md
head -5 .github/skills/outside-in-tdd/SKILL.md
```

Expected: Both have `---` delimiters with `name:` (lowercase, hyphens) and `description:` (starts with "Use when...").

**Step 6: Final commit (if any fixes were needed)**

```bash
git add .github/skills/
git commit -m "fix: resolve orphan references and duplication from skill refactor"
```
