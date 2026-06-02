---
description: "Use when creating, modifying, or auditing skraft SDLC pipeline agents (.github/agents) or their gh-aw workflow adapters (.github/workflows). Defines the agent/workflow separation, source policy, file-handoff contract, and open remediation items."
applyTo: ".github/agents/**/*.agent.md, .github/workflows/*.md"
---

# Skraft Pipeline — Agent / Workflow Conventions

The skraft SDLC pipeline (`DISCOVER → DISCUSS → DESIGN → DISTILL → DELIVER`) has 11 agent/workflow
pairs plus the `skraft-orchestrator`. Each phase has an agent and an adversarial reviewer.

## Architecture principle (model B)

- The **agent** file (`.github/agents/<name>.agent.md`) is the **canonical source** of business
  behaviour, persona, and phase logic. It must stay reusable and harness-neutral (VS Code-first).
- The **workflow** file (`.github/workflows/<name>.md`) is a **thin gh-aw adapter**: it only wires
  `on:` triggers, gh-aw `tools:`, `safe-outputs:`, `dispatch-workflow:`, `imports:`, and `source:`.
- A workflow MUST NOT duplicate or introduce business behaviour that belongs in the agent
  (directory init, dispatch policy, slash-command business rules, ratification logic). When a behaviour
  exists in both, the agent body is authoritative and the workflow copy is removed.

## Source provenance policy

- Every workflow declares `source:` pointing at the upstream catalog with the `@main` ref:
  `source: SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/<name>.md@main`.
- No SHA-pinning, no missing `source:`. All 11 workflows must be uniform.

## File-handoff contract (`.skraft/sdlc/`)

Agents pass artefacts to each other through git directories. gh-aw repo-memory is NOT used
(hard 100 KB max-patch-size cap). Canonical paths per phase:

| Phase    | Directory                     | Key artefacts |
| -------- | ----------------------------- | ------------- |
| DISCOVER | `.skraft/sdlc/discover/`      | `triage-{date}.md`, `sprint-proposal.md` |
| DISCUSS  | `.skraft/sdlc/discuss/`       | `stories-{milestone}.md`, `ac-draft-{story}.md` |
| DESIGN   | `.skraft/sdlc/design/`        | `event-model-{story}.md`, `adr-{n}-{slug}.md`, `diagrams-{story}.md`, `contracts-{story}.md`, `context-map.md`, `consistency-matrix-{story}.md`, `blockers/decision-drift-*.md` |
| DISTILL  | `.skraft/sdlc/distill/`       | `{feature}.feature`, `test-plan-{story}.md`, `impl-plan-{story}.md` |
| DELIVER  | (production + test code, git) | — |

- Directory init (`.gitkeep`) is owned by the agent `backlog-discoverer` Phase 0 INITIALIZATION only.
  Workflows must not re-create the structure.
- Contracts are written ONCE by `solution-architect` as `.skraft/sdlc/design/contracts-{story}.md`.
  Any reader (orchestrator, acceptance-designer) MUST use that exact path.

## Response language

- Human-facing output (issue/PR comments, summaries, findings, logs) is in **French**.
- Structured fields stay in **English**: enum values (`verdict: rejected`), gate ids (`G14`),
  `state:*` labels, file paths, YAML/JSON keys, code identifiers, GitHub API terms.

## Remediation ownership

Fixes are applied in the **agentic workflow folder** (`.github/workflows/*.md`) — the gh-aw
adapters that get compiled to `.lock.yml` and actually run. The agent folder
(`.github/agents/*.agent.md`) stays **neutral and reusable**: it is NOT edited to carry
harness-specific wiring (concrete handoff paths, `tools:` capabilities, evidence locations).
Agents express intent and the phase contract; the workflow layer wires the concrete paths and tools.

## Resolved remediation items

1. **Path alignment — contracts (FIXED, agent side).** The divergent reference
   `.skraft/sdlc/distill/contracts/` lived only in `skraft-orchestrator.agent.md` (DELIVER loop),
   not in any workflow — so there was nothing to reconcile in the adapter. A handoff path is the
   agent's business contract (Model B: the agent body is authoritative for handoff paths), so the
   fix belongs in the agent, not the workflow. Corrected to the canonical
   `.skraft/sdlc/design/contracts-{story}.md`, matching the file-handoff table and every other reader.

## Deferred (not actioned now)

- **Evidence / Playwright.** The `.skraft/sdlc/evidence/` reference (Playwright reports) has no
  producer today. Left as-is on purpose: it will be wired later, together with the arrival of
  frontend capability. Do not remediate now.

## Non-issues (do not "fix")

- **Tool capability declarations.** `search/codebase` / `search/listDirectory` are VS Code agent
  tool identifiers, not gh-aw `tools:` entries. At runtime the gh-aw AWF sandbox enables `bash`/`edit`
  by default, which already provides codebase search and directory listing. Workflows that rely on
  default toolsets are therefore correct; declaring those literal names in a gh-aw `tools:` block
  would be invalid syntax. No change needed.
- The 4 reviewer lenses (`architecture-boundaries`, `cold-reader`, `quality-gates`, `test-integrity`)
  already exist as standalone agents under `.github/agents/reviewer-lenses/`. They are correctly
  extracted; an inline copy in a workflow, if any, is the adapter wiring, not a duplication to remove.
- Reviewers declaring `engine: { id: copilot, model: claude-haiku-4.5 }` while phase agents use
  `engine: copilot` is intentional (cheaper model for review) and coherent by role.
