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
