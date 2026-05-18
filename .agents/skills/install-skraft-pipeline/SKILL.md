---
name: install-skraft-pipeline
description: Install the full eleven-workflow skraft SDLC pipeline (discover → discuss → design → distill → deliver, each with an automated reviewer) into the current repo in one pass. Use when the user wants an end-to-end agent pipeline driven by a single issue label, or types /install-skraft-pipeline.
---

# install-skraft-pipeline

Install all eleven skraft-pipeline workflows into the current repo in one pass: fetch via
`gh aw add`, create the labels, validate, and summarize.

The result: the user dispatches a task by adding the single `sdlc` label to any issue.
The eleven agents (five specialist roles + five reviewers + one orchestrator) coordinate
across the issue thread via structured HTML-comment blocks and a label-based state machine.

## When to use this

- User says "install skraft", "set up the SDLC pipeline", "install the discover/discuss/design/distill/implement/review workflows", or similar.
- User types `/install-skraft-pipeline`.
- User has just heard about the pattern via the catalog README and asked to install it.

## When NOT to use this

- User wants a single workflow — hand off to `gh aw new` instead.
- User's repo has no tests AND they want PRs that check passing tests — warn them: the
  reviewer expects a runnable test command. Offer to install anyway with the caveat that
  the reviewer will note "no test infrastructure" on every review.

---

## Flow

### 1. Explain what's about to happen

One paragraph: eleven workflows will be added (five specialist roles, five automated
reviewers, one orchestrator), one auth secret will be set, thirteen labels will be
created, nothing runs until the user opens an issue and adds `sdlc`.  
Ask for **explicit confirmation** to proceed. The user must opt in — workflows run on push.

### 2. Preflight

Check **in parallel**:

- `gh` CLI authenticated (`gh auth status`)
- **`workflow` scope present on the gh token**
  (`gh auth status -t 2>&1 | grep -i 'token scopes'`).  
  Without it, the first `git push` of `*.lock.yml` will fail with *"refusing to allow an
  OAuth App to create or update workflow ... without `workflow` scope"*.  
  If missing → `gh auth refresh -s workflow -h github.com` (browser flow, ~30 sec).
- `gh aw` extension installed (`gh extension list | grep gh-aw`)
- Current dir is a git repo clean enough to commit (`git status --porcelain`)
- User has write access to `origin` (`gh repo view --json viewerPermission`)
- Repo Actions settings allow PR creation. Warn the user that  
  **Settings → Actions → General → "Allow GitHub Actions to create and approve pull requests"**  
  must be ON. The skill cannot flip this.

If any check fails, surface it plainly. Don't install tools on the user's behalf.

### 3. Set up auth once

Ask once: **"GitHub Copilot subscription, Claude Pro/Max subscription, or API key?"**

Check `gh secret list` first — if the secret already exists, reuse it. Do not re-prompt.

| Engine | Secret name | How to set |
|--------|-------------|------------|
| Copilot (default) | *(no extra secret needed — uses `GITHUB_TOKEN`)* | Nothing to do |
| Claude OAuth | `CLAUDE_CODE_OAUTH_TOKEN` | `claude setup-token` then `gh secret set CLAUDE_CODE_OAUTH_TOKEN` |
| Anthropic API key | `ANTHROPIC_API_KEY` | `gh secret set ANTHROPIC_API_KEY` |

Never echo or store the token. One secret covers all six workflows.  
If using Claude, update the `engine:` field in each `.md` file from `copilot` to `claude`
before running `gh aw add` (or edit after, then recompile).

### 4. Install all eleven workflows

Run **in sequence** (each `gh aw add` auto-compiles and adds a `source:` field for
future `gh aw update`):

```bash
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/backlog-discoverer.md@main
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/backlog-discoverer-reviewer.md@main
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/backlog-planner.md@main
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/backlog-planner-reviewer.md@main
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/solution-architect.md@main
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/solution-architect-reviewer.md@main
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/acceptance-designer.md@main
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/acceptance-designer-reviewer.md@main
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/software-engineer.md@main
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/software-engineer-reviewer.md@main
gh aw add SebastienDegodez/agentic-project-demo/catalog/skraft-pipeline/skraft-orchestrator.md@main
```

Installer l'orchestrateur en dernier (il dépend des 10 autres pour ses dispatches).

If any `gh aw add` fails, **stop and surface the exact error** — do not proceed with a
partial install. The eleven are a unit; a half-installed pipeline dead-ends on the first
reviewer handoff.

### 5. Create the labels

Create these labels (idempotent — `--force` ignores "already exists"):

```bash
# Pipeline trigger
gh label create sdlc                   --color 0052CC --description "Kicks off the skraft SDLC pipeline" --force

# State machine labels
gh label create "state:plan-needed"    --color E4E669 --description "skraft: ready for the planner" --force
gh label create "state:design-needed"  --color E4E669 --description "skraft: ready for the architect" --force
gh label create "state:distill-needed" --color E4E669 --description "skraft: ready for the acceptance designer" --force
gh label create "state:impl-needed"    --color FCD34D --description "skraft: ready for the implementer" --force
gh label create "state:review-needed"  --color FDBA74 --description "skraft: ready for the reviewer" --force
gh label create "state:done"           --color 86EFAC --description "skraft: task approved by reviewer" --force
gh label create "state:blocked"        --color F87171 --description "skraft: paused, human intervention required" --force

# story_type detection labels (backlog-discoverer reads these)
gh label create "type/feature"         --color 0075ca --description "Functional story — Gherkin produced" --force
gh label create "type/bug"             --color d73a4a --description "Functional story — Gherkin produced" --force
gh label create "type/tech-debt"       --color E4E669 --description "Technical story — impl-plan only" --force
gh label create "type/infra"           --color E4E669 --description "Technical story — impl-plan only" --force
gh label create "type/refactoring"     --color E4E669 --description "Technical story — impl-plan only" --force
```

### 6. Validate

```bash
gh aw validate
```

Runs against all lock files. Safe (no recompile). Surface any warnings.

### 7. Summarize

Show the user, in this order:

- Eleven files added under `.github/workflows/` (name each `.md` + `.lock.yml` pair)
- Secret configured (name only, never value) or reused / skipped for Copilot
- Thirteen labels created (or "N already existed, skipped")
- **How to dispatch a task**:
  > "Open an issue describing what you want built. Add the `sdlc` label. Done."
- Reminder: `gh aw compile` reverts any manual engine tweaks. Re-apply on every
  recompile. `gh aw validate` is safe.

Then **ask whether to commit and push**. Do not commit without explicit confirmation.

---

## Updated Installation Steps

To install all agents and skills for the skraft SDLC pipeline, follow these steps:

1. **Install Agents**:
   ```bash
   apm install sebastiendegodez/agentic-project-demo/.github/agents/acceptance-designer-reviewer.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/acceptance-designer.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/agentic-workflows.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/backlog-discoverer-reviewer.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/backlog-discoverer.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/backlog-planner-reviewer.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/backlog-planner.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/skraft-orchestrator.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/software-engineer-reviewer.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/software-engineer.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/solution-architect-reviewer.agent.md
   apm install sebastiendegodez/agentic-project-demo/.github/agents/solution-architect.agent.md
   ```

2. **Install Skills**:
   ```bash
   apm install sebastiendegodez/agentic-project-demo/.github/skills/acceptance-review-criteria/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/architecture-decisions/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/architecture-patterns/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/architecture-review-criteria/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/bdd-methodology/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/clean-architecture-testing/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/contract-testing/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/craft-discipline/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/discovery-review-criteria/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/github-search-protocol/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/issue-refinement/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/issue-triage/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/mutation-testing/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/outside-in-tdd/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/planning-review-criteria/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/playwright-evidence/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/red-synthesize-green/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/sprint-planning/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/test-design-mandates/
   apm install sebastiendegodez/agentic-project-demo/.github/skills/test-refactoring-catalog/
   ```

---

## Hard rules

- **All or nothing.** If any of the eleven `gh aw add` calls fails, stop and back out.
  A half-installed pipeline is worse than none — users will dispatch tasks that stall
  silently at the first missing reviewer.
- Never write workflow YAML by hand. Always delegate to `gh aw add`. The `.md` sources
  live in this repo's `catalog/skraft-pipeline/`.
- Never store or echo the auth token. Pipe through `gh secret set` stdin.
- Never commit or push without explicit user confirmation. Workflows run on push.
- Never install on top of an existing skraft setup without asking. If
  `.github/workflows/backlog-discoverer.lock.yml` already exists, ask before
  overwriting — the user may have customized it.

---

## User journey (for surfacing to the user)

After install, the entire per-task journey is:

1. User opens an issue describing a feature or bug.
2. User adds label **`sdlc`** (or runs `/sdlc` slash command via `skraft-orchestrator`).
3. `backlog-discoverer` posts a triage report → dispatches `backlog-discoverer-reviewer`.
4. `backlog-discoverer-reviewer` posts verdict → **approve**: `state:plan-needed` + dispatch `backlog-planner` / **reject**: re-dispatch `backlog-discoverer`.
5. `backlog-planner` posts user story + AC → dispatches `backlog-planner-reviewer`.
6. `backlog-planner-reviewer` posts verdict → **approve**: `state:design-needed` + dispatch `solution-architect` / **reject**: re-dispatch `backlog-planner`.
7. `solution-architect` posts event model + ADR + contracts → dispatches `solution-architect-reviewer`.
8. `solution-architect-reviewer` posts verdict → **approve**: `state:distill-needed` + dispatch `acceptance-designer` / **reject**: re-dispatch `solution-architect`.
9. `acceptance-designer` posts Gherkin + impl-plan → dispatches `acceptance-designer-reviewer`.
10. `acceptance-designer-reviewer` posts verdict → **approve**: `state:impl-needed` + dispatch `software-engineer` / **reject**: re-dispatch `acceptance-designer`.
11. `software-engineer` opens a draft PR (`Closes #N`) → `state:review-needed` + dispatch `software-engineer-reviewer`.
12. `software-engineer-reviewer` posts a structured verdict:
    - **approved** → `state:done` (user merges)
    - **kickback** → re-dispatch `software-engineer` with `iteration+1` (max 3 rounds)
    - **blocked** → `state:blocked` (human resolves)
13. User reviews the approved draft PR and merges. Agents never merge.

Escape hatches at any time: add `state:blocked` to halt, edit a comment to steer the
next agent, or manually `gh workflow run` a specific role to retry a stuck stage.

---

## Out of scope for v0.1

- Uninstalling the pipeline (remove the eleven `.md`/`.lock.yml` files + labels manually)
- Cross-repo install
- Customizing max iterations without editing the workflow source
- Turning individual roles on/off — the eleven are designed to work as a unit
- Updating installed workflows to a newer version (use `gh aw update` once supported)
