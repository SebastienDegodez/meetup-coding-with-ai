---
description: "Use when porting agent updates from skraft-plugin (source of truth) to this meetup repo. Performs a body-only propagation that preserves this repo's frontmatter conventions (paths, entrypoint) and never overwrites them with skraft-flavored values."
name: "Propagate skraft-plugin agents"
argument-hint: "Optional: branch or commit ref in skraft-plugin to propagate from (default: feat/hve-compatibility)"
agent: "agent"
---

# Propagate skraft-plugin agents into this repo

Source of truth: `../skraft-plugin/plugins/agents/` and `../skraft-plugin/plugins/skills/` on the ref the user provides (default `feat/hve-compatibility`).
Target: `.github/agents/` and `.github/skills/` in this repo (`meetup-coding-with-ai`, branch `main`).

A naive `cp` is forbidden. Frontmatter and body diverge by design between the two repos. Only the **body** of agents is propagated; **frontmatter is preserved** from this repo.

## Invariants this repo owns (NEVER overwrite from skraft)

For every `.github/agents/*.agent.md` in this repo:

| Field / convention | Value here (meetup) | Value in skraft (do NOT import) |
|---|---|---|
| Plan / state I/O paths in body | `.skraft/sdlc/{phase}/...` | `.copilot-tracking/skraft-plans/{projectSlug}/...` |
| `instructions:` frontmatter block | absent | present, points to `plugins/instructions/...` |
| `phase:` field in frontmatter metadata | absent | present |
| Orchestrator entrypoint name | `/sdlc` | `/skraft` |
| `model_requirement` on `software-engineer` | present | absent |

If any of those slips through, the propagation is broken — revert and redo.

## Procedure

### 1. Locate sources

```bash
cd ../skraft-plugin
git fetch
git checkout ${1:-feat/hve-compatibility}
ls plugins/agents/*.agent.md
ls plugins/skills/
```

### 2. Identify candidates

For each file in `plugins/agents/*.agent.md` that exists in this repo's `.github/agents/`:
- Compute the **body diff** between the skraft version and this repo's version, ignoring frontmatter.
- Build a list `{agentName, hasBodyChanges: bool, summaryOfChanges}`.

For each directory in `plugins/skills/` that does NOT exist in `.github/skills/`:
- Mark as **new skill candidate** (the body of a skill is portable; check its frontmatter against this repo's conventions before copying).

Present the list to the user. Ask: "Propage tout ? Sélection ? Annule ?". Wait for confirmation.

### 3. Body-only propagation (per approved agent)

For each approved agent file:

1. Read this repo's current frontmatter (between the two `---` delimiters at the top). **Keep it intact.**
2. Read the skraft version's body (everything after the closing `---`).
3. Apply ONE of these strategies based on diff complexity:
   - **Trivial body delta** (typo, single paragraph): `replace_string_in_file` on the meetup file targeting just the changed paragraph.
   - **Structural body change** (new section, reorganized handoff): replace the entire body of the meetup file with skraft's body, but rewrite all path references using the table below before writing.
4. **Path rewrite table** (apply to skraft body before writing into meetup):

   | skraft body string | meetup body string |
   |---|---|
   | `.copilot-tracking/skraft-plans/{projectSlug}/discover/` | `.skraft/sdlc/discover/` |
   | `.copilot-tracking/skraft-plans/{projectSlug}/discuss/` | `.skraft/sdlc/discuss/` |
   | `.copilot-tracking/skraft-plans/{projectSlug}/design/` | `.skraft/sdlc/design/` |
   | `.copilot-tracking/skraft-plans/{projectSlug}/distill/` | `.skraft/sdlc/distill/` |
   | `.copilot-tracking/skraft-plans/{projectSlug}/deliver/` | `.skraft/sdlc/deliver/` |
   | `.copilot-tracking/skraft-plans/{projectSlug}/` | `.skraft/sdlc/` |
   | `/skraft` (when used as orchestrator entrypoint) | `/sdlc` |
   | references to `plugins/instructions/...` | drop the reference; this repo carries those rules elsewhere |

5. Do NOT touch the frontmatter. Do NOT add an `instructions:` block. Do NOT add a `phase:` field.

### 4. New skill propagation (per approved skill)

For each approved new skill directory:
1. Copy the directory tree to `.github/skills/<skill-name>/`.
2. Open every `SKILL.md` and verify its frontmatter has only the meetup-conformant fields (`name`, `description`, optional `applyTo`). Strip skraft-only fields if present.
3. Apply the path rewrite table to skill bodies as well.

### 5. Verify

After all writes:

```bash
# 1. No skraft-flavored paths leaked into bodies
grep -rn "\.copilot-tracking/skraft-plans" .github/agents .github/skills && echo "LEAK"

# 2. No `instructions:` block was added to any agent frontmatter
grep -l "^instructions:" .github/agents/*.agent.md

# 3. No `phase:` field added
grep -l "^phase:" .github/agents/*.agent.md

# 4. /skraft entrypoint did not survive
grep -rn "/skraft" .github/agents

# 5. Diff summary
git diff --stat .github/agents .github/skills
```

All four greps MUST return empty (only the diff stat should show changes). If any leak is found, **revert** the affected file with `git checkout HEAD -- <file>` and reapply the procedure for that file.

### 6. Hand off

Show the user:
- The list of agents whose bodies were updated (with one-line change summary each).
- The list of new skills installed.
- The verification grep output (must be clean).
- The `git diff --stat`.

**Do not commit.** Let the user choose the commit message and scope.

## Failure mode this prompt prevents

Recent regression: a naive `cp` from skraft into meetup overwrote 12 agent files plus added 2 skill directories carrying skraft frontmatter (`.copilot-tracking/skraft-plans/{projectSlug}/...`, `instructions:` block referencing nonexistent `plugins/instructions/...`, `/skraft` entrypoint). The user had to rollback with `git checkout HEAD -- .github/agents/ && rm -rf .github/skills/quality-gates-*`. This prompt is the body-only, frontmatter-preserving alternative.
