# AGENTS.md

## Project

Ubuntu Sentinel is a .NET/Blazor PeaceTech MVP for the Andela x OpenAI Codex Accelerator. The app converts raw community reports into structured accountability evidence for OSF-aligned peacebuilding workflows.

## Build Priorities

- Working demo beats architectural completeness.
- Keep the solution modular but not over-engineered.
- Preserve human-in-the-loop validation; AI assists, humans decide.
- Treat localization, low-bandwidth use, anonymity, and regional adaptability as first-class requirements.

## Stack

- .NET for API and Blazor frontend.
- SignalR for realtime sync/dashboard updates.
- Browser storage for offline report queueing.
- PostgreSQL preferred for persistent server data; SQLite/in-memory fallback is acceptable for demo speed if documented.
- OpenAI API for the AI-assisted pipeline, with deterministic fallback data for demo resilience.

## Style

- Prefer feature folders over scattered layers for MVP velocity.
- Use clear contracts in shared DTOs.
- Keep UI text localizable.
- Avoid speculative abstractions until there is real duplication.

## Testing

- Run targeted builds/tests after each meaningful slice.
- Verify the demo path manually in the browser.
- If live AI or database dependencies are unavailable, provide seeded/demo fallbacks and document the limitation.

## Review

- Report files changed, commands run, and remaining risks.
- Do not revert user changes.
- Keep changes scoped to the active task.
