# Plan Execution Strategy

## Strategy

Build thin vertical slices from UI to API instead of completing isolated layers first. Every major slice should leave the app more demonstrable than before.

## Execution Principles

- Start with the judge-facing path.
- Keep scope narrow and visible.
- Prefer seeded data and deterministic fallbacks where external services could break the demo.
- Add abstractions only when they support the current MVP.
- Make localization and region adaptability structural from the beginning, not a final decoration.

## Slice Order

1. Solution scaffold and shared contracts.
2. Submit report flow.
3. Region and localization support.
4. Offline queue and sync.
5. SignalR realtime dashboard.
6. Validation workflow.
7. AI-assisted accountability pipeline.
8. Accountability brief view.
9. README, demo script, and Codex usage notes.

## Human Input

The human defines:

- The OSF-aligned problem framing.
- Track choice and ethical boundaries.
- MVP scope and tradeoffs.
- What counts as responsible use of AI.
- Final acceptance of generated code and demo narrative.

## Codex Contribution

Codex assists with:

- Translating documents into implementation tasks.
- Scaffolding the solution.
- Implementing vertical slices.
- Refactoring and debugging.
- Creating docs, demo script, and review notes.
- Checking consistency against the capstone brief.

## Risk Controls

- If PostgreSQL setup slows progress, use an in-memory or SQLite demo repository and keep the persistence boundary clear.
- If OpenAI API access is unavailable, use deterministic sample pipeline output and document the fallback.
- If map integration becomes costly, use a region-aware list/cards dashboard and leave map coordinates ready for future extension.
- If localization breadth becomes too large, ship English plus one polished second language and document how more languages plug in.

## Verification Loop

After each slice:

1. Build the solution.
2. Run targeted tests if present.
3. Manually exercise the feature.
4. Update docs if behavior changed.

## Final Packaging

The final repo should contain:

- Working source code.
- Clear README.
- `AGENTS.md`.
- `CAPSTONE_SPEC.md`.
- Demo script.
- Notes explaining Codex usage versus human decisions.
