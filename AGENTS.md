# AGENTS.md

## Project

Ubuntu Sentinel is a .NET 10 PeaceTech product for the Andela x OpenAI Codex Accelerator. It is positioned as community intelligence infrastructure for OSF-aligned transformative peace, not merely a reporting dashboard.

## Current Strategy

Build a prize-ready demo over the next 48 hours:

- BaseTemplate-inspired clean architecture.
- Multi-channel intake with USSD simulator and offline web/PWA queue.
- Seeded policy/RAG promise tracking.
- Four-agent Codex pipeline.
- Community validation gate.
- Dual conflict/resilience map.
- Role-shaped Reporter, Validator, Advocate, and OSF Partner views.
- Accountability brief with PDF-ready output.

## Non-Negotiables

- AI assists; community validators decide.
- Original community testimony must be preserved.
- Consent and safety checks must be visible.
- Conflict zones and resilience zones are both first-class.
- External integrations need deterministic demo fallbacks.
- EN/FR localization must cover the judge-facing flow.

## Phase 2 Only

Do not build during the demo sprint:

- Marten oral histories.
- Full women peacebuilder registry.
- Satellite imagery correlation.
- Production IAM.

## Engineering Rules

- Keep the solution buildable after each major slice.
- Prefer BaseTemplate structure and discipline.
- Move behavior into `US.Domain`, `US.Application`, `US.Persistence`, and `US.Infrastructure` instead of expanding API feature code.
- Web and Mobile must call API boundaries, not server internals.
- Use MudBlazor for app UI controls where practical.
- Keep docs in sync when strategy, architecture, or user-visible behavior changes.

## Verification

- Stop running API/Web before building if file locks occur.
- Run `dotnet build UbuntuSentinel.slnx` after structural changes.
- Browser-verify UI changes when the browser backend is available; otherwise inspect HTTP/logs and ask the user to refresh.
