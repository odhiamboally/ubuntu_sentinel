# Contributing

## Purpose

This project was built in a fast capstone sprint, but it still follows disciplined engineering habits. These rules document the expected workflow for future work and help explain how Codex was used responsibly.

## Branches

- Short-lived feature branches should use `codex/` when Codex is driving the work.
- Direct work on `main` is acceptable only during the time-boxed demo sprint.
- After the demo, protect `main` and require PR review.

## Commit Messages

Use concise, imperative commit messages:

```text
Add status-aware accountability briefs
Fix validation decision state rules
Document Codex collaboration workflow
```

## Pull Requests

Each PR should include:

- what changed,
- why it matters,
- how it was verified,
- screenshots for UI changes,
- notes about any deterministic fallback or external dependency.

## Review Rules

Review should check:

- architecture boundaries,
- community validation and safety rules,
- localization impact,
- fallback behavior,
- demo path stability,
- build result.

## Required Checks After Demo

Target branch protection should require:

- solution build,
- architecture tests when added,
- unit/integration tests when added,
- PR review approval,
- no unresolved review comments.

## Codex Usage in PRs

When Codex contributes meaningfully, mention:

- which tasks Codex implemented,
- what human decisions guided the work,
- what was reviewed manually,
- any known limitations or maturity backlog items.
