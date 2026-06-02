---
name: resolving-stack-commands
description: Use whenever an agent must run a toolchain command (build, test, mutation) and needs the concrete invocation. Resolves build/test/mutation commands from the detected stack so no agent hardcodes `dotnet test`, `dotnet build`, or any toolchain command. Loaded by acceptance-designer and software-engineer.
---

# Resolving Stack Commands (stack-agnostic)

The single place that maps a repository's stack to its concrete toolchain
commands (build, test, mutation). No agent and no workflow step may hardcode a
command (`dotnet test`, `dotnet build`, `mvn test`, `pytest`, ...). They resolve
it here instead.

## Why this exists

The pipeline must run with .NET today and other stacks tomorrow (Java planned,
not yet supported). If each agent embeds `dotnet test`, swapping or adding a
stack means editing every agent. Centralizing the mapping keeps agents
tech-agnostic: they say "build the solution" / "run the test suite" / "run
mutation", this skill says how.

## Detection → commands

Detect the stack from markers at the repo root, then use its commands:

| Stack | Detection markers | Build | Test | Mutation | Status |
|---|---|---|---|---|---|
| .NET | `*.sln`, `*.slnx`, `**/*.csproj`, `Directory.Packages.props` | `dotnet build` | `dotnet test` | `dotnet stryker` | supported |
| Java | `pom.xml`, `build.gradle`, `build.gradle.kts` | _(not yet provided)_ | _(not yet provided)_ | _(not yet provided)_ | NOT SUPPORTED |

If multiple stacks coexist, run each supported stack's commands and aggregate results.

## Unsupported stack → stop, never guess

If the detected stack has no supported command, STOP and emit a structured
blocker. Never invent a command:

```yaml
status: blocked
type: unsupported_stack
message: No command mapping for the detected stack
context:
  stack: java
  markers:
    - pom.xml
  needed: test
```

## Contract for callers

- Resolve the command (build / test / mutation) from the table above; do not embed it in workflow steps.
- Run it, then assert on its output (build succeeds, RED on a business assertion, GREEN gate, mutation score, etc.).
- Adding a stack = add a row here (or its `quality-gates-<tech>` adapter), with zero edits to agents.
