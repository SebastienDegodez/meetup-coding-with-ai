---
description: "Enforces design documentation discipline: journal updates accompany every spec change, and spec + journal stay in sync."
applyTo: "docs/**/*.md"
---

# Design Documentation Discipline

Every design choice leaves a trace. The spec captures *what* was decided; the journal captures *why* and *how*. Both must stay in sync at all times.

## Before finalizing any spec change

- [ ] Identify the journal file for the current session (`docs/journal/YYYY-MM-DD-*-design-choices-log.md`)
- [ ] Add or update the journal entry for the change (see format below)
- [ ] Cross-reference the journal entry with the affected spec section (and vice versa)
- [ ] Classify the choice as **skill-guided** (followed a skill/pattern without deliberation) or **deliberated** (trade-offs were explicitly weighed)
- [ ] List any references consulted and link each one to the specific choice it informed

## Journal entry format

Each entry must include at minimum:

```markdown
### [Section or decision title]

- **Trigger**: [what prompted this change — new requirement, constraint, feedback, etc.]
- **Mode**: skill-guided | deliberated
- **Summary**: [one or two sentences describing what changed and why]
- **References**: [link or name] — [which choice it informed]
```

Omit **References** only when no external source was consulted.

## Anti-patterns to avoid

- **Spec changed without a journal update** — any modification to the spec file must have a corresponding journal entry in the same working session.
- **Choice marked as deliberated when it was skill-guided** — if a skill, pattern, or convention drove the decision without weighing alternatives, it must be marked `skill-guided`, not `deliberated`.
- **References listed without linking to a specific choice** — a reference block with no binding to a decision provides no traceability value; each reference must state which choice it supported.
- **Journal and spec diverge over time** — the journal must reflect the current state of the spec; stale entries that no longer match the spec are worse than no entries.
