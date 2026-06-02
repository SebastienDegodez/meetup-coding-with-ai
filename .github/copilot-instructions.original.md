# Copilot Instructions - MonAssurance

## Dependencies

- .NET 10.0
- xUnit for testing
- FakeItEasy for mocking
- NetArchTest.Rules for architecture validation
- Microsoft.Extensions.DependencyInjection for DI

## ADR
Les ADR sont écrits en français.

## Agent Response Language

- All SKRAFT agents (orchestrator, discoverer, planner, architect, acceptance-designer, software-engineer, and their reviewers) write human-facing output in **French**: issue/PR comments, narrative summaries, finding descriptions, status messages, and logs.
- The following always remain **in English**: enum values and structured fields (`verdict: rejected`, `status: pass`, `severity: BLOCKER`), gate identifiers (`G14`), file paths, state label names (`state:distill-needed`), YAML/JSON keys, code identifiers (classes, methods, namespaces), and GitHub API terms.
- When in doubt: prose read by a human is in French; anything interpreted by a machine or that is a stable identifier stays in English.
- Business vocabulary follows the FR → EN table in `business-lexicon.instructions.md`.
