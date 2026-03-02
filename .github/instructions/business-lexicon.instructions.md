---
description: "Use when writing, translating, or reviewing business/domain language. Keep the FR->EN business vocabulary table always up to date."
applyTo: "**"
---

# Business Lexicon FR -> EN (Always Updated)

Hard rule:
- This table is a living source of truth and must be updated whenever a new business term appears.
- If a French business term is introduced, changed, or clarified in code, tests, docs, comments, commit messages, or PR descriptions, add or update its row in this file in the same change.
- Prefer stable domain terms over literal translation.
- English terminology standard is US.
- Scope is limited to the Auto Insurance domain for now.

## Correspondence Table

| Français | English | Notes |
| --- | --- | --- |
| éligibilité | eligibility | Main business capability |


## Update Checklist

- Before finalizing any task, scan changed code/docs for French business terms.
- Update the table with any new or adjusted term.
- Keep one canonical English term per French term unless a note justifies alternatives.
