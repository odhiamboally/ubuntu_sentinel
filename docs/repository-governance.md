# Repository Governance

## Current Sprint Rule

Ubuntu Sentinel is still in a time-boxed capstone sprint, so direct work on `main` is allowed when speed is necessary. Each meaningful slice should still be built, documented, and committed intentionally.

## Post-Demo Branch Protection Target

After the demo sprint, protect `main` with:

- pull request required before merge,
- at least one review approval,
- required build check,
- no unresolved review conversations,
- branch up to date before merge where practical.

## Pull Request Expectations

Every PR should explain:

- what changed,
- why it matters for the OSF/PeaceTech mission,
- how it was verified,
- how community safety and validation rules were preserved,
- what Codex implemented or assisted,
- what human decisions guided the work.

## Review Focus

Reviewers should check:

- clean architecture boundaries,
- localization behavior,
- USSD/offline behavior,
- report validation status rules,
- sensitive report handling,
- brief and pipeline traceability,
- deterministic fallback honesty when OpenAI is not configured.

## Why This Matters for the Showcase

The governance trail demonstrates that Codex was used as an engineering collaborator inside a controlled workflow. The project is not just "AI wrote code"; it shows human judgment, documented architecture, review habits, and auditability.
