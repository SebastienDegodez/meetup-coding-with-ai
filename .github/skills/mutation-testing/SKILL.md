---
name: mutation-testing
description: Use when running mutation testing, killing mutants, verifying test quality, checking mutation score, or analyzing survivors after the test baseline is green
---

# Mutation Testing

Add a third validation layer to Outside-In TDD workflow.
Acceptance tests verify WHAT (observable behavior), Domain tests verify HOW (business rules), mutation testing verifies tests **actually catch bugs**.

## Core Concept

Mutation testing introduces deliberate bugs (mutants) into source code, then runs the test suite. If tests fail, the mutant is **killed** ✓. If tests pass despite the bug, the mutant **survives** ✗ (test gap found).

```
Source code → introduce mutation → run tests
                                     ├── tests FAIL → mutant killed ✓
                                     └── tests PASS → mutant survived ✗
```

A project with 100% code coverage can still have a 60% mutation score — meaning 40% of introduced bugs go undetected.

## When to Use

Run mutation testing **after the relevant test baseline is green**:

1. ✅ Core behavior tests pass
2. ✅ Rule-focused tests pass
3. 🧬 **Mutation testing** — verify tests detect regressions

**Never run on red baseline** — mutation assumes tests work correctly first.

## Approach for .NET/C#

### Primary: Stryker.NET (Recommended)

For .NET projects, [Stryker.NET](https://stryker-mutator.io/docs/stryker-net/introduction/) is the established mutation framework with excellent C# support.

> **No `stryker-config.json` — by design.** This project does NOT use a Stryker
> config file. Never create one, and never run bare `dotnet stryker` (it relies
> on a config or mutates everything). Always pass `--project`, `-tp`, and
> `--since` explicitly so the run is reproducible and scoped to the diff.

**Install (only if not already available):**
```bash
# Check first — if this succeeds, skip installation entirely. Do NOT manipulate PATH.
dotnet stryker --version

# Only run if the above command fails (tool not found)
dotnet tool install -g dotnet-stryker
```

**Primary command — diff vs the destination branch, no config file:**
```bash
# Mutate the Domain project, run its UnitTests, scope to the PR diff.
# Replace <target-branch> with the branch you will merge INTO (the PR base).
dotnet stryker \
  --project YourProject.Domain.csproj \
  -tp tests/YourProject.UnitTests/YourProject.UnitTests.csproj \
  --since:<target-branch> \
  -r markdown -r json -r cleartext
```

> **Why `--since`:** it mutates only the code changed between the current branch
> and `<target-branch>` (the PR base) — fast, focused on what the PR actually
> touches. In CI, resolve the base from the PR event and prefix with the remote
> if needed (e.g. `--since:origin/main`). If the diff touches no Domain code,
> Stryker reports zero mutants — that is a valid PASS, not a failure.

> **`--project` takes the .csproj FILE NAME, not a path** — Stryker locates it
> inside the solution. The no-config form is `--project` + `-tp` + `--since`,
> with NO `--mutate` path globs.

> ⚠️ **Footgun — project-relative `--mutate`:** `--mutate` globs are resolved
> RELATIVE TO THE MUTATED PROJECT directory, not the solution root. A pattern
> like `--mutate "src/YourProject.Domain/**/*.cs"` matches NOTHING from inside
> the Domain project, so every mutant is silently "Removed by mutate filter"
> and Stryker reports *"unable to calculate a mutation score"*. To exclude
> files, use suffix-only EXCLUDE patterns (`--mutate "!**/*Marker.cs"`); never
> prefix an include glob with a solution-relative path.

**Exclude non-logic files (combine with `--since`, suffix-only patterns are safe):**
```bash
dotnet stryker \
  --project YourProject.Domain.csproj \
  -tp tests/YourProject.UnitTests/YourProject.UnitTests.csproj \
  --since:<target-branch> \
  --mutate "!**/*Marker.cs" --mutate "!**/DependencyInjection.cs" \
  -r markdown -r json -r cleartext
```

**Cumulative baseline — full picture across PRs:**
```bash
# --with-baseline = --since + a persistent baseline report. Keeps a full score
# history in CI while only re-testing code changed vs the target branch.
dotnet stryker \
  --project YourProject.Domain.csproj \
  -tp tests/YourProject.UnitTests/YourProject.UnitTests.csproj \
  --with-baseline:<target-branch> \
  -r markdown -r json
```

### Alternative: Custom Mutation Tool (For Specific Needs)

Build a custom tool only when:
- Stryker doesn't cover domain-specific mutation patterns
- You need tight integration with custom test infrastructure
- Performance optimization requires targeted mutation scope

**Architecture (3 modules):**
1. **Mutations** — rules table (`+` → `-`, `true` → `false`, `>=` → `>`)
2. **Runner** — source-to-test mapping, targeted test execution
3. **Core** — orchestration: apply mutation → run tests → restore → report

For full custom tool reference, see Uncle Bob's [empire-2025 mutation testing](https://github.com/unclebob/empire-2025/blob/master/docs/plans/2026-02-21-mutation-testing.md).

## Core Mutation Categories

| Category | Examples |
|----------|----------|
| Arithmetic | `+` ↔ `-`, `*` ↔ `/`, `++` ↔ `--` |
| Comparison | `>` ↔ `>=`, `<` ↔ `<=`, `==` ↔ `!=` |
| Boolean | `true` ↔ `false`, `&&` ↔ `\|\|`, `!x` ↔ `x` |
| Conditional | negate conditions, swap if/else branches |
| Constant | `0` ↔ `1`, `""` ↔ `"mutant"`, `null` ↔ `new()` |
| Return value | `return true` → `return false` |
| Void method | remove method call entirely |
| LINQ | `.Any()` ↔ `.All()`, `.First()` ↔ `.Last()` |

## Workflow

> **Universal prerequisite — applies to every step, every scenario:**
> Before any mutation activity (first run, CI setup, killing survivors, analyzing reports), the test suite for the affected scope must be **green**. If tests are failing, fix them first. Mutation results on a red baseline are meaningless — failing tests cannot kill mutants they already can't run.

### Step 1: Verify Prerequisites

Before running mutation testing, confirm:
- ✅ Baseline tests are green for the mutated scope
- ✅ Meaningful unit tests exist (mutation runs against unit tests)
- ✅ No uncommitted changes (mutations modify source temporarily)
- ✅ Tests are fast (< 100ms each) — slow tests = slow mutation runs

### Step 2: Set Mutation Scope

**Target critical business logic first:**
- Domain policies, decision engines, pricing/risk calculators
- Application orchestration with complex conditionals
- Validation rules and boundary behavior

**Exclude from mutation:**
- DTOs, data structures without logic
- Infrastructure (repositories, adapters)
- Configuration, DependencyInjection files
- Generated code, marker interfaces

**Progressive scoping:**
| Phase | Scope | Goal |
|-------|-------|------|
| Week 1-2 | One critical rule module | Baseline + learning |
| Week 3-4 | All core rule modules | Establish quality gate |
| Ongoing | Core + critical orchestration handlers | Full confidence |

### Step 3: Run Mutations

**Scope to the PR diff (default — no config file, compare vs the target branch):**
```bash
dotnet stryker \
  --project YourProject.Domain.csproj \
  -tp tests/YourProject.UnitTests/YourProject.UnitTests.csproj \
  --since:<target-branch> \
  -r markdown -r json -r cleartext
```

> `<target-branch>` is the PR base (the branch you merge INTO). In CI, resolve it
> from the PR event and prefix with the remote if needed (`--since:origin/main`).

**Metrics:**
- Total mutants generated
- Mutants killed (tests caught the bug ✓)
- Mutants survived (test gap ✗)
- Mutation score: (killed / total) × 100

> **`--since` note:** unchanged files produce no result — this is expected. Survivors and kills only apply to the diff scope. A PR with no Domain changes yields zero mutants (valid PASS).

**Expected duration:** `--since` run: ~1-3 min on the changed scope.

### Step 4: Analyze Survivors

Query survivors directly from the JSON report — do **not** read the full file:

```bash
jq '[.files | to_entries[] | {file: .key, survivors: [.value.mutants[] | select(.status == "Survived") | {mutator: .mutatorName, line: .location.start.line, replacement: .replacement}]}] | map(select(.survivors | length > 0))' \
  StrykerOutput/$(ls -t StrykerOutput | head -1)/reports/mutation-report.json
```

For each surviving mutant:
1. **Read the mutation** — what was changed? (e.g., `>=` → `>`, removed `if` branch)
2. **Identify unguarded behavior** — which business rule isn't tested?
3. **Categorize:**
   - **Real gap** — behavior change not caught by tests
   - **Equivalent mutant** — mutation doesn't change observable behavior

**Equivalent mutant examples:**
- `x = x + 0` changed to `x = x + 1` (dead code)
- Logging statements removed (no observable effect)
- Defensive null checks when value is guaranteed non-null by type

**After classifying survivors, always include a targeted re-run command** scoped to the files that contain real gaps — this confirms kills after you write new tests and gives reviewers a runnable artifact:

```bash
dotnet stryker \
  --project YourProject.Domain.csproj \
  -tp tests/YourProject.UnitTests/YourProject.UnitTests.csproj \
  --mutate "**/<FileWithRealGap>.cs" \
  --mutate "!**/*Marker.cs" --mutate "!**/DependencyInjection.cs" \
  -r cleartext
```

### Step 5: Kill Surviving Mutants

For each **real survivor** (not equivalent):

1. Write a new test targeting the unguarded behavior
2. Run test against **mutated code** (using Stryker's mutation operator):
   - Expected: test FAILS (catches the bug)
3. Run test against **original code**:
   - Expected: test PASSES
4. Re-run Stryker to confirm kill

**Example:**

Survivor: `if (age >= 18)` mutated to `if (age > 18)` → survived

```csharp
// New test to kill the boundary mutant
[Fact]
public void WhenDriverIsExactly18_ShouldBeEligible()
{
    var policy = new EligibilityPolicy();
    var driver = new DriverInfo(Age: 18, LicenseYears: 1);
    var vehicle = new VehicleInfo(Type: "sedan", Age: 1);

    var result = policy.Evaluate(driver, vehicle);

    Assert.True(result.IsEligible); // Fails if mutant uses `age > 18`
}
```

### Step 6: Report & Document

Present summary with before/after metrics:

```
Mutation Testing Report — Core Business Layer
═══════════════════════════════════════
Scope: YourProject.Core.Policies

Score:  68% → 82% (after killing survivors)
Killed:   82 / 100
Survived:  18 → 10

New tests added: 8
- Boundary tests for age/experience thresholds: 4
- Edge cases for vehicle type combinations: 3
- Null/empty validation: 1

Remaining survivors (equivalent mutants — documented):
- EligibilityPolicy.cs:L42 — removed log statement (no observable effect)
- DriverAge.cs:L15 — defensive null check (guaranteed non-null by type)
```

**Document legitimate survivors** in code comments or architecture decision records.

## Mutation Score Targets

Set thresholds based on team policy and risk profile. Common practice is to start with a progressive threshold and tighten it over time.

| Score | Assessment | Action |
|-------|-----------|--------|
| High threshold met | Healthy signal | Keep survivor review discipline |
| Near threshold | Potential gaps | Add targeted tests for risky survivors |
| Below threshold | Quality risk | Block merge or require mitigation plan |

Equivalent mutants are the only legitimate exception — document them explicitly.

## Progressive Threshold Strategy

| Phase | Threshold | Enforcement |
|-------|-----------|-------------|
| Week 1-2 | Baseline only | Measure, learn mutation categories |
| Week 3-4 | Team-defined threshold (e.g., 80%) | Block PR if below |
| Month 2 | Tightened threshold (e.g., 90%) | Ramp up |
| Steady state | Risk-based target per module | Block merge when policy is not met |

**CI/CD integration:**
```bash
# In CI - scope to the PR diff vs the base branch, fail if survivors remain.
dotnet stryker \
  --project YourProject.Domain.csproj \
  -tp tests/YourProject.UnitTests/YourProject.UnitTests.csproj \
  --since:origin/<target-branch> \
  --break-at [team-threshold] \
  -r markdown -r json
```

> When the CI gate fails, it means survivors remain. **Do not raise the threshold to pass — investigate each survivor first.** Classify them as real gap (write a test) or equivalent mutant (document). Only equivalent mutants are an acceptable reason to adjust the threshold.

## Integration with Outside-In TDD

Mutation testing is the **third validation layer**:

```
1. Gherkin scenarios (WHAT)       → Acceptance tests
2. Business rules (HOW)            → Domain tests  
3. Test effectiveness (REAL?)     → Mutation testing
```

**Workflow integration:**
1. Write Gherkin scenario (outside-in-tdd)
2. RED → validate → SYNTHESIZE GREEN (red-synthesize-green)
3. **After story complete:** Run mutation testing on affected business logic modules
4. Kill critical survivors before merge

## Anti-Patterns

### "Let me mutate before tests are green"
No. Fix failing tests first. Mutation assumes a green baseline.

### "100% is unrealistic"
Aggressive targets can be appropriate for critical logic, but thresholds are a policy decision. Equivalent mutants remain the only valid exception to survivor cleanup.

### "Mutate everything including Infrastructure"
Never mutate repositories, adapters, and pure plumbing. Focus on business logic first.

### "Run mutations on every commit"
Too slow. Run on feature completion or weekly. CI runs only on PR.

### "Scope with --mutate \"src/Project/**/*.cs\""
Footgun. `--mutate` globs are resolved relative to the MUTATED PROJECT, not the
solution root. A solution-relative path glob matches zero files, every mutant is
"Removed by mutate filter", and Stryker reports "unable to calculate a mutation
score". Mutate the whole project (`--project` + `-tp`, no include glob) and use
suffix-only EXCLUDE patterns (`--mutate "!**/*Marker.cs"`).

### "Generate the HTML report in CI"
The interactive HTML report is heavy and unread by reviewers. Use the lighter
`markdown` + `json` reporters (`-r markdown -r json`): markdown is a compact
human summary for the PR comment, json is machine-readable for survivor queries.

### "Ignore all survivors as equivalent"
Rationalization. Most survivors are real gaps. Investigate each one.

### "Chase the score, not the quality"
Mutation score is a signal, not the goal. Focus on killing mutants that represent real behavioral gaps.

## Common Mistakes

| Mistake | Fix |
|---------|-----|
| Running mutation on failing tests | Green baseline required — fix tests first |
| Mutating test files | Configure Stryker to mutate source only |
| Solution-relative `--mutate` path glob | Mutate whole project (no include glob); exclude with suffix-only `!**/*Marker.cs` |
| Generating the heavy HTML report in CI | Use lighter `-r markdown -r json` reporters |
| Treating all survivors as equivalent | Only equivalent mutants are exempt — document them, kill the rest |
| Mutation testing without fast tests | Optimize test speed — slow tests = slow mutations |
| Not scoping mutations progressively | Start small (one policy), expand gradually |
| Accepting < 100% on business logic | 100% is the target — find the gap and test it |

## Tools & Commands

**Install / update Stryker.NET:**
```bash
dotnet tool install -g dotnet-stryker
dotnet tool update -g dotnet-stryker
```

**Scope to the PR diff (default — compare vs the target branch, no config):**
```bash
dotnet stryker \
  --project YourProject.Domain.csproj \
  -tp tests/YourProject.UnitTests/YourProject.UnitTests.csproj \
  --since:<target-branch> \
  -r markdown -r json -r cleartext
```

**Cumulative baseline in CI (full picture + incremental speed):**
```bash
dotnet stryker \
  --project YourProject.Domain.csproj \
  -tp tests/YourProject.UnitTests/YourProject.UnitTests.csproj \
  --with-baseline:<target-branch> \
  -r markdown -r json
```

**Scope to a specific file or feature (debug a survivor):**
```bash
dotnet stryker \
  --project YourProject.Domain.csproj \
  -tp tests/YourProject.UnitTests/YourProject.UnitTests.csproj \
  --mutate "**/<TargetFile>.cs" \
  --mutate "!**/*Marker.cs" --mutate "!**/DependencyInjection.cs" \
  -r cleartext
```

**Inspect JSON report:**
```bash
jq '.' StrykerOutput/**/reports/mutation-report.json | head -n 120
```

**Key CLI flags reference:**

| Flag | Short | Purpose |
|------|-------|---------|
| `--project <name.csproj>` | `-p` | Source project to mutate (filename only) |
| `--test-project <path>` | `-tp` | Test project(s) — repeatable |
| `--mutate <glob>` | `-m` | Include/exclude files (prefix `!` to exclude) — repeatable |
| `--since:<committish>` | | Only test mutants in git-diff vs committish |
| `--with-baseline:<committish>` | | Like `--since` + persist baseline for full cumulative report |
| `--break-at <0-100>` | `-b` | Exit code 1 if score < value |
| `--threshold-high <0-100>` | | Score ≥ this → green |
| `--threshold-low <0-100>` | | Score < high but ≥ this → warning |
| `--reporter <name>` | `-r` | `json`, `cleartext`, `dots`, `markdown`, `html` — repeatable |
| `--concurrency <n>` | `-c` | Parallel worker count |
| `--verbosity <level>` | `-V` | `error`, `warning`, `info`, `debug`, `trace` |

## References

- [Stryker.NET Documentation](https://stryker-mutator.io/docs/stryker-net/introduction/)
- [Stryker.NET Configuration](https://stryker-mutator.io/docs/stryker-net/configuration/)
- [Uncle Bob's Mutation Testing Plan](https://github.com/unclebob/empire-2025/blob/master/docs/plans/2026-02-21-mutation-testing.md)
- [Mutation Testing Patterns](https://stryker-mutator.io/docs/mutation-testing-elements/supported-mutators/)

## Integration

**REQUIRED BACKGROUND:** `superpowers-whetstone:outside-in-tdd` — defines the two test streams (Application + Domain)
**REQUIRED BACKGROUND:** `superpowers-whetstone:red-synthesize-green` — TDD cycle that produces tests to mutate

**WORKFLOW:**  
Run mutation testing after story completion, before PR/merge. Use as quality gate, not coverage metric.
