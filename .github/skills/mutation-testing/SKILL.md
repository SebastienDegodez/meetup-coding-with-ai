---
name: mutation-testing
description: Use when running mutation tests with Stryker to identify surviving mutants in changed code
---

# Mutation Testing

## Install

```bash
command -v dotnet-stryker &>/dev/null || dotnet tool install --global dotnet-stryker
```

## CLI Pattern

```bash
dotnet stryker \
  --since:"<ref>" \
  --reporter json \
  --mutate "src/**/*.cs" \
  --mutate "!src/**/*Marker.cs" \
  --output-path "stryker-mutation-report"
```

## Extract & Filter Results

```bash
dotnet stryker [...] | jq '.. | objects | select((.status? // "") == "Survived")'
```

## Flags

| Flag | Purpose |
|---|---|
| `--since:<ref>` | Compare from git ref |
| `--reporter json` | JSON output |
| `--reporter html` | HTML output |
| `--mutate "..."` | Glob to mutate |
| `--output-path` | Report directory |
