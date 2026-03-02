---
description: "Use when modifying README.md. Ensures README.fr.md stays aligned with every change."
applyTo: "README.md"
---

# Multilingual README Alignment

Hard rule:
- When `README.md` is modified, `README.fr.md` **must** be updated in the same change to reflect the modification.
- The French file mirrors the structure, sections, and content of the English README — translated to French.
- Code blocks, commands, URLs, and technical identifiers (class names, project paths, tool names) stay in English.
- Mermaid diagram labels should be translated to French in `README.fr.md`.
- If a section is added, removed, or reordered in `README.md`, apply the same change in `README.fr.md`.

## Checklist

- Before finalizing any change to `README.md`, open `README.fr.md` and apply the equivalent update.
- Verify both files have the same heading structure.
- Do not leave `README.fr.md` out of sync.
